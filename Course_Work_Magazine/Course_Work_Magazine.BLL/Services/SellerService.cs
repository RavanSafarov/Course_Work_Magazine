using AutoMapper;
using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Customer_DTOs;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Course_Work_Magazine.Models.Order;

namespace Course_Work_Magazine.Services;

public class SellerService : ISellerService
{
    private readonly ISellerRepository _sellerRepository;
    private readonly IMapper _mapper;

    public SellerService(ISellerRepository sellerRepository, IMapper mapper)
    {
        _sellerRepository = sellerRepository;
        _mapper = mapper;
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        var seller = await _sellerRepository.GetByIdAsync(id);
        if (seller is null)
            return false;

        seller.DeletedAt = DateTimeOffset.UtcNow;
        await _sellerRepository.SaveChangesAsync();

        return true;
    }

    public async Task<SellerReadDto> CreateAsync(SellerCreateUpdateDto dto, string userId)
    {
        
        var seller = _mapper.Map<Seller>(dto);

        
        seller.UserId = userId;

        await _sellerRepository.AddAsync(seller);
        await _sellerRepository.SaveChangesAsync();

        return _mapper.Map<SellerReadDto>(seller);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var seller = await _sellerRepository.GetByIdWithDetailsAsync(id);

        if (seller is null)
            return false;

        if (seller.Orders.Any(o => o.Status != OrderStatus.Created))
            return false;

        _sellerRepository.Remove(seller);
        await _sellerRepository.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<SellerReadDto>> GetAllAsync()
    {
        var sellers = await _sellerRepository.Query()
            .Where(s => s.DeletedAt == null)
            .ToListAsync();

        return _mapper.Map<IEnumerable<SellerReadDto>>(sellers);
    }

    public async Task<SellerReadDto?> GetByIdAsync(int id)
    {
        var seller = await _sellerRepository.GetByIdWithDetailsAsync(id);

        if (seller is null)
            return null;

        return _mapper.Map<SellerReadDto>(seller);
    }

    public async Task<PagedResult<SellerReadDto>> GetPagedAsync(SellerQueryParams queryParams)
    {
        var query = _sellerRepository.Query();

        if (!queryParams.IncludeArchived)
            query = query.Where(s => s.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(queryParams.SearchByName))
        {
            var search = queryParams.SearchByName.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(search));
        }

        query = query.OrderBy(s => s.Id);

        var totalCount = await query.CountAsync();

        var sellers = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        var dtoItems = _mapper.Map<IEnumerable<SellerReadDto>>(sellers);

        return PagedResult<SellerReadDto>.Create(
            dtoItems,
            queryParams.Page,
            queryParams.PageSize,
            totalCount);
    }

    public async Task<SellerReadDto?> UpdateAsync(int id, SellerCreateUpdateDto dto)
    {
        var seller = await _sellerRepository.GetByIdAsync(id);
        if (seller is null)
            return null;

        _mapper.Map(dto, seller);
        seller.UpdatedAt = DateTimeOffset.UtcNow;

        await _sellerRepository.SaveChangesAsync();

        return _mapper.Map<SellerReadDto>(seller);
    }
}
