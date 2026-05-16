using Course_Work_Magazine.Data;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly OrderFlowDbContext _context;
    public CategoryRepository(OrderFlowDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Category category)
    {
       await _context.Categories.AddAsync(category);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public IQueryable<Category> Query()
    {
        return _context.Categories.AsQueryable();
    }

    public void Remove(Category category)
    {
        _context.Categories.Remove(category);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
