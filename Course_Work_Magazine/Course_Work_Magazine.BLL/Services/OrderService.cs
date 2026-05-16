using AutoMapper;
using Course_Work_Magazine.Common;
using Course_Work_Magazine.Data;
using Course_Work_Magazine.DTO.Order_DTOs;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Course_Work_Magazine.Models.Order;

namespace Course_Work_Magazine.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBasketRepository _basketRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, IBasketRepository basketRepository, UserManager<User> userManager, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _basketRepository = basketRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
        {
            return false;
        }

        order.DeletedAt = DateTimeOffset.UtcNow;
        await _orderRepository.SaveChangesAsync();

        return true;
    }

    public async Task<OrderReadDto?> ChangeStatusAsync(int id, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null) return null;

        bool isSequential = newStatus == order.Status + 1;
        bool isCancelOrReject = newStatus == OrderStatus.Cancelled || newStatus == OrderStatus.Rejected;
        bool isFromActiveStatus = order.Status == OrderStatus.Created ||
                                  order.Status == OrderStatus.Sent ||
                                  order.Status == OrderStatus.Received;

        if (!isSequential && !(isCancelOrReject && isFromActiveStatus))
            return null;

        order.Status = newStatus;
        await _orderRepository.SaveChangesAsync();

        return _mapper.Map<OrderReadDto>(order);
    }

    public async Task<OrderReadDto?> CheckoutAsync(string userId)
    {
        var basketItems = await _basketRepository.Query().Where(b => b.UserId == userId).Include(b => b.Product).ToListAsync();
        if (!basketItems.Any())
        {
            return null;
        }

        var sellerIds = basketItems.Select(b => b.Product.SellerId).Distinct().ToList();

        var sellerId = sellerIds.First();

        var order = new Order
        {
            UserId = userId,
            SellerId = sellerId,
            Status = OrderStatus.Created,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        var orderItems = basketItems.Select(b => new OrderItem
        {
            OrderId = order.Id,
            ProductId = b.ProductId,
            Quantity = b.Quantity,
            Rate = b.Product.Price,
            Sum = b.Quantity * b.Product.Price
        }).ToList();
        await _orderRepository.AddRangeAsync(orderItems);

        order.TotalSum = orderItems.Sum(x => x.Sum);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        if (user.Balance < order.TotalSum)
        {
            throw new Exception("Not enough balance");
        }
        user.Balance -= order.TotalSum;
        await _userManager.UpdateAsync(user);
        await _orderRepository.SaveChangesAsync();

        _basketRepository.RemoveRange(basketItems);
        await _basketRepository.SaveChangesAsync();

        var orderWithDetails = await _orderRepository.GetByIdWithDetailsAsync(order.Id);
        return _mapper.Map<OrderReadDto>(orderWithDetails);
    }

    public async Task<OrderReadDto> CreateAsync(OrderCreateUpdateDto dto, string userId)
    {
        var order = _mapper.Map<Order>(dto);

        order.UserId = userId;
        order.TotalSum = 0;

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        if (dto.Items != null && dto.Items.Any())
        {
            var orderItems = dto.Items.Select(r =>
            {
                var item = _mapper.Map<OrderItem>(r);
                item.OrderId = order.Id;
                item.Sum = r.Quantity * r.Rate;
                return item;
            }).ToList();

            await _orderRepository.AddRangeAsync(orderItems);

            order.TotalSum = orderItems.Sum(i => i.Sum);

            await _orderRepository.SaveChangesAsync();
        }

        var orderWithDetails = await _orderRepository.GetByIdWithDetailsAsync(order.Id);

        return _mapper.Map<OrderReadDto>(orderWithDetails);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null) return false;

        if (order.Status != OrderStatus.Created)
            return false;

        _orderRepository.Remove(order);
        await _orderRepository.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<OrderReadDto>> GetAllAsync(bool includeArchived = false)
    {
        var orders = await _orderRepository.GetAllAsync(includeArchived);
        return _mapper.Map<IEnumerable<OrderReadDto>>(orders);
    }

    public async Task<OrderReadDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(id);
        if (order is null) return null;

        return _mapper.Map<OrderReadDto>(order);
    }

    public async Task<PagedResult<OrderReadDto>> GetPagedAsync(OrderQueryParams orderQueryParams)
    {
        var query = _orderRepository.QueryWithDetails();

        if (orderQueryParams.StartDateFrom.HasValue)
            query = query.Where(o => o.StartDate >= orderQueryParams.StartDateFrom.Value);

        if (orderQueryParams.StartDateTo.HasValue)
            query = query.Where(o => o.StartDate <= orderQueryParams.StartDateTo.Value);

        if (!string.IsNullOrWhiteSpace(orderQueryParams.Sort))
        {
            var isAsc = orderQueryParams.SortDirection?.ToLower() == "asc";

            query = orderQueryParams.Sort.ToLower() switch
            {
                "createdat" => isAsc ? query.OrderBy(i => i.CreatedAt) : query.OrderByDescending(i => i.CreatedAt),
                "updatedat" => isAsc ? query.OrderBy(i => i.UpdatedAt) : query.OrderByDescending(i => i.UpdatedAt),
                "startdate" => isAsc ? query.OrderBy(i => i.StartDate) : query.OrderByDescending(i => i.StartDate),
                "enddate" => isAsc ? query.OrderBy(i => i.EndDate) : query.OrderByDescending(i => i.EndDate),
                "status" => isAsc ? query.OrderBy(i => i.Status) : query.OrderByDescending(i => i.Status),
                _ => query
            };
        }
        else
        {
            query = query.OrderBy(i => i.Id);
        }

        var totalCount = await query.CountAsync();

        var orders = await query
            .Skip((orderQueryParams.Page - 1) * orderQueryParams.PageSize)
            .Take(orderQueryParams.PageSize)
            .ToListAsync();

        var dtoItems = _mapper.Map<IEnumerable<OrderReadDto>>(orders);

        return PagedResult<OrderReadDto>.Create(
            dtoItems,
            orderQueryParams.Page,
            orderQueryParams.PageSize,
            totalCount
        );
    }

    public async Task<OrderReadDto?> UpdateAsync(int id, OrderCreateUpdateDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null) return null;

        if (order.Status != OrderStatus.Created)
            return null;

        _mapper.Map(dto, order);
        order.UpdatedAt = DateTimeOffset.UtcNow;

        await _orderRepository.SaveChangesAsync();

        return _mapper.Map<OrderReadDto>(order);
    }
}