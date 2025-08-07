using LinqToDB;
using LinqToDB.Data;
using Serilog;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.InventoryData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.ScannedInventoryData;

namespace StockAccounting.Api.Repositories
{
    public class ScannedInventoryDataRepository : IScannedInventoryDataRepository
    {
        private readonly AppDataConnection _conn;

        public ScannedInventoryDataRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public async Task<List<ScannedInventoryDataModel>> GetScannedData() =>
            await _conn
                .ScannedInventoryData
                .ToListAsync();

        public async Task<List<ScannedInventoryDataModel>> GetLatestScannedData(IEnumerable<InventoryDataModel> data)
        {
            var inventoryData = data.Select(y => y.Id);
            return await _conn
                        .ScannedInventoryData
                            .Where(x => inventoryData.Contains(x.InventoryDataId))
                            .ToListAsync();
        }

        public async Task InsertScannedDataAsync(IEnumerable<ScannedInventoryDataModel> scannedData, int id)
        {
            try
            {
                await _conn.BeginTransactionAsync();

                var inventoryDataExists = await _conn.InventoryData
                    .AnyAsync(x => x.Id == id);

                if (!inventoryDataExists)
                {
                    throw new Exception($"InventoryDataId {id} does not exist in InventoryData table.");
                }

                foreach (var item in scannedData)
                {
                    if (string.IsNullOrEmpty(item.PluCode) && string.IsNullOrEmpty(item.Name) && string.IsNullOrEmpty(item.Unit))
                    {
                        var externalData = _conn.ExternalData.Where(x => x.Barcode == item.Barcode).FirstOrDefault();
                        if (externalData != null)
                        {
                            await _conn.ScannedInventoryData
                                  .InsertAsync(() => new ScannedInventoryDataModel
                                  {
                                      InventoryDataId = id,
                                      Name = externalData!.Name,
                                      Barcode = item.Barcode,
                                      PluCode = externalData.PluCode,
                                      Quantity = item.Quantity,
                                      IsExternal = item.IsExternal,
                                      CheckedQuantity = item.CheckedQuantity,
                                      FinalQuantity = item.FinalQuantity,
                                      Unit = externalData.Unit,
                                      ItemNumber = externalData.ItemNumber
                                  });
                        }
                    }
                    else
                    {
                        await _conn.ScannedInventoryData
                            .InsertAsync(() => new ScannedInventoryDataModel
                            {
                                InventoryDataId = id,
                                Name = item.Name,
                                Barcode = item.Barcode,
                                PluCode = item.PluCode,
                                Quantity = item.Quantity,
                                IsExternal = item.IsExternal,
                                CheckedQuantity = item.CheckedQuantity > 0 ? item.CheckedQuantity : 0,
                                FinalQuantity = item.FinalQuantity > 0 ? item.FinalQuantity : 0,
                                Unit = item.Unit,
                                ItemNumber = item.ItemNumber
                            });
                    }
                }

                await _conn.CommitTransactionAsync();

            }
            catch (Exception ex)
            {
                await _conn.RollbackTransactionAsync();
                Log.Information("API_InsertScannedDataAsync {message}", ex.Message);
                throw;
            }
        }

        public async Task UpdateScannedDataAsync(IEnumerable<ScannedInventoryDataModel> scannedData, int id)
        {
            try
            {
                await _conn.BeginTransactionAsync();

                foreach (var item in scannedData)
                {
                    await _conn.ScannedInventoryData
                        .Where(x => x.InventoryDataId == id && x.Id == item.Id)
                        .Set(x => x.CheckedQuantity, item.CheckedQuantity)
                        .Set(x => x.FinalQuantity, item.FinalQuantity)
                        .Set(x => x.Updated, DateTime.Now)
                        .UpdateAsync();
                }

                await _conn.CommitTransactionAsync();

            }
            catch (Exception ex)
            {
                await _conn.RollbackTransactionAsync();
                Log.Information("API_UpdateScannedDataAsync {message}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ScannedInventoryDataModel>> GetDetailsByEmployeeIdAsync(int id)
        {
            var sql = @$"SELECT ex.Name, ex.Barcode, ex.ItemNumber, 
	                            ex.PluCode, SUM(ste.Quantity) as Quantity, ex.Unit
	                         FROM TBL_Stock_Employees ste
	                         JOIN TBL_StockData st ON ste.StockDataID = st.ID
	                         JOIN TBL_ExternalData ex ON st.ExternalDataID = ex.ID
	                         JOIN TBL_CONF_Employees em ON ste.EmployeeID = em.ID
	                         WHERE em.ID = {id}
	                         GROUP BY ex.Name, ex.Barcode, ex.ItemNumber, ex.PluCode,
                                      em.Id, ex.Unit
	                         HAVING SUM(ste.Quantity) != 0";

            return await _conn.QueryToListAsync<ScannedInventoryDataModel>(sql)
                .ConfigureAwait(false);
        }

        public async Task<decimal> GetDetailQuantityByEmployeeIdAsync(int employeeId, int detailId)
        {
            var sql = @$"SELECT SUM(ste.Quantity) as Quantity
	                         FROM TBL_Stock_Employees ste
	                         JOIN TBL_StockData st ON ste.StockDataID = st.ID
	                         JOIN TBL_ExternalData ex ON st.ExternalDataID = ex.ID
	                         JOIN TBL_CONF_Employees em ON ste.EmployeeID = em.ID
	                         WHERE em.ID = {employeeId} AND ex.ID = {detailId}
	                         GROUP BY ex.Name, ex.Barcode, ex.ItemNumber, ex.PluCode,
                                      em.Id, ex.Unit
	                         HAVING SUM(ste.Quantity) != 0";

            return await _conn.ExecuteAsync<decimal>(sql)
                .ConfigureAwait(false);
        }
    }
}
