using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Identity.Client;
using Serilog;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.EmailBot.Models;
using StockAccounting.EmailBot.Services.Interfaces;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;

namespace StockAccounting.EmailBot
{
    public class IdleClient : IDisposable
    {
        private readonly IEmployeeDataRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private readonly string _host, _username, _password;
        private readonly SecureSocketOptions _sslOptions;
        private readonly int _port;
        private List<IMessageSummary> _messages;
        private List<UniqueId> _previousMessages;
        private List<string> _commands;
        private CancellationTokenSource _cancel;
        private CancellationTokenSource _done;
        private FetchRequest _request;
        private bool _messagesArrived;
        private ImapClient _client;
        private int _startIndex;

        // OAuth token management
        private string _currentAccessToken;
        private DateTime _tokenExpiryTime;
        private readonly object _tokenLock = new object();
        private IConfidentialClientApplication _msalApp;

        // Error handling and reconnection
        private int _consecutiveErrors = 0;
        private const int MaxConsecutiveErrors = 5;
        private const int ReconnectDelaySeconds = 30;
        private const int MaxReconnectDelaySeconds = 300; // 5 minutes
        private DateTime _lastSuccessfulConnection = DateTime.Now;

        public IdleClient(string host, int port, SecureSocketOptions sslOptions, string username, string password, string commands,
            IEmployeeDataRepository employeeRepository,
            IEmailService emailService)
        {
            _client = new ImapClient(new ProtocolLogger(Console.OpenStandardError()));
            _request = new FetchRequest(MessageSummaryItems.Full | MessageSummaryItems.UniqueId);
            _messages = new List<IMessageSummary>();
            _previousMessages = new List<UniqueId>();
            _cancel = new CancellationTokenSource();
            _commands = new List<string>(commands.Split(new char[] { ';' }));

            _sslOptions = sslOptions;
            _username = username;
            _password = password;
            _host = host;
            _port = port;

            _employeeRepository = employeeRepository;
            _emailService = emailService;

            InitializeMsalApp();
        }

