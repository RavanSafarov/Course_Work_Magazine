namespace Course_Work_Magazine.DTO.OrderItem_DTOs;

public class OrderItemCreateUpdateDto
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public string? Service { get; set; } 
}
