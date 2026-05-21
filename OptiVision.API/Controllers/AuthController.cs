using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiVision.API.Data;
using OptiVision.API.Models;

namespace OptiVision.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly OptiVisionDbContext _context;

        public AuthController(OptiVisionDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<Usuario>> Register([FromBody] RegisterDto model)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                return BadRequest("El correo electrónico ya está registrado.");
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Email = model.Email,
                PasswordHash = model.Password, // En un sistema real se encripta
                Rol = "Cliente",
                FechaRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerfil), new { id = usuario.Id }, usuario);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto model)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower() && u.PasswordHash == model.Password);

            if (usuario == null)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            return Ok(new LoginResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Receta = usuario.Receta,
                Token = $"mock-jwt-token-for-user-{usuario.Id}"
            });
        }

        // GET: api/auth/perfil/{id}
        [HttpGet("perfil/{id}")]
        public async Task<ActionResult<Usuario>> GetPerfil(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Citas)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            return Ok(usuario);
        }

        // PUT: api/auth/perfil/{id}/receta
        [HttpPut("perfil/{id}/receta")]
        public async Task<IActionResult> UpdateReceta(int id, [FromBody] RecetaUpdateDto model)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            usuario.Receta = model.RecetaJson;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTOs para solicitudes y respuestas
    public class RegisterDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string? Receta { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class RecetaUpdateDto
    {
        public string RecetaJson { get; set; } = string.Empty;
    }
}
