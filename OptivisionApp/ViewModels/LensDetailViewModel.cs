using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptivisionApp.Models;
using OptivisionApp.Services;
using System.Collections.ObjectModel;

namespace OptivisionApp.ViewModels;

[QueryProperty(nameof(LenseId), "lenseId")]
public partial class LensDetailViewModel : BaseViewModel
{
    private readonly MockDatabaseService _db;

    [ObservableProperty]
    private int lenseId;

    [ObservableProperty]
    private Lense? currentLense;

    [ObservableProperty]
    private string selectedFrameColor = "Gold";

    [ObservableProperty]
    private ObservableCollection<CharacteristicItem> characteristics = new();

    [ObservableProperty]
    private ObservableCollection<FrameColorItem> frameColorOptions = new();

    partial void OnLenseIdChanged(int value) => _ = LoadLenseAsync(value);

    public LensDetailViewModel(MockDatabaseService db)
    {
        _db = db;
        Title = "Detalle";
    }

    private async Task LoadLenseAsync(int id)
    {
        var lense = await _db.GetLenseByIdAsync(id);
        if (lense == null) return;

        CurrentLense = lense;

        // Construir características
        Characteristics.Clear();
        var icons = new Dictionary<string, string>
        {
            { "UV400", "sun.svg" }, { "Antirreflejo", "sparkle.svg" }, { "Polarizado", "circle.svg" },
            { "Luz azul", "lightbulb.svg" }, { "Liviano", "feather.svg" }, { "Ultra ligero", "feather.svg" },
            { "Titanio", "gear.svg" }, { "Bifocal", "eye.svg" }, { "HD", "search.svg" },
            { "Miopía", "glasses.svg" }, { "Deportivo", "running.svg" }, { "Impacto", "shield.svg" },
            { "Flexible", "target.svg" }, { "Alta definición", "diamond.svg" }, { "Astigmatismo", "telescope.svg" }
        };

        foreach (var c in lense.CharacteristicsList)
        {
            var clean = c.Trim();
            if (!string.IsNullOrEmpty(clean))
                Characteristics.Add(new CharacteristicItem
                {
                    Name = clean,
                    Icon = icons.TryGetValue(clean, out var ico) ? ico : "check_circle.svg"
                });
        }

        // Construir opciones de color del marco
        FrameColorOptions.Clear();
        var colorMap = new Dictionary<string, string>
        {
            { "Gold", "#E5A93B" }, { "Blue", "#2E86C1" }, { "Black", "#1A1A2E" },
            { "Ruby", "#8B1A4A" }, { "Silver", "#A0A0A8" }
        };

        foreach (var fc in lense.FrameColorsList)
        {
            var clean = fc.Trim();
            if (!string.IsNullOrEmpty(clean))
                FrameColorOptions.Add(new FrameColorItem
                {
                    Name = clean,
                    Hex = colorMap.TryGetValue(clean, out var hex) ? hex : "#888888"
                });
        }

        if (FrameColorOptions.Count > 0)
        {
            FrameColorOptions[0].IsSelected = true;
            SelectedFrameColor = FrameColorOptions[0].Name;
        }
    }

    [RelayCommand]
    private void SelectFrameColor(FrameColorItem item)
    {
        foreach (var fc in FrameColorOptions) fc.IsSelected = false;
        item.IsSelected = true;
        SelectedFrameColor = item.Name;
    }

    [RelayCommand]
    private async Task GoToARIntroAsync()
    {
        await Shell.Current.GoToAsync($"//MainApp/TabAR?lenseId={LenseId}&frameColor={SelectedFrameColor}&from=TabCatalog");
    }

    [RelayCommand]
    private async Task OpenWhatsAppAsync()
    {
        if (CurrentLense == null) return;
        var number = CurrentLense.WhatsAppNumber ?? "593995987809";
        var msg = Uri.EscapeDataString(
            $"Hola! Quiero información sobre estos lentes: *{CurrentLense.Name}* — ${CurrentLense.Price:F0}");
        try
        {
            await Launcher.OpenAsync(new Uri($"https://wa.me/{number}?text={msg}"));
        }
        catch
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Error", "No se pudo abrir WhatsApp.", "OK");
        }
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");
}

public partial class CharacteristicItem : ObservableObject
{
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "check_circle.svg";
}

public partial class FrameColorItem : ObservableObject
{
    public string Name { get; set; } = "";
    public string Hex { get; set; } = "#888888";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StrokeColor))]
    [NotifyPropertyChangedFor(nameof(StrokeThickness))]
    private bool isSelected;

    public Color Color => Color.FromArgb(Hex);
    public Color StrokeColor => IsSelected ? Colors.White : Colors.Transparent;
    public double StrokeThickness => IsSelected ? 2.5 : 0;
}
