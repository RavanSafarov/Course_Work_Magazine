using Course_Work_Magazine.Models;

namespace Course_Work_Magazine.Repositories.IRepositories;

public interface ISellerRepository
{
    Task SaveChangesAsync();
    Task AddAsync(Seller seller);
    void Remove(Seller seller);
    IQueryable<Seller> Query();
    Task<Seller?> GetByIdAsync(int id);
    Task<Seller?> GetByIdWithDetailsAsync(int id);
}
