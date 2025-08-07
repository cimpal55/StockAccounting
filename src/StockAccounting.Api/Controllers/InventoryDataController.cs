using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Android.Models.DataTransferObjects;
using StockAccounting.Core.Data.Models.Data.InventoryData;

namespace StockAccounting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryDataController : Controller
    {
        private readonly IInventoryDataRepository _inventoryDataRepository;

        public InventoryDataController(IInventoryDataRepository inventoryDataRepository)
        {
            _inventoryDataRepository = inventoryDataRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryDataModel>>> GetInventoryData()
        {
            return Ok(await _inventoryDataRepository.GetInventoryData());
        }

        [HttpPost("GetLatestInventoryData")]
        public async Task<ActionResult<List<InventoryDataModel>>> GetLatestInventoryData(LatestInventoryDataModel latestSyncData)
        {
            return Ok(await _inventoryDataRepository.GetLatestInventoryData(latestSyncData.LastSyncDateTime));
        }

        [HttpGet("GetCheckedInventoryData")]
        public async Task<ActionResult<List<InventoryDataModel>>> GetCheckedInventoryData()
        {
            return Ok(await _inventoryDataRepository.GetCheckedInventoryData());
        }

        [HttpPost("InsertInventory")]
        public async Task<ActionResult<InventoryDataModel>> InsertInventory(InventoryDataModel data)
        {
            Log.Information("API_InsertDocument {@Data}", data);

            var docId = await _inventoryDataRepository.InsertInventoryDataAsync(data);

            Log.Information("API_InsertDocument {DocId}", docId);

            await Log.CloseAndFlushAsync();

            return CreatedAtAction(nameof(GetInventoryData), new { docId }, docId);
        }

        [HttpPost("UpdateDocument")]
        public async Task<ActionResult<InventoryDataModel>> UpdateInventory(InventoryDataModel data)
        {
            Log.Information("API_UpdateDocument {@Data}", data);

            if (data is not null)
                await _inventoryDataRepository.UpdateInventoryDataAsync(data);

            await Log.CloseAndFlushAsync();

            return CreatedAtAction(nameof(GetInventoryData), new { data }, data);
        }


        [HttpGet("GetInprocessInventoryData")]
        public async Task<ActionResult<List<InventoryDataModel>>> GetInprocessInventoryData()
        {
            return Ok(await _inventoryDataRepository.GetInprocessInventoryData());
        }

        [HttpGet]
        [Route("name={name}")]
        public async Task<ActionResult<int>> GetInventoryDataByName(string name)
        {
            var result = await _inventoryDataRepository.GetInventoryDataIdByNameAsync(name);
            return Ok(result);
        }
    }
}
