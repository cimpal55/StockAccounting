using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20240325_ToolkitsHistory
{
    internal sealed class EveningWorkerInsertMigration : ISubMigration
    {
        private const string TableName = Tables.ToolkitHistory;
        private const string Toolkits = Tables.Toolkits;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(ToolkitHistory.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(ToolkitHistory.ToolkitId).AsInt32().NotNullable().ForeignKey(Toolkits, Toolkit.Id)
                .WithColumn(ToolkitHistory.EmployeeId).AsInt32().NotNullable().ForeignKey(Tables.Employee, Employee.Id)
                .WithColumn(ToolkitHistory.Created).AsDateTime().NotNullable();

            migration.Alter.Table(Toolkits)
                .AddColumn(Toolkit.IsDeleted).AsBoolean().NotNullable().WithDefaultValue(false);
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);

            migration.Alter.Table(Toolkits)
                .AlterColumn(Toolkit.IsDeleted);
        }
    }
}
