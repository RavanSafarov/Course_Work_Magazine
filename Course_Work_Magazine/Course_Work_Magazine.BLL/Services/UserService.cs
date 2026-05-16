using AutoMapper;
using Course_Work_Magazine.Config;
using Course_Work_Magazine.DTO.Auth_DTOs;
using Course_Work_Magazine.Models;
using Course_Work_Magazine.Repositories.IRepositories;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Course_Work_Magazine.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtConfig _jwtConfig;

    private const string RefreshTokenType = "refresh";
    public UserService(UserManager<User> userManager, IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository, JwtConfig jwtConfig)
    {
        _userManager = userManager;
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtConfig = jwtConfig;

    }
    public async Task<AuthReadDto?> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }
        var roles = await _userManager.GetRolesAsync(user);


        return new AuthReadDto
        {
            Email = user.Email!,
            AvatarUrl = user.AvatarUrl,
            Balance = user.Balance,
            Roles = roles
        };
    }

    public async Task<AuthReadDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user is null)
        {
            return null!;
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (!isValidPassword)
        {
            return null!;
        }

        return await GenerateTokenAsync(user);
    }

    public async Task<AuthReadDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
    {
        var (principial, jti) = ValidateRefreshJwtAndGetJti(refreshTokenRequest.RefreshToken);

        var storedToken = await _refreshTokenRepository.GetByJwtIdAsync(jti);
        if (storedToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token");
        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token has been revoked or expired");

        var userId = principial.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user is null)
            throw new UnauthorizedAccessException("User not found");

        storedToken.RevokedAt = DateTime.UtcNow;

        var newTokens = await GenerateTokenAsync(user);

        var newStoredToken = await _refreshTokenRepository.GetByJwtIdAsync(GetJtiFromRefreshToken(newTokens.RefreshToken));
        if (newStoredToken is not null)
            storedToken.ReplacedByJwtId = newStoredToken.JwtId;

        await _refreshTokenRepository.UpdateAsync(storedToken);
        return newTokens;
    }

    public async Task<AuthReadDto> RegisterAsync(RegisterRequestDto registerRequest)
    {

        if (registerRequest.Password != registerRequest.ConfirmedPassword)
        {
            throw new InvalidOperationException("Passwords do not match");
        }

        var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User
        {
            UserName = registerRequest.Email,
            Email = registerRequest.Email,
            Name = registerRequest.Name,
            AvatarUrl = registerRequest.AvatarUrl,
            Balance = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(",", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }
        if (registerRequest.Email == "admin@orderflow.com" && registerRequest.Password == "Admin123!")
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        else if (registerRequest.Email == "manager@orderflow.com" && registerRequest.Password == "Manager123!")
        {
            await _userManager.AddToRoleAsync(user, "Manager");
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "User");
        }
        return await GenerateTokenAsync(user);
    }

    public async Task RevokeRefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
    {
        string? jti;
        try
        {
            (_, jti) = ValidateRefreshJwtAndGetJti(refreshTokenRequest.RefreshToken, validateLifeTime: false);
        }
        catch
        {
            return;
        }

        var storedToken = await _refreshTokenRepository.GetByJwtIdAsync(jti);
        if (storedToken is null || !storedToken.IsActive) return;

        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(storedToken);
    }
    private (ClaimsPrincipal principial, string jti) ValidateRefreshJwtAndGetJti(string refreshToken, bool validateLifeTime = true)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey!));

        var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = validateLifeTime,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfig.Issuer,
            ValidAudience = _jwtConfig.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwt)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var tokenType = jwt.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
        if (tokenType != RefreshTokenType)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        return (principal, jti);
    }

    public async Task<AuthReadDto?> UpdateProfileAsync(string userId, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }
        if (dto.Balance.HasValue)
        {
            user.Balance = dto.Balance.Value;
        }
        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, dto.Email);
            var setUserNameResult = await _userManager.SetUserNameAsync(user, dto.Email);
            if (!setEmailResult.Succeeded || !setUserNameResult.Succeeded)
            {
                var errors = string.Join(", ", setEmailResult.Errors.Concat(setUserNameResult.Errors).Select(e => e.Description));
                throw new InvalidOperationException($"Update failed: {errors}");
            }
        }


        if (!string.IsNullOrWhiteSpace(dto.UserName))
        {
            user.Name = dto.UserName;
        }


        if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
        {
            user.AvatarUrl = dto.AvatarUrl;
        }


        if (!string.IsNullOrWhiteSpace(dto.CurrentPassword) && !string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            var passwordChangeResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!passwordChangeResult.Succeeded)
            {
                var errors = string.Join(", ", passwordChangeResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Password update failed: {errors}");
            }
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);


        var token = await GenerateTokenAsync(user);

        return new AuthReadDto
        {
            Name = user.Name,
            Email = user.Email!,
            AvatarUrl = user.AvatarUrl,
            Balance = user.Balance,
            Roles = roles,
            AccessToken = token.AccessToken,
            ExpiredAt = token.ExpiredAt
        };

    }

    private async Task<AuthReadDto> GenerateTokenAsync(User user)
    {
        var jwtSettings = _configuration.GetSection("JWTSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationInMinutes = int.Parse(jwtSettings["ExpirationInMinutes"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name , user.UserName!),
        new Claim(ClaimTypes.Email , user.Email!),
        new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
    };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var accessToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
            signingCredentials: credentials
        );

        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);


        var refreshTokenEntity = new RefreshToken
        {
            JwtId = Guid.NewGuid().ToString("N"),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpirationInDays)
        };

        var refreshTokenClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, refreshTokenEntity.JwtId),
        new Claim("token_type", RefreshTokenType)
    };

        var refreshTokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecretKey));
        var refreshTokenCredentials = new SigningCredentials(refreshTokenKey, SecurityAlgorithms.HmacSha256);

        var refreshJwt = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: refreshTokenClaims,
            expires: refreshTokenEntity.ExpiresAt.UtcDateTime,
            signingCredentials: refreshTokenCredentials
        );

        refreshTokenEntity.Token = new JwtSecurityTokenHandler().WriteToken(refreshJwt);

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);


        return new AuthReadDto
        {
            Name = user.Name,
            Email = user.Email!,
            AccessToken = accessTokenString,
            ExpiredAt = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            RefreshToken = refreshTokenEntity.Token,
            RefreshTokenExpiredAt = refreshTokenEntity.ExpiresAt.UtcDateTime,
            Roles = roles,
            AvatarUrl = user.AvatarUrl,
            Balance = user.Balance
        };
    }

    private static string GetJtiFromRefreshToken(string refreshJwt)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(refreshJwt)) return string.Empty;

        var jwt = handler.ReadJwtToken(refreshJwt);
        return jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    }

    public async Task<AuthReadDto?> UpdateBalanceAsync(string userId, decimal amountToAdd)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return null;

        user.Balance += amountToAdd;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthReadDto
        {
            Name = user.Name,
            Email = user.Email!,
            AvatarUrl = user.AvatarUrl,
            Balance = user.Balance,
            Roles = roles
        };
    }
}
