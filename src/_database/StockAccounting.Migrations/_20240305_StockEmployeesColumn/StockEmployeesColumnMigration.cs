using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20240305_StockEmployeesColumn
{
    internal sealed class StockEmployeesColumnMigration : ISubMigration
    {
        private const string TableName = Tables.StockEmployees;
        public void Up(Migration migration)
        {
            migration.Alter.Table(TableName)
                .AddColumn(StockEmployees.DocumentSerialNumber).AsString(20).Nullable()
                .AddColumn(StockEmployees.DocumentNumber).AsInt32().Nullable();
        }

        public void Down(Migration migration)
        {
            migration.Alter.Table(TableName)
                .AlterColumn(StockEmployees.DocumentSerialNumber);

            migration.Alter.Table(TableName)
                .AlterColumn(StockEmployees.DocumentNumber);
        }
    }
}
