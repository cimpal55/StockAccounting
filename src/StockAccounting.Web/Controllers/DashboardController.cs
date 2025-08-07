using Microsoft.AspNetCore.Mvc;
using StockAccounting.Web.Controllers.Base;
using StockAccounting.Web.ViewModels;

namespace StockAccounting.Web.Controllers
{
    public class DashboardController : BaseController
    {
        //private readonly IStockDataRepository _stockDataRepository;
        //private readonly IRepository
        public DashboardController() { }

        public async Task<IActionResult> List()
        {
            return View();
        }
    }
}
