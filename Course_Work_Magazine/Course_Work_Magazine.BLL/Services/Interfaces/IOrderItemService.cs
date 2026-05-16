using Course_Work_Magazine.DTO.OrderItem_DTOs;

namespace Course_Work_Magazine.Services.Interfaces;

public interface IOrderItemService
{
    Task<OrderItemReadDto?> CreateAsync(OrderItemCreateUpdateDto dto);
    Task<OrderItemReadDto?> UpdateAsync(int id, OrderItemCreateUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<OrderItemReadDto?> GetByIdAsync(int id);
    Task<IEnumerable<OrderItemReadDto>> GetAllAsync();
}
