using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.Employee)]
    public sealed record EmployeeDataModel
    {
        [Column(Employee.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Employee.Name, CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        [Column(Employee.Surname, CanBeNull = false)]
        public string Surname { get; set; } = string.Empty;

        [Column(Employee.Code, CanBeNull = false)]
        public string Code { get; set; } = string.Empty;

        [Column(Employee.Email, CanBeNull = false)]
        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        [Column(Employee.IsManager, CanBeNull = false)]
        public bool IsManager { get; set; }

        [Column(Employee.Created, CanBeNull = false)]
        public DateTime Created { get; set; }
    }
}