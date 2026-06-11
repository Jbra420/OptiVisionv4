namespace OptivisionApp.Models;

public class AlertItem
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string BadgeText { get; set; } = "";
    public string AccentColorHex { get; set; } = "#E5A93B";
    public string Icon { get; set; } = "bell.svg";

    public Color AccentColor => Color.FromArgb(AccentColorHex);
    public Color BadgeBackgroundColor => Color.FromArgb(AccentColorHex + "33"); // 20% opacity
}
