using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Models;
using DocumentFormat.OpenXml.Spreadsheet;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<UsersModel>()
                .ToTable("Users");
            modelBuilder.Entity<UsersModel>()
                .HasData(
                    new UsersModel
                    {
                        Id = 1,
                        Username = "Admin",
                        PasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                        Email = "admin@lms.com",
                        Role = "A"
                    }
                );
        }
    }
}
