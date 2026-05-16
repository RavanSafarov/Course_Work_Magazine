using AutoMapper;
using Course_Work_Magazine.Common;
using Course_Work_Magazine.Data;
using Course_Work_Magazine.DTO.Product_DTOs;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Course_Work_Magazine.Models.Product;

namespace Course_Work_Magazine.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductReadDto> CreateAsync(ProductCreateUpdateDto dto)
    {
        var product = _mapper.Map<Product>(dto);

        if (product.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative");
        }


        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return _mapper.Map<ProductReadDto>(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return false;
        }

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ProductReadDto>> GetAllAsync()
    {
        var products = await _productRepository.Query().ToListAsync();

        return _mapper.Map<IEnumerable<ProductReadDto>>(products);
    }

    public async Task<ProductReadDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return null;
        }

        return _mapper.Map<ProductReadDto>(product);
    }

    public async Task<PagedResult<ProductReadDto>> GetPagedAsync(ProductQueryParams queryParams)
    {
        var query = _productRepository.Query();

        if (!string.IsNullOrWhiteSpace(queryParams.Sort))
        {
            var isAsc = queryParams.SortDirection?.ToLower() == "asc";

            query = queryParams.Sort.ToLower() switch
            {
                "name" => isAsc
                    ? query.OrderBy(i => i.NameOfProduct)
                    : query.OrderByDescending(i => i.NameOfProduct),

                "price" => isAsc
                    ? query.OrderBy(i => i.Price)
                    : query.OrderByDescending(i => i.Price),

                _ => query.OrderBy(i => i.Id)
            };
        }
        else
        {
            query = query.OrderBy(i => i.Id);
        }

        var totalCount = await query.CountAsync();

        var items = await query.Skip((queryParams.Page - 1) * queryParams.PageSize).Take(queryParams.PageSize).ToListAsync();

        var dtoItems = _mapper.Map<IEnumerable<ProductReadDto>>(items);

        return PagedResult<ProductReadDto>.Create(
            dtoItems,
            queryParams.Page,
            queryParams.PageSize,
            totalCount
        );
    }

    public async Task<ProductReadDto?> UpdateAsync(int id, ProductCreateUpdateDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return null;
        }


        _mapper.Map(dto, product);

        if (product.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative");
        }


        await _productRepository.SaveChangesAsync();

        return _mapper.Map<ProductReadDto>(product);
    }
}