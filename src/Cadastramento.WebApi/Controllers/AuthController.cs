using Microsoft.AspNetCore.Mvc;
using Cadastramento.Infrastructure;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace Cadastramento.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IRedisSessionService _redis;

        public AuthController(IRedisSessionService redis)
        {
            _redis = redis;
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            // simple validation - replace with real DB check if needed
            if (dto.Username != "admin" || dto.Password != "123")
                return Unauthorized("Credenciais inválidas.");

            // create session id
            string sessionId = Guid.NewGuid().ToString();

            var values = new System.Collections.Generic.Dictionary<string, string>
            {
                { "usuario", dto.Username },
                { "acesso", "true" },
                { "idUsuario", "1" }
            };

            await _redis.SaveSessionAsync(sessionId, values, 3600);

            // Development-friendly cookie settings: SameSite=Lax and Secure=false so cookie is visible when
            // front and API are on HTTP. In production, revert to SameSite=None and Secure=true (HTTPS).
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // DEV: allow on HTTP; set to true in production (HTTPS)
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("SessionEntrevistaId", sessionId, cookieOptions);

            return Ok(new { ok = true, sessionId });
        }
    }
}

