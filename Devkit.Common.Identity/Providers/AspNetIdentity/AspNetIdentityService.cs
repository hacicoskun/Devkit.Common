using Devkit.Common.Identity.Core.Entities;
using Devkit.Common.Identity.Core.Extensions.AspNetIdentity;
using Devkit.Common.Identity.Core.Interfaces;
using Devkit.Common.Identity.Core.Models;
using Devkit.Common.Identity.Enums;
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
            if (user == null)
                return new AuthResponse { IsSuccess = false };

            if (user.MustChangePassword)
                return new AuthResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Şifre değiştirmeniz gerekiyor"
                };

            if (!user.EmailConfirmed)
                return new AuthResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "E-posta doğrulanmadı"
                };

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return new AuthResponse { IsSuccess = false };

            var roles = await userManager.GetRolesAsync(user);
            var (token, expiresIn) = tokenGenerator.GenerateToken(user, roles);

            return new AuthResponse { IsSuccess = true, AccessToken = token, ExpiresIn = expiresIn };
        }


        public async Task<bool> LogoutAsync(string refreshToken)
        {
            await signInManager.SignOutAsync();
            return true;
        }

        public async Task<string> CreateUserAsync(CreateUserCommand userCommand)
        {
            var user = new ApplicationUser
            {
                UserName = userCommand.Username,
                Email = userCommand.Email,
                FirstName = userCommand.FirstName,
                LastName = userCommand.LastName,
                EmailConfirmed = !userCommand.RequiredActions.Contains(IdentityRequiredAction.VerifyEmail)
            };

            var result = await userManager.CreateAsync(user, userCommand.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            foreach (var action in userCommand.RequiredActions)
            {
                await IdentityRequiredActionApplier.ApplyAsync(action, user, userManager);
            }

            await userManager.UpdateAsync(user);

            foreach (var attr in userCommand.Attributes)
            {
                await userManager.AddClaimAsync(
                    user,
                    new System.Security.Claims.Claim(attr.Key, attr.Value));
            }

            return user.Id;
        }


        public async Task<UserDetailDto> GetUserByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");
            return new UserDetailDto
            {
                Id = user.Id, Username = user.UserName!, Email = user.Email!, FirstName = user.FirstName,
                LastName = user.LastName, IsEnabled = true
            };
        }
 
        public Task UpdateUserAsync(UpdateUserDto userDto) => Task.CompletedTask;
        public Task DeleteUserAsync(string userId) => Task.CompletedTask;
        public Task SetUserStatusAsync(string userId, bool isEnabled) => Task.CompletedTask;
        public Task SendForgotPasswordEmailAsync(string email) => Task.CompletedTask; 
    }
}