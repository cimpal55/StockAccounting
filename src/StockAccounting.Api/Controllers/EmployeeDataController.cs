using Microsoft.AspNetCore.Mvc;
using StockAccounting.Api.Repositories.Interfaces;
using StockAccounting.Core.Data.Models.Data.EmployeeData;

namespace StockAccounting.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDataController : Controller
    {
        private readonly IEmployeeDataRepository _repository;
        public EmployeeDataController(IEmployeeDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeDataModel>>> GetEmployeeData()
        {
            var result = await _repository.GetEmployeeData();
            return Ok(result);
        }
    }
}
