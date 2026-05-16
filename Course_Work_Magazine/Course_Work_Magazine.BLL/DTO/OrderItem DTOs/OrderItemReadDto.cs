namespace Course_Work_Magazine.DTO.OrderItem_DTOs;

public class OrderItemReadDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public string? ProductName { get; set; }

    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Sum { get; set; }
    public string Service { get; set; } = string.Empty;
}
