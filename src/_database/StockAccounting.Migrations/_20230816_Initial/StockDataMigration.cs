using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20230816_Initial
{
    internal sealed class StockDataMigration : ISubMigration
    {
        private const string TableName = Tables.StockData;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(StockData.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(StockData.ExternalDataId).AsInt32().NotNullable().ForeignKey(Tables.ExternalData, ExternalData.Id)
                .WithColumn(StockData.Quantity).AsDecimal(7, 2).NotNullable()
                .WithColumn(StockData.LastSynchronization).AsDateTime().NotNullable()
                .WithColumn(StockData.Created).AsDateTime().NotNullable();
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
