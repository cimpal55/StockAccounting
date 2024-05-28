using StockAccounting.Checklist.Models.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services.Interfaces
{
    public interface IExternalDataService
    {
        Task<ObservableCollection<ExternalDataModel>> GetExternalDataAsync();
        Task<ExternalDataModel> GetExternalDataByBarcodeAsync(string barcode);
        Task<ExternalDataModel> GetExternalDataByIdAsync(int externalDataId);
    }
}
