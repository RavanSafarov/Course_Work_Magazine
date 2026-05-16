using AutoMapper;
using Course_Work_Magazine.Data;
using Course_Work_Magazine.DTO.Basket_DTOs;
using Course_Work_Magazine.DTO.Product_DTOs;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Services;

public class BasketService : IBasketService
{
   
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;
    public BasketService(IBasketRepository basketRepository, IMapper maper)
    {
        _basketRepository = basketRepository;
        _mapper = maper;
    }

    public async Task<BasketReadDto?> AddToBasketAsync(string userId, BasketCreateUpdateDto dto)
    {
        if (dto.Quantity <= 0)
            throw new Exception("Quantity must be higher than 0");

        var basket =  _basketRepository.Query()
            .FirstOrDefault(b => b.ProductId == dto.ProductId && b.UserId == userId);

        if (basket != null)
        {
            basket.Quantity += dto.Quantity;
        }
        else
        {
            basket = new Basket
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UserId = userId
            };

            await _basketRepository.AddAsync(basket);
        }

        await _basketRepository.SaveChangesAsync();

       
        var basketWithProduct = await _basketRepository.GetByIdAsync(basket.Id);

        
        return _mapper.Map<BasketReadDto>(basketWithProduct);
    }

    public async Task<IEnumerable<BasketReadDto>> GetUserBasketAsync(string userId)
    {
        var baskets = await _basketRepository.GetUserBasketAsync(userId);
        return _mapper.Map<IEnumerable<BasketReadDto>>(baskets);
    }

    public async Task<bool> RemoveFromBasketAsync(int id, string userId)
    {
        var basket = await _basketRepository.GetByIdAsync(id);

        if (basket is null || basket.UserId != userId)
        {
            return false;
        }
           
        _basketRepository.Remove(basket);
        await _basketRepository.SaveChangesAsync();

        return true;
    }

    public async Task<BasketReadDto> UpdateQuantityAsync(int id, BasketCreateUpdateDto dto, string userId)
    {
        if (dto.Quantity <= 0)
        {
            throw new Exception("Quantity must be higher than 0");
        }
            
        var basket = await _basketRepository.GetByIdAsync(id);

        if (basket is null || basket.UserId != userId)
        {
            return null;
        }

        basket.Quantity = dto.Quantity;

        await _basketRepository.SaveChangesAsync();

        return _mapper.Map<BasketReadDto>(basket);
    }
}

