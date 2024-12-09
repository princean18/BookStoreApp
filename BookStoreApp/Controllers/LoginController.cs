using BookStoreApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace BookStoreApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly BookStoreAppContext _context;

        public LoginController(BookStoreAppContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var viewModel = new LoginModel();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            string emailId = loginModel.EmailId;
            string password = loginModel.Password;
            string passwordHash = "";

            // SHA256 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Get the hashed string.  
                passwordHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                // Print the string.   

            }

            var obj = _context.Users.Where(a => a.Email.Equals(loginModel.EmailId) && a.PasswordHash.Equals(passwordHash)).FirstOrDefault();
            if (obj != null)
            {
                HttpContext.Session.SetString("emailId", obj.Email);
                HttpContext.Session.SetString("userName", obj.Username);
                return RedirectToAction("Index", "Dashboard");
            }

            return View(loginModel);
        }

    }
}
