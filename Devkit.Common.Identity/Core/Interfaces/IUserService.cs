using Devkit.Common.Identity.Core.Models;

namespace Devkit.Common.Identity.Core.Interfaces;

public interface IUserService
{
    Task<string> CreateUserAsync(CreateUserDto userDto);
    Task UpdateUserAsync(UpdateUserDto userDto);
    Task DeleteUserAsync(string userId);
    Task<UserDetailDto> GetUserByIdAsync(string userId);
    Task SetUserStatusAsync(string userId, bool isEnabled);
    Task SendForgotPasswordEmailAsync(string email);
}