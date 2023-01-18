namespace Random_Generator;
using System.Web;
using static Preferences;
using static MauiProgram;
using System.Collections.ObjectModel;
using MathNet.Numerics;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public partial class RandomChinese : ContentPage
{
	private struct 字符集信息
	{
		internal string 名称;
		internal Lazy<Task<MemoryStream>> 流;
		internal 字符集信息(string 名称)
		{
			this.名称 = 名称;
			流 = new Lazy<Task<MemoryStream>>(async () =>
			{
				//Android不支持文件流反复读，所以只能第一次读完留在内存
				MemoryStream 返回 = new();
				(await FileSystem.OpenAppPackageFileAsync(名称 + ".u32")).CopyTo(返回);
				return 返回;
			});
		}
	}
	private readonly Dictionary<CheckBox, 字符集信息> 字符集 = new();
	public RandomChinese()
	{
		InitializeComponent();
		无重复.IsChecked = Default.Get("汉字.无重复", false);
		生成个数.Text = Default.Get("汉字.生成个数", "");
		生成结果.Html = Default.Get("汉字.生成结果", "");
		自动清除记录.IsChecked = Default.Get("汉字.自动清除记录", true);
		字符集[一级规范字] = new 字符集信息("一级规范字");
		字符集[二级规范字] = new 字符集信息("二级规范字");
		字符集[三级规范字] = new 字符集信息("三级规范字");
		字符集[中日韩统一表意文字] = new 字符集信息("中日韩统一表意文字");
		字符集[扩展A] = new 字符集信息("扩展A");
		字符集[扩展B] = new 字符集信息("扩展B");
		字符集[扩展C] = new 字符集信息("扩展C");
		字符集[扩展D] = new 字符集信息("扩展D");
		foreach (KeyValuePair<CheckBox, 字符集信息> a in 字符集)
			a.Key.IsChecked = Default.Get("汉字." + a.Value.名称, false);
	}

	private void 自动清除记录_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("汉字.自动清除记录", 自动清除记录.IsChecked);
	}

	private void 复制到剪贴板_Clicked(object sender, EventArgs e)
	{
		Clipboard.SetTextAsync(HttpUtility.HtmlDecode(生成结果.Html.Replace("<br/>", Environment.NewLine)));
	}

	private void 清除记录_Clicked(object sender, EventArgs e)
	{
		生成结果.Html = "";
		Default.Set("汉字.生成结果", "");
	}

	private void 无重复_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("汉字.无重复", 无重复.IsChecked);
	}

	private void 字符集_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("汉字." + 字符集[(CheckBox)sender].名称, ((CheckBox)sender).IsChecked);
	}
	private async void 生成_Clicked(object sender, EventArgs e)
	{
		string 生成结果string;
		uint 总字节数 = 0;
		try
		{
			int 生成个数int = (int)数值解析<uint>(生成个数);
			//Android不支持Unsafe.As，所以必须用uint[]收集字符集，所以必须先取得总字数
			Collection<MemoryStream> 收集字符集 = new();
			foreach (KeyValuePair<CheckBox, 字符集信息> a in 字符集)
				if (a.Key.IsChecked)
				{
					MemoryStream 文件 = await a.Value.流.Value;
					文件.Position = 0;
					收集字符集.Add(文件);
					总字节数 += (uint)文件.Length;
				}
			uint[] 所有字符uint = new uint[总字节数 / 4];
			总字节数 = 0;
			foreach (MemoryStream a in 收集字符集)
			{
				a.Read(MemoryMarshal.AsBytes(所有字符uint.AsSpan((int)总字节数 / 4)));
				总字节数 += (uint)a.Length;
			}
			生成结果string = Encoding.UTF32.GetString(MemoryMarshal.AsBytes((无重复.IsChecked ? Combinatorics.SelectVariation(所有字符uint.Distinct(), 生成个数int) : Combinatorics.SelectVariationWithRepetition(所有字符uint, 生成个数int)).ToArray().AsSpan()));
		}
		catch(ArgumentOutOfRangeException ex)
		{
			if (总字节数 == 0)
				生成结果string = "没有选择任何字符集";
			else
				生成结果string = ex.Message;
		}
		catch (Exception ex)
		{
			生成结果string = ex.Message;
		}
		if (自动清除记录.IsChecked)
			生成结果.Html = HttpUtility.HtmlEncode(生成结果string);
		else
			生成结果.Html += HttpUtility.HtmlEncode("【" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "】" + 生成结果string) + "<br/>";
		Default.Set("汉字.生成结果", 生成结果.Html);
	}
}