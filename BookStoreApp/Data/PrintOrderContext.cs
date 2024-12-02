using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrintOrder.Models;

namespace PrintOrder.Data
{
    public class PrintOrderContext : DbContext
    {
        public PrintOrderContext (DbContextOptions<PrintOrderContext> options)
            : base(options)
        {
        }

        public DbSet<PrintOrder.Models.Books> Books { get; set; } = default!;
    }
}
