namespace OptivisionApp;

public partial class App : Application
{
	public static Models.Usuario? UsuarioActual { get; set; }

	public App()
	{
		InitializeComponent();

		// Verificar si hay una sesión guardada
		string savedEmail = Preferences.Get("UserEmail", string.Empty);
		int savedUserId = Preferences.Get("UserId", 0);

		// Configurar siempre AppShell para que Shell.Current no sea null
		MainPage = new AppShell();
			
		if (!string.IsNullOrEmpty(savedEmail) && savedUserId != 0)
		{
            // Restaurar sesión en memoria
			UsuarioActual = new Models.Usuario 
			{ 
				Id = savedUserId, 
				Email = savedEmail,
				Nombre = "Usuario Local"
			};
        }

		// Configurar siempre AppShell. AppShell.xaml.cs leerá UsuarioActual para decidir el CurrentItem.
		MainPage = new AppShell();
	}
}
