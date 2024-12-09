using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Data;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Security.Cryptography;

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

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<List<Books>>> CreateMember(UsersModel user)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            // SHA256 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash));
                // Get the hashed string.  
                user.PasswordHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                // Print the string.   
            }

            var usersModel = await _context.Users.FirstOrDefaultAsync(m => m.Email == user.Email);
            if (usersModel != null)
            {
                return Problem("Member already exists.<br> Duplicate entries are not allowed.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
            }
            return Json(user);

        }

        [HttpGet]
        //public async Task<ActionResult<Books>> Details(int? id)
        public async Task<ActionResult<List<UsersModel>>> UserDetails()

        {
            //var usersModel = await _context.Users.ToListAsync();
            var usersModel = from u in _context.Users
                             where u.Role != "A"
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
