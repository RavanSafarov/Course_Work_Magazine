using Course_Work_Magazine.Data;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly OrderFlowDbContext _context;
    public OrderItemRepository(OrderFlowDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
    }

    public async Task<IEnumerable<OrderItem>> GetAllAsync(bool includeArchived = false)
    {
        var query = QueryWithDetails();
        if (!includeArchived)
        {
            query = query.Where(o =>
                o.Order != null &&
                o.Order.DeletedAt == null
            );
        }
        return await query.ToListAsync();
    }

    public async Task<OrderItem?> GetByIdAsync(int id)
    {
        return await _context.OrderItems.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<OrderItem?> GetByIdWithDetailsAsync(int id)
    {
        return await QueryWithDetails().FirstOrDefaultAsync(o => o.Id == id);
    }

    public IQueryable<OrderItem> QueryWithDetails()
    {
        return _context.OrderItems.Include(o => o.Order).Include(o => o.Product).AsQueryable();
    }

    public void Remove(OrderItem orderItem)
    {
        _context.OrderItems.Remove(orderItem);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
