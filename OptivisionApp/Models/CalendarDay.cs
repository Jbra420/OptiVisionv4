namespace OptivisionApp.Models;

public class CalendarDay
{
    public int Day { get; set; }
    public DateTime Date { get; set; }
    public bool IsCurrentMonth { get; set; }
    public bool IsToday { get; set; }
    public bool IsSelected { get; set; }
    public bool IsAvailable { get; set; }

    // Computed display
    public string DayText => IsCurrentMonth ? Day.ToString() : "";

    public Color BackgroundColor =>
        IsSelected ? Color.FromArgb("#E5A93B") : Colors.Transparent;

    public Color TextColor =>
        IsSelected ? Color.FromArgb("#090A0F") :
        IsToday ? Color.FromArgb("#E5A93B") :
        IsCurrentMonth && IsAvailable ? Color.FromArgb("#FFFFFF") :
        Color.FromArgb("#2A2B38");

    public FontAttributes TextWeight =>
        IsSelected || IsToday ? FontAttributes.Bold : FontAttributes.None;

    public Color StrokeColor =>
        IsToday && !IsSelected ? Color.FromArgb("#E5A93B") : Colors.Transparent;

    public double StrokeThickness => IsToday && !IsSelected ? 1.5 : 0;
}
