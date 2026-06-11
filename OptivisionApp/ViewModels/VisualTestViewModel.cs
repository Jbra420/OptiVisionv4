using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptivisionApp.Models;
using OptivisionApp.Services;

namespace OptivisionApp.ViewModels;

public partial class VisualTestViewModel : BaseViewModel
{
    private readonly MockDatabaseService _db;

    // Rotaciones de la E: 0=Arriba, 90=Derecha, 180=Abajo, 270=Izquierda
    private readonly int[] _questionRotations = { 0, 270, 90, 180, 0 };
    private int _currentQuestionIndex = 0;
    private int _correctAnswers = 0;
    private string _selectedDirection = "";

    [ObservableProperty]
    private string progressText = "Pregunta 1 de 5";

    [ObservableProperty]
    private double currentRotation = 0;

    [ObservableProperty]
    private string selectedDirectionDisplay = "Selecciona una dirección";

    [ObservableProperty]
    private double progressPercent = 0.2;

    [ObservableProperty]
    private bool isUpSelected = false;

    [ObservableProperty]
    private bool isDownSelected = false;

    [ObservableProperty]
    private bool isLeftSelected = false;

    [ObservableProperty]
    private bool isRightSelected = false;

    public VisualTestViewModel(MockDatabaseService db)
    {
        _db = db;
        Title = "Test Visual";
        LoadQuestion(0);
    }

    private void LoadQuestion(int index)
    {
        if (index == 0)
        {
            _correctAnswers = 0;
        }
        _currentQuestionIndex = index;
        _selectedDirection = "";
        CurrentRotation = _questionRotations[index];
        ProgressText = $"Pregunta {index + 1} de 5";
        ProgressPercent = (index + 1) / 5.0;
        SelectedDirectionDisplay = "Selecciona una dirección";
        ResetDirections();
    }

    private void ResetDirections()
    {
        IsUpSelected = IsDownSelected = IsLeftSelected = IsRightSelected = false;
    }

    [RelayCommand]
    private void AnswerUp()
    {
        _selectedDirection = "Arriba";
        SelectedDirectionDisplay = "↑ Arriba seleccionado";
        ResetDirections();
        IsUpSelected = true;
    }

    [RelayCommand]
    private void AnswerDown()
    {
        _selectedDirection = "Abajo";
        SelectedDirectionDisplay = "↓ Abajo seleccionado";
        ResetDirections();
        IsDownSelected = true;
    }

    [RelayCommand]
    private void AnswerLeft()
    {
        _selectedDirection = "Izquierda";
        SelectedDirectionDisplay = "← Izquierda seleccionado";
        ResetDirections();
        IsLeftSelected = true;
    }

    [RelayCommand]
    private void AnswerRight()
    {
        _selectedDirection = "Derecha";
        SelectedDirectionDisplay = "→ Derecha seleccionado";
        ResetDirections();
        IsRightSelected = true;
    }

    [RelayCommand]
    private async Task ConfirmAnswerAsync()
    {
        if (string.IsNullOrEmpty(_selectedDirection))
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "", "Selecciona una dirección primero", "OK");
            return;
        }

        // Verificar respuesta correcta
        int rotation = _questionRotations[_currentQuestionIndex];
        bool correct =
            (_selectedDirection == "Derecha" && rotation == 0) ||
            (_selectedDirection == "Abajo" && rotation == 90) ||
            (_selectedDirection == "Izquierda" && rotation == 180) ||
            (_selectedDirection == "Arriba" && rotation == 270);

        if (correct) _correctAnswers++;

        if (_currentQuestionIndex < 4)
        {
            LoadQuestion(_currentQuestionIndex + 1);
        }
        else
        {
            await FinishTestAsync();
        }
    }

    private async Task FinishTestAsync()
    {
        int score = (_correctAnswers * 100) / 5;
        string rec = score >= 80
            ? "Buena agudeza visual. ¡Sigue así!"
            : "Se recomienda una visita al óptico.";

        var record = new VisualTestRecord
        {
            TestDate = DateTime.Now,
            Score = score,
            Recommendation = rec
        };

        await _db.SaveVisualTestAsync(record);

        await Application.Current!.MainPage!.DisplayAlert(
            "Test Finalizado",
            $"Puntuación: {score} / 100\n\n{rec}",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");
}
