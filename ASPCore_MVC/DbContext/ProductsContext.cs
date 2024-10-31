using ASPCore_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPCore_MVC.DbContext
{
    public class ProductsContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options) { }
        public DbSet<Product>? Products { get; set; }
    }
}
