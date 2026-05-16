using AutoMapper;
using Course_Work_Magazine.Data;
using Course_Work_Magazine.DTO.OrderItem_DTOs;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Course_Work_Magazine.Models.Order;

namespace Course_Work_Magazine.Services;

public class OrderItemService : IOrderItemService
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IMapper _mapper;

    public OrderItemService(IOrderItemRepository orderItemRepository, IMapper mapper)
    {
        _orderItemRepository = orderItemRepository;
        _mapper = mapper;
    }

    public async Task<OrderItemReadDto?> CreateAsync(OrderItemCreateUpdateDto dto)
    {
        
        var orderItem = _mapper.Map<OrderItem>(dto);
        orderItem.Sum = dto.Quantity * dto.Rate;

        await _orderItemRepository.AddAsync(orderItem);
        await _orderItemRepository.SaveChangesAsync();

       
        var orderItemWithDetails = await _orderItemRepository.GetByIdWithDetailsAsync(orderItem.Id);

        return _mapper.Map<OrderItemReadDto>(orderItemWithDetails);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var orderItem = await _orderItemRepository.GetByIdWithDetailsAsync(id);
        if (orderItem == null || orderItem.Order == null || orderItem.Order.Status != Order.OrderStatus.Created)
        {
            return false;
        }    
            

        _orderItemRepository.Remove(orderItem);

        
        await _orderItemRepository.SaveChangesAsync();

        
        orderItem.Order.TotalSum = orderItem.Order.OrderItems.Where(oi => oi.Id != id).Sum(oi => oi.Sum);

        await _orderItemRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<OrderItemReadDto>> GetAllAsync()
    {
        var items = await _orderItemRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OrderItemReadDto>>(items);
    }

    public async Task<OrderItemReadDto?> GetByIdAsync(int id)
    {
        var orderItem = await _orderItemRepository.GetByIdWithDetailsAsync(id);
        if (orderItem == null)
        {
            return null; 
        }

        return _mapper.Map<OrderItemReadDto>(orderItem);
    }

    public async Task<OrderItemReadDto?> UpdateAsync(int id, OrderItemCreateUpdateDto dto)
    {
        var orderItem = await _orderItemRepository.GetByIdWithDetailsAsync(id);
        if (orderItem == null || orderItem.Order == null || orderItem.Order.Status != OrderStatus.Created)
        {
            return null;
        }
            
        _mapper.Map(dto, orderItem);
        orderItem.Sum = dto.Quantity * dto.Rate;

        await _orderItemRepository.SaveChangesAsync();

        
        orderItem.Order.TotalSum = orderItem.Order.OrderItems.Sum(oi => oi.Sum);
        await _orderItemRepository.SaveChangesAsync();

        return _mapper.Map<OrderItemReadDto>(orderItem);
    }
}

