using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.StockEmployees)]
    public record StockEmployeesBaseModel
    {
        [Column(StockEmployees.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(StockEmployees.StockDataId, CanBeNull = false)]
        public int StockDataId { get; set; }

        [Column(StockEmployees.EmployeeId, CanBeNull = false)]
        public int EmployeeId { get; set; }

        [Column(StockEmployees.StockTypeId, CanBeNull = false)]
        public int StockTypeId { get; set; }

        [Column(StockEmployees.Quantity, CanBeNull = false)]
        public decimal Quantity { get; set; }

        [Column(StockEmployees.Created, CanBeNull = false)]
        public DateTime Created { get; set; }
    }
}
