using FluentMigrator;

using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using StockAccounting.Migrations.Utils.Extensions;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20230816_Initial
{
    internal sealed class EmployeesMigration : ISubMigration
    {
        private const string TableName = Tables.Employee;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(Employee.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(Employee.Name).AsString(50).NotNullable()
                .WithColumn(Employee.Surname).AsString(50).NotNullable()
                .WithColumn(Employee.Code).AsString(50).NotNullable()
                .WithColumn(Employee.Email).AsString(50).NotNullable()
                .WithColumn(Employee.IsManager).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(Employee.Created).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        }
        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }

    }
}
