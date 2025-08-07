using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20250210_InventoryMigration
{
    internal sealed class InventoryDataMigration : ISubMigration
    {
        private const string TableName = Tables.InventoryData;
        private const string SecondTable = Tables.DocumentData;

        public void Up(Migration migration)
        {
            //migration.Rename.Table(TableName).To(SecondTable);

            //migration.Alter.Table(SecondTable)
            //    .AddColumn(DocumentData.DocumentType).AsInt32().Nullable().ForeignKey(Tables.StockTypes, StockTypes.Id);

            migration.Create.Table(TableName)
                .WithColumn(InventoryData.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(InventoryData.Name).AsString(255).Nullable()
                .WithColumn(InventoryData.Employee1CheckerId).AsInt32().NotNullable().ForeignKey(Tables.Employee, Employee.Id)
                .WithColumn(InventoryData.Employee2CheckerId).AsInt32().NotNullable().ForeignKey(Tables.Employee, Employee.Id)
                .WithColumn(InventoryData.ScannedEmployeeId).AsInt32().NotNullable().ForeignKey(Tables.Employee, Employee.Id)
                .WithColumn(InventoryData.ManuallyAdded).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(InventoryData.Status).AsString(20).Nullable()
                .WithColumn(InventoryData.Created).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn(InventoryData.Updated).AsDateTime().Nullable()
                .WithColumn(InventoryData.Finished).AsDateTime().Nullable();
                
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
