﻿using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
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
        public int AvailableCopies { get; set; }

    }

    public class BookList
    {
        public List<Books> books { get; set; }
    }
}
