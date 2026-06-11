using SQLite;
using System;

namespace OptivisionApp.Models;

public class Appointment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime Date { get; set; }
    
    public TimeSpan Time { get; set; }
    
    public string ConsultationType { get; set; } // Ej: "Revisión general"
    
    public string Status { get; set; } // Ej: "Pendiente", "Confirmada", "Cancelada"
}
