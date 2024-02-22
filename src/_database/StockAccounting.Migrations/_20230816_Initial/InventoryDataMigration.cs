using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20230816_Initial
{
    internal sealed class InventoryDataMigration : ISubMigration
    {
        private const string TableName = Tables.InventoryData;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(InventoryData.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(InventoryData.Employee1Id).AsInt32().NotNullable().ForeignKey(Tables.Employee, Employee.Id)
                .WithColumn(InventoryData.Employee2Id).AsInt32().NotNullable().ForeignKey(Tables.Employee, Employee.Id)
                .WithColumn(InventoryData.ManuallyAdded).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(InventoryData.IsSynchronization).AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn(InventoryData.Created).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn(InventoryData.Updated).AsDateTime().Nullable();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(InventoryData.Id).Ascending();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(InventoryData.Employee1Id).Ascending();
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
