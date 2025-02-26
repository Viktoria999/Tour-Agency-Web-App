using Auth.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var authSettings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();

            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.Cookie.Name = "authToken";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.LoginPath = "/Auth/ShowAuthorizationModal";
                    options.Cookie.SameSite = SameSiteMode.Strict;
                });

            return serviceCollection;
        }
    }
}
