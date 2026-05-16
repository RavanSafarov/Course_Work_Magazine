namespace Course_Work_Magazine.DTO.Product_DTOs;

public class ProductCreateUpdateDto
{
    public int Id { get; set; }
    public string? NameOfProduct { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public int SellerId { get; set; }
}
