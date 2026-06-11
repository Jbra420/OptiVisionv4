using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptivisionApp.Models;
using OptivisionApp.Services;
using System.Collections.ObjectModel;

namespace OptivisionApp.ViewModels;

public partial class CatalogViewModel : BaseViewModel
{
    private readonly MockDatabaseService _db;

    [ObservableProperty]
    private ObservableCollection<Lense> lenses = new();

    [ObservableProperty]
    private string selectedCategory = "Todos";

    public CatalogViewModel(MockDatabaseService db)
    {
        _db = db;
        Title = "Catálogo";
        _ = LoadCatalogAsync("Todos");
    }

    [RelayCommand]
    private async Task ShowFiltersAsync()
    {
        string[] options = { "Todos", "Miopía", "Astigmatismo", "Sol", "Bifocal", "Ultra ligero" };
        var result = await Shell.Current.DisplayActionSheet(
            "Filtrar por categoría", "Cancelar", null, options);

        if (!string.IsNullOrEmpty(result) && result != "Cancelar")
        {
            SelectedCategory = result;
            await LoadCatalogAsync(result);
        }
    }

    [RelayCommand]
    private async Task OpenLensDetailAsync(Lense lense)
    {
        if (lense == null) return;
        await Shell.Current.GoToAsync($"vLensDetail?lenseId={lense.Id}");
    }

    [RelayCommand]
    private async Task OpenWhatsAppAsync(Lense lense)
    {
        if (lense == null) return;
        var number = lense.WhatsAppNumber ?? "593995987809";
        var msg = Uri.EscapeDataString(
            $"Hola! Quiero información sobre estos lentes del catálogo: *{lense.Name}* — ${lense.Price:F0}");
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

    private async Task LoadCatalogAsync(string category)
    {
        IsBusy = true;
        var items = await _db.GetCatalogAsync(category);
        Lenses.Clear();
        foreach (var item in items)
            Lenses.Add(item);
        IsBusy = false;
    }
}
