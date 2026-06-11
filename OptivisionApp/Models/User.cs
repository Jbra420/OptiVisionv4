using SQLite;

namespace OptivisionApp.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    [Unique]
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
}
