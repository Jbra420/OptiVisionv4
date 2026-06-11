using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptivisionApp.Services;

namespace OptivisionApp.ViewModels;

[QueryProperty(nameof(LenseId), "lenseId")]
[QueryProperty(nameof(FrameColor), "frameColor")]
public partial class ARSimulatorViewModel : BaseViewModel
{
    private readonly MockDatabaseService _db;

    [ObservableProperty] private int lenseId;
    [ObservableProperty] private string frameColor = "Gold";

    // Foto capturada por el usuario
    [ObservableProperty] private ImageSource? capturedPhoto;
    [ObservableProperty] private bool hasPhoto = false;

    // Lente actual
    [ObservableProperty] private Models.Lense? currentLense;

    // Ajustes manuales del usuario para 2D Overlay
    [ObservableProperty] private double overlayScale = 1.0;
    [ObservableProperty] private double overlayTranslateX = 0.0;
    [ObservableProperty] private double overlayTranslateY = -30.0; // Por defecto un poco más arriba

    // Tinte de luna seleccionado
    [ObservableProperty] private string selectedTintName = "Rubí";
    [ObservableProperty] private Color tintColor = Color.FromArgb("#8B1A4A");
    [ObservableProperty] private double tintOpacity = 0.4; // Ligeramente más transparente

    // Color de marco seleccionado (Filtro base)
    [ObservableProperty] private string selectedFrameName = "Gold";
    [ObservableProperty] private Color frameDisplayColor = Color.FromArgb("#E5A93B");

    // Estados de selección de tintes
    [ObservableProperty] private bool isSapphireSelected = false;
    [ObservableProperty] private bool isRubySelected = true;
    [ObservableProperty] private bool isEmeraldSelected = false;
    [ObservableProperty] private bool isAmberSelected = false;

    // Estados de selección de marcos
    [ObservableProperty] private bool isGoldFrame = true;
    [ObservableProperty] private bool isBlueFrame = false;
    [ObservableProperty] private bool isBlackFrame = false;
    [ObservableProperty] private bool isRubyFrame = false;

    public ARSimulatorViewModel(MockDatabaseService db)
    {
        _db = db;
        Title = "Probador AR";
    }

    async partial void OnLenseIdChanged(int value)
    {
        var lense = await _db.GetLenseByIdAsync(value);
        if (lense != null)
        {
            CurrentLense = lense;
            Title = $"Probando {lense.Name}";
        }
    }

    partial void OnFrameColorChanged(string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        
        switch (value.Trim().ToLower())
        {
            case "gold":
                SelectFrameGold();
                break;
            case "blue":
                SelectFrameBlue();
                break;
            case "black":
                SelectFrameBlack();
                break;
            case "ruby":
                SelectFrameRuby();
                break;
        }
    }

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        try
        {
            // Request camera permission first
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status != PermissionStatus.Granted)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Permiso requerido",
                    "Se necesita permiso de cámara para tomar una selfie. Usando modelo de prueba.",
                    "OK");
                // Use fallback placeholder
                CapturedPhoto = ImageSource.FromFile("face_placeholder.jpg");
                HasPhoto = true;
                return;
            }

            var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Toma una selfie"
            });

            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                CapturedPhoto = ImageSource.FromStream(() => stream);
                HasPhoto = true;
            }
        }
        catch (Exception)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Demostración AR", "Cámara no disponible. Usando modelo de prueba para la simulación.", "OK");
            
            CapturedPhoto = ImageSource.FromFile("face_placeholder.jpg");
            HasPhoto = true;
        }
    }

    [RelayCommand]
    private void SelectTintSapphire()
    {
        ResetTints();
        IsSapphireSelected = true;
        SelectedTintName = "Zafiro";
        TintColor = Color.FromArgb("#1A5276");
    }

    [RelayCommand]
    private void SelectTintRuby()
    {
        ResetTints();
        IsRubySelected = true;
        SelectedTintName = "Rubí";
        TintColor = Color.FromArgb("#8B1A4A");
    }

    [RelayCommand]
    private void SelectTintEmerald()
    {
        ResetTints();
        IsEmeraldSelected = true;
        SelectedTintName = "Esmeralda";
        TintColor = Color.FromArgb("#1A6B3A");
    }

    [RelayCommand]
    private void SelectTintAmber()
    {
        ResetTints();
        IsAmberSelected = true;
        SelectedTintName = "Ámbar";
        TintColor = Color.FromArgb("#B7770D");
    }

    [RelayCommand]
    private void SelectFrameGold()
    {
        ResetFrames();
        IsGoldFrame = true;
        SelectedFrameName = "Gold";
        FrameDisplayColor = Color.FromArgb("#E5A93B");
    }

    [RelayCommand]
    private void SelectFrameBlue()
    {
        ResetFrames();
        IsBlueFrame = true;
        SelectedFrameName = "Azul";
        FrameDisplayColor = Color.FromArgb("#2E86C1");
    }

    [RelayCommand]
    private void SelectFrameBlack()
    {
        ResetFrames();
        IsBlackFrame = true;
        SelectedFrameName = "Negro";
        FrameDisplayColor = Color.FromArgb("#1A1A2E");
    }

    [RelayCommand]
    private void SelectFrameRuby()
    {
        ResetFrames();
        IsRubyFrame = true;
        SelectedFrameName = "Rubí";
        FrameDisplayColor = Color.FromArgb("#8B1A4A");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await Application.Current!.MainPage!.DisplayAlert(
            "Guardado",
            $"Combinación guardada:\nMarco: {SelectedFrameName}\nTinte: {SelectedTintName}",
            "OK");
    }

    [RelayCommand]
    private async Task ShareAsync()
    {
        await Application.Current!.MainPage!.DisplayAlert(
            "Compartir",
            "Función de compartir próximamente disponible.",
            "OK");
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    private void ResetTints()
    {
        IsSapphireSelected = IsRubySelected = IsEmeraldSelected = IsAmberSelected = false;
    }

    private void ResetFrames()
    {
        IsGoldFrame = IsBlueFrame = IsBlackFrame = IsRubyFrame = false;
    }
}
