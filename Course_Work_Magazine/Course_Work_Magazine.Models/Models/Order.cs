namespace Course_Work_Magazine.Models;

public class Order
{
    public int Id { get; set; }

    public int SellerId { get; set; }
    public Seller Seller { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }  = DateTimeOffset.UtcNow;
    public DateTimeOffset EndDate { get; set; } = DateTimeOffset.UtcNow;

    public decimal TotalSum { get; set; }
    public string? Comment { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Created;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();

    public enum OrderStatus
    {
        Created,
        Sent,
        Received,
        Paid,
        Cancelled,
        Rejected
    }

}
