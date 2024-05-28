using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20240223_Toolkits
{
    internal sealed class ToolkitExternalDataMigration : ISubMigration
    {
        private const string TableName = Tables.ToolkitExternal;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(ToolkitExternal.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(ToolkitExternal.ToolkitId).AsInt32().NotNullable().ForeignKey(Tables.Toolkits, Toolkit.Id)
                .WithColumn(ToolkitExternal.ExternalDataId).AsInt32().NotNullable().ForeignKey(Tables.ExternalData, ExternalData.Id)
                .WithColumn(ToolkitExternal.Quantity).AsDecimal(7, 2).Nullable()
                .WithColumn(ToolkitExternal.Created).AsDateTime().NotNullable()
                .WithColumn(ToolkitExternal.Updated).AsDateTime().NotNullable();
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
