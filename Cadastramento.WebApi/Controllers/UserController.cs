using Microsoft.AspNetCore.Mvc;
using Cadastramento.Infrastructure;
using Cadastramento.Application;
using Cadastramento.Core;

namespace Cadastramento.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;
        public UserController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (users, total) = await _repo.GetPagedAsync(page, pageSize);
            var dtos = users.Select(u => new UserDto {
                Id = u.Id,
                Username = u.Username,
                Status = u.Status,
                Perfis = u.Perfis
            }).ToList();
            return Ok(new { data = dtos, total });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var u = await _repo.GetByIdAsync(id);
            if (u == null) return NotFound();
            return new UserDto {
                Id = u.Id,
                Username = u.Username,
                Status = u.Status,
                Perfis = u.Perfis
            };
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserDto dto)
        {
            var user = new User {
                Username = dto.Username,
                Status = dto.Status,
                Perfis = dto.Perfis
            };
            var id = await _repo.CreateAsync(user);
            return CreatedAtAction(nameof(GetById), new { id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UserDto dto)
        {
            var user = new User {
                Id = id,
                Username = dto.Username,
                Status = dto.Status,
                Perfis = dto.Perfis
            };
            await _repo.UpdateAsync(user);
            return NoContent();
        }
    }
}
