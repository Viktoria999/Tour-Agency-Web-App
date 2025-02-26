using Auth.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Auth
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AuthMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["authToken"];

            if (token != null)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
                    var principal = accountService.ValidateToken(token);

                    if (principal != null)
                    {
                        string? isAdminVal = principal.FindFirst("isAdmin")?.Value;
                        context.Items["IsAuthenticated"] = true;
                        context.Items["UserName"] = principal.FindFirst("userName")?.Value;
                        context.Items["IsAdmin"] = (isAdminVal == null || isAdminVal == "false") ? false : true;
                        context.Response.Cookies.Append("authToken", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                        });
                    }
                }
            }

            await _next(context);
        }
    }
}
