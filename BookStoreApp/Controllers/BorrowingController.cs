using BookStoreApp.Data;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Controllers
{
    public class BorrowingController : Controller
    {
        private readonly BookStoreAppContext _context;

        public BorrowingController(BookStoreAppContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> BorrowingDetails()
        {

            var borrowingModel = from borrowing in _context.Borrowing
                       join users in _context.Users on borrowing.UserId equals users.Id
                       join books in _context.Books on borrowing.BookId equals books.Id
                       select new { 
                           userName = users.Username, 
                           bookName = books.Title, 
                           borrowDate = borrowing.BorrowDate, 
                           dueDate = borrowing.DueDate,
                           returnDate = borrowing.ReturnDate,
                           lateFee = borrowing.LateFee
                       };

            if (borrowingModel == null)
            {
                return NotFound();
            }

            return Json(borrowingModel);
        }
    }
}

    
