namespace Random_Generator;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		CurrentItem = Items[Preferences.Default.Get("首页", 0)];
	}

	private void Shell_Navigated(object sender, ShellNavigatedEventArgs e)
	{
		Preferences.Default.Set("首页", Items.IndexOf(CurrentItem));
	}
}
