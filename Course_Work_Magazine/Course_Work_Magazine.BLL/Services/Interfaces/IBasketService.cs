using Course_Work_Magazine.DTO.Basket_DTOs;


namespace Course_Work_Magazine.Services.Interfaces;

public interface IBasketService
{
    Task<BasketReadDto> AddToBasketAsync(string userId, BasketCreateUpdateDto dto);
    Task<IEnumerable<BasketReadDto>> GetUserBasketAsync(string userId);
    Task<bool> RemoveFromBasketAsync(int id, string userId);
    Task<BasketReadDto> UpdateQuantityAsync(int id, BasketCreateUpdateDto dto, string userId);
}
