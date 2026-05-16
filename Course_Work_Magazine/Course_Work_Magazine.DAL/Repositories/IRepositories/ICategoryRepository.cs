using Course_Work_Magazine.Models;

namespace Course_Work_Magazine.Repositories.IRepositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task<Category?> GetByIdAsync(int id);
    IQueryable<Category> Query(); 
    void Remove(Category category);
    Task SaveChangesAsync();
}
