using Course_Work_Magazine.Models;

namespace Course_Work_Magazine.Repositories.IRepositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByIdWithDetailsAsync(int id);
    IQueryable<Order> QueryWithDetails();
    Task<IEnumerable<Order>> GetAllAsync(bool includeArchived);
    Task AddAsync(Order order);
    Task AddRangeAsync(IEnumerable<OrderItem> items);
    void Remove(Order order);
    Task SaveChangesAsync();
}
