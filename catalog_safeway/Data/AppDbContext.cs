using Microsoft.EntityFrameworkCore;
using catalog_safeway.Models;

namespace catalog_safeway.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Product> Products { get; set; }
    }
}
