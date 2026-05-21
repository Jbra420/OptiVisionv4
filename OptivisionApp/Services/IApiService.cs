using System.Collections.Generic;
using System.Threading.Tasks;
using OptivisionApp.Models;

namespace OptivisionApp.Services
{
    public interface IApiService
    {
        Task<Usuario?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string nombre, string email, string password);
        Task<List<MarcoLente>> GetLentesAsync(string? categoria = null);
        Task<List<Cita>> GetCitasUsuarioAsync(int usuarioId);
        Task<bool> CrearCitaAsync(Cita cita);
        Task<bool> ActualizarRecetaAsync(int usuarioId, string recetaJson);
        Task<bool> CancelarCitaAsync(int citaId);
    }
}
