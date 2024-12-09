using BookStoreApp.Helper;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.Controllers
{
    public class DashboardController : Controller
    {
        [UserPermissionAttribute]
        public IActionResult Index()
        {
            return View();
        }
    }
}
