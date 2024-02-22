using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;
using static StockAccounting.Core.Data.Resources.Columns;

namespace StockAccounting.Core.Data.Models.Data
{
    [Table(Tables.StockData)]
    public record StockDataBaseModel
    {
        [Column(StockData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(StockData.ExternalDataId, CanBeNull = false)]
        public int ExternalDataId { get; set; }

        [Column(StockData.Quantity, CanBeNull = false)]
        public decimal Quantity { get; set; }

        [Column(StockData.LastSynchronization, CanBeNull = false)]
        public DateTime LastSynchronization { get; set; }

        [Column(StockData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

    }
}
