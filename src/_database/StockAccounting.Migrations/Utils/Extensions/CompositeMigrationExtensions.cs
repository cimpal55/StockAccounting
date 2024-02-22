using FluentMigrator;
using StockAccounting.Migrations.Interfaces;

namespace StockAccounting.Migrations.Utils.Extensions
{
    public static class CompositeMigrationExtensions
    {
        public static void RunUp(this ICompositeMigration @this, Migration migration)
        {
            foreach (var subMigration in @this.GetMigrations())
                subMigration.Up(migration);
        }

        public static void RunDown(this ICompositeMigration @this, Migration migration)
        {
            var migrations = @this.GetMigrations();

            for (var i = migrations.Length - 1; i >= 0; i--)
                migrations[i].Down(migration);
        }
    }
}
