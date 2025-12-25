using Devkit.Common.Identity.Core.Models;

namespace Devkit.Common.Identity.Core.Interfaces;

public interface IAuthenticationService
{
    Task<AuthResponse> LoginAsync(AuthRequest request);
    Task<bool> LogoutAsync(string refreshToken);
}