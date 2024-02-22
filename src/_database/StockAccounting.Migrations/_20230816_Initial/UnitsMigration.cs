using FluentMigrator;
using StockAccounting.Core.Data.Models;
using StockAccounting.Core.Data.Resources;
using StockAccounting.Migrations.Interfaces;
using StockAccounting.Migrations.Utils.Extensions;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Migrations._20230816_Initial
{
    internal sealed class UnitsMigration : ISubMigration
    {
        private const string TableName = Tables.Unit;

        public void Up(Migration migration)
        {
            migration.Create.Table(TableName)
                .WithColumn(Unit.Id).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(Unit.Name).AsString(20).NotNullable()
                .WithColumn(Unit.Created).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Litrs", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Kanna", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Package", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Kaste", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "- None -", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new{ Name = "Time", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Meter", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Kilometer", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Square meter", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Iepakojums", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Gabals", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Liter", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Piece", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Roll", Created = DateTime.Now });

            migration.Insert.IntoTable(TableName)
                .Row(new { Name = "Sett", Created = DateTime.Now });

        }

        public void Down(Migration migration)
        {
            migration.Delete.Table(TableName);
        }
    }
}