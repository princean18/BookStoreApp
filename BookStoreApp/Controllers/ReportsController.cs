using BookStoreApp.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly BookStoreAppContext _context;
        public ReportsController(BookStoreAppContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public async Task<IActionResult> GenerateReport(string? rptType)
        {
            string fileName = "";
            var strDate = DateTime.Now.ToString("dd_MM_yyyy");
            var workbook = new XLWorkbook();
            switch (rptType)
            {
                case "BI":
                    workbook = await GetReportDataBI();
                    fileName = "Book_Inventory_" + strDate;
                    break;
                case "AB":
                    workbook = await GetReportDataAB();
                    fileName = "Active_Borrowing_" + strDate;
                    break;
                case "OB":
                    workbook = await GetReportDataOB();
                    fileName = "Overdue_Books_" + strDate;
                    break;
            }

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string filename = string.Format($"{fileName}.xlsx");

            workbook.Dispose();
            stream.Dispose();

            return File(content, contentType, filename);
        }

        public async Task<XLWorkbook> GetReportDataBI()
        {
            var booksModel = await _context.Books.ToListAsync();

            var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Reports");
            worksheet.Cell(1, 1).Value = "Title";
            worksheet.Cell(1, 2).Value = "Author Name";
            worksheet.Cell(1, 3).Value = "Genre";
            worksheet.Cell(1, 4).Value = "ISBN";
            worksheet.Cell(1, 5).Value = "Copies";

            IXLRange range = worksheet.Range(worksheet.Cell(1, 1).Address, worksheet.Cell(1, 5).Address);
            range.Style.Fill.SetBackgroundColor(XLColor.Aquamarine1);

            int index = 1;
            foreach (var item in booksModel)
            {
                index++;
                worksheet.Cell(index, 1).Value = item.Title;
                worksheet.Cell(index, 2).Value = item.AuthorName;
                worksheet.Cell(index, 3).Value = item.Genre;
                worksheet.Cell(index, 4).Value = item.ISBN;
                worksheet.Cell(index, 5).Value = item.AvailableCopies;
            }

            return workbook;
        }

        public async Task<XLWorkbook> GetReportDataAB()
        {
            var borrowingModel = from borrowing in _context.Borrowing
                                 join users in _context.Users on borrowing.UserId equals users.Id
                                 join books in _context.Books on borrowing.BookId equals books.Id
                                 where borrowing.ReturnDate == null
                                 select new
                                 {
                                     userName = users.Username,
                                     bookName = books.Title,
                                     borrowDate = borrowing.BorrowDate,
                                     dueDate = borrowing.DueDate,
                                     lateFee = borrowing.LateFee
                                 };

            var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Reports");
            worksheet.Cell(1, 1).Value = "Book Name";
            worksheet.Cell(1, 2).Value = "User";
            worksheet.Cell(1, 3).Value = "Borrowed Date";
            worksheet.Cell(1, 4).Value = "Due Date";
            worksheet.Cell(1, 5).Value = "Late Fee";

            IXLRange range = worksheet.Range(worksheet.Cell(1, 1).Address, worksheet.Cell(1, 5).Address);
            range.Style.Fill.SetBackgroundColor(XLColor.Aquamarine1);

            int index = 1;
            foreach (var item in borrowingModel)
            {
                index++;
                worksheet.Cell(index, 1).Value = item.bookName;
                worksheet.Cell(index, 2).Value = item.userName;
                worksheet.Cell(index, 3).Value = item.borrowDate.ToString("dd/MM/yyyy");
                worksheet.Cell(index, 4).Value = item.dueDate.ToString("dd/MM/yyyy");
                worksheet.Cell(index, 5).Value = item.lateFee;
            }
            return workbook;
        }

        public async Task<XLWorkbook> GetReportDataOB()
        {
            CalculateLateFee();
            var borrowingModel = from borrowing in _context.Borrowing
                                 join users in _context.Users on borrowing.UserId equals users.Id
                                 join books in _context.Books on borrowing.BookId equals books.Id
                                 where (borrowing.ReturnDate == null) && (borrowing.DueDate < DateTime.Now)
                                 select new
                                 {
                                     userName = users.Username,
                                     bookName = books.Title,
                                     borrowDate = borrowing.BorrowDate,
                                     dueDate = borrowing.DueDate,
                                     returnDate = borrowing.ReturnDate,
                                     lateFee = borrowing.LateFee,
                                 };

            var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Reports");
            worksheet.Cell(1, 1).Value = "Book Name";
            worksheet.Cell(1, 2).Value = "User";
            worksheet.Cell(1, 3).Value = "Borrowed Date";
            worksheet.Cell(1, 4).Value = "Due Date";
            worksheet.Cell(1, 5).Value = "Late Fee";

            IXLRange range = worksheet.Range(worksheet.Cell(1, 1).Address, worksheet.Cell(1, 5).Address);
            range.Style.Fill.SetBackgroundColor(XLColor.Aquamarine1);

            int index = 1;
            foreach (var item in borrowingModel)
            {
                index++;
                worksheet.Cell(index, 1).Value = item.bookName;
                worksheet.Cell(index, 2).Value = item.userName;
                worksheet.Cell(index, 3).Value = item.borrowDate.ToString("dd/MM/yyyy");
                worksheet.Cell(index, 4).Value = item.dueDate.ToString("dd/MM/yyyy");
                worksheet.Cell(index, 5).Value = item.lateFee;
            }
            return workbook;
        }

        //[HttpPost]
        public ActionResult CalculateLateFee()
        {
            var lapsedDays = 0.00;
            var lateFee = 0.00;
            var lateFeeForDay = 0.25;
            DateTime currentDate = DateTime.Now;

            var borrowingModel = from borrowing in _context.Borrowing
                                 where (borrowing.ReturnDate == null) && (borrowing.DueDate < DateTime.Now)
                                 select borrowing;

            //Calculate diff. b/w duedate and current date
            foreach (var row in borrowingModel)
            {
                DateTime dueDate = row.DueDate;
                lapsedDays = (currentDate - dueDate).TotalDays;
                //Calculate latefee for lapsed days
                if (lapsedDays > 0)
                {
                    lateFee = lapsedDays * lateFeeForDay;
                }
                row.LateFee = "£" + Math.Round((Decimal)lateFee, 2).ToString();
            }

            _context.SaveChanges();
            return null;
        }

    }
}
