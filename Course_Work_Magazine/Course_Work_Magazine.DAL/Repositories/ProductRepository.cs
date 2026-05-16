using Course_Work_Magazine.Data;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrderFlowDbContext _context;
    public ProductRepository(OrderFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }
    public IQueryable<Product> Query()
    {
        return _context.Products.Include(p => p.Category).Include(p => p.Seller).AsQueryable();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Products.Include(p => p.Category).Include(p => p.Seller).FirstOrDefaultAsync(p => p.Id == id);
    }

    public void Remove(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
