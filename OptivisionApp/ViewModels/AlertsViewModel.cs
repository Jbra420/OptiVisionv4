using CommunityToolkit.Mvvm.ComponentModel;
using OptivisionApp.Models;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;

namespace OptivisionApp.ViewModels;

public partial class AlertsViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<AlertItem> alerts = new();

    public AlertsViewModel()
    {
        Title = "Alertas";
        LoadMockAlerts();
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    private void LoadMockAlerts()
    {
        Alerts = new ObservableCollection<AlertItem>
        {
            new()
            {
                Title = "Cita en 2 horas",
                Description = "Control visual · Óptica Central",
                BadgeText = "URGENTE",
                AccentColorHex = "#E5A93B",
                Icon = "bell.svg"
            },
            new()
            {
                Title = "Recuerda tu test visual semanal",
                Description = "Han pasado 7 días desde tu última evaluación de agudeza visual.",
                BadgeText = "INFO",
                AccentColorHex = "#3498DB",
                Icon = "eye.svg"
            },
            new()
            {
                Title = "Nuevo en catálogo: Aviator Gold",
                Description = "Lentes polarizados con protección UV400 ya disponibles.",
                BadgeText = "NOVEDAD",
                AccentColorHex = "#2ECC71",
                Icon = "sparkle.svg"
            },
            new()
            {
                Title = "Descanso sugerido",
                Description = "Llevas 2 horas frente a la pantalla. Aplica la regla 20-20-20.",
                BadgeText = "SALUD",
                AccentColorHex = "#E5A93B",
                Icon = "shield.svg"
            },
            new()
            {
                Title = "Meta alcanzada",
                Description = "Has mantenido un buen nivel de iluminación hoy. ¡Sigue así!",
                BadgeText = "LOGRO",
                AccentColorHex = "#2ECC71",
                Icon = "check_circle.svg"
            }
        };
    }
}
