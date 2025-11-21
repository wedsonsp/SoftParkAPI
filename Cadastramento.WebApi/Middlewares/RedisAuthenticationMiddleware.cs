using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Cadastramento.Infrastructure;

namespace Cadastramento.WebApi.Middlewares
{
    public class RedisAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CookieName = "SessionEntrevistaId";

        public RedisAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRedisSessionService redisService)
        {
            if (!context.Request.Cookies.TryGetValue(CookieName, out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing SessionEntrevistaId cookie.");
                return;
            }

            bool sessionExists = await redisService.SessionExistsAsync(sessionId);

            if (!sessionExists)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid or expired session.");
                return;
            }

            await _next(context);
        }
    }
}
