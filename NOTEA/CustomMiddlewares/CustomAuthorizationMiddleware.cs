using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;


namespace NOTEA.CustomMiddlewares
{
    public class CustomAuthorizationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!IsUserAuthorized(context.User))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("You are not authorized to access this resource.");
                return;
            }
            await next(context);
        }

        private bool IsUserAuthorized(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }
    }

    public static class CustomAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomAuthorizationMiddleware>();
        }
    }
}
