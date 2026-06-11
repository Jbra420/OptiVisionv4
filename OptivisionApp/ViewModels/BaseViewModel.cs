using CommunityToolkit.Mvvm.ComponentModel;

namespace OptivisionApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title;
}