        private void InitializeMsalApp()
        {
            try
            {
                var oAuthCredentials = OAuth2Credentials.GetOAuth2Credentials();
                _msalApp = ConfidentialClientApplicationBuilder
                    .Create(oAuthCredentials.ClientId)
                    .WithClientSecret(oAuthCredentials.Secret)
                    .WithTenantId(oAuthCredentials.TenantId)
                    .Build();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize MSAL application");
                throw;
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            lock (_tokenLock)
            {
                // Check if current token is still valid (with 5-minute buffer)
                if (!string.IsNullOrEmpty(_currentAccessToken) &&
                    DateTime.UtcNow < _tokenExpiryTime.AddMinutes(-5))
                {
                    Log.Debug("Using cached access token (expires at {ExpiryTime})", _tokenExpiryTime);
                    return _currentAccessToken;
                }
            }

            try
            {
                Log.Information("Acquiring new OAuth2 token");
                var scopes = new string[] { "https://outlook.office365.com/.default" };
                var result = await _msalApp.AcquireTokenForClient(scopes).ExecuteAsync();

                lock (_tokenLock)
                {
                    _currentAccessToken = result.AccessToken;
                    _tokenExpiryTime = result.ExpiresOn.UtcDateTime;
                }

                Log.Information("Successfully acquired token, expires at {ExpiryTime}", _tokenExpiryTime);
                return result.AccessToken;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to acquire OAuth2 token");
                throw;
            }
        }

        private bool ShouldRefreshToken()
        {
            lock (_tokenLock)
            {
                // Refresh if token expires within 10 minutes or if we don't have a token
                return string.IsNullOrEmpty(_currentAccessToken) ||
                       DateTime.UtcNow > _tokenExpiryTime.AddMinutes(-10);
            }
        }

        public async Task RunAsync()
        {
            while (!_cancel.Token.IsCancellationRequested)
            {
                try
                {
                    Log.Information("{0} - Starting email bot", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                    await ReconnectAsync();

                    await DeleteAllInboxMessagesAsync();

                    // Reset error counter on successful connection
                    _consecutiveErrors = 0;
                    _lastSuccessfulConnection = DateTime.Now;

                    // Note: We capture client.Inbox here because cancelling IdleAsync() *may* require
                    // disconnecting the IMAP client connection, and, if it does, the `client.Inbox`
                    // property will no longer be accessible which means we won't be able to disconnect
                    // our event handlers.
                    var inbox = _client.Inbox;

                    // keep track of changes to the number of messages in the folder
                    inbox.CountChanged += OnCountChanged;
                    inbox.MessageExpunged += OnMessageExpunged;
                    inbox.MessageFlagsChanged += OnMessageFlagsChanged;

                    Log.Information("{0} - Starting idling.", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                    await IdleAsync();

                    // Clean up event handlers
                    inbox.MessageFlagsChanged -= OnMessageFlagsChanged;
                    inbox.MessageExpunged -= OnMessageExpunged;
                    inbox.CountChanged -= OnCountChanged;

                    await SafeDisconnectAsync();
                }
                catch (OperationCanceledException)
                {
                    Log.Information("Operation was cancelled, shutting down email bot");
                    break;
                }
                catch (Exception ex)
                {
                    _consecutiveErrors++;
                    Log.Error(ex, "Error in RunAsync (attempt {ConsecutiveErrors}/{MaxErrors}): {Message}",
                        _consecutiveErrors, MaxConsecutiveErrors, ex.Message);

                    if (_consecutiveErrors >= MaxConsecutiveErrors)
                    {
                        Log.Fatal("Max consecutive errors reached ({MaxErrors}), shutting down", MaxConsecutiveErrors);
                        break;
                    }

                    var delaySeconds = Math.Min(ReconnectDelaySeconds * Math.Pow(2, _consecutiveErrors - 1), MaxReconnectDelaySeconds);
                    Log.Information("Waiting {DelaySeconds} seconds before retrying...", delaySeconds);

                    await SafeDisconnectAsync();

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds), _cancel.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }

            await SafeDisconnectAsync();
            Log.Information("Email bot has stopped");
        }

        public void Exit()
        {
            _cancel.Cancel();
        }

        public void Dispose()
        {
            try
            {
                _client?.Dispose();
                _cancel?.Dispose();
                _done?.Dispose();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error during dispose");
            }
        }

        private async Task<EmployeeDataModel?> ReturnSenderByEmailIfExists(string email)
        {
            try
            {
                IEnumerable<EmployeeDataModel> employees = await _employeeRepository.GetEmployeesAsync();
                var employee = employees.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                return employee;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving employee data for email: {Email}", email);
                return null;
            }
        }

        private async Task ReconnectAsync()
        {
            Log.Debug("Attempting to reconnect to IMAP server...");

            await SafeDisconnectAsync();

            _client?.Dispose();
            _client = new ImapClient(new ProtocolLogger(Console.OpenStandardError()));

            try
            {
                if (!_client.IsConnected)
                {
                    await _client.ConnectAsync(_host, _port, _sslOptions, _cancel.Token);
                    Log.Information("Connected to IMAP server at {Host}:{Port}", _host, _port);
                }

                var accessToken = await GetAccessTokenAsync();
                var oauth2 = new SaslMechanismOAuth2(_username, accessToken);

                await _client.AuthenticateAsync(oauth2, _cancel.Token);

                await Task.Delay(1000, _cancel.Token);

                await _client.Inbox.OpenAsync(FolderAccess.ReadWrite, _cancel.Token);

                Log.Information("Authenticated and inbox opened.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to connect/authenticate: {Message}", ex.Message);
                await SafeDisconnectAsync();
                throw;
            }
        }

        private async Task SafeDisconnectAsync()
        {
            try
            {
                if (_client?.IsConnected == true)
                {
                    await _client.DisconnectAsync(true);
                    Log.Debug("Disconnected from IMAP server");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error during disconnect: {Message}", ex.Message);
            }
        }

        private async Task FetchMessageSummariesAsync(bool print)
        {
            IList<IMessageSummary> fetched;
            _startIndex = Math.Max(0, _client.Inbox.Count - 1);
            int retryCount = 0;
            const int maxRetries = 3;

            do
            {
                try
                {
                    fetched = await _client.Inbox.FetchAsync(_startIndex, -1, _request, _cancel.Token);
                    Log.Information("Fetched {Count} new message summaries", fetched.Count);
                    break;
                }
                catch (ImapProtocolException ex) when (ex.Message.Contains("AccessTokenExpired"))
                {
                    Log.Warning("Access token expired during fetch, reconnecting");
                    await ReconnectAsync();
                    continue;
                }
                catch (ImapProtocolException ex)
                {
                    retryCount++;
                    Log.Warning(ex, "IMAP protocol exception during fetch (attempt {Retry}/{MaxRetries})", retryCount, maxRetries);

                    if (retryCount >= maxRetries)
                        throw;

                    await ReconnectAsync();
                }
                catch (IOException ex)
                {
                    retryCount++;
                    Log.Warning(ex, "I/O exception during fetch (attempt {Retry}/{MaxRetries})", retryCount, maxRetries);

                    if (retryCount >= maxRetries)
                        throw;

                    await ReconnectAsync();
                }
            } while (true);

            await ProcessMessagesAsync(fetched, print);
        }

        private async Task ProcessMessagesAsync(IList<IMessageSummary> messages, bool print)
        {
            foreach (var message in messages)
            {
                if (_previousMessages.Contains(message.UniqueId))
                    continue;

                _previousMessages.Add(message.UniqueId);

                try
                {
                    var sender = message.Envelope.From.Mailboxes.Single().Address;
                    Log.Debug("Processing message UID={Uid} from {Sender}", message.UniqueId, sender);

                    var command = message.NormalizedSubject;
                    Log.Debug("{0} - Request arrived from sender '{1}' ", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), sender);

                    if (sender == "postmaster@daytongroup.lv")
                    {
                        Log.Debug("Skipping postmaster message");
                        continue;
                    }

                    if (print)
                    {
                        Console.WriteLine("{0}: New message '{1}' from '{2}'. ({3})",
                            _client.Inbox, message.Envelope.Subject, sender, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                    }

                    var verified = await ReturnSenderByEmailIfExists(sender);

                    if (_commands.Contains(command.Replace(" ", "").Trim()))
                    {
                        if (verified != null)
                        {
                            Log.Debug("{0} - Sender is correct. Checking for stocks", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                            await _emailService.SendEmailAsync(verified);
                            Log.Debug("{0} - Successfully sent a list with remaining stocks to '{1}'", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), sender);
                            Console.WriteLine("EMAIL NOTIFICATION: List of stocks sent to the email: '{0}' ({1})", sender, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                        }
                        else
                        {
                            _emailService.SendErrorMessageAsync(sender);
                            Log.Debug("{0} - Sender is incorrect. Employee with email '{1}' doesn't exists.", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), sender);
                            Console.WriteLine("EMAIL NOTIFICATION: Worker with email '{0}' wasn't found.", sender);
                        }
                    }
                    else
                    {
                        Log.Debug("Command '{0}' not recognized from sender '{1}'", command, sender);
                    }

                    await ArchiveMessageAsync(message.UniqueId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error processing message {Uid}", message.UniqueId);
                }
            }
        }

        private async Task ArchiveMessageAsync(UniqueId uid)
        {
            try
            {
                await _client.Inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                var archiveFolder = await _client.GetFolderAsync("Archive");
                await _client.Inbox.MoveToAsync(uid, archiveFolder);
                Log.Debug("Message {Uid} moved to archive", uid);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to archive message {Uid}", uid);
            }
        }

        private async Task WaitForNewMessagesAsync()
        {
            int retryCount = 0;
            const int maxRetries = 3;

            do
            {
                try
                {
                    if (ShouldRefreshToken())
                    {
                        Log.Information("Token expiring soon, proactively refreshing connection");
                        await ReconnectAsync();
                        break;
                    }

                    if (_client.Capabilities.HasFlag(ImapCapabilities.Idle))
                    {
                        // IDLE for 9 minutes to avoid token expiration issues
                        _done = new CancellationTokenSource(new TimeSpan(0, 9, 0));
                        try
                        {
                            Log.Debug("Starting IDLE operation");
                            await _client.IdleAsync(_done.Token, _cancel.Token);
                            Log.Debug("IDLE operation completed");
                        }
                        finally
                        {
                            _done.Dispose();
                            _done = null;
                        }
                    }
                    else
                    {
                        Log.Debug("IDLE not supported, using NOOP");
                        await Task.Delay(new TimeSpan(0, 1, 0), _cancel.Token);
                        await _client.NoOpAsync(_cancel.Token);
                    }
                    break;
                }
                catch (ImapProtocolException ex) when (ex.Message.Contains("AccessTokenExpired"))
                {
                    Log.Warning("Access token expired during wait, reconnecting");
                    await ReconnectAsync();
                    break;
                }
                catch (ImapProtocolException ex)
                {
                    retryCount++;
                    Log.Warning(ex, "IMAP protocol exception during wait (attempt {Retry}/{MaxRetries})", retryCount, maxRetries);

                    if (retryCount >= maxRetries)
                        throw;

                    await ReconnectAsync();
                }
                catch (IOException ex)
                {
                    retryCount++;
                    Log.Warning(ex, "I/O exception during wait (attempt {Retry}/{MaxRetries})", retryCount, maxRetries);

                    if (retryCount >= maxRetries)
                        throw;

                    await ReconnectAsync();
                }
            } while (true);
        }

        private async Task IdleAsync()
        {
            while (!_cancel.IsCancellationRequested)
            {
                try
                {
                    _startIndex = _client.Inbox.Count;

                    await WaitForNewMessagesAsync();

                    if (_messagesArrived)
                    {
                        Log.Information("{0} - New message arrived", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                        await FetchMessageSummariesAsync(true);
                        _messagesArrived = false;
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.Debug("Idle operation was cancelled");
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in IdleAsync: {Message}", ex.Message);
                    throw; // Let RunAsync handle the retry logic
                }
            }
        }

        private void OnCountChanged(object sender, EventArgs e)
        {
            var folder = (ImapFolder)sender;

            if (folder.Count > _messages.Count)
            {
                int arrived = folder.Count - _messages.Count;

                if (arrived > 1)
                {
                    Console.WriteLine("\t{0} new messages have arrived.", arrived);
                    Log.Information("{0} new messages have arrived", arrived);
                }
                else
                {
                    Console.WriteLine("\t1 new message has arrived.");
                    Log.Information("1 new message has arrived");
                }

                _messagesArrived = true;
                _done?.Cancel();
            }
        }

        private void OnMessageExpunged(object sender, MessageEventArgs e)
        {
            var folder = (ImapFolder)sender;

            if (e.Index < _messages.Count)
            {
                var message = _messages[e.Index];
                Console.WriteLine("{0}: message #{1} has been expunged: {2}", folder, e.Index, message.Envelope.Subject);
                Log.Debug("Message #{0} has been expunged: {1}", e.Index, message.Envelope.Subject);
                _messages.RemoveAt(e.Index);
            }
            else
            {
                Console.WriteLine("{0}: message #{1} has been expunged.", folder, e.Index);
                Log.Debug("Message #{0} has been expunged", e.Index);
            }
        }

        private static void OnMessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        {
            var folder = (ImapFolder)sender;
            Console.WriteLine("{0}: flags have changed for message #{1} ({2}).", folder, e.Index, e.Flags);
            Log.Debug("Flags changed for message #{0}: {1}", e.Index, e.Flags);
        }

        private async Task DeleteAllInboxMessagesAsync()
        {
            try
            {
                var inbox = _client.Inbox;

                if (!inbox.IsOpen)
                    await inbox.OpenAsync(FolderAccess.ReadWrite);

                var uids = await inbox.SearchAsync(SearchQuery.All);
                Log.Information("Deleting {Count} messages from inbox", uids.Count);

                if (uids.Count > 0)
                {
                    foreach (var uid in uids)
                    {
                        await inbox.AddFlagsAsync(uid, MessageFlags.Deleted, true);
                    }

                    await inbox.ExpungeAsync();
                    Log.Information("Inbox expunged successfully");
                }
                else
                {
                    Log.Information("No messages to delete from inbox");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to delete inbox messages: {Message}", ex.Message);
            }
        }
    }
}