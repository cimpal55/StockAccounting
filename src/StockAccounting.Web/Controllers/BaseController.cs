using Microsoft.AspNetCore.Mvc;

namespace StockAccounting.Web.Controllers
{
    public class BaseController : Controller
    {
        protected const int _itemsPerPage = 10;

        protected const int _pagesInRow = 9;
    }
}
