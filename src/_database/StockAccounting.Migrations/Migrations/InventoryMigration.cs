using FluentMigrator;
using StockAccounting.Migrations._20250210_InventoryMigration;
using StockAccounting.Migrations.Interfaces;

namespace StockAccounting.Migrations.Migrations
{
    [TimestampedMigration(2025, 2, 10, 0, 0, "New inventory data table migration")]
    public sealed class InventoryMigration : CompositeMigration
    {
        public override ISubMigration[] GetMigrations() =>
            new ISubMigration[]
            {
                new InventoryDataMigration(),
                new ScannedDataQuantityColumnMigration(),
            };
    }
}
