using SQLite;

namespace OptivisionApp.Models
{
    public class MarcoLente
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public string? Modelo3DPath { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string TipoMarco { get; set; } = string.Empty;
        public string Categoria { get; set; } = "Unisex";
        
        // Propiedad calculada para visualización del precio
        public string PrecioFormateado => $"${Precio:N2}";
    }
}
