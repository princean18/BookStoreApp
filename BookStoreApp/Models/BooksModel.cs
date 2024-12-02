using System.ComponentModel.DataAnnotations;

namespace PrintOrder.Models
{
    public class Books
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Genre { get; set; }
        public string ISBN { get; set; }
        public string AvailableCopies { get; set; }

    }
}
