using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Api.Repositories;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.Models.Data.DocumentData;
using StockAccounting.Core.Data.Models.DataTransferObjects;
using System.Collections.ObjectModel;
using StockAccounting.Core.Android.Models.Data;
using StockAccounting.Core.Data.Enums;
using StockAccounting.Core.Data.Models.Data.InventoryData;
using static LinqToDB.Common.Configuration;

namespace StockAccounting.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentDataController : ControllerBase
    {
        private readonly IDocumentDataRepository _repository;

        public DocumentDataController(IDocumentDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentDataModel>>> GetDocumentData()
        {
            return Ok(await _repository.GetDocumentData());
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<DocumentDataModel>> InsertDocument([FromBody]ScannedModel data)
        {
            Log.Information("API_InsertDocument {@Data}", data);

            await _repository.InsertData(data);

            Log.Information("API_InsertDocument");

            await Log.CloseAndFlushAsync();

            return Ok(data);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<DocumentDataModel>> InsertDocumentAfterInventoryCheck(InventoryDataRecord inventory)
        {
            Log.Information("API_InsertDocumentAfterInventory {@Data}", inventory);

            var documentData = new DocumentDataModel()
            {
                Employee1Id = inventory.Employee1CheckerId,
                Employee2Id = inventory.ScannedEmployeeId,
                DocumentType = (int)StockTypes.Inventory,
                IsSynchronization = false,
                ManuallyAdded = false,
                Created = DateTime.Now
            };

            var docId = await _repository.InsertDocumentWithIdentityAsync(documentData);

            Log.Information("API_InsertedDocumentId {DocId}", docId);

            await Log.CloseAndFlushAsync();

            return CreatedAtAction(nameof(GetDocumentData), new { docId }, docId);
        }

        [HttpPost("[action]/docId={docId}")]
        public async Task<ActionResult<DocumentDataModel>> InsertDetailsAfterInventoryCheck(IEnumerable<ScannedInventoryDataRecord> details, int docId)
        {
            Log.Information("API_InsertDetailsAfterInventory {@Data}", details);

            await _repository.InsertDetailsAfterInventory(details, docId);

            await Log.CloseAndFlushAsync();

            return Ok(details);
        }
    }
}
