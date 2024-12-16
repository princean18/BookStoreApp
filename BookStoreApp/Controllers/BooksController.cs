using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Data;
using BookStoreApp.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using static System.Reflection.Metadata.BlobBuilder;
using DocumentFormat.OpenXml.Office2010.Excel;

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

        public IActionResult ImportBooks()
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
            if (borrowingModel != null)
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

        [HttpPost]
        public IActionResult ImportExcelDataSet(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("File Not Selected");

            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
                return Content("File Not Selected");

            using var workbook = new XLWorkbook(file.OpenReadStream());
            var ws = workbook.Worksheet(1);

            int totalRows = ws.Rows().Count();

            var BookList = new List<Books>();
            var booksModel = _context.Books;
            var bookId = 0;
            for (int i = 2; i <= totalRows; i++)
            {
                var booksModelExisting = booksModel.FirstOrDefault(b => b.ISBN == ws.Cell(i, 4).GetValue<string>());

                if (booksModelExisting != null)
                {
                    bookId = booksModelExisting.Id;
                }
                else
                {
                    bookId = 0;
                }
                BookList.Add(new Books
                {
                    Id = bookId,
                    Title = ws.Cell(i, 1).GetValue<string>(),
                    AuthorName = ws.Cell(i, 2).GetValue<string>(),
                    Genre = ws.Cell(i, 3).GetValue<string>(),
                    ISBN = ws.Cell(i, 4).GetValue<string>(),
                    AvailableCopies = ws.Cell(i, 5).GetValue<int>()
                });
                booksModelExisting = null;
            }

            //_context.Books.AddRange(BookList);
            //_context.SaveChanges();
            
            return Json(BookList);
        }

        public FileStreamResult DownloadSampleExcel()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files\\Sample_Book_Data_set.xlsx");
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            var filename = System.IO.Path.GetFileName(path);
            var mimeType = "application/octet-stream";
            var stream = System.IO.File.OpenRead(path);

            return new FileStreamResult(stream, mimeType) { FileDownloadName = filename };
        }

        [HttpPost]
        public async Task<ActionResult> importSave([FromBody] BookList books)
        {

            if (ModelState.IsValid)
            {
                _context.Books.AddRange(books.books);
                await _context.SaveChangesAsync();
            }
            return Json(books);

        }

    
    }
}
