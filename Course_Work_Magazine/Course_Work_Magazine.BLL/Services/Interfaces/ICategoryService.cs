using Course_Work_Magazine.DTO.Category_DTOs;


namespace Course_Work_Magazine.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryReadDto?> CreateCategoryAsync(CategoryCreateUpdateDto categoryCreateUpdateDto);
    Task<IEnumerable<CategoryReadDto>> GetAllCategoriesAsync();
    Task<bool> DeleteCategorieAsync(int id);
    Task<CategoryReadDto?> GetCategorieByIdAsync(int id);
    Task<CategoryReadDto?> UpdateCategorieAsync(int id, CategoryCreateUpdateDto categoryCreateUpdateDto);
}
