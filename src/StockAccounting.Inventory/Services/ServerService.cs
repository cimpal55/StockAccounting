using StockAccounting.Core.Android.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StockAccounting.Core.Android.Data;
using StockAccounting.Inventory.Models;
using StockAccounting.Inventory.Repositories.Interfaces;
using StockAccounting.Inventory.Services.Interfaces;

namespace StockAccounting.Inventory.Services
{
    public class ServerService : IServerService
    {
        private readonly IRestService _restService;
        private readonly IAdministrationRepository _administrationRepository;
        private readonly DatabaseContext _context;
        public ServerService(
            IRestService restService,
            DatabaseContext context,
            IAdministrationRepository administrationRepository)
        {
            _restService = restService;
            _context = context;
            _administrationRepository = administrationRepository;
        }

        public async Task<IReadOnlyList<ScannedInventoryDataJson>?> GetScannedDataFromServer()
        {
            var items = await _restService.GetScannedDataAsync();
            if (items is { Count: > 0 })
            {
                foreach (var item in items)
                {
                    var record = new ScannedInventoryDataRecord
                    {
                        ExternalId = item.Id,
                        InventoryDataId = item.InventoryDataId,
                        PluCode = item.PluCode,
                        Name = item.Name,
                        Barcode = item.Barcode,
                        ItemNumber = item.ItemNumber,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Created = DateTime.Now
                    };
                    await _context.InsertItemAsync(record)
                        .ConfigureAwait(false);
                }
            }

            return items;
        }

        public async Task<IReadOnlyList<InventoryDataJson>?> GetInventoryDataFromServer()
        {
            var items = await _restService.GetInventoryDataAsync();

            if (items is { Count: > 0 })
            {
                foreach (var item in items)
                {
                    var record = new InventoryDataRecord
                    {
                        ExternalId = item.Id,
                        Name = item.Name,
                        Employee1CheckerId = item.Employee1CheckerId,
                        Employee2CheckerId = item.Employee2CheckerId,
                        ScannedEmployeeId = item.ScannedEmployeeId,
                        Status = item.Status,
                        Created = DateTime.Now
                    };

                    await _context.InsertItemAsync(record)
                        .ConfigureAwait(false);
                }
            }

            return items;
        }

        public async Task<IReadOnlyList<InventoryDataJson>?> GetLatestInventoryDataFromServer()
        {
            var lastDateTime = await _administrationRepository.GetLatestServerSyncDateTimeAsync("tblInventoryData")
                .ConfigureAwait(false);

            var jsonDoc = JsonSerializer.Serialize(lastDateTime);
            var items = await _restService.GetLatestInventoryDataAsync(jsonDoc);

            if (items is { Count: > 0 })
            {
                foreach (var item in items)
                {
                    var record = new InventoryDataRecord
                    {
                        ExternalId = item.Id,
                        Name = item.Name,
                        Employee1CheckerId = item.Employee1CheckerId,
                        Employee2CheckerId = item.Employee2CheckerId,
                        ScannedEmployeeId = item.ScannedEmployeeId,
                        Status = item.Status,
                        Created = DateTime.Now
                    };

                    await _context.InsertItemAsync(record)
                        .ConfigureAwait(false);
                }
            }

            return items;
        }

        public async Task<IReadOnlyList<EmployeeJson>?> GetEmployeesFromServer()
        {
            var items = await _restService.GetEmployeesAsync();

            if (items is { Count: > 0 })
            {
                foreach (var item in items)
                {
                    var record = new EmployeeRecord
                    {
                        ExternalId = item.Id,
                        Name = item.Name,
                        Surname = item.Surname,
                        Code = item.Code,
                        Created = DateTime.Now
                    };

                    await _context.InsertItemAsync(record)
                        .ConfigureAwait(false);
                }
            }

            return items;
        }


        public async Task GetLatestScannedDataFromServer(IReadOnlyList<InventoryDataJson> docs)
        {
            var jsonDetails = JsonSerializer.Serialize(docs);
            var items = await _restService.GetLatestScannedDataAsync(jsonDetails);

            if (items is { Count: > 0 })
            {
                foreach (var item in items)
                {
                    var record = new ScannedInventoryDataRecord
                    {
                        ExternalId = item.Id,
                        InventoryDataId = item.InventoryDataId,
                        PluCode = item.PluCode,
                        Name = item.Name,
                        Barcode = item.Barcode,
                        ItemNumber = item.ItemNumber,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Created = DateTime.Now
                    };

                    await _context.InsertItemAsync(record)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
