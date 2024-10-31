using ASPCore_MVC.DbContext;
using ASPCore_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPCore_MVC.Services
{
    public interface IProductsService
    {
        public ProductsContext? Context { get; set; }
        public Task<Product?> Create(Product product);
        public Task<IEnumerable<Product>?> Read();
        public Task<Product?> Read(int id);
        public Task<Product?> Read(string name);
        public Task<Product?> Update(Product product);
        public Task<bool> Delete(int id);

    }
    public class ProductsService : IProductsService
    {
        public ProductsContext? Context { get; set; }

        public async Task<Product?> Create(Product product)
        {
            await Context.Products.AddAsync(product);
            await Context?.SaveChangesAsync();
            foreach(var p in Context.Products)
            {
                Console.WriteLine(p.Name);
            }
            return product;
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                Context?.Products.Remove(Context?.Products.Find(id));
                await Context?.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Product>?> Read()
        {
            return await Context?.Products?.ToListAsync();
        }

        public async Task<Product?> Read(int id)
        {
            return await Context?.Products?.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> Read(string name)
        {
            return await Context?.Products?.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<Product?> Update(Product product)
        {
            Console.WriteLine("A");
            Context?.Products?.Update(product);
            await Context?.SaveChangesAsync();

            return product;
        }
    }
}
