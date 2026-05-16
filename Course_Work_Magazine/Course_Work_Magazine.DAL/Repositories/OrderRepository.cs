using Course_Work_Magazine.Data;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderFlowDbContext _context;

    public OrderRepository(OrderFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task AddRangeAsync(IEnumerable<OrderItem> items)
    {
        await _context.Set<OrderItem>().AddRangeAsync(items);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(bool includeArchived = false)
    {
        var query = QueryWithDetails();

        if (!includeArchived)
        {
            query = query.Where(o =>
                o.DeletedAt == null &&
                o.Seller != null &&
                o.Seller.DeletedAt == null
            );
        }

        return await query.ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id)
    {
        return await QueryWithDetails().FirstOrDefaultAsync(o => o.Id == id);
    }

    public IQueryable<Order> QueryWithDetails()
    {
        return _context.Orders.Include(o => o.Seller).Include(o => o.OrderItems).AsQueryable();
    }

    public void Remove(Order order)
    {
        _context.Orders.Remove(order);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}