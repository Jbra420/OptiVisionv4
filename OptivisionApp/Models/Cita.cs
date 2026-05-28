using System;
using SQLite;

namespace OptivisionApp.Models
{
    public class Cita
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaCita { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string Optica { get; set; } = "Sede Central";
        public string Motivo { get; set; } = "Examen de la vista";
        public string? Notas { get; set; }

        // Propiedades de formato para XAML
        public string FechaFormateada => FechaCita.ToString("dd/MM/yyyy hh:mm tt");
    }
}
