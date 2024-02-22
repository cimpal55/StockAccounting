using Microsoft.Extensions.Configuration;
using StockAccounting.Core.Data.Services.Interfaces;
using System.Net.Mail;
using System.Net;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Repositories.Interfaces;

namespace StockAccounting.Core.Data.Services
{
    public class SmtpEmailService : ISmtpEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IExternalDataRepository _externalDataRepository;
        public SmtpEmailService(IConfiguration configuration, IExternalDataRepository externalDataRepository)
        {
            _configuration = configuration;
            _externalDataRepository = externalDataRepository;
        }

        public List<ScannedDataModel> GetFinishedListForHtml(List<ScannedDataBaseModel> stocksList)
        {
            List<ScannedDataModel> stocks = new();

            var tzi = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            foreach (var item in stocksList)
            {
                var record = _externalDataRepository.GetExternalDataById(item.ExternalDataId);
                var date = @TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(item.Created, DateTimeKind.Unspecified), tzi);

                ScannedDataModel stock = new()
                {
                    Name = record.Name,
                    ItemNumber = record.ItemNumber,
                    PluCode = record.PluCode,
                    Quantity = item.Quantity,
                    Created = date
                };
                stocks.Add(stock);
            }

            return stocks;
        }
        public string GetHtmlForEmailNotification(List<ScannedDataModel> stocksList)
        {
            int id = 1;

            string textBody = @"<table border=" + 1 + " cellpadding=" + 5 + " cellspacing=" + 0 + " width = " + 900 + ">" +
                               "<tr bgcolor='#d3d3d3'> <td><b>ID</b></td> <td><b>Name</b></td> <td><b>ItemNumber</b></td>" +
                               "<td><b>PluCode</b></td> <td><b>Quantity</b></td> <td><b>Date</b></td> </tr>";

            foreach (var item in stocksList)
            {
                textBody += @"<tr><td>" + id + "</td><td> " + item.Name + "</td><td> " + item.ItemNumber + "</td>" +
                             "<td> " + item.PluCode + "</td><td> " + item.Quantity + "</td><td> " + item.Created + "</td></tr>";
                id++;
            }
            textBody += "</table>";

            return textBody;
        }
        public void SendEmail(string emailTo, List<ScannedDataBaseModel> stocksList)
        {
            if (stocksList.Count == 0)
            {
                return;
            }

            var stocks = GetFinishedListForHtml(stocksList);
            var textBody = GetHtmlForEmailNotification(stocks);

            var port = Convert.ToInt32(_configuration["EmailParameters:Port"]);
            var host = _configuration["EmailParameters:Host"];
            var emailFrom = _configuration["EmailParameters:Email"];
            var password = _configuration["EmailParameters:Password"];
            var enableSsl = Convert.ToBoolean(_configuration["EmailParameters:EnableSsl"]);
            var isBodyHtml = Convert.ToBoolean(_configuration["EmailParameters:IsBodyHtml"]);

            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(emailFrom, password),
                EnableSsl = enableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailFrom ?? ""),
                Subject = $"Report on parts taken {stocksList.Select(x => x.Created.ToShortDateString()).FirstOrDefault()}",
                Body = textBody,
                IsBodyHtml = isBodyHtml,
            };

            mailMessage.To.Add(emailTo);

            smtpClient.Send(mailMessage);
        }
    }
}
