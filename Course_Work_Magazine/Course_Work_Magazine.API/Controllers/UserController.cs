using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Auth_DTOs;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Course_Work_Magazine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly string _uploadsFolder;

    public UserController(IUserService userService)
    {
        _userService = userService;

        _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "avatars");
        if (!Directory.Exists(_uploadsFolder))
            Directory.CreateDirectory(_uploadsFolder);
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthReadDto>>> Register([FromBody] RegisterRequestDto registerRequest)
    {
        var result = await _userService.RegisterAsync(registerRequest);
        return Ok(
             ApiResponse<AuthReadDto>.SuccessResponse(result, "User registered successfully")
         );
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthReadDto>>> Login([FromBody] LoginRequestDto loginRequest)
    {
        var result = await _userService.LoginAsync(loginRequest);
        return Ok(
             ApiResponse<AuthReadDto>.SuccessResponse(result, "Login successfully")
         );
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<AuthReadDto>>> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var profile = await _userService.GetProfileAsync(userId!);

        if (profile is null)
            return NotFound(ApiResponse<AuthReadDto>
                .ErrorResponse("User not found"));

        return Ok(ApiResponse<AuthReadDto>
            .SuccessResponse(profile, "Profile retrieved successfully"));
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<AuthReadDto>>> UpdateProfile(
        [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<AuthReadDto>
                .ErrorResponse("Invalid profile data"));

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var updated = await _userService.UpdateProfileAsync(userId!, dto);

        if (updated is null)
            return NotFound(ApiResponse<AuthReadDto>
                .ErrorResponse("User not found"));

        return Ok(ApiResponse<AuthReadDto>
            .SuccessResponse(updated, "Profile updated successfully"));
    }

    [Authorize]
    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
        if (avatar == null || avatar.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        if (!avatar.ContentType.StartsWith("image/"))
            return BadRequest(new { message = "Only images allowed" });

        if (avatar.Length > 2 * 1024 * 1024)
            return BadRequest(new { message = "Max file size is 2MB" });

        var fileName = $"{DateTime.Now.Ticks}-{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
        var filePath = Path.Combine(_uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatar.CopyToAsync(stream);
        }

        var avatarUrl = $"{Request.Scheme}://{Request.Host}/uploads/avatars/{fileName}";

        return Ok(new { success = true, avatarUrl });
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthReadDto>>> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        try
        {
            var result = await _userService.RefreshTokenAsync(dto);
            return Ok(ApiResponse<AuthReadDto>.SuccessResponse(result, "Token refreshed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<AuthReadDto>.ErrorResponse(ex.Message));
        }
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequestDto dto)
    {
        await _userService.RevokeRefreshTokenAsync(dto);
        return NoContent();
    }

    [Authorize]
    [HttpPatch("balance")]
    public async Task<ActionResult<ApiResponse<AuthReadDto>>> UpdateMyBalance([FromBody] UpdateBalanceDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized(ApiResponse<AuthReadDto>.ErrorResponse("User not found"));

        var updated = await _userService.UpdateBalanceAsync(userId, dto.Balance);

        if (updated == null)
            return NotFound(ApiResponse<AuthReadDto>.ErrorResponse("User not found"));

        return Ok(ApiResponse<AuthReadDto>.SuccessResponse(updated, "Balance updated successfully"));
    }
}