using Course_Work_Magazine.Models;

namespace Course_Work_Magazine.Repositories.IRepositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);                  
    Task<Product?> GetByIdWithDetailsAsync(int id);
    IQueryable<Product> Query();
    Task AddAsync(Product product);                       
    void Remove(Product product);                         
    Task SaveChangesAsync();                              
}
