using LinqToDB;
using LinqToDB.Data;
using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.Data.EmployeeData;
using StockAccounting.Core.Data.Models.Data.ExternalData;
using StockAccounting.Core.Data.Models.Data.InventoryData;
using StockAccounting.Core.Data.Models.Data.ScannedData;
using StockAccounting.Core.Data.Models.Data.ScannedInventoryData;
using StockAccounting.Core.Data.Models.Data.StockData;
using StockAccounting.Core.Data.Models.Data.StockEmployees;
using StockAccounting.Core.Data.Models.Data.Toolkit;
using StockAccounting.Core.Data.Models.Data.ToolkitExternal;
using StockAccounting.Core.Data.Models.Data.Unit;

namespace StockAccounting.Core.Data.DbAccess
{
    public class AppDataConnection : DataConnection
    {
        public AppDataConnection(DataOptions<AppDataConnection> options)
            : base(options.Options)
        { }
        public ITable<ExternalDataModel> ExternalData => this.GetTable<ExternalDataModel>();
        public ITable<DocumentDataModel> DocumentData => this.GetTable<DocumentDataModel>();
        public ITable<InventoryDataModel> InventoryData => this.GetTable<InventoryDataModel>();
        public ITable<EmployeeDataModel> Employees => this.GetTable<EmployeeDataModel>();
        public ITable<ScannedDataBaseModel> ScannedData => this.GetTable<ScannedDataBaseModel>();
        public ITable<ScannedInventoryDataModel> ScannedInventoryData => this.GetTable<ScannedInventoryDataModel>();
        public ITable<StockDataBaseModel> StockData => this.GetTable<StockDataBaseModel>();
        public ITable<StockEmployeesBaseModel> StockEmployees => this.GetTable<StockEmployeesBaseModel>();
        public ITable<ToolkitModel> Toolkits => this.GetTable<ToolkitModel>();
        public ITable<ToolkitExternalModel> ToolkitExternal => this.GetTable<ToolkitExternalModel>();
        public ITable<UnitModel> Units => this.GetTable<UnitModel>();

    }
}
