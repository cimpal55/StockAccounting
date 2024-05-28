using FluentMigrator;
using StockAccounting.Migrations.Interfaces;
using StockAccounting.Migrations.Utils.Extensions;

namespace StockAccounting.Migrations.Migrations
{
    public abstract class CompositeMigration : Migration, ICompositeMigration
    {
        public sealed override void Up() =>
            this.RunUp(this);

        public sealed override void Down() =>
            this.RunDown(this);

        public abstract ISubMigration[] GetMigrations();
    }
}