using Devkit.Common.Identity.Core.Conventions;
using Devkit.Common.Identity.Enums;

namespace Devkit.Common.Identity.Core.Extensions.Keycloak;

internal static class IdentityRequiredActionExtensions
{
    public static string ToKeycloak(this IdentityRequiredAction action)
    {
        return action switch
        {
            IdentityRequiredAction.VerifyEmail => KeycloakRequiredActions.VerifyEmail,
            IdentityRequiredAction.UpdatePassword => KeycloakRequiredActions.UpdatePassword,
            IdentityRequiredAction.ConfigureMfa => KeycloakRequiredActions.ConfigureTotp,
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };
    }
}
