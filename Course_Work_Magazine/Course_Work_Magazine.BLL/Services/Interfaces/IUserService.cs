using Course_Work_Magazine.DTO.Auth_DTOs;
using Microsoft.AspNetCore.Identity.Data;

namespace Course_Work_Magazine.Services.Interfaces;

public interface IUserService
{
    Task<AuthReadDto> RegisterAsync(RegisterRequestDto registerRequest);
    Task<AuthReadDto> LoginAsync(LoginRequestDto loginRequest);
    Task<AuthReadDto?> GetProfileAsync(string userId);
    Task<AuthReadDto?> UpdateProfileAsync(string userId, UpdateUserDto dto);
    Task<AuthReadDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
    Task RevokeRefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
    Task<AuthReadDto?> UpdateBalanceAsync(string userId, decimal newBalance);
}