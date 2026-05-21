namespace OptivisionApp;

public partial class App : Application
{
	public static Models.Usuario? UsuarioActual { get; set; }

	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}

