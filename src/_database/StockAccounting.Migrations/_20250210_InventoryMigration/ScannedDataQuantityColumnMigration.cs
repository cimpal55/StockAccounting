using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20250210_InventoryMigration
{
    internal sealed class ScannedDataQuantityColumnMigration : ISubMigration
    {
        private const string TableName = Tables.ScannedData;

        public void Up(Migration migration)
        {
            //migration.Rename.Column(ScannedData.InventoryDataId).OnTable(TableName).To(ScannedData.DocumentDataId);

            migration.Alter.Table(TableName)
                .AddColumn(ScannedInventoryData.InventoryDataId).AsInt32().Nullable().ForeignKey(Tables.InventoryData, InventoryData.Id)
                .AddColumn(ScannedInventoryData.FinalQuantity).AsInt32().Nullable();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(ScannedData.Id).Ascending();
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
