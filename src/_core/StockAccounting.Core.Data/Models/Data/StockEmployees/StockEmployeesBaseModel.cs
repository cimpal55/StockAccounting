using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.StockEmployees
{
    [Table(Tables.StockEmployees)]
    public record StockEmployeesBaseModel
    {
        [Column(Columns.StockEmployees.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.ScannedData.DocumentSerialNumber, CanBeNull = false)]
        public string DocumentSerialNumber { get; set; } = string.Empty;

        [Column(Columns.ScannedData.DocumentNumber, CanBeNull = false)]
        public int? DocumentNumber { get; set; }

        [Column(Columns.StockEmployees.StockDataId, CanBeNull = false)]
        public int StockDataId { get; set; }

        [Column(Columns.StockEmployees.EmployeeId, CanBeNull = false)]
        public int EmployeeId { get; set; }

        [Column(Columns.StockEmployees.StockTypeId, CanBeNull = false)]
        public int StockTypeId { get; set; }

        [Column(Columns.StockEmployees.Quantity, CanBeNull = false)]
        public decimal Quantity { get; set; }

        [Column(Columns.StockEmployees.LastSynchronization, CanBeNull = false)]
        public DateTime? LastSynchronization { get; set; }

        [Column(Columns.StockEmployees.Created, CanBeNull = false)]
        public DateTime Created { get; set; }
    }
}
