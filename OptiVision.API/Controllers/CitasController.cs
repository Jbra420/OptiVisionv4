using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiVision.API.Data;
using OptiVision.API.Models;

namespace OptiVision.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly OptiVisionDbContext _context;

        public CitasController(OptiVisionDbContext context)
        {
            _context = context;
        }

        // GET: api/citas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitas()
        {
            return await _context.Citas
                .Include(c => c.Usuario)
                .OrderBy(c => c.FechaCita)
                .ToListAsync();
        }

        // GET: api/citas/usuario/{usuarioId}
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitasUsuario(int usuarioId)
        {
            var citas = await _context.Citas
                .Where(c => c.UsuarioId == usuarioId)
                .OrderBy(c => c.FechaCita)
                .ToListAsync();

            return Ok(citas);
        }

        // POST: api/citas
        [HttpPost]
        public async Task<ActionResult<Cita>> PostCita([FromBody] CrearCitaDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == model.UsuarioId);
            if (!usuarioExiste)
            {
                return BadRequest("El usuario especificado no existe.");
            }

            var cita = new Cita
            {
                UsuarioId = model.UsuarioId,
                FechaCita = model.FechaCita,
                Estado = "Pendiente",
                Optica = model.Optica,
                Motivo = model.Motivo,
                Notas = model.Notas
            };

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            return CreatedAtRoute(new { id = cita.Id }, cita);
        }

        // PUT: api/citas/{id}/estado
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> UpdateCitaEstado(int id, [FromBody] EstadoCitaDto model)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound("Cita no encontrada.");
            }

            cita.Estado = model.Estado; // Pendiente, Confirmada, Cancelada, Completada
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/citas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound("Cita no encontrada.");
            }

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CrearCitaDto
    {
        public int UsuarioId { get; set; }
        public DateTime FechaCita { get; set; }
        public string Optica { get; set; } = "Sede Central";
        public string Motivo { get; set; } = "Examen de la vista";
        public string? Notas { get; set; }
    }

    public class EstadoCitaDto
    {
        public string Estado { get; set; } = "Pendiente";
    }
}
