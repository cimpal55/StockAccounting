using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;

namespace StockAccounting.Migrations._20240515_EveningWorker
{
    internal sealed class EveningWorkerMigration : ISubMigration
    {
        private const string TableName = Tables.Employee;

        public void Up(Migration migration)
        {
            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Evening", Surname = "Worker", Code = "EWO",
                    Email = "eveningworker@test.com", IsManager = true, Created = DateTime.Now });

        }

        public void Down(Migration migration)
        {
            migration.Delete.FromTable(TableName)
                .Row(new { Name = "Evening", Surname = "Worker", Code = "EWO",
                    Email = "eveningworker@test.com", IsManager = true, Created = DateTime.Now });
        }
    }
}
