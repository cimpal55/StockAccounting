namespace StockAccounting.Migrations.Interfaces
{
    public interface ICompositeMigration
    {
        ISubMigration[] GetMigrations();
    }
}
