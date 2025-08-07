using FluentMigrator;
using StockAccounting.Migrations._20240325_ToolkitsHistory;
using StockAccounting.Migrations.Interfaces;

namespace StockAccounting.Migrations.Migrations
{
    [TimestampedMigration(2024, 3, 25, 0, 0, "Toolkits history migration")]
    public sealed class ToolkitsHistoryMigration : CompositeMigration
    {
        public override ISubMigration[] GetMigrations() =>
            new ISubMigration[]
            {
                new EveningWorkerInsertMigration(),
            };
    }
}
