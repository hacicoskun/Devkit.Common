using Microsoft.AspNetCore.Identity;

namespace Devkit.Common.Identity.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public bool MustChangePassword { get; set; }

}