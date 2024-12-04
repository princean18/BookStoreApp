using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
