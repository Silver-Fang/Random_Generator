namespace Random_Generator;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		GoToAsync(Preferences.Default.Get("首页", "RandomNumber"));
	}

	private void Shell_Navigated(object sender, ShellNavigatedEventArgs e)
	{
		Preferences.Default.Set("首页", e.Current.Location.OriginalString);
	}
}
