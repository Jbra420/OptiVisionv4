using System.ComponentModel.DataAnnotations;

namespace OptiVision.API.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Rol { get; set; } = "Cliente"; // Cliente, Optometrista, Administrador

        public string? Receta { get; set; } // Receta de agudeza visual / optometría en formato JSON o Texto

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
