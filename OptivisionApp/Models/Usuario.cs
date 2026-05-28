using System;
using SQLite;

namespace OptivisionApp.Models
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = "Cliente";
        public string? Receta { get; set; } // Receta de agudeza visual
        public DateTime FechaRegistro { get; set; }
    }
}
