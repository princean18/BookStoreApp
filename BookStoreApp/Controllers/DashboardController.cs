using BookStoreApp.Data;
using BookStoreApp.Helper;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly BookStoreAppContext _context;
        public DashboardController(BookStoreAppContext context)
        {
            _context = context;
        }

        [UserPermissionAttribute]
        public IActionResult Index()
        {

            ViewData["booksModel"] = _context.Books.Count();
            ViewData["totalBorrowing"] = _context.Borrowing.Count();
            ViewData["totalUsers"] = _context.Users.Count();

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("emailId");
            return Redirect("/Login");
        }

    }
}
