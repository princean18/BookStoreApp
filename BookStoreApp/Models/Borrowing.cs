using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class Borrowing
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string LateFee { get; set; }
    }
}
