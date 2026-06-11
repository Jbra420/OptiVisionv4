using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptivisionApp.Models;
using OptivisionApp.Services;
using System.Collections.ObjectModel;

namespace OptivisionApp.ViewModels;

public partial class AppointmentsViewModel : BaseViewModel
{
    private readonly MockDatabaseService _db;
    private Appointment? _appointmentToReschedule;

    [ObservableProperty]
    private ObservableCollection<Appointment> appointments = new();

    [ObservableProperty]
    private ObservableCollection<CalendarDay> calendarDays = new();

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private string selectedTime = "";

    [ObservableProperty]
    private string selectedConsultationType = "Control";

    [ObservableProperty]
    private int currentMonth;

    [ObservableProperty]
    private int currentYear;

    [ObservableProperty]
    private string monthYearText = "";

    [ObservableProperty]
    private string confirmButtonText = "Selecciona un horario";

    // Horarios disponibles (el 15:00 siempre ocupado para demo)
    public List<TimeSlot> TimeSlots { get; } = new()
    {
        new("09:00", true),
        new("10:30", true),
        new("11:00", true),
        new("14:00", true),
        new("15:00", false), // Ocupado
        new("16:00", true),
    };

    public AppointmentsViewModel(MockDatabaseService db)
    {
        _db = db;
        Title = "Citas";
        CurrentMonth = DateTime.Today.Month;
        CurrentYear = DateTime.Today.Year;
        UpdateMonthYearText();
        GenerateCalendarDays();
        _ = LoadAppointmentsAsync();
    }

    partial void OnSelectedTimeChanged(string value) => UpdateConfirmButtonText();
    partial void OnSelectedDateChanged(DateTime value)
    {
        GenerateCalendarDays();
        UpdateConfirmButtonText();
    }

    private void UpdateConfirmButtonText()
    {
        ConfirmButtonText = !string.IsNullOrEmpty(SelectedTime)
            ? $"Confirmar · {SelectedDate:dd MMM} · {SelectedTime}"
            : "Selecciona un horario";
    }

    [RelayCommand]
    private void PreviousMonth()
    {
        if (CurrentMonth == 1) { CurrentMonth = 12; CurrentYear--; }
        else CurrentMonth--;
        UpdateMonthYearText();
        GenerateCalendarDays();
    }

    [RelayCommand]
    private void NextMonth()
    {
        if (CurrentMonth == 12) { CurrentMonth = 1; CurrentYear++; }
        else CurrentMonth++;
        UpdateMonthYearText();
        GenerateCalendarDays();
    }

    [RelayCommand]
    private void SelectDay(CalendarDay day)
    {
        if (day == null || !day.IsCurrentMonth || !day.IsAvailable) return;
        SelectedDate = day.Date;
        // OnSelectedDateChanged fires → GenerateCalendarDays + UpdateConfirmButtonText
    }

    [RelayCommand]
    private void SelectTime(string time)
    {
        SelectedTime = time;
        // Refresh slot visual states
        foreach (var slot in TimeSlots)
            slot.IsSelected = slot.Time == time;
    }

    [RelayCommand]
    private void SelectConsultationType(string type) =>
        SelectedConsultationType = type;

    [RelayCommand]
    private async Task GoToScheduleAsync() => await Shell.Current.GoToAsync("vScheduleAppointment");

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task ConfirmAppointmentAsync()
    {
        if (string.IsNullOrEmpty(SelectedTime))
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Error", "Por favor selecciona un horario.", "OK");
            return;
        }

