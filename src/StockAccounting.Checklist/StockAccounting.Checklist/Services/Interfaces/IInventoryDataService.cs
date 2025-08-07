using StockAccounting.Checklist.Models.Data;
using StockAccounting.Checklist.Models.Data.DataTransferObjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StockAccounting.Checklist.Services.Interfaces
{
    public interface IDocumentDataService
    {
        Task<ObservableCollection<DocumentDataModel>> GetDocumentDataAsync();
        Task<ScannedModel> InsertDocumentData(ScannedModel data);
    }
}
