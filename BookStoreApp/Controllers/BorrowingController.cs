using BookStoreApp.Data;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

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

        public IActionResult IssueBooks()
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

        [HttpPost]
        public async Task<ActionResult> CalculateLateFee(int? id)
        {
            var lapsedDays = 0.00;
            var lateFee = 0.00;
            var lateFeeForDay = 0.25;
            DateTime currentDate = DateTime.Now;

            var borrowingModel = await _context.Borrowing.FirstOrDefaultAsync(m => m.Id == id);            

            //Calculate diff. b/w duedate and current date
            if (borrowingModel!=null)
            {
                DateTime dueDate = borrowingModel.DueDate;                
                lapsedDays = (currentDate - dueDate).TotalDays;
            }

            //Calculate latefee for lapsed days
            if (lapsedDays > 0)
            {
                lateFee = lapsedDays * lateFeeForDay;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    borrowingModel.LateFee = "£"+ Math.Round((Decimal)lateFee, 2).ToString();
                    _context.Update(borrowingModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowingModelExists(borrowingModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (borrowingModel == null)
            {
                return NotFound();
            }

            return Json(borrowingModel);
        }

        [HttpPost]
        public async Task<ActionResult<List<Borrowing>>> ReturnBook(int id)
        {
            var borrowingModel = await _context.Borrowing.FirstOrDefaultAsync(m => m.Id == id);
            
            if (borrowingModel != null)
            {
                borrowingModel.ReturnDate = DateTime.Now;
                try
                {
                    _context.Update(borrowingModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowingModelExists(borrowingModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Json(borrowingModel);
        }

        [HttpGet]
        public async Task<ActionResult> GetMemberName(string q)
        {

            var usersModel = from u in _context.Users
                             where u.Username.Contains(q) && u.Role != "A"
                             select new
                             {
                                 Id = u.Id,
                                 Username = u.Username,
                                 EmailId = u.Email
                             };

            if (usersModel == null)
            {
                return NotFound();
            }

            return Json(usersModel);
        }

        [HttpGet]
        public async Task<ActionResult> GetBookTitle(string q)
        {

            var usersModel = from b in _context.Books
                             where b.Title.Contains(q)
                             select new
                             {
                                 Id = b.Id,
                                 BookTitle = b.Title,
                                 ISBN = b.ISBN,
                                 AvailableCopies = b.AvailableCopies
                             };

            if (usersModel == null)
            {
                return NotFound();
            }

            return Json(usersModel);
        }

        [HttpPost]
        public async Task<ActionResult<List<Books>>> IssueNewBook(Borrowing book)
        {
            var booksIssueModel = await _context.Borrowing.FirstOrDefaultAsync(
                                            m => m.UserId == book.UserId && 
                                            m.BookId == book.BookId &&
                                            m.ReturnDate == null);
            book.LateFee = "£0";

            //var errors = ModelState
            //    .Where(x => x.Value.Errors.Count > 0)
            //    .Select(x => new { x.Key, x.Value.Errors })
            //    .ToArray();

            var booksModel = await _context.Books.FirstOrDefaultAsync(m => m.Id == book.BookId);

            if (booksIssueModel != null)
            {
                return Problem("Book already issued for the member.<br> Duplicate entries are not allowed.");
            }

            if (ModelState.IsValid)
            {
                booksModel.AvailableCopies =  booksModel.AvailableCopies-1;
                _context.Update(booksModel);
                _context.Add(book);
                await _context.SaveChangesAsync();
            }
            return Json(book);
        }


        private bool BorrowingModelExists(int id)
        {
            return (_context.Borrowing?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

    
