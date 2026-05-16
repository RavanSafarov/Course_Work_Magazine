using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Category_DTOs;
using Course_Work_Magazine.DTO.Order_DTOs;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course_Work_Magazine.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryReadDto>>>> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(ApiResponse<IEnumerable<CategoryReadDto>>.SuccessResponse(categories, "Categories retrieved successfully"));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CategoryReadDto>>> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategorieByIdAsync(id);

        if (category is null)
        {
            return NotFound(ApiResponse<CategoryReadDto>.ErrorResponse($"Category with ID {id} was not found"));
        }
           

        return Ok(ApiResponse<CategoryReadDto>.SuccessResponse(category, "Category retrieved successfully"));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CategoryReadDto>>> CreateCategory(
        [FromBody] CategoryCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CategoryReadDto>.ErrorResponse("Invalid request data"));

        }

        var createdCategory = await _categoryService.CreateCategoryAsync(dto);

        if (createdCategory is null)
        {
            return BadRequest(ApiResponse<CategoryReadDto>.ErrorResponse("Invalid category data"));
        }
            

        return CreatedAtAction(
            nameof(GetCategoryById),
            new { id = createdCategory.Id },
            ApiResponse<CategoryReadDto>.SuccessResponse(createdCategory, "Category created successfully"));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CategoryReadDto>>> UpdateCategory(int id, [FromBody] CategoryCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CategoryReadDto>.ErrorResponse("Invalid request data"));
        }
     
        var updatedCategory = await _categoryService.UpdateCategorieAsync(id, dto);

        if (updatedCategory is null)
        {
            return NotFound(ApiResponse<CategoryReadDto>.ErrorResponse($"Category with ID {id} not found or invalid data"));
        }
           

        return Ok(ApiResponse<CategoryReadDto>.SuccessResponse(updatedCategory, "Category updated successfully"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CategoryReadDto>>> DeleteCategory(int id)
    {
        var deleted = await _categoryService.DeleteCategorieAsync(id);

        if (!deleted)
        {
            return BadRequest(ApiResponse<CategoryReadDto>
                .ErrorResponse("Category not found or contains products"));
        }
        return Ok(ApiResponse<CategoryReadDto>.SuccessResponse(null, "Category deleted successfully"));
    }
}
