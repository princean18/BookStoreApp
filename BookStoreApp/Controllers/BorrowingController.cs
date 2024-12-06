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

        private bool BorrowingModelExists(int id)
        {
            return (_context.Borrowing?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

    
