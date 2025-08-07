using LinqToDB.Mapping;
using StockAccounting.Core.Data.Resources;

namespace StockAccounting.Core.Data.Models.Data.DocumentData
{
    [Table(Tables.DocumentData)]
    public record DocumentDataBaseModel
    {
        [Column(Columns.DocumentData.Id, IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Column(Columns.DocumentData.Employee1Id, CanBeNull = false)]
        public int Employee1Id { get; set; }

        [Column(Columns.DocumentData.Employee2Id, CanBeNull = false)]
        public int Employee2Id { get; set; }

        [Column(Columns.DocumentData.ManuallyAdded, CanBeNull = false)]
        public bool ManuallyAdded { get; set; } = false;

        [Column(Columns.DocumentData.IsSynchronization, CanBeNull = false)]
        public bool IsSynchronization { get; set; }

        [Column(Columns.DocumentData.DocumentType, CanBeNull = true)]
        public int DocumentType { get; set; }

        [Column(Columns.DocumentData.Created, CanBeNull = false)]
        public DateTime Created { get; set; }

        [Column(Columns.DocumentData.Updated, CanBeNull = true)]
        public DateTime Updated { get; set; }
    }
}
