using FluentMigrator;

namespace StockAccounting.Migrations.Interfaces
{
    public interface ISubMigration
    {
        void Up(Migration migration);

        void Down(Migration migration);
    }
}
