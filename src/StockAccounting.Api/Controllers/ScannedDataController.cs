using Microsoft.AspNetCore.Mvc;

namespace StockAccounting.Api.Controllers
{
    public class ScannedDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
