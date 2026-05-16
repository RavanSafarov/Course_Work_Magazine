using Microsoft.AspNetCore.Identity;

namespace Course_Work_Magazine.Models;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public decimal Balance { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; } = null!;

    public List<Seller> Sellers { get; set; } = new();
    public List<Basket> Baskets { get; set; } = new();
    
}
