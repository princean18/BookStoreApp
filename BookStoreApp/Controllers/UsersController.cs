using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Data;
using BookStoreApp.Models;

namespace BookStoreApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly BookStoreAppContext _context;

        public UsersController(BookStoreAppContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //public async Task<ActionResult<Books>> Details(int? id)
        public async Task<ActionResult<List<UsersModel>>> UserDetails()

        {
            //var usersModel = await _context.Users.ToListAsync();
            var usersModel = from u in _context.Users
                                   select new
                                   {
                                       Username = u.Username,
                                       Email = u.Email,
                                       Role = u.Role
                                   };

            if (usersModel == null)
            {
                return NotFound();
            }

            return Json(usersModel);
        }

    }
}
