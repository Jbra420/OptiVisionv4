namespace OptivisionApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        
        if (App.UsuarioActual != null)
        {
            CurrentItem = MainAppRoute;
        }
        else
        {
            CurrentItem = HomeRoute;
        }
	}
}

