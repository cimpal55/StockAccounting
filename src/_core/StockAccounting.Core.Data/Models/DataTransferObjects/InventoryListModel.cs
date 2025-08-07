using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public class InventoryListModel
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Employee1CheckerId")]
        public int Employee1CheckerId { get; set; }

        [Column("Employee1Checker")]
        public string Employee1Checker { get; set; } = string.Empty;

        [Column("Employee2CheckerId")]
        public int? Employee2CheckerId { get; set; }

        [Column("Employee2Checker")]
        public string? Employee2Checker { get; set; }

        [Column("ScannedEmployeeId")]
        public int ScannedEmployeeId { get; set; }

        [Column("ScannedEmployee")]
        public string ScannedEmployee { get; set; } = string.Empty;

        [Column("Status")]
        public string Status { get; set; } = string.Empty;

        [Column("Created")]
        public DateTime Created { get; set; }

        [Column("Finished")]
        public DateTime? Finished { get; set; }

    }
}
