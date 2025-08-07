using LinqToDB.Mapping;

namespace StockAccounting.Core.Data.Models.DataTransferObjects
{
    public class InventoryDetailsListModel
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("ItemNumber")] 
        public string ItemNumber { get; set; } = string.Empty;

        [Column("PluCode")]
        public string PluCode { get; set; } = string.Empty;

        [Column("Barcode")]
        public string Barcode { get; set; } = string.Empty;

        [Column("Unit")] 
        public string Unit { get; set; } = string.Empty;

        [Column("Quantity")]
        public decimal Quantity { get; set; }

        [Column("FinalQuantity")]
        public decimal FinalQuantity { get; set; }

        [Column("Created")]
        public DateTime Created { get; set; }


    }
}
