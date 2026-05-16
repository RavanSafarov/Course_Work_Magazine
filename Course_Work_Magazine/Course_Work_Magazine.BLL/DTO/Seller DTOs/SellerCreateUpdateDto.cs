namespace Course_Work_Magazine.DTO.Customer_DTOs;

public class SellerCreateUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}
