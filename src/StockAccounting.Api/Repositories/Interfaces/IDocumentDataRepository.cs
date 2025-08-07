using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using System.Collections.ObjectModel;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IDocumentDataRepository
    {
        Task<List<DocumentDataModel>> GetDocumentData();
        Task InsertDetailsAfterInventory(IEnumerable<ScannedInventoryDataRecord> details, int docId);
        Task<int> InsertDocumentWithIdentityAsync(DocumentDataModel document);
        Task InsertData(ScannedModel data);
        Task<int?> InsertDocumentDataAsync(ScannedModel data, List<int>? warehouseEmployees);
        Task MoveStocksToOtherEmployeeAsync(ScannedModel data);

    }
}
