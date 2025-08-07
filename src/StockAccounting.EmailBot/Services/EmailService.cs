using System.Net.Mail;
using System.Net;
using StockAccounting.EmailBot.Models;
using StockAccounting.EmailBot.Services.Interfaces;
using System.Globalization;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.EmailBot.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmployeeDataRepository _employeeDataRepository;
        public EmailService(IEmployeeDataRepository employeeDataRepository)
        {
            _employeeDataRepository = employeeDataRepository;
        }

        public async Task<string> GetHtmlForEmailNotification(int employeeId)
        {
            int id = 1;

            string textBody = string.Empty;

            var stocksList = await _employeeDataRepository.GetEmployeeDetailsByIdAsync(employeeId);

            if (stocksList.Count() > 0) {

                textBody = @"<table border=" + 1 + " cellpadding=" + 5 + " cellspacing=" + 0 + " width = " + 900 + ">" +
                            "<tr bgcolor='#d3d3d3'> <td><b>ID</b></td> <td><b>Name</b></td> <td><b>ItemNumber</b></td>" +
                            "<td><b>Barcode</b></td> <td><b>PluCode</b></td> <td><b>Quantity</b></td></tr>";

                foreach (var item in stocksList)
                {
                    textBody += @"<tr><td>" + id + "</td><td> " + item.Name + "</td><td> " + item.ItemNumber + "</td>" +
                                 "<td> " + item.Barcode + "</td> <td> " + item.PluCode + "</td><td> " + item.Quantity + "</td></tr>";
                    id++;
                }
            }
            else
            {
                textBody = @"<table border=" + 1 + " cellpadding=" + 5 + " cellspacing=" + 0 + ">" +
                            "<tr bgcolor='#d3d3d3'> <td></td> </tr>" +
                            "<tr> <td>Employee doesn't have stocks at this moment.</td> </tr>";
            }

            textBody += "</table>";

            return textBody;
        }

        public async Task SendEmailAsync(EmployeeDataModel employee)
        {
            var smtpSettings = SMTPSettings.GetSMTPSettings();
            var textBody = await GetHtmlForEmailNotification(employee.Id);

            var port = smtpSettings.Port;
            var host = smtpSettings.Host;
            var emailFrom = smtpSettings.Email;
            var password = smtpSettings.Password;
            var enableSsl = smtpSettings.EnableSsl;
            var isBodyHtml = smtpSettings.IsBodyHtml;

            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(emailFrom, password),
                EnableSsl = enableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailFrom ?? ""),
                Subject = $"Report on stocks left {DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)}",
                Body = textBody,
                IsBodyHtml = isBodyHtml,
            };

            mailMessage.To.Add(employee.Email);

            smtpClient.Send(mailMessage);
        }

        public void SendErrorMessageAsync(string email)
        {
            var smtpSettings = SMTPSettings.GetSMTPSettings();

            var port = smtpSettings.Port;
            var host = smtpSettings.Host;
            var emailFrom = smtpSettings.Email;
            var password = smtpSettings.Password;
            var enableSsl = smtpSettings.EnableSsl;

            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(emailFrom, password),
                EnableSsl = enableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailFrom ?? ""),
                Subject = $"Error",
                Body = "Error when receiving left stock. The command was misspelled or sender email doesn't exist in database.\n" +
                       "\nОшибка при получении остатков на складе. Неправильно указана команда или в базе данных не существует электронной почты отправителя.\n" +
                       "\nKļūda, saņemot atlikušo krājumu. Komanda ir nepareizi uzrakstīta vai datu bāzē nav sūtītāja e-pasta."
            };

            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
        }
    }
}
