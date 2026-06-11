using SQLite;

namespace OptivisionApp.Models;

public class Lense
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Category { get; set; } = ""; // "Miopía", "Astigmatismo", "Sol", etc.

    public double Price { get; set; }

    public string ImageIcon { get; set; } = "lentes1.png";

    public int Stock { get; set; }

    // Campos nuevos HU-02 / HU-10
    public string Description { get; set; } = "";

    public string Characteristics { get; set; } = ""; // "UV400,Antirreflejo,Polarizado"

    public string FrameColors { get; set; } = ""; // "Gold,Blue,Black,Ruby"

    public double Rating { get; set; } = 4.5;

    public int ReviewCount { get; set; } = 0;

    public string WhatsAppNumber { get; set; } = "593999999999";

    // Helpers no guardados en BD
    [Ignore]
    public string[] CharacteristicsList =>
        string.IsNullOrEmpty(Characteristics) ? Array.Empty<string>() : Characteristics.Split(',');

    [Ignore]
    public string[] FrameColorsList =>
        string.IsNullOrEmpty(FrameColors) ? Array.Empty<string>() : FrameColors.Split(',');

    [Ignore]
    public string StarsText
    {
        get
        {
            int full = (int)Math.Round(Rating);
            return new string('★', Math.Min(full, 5)) + new string('☆', Math.Max(0, 5 - full));
        }
    }
}
