using System.Text;
using Devkit.Common.Identity.Controllers;
using Devkit.Common.Identity.Core.Entities;
using Devkit.Common.Identity.Core.Features;
using Devkit.Common.Identity.Core.Interfaces;
using Devkit.Common.Identity.Options;
using Devkit.Common.Identity.Providers.AspNetIdentity;
using Devkit.Common.Identity.Providers.AspNetIdentity.Utilities;
using Devkit.Common.Identity.Providers.Keycloak;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using IdentityOptions = Devkit.Common.Identity.Options.IdentityOptions;

namespace Devkit.Common.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDevkitIdentity(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            var options = GetOptions(services, configuration);

            if (options.Provider.Equals("AspNetIdentity", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Provider 'AspNetIdentity' seçili ama generic olmayan metod çağrıldı. Lütfen 'AddDevkitIdentity<TContext>' kullanın.");
            }

            if (options.Provider.Equals("Keycloak", StringComparison.OrdinalIgnoreCase))
            {
                ConfigureKeycloak(services, options, environment);
            }

            AddCommonServices(services, options);
            return services;
        }

        public static IServiceCollection AddDevkitIdentity<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
            where TContext : IdentityDbContext<ApplicationUser>
        {
            var options = GetOptions(services, configuration);

            if (options.Provider.Equals("AspNetIdentity", StringComparison.OrdinalIgnoreCase))
            {
                services.AddIdentityCore<ApplicationUser>(opt =>
                    {
                        opt.Password.RequireDigit = false;
                        opt.Password.RequiredLength = 6;
                        opt.User.RequireUniqueEmail = true;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<TContext>()
                    .AddSignInManager<SignInManager<ApplicationUser>>()
                    .AddDefaultTokenProviders();

                services.AddScoped<JwtTokenGenerator>();
                services.AddScoped<IAuthenticationService, AspNetIdentityService>();
                services.AddScoped<IUserService, AspNetIdentityService>();

                ConfigureAspNetIdentityJwt(services, options, environment);
            }
            else if (options.Provider.Equals("Keycloak", StringComparison.OrdinalIgnoreCase))
            {
                ConfigureKeycloak(services, options, environment);
            }

            AddCommonServices(services, options);
            return services;
        }

        private static IdentityOptions GetOptions(IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(IdentityOptions.SectionName);
            services.Configure<IdentityOptions>(section);
            return section.Get<IdentityOptions>() ?? new IdentityOptions();
        }

        private static void ConfigureKeycloak(IServiceCollection services, IdentityOptions options,
            IWebHostEnvironment environment)
        {
            services.AddHttpClient<KeycloakIdentityService>();
            services.AddScoped<IAuthenticationService>(p => p.GetRequiredService<KeycloakIdentityService>());
            services.AddScoped<IUserService>(p => p.GetRequiredService<KeycloakIdentityService>());

            bool isDev = environment.IsDevelopment();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwt =>
                {
                    jwt.Authority = $"{options.Keycloak.BaseUrl}/realms/{options.Keycloak.Realm}";
                    jwt.Audience = options.Keycloak.ClientId;
                    jwt.RequireHttpsMetadata = !isDev;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = options.Keycloak.VerifyTokenAudience,
                        ValidAudience = options.Keycloak.ClientId,
                        ValidateIssuer = true,
                        ValidIssuer = $"{options.Keycloak.BaseUrl}/realms/{options.Keycloak.Realm}",
                        ValidateLifetime = true
                    };
                });
        }

        private static void ConfigureAspNetIdentityJwt(IServiceCollection services, IdentityOptions options,
            IWebHostEnvironment environment)
        {
            bool isDev = environment.IsDevelopment();
            var jwtOpts = options.AspNetIdentity;
            var key = Encoding.UTF8.GetBytes(jwtOpts.JwtSecretKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwt =>
                {
                    jwt.RequireHttpsMetadata = !isDev;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = jwtOpts.JwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOpts.JwtAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        private static void AddCommonServices(IServiceCollection services, IdentityOptions options)
        {
            services.AddControllers()
                .AddApplicationPart(typeof(AuthController).Assembly)
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new IdentityFeatureProvider(options.EnableAuthApi,
                        options.EnableUserApi));
                })
                .AddControllersAsServices();
        }
    }
}