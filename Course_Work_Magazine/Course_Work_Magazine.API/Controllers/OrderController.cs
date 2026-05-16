using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Order_DTOs;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Course_Work_Magazine.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin, Manager, User")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderReadDto>>>> GetAll([FromQuery] bool includeArchived = false)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

        var orders = await _orderService.GetAllAsync(includeArchived);

        if (!isAdminOrManager && !string.IsNullOrEmpty(userId))
        {
            orders = orders.Where(o => o.UserId == userId);
        }

        return Ok(ApiResponse<IEnumerable<OrderReadDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin, Manager, User")]
    public async Task<ActionResult<ApiResponse<PagedResult<OrderReadDto>>>> GetPaged([FromQuery] OrderQueryParams orderQueryParams)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

        if (!isAdminOrManager && !string.IsNullOrEmpty(userId))
        {
            orderQueryParams.UserId = userId;
        }

        var orders = await _orderService.GetPagedAsync(orderQueryParams);
        return Ok(ApiResponse<PagedResult<OrderReadDto>>.SuccessResponse(orders, "Orders retrieved successfully"));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin, Manager, User")]
    public async Task<ActionResult<ApiResponse<OrderReadDto>>> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order is null)
        {
            return NotFound(ApiResponse<OrderReadDto>.ErrorResponse($"Order with ID {id} was not found"));
        }
        return Ok(ApiResponse<OrderReadDto>.SuccessResponse(order, "Order retrieved successfully"));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin, Manager, User")]
    public async Task<ActionResult<ApiResponse<OrderReadDto>>> Create([FromBody] OrderCreateUpdateDto orderCreate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<OrderReadDto>.ErrorResponse("Invalid request data"));
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var createdOrder = await _orderService.CreateAsync(orderCreate, userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdOrder.Id },
            ApiResponse<OrderReadDto>.SuccessResponse(createdOrder, "Order created successfully")
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<ActionResult<ApiResponse<OrderReadDto>>> Update(int id, [FromBody] OrderCreateUpdateDto orderUpdate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<OrderReadDto>.ErrorResponse("Invalid request data"));
        }

        var updatedOrder = await _orderService.UpdateAsync(id, orderUpdate);
        if (updatedOrder is null)
        {
            return NotFound(ApiResponse<OrderReadDto>.ErrorResponse($"Order with ID {id} not found"));
        }

        return Ok(ApiResponse<OrderReadDto>.SuccessResponse(updatedOrder, "Order updated successfully"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<OrderReadDto>>> Delete(int id)
    {
        var isDeleted = await _orderService.DeleteAsync(id);
        if (!isDeleted)
        {
            return NotFound(ApiResponse<OrderReadDto>.ErrorResponse($"Order with ID {id} not found"));
        }
        return Ok(ApiResponse<OrderReadDto>.SuccessResponse(null!, "Order deleted successfully"));
    }

    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<ActionResult<ApiResponse<OrderReadDto>>> ChangeStatus(int id, [FromQuery] Order.OrderStatus newStatus)
    {
        var order = await _orderService.ChangeStatusAsync(id, newStatus);
        if (order is null)
        {
            return NotFound(ApiResponse<OrderReadDto>.ErrorResponse($"Order with ID {id} not found"));
        }
        return Ok(ApiResponse<OrderReadDto>.SuccessResponse(order, "Order status updated successfully"));
    }

    [HttpPatch("{id}/archive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Manager, Admin")]
    public async Task<ActionResult<ApiResponse<OrderReadDto>>> Archive(int id)
    {
        var result = await _orderService.ArchiveAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse<OrderReadDto>.ErrorResponse($"Order with ID {id} not found"));
        }
        return Ok(ApiResponse<OrderReadDto>.SuccessResponse(null!, "Order archived successfully"));
    }

    [HttpPost("checkout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin, Manager, User")]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = await _orderService.CheckoutAsync(userId!);

        if (order == null)
            return BadRequest("Basket is empty");

        return Ok(order);
    }
}
