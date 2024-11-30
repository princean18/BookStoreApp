using Microsoft.AspNetCore.Mvc;

namespace PrintOrder.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
