using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20230816_Initial
{
    internal sealed class StockTypesMigration : ISubMigration
    {
        private const string TableName = Tables.StockTypes;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(StockTypes.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(StockTypes.Name).AsString(50).NotNullable()
                .WithColumn(StockTypes.Created).AsDateTime().NotNullable();

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Taken", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Returned", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Used", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Moved", Created = DateTime.Now });
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
