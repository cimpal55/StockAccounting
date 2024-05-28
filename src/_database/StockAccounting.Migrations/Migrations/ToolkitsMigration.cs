using FluentMigrator;
using StockAccounting.Migrations._20240223_Toolkits;
using StockAccounting.Migrations.Interfaces;

namespace StockAccounting.Migrations.Migrations
{
    [TimestampedMigration(2024, 2, 23, 0, 0, "Toolkits migration")]
    public sealed class ToolkitMigration : CompositeMigration
    {
        public override ISubMigration[] GetMigrations() =>
            new ISubMigration[]
            {
                new ToolkitsMigration(),
                new ToolkitExternalDataMigration(),
            };
    }
}
