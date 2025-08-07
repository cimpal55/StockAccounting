using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.StockData
{
    [Table(Tables.StockData)]
    public record StockDataBaseModel
    {
        [Column(Columns.StockData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.StockData.ExternalDataId, CanBeNull = false)]
        public int ExternalDataId { get; set; }

        [Column(Columns.StockData.Quantity, CanBeNull = false)]
        public decimal Quantity { get; set; }

        [Column(Columns.StockData.LastSynchronization, CanBeNull = false)]
        public DateTime LastSynchronization { get; set; }

        [Column(Columns.StockData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

    }
}
