using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Order_DTOs;
using Course_Work_Magazine.Models;

namespace Course_Work_Magazine.Services.Interfaces;

public interface IOrderService
{
    Task<PagedResult<OrderReadDto>> GetPagedAsync(OrderQueryParams orderQueryParams);
    Task<OrderReadDto> CreateAsync(OrderCreateUpdateDto dto, string userId);
    Task<OrderReadDto?> UpdateAsync(int id, OrderCreateUpdateDto dto);
    Task<OrderReadDto?> ChangeStatusAsync(int id, Order.OrderStatus newStatus);
    Task<bool> DeleteAsync(int id);
    Task<bool> ArchiveAsync(int id);
    Task<OrderReadDto?> GetByIdAsync(int id);
    Task<OrderReadDto?> CheckoutAsync(string userId);
    Task<IEnumerable<OrderReadDto>> GetAllAsync(bool includeArchived = false);
}
