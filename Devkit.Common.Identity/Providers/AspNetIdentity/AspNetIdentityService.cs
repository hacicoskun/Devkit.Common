using Devkit.Common.Identity.Core.Entities;
using Devkit.Common.Identity.Core.Interfaces;
using Devkit.Common.Identity.Core.Models;
using Devkit.Common.Identity.Providers.AspNetIdentity.Utilities;
using Microsoft.AspNetCore.Identity;

namespace Devkit.Common.Identity.Providers.AspNetIdentity
{
    public class AspNetIdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenGenerator tokenGenerator)
        : IAuthenticationService, IUserService
    {
        public async Task<AuthResponse> LoginAsync(AuthRequest request)
        {
            var user = await userManager.FindByNameAsync(request.Username);
            if (user == null) return new AuthResponse { IsSuccess = false, ErrorMessage = "Kullanıcı bulunamadı" };

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded) return new AuthResponse { IsSuccess = false, ErrorMessage = "Şifre hatalı" };

            var roles = await userManager.GetRolesAsync(user);
            var (token, expiresIn) = tokenGenerator.GenerateToken(user, roles);

            return new AuthResponse { IsSuccess = true, AccessToken = token, ExpiresIn = expiresIn };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            await signInManager.SignOutAsync();
            return true;
        }

        public async Task<string> CreateUserAsync(CreateUserDto userDto)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.Username,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded) throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return user.Id;
        }

        public async Task<UserDetailDto> GetUserByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");
            return new UserDetailDto { Id = user.Id, Username = user.UserName!, Email = user.Email!, FirstName = user.FirstName, LastName = user.LastName, IsEnabled = true };
        }

        // Interface gereği boş implementasyonlar (Hata vermemesi için)
        public Task UpdateUserAsync(UpdateUserDto userDto) => Task.CompletedTask;
        public Task DeleteUserAsync(string userId) => Task.CompletedTask;
        public Task SetUserStatusAsync(string userId, bool isEnabled) => Task.CompletedTask;
        public Task SendForgotPasswordEmailAsync(string email) => Task.CompletedTask;
    }
}