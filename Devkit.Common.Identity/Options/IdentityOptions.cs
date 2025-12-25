namespace Devkit.Common.Identity.Options;

public class IdentityOptions
{
    public const string SectionName = "Identity";

    public string Provider { get; set; } = "Keycloak";
    
    public bool EnableAuthApi { get; set; } = true;
    public bool EnableUserApi { get; set; } = true;
    public KeycloakOptions Keycloak { get; set; } = new();
    
    public AspNetIdentityOptions AspNetIdentity { get; set; } = new();
}

public class KeycloakOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;    
    public string ClientSecret { get; set; } = string.Empty;
    public bool VerifyTokenAudience { get; set; } = true;
}

public class AspNetIdentityOptions
{
    public string JwtSecretKey { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = "https://devkit-api.com";
    public string JwtAudience { get; set; } = "devkit-client";
    public int AccessTokenExpirationMinutes { get; set; } = 60;
}