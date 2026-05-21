using System.Threading.Tasks;

namespace OptivisionApp.Services
{
    public interface IArService
    {
        Task<bool> InicializarCamaraAsync();
        Task<bool> CargarModelo3DAsync(string modeloPath);
        Task<bool> AplicarFiltroLentesAsync(int lenteId);
        bool DetenerPruebaVirtual();
    }
}
