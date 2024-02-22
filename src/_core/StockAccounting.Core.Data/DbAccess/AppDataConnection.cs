using LinqToDB;
using LinqToDB.Data;
using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Core.Data.DbAccess
{
    public class AppDataConnection : DataConnection
    {
        public AppDataConnection(DataOptions<AppDataConnection> options)
            : base(options.Options)
        { }
        public ITable<ExternalDataModel> ExternalData => this.GetTable<ExternalDataModel>();
        public ITable<InventoryDataModel> InventoryData => this.GetTable<InventoryDataModel>();
        public ITable<EmployeeDataModel> Employees => this.GetTable<EmployeeDataModel>();
        public ITable<ScannedDataBaseModel> ScannedData => this.GetTable<ScannedDataBaseModel>();
        public ITable<StockDataBaseModel> StockData => this.GetTable<StockDataBaseModel>();
        public ITable<StockEmployeesBaseModel> StockEmployees => this.GetTable<StockEmployeesBaseModel>();

    }
}
