using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrintOrder.Data;
using PrintOrder.Models;

namespace PrintOrder.Controllers
{
    public class BooksModelsController : Controller
    {
        private readonly PrintOrderContext _context;

        public BooksModelsController(PrintOrderContext context)
        {
            _context = context;
        }

        // GET: BooksModels
        public async Task<IActionResult> Index()
        {
              return _context.Books != null ? 
                          View(await _context.Books.ToListAsync()) :
                          Problem("Entity set 'PrintOrderContext.BooksModel'  is null.");
        }

        // GET: BooksModels/Details/5
        [HttpGet]
        //public async Task<ActionResult<Books>> Details(int? id)
        public async Task<ActionResult<List<Books>>> Details(int? id)

        {
            //if (id == null || _context.Books == null)
            //{
            //    return NotFound();
            //}

            var booksModel = await _context.Books.ToListAsync();
            if (booksModel == null)
            {
                return NotFound();
            }

            return Json( booksModel);
        }

        // GET: BooksModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BooksModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,AuthorName,Genre,ISBN,AvailableCopies")] Books booksModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booksModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booksModel);
        }

        // GET: BooksModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: BooksModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,AuthorName,Genre,ISBN,AvailableCopies")] Books booksModel)
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
                return RedirectToAction(nameof(Index));
            }
            return View(booksModel);
        }

        // GET: BooksModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var booksModel = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booksModel == null)
            {
                return NotFound();
            }

            return View(booksModel);
        }

        // POST: BooksModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'PrintOrderContext.BooksModel'  is null.");
            }
            var booksModel = await _context.Books.FindAsync(id);
            if (booksModel != null)
            {
                _context.Books.Remove(booksModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BooksModelExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
