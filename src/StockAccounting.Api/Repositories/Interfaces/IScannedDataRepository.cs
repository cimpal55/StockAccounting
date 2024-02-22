using Microsoft.AspNetCore.Mvc;
using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Api.Repositories.Interfaces
{
    public interface IScannedDataRepository
    {
        Task<List<ScannedDataBaseModel>> GetScannedData();
        Task<int> InsertScannedData(ScannedDataBaseModel scannedData);
        Task<IEnumerable<ScannedDataBaseModel>> GetScannedDataByIsSynchronizationAsync(int id, int externalDataId);

    }
}
