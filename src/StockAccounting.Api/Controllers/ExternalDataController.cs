using Microsoft.AspNetCore.Mvc;
using StockAccounting.Api.Repositories;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalDataController : Controller
    {
        private readonly IExternalDataRepository _repository;
        public ExternalDataController(IExternalDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExternalDataModel>>> GetExternalData()
        {
            var result = await _repository.GetExternalData();
            return Ok(result);
        }

        [HttpGet]
        [Route("{barcode}")]
        public async Task<ActionResult<List<ExternalDataModel>>> GetExternalDataByBarcode(string barcode)
        {
            var result = await _repository.GetExternalDataByBarcode(barcode);
            return Ok(result);
        }
    }
}
