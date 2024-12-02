using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrintOrder.Data;
using PrintOrder.Models;

namespace PrintOrder.Controllers
{
    public class BooksController : Controller
    {

        private readonly PrintOrderContext _context;

        public BooksController(PrintOrderContext context)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,AuthorName,Genre,ISBN,AvailableCopies")] Books books)
        {
            if (ModelState.IsValid)
            {
                _context.Add(books);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(books);
        }
    }
}
