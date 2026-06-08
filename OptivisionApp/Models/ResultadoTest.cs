using System;
using SQLite;

namespace OptivisionApp.Models
{
    public class ResultadoTest
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UsuarioId { get; set; }

        public DateTime FechaTest { get; set; } = DateTime.Now;

        public int Puntaje { get; set; } // Ejemplo: 100 es visión perfecta, 50 es regular

        public string NivelAgudeza { get; set; } = string.Empty; // "Óptima", "Regular", "Baja"

        public string Recomendacion { get; set; } = string.Empty;
    }
}
