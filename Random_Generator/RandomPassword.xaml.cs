namespace Random_Generator;
using static Preferences;
using static MauiProgram;
using MathNet.Numerics;
using System.Web;

public partial class RandomPassword : ContentPage
{
	private Dictionary<CheckBox, string> CheckBox名称 = new();
	string 记录文本;
	public RandomPassword()
	{
		InitializeComponent();
		其它字符Entry.Text = Default.Get("密码.其它字符", "~!@#$%^&*()_+`-={}|[]\\:\";'<>?,./");
		密码长度.Text = Default.Get("密码.密码长度", "");
		CheckBox名称[数字] = "密码.数字";
		CheckBox名称[小写字母] = "密码.小写字母";
		CheckBox名称[大写字母] = "密码.大写字母";
		CheckBox名称[其它字符CheckBox] = "密码.其它字符CheckBox";
		CheckBox名称[每种字符至少一个] = "密码.每种字符至少一个";
		CheckBox名称[自动清除记录] = "密码.自动清除记录";
		foreach (KeyValuePair<CheckBox, string> a in CheckBox名称)
			a.Key.IsChecked = Default.Get(a.Value, true);
		生成结果.Html = (记录文本 = Default.Get("密码.记录文本", "")).HtmlPre();
	}

	private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set(CheckBox名称[(CheckBox)sender], ((CheckBox)sender).IsChecked);
	}

	private void 复制到剪贴板_Clicked(object sender, EventArgs e)
	{
		Clipboard.SetTextAsync(记录文本);
	}

	private void 清除记录_Clicked(object sender, EventArgs e)
	{
		Default.Set("密码.记录文本", 生成结果.Html = 记录文本 = "");
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
				{
					生成密码[最小长度++] = 其它字符Entry.Text[随机生成器.Next(0, 其它字符Entry.Text.Length)];
					Default.Set("密码.其它字符", 其它字符Entry.Text);
				}
				if (最小长度 > 密码长度byte)
					throw new Exception("指定的密码长度不可能满足每种字符至少一个的要求");
			}
			生成结果string = new string(生成密码.Take(最小长度).Concat(((数字.IsChecked ? 所有数字 : "") + (小写字母.IsChecked ? 所有小写 : "") + (大写字母.IsChecked ? 所有大写 : "") + (其它字符CheckBox.IsChecked ? 其它字符Entry.Text : "")).SelectVariationWithRepetition(密码长度byte - 最小长度)).SelectPermutation().ToArray());

		}
		catch(ArgumentException ex)
		{
			if (数字.IsChecked || 小写字母.IsChecked || 大写字母.IsChecked || 其它字符CheckBox.IsChecked)
				生成结果string = ex.Message;
			else
				生成结果string = "没有选择任何字符集";
		}
		catch(Exception ex) 
		{
			生成结果string = ex.Message;
		}
		if (自动清除记录.IsChecked)
			记录文本 = 生成结果string;
		else
			记录文本 += "【" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "】" + 生成结果string + Environment.NewLine;
		生成结果.Html = 记录文本.HtmlPre();
		Default.Set("密码.记录文本", 记录文本);
	}
}