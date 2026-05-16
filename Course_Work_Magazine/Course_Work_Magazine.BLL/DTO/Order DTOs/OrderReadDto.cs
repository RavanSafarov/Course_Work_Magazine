using Course_Work_Magazine.DTO.OrderItem_DTOs;

namespace Course_Work_Magazine.DTO.Order_DTOs;

public class OrderReadDto
{
    public int Id { get; set; }

    public int SellerId { get; set; }
    public string? SellerName { get; set; }

    public decimal TotalSum { get; set; }
    public string? Comment { get; set; }

    public string Status { get; set; } = "Created";

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public List<OrderItemReadDto> OrderItems { get; set; } = new();
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}
