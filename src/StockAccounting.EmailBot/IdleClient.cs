using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using Serilog;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Repositories.Interfaces;
using StockAccounting.EmailBot.Services.Interfaces;
using System.Net.Http.Headers;

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
        private List<string> _commands;
        private CancellationTokenSource _cancel;
        private CancellationTokenSource _done;
        private FetchRequest _request;
        private bool _messagesArrived;
        private ImapClient _client;
        private int _startIndex;

        public IdleClient(string host, int port, SecureSocketOptions sslOptions, string username, string password, string commands,
            IEmployeeDataRepository employeeRepository,
            IEmailService emailService)
        {
            _client = new ImapClient(new ProtocolLogger(Console.OpenStandardError()));
            _request = new FetchRequest(MessageSummaryItems.Full | MessageSummaryItems.UniqueId);
            _messages = new List<IMessageSummary>();
            _cancel = new CancellationTokenSource();
            _commands = new List<string>(commands.Split(new char[] { ';' }));

            _sslOptions = sslOptions;
            _username = username;
            _password = password;
            _host = host;
            _port = port;

            _employeeRepository = employeeRepository;
            _emailService = emailService;
        }

        async Task<EmployeeDataModel?> ReturnSenderByEmailIfExists(string email)
        {
            IEnumerable<EmployeeDataModel> employees = await _employeeRepository.GetEmployeesAsync();

            if (employees.Any(x => x.Email == email))
            {
                var employee = employees.Where(x => x.Email == email).FirstOrDefault();
                return employee;
            }

            return null;
        }

        async Task ReconnectAsync()
        {
            if (!_client.IsConnected)
                await _client.ConnectAsync(_host, _port, _sslOptions, _cancel.Token);

            if (!_client.IsAuthenticated)
            {
                await _client.AuthenticateAsync(_username, _password, _cancel.Token);

                await _client.Inbox.OpenAsync(FolderAccess.ReadOnly, _cancel.Token);
            }
        }

        async Task FetchMessageSummariesAsync(bool print)
        {
            IList<IMessageSummary> fetched;
            _startIndex = _client.Inbox.Count;
            do
            {
                try
                {
                    // fetch summary information for messages that we don't already have
                    fetched = _client.Inbox.Fetch(_startIndex, -1, _request, _cancel.Token);
                    break;
                }
                catch (ImapProtocolException)
                {
                    // protocol exceptions often result in the client getting disconnected
                    await ReconnectAsync();
                }
                catch (IOException)
                {
                    // I/O exceptions always result in the client getting disconnected
                    await ReconnectAsync();
                }
            } while (true);

            foreach (var message in fetched)
            {
                var sender = message.Envelope.From.Mailboxes.Single().Address;
                var command = message.NormalizedSubject;
                Log.Debug("{0} - Request arrived from sender '{1}' ", DateTime.Now.ToString("dd.MM.yyyy HH:mm"), sender);

                if (print)
                    Console.WriteLine("{0}: New message '{1}' from '{2}'. ({3})", _client.Inbox, message.Envelope.Subject, sender, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

                var verified = await ReturnSenderByEmailIfExists(sender);

                if (_commands.Contains(command.Replace(" ", "")))
                {
                    if (verified != null)
                    {
                        Log.Debug("{0} - Sender is correct. Checking for stocks", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                        await _emailService.SendEmailAsync(verified);
                        Log.Debug("{0} - Successfully sent a list with remaining stocks to '{1}'", DateTime.Now.ToString("dd.MM.yyyy HH:mm"), sender);
                        Console.WriteLine("EMAIL NOTIFICATION: List of stocks sent to the email: '{0}' ({1})", sender, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    }
                    else
                    {
                        _emailService.SendErrorMessageAsync(sender);
                        Log.Debug("{0} - Sender is incorrect. Employee with email '{1}' doesn't exists.", DateTime.Now.ToString("dd.MM.yyyy HH:mm"), sender);
                        Console.WriteLine("EMAIL NOTIFICATION: Worker with email '{0}' wasn't found.", sender);
                    }
                }
                else
                {
                    _emailService.SendErrorMessageAsync(sender);
                    Log.Debug("{0} - Wrong command '{1}' was sent by '{2}'", DateTime.Now.ToString("dd.MM.yyyy HH:mm"), command, sender);
                    Console.WriteLine("EMAIL NOTIFICATION: Command '{0}' by sender '{1}' wasn't found. ({2})", command, sender, DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                }
            }
        }

        async Task WaitForNewMessagesAsync()
        {
            do
            {
                try
                {
                    if (_client.Capabilities.HasFlag(ImapCapabilities.Idle))
                    {
                        // Note: IMAP servers are only supposed to drop the connection after 30 minutes, so normally
                        // we'd IDLE for a max of, say, ~29 minutes... but GMail seems to drop idle connections after
                        // about 10 minutes, so we'll only idle for 9 minutes.
                        _done = new CancellationTokenSource(new TimeSpan(0, 9, 0));
                        try
                        {
                            await _client.IdleAsync(_done.Token, _cancel.Token);
                        }
                        finally
                        {
                            _done.Dispose();
                            _done = null;
                        }
                    }
                    else
                    {
                        // Note: we don't want to spam the IMAP server with NOOP commands, so lets wait a minute
                        // between each NOOP command.
                        await Task.Delay(new TimeSpan(0, 0, 20), _cancel.Token);
                        await _client.NoOpAsync(_cancel.Token);
                    }
                    break;
                }
                catch (ImapProtocolException)
                {
                    // protocol exceptions often result in the client getting disconnected
                    await ReconnectAsync();
                }
                catch (IOException)
                {
                    // I/O exceptions always result in the client getting disconnected
                    await ReconnectAsync();
                }
            } while (true);
        }

        async Task IdleAsync()
        {
            do
            {
                try
                {
                    await WaitForNewMessagesAsync();

                    if (_messagesArrived)
                    {
                        Log.Information("{0} - New message arrived", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                        await FetchMessageSummariesAsync(true);
                        _messagesArrived = false;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            } while (!_cancel.IsCancellationRequested);
        }

        public async Task RunAsync()
        {
            // connect to the IMAP server and get our initial list of messages
            try
            {
                await ReconnectAsync();
                Log.Information("{0} - Starting email bot", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            }
            catch (OperationCanceledException)
            {
                await _client.DisconnectAsync(true);
                return;
            }

            // Note: We capture client.Inbox here because cancelling IdleAsync() *may* require
            // disconnecting the IMAP client connection, and, if it does, the `client.Inbox`
            // property will no longer be accessible which means we won't be able to disconnect
            // our event handlers.
            var inbox = _client.Inbox;

            // keep track of changes to the number of messages in the folder (this is how we'll tell if new messages have arrived).
            inbox.CountChanged += OnCountChanged;
            // keep track of messages being expunged so that when the CountChanged event fires, we can tell if it's
            // because new messages have arrived vs messages being removed (or some combination of the two).
            inbox.MessageExpunged += OnMessageExpunged;

            // keep track of flag changes
            inbox.MessageFlagsChanged += OnMessageFlagsChanged;

            Log.Information("{0} - Starting idling.", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            await IdleAsync();

            inbox.MessageFlagsChanged -= OnMessageFlagsChanged;
            inbox.MessageExpunged -= OnMessageExpunged;
            inbox.CountChanged -= OnCountChanged;

            await _client.DisconnectAsync(true);
        }

        // Note: the CountChanged event will fire when new messages arrive in the folder and/or when messages are expunged.
        void OnCountChanged(object sender, EventArgs e)
        {
            var folder = (ImapFolder)sender;

            // Note: because we are keeping track of the MessageExpunged event and updating our
            // 'messages' list, we know that if we get a CountChanged event and folder.Count is
            // larger than messages.Count, then it means that new messages have arrived.
            if (folder.Count > _messages.Count)
            {
                int arrived = folder.Count - _messages.Count;

                if (arrived > 1)
                    Console.WriteLine("\t{0} new messages have arrived.", arrived);
                else
                    Console.WriteLine("\t1 new message has arrived.");

                // Note: your first instinct may be to fetch these new messages now, but you cannot do
                // that in this event handler (the ImapFolder is not re-entrant).
                //
                // Instead, cancel the `done` token and update our state so that we know new messages
                // have arrived. We'll fetch the summaries for these new messages later...
                _messagesArrived = true;
                _done?.Cancel();
            }
        }

        void OnMessageExpunged(object sender, MessageEventArgs e)
        {
            var folder = (ImapFolder)sender;

            if (e.Index < _messages.Count)
            {
                var message = _messages[e.Index];

                Console.WriteLine("{0}: message #{1} has been expunged: {2}", folder, e.Index, message.Envelope.Subject);

                // Note: If you are keeping a local cache of message information
                // (e.g. MessageSummary data) for the folder, then you'll need
                // to remove the message at e.Index.
                _messages.RemoveAt(e.Index);
            }
            else
            {
                Console.WriteLine("{0}: message #{1} has been expunged.", folder, e.Index);
            }
        }

        void OnMessageFlagsChanged(object sender, MessageFlagsChangedEventArgs e)
        {
            var folder = (ImapFolder)sender;

            Console.WriteLine("{0}: flags have changed for message #{1} ({2}).", folder, e.Index, e.Flags);
        }

        public void Exit()
        {
            _cancel.Cancel();
        }

        public void Dispose()
        {
            _client.Dispose();
            _cancel.Dispose();
        }
    }
}
