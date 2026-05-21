using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiVision.API.Data;
using OptiVision.API.Models;

namespace OptiVision.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LentesController : ControllerBase
    {
        private readonly OptiVisionDbContext _context;

        public LentesController(OptiVisionDbContext context)
        {
            _context = context;
        }

        // GET: api/lentes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarcoLente>>> GetLentes(
            [FromQuery] string? marca = null,
            [FromQuery] string? categoria = null,
            [FromQuery] string? tipoMarco = null)
        {
            var query = _context.MarcosLentes.AsQueryable();

            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(l => l.Marca.ToLower() == marca.ToLower());
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(l => l.Categoria.ToLower() == categoria.ToLower());
            }

            if (!string.IsNullOrEmpty(tipoMarco))
            {
                query = query.Where(l => l.TipoMarco.ToLower() == tipoMarco.ToLower());
            }

            return await query.ToListAsync();
        }

        // GET: api/lentes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MarcoLente>> GetLente(int id)
        {
            var lente = await _context.MarcosLentes.FindAsync(id);

            if (lente == null)
            {
                return NotFound("Lente no encontrado.");
            }

            return Ok(lente);
        }

        // POST: api/lentes
        [HttpPost]
        public async Task<ActionResult<MarcoLente>> PostLente([FromBody] MarcoLente lente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MarcosLentes.Add(lente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLente), new { id = lente.Id }, lente);
        }

        // DELETE: api/lentes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLente(int id)
        {
            var lente = await _context.MarcosLentes.FindAsync(id);
            if (lente == null)
            {
                return NotFound();
            }

            _context.MarcosLentes.Remove(lente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
