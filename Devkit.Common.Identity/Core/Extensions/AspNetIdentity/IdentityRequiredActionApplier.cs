using Devkit.Common.Identity.Core.Entities;
using Devkit.Common.Identity.Enums;
using Microsoft.AspNetCore.Identity;

namespace Devkit.Common.Identity.Core.Extensions.AspNetIdentity;

internal static class IdentityRequiredActionApplier
{
    public static async Task ApplyAsync(
        IdentityRequiredAction action,
        ApplicationUser user,
        UserManager<ApplicationUser> userManager)
    {
        switch (action)
        {
            case IdentityRequiredAction.VerifyEmail:
                user.EmailConfirmed = false;
                break;

            case IdentityRequiredAction.UpdatePassword:
                user.MustChangePassword = true;
                break;

            case IdentityRequiredAction.ConfigureMfa:
                await userManager.SetTwoFactorEnabledAsync(user, true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }
}