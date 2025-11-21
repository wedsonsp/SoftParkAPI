using Microsoft.AspNetCore.Mvc;
using Cadastramento.Core;
using Cadastramento.WebApi.Extensions;

namespace Cadastramento.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        [HttpGet("session")]
        public IActionResult GetSession()
        {
            var session = HttpContext.GetSessionEntrevista();
            if (session == null) return Unauthorized("No session in context");
            return Ok(new {
                session.SessionId,
                session.Acesso,
                session.IdUsuario,
                session.Usuario,
                session.Raw
            });
        }
    }
}
