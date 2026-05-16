using Course_Work_Magazine.DTO.OrderItem_DTOs;

namespace Course_Work_Magazine.DTO.Order_DTOs;

public class OrderCreateUpdateDto
{
    public int SellerId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public List<OrderItemCreateUpdateDto> Items { get; set; } = new();
    public string? Comment { get; set; }
}
