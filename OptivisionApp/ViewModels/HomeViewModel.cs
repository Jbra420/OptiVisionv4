using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptivisionApp.Services;

namespace OptivisionApp.ViewModels;

[QueryProperty(nameof(UserName), "userName")]
public partial class HomeViewModel : BaseViewModel
{
    private readonly MockDatabaseService _db;

    [ObservableProperty]
    private string userName = "Usuario";

    [ObservableProperty]
    private int lastScore = 0;

    [ObservableProperty]
    private string greetingText = "Hola";

    [ObservableProperty]
    private string scoreChangeText = "";

    [ObservableProperty]
    private bool hasScore = false;

    // Dashboard Properties
    [ObservableProperty] private Color arcColor = Color.FromArgb("#3498DB");
    [ObservableProperty] private DoubleCollection scoreDashArray = new DoubleCollection { 0.0, 1000.0 };
    [ObservableProperty] private string nextAppointmentText = "Sin citas programadas";

    public HomeViewModel(MockDatabaseService db)
    {
        _db = db;
        Title = "Inicio";
        UpdateGreeting();
        _ = LoadScoreAsync();
    }

    public async Task RefreshAsync()
    {
        UpdateGreeting();
        await LoadScoreAsync();
        await LoadNextAppointmentAsync();
    }

    private void UpdateGreeting()
    {
        var hour = DateTime.Now.Hour;
        GreetingText = hour < 12 ? "Buenos días" :
                       hour < 19 ? "Buenas tardes" : "Buenas noches";
    }

    private async Task LoadScoreAsync()
    {
        var lastTest = await _db.GetLastVisualTestAsync();
        if (lastTest != null)
        {
            LastScore = lastTest.Score;
            HasScore = true;
            ScoreChangeText = LastScore >= 80 ? "Visión óptima" : "Visita al óptico recomendada";
            
            if (LastScore < 50)
                ArcColor = Color.FromArgb("#E74C3C"); // Red
            else if (LastScore < 80)
                ArcColor = Color.FromArgb("#F1C40F"); // Yellow
            else
                ArcColor = Color.FromArgb("#2ECC71"); // Green
            
            // Calc dash array for arc (approx 408 is full length of half circle with radius 130)
            // In MAUI, StrokeDashArray values are multiples of StrokeThickness (which is 20)
            double length = Math.PI * 130;
            double filled = length * (LastScore / 100.0);
            double dashMulti = filled / 20.0;
            ScoreDashArray = new DoubleCollection { dashMulti, 1000.0 };
        }
        else
        {
            LastScore = 0;
            ScoreDashArray = new DoubleCollection { 0.0, 1000.0 };
            ArcColor = Color.FromArgb("#3498DB");
        }
    }

    private async Task LoadNextAppointmentAsync()
    {
        var items = await _db.GetUserAppointmentsAsync();
        var next = items.Where(a => a.Status != "Cancelada" && a.Date >= DateTime.Today).OrderBy(a => a.Date).ThenBy(a => a.Time).FirstOrDefault();
        if (next != null)
        {
            NextAppointmentText = $"{next.Date:dd MMM}, {next.Time:hh\\:mm}";
        }
        else
        {
            NextAppointmentText = "Sin citas programadas";
        }
    }

    [RelayCommand]
    private async Task GoToCatalogAsync() =>
        await Shell.Current.GoToAsync("//MainApp/TabCatalog");

    [RelayCommand]
    private async Task GoToAppointmentsAsync() =>
        await Shell.Current.GoToAsync("vAppointments");

    [RelayCommand]
    private async Task GoToVisualTestAsync() =>
        await Shell.Current.GoToAsync("vVisualTest");

    [RelayCommand]
    private async Task GoToARIntroAsync()
    {
        await Shell.Current.GoToAsync("//MainApp/TabAR?from=TabHome");
    }

    [RelayCommand]
    private async Task GoToAlertsAsync() =>
        await Shell.Current.GoToAsync("vAlerts");
}
