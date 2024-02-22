using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20230816_Initial
{
    internal sealed class StockEmployeesMigration : ISubMigration
    {
        private const string TableName = Tables.StockEmployees;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(StockEmployees.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(StockEmployees.StockDataId).AsInt32().NotNullable().ForeignKey(Tables.StockData, StockData.Id)
                .WithColumn(StockEmployees.EmployeeId).AsInt32().NotNullable().ForeignKey(Tables.Employee, Employee.Id)
                .WithColumn(StockEmployees.StockTypeId).AsInt32().NotNullable().ForeignKey(Tables.StockTypes, StockTypes.Id)
                .WithColumn(StockEmployees.Quantity).AsDecimal(7, 2).NotNullable()
                .WithColumn(StockEmployees.Created).AsDateTime().NotNullable();
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
