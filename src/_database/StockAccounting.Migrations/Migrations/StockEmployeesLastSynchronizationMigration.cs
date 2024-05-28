using FluentMigrator;
using StockAccounting.Migrations.Interfaces;

namespace StockAccounting.Migrations.Migrations
{
    [TimestampedMigration(2024, 3, 15, 0, 0, "Stock employees last synchronization column migration")]
    public sealed class StockEmployeesLastSynchronizationMigration : CompositeMigration
    {
        public override ISubMigration[] GetMigrations() =>
            new ISubMigration[]
            {
                new _20240315_StockEmployeesLastSynchronization.StockEmployeesLastSynchronizationMigration(),
            };
    }
}
