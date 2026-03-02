using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.Data;
using Infrastructure.Security.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security.Token;

public class JwtTokenService : IJwtTokenService
{

    private readonly JWTConfiguration _config;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public JwtTokenService(
        IOptions<JWTConfiguration> options,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext)
    {
        _config = options.Value;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user)
    {
        if (_config == null || string.IsNullOrEmpty(_config.Key) || string.IsNullOrEmpty(_config.Issuer) || string.IsNullOrEmpty(_config.Audience))
            throw new InvalidOperationException("JWT configuration is missing or invalid.");
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null.");

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
        claims.AddRange(userClaims);

        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_config.TokenExpiryDurationInMinutes),
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = await CreateRefreshTokenAsync(user.Id).ConfigureAwait(false);

        return (accessToken, refreshToken.Token);
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(string userId)
    {
        var tokenBytes = new byte[64];
        RandomNumberGenerator.Fill(tokenBytes);
        var token = Convert.ToBase64String(tokenBytes);

        var refresh = new RefreshToken
        {
            Token = token,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(_config.RefreshTokenExpiryDurationInDays),
            UserId = userId
        };

        _dbContext.RefreshTokens.Add(refresh);
        await _dbContext.SaveChangesAsync();
        return refresh;
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }
}