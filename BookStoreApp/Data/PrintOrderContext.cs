using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Models;

namespace BookStoreApp.Data
{
    public class BookStoreAppContext : DbContext
    {
        public BookStoreAppContext(DbContextOptions<BookStoreAppContext> options)
            : base(options)
        {
        }

        public DbSet<BookStoreApp.Models.Books> Books { get; set; } = default!;
        public DbSet<BookStoreApp.Models.UsersModel> Users { get; set; } = default!;
        public DbSet<BookStoreApp.Models.Borrowing> Borrowing { get; set; } = default!;
    }
}
