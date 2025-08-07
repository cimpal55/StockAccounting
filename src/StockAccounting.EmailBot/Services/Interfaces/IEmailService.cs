using StockAccounting.Core.Data.Models.Data.EmployeeData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAccounting.EmailBot.Services.Interfaces
{
    public interface IEmailService
    {
        Task<string> GetHtmlForEmailNotification(int employeeId);

        Task SendEmailAsync(EmployeeDataModel employee);

        void SendErrorMessageAsync(string email);
    }
}
