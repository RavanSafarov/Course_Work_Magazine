using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.OrderItem_DTOs;
using Course_Work_Magazine.Services;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course_Work_Magazine.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderItemController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;
    public OrderItemController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin, Manager, User")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderItemReadDto>>>> GetAll()
    {
        var rows = await _orderItemService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<OrderItemReadDto>>.SuccessResponse(rows, "OrderItem retrieved successfully"));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin, Manager, User")]
    public async Task<ActionResult<ApiResponse<OrderItemReadDto>>> GetById(int id)
    {
        var row = await _orderItemService.GetByIdAsync(id);
        if (row is null)
        {
            return NotFound(ApiResponse<OrderItemReadDto>.ErrorResponse($"OrderItem with ID {id} not found"));
        }
        return Ok(ApiResponse<OrderItemReadDto>.SuccessResponse(row, "OrderItem retrieved successfully"));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Manager,User")]
    public async Task<ActionResult<ApiResponse<OrderItemReadDto>>> Create(
    [FromBody] OrderItemCreateUpdateDto rowCreate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ApiResponse<OrderItemReadDto>.ErrorResponse("Invalid request data"));
        }

        var createdRow = await _orderItemService.CreateAsync(rowCreate);

        if (createdRow is null)
        {
            return BadRequest(
                ApiResponse<OrderItemReadDto>
                    .ErrorResponse("Order not found or order is not in Created status"));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdRow.Id },
            ApiResponse<OrderItemReadDto>
                .SuccessResponse(createdRow, "OrderItem created successfully")
        );
    }
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<ApiResponse<OrderItemReadDto>>> Update(int id, [FromBody] OrderItemCreateUpdateDto rowUpdate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<OrderItemReadDto>.ErrorResponse("Invalid request data"));
        }

        var updatedRow = await _orderItemService.UpdateAsync(id, rowUpdate);
        if (updatedRow is null)
        {
            return NotFound(ApiResponse<OrderItemReadDto>.ErrorResponse($"OrderItem with ID {id} not found"));
        }

        return Ok(ApiResponse<OrderItemReadDto>.SuccessResponse(updatedRow, "OrderItem updated successfully"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<OrderItemReadDto>>> Delete(int id)
    {
        var isDeleted = await _orderItemService.DeleteAsync(id);
        if (!isDeleted)
        {
            return NotFound(ApiResponse<OrderItemReadDto>.ErrorResponse($"OrderItem with ID {id} not found"));
        }
        return Ok(ApiResponse<OrderItemReadDto>.SuccessResponse(null, "OrderItem deleted successfully"));
    }
}
