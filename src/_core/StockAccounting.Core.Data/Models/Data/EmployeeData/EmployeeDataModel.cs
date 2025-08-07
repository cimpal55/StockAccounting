using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.EmployeeData
{
    [Table(Tables.Employee)]
    public sealed record EmployeeDataModel
    {
        [Column(Columns.Employee.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.Employee.Name, CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        [Column(Columns.Employee.Surname, CanBeNull = false)]
        public string Surname { get; set; } = string.Empty;

        [Column(Columns.Employee.Code, CanBeNull = false)]
        public string Code { get; set; } = string.Empty;

        [Column(Columns.Employee.Email, CanBeNull = false)]
        public string Email { get; set; } = string.Empty;

        [Column(Columns.Employee.IsManager, CanBeNull = false)]
        public bool IsManager { get; set; }

        [Column(Columns.Employee.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        public string FullName => $"{Name} {Surname} {Code}";
    }
}