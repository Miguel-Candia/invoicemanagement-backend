using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class SqliteDbContext : DbContext
    {
        public SqliteDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CreditNote> CreditNotes { get; set; }
    }
}
