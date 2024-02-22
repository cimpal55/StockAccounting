using Acr.UserDialogs;
using StockAccounting.Checklist.Services.Interfaces;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowDialog(string message, string title, string buttonLabel)
        {
            return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public void ShowToast(string message)
        {
            UserDialogs.Instance.Toast(message);
        }
    }
}
