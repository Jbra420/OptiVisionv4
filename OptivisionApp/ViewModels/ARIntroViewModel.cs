using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OptivisionApp.ViewModels;

[QueryProperty(nameof(LenseId), "lenseId")]
[QueryProperty(nameof(FrameColor), "frameColor")]
[QueryProperty(nameof(FromRoute), "from")]
public partial class ARIntroViewModel : BaseViewModel
{
    [ObservableProperty] private int lenseId;
    [ObservableProperty] private string frameColor = "Gold";
    [ObservableProperty] private string fromRoute = "TabCatalog";
    [ObservableProperty] private int currentStep = 0;
    [ObservableProperty] private string stepTitle = "Prueba tus lentes antes de comprar";
    [ObservableProperty] private string stepSubtitle = "Usa la cámara frontal para ver cómo quedan los marcos en tu rostro en tiempo real.";
    [ObservableProperty] private string stepIcon = "glasses.svg";
    [ObservableProperty] private string buttonText = "Siguiente";
    [ObservableProperty] private bool isDot0Active = true;
    [ObservableProperty] private bool isDot1Active = false;
    [ObservableProperty] private bool isDot2Active = false;

    public ARIntroViewModel()
    {
        Title = "Probador AR";
    }

    [RelayCommand]
    private async Task NextStepAsync()
    {
        if (CurrentStep < 2)
        {
            CurrentStep++;
            UpdateStep();
        }
        else
        {
            // Marcar que ya vio el intro
            Preferences.Set("ar_intro_shown", true);
            await Shell.Current.GoToAsync($"vARSimulator?lenseId={LenseId}&frameColor={FrameColor}");
        }
    }

    [RelayCommand]
    private async Task SkipAsync()
    {
        Preferences.Set("ar_intro_shown", true);
        await Shell.Current.GoToAsync($"vARSimulator?lenseId={LenseId}&frameColor={FrameColor}");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        if (Shell.Current.Navigation.NavigationStack.Count > 1)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            var route = string.IsNullOrEmpty(FromRoute) ? "TabCatalog" : FromRoute;
            await Shell.Current.GoToAsync($"//MainApp/{route}");
        }
    }
    private void UpdateStep()
    {
        IsDot0Active = CurrentStep == 0;
        IsDot1Active = CurrentStep == 1;
        IsDot2Active = CurrentStep == 2;

        switch (CurrentStep)
        {
            case 0:
                StepIcon = "glasses.svg";
                StepTitle = "Prueba tus lentes antes de comprar";
                StepSubtitle = "Usa la cámara frontal para ver cómo quedan los marcos en tu rostro en tiempo real.";
                ButtonText = "Siguiente";
                break;
            case 1:
                StepIcon = "camera.svg";
                StepTitle = "Toma una foto";
                StepSubtitle = "Captura una selfie y los lentes se superpondrán automáticamente sobre tu rostro.";
                ButtonText = "Siguiente";
                break;
            case 2:
                StepIcon = "sparkle.svg";
                StepTitle = "Cambia colores al instante";
                StepSubtitle = "Selecciona entre tintes Zafiro, Rubí, Esmeralda o Ámbar y cambia el color del marco.";
                ButtonText = "Empezar";
                break;
        }
    }
}
