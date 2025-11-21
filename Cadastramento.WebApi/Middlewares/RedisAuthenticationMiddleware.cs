using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Cadastramento.Infrastructure;
using Cadastramento.Core;
using Serilog;
using System;

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
            // allow swagger and health through without session
            if (context.Request.Path.StartsWithSegments("/swagger") || context.Request.Path.StartsWithSegments("/health"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Cookies.TryGetValue(CookieName, out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing SessionEntrevistaId cookie.");
                return;
            }

            try
            {
                Log.Information("Auth: received session cookie {sessionId}", sessionId);
                bool sessionExists = await redisService.SessionExistsAsync(sessionId);

                if (!sessionExists)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid or expired session.");
                    return;
                }

                var hash = await redisService.GetSessionHashAsync(sessionId);
                var info = new SessionInfo
                {
                    SessionId = sessionId,
                    Raw = hash
                };

                if (hash.TryGetValue("acesso", out var acessoVal))
                {
                    info.Acesso = string.Equals(acessoVal, "true", StringComparison.OrdinalIgnoreCase);
                }
                if (hash.TryGetValue("idUsuario", out var idUsuarioVal) && int.TryParse(idUsuarioVal, out var idu))
                {
                    info.IdUsuario = idu;
                }
                if (hash.TryGetValue("usuario", out var usuarioVal))
                {
                    info.Usuario = usuarioVal;
                }

                // store in context for controllers
                context.Items["SessionEntrevista"] = info;

                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Auth: error while validating session {sessionId}", sessionId);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Error validating session.");
                return;
            }
        }
    }
}
