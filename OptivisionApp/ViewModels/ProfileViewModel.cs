using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptivisionApp.Models;
using OptivisionApp.Services;
using System.Collections.ObjectModel;

namespace OptivisionApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly MockDatabaseService _db;

    [ObservableProperty] private string userName = "Usuario";
    [ObservableProperty] private string userEmail = "";
    [ObservableProperty] private string userInitial = "U";
    [ObservableProperty] private int lastScore = 0;
    [ObservableProperty] private string lastTestDate = "";
    [ObservableProperty] private string lastTestCondition = "";
    [ObservableProperty] private bool hasTestHistory = false;
    [ObservableProperty] private ObservableCollection<Appointment> upcomingAppointments = new();
    [ObservableProperty] private bool hasUpcomingAppointments = false;
    [ObservableProperty] private Color scoreColor = Color.FromArgb("#8B8D99");

    public ProfileViewModel(MockDatabaseService db)
    {
        _db = db;
        Title = "Perfil";
        _ = LoadProfileAsync();
    }

    public async Task RefreshAsync() => await LoadProfileAsync();

    private async Task LoadProfileAsync()
    {
        var user = await _db.GetUserByIdAsync(MockDatabaseService.CurrentUserId);
        if (user != null)
        {
            UserName = user.Name ?? "Usuario";
            UserEmail = user.Email ?? "";
            UserInitial = UserName.Length > 0
                ? UserName[0].ToString().ToUpper()
                : "U";
        }

        var lastTest = await _db.GetLastVisualTestAsync();
        if (lastTest != null)
        {
            LastScore = lastTest.Score;
            LastTestDate = lastTest.TestDate.ToString("dd 'de' MMMM, yyyy");
            LastTestCondition = lastTest.Recommendation ?? "";
            HasTestHistory = true;
            ScoreColor = LastScore >= 80
                ? Color.FromArgb("#2ECC71")
                : Color.FromArgb("#E5A93B");
        }

        var apts = await _db.GetUpcomingAppointmentsAsync();
        UpcomingAppointments.Clear();
        foreach (var a in apts)
            UpcomingAppointments.Add(a);
        HasUpcomingAppointments = UpcomingAppointments.Count > 0;
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        bool confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Cerrar sesión",
            "¿Estás seguro de que quieres cerrar sesión?",
            "Sí, salir",
            "Cancelar");

        if (!confirm) return;

        MockDatabaseService.CurrentUserId = 0;
        await Shell.Current.GoToAsync("//vLogin");
    }
}
