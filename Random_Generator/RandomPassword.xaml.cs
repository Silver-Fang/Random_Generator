namespace Random_Generator;
using static Preferences;
using static MauiProgram;
using MathNet.Numerics;
using System.Web;

public partial class RandomPassword : ContentPage
{
	public RandomPassword()
	{
		InitializeComponent();
		数字.IsChecked = Default.Get("密码.数字", true);
		小写字母.IsChecked = Default.Get("密码.小写字母", true);
		大写字母.IsChecked = Default.Get("密码.大写字母", true);
		其它字符CheckBox.IsChecked = Default.Get("密码.其它字符CheckBox", true);
		其它字符Entry.Text = Default.Get("密码.其它字符", "~!@#$%^&*()_+`-={}|[]\\:\";'<>?,./");
		每种字符至少一个.IsChecked = Default.Get("密码.每种字符至少一个", true);
		密码长度.Text = Default.Get("密码.密码长度", "");
		自动清除记录.IsChecked = Default.Get("密码.自动清除记录", true);
		生成结果.Html = Default.Get("密码.生成结果", "");
	}

	private void 数字_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("密码.数字", 数字.IsChecked);
	}

	private void 小写字母_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("密码.小写字母", 小写字母.IsChecked);
	}

	private void 大写字母_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("密码.大写字母", 大写字母.IsChecked);
	}

	private void 其它字符CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("密码.其它字符CheckBox", 其它字符CheckBox.IsChecked);
	}

	private void 每种字符至少一个_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("密码.每种字符至少一个", 每种字符至少一个.IsChecked);
	}

	private void 自动清除记录_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("密码.自动清除记录", 自动清除记录.IsChecked);
	}

	private void 复制到剪贴板_Clicked(object sender, EventArgs e)
	{
		Clipboard.SetTextAsync(HttpUtility.HtmlDecode(生成结果.Html.Replace("<br/>", Environment.NewLine)));
	}

	private void 清除记录_Clicked(object sender, EventArgs e)
	{
		生成结果.Html = "";
		Default.Set("密码.生成结果", "");
	}

	private void 生成_Clicked(object sender, EventArgs e)
	{
		const string 所有数字 = "0123456789";
		const string 所有小写 = "abcdefghijklmnopqrstuvwxyz";
		const string 所有大写 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		string 生成结果string;
		try 
		{
			byte 密码长度byte = 数值解析<byte>(密码长度);
			byte 最小长度 = 0;
			char[] 生成密码 = new char[4];
			if (每种字符至少一个.IsChecked)
			{
				if (数字.IsChecked)
					生成密码[最小长度++] = 所有数字[随机生成器.Next(0, 10)];
				if (小写字母.IsChecked)
					生成密码[最小长度++] = 所有小写[随机生成器.Next(0, 26)];
				if (大写字母.IsChecked)
					生成密码[最小长度++] = 所有大写[随机生成器.Next(0, 26)];
				if (其它字符CheckBox.IsChecked)
					生成密码[最小长度++] = 其它字符Entry.Text[随机生成器.Next(0, 其它字符Entry.Text.Length)];
				if (最小长度 > 密码长度byte)
					throw new Exception("指定的密码长度不可能满足每种字符至少一个的要求");
			}
			生成结果string = new string(Combinatorics.SelectPermutation(生成密码.Take(最小长度).Concat(Combinatorics.SelectVariationWithRepetition((数字.IsChecked ? 所有数字 : "") + (小写字母.IsChecked ? 所有小写 : "") + (大写字母.IsChecked ? 所有大写 : "") + (其它字符CheckBox.IsChecked ? 其它字符Entry.Text : ""), 密码长度byte - 最小长度))).ToArray());

		}
		catch(Exception ex) 
		{
			生成结果string = ex.Message;
		}
		if (自动清除记录.IsChecked)
			生成结果.Html = HttpUtility.HtmlEncode(生成结果string);
		else
			生成结果.Html += HttpUtility.HtmlEncode("【" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "】" + 生成结果string) + "<br/>";
		Default.Set("密码.生成结果", 生成结果.Html);
	}
}