using System.ComponentModel.DataAnnotations;

namespace OptiVision.API.Models
{
    public class MarcoLente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Marca { get; set; } = string.Empty;

        [Range(0.01, 10000.00)]
        public decimal Precio { get; set; }

        [MaxLength(255)]
        public string ImagenUrl { get; set; } = string.Empty; // Ruta o URL de la imagen del marco

        [MaxLength(255)]
        public string? Modelo3DPath { get; set; } // Ruta al modelo 3D para el probador AR (ej: .glb o .obj)

        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(50)]
        public string TipoMarco { get; set; } = string.Empty; // Completo, Ranurado, Al aire, Metal, Pasta

        [MaxLength(20)]
        public string Categoria { get; set; } = "Unisex"; // Hombre, Mujer, Unisex, Niño
    }
}
