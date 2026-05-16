namespace Course_Work_Magazine.Models;

public class Product
{
    public int Id { get; set; }
    public string NameOfProduct { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int SellerId { get; set; }
    public Seller Seller { get; set; } = null!;
   
}
