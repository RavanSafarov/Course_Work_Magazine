using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Customer_DTOs;
using Course_Work_Magazine.DTO.OrderItem_DTOs;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Course_Work_Magazine.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class SellerController : ControllerBase
{
    private readonly ISellerService _sellerService;
    public SellerController(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SellerReadDto>>>> GetAll()
    {
        var seller = await _sellerService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<SellerReadDto>>.SuccessResponse(seller, "Sellers retrieved successfully"));
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<PagedResult<SellerReadDto>>>> GetPaged([FromQuery] SellerQueryParams sellerQueryParams)
    {
        var seller = await _sellerService.GetPagedAsync(sellerQueryParams);
        return Ok(ApiResponse<PagedResult<SellerReadDto>>.SuccessResponse(seller, "Sellers retrieved successfully"));
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SellerReadDto>> Create([FromBody] SellerCreateUpdateDto dto)
    {
        
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not found");
        }    
            

        var seller = await _sellerService.CreateAsync(dto, userId);

        return Ok(seller);
    }
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<SellerReadDto>>> GetById(int id)
    {
        var seller = await _sellerService.GetByIdAsync(id);
        if (seller is null)
        {
            return NotFound(ApiResponse<SellerReadDto>.ErrorResponse($"Seller with this ID {id} was not founded!!!"));
        }
        return Ok(ApiResponse<SellerReadDto>.SuccessResponse(seller, "Seller was found successfully"));
    }
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SellerReadDto>>> Delete(int id)
    {
        var isDeleted = await _sellerService.DeleteAsync(id);
        if (!isDeleted)
        {
            return NotFound(ApiResponse<SellerReadDto>.ErrorResponse($"Seller with this ID {id} was not found!!!"));
        }
        return Ok(ApiResponse<SellerReadDto>.SuccessResponse(null, "Seller was deleted successfully"));
    }
    [HttpPatch("{id}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SellerReadDto>>> Archive(int id)
    {
        var result = await _sellerService.ArchiveAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse<SellerReadDto>.ErrorResponse($"Seller with ID {id} not found"));
        }
        return Ok(ApiResponse<SellerReadDto>.SuccessResponse(null, "Seller archived successfully"));
    }
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<SellerReadDto>>> Update(
     int id,
     [FromBody] SellerCreateUpdateDto sellerCreateUpdate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ApiResponse<SellerReadDto>.ErrorResponse("Invalid request data")
            );
        }

        var updatedSeller = await _sellerService.UpdateAsync(id, sellerCreateUpdate);

        if (updatedSeller is null)
        {
            return NotFound(
                ApiResponse<SellerReadDto>.ErrorResponse($"Seller with ID {id} not found")
            );
        }

        return Ok(
           ApiResponse<SellerReadDto>.SuccessResponse(updatedSeller, "Seller updated successfully")
       );
    }

}
