using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Product_DTOs;

namespace Course_Work_Magazine.Services.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductReadDto>> GetPagedAsync(ProductQueryParams productQueryParams);
    Task<ProductReadDto> CreateAsync(ProductCreateUpdateDto productCreateUpdateDto);
    Task<ProductReadDto> UpdateAsync(int id, ProductCreateUpdateDto productCreateUpdateDto);
    Task<bool> DeleteAsync(int id);
    Task<ProductReadDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductReadDto>> GetAllAsync();
}
