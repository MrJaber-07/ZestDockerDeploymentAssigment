using API.Filters;
using Infrastructure.Data;
using Infrastructure.Security.Entities;
using Infrastructure.Security.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(LogActionFilter))]
[ServiceFilter(typeof(ValidationFilter))]


public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _tokenService;
    private readonly ApplicationDbContext _db;

    public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService tokenService, ApplicationDbContext db)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _db = db;
    }

    public record LoginRequest(string Email, string Password);
    public record TokenResponse(string AccessToken, string RefreshToken);
    public record RefreshRequest(string RefreshToken);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null) return Unauthorized("Invalid credentials");

        if (!await _userManager.CheckPasswordAsync(user, req.Password)) return Unauthorized("Invalid credentials");

        var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user);
        return Ok(new TokenResponse(accessToken, refreshToken));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        var tokenEntry = await _db.RefreshTokens.Include(t => t.User).SingleOrDefaultAsync(t => t.Token == req.RefreshToken);
        if (tokenEntry == null || !tokenEntry.IsActive) return BadRequest("Invalid or expired refresh token");

        tokenEntry.Revoked = DateTime.UtcNow;
        _db.RefreshTokens.Update(tokenEntry);
        await _db.SaveChangesAsync();

        var user = tokenEntry.User!;
        var (newAccess, newRefresh) = await _tokenService.GenerateTokensAsync(user);
        return Ok(new TokenResponse(newAccess, newRefresh));
    }

}