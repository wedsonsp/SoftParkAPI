using Microsoft.AspNetCore.Http;
using Cadastramento.Core;

namespace Cadastramento.WebApi.Extensions
{
    public static class HttpContextExtensions
    {
        public static SessionInfo? GetSessionEntrevista(this HttpContext context)
        {
            if (context.Items.TryGetValue("SessionEntrevista", out var obj) && obj is SessionInfo info)
                return info;
            return null;
        }
    }
}
