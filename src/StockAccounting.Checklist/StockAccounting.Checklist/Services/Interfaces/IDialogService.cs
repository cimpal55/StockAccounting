using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services.Interfaces
{
    public interface IDialogService
    {
        Task ShowDialog(string message, string title, string buttonLabel);

        void ShowToast(string message);
    }
}
