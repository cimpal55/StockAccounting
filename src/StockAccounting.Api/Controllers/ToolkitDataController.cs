using Microsoft.AspNetCore.Mvc;
using Serilog;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.Models.Data;

namespace StockAccounting.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ToolkitDataController : ControllerBase
    {
        private readonly IToolkitDataRepository _repository;

        public ToolkitDataController(IToolkitDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ToolkitModel>>> GetToolkitData()
        {
            var result = await _repository.GetToolkitData();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<List<ToolkitExternalModel>>> GetToolkitExternalDataByToolkitIdAsync(int id)
        {
            var result = await _repository.GetToolkitExternalDataByToolkitIdAsync(id);
            return Ok(result);
        }
    }
}
