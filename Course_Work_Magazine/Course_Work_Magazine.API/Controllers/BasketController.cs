using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Basket_DTOs;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Course_Work_Magazine.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User,Manager,Admin")]
public class BasketController : ControllerBase
{
    public readonly IBasketService _basketService;
    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }
    private string GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return userId;
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BasketReadDto>>> AddToBasket([FromBody] BasketCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<BasketReadDto>.ErrorResponse("Invalid request data"));
        }

        var userId = GetUserId();

        var result = await _basketService.AddToBasketAsync(userId, dto);

        if (result is null)
        {
            return NotFound(ApiResponse<BasketReadDto>.ErrorResponse("Product not found"));
        }

        return Created("", ApiResponse<BasketReadDto>.SuccessResponse(result, "Product added to basket"));
    }



    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BasketReadDto>>>> GetUserBasket()
    {
        var userId = GetUserId();

        var basket = await _basketService.GetUserBasketAsync(userId);

        return Ok(ApiResponse<IEnumerable<BasketReadDto>>.SuccessResponse(basket, "Basket retrieved successfully"));
    }


    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BasketReadDto>>> UpdateQuantity(int id, [FromBody] BasketCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<BasketReadDto>.ErrorResponse("Invalid request data"));
        }
           

        var userId = GetUserId();

        var updated = await _basketService.UpdateQuantityAsync(id, dto, userId);

        if (updated is null)
        {
            return NotFound(ApiResponse<BasketReadDto>.ErrorResponse("Basket item not found"));
        }
           

        return Ok(ApiResponse<BasketReadDto>.SuccessResponse(updated, "Quantity updated successfully"));
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<string>>> RemoveFromBasket(int id)
    {
        var userId = GetUserId();

        var removed = await _basketService.RemoveFromBasketAsync(id, userId);

        if (!removed)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("Basket item not found"));
        }
            
        return Ok(ApiResponse<string>.SuccessResponse(null, "Item removed from basket"));
    }
}
