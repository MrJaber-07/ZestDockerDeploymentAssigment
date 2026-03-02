using System.Threading.Tasks;
using Infrastructure.Security.Entities;

namespace Infrastructure.Security.Token;

public interface IJwtTokenService
{
    Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user);
    Task<RefreshToken> CreateRefreshTokenAsync(string userId);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
}