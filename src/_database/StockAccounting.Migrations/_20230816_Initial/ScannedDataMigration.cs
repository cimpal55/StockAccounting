using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20230816_Initial
{
    internal sealed class ScannedDataMigration : ISubMigration
    {
        private const string TableName = Tables.ScannedData;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(ScannedData.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(ScannedData.DocumentSerialNumber).AsString(20).NotNullable()
                .WithColumn(ScannedData.DocumentNumber).AsInt32().NotNullable()
                .WithColumn(ScannedData.DocumentDataId).AsInt32().NotNullable().ForeignKey(Tables.DocumentData, DocumentData.Id)
                .WithColumn(ScannedData.ExternalDataId).AsInt32().NotNullable().ForeignKey(Tables.ExternalData, ExternalData.Id)
                .WithColumn(ScannedData.Quantity).AsDecimal(7, 2).NotNullable()
                .WithColumn(ScannedData.Created).AsDateTime().NotNullable();
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
