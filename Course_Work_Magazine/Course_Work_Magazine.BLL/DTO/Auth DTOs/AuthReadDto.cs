namespace Course_Work_Magazine.DTO.Auth_DTOs;

public class AuthReadDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime ExpiredAt { get; set; }
    public DateTime RefreshTokenExpiredAt { get; set; }
    public string? AvatarUrl { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
