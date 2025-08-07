using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.ToolkitHistory
{
    [Table(Tables.ToolkitHistory)]
    public class ToolkitHistoryModel
    {
        [Column(Columns.ToolkitHistory.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.ToolkitHistory.ToolkitId, CanBeNull = false)]
        public int ToolkitId { get; set; }

        [Column(Columns.ToolkitHistory.EmployeeId, CanBeNull = false)]
        public int EmployeeId { get; set; }

        [Column(Columns.ToolkitHistory.Created, CanBeNull = false)]
        public DateTime Created { get; set; }
    }
}
