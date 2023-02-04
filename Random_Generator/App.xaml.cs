using Microsoft.Maui.Platform;

namespace Random_Generator;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
#if WINDOWS
	protected override Window CreateWindow(IActivationState activationState)
	{
		Window 窗口 = base.CreateWindow(activationState);
		窗口.Height = Preferences.Default.Get("窗口高度", double.NaN);
		窗口.Width = Preferences.Default.Get("窗口宽度", double.NaN);
		窗口.X = Preferences.Default.Get("窗口X", double.NaN);
		窗口.Y = Preferences.Default.Get("窗口Y", double.NaN);
		窗口.SizeChanged += 窗口_SizeChanged;
		return 窗口;
	}

	private void 窗口_SizeChanged(object sender, EventArgs e)
	{
		Window 窗口 = sender as Window;
		if (窗口.Y > 0)
		{
			Preferences.Default.Set("窗口高度", 窗口.Height);
			Preferences.Default.Set("窗口宽度", 窗口.Width);
			Preferences.Default.Set("窗口X", 窗口.X);
			Preferences.Default.Set("窗口Y", 窗口.Y);
		}
	}
#endif
}
