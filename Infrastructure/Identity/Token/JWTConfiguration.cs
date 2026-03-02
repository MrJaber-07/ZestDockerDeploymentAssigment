namespace Infrastructure.Security.Token;

public class JWTConfiguration
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int TokenExpiryDurationInMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDurationInDays { get; set; } = 7;
}