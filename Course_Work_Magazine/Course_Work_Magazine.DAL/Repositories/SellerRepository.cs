using Course_Work_Magazine.Data;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;


namespace Course_Work_Magazine.Repositories;

public class SellerRepository : ISellerRepository
{
    private readonly OrderFlowDbContext _context;
    public SellerRepository(OrderFlowDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Seller seller)
    {
       await _context.Sellers.AddAsync(seller);
    }

    public IQueryable<Seller> Query()
    {
       return _context.Sellers.AsQueryable();
    }

    public async Task<Seller?> GetByIdAsync(int id)
    {
        return await _context.Sellers.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Seller?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Sellers.Include(s => s.Orders).FirstOrDefaultAsync(s => s.Id == id);
    }

    public void Remove(Seller seller)
    {
        _context.Sellers.Remove(seller);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
