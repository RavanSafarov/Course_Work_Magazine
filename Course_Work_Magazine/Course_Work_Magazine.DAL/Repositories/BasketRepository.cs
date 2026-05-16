using Course_Work_Magazine.Data;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly OrderFlowDbContext _context;

    public BasketRepository(OrderFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Basket basketItem)
    {
        await _context.Baskets.AddAsync(basketItem);
    }

    public async Task<Basket?> GetByIdAsync(int id)
    {
        return await _context.Baskets.Include(b => b.Product).Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Basket>> GetUserBasketAsync(string userId)
    {
        return await _context.Baskets.Where(b => b.UserId == userId).Include(b => b.Product).ToListAsync();
    }

    public IQueryable<Basket> Query()
    {
        return _context.Baskets.Include(b => b.Product).Include(b => b.User).AsQueryable();
    }

    public void Remove(Basket basketItem)
    {
        _context.Baskets.Remove(basketItem);
    }
    public void RemoveRange(IEnumerable<Basket> basketItems)
    {
        _context.Baskets.RemoveRange(basketItems);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
