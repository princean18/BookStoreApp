using BookStoreApp.Data;
using BookStoreApp.Helper;
using BookStoreApp.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
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
            ViewData["totalUsers"] = _context.Users.Where(b => b.Role.Equals("M")).Count();

            return View();
        }

        [UserPermissionAttribute]
        public async Task<IActionResult> SearchResult(int? id)
        {

            ViewData["BookId"] = id;
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> BorrowingSearchResult(int? bookId)
        {

            var borrowingModel = from borrowing in _context.Borrowing
                                 join users in _context.Users on borrowing.UserId equals users.Id
                                 join books in _context.Books on borrowing.BookId equals books.Id
                                 where books.Id == bookId
                                 select new
                                 {
                                     Id = borrowing.Id,
                                     userName = users.Username,
                                     bookName = books.Title,
                                     borrowDate = borrowing.BorrowDate,
                                     dueDate = borrowing.DueDate,
                                     returnDate = borrowing.ReturnDate,
                                     lateFee = borrowing.LateFee,
                                     dueDateAlert = borrowing.DueDate < DateTime.Now && borrowing.ReturnDate == null ? "Y" : "N"
                                 };

            if (borrowingModel == null)
            {
                return NotFound();
            }

            return Json(borrowingModel);
        }

        [HttpGet]
        public async Task<ActionResult> BookDetails(int? bookId)
        {
            var booksModel = await _context.Books.FindAsync(bookId);
            if (booksModel == null)
            {
                return NotFound();
            }
            return Json(booksModel);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("emailId");
            return Redirect("/Login");
        }

        public class BookAndBorrowData
        {
            public List<Books> Books { get; set; }
            public IQueryable<Borrowing> Borrowing { get; set; }
        }

    }
}
