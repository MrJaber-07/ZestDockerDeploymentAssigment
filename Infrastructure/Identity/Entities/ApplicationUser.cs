using Application.Common;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Security.Entities;

public class ApplicationUser : IdentityUser, IAuditable
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public List<RefreshToken> RefreshTokens { get; set; } = new();

    public string? DisplayName { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedOn { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}