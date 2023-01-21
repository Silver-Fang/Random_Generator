namespace Random_Generator;
using static Preferences;
using MathNet.Numerics;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public partial class RandomShuffle : ContentPage
{
	string 记录文本;
	public RandomShuffle()
	{
		InitializeComponent();
		输入字符串.Text = Default.Get("乱序.输入字符串", "");
		保留首尾字符.IsChecked = Default.Get("乱序.保留首尾字符", false);
		分隔符.Text = Default.Get("乱序", "");
		自动清除记录.IsChecked = Default.Get("乱序.自动清除记录", false);
		生成结果.Html = (记录文本 = Default.Get("乱序.记录文本", "")).HtmlPre();
	}

	private void 自动清除记录_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("乱序.自动清除记录", 自动清除记录.IsChecked);
	}

	private void 复制到剪贴板_Clicked(object sender, EventArgs e)
	{
		Clipboard.SetTextAsync(记录文本);
	}

	private void 清除记录_Clicked(object sender, EventArgs e)
	{
		Default.Set("乱序.记录文本", 生成结果.Html = 记录文本 = "");
	}

	private void 保留首尾字符_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		分隔符.IsEnabled = !保留首尾字符.IsChecked;
		Default.Set("乱序.保留首尾字符", 保留首尾字符.IsChecked);
	}

	private void 生成_Clicked(object sender, EventArgs e)
	{
		string 生成结果string;
		Default.Set("乱序.输入字符串", 输入字符串.Text);
		if (分隔符.IsEnabled && 分隔符.Text.Any())
			生成结果string = string.Join(分隔符.Text, 输入字符串.Text.Split(分隔符.Text).SelectPermutation());
		else
		{
			uint[] 缓冲区 = new uint[输入字符串.Text.Length];
			int 字数 = Encoding.UTF32.GetBytes(输入字符串.Text, MemoryMarshal.AsBytes(缓冲区.AsSpan())) / 4;
			if (保留首尾字符.IsChecked)
			{
				if (字数 > 2)
					缓冲区.SelectPermutationInplace(1, 字数 - 2);
			}
			else
				缓冲区.SelectPermutationInplace(0, 字数 - 1);
			生成结果string = Encoding.UTF32.GetString(MemoryMarshal.AsBytes(缓冲区.AsSpan(0, 字数)));
		}
		if (自动清除记录.IsChecked)
			记录文本 = 生成结果string;
		else
			记录文本 += "【" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "】" + 生成结果string + Environment.NewLine;
		生成结果.Html = 记录文本.HtmlPre();
		Default.Set("乱序.记录文本", 记录文本);
	}
}