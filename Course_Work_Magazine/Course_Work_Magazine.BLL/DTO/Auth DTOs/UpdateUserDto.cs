namespace Course_Work_Magazine.DTO.Auth_DTOs;

public class UpdateUserDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }

    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public decimal? Balance { get; set; }
}