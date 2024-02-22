using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20230816_Initial
{
    internal sealed class ExternalDataMigration : ISubMigration
    {
        private const string TableName = Tables.ExternalData;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(ExternalData.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(ExternalData.ItemNumber).AsString(50).NotNullable()
                .WithColumn(ExternalData.Barcode).AsString(20).NotNullable()
                .WithColumn(ExternalData.PluCode).AsString(50).NotNullable()
                .WithColumn(ExternalData.Name).AsString(200).NotNullable()
                .WithColumn(ExternalData.Unit).AsString().Nullable()
                .WithColumn(ExternalData.Created).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn(ExternalData.Updated).AsDateTime().Nullable();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(ExternalData.Id).Ascending();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(ExternalData.PluCode).Ascending();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(ExternalData.ItemNumber).Ascending();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(ExternalData.Barcode).Ascending();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(ExternalData.Name).Ascending();
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
