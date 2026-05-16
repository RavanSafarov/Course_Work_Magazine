using Course_Work_Magazine.Models;

namespace Course_Work_Magazine.Repositories.IRepositories;

public interface IOrderItemRepository
{
    Task<OrderItem?> GetByIdAsync(int id);
    Task<OrderItem?> GetByIdWithDetailsAsync(int id); 
    IQueryable<OrderItem> QueryWithDetails();
    Task<IEnumerable<OrderItem>> GetAllAsync(bool includeArchived = false);
    Task AddAsync(OrderItem orderItem);
    void Remove(OrderItem orderItem);
    Task SaveChangesAsync();
}
