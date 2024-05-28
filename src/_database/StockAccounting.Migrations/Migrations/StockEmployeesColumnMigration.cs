using FluentMigrator;
using StockAccounting.Migrations.Interfaces;

namespace StockAccounting.Migrations.Migrations
{
    [TimestampedMigration(2024, 3, 05, 0, 0, "Stock employees columns migration")]
    public sealed class StockEmployeesColumnMigration : CompositeMigration
    {
        public override ISubMigration[] GetMigrations() =>
            new ISubMigration[]
            {
                new _20240305_StockEmployeesColumn.StockEmployeesColumnMigration(),
            };
    }
}
