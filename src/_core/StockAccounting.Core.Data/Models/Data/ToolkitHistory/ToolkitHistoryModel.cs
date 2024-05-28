using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.ToolkitHistory)]
    public class ToolkitHistoryModel
    {
        [Column(ToolkitHistory.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(ToolkitHistory.ToolkitId, CanBeNull = false)]
        public int ToolkitId { get; set; }

        [Column(ToolkitHistory.EmployeeId, CanBeNull = false)]
        public int EmployeeId { get; set; }

        [Column(ToolkitHistory.Created, CanBeNull = false)]
        public DateTime Created { get; set; }
    }
}
