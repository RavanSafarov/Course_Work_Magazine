using AutoMapper;
using Course_Work_Magazine.Data;
using Course_Work_Magazine.DTO.Category_DTOs;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Course_Work_Magazine.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryReadDto?> CreateCategoryAsync(CategoryCreateUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NameOfCategory))
        {
            return null;
        }
        var exists = await _categoryRepository.Query().AnyAsync(c => c.NameOfCategory!.ToLower() == dto.NameOfCategory.ToLower());

        if (exists)
        {
            throw new Exception("Category with this name already exists");
        }
            

        var category = _mapper.Map<Category>(dto);

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return _mapper.Map<CategoryReadDto>(category);
    }

    public async Task<IEnumerable<CategoryReadDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.Query().ToListAsync();
        return _mapper.Map<IEnumerable<CategoryReadDto>>(categories);
    }

    public async Task<CategoryReadDto?> GetCategorieByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null) 
        {
            return null; 
        }

        return _mapper.Map<CategoryReadDto>(category);
    }

    public async Task<CategoryReadDto?> UpdateCategorieAsync(int id, CategoryCreateUpdateDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null || string.IsNullOrWhiteSpace(dto.NameOfCategory))
            return null;

        
        var exists = await _categoryRepository.Query().AnyAsync(c => c.Id != id && c.NameOfCategory!.ToLower() == dto.NameOfCategory.ToLower());

        if (exists)
        {
            throw new Exception("Category with this name already exists");
        }
            

        _mapper.Map(dto, category);
        await _categoryRepository.SaveChangesAsync();

        return _mapper.Map<CategoryReadDto>(category);
    }

    public async Task<bool> DeleteCategorieAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return false;
        }
            
        var hasProducts = await _categoryRepository.Query().Include(c => c.Products).Where(c => c.Id == id).AnyAsync(c => c.Products.Any());

        if (hasProducts)
        {
            return false;
        }
            

        _categoryRepository.Remove(category);
        await _categoryRepository.SaveChangesAsync();

        return true;
    }
}



