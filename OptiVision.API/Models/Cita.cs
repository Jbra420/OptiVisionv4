using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiVision.API.Models
{
    public class Cita
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required]
        public DateTime FechaCita { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, Cancelada, Completada

        [MaxLength(100)]
        public string Optica { get; set; } = "Sede Central";

        [MaxLength(255)]
        public string Motivo { get; set; } = "Examen de la vista";

        public string? Notas { get; set; }
    }
}
