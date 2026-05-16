using Course_Work_Magazine.Models;

namespace Course_Work_Magazine.Repositories.IRepositories;

public interface IBasketRepository
{
    Task AddAsync(Basket basketItem);
    Task<Basket?> GetByIdAsync(int id);
    Task<IEnumerable<Basket>> GetUserBasketAsync(string userId);
    void Remove(Basket basketItem);
    void RemoveRange(IEnumerable<Basket> basketItems);
    Task SaveChangesAsync();
    IQueryable<Basket> Query();
}
