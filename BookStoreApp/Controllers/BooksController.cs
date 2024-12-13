using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Data;
using BookStoreApp.Models;

namespace BookStoreApp.Controllers
{
    public class BooksController : Controller
    {

        private readonly BookStoreAppContext _context;

        public BooksController(BookStoreAppContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddBooks()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult<List<Books>>> Details()

        {

            var booksModel = await _context.Books.ToListAsync();
            if (booksModel == null)
            {
                return NotFound();
            }

            return Json(booksModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<List<Books>>> Create(Books books)
        {
            var booksModel = await _context.Books.FirstOrDefaultAsync(m => m.ISBN == books.ISBN);
            if (booksModel != null && booksModel.ISBN == books.ISBN)
            {
                return Problem("Book entry already exists.<br> Duplicate entries are not allowed.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(books);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            return Json(books);

        }

        public async Task<IActionResult> EditBooks(int? id)
        {

            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var booksModel = await _context.Books.FindAsync(id);
            if (booksModel == null)
            {
                return NotFound();
            }
            return View(booksModel);
        }

        [HttpPost]
        public async Task<ActionResult<List<Books>>> UpdateBook(int id, Books booksModel)
        {
            
            if (id != booksModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booksModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BooksModelExists(booksModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Json(booksModel);
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult<List<Books>>> DeleteBook(int id)
        {
            var borrowingModel = await _context.Borrowing.FirstOrDefaultAsync(m => m.BookId == id);
            if (borrowingModel!=null)
            {
                return Problem("Cannot delete book. Already issued to member.");
            }

            if (_context.Books == null)
            {
                return Problem("Entity set 'BookStoreAppContext.BooksModel'  is null.");
            }
            var booksModel = await _context.Books.FindAsync(id);
            if (booksModel != null)
            {
                _context.Books.Remove(booksModel);
            }

            await _context.SaveChangesAsync();
            return Json(booksModel);
        }

        private bool BooksModelExists(int id)
        {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
