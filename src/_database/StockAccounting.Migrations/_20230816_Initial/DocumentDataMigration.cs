using FluentMigrator;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20230816_Initial
{
    internal sealed class DocumentDataMigration : ISubMigration
    {
        private const string TableName = Tables.DocumentData;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(DocumentData.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(DocumentData.Employee1Id).AsInt32().NotNullable().ForeignKey(Tables.Employee, Employee.Id)
                .WithColumn(DocumentData.Employee2Id).AsInt32().NotNullable().ForeignKey(Tables.Employee, Employee.Id)
                .WithColumn(DocumentData.ManuallyAdded).AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn(DocumentData.IsSynchronization).AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn(DocumentData.Created).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn(DocumentData.Updated).AsDateTime().Nullable();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(DocumentData.Id).Ascending();

            migration.Create.Index()
                .OnTable(TableName)
                .OnColumn(DocumentData.Employee1Id).Ascending();
        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}
