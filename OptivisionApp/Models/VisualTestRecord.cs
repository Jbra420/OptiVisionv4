using SQLite;
using System;

namespace OptivisionApp.Models;

public class VisualTestRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime TestDate { get; set; }
    
    public int Score { get; set; } // 0 a 100
    
    public string Recommendation { get; set; } // Ej: "Buena agudeza visual" o "Visitar especialista"
}
