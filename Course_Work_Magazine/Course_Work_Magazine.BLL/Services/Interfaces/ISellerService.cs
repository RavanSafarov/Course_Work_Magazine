using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Customer_DTOs;

namespace Course_Work_Magazine.Services.Interfaces;

public interface ISellerService
{
    Task<PagedResult<SellerReadDto>> GetPagedAsync(SellerQueryParams sellerQueryParams);
    Task<SellerReadDto> CreateAsync(SellerCreateUpdateDto dto, string userId);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SellerReadDto>> GetAllAsync();
    Task<SellerReadDto> GetByIdAsync(int id);
    Task<SellerReadDto?> UpdateAsync(int id, SellerCreateUpdateDto sellerCreateUpdate);
    Task<bool> ArchiveAsync(int id);
}
