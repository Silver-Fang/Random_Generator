using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Random_Generator;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
	private static class 特化器<T>
	{
		internal static Func<string, T> Parse;
	}
	static MauiProgram()
	{
		特化器<byte>.Parse = byte.Parse;
		特化器<uint>.Parse = uint.Parse;
		特化器<double>.Parse = double.Parse;
		特化器<BigInteger>.Parse = BigInteger.Parse;
	}
	private static T Parse<T>(string 值)
	{
		return 特化器<T>.Parse(值);
	}
	//根据条目的Placeholder自动完成保存
	internal static T 数值解析<T>(Entry 条目)
	{
		T 返回值;
		try
		{
			返回值 = Parse<T>(条目.Text);
		}
		catch (Exception ex)
		{
			throw new Exception(条目.Placeholder + "：" + ex.Message);
		}
		Preferences.Default.Set(Shell.Current.CurrentPage.Title + "." + 条目.Placeholder, 条目.Text);
		return 返回值;
	}
	internal static readonly Random 随机生成器 = new();
}
