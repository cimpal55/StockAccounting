using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20240223_Toolkits
{
    internal sealed class ToolkitsMigration : ISubMigration
    {
        private const string TableName = Tables.Toolkits;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(Toolkit.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(Toolkit.Name).AsString(100).NotNullable()
                .WithColumn(Toolkit.Barcode).AsString(20).NotNullable()
                .WithColumn(Toolkit.Description).AsString(500).Nullable()
                .WithColumn(Toolkit.TotalQuantity).AsDecimal(7, 2).Nullable()
                .WithColumn(Toolkit.Created).AsDateTime().NotNullable()
                .WithColumn(Toolkit.Updated).AsDateTime().NotNullable();
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
