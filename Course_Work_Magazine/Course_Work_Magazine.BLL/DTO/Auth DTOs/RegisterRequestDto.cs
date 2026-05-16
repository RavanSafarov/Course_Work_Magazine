namespace Course_Work_Magazine.DTO.Auth_DTOs;

public class RegisterRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmedPassword { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}
