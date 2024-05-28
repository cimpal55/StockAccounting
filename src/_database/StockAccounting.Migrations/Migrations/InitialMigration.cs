using FluentMigrator;
using StockAccounting.Migrations._20230816_Initial;
using StockAccounting.Migrations.Interfaces;

namespace StockAccounting.Migrations.Migrations
{
    [TimestampedMigration(2023, 8, 16, 0, 0, "Initial migration")]
    public sealed class InitialMigration : CompositeMigration
    {
        public override ISubMigration[] GetMigrations() =>
            new ISubMigration[]
            {
                new EmployeesMigration(),
                new UnitsMigration(),
                new ExternalDataMigration(),
                new InventoryDataMigration(),
                new ScannedDataMigration(),
                new StockTypesMigration(),
                new StockDataMigration(),
                new StockEmployeesMigration(),
            };
    }
}
