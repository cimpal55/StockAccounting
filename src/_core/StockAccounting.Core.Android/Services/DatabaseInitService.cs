using StockAccounting.Core.Android.Data;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Android.Models.DataTransferObjects;
using StockAccounting.Core.Android.Services.Interfaces;
using System.Data.Common;

namespace StockAccounting.Core.Android.Services
{
    public class DatabaseInitService : DatabaseContext, IDatabaseInitService
    {
        public void InitSchema()
        {
            using var connection = OpenConnection();
            connection.CreateTable<ScannedInventoryDataRecord>();
            connection.CreateTable<InventoryDataRecord>();
            connection.CreateTable<ExternalDataRecord>();
            connection.CreateTable<EmployeeRecord>();
            connection.CreateTable<Employee2InventoryDataRecord>();
            connection.CreateTable<ServerSyncDateTimeDataRecord>();

            connection.Execute("CREATE INDEX IF NOT EXISTS idx_external_id ON tblInventoryData (ExternalId);");
            connection.Execute("CREATE INDEX IF NOT EXISTS idx_inventory_data_id ON tblScannedData (InventoryDataId);");
        }

    }
}
