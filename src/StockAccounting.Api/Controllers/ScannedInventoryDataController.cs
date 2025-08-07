using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.Models.Data.InventoryData;
using StockAccounting.Core.Data.Models.Data.ScannedInventoryData;
using static LinqToDB.Common.Configuration;

namespace StockAccounting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScannedInventoryDataController : Controller
    {
        private readonly IScannedInventoryDataRepository _scannedInventoryDataRepository;

        public ScannedInventoryDataController(IScannedInventoryDataRepository scannedDataRepository)
        {
            _scannedInventoryDataRepository = scannedDataRepository ?? throw new ArgumentNullException(nameof(scannedDataRepository));
        }

        [HttpGet]
        public async Task<ActionResult<List<ScannedInventoryDataModel>>> GetScannedData()
        {
            return Ok(await _scannedInventoryDataRepository.GetScannedData());
        }

        [HttpPost("GetLatestScannedData")]
        public async Task<ActionResult<List<ScannedInventoryDataModel>>> GetLatestScannedData(List<InventoryDataModel> data)
        {
            return Ok(await _scannedInventoryDataRepository.GetLatestScannedData(data));
        }


        [HttpPost("InsertDocumentDetails/{id}")]
        public async Task<ActionResult<ScannedInventoryDataModel>> InsertDocumentDetails(List<ScannedInventoryDataModel> data, int id)
        {
            Log.Information("API_InsertDocumentDetails {@Data} {DocId}", data, id);
            await Log.CloseAndFlushAsync();

            try
            {
                if (data is { Count: > 0 })
                    await _scannedInventoryDataRepository.InsertScannedDataAsync(data, id);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error in InsertDocumentDetails {@Data} {DocId}", data, id);
                await Log.CloseAndFlushAsync();
            }

            return CreatedAtAction(nameof(GetScannedData), new { data }, data);
        }

        [HttpPost("UpdateDocumentDetails/{id}")]
        public async Task<ActionResult<ScannedInventoryDataModel>> UpdateDocumentDetails(List<ScannedInventoryDataModel> data, int id)
        {
            Log.Information("API_UpdateDocumentDetails {@Data} {DocId}", data, id);
            await Log.CloseAndFlushAsync();

            if (data is { Count: > 0 })
                await _scannedInventoryDataRepository.UpdateScannedDataAsync(data, id);


            return CreatedAtAction(nameof(GetScannedData), new { data }, data);
        }

        [HttpGet]
        [Route("employeeId={id}")]
        public async Task<ActionResult<ScannedInventoryDataModel>> GetDetailsByEmployeeId(int id)
        {
            return Ok(await _scannedInventoryDataRepository.GetDetailsByEmployeeIdAsync(id));
        }
    }
}