        var parts = SelectedTime.Split(':');
        var time = new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);

        // Verificar conflicto
        bool conflict = await _db.CheckTimeConflictAsync(SelectedDate, time);
        if (conflict)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Horario ocupado", "Ese horario ya está reservado. Elige otro.", "OK");
            return;
        }

        if (_appointmentToReschedule != null)
        {
            _appointmentToReschedule.Date = SelectedDate.Date;
            _appointmentToReschedule.Time = time;
            _appointmentToReschedule.ConsultationType = SelectedConsultationType;
            await _db.UpdateAppointmentAsync(_appointmentToReschedule);
            _appointmentToReschedule = null;
        }
        else
        {
            var appointment = new Appointment
            {
                Date = SelectedDate.Date,
                Time = time,
                ConsultationType = SelectedConsultationType,
                Status = "Pendiente"
            };
            await _db.AddAppointmentAsync(appointment);
        }

        await Application.Current!.MainPage!.DisplayAlert(
            "Cita confirmada",
            $"Tu cita fue agendada para el {SelectedDate:dd 'de' MMMM} a las {SelectedTime}.",
            "OK");

        SelectedTime = "";
        foreach (var slot in TimeSlots) slot.IsSelected = false;
        await LoadAppointmentsAsync();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelAppointmentAsync(Appointment appointment)
    {
        if (appointment == null || appointment.Status == "Cancelada") return;
        var confirm = await Application.Current!.MainPage!.DisplayAlert("Cancelar Cita", "¿Estás seguro de que quieres cancelar esta cita?", "Sí, cancelar", "No");
        if (confirm)
        {
            appointment.Status = "Cancelada";
            await _db.UpdateAppointmentAsync(appointment);
            await LoadAppointmentsAsync();
        }
    }

    [RelayCommand]
    private async Task RescheduleAppointmentAsync(Appointment appointment)
    {
        if (appointment == null || appointment.Status == "Cancelada") return;
        _appointmentToReschedule = appointment;
        SelectedConsultationType = appointment.ConsultationType;
        SelectedDate = DateTime.Today;
        SelectedTime = "";
        await Shell.Current.GoToAsync("vScheduleAppointment");
    }

    private void UpdateMonthYearText()
    {
        var dt = new DateTime(CurrentYear, CurrentMonth, 1);
        MonthYearText = dt.ToString("MMMM yyyy");
        MonthYearText = char.ToUpper(MonthYearText[0]) + MonthYearText[1..];
    }

    private void GenerateCalendarDays()
    {
        CalendarDays.Clear();
        var firstDay = new DateTime(CurrentYear, CurrentMonth, 1);

        // Offset: Monday=0 ... Sunday=6
        int startOffset = ((int)firstDay.DayOfWeek + 6) % 7;
        var startDate = firstDay.AddDays(-startOffset);

        for (int i = 0; i < 42; i++)
        {
            var date = startDate.AddDays(i);
            CalendarDays.Add(new CalendarDay
            {
                Day = date.Day,
                Date = date,
                IsCurrentMonth = date.Month == CurrentMonth,
                IsToday = date.Date == DateTime.Today,
                IsSelected = date.Date == SelectedDate.Date && date.Month == CurrentMonth,
                IsAvailable = date.Date >= DateTime.Today
            });
        }
    }

    public async Task RefreshAsync()
    {
        await LoadAppointmentsAsync();
        GenerateCalendarDays();
    }

    private async Task LoadAppointmentsAsync()
    {
        var items = await _db.GetUserAppointmentsAsync();
        Appointments.Clear();
        foreach (var item in items)
            Appointments.Add(item);
    }
}

// Modelo helper para los slots de horario
public partial class TimeSlot : ObservableObject
{
    [ObservableProperty] private bool isSelected;

    public string Time { get; }
    public bool IsAvailable { get; }

    public TimeSlot(string time, bool available)
    {
        Time = time;
        IsAvailable = available;
    }

    public Color BackgroundColor =>
        IsSelected ? Color.FromArgb("#E5A93B") :
        !IsAvailable ? Color.FromArgb("#1A0D0D") :
        Color.FromArgb("#161722");

    public Color TextColor =>
        IsSelected ? Color.FromArgb("#090A0F") :
        !IsAvailable ? Color.FromArgb("#E74C3C") :
        Color.FromArgb("#8B8D99");

    public Color StrokeColor =>
        IsSelected ? Color.FromArgb("#E5A93B") : Colors.Transparent;
}
