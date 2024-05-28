using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20240315_StockEmployeesLastSynchronization
{
    internal sealed class StockEmployeesLastSynchronizationMigration : ISubMigration
    {
        private const string TableName = Tables.StockEmployees;
        public void Up(Migration migration)
        {
            migration.Alter.Table(TableName)
                .AddColumn(StockEmployees.LastSynchronization).AsDateTime().Nullable();
        }

        public void Down(Migration migration)
        {
            migration.Alter.Table(TableName)
                .AlterColumn(StockEmployees.LastSynchronization);
        }
    }
}
