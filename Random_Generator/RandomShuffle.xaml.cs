namespace Random_Generator;
using System.Web;
using static Preferences;
using MathNet.Numerics;
using System.Text;
using System.Runtime.InteropServices;

public partial class RandomShuffle : ContentPage
{
	public RandomShuffle()
	{
		InitializeComponent();
		输入字符串.Text = Default.Get("乱序.输入字符串", "");
		保留首尾字符.IsChecked = Default.Get("乱序.保留首尾字符", false);
		分隔符.Text = Default.Get("乱序", "");
		自动清除记录.IsChecked = Default.Get("乱序.自动清除记录", false);
		生成结果.Html = Default.Get("乱序.生成结果", "");
	}

	private void 自动清除记录_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("乱序.自动清除记录", 自动清除记录.IsChecked);
	}

	private void 复制到剪贴板_Clicked(object sender, EventArgs e)
	{
		Clipboard.SetTextAsync(HttpUtility.HtmlDecode(生成结果.Html.Replace("<br/>", Environment.NewLine)));
	}

	private void 清除记录_Clicked(object sender, EventArgs e)
	{
		生成结果.Html = "";
		Default.Set("乱序.生成结果", "");
	}

	private void 保留首尾字符_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		分隔符.IsEnabled = false;
		Default.Set("乱序.保留首尾字符", 保留首尾字符.IsChecked);
	}

	private void 生成_Clicked(object sender, EventArgs e)
	{
		string 生成结果string;
		if (分隔符.IsEnabled && 分隔符.Text.Any())
			生成结果string = string.Join(分隔符.Text, 输入字符串.Text.Split(分隔符.Text).SelectPermutation());
		else
		{
			uint[] 缓冲区 = new uint[输入字符串.Text.Length];
			int 字数 = Encoding.UTF32.GetBytes(输入字符串.Text, MemoryMarshal.AsBytes(缓冲区.AsSpan())) / 4;
			if(保留首尾字符.IsChecked)
			{

			}
		}
		if (自动清除记录.IsChecked)
			生成结果.Html = HttpUtility.HtmlEncode(生成结果string);
		else
			生成结果.Html += HttpUtility.HtmlEncode("【" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "】" + 生成结果string) + "<br/>";
		Default.Set("乱序.生成结果", 生成结果.Html);
	}
}