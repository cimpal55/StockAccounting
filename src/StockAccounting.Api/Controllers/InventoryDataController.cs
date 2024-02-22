using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Api.Repositories;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.Models.Data;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using System.Collections.ObjectModel;

namespace StockAccounting.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryDataController : ControllerBase
    {
        private readonly IInventoryDataRepository _repository;

        public InventoryDataController(IInventoryDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryDataModel>>> GetInventoryData()
        {
            var result = await _repository.GetInventoryData();
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<InventoryDataModel>> InsertDocument([FromBody]ScannedModel data)
        {
            Log.Information("API_InsertDocument {@Data}", data);

            var docId = await _repository.InsertInventoryDataAsync(data);

            Log.Information("API_InsertDocument {DocId}", docId);

            await Log.CloseAndFlushAsync();

            return Ok(data);
        }
    }
}
