using LinqToDB;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Core.Data.Models.Data.InventoryData;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using StockAccounting.Core.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAccounting.Core.Data.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDataConnection _conn;
        public InventoryRepository(AppDataConnection conn)
        {
            _conn = conn;
        }

        public IQueryable<InventoryListModel> GetInventoryDataQueryable()
        {
            var query = from x in _conn.InventoryData
                join e1 in _conn.Employees on x.Employee1CheckerId equals e1.Id into e1Join
                from e1 in e1Join.DefaultIfEmpty()
                join e2 in _conn.Employees on x.Employee2CheckerId equals e2.Id into e2Join
                from e2 in e2Join.DefaultIfEmpty()
                join e3 in _conn.Employees on x.ScannedEmployeeId equals e3.Id into e3Join
                from e3 in e3Join.DefaultIfEmpty()
                select new InventoryListModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Employee1CheckerId = x.Employee1CheckerId,
                    Employee1Checker = e1 != null ? string.Join(" ", e1.Name, e1.Surname, e1.Code) : "No employee assigned",
                    Employee2CheckerId = x.Employee2CheckerId,
                    Employee2Checker = e2 != null ? string.Join(" ", e2.Name, e2.Surname, e2.Code) : "No employee assigned",
                    ScannedEmployeeId = x.ScannedEmployeeId,
                    ScannedEmployee = e3 != null ? string.Join(" ", e3.Name, e3.Surname, e3.Code) : "No employee assigned",
                    Status = x.Status,
                    Created = x.Created,
                    Finished = x.Finished
                };

            return query;
        }

        public IQueryable<InventoryListModel> GetInventoryDataSearchTextQueryable(string searchText)
        {
            // Initial query setup
            var query = from x in _conn.InventoryData
                        join e1 in _conn.Employees on x.Employee1CheckerId equals e1.Id into e1Join
                        from e1 in e1Join.DefaultIfEmpty()
                        join e2 in _conn.Employees on x.Employee2CheckerId equals e2.Id into e2Join
                        from e2 in e2Join.DefaultIfEmpty()
                        join e3 in _conn.Employees on x.ScannedEmployeeId equals e3.Id into e3Join
                        from e3 in e3Join.DefaultIfEmpty()
                        select new InventoryListModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Employee1CheckerId = x.Employee1CheckerId,
                            Employee1Checker = e1 != null ? e1.Name + " " + e1.Surname + " " + e1.Code : "No employee assigned",
                            Employee2CheckerId = x.Employee2CheckerId,
                            Employee2Checker = e2 != null ? e2.Name + " " + e2.Surname + " " + e2.Code : "No employee assigned",
                            ScannedEmployeeId = x.ScannedEmployeeId,
                            ScannedEmployee = e3 != null ? e3.Name + " " + e3.Surname + " " + e3.Code : "No employee assigned",
                            Status = x.Status,
                            Created = x.Created,
                            Finished = x.Finished
                        };

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(item =>
                    (item.Employee1Checker.Contains(searchText) ||
                    item.Employee2Checker.Contains(searchText) ||
                    item.ScannedEmployee.Contains(searchText) ||
                    item.Status.Contains(searchText) ||
                    item.Created.ToString().Contains(searchText) ||
                    (item.Finished.HasValue && item.Finished.Value.ToString().Contains(searchText))
                    )
                );
            }

            return query;
        }

        public IQueryable<InventoryDetailsListModel> GetInventoryDetailsQueryable(int inventoryId, string searchText)
        {
            var query = from x in _conn.ScannedInventoryData
                        where x.InventoryDataId == inventoryId
                        select new InventoryDetailsListModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Barcode = x.Barcode,
                            ItemNumber = x.ItemNumber,
                            PluCode = x.PluCode,
                            Unit = x.Unit,
                            Quantity = x.Quantity,
                            FinalQuantity = x.FinalQuantity,
                            Created = x.Created,
                        };

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(item =>
                    (item.Name.Contains(searchText) ||
                     item.Barcode.Contains(searchText) ||
                     item.PluCode.Contains(searchText) ||
                     item.ItemNumber.Contains(searchText) ||
                     item.Created.ToString(CultureInfo.InvariantCulture).Contains(searchText) ||
                     item.Quantity.ToString(CultureInfo.InvariantCulture).Contains(searchText) ||
                     item.FinalQuantity.ToString(CultureInfo.InvariantCulture).Contains(searchText)
                    )
                );
            }

            return query;
        }
    }
}
