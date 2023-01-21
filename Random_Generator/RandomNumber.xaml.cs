namespace Random_Generator;
using static Preferences;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using System.Numerics;
using static MauiProgram;
using System.Web;

public partial class RandomNumber: ContentPage
{
	string 记录文本;
	public RandomNumber()
	{
		InitializeComponent();
		最小值.Text = Default.Get("数值.最小值", "");
		最大值.Text = Default.Get("数值.最大值", "");
		浮点.IsChecked = Default.Get("数值.浮点", false);
		平均值CheckBox.IsChecked = Default.Get("数值.平均值CheckBox", false);
		平均值Entry.Text = Default.Get("数值.平均值", "");
		标准差CheckBox.IsChecked = Default.Get("数值.标准差CheckBox", false);
		标准差Entry.Text = Default.Get("数值.标准差", "");
		无重复.IsChecked = Default.Get("数值.无重复", false);
		生成个数.Text = Default.Get("数值.生成个数", "");
		自动清除记录.IsChecked = Default.Get("数值.自动清除记录", true);
		生成结果.Html = (记录文本 = Default.Get("数值.记录文本", "")).HtmlPre();
	}
	private static string 枚举随机数<T>(IEnumerable<T> 枚举器)
	{
		System.Text.StringBuilder 拼接器 = new();
		foreach (T a in 枚举器)
			拼接器.Append(a.ToString()).Append(' ');
		return 拼接器.ToString();
	}
	private static IEnumerable<double> 连续指数分布(double 最小值, double 最大值, double 平均值)
	{
		double 斜率 = 最大值 - 最小值;
		double 指数 = 平均值 == 最小值 ? double.PositiveInfinity : (最大值 - 平均值) / (平均值 - 最小值);
		for (; ; )
			yield return Math.Pow(随机生成器.NextDouble(), 指数) * 斜率 + 最小值;
	}
	private IEnumerable<double> 浮点随机数()
	{
		int 生成个数int = (int)数值解析<uint>(生成个数);
		double 最小值double = 数值解析<double>(最小值);
		double 最大值double = 数值解析<double>(最大值);
		double 范围 = 最大值double - 最小值double;
		if (标准差CheckBox.IsChecked)
		{
			double 方差 = 数值解析<double>(标准差Entry) / 范围;
			方差 *= 方差;
			if (平均值CheckBox.IsChecked)
			{
				double 均值 = 数值解析<double>(平均值Entry);
				double 共轭均值 = 1 - (均值 = (均值 - 最小值double) / 范围);
				if ((均值 *= (方差 = 均值 * 共轭均值 / 方差 - 1)) < 0 || (共轭均值 *= 方差) < 0)
					throw new Exception("指定的范围和平均值条件下，不可能实现如此大的标准差");
				else
					return from double a in Beta.Samples(均值, 共轭均值).Take(生成个数int) select a * 范围 + 最小值double;
			}
			else if ((方差 = 1 / (8 * 方差) - 0.5) < 0)
				throw new Exception("不可能实现如此大的标准差");
			else
				return from double a in Beta.Samples(方差, 方差).Take(生成个数int) select a * 范围 + 最小值double;
		}
		else if (平均值CheckBox.IsChecked)
			return 连续指数分布(最小值double, 最大值double, 数值解析<double>(平均值Entry)).Take(生成个数int);
		else
			return ContinuousUniform.Samples(最小值double, 最大值double).Take(生成个数int);

	}
	private void 生成_Clicked(object sender, EventArgs e)
	{
		string 生成结果string;
		try
		{
			if (浮点.IsChecked)
				生成结果string = 枚举随机数(浮点随机数());
			else
			{
				BigInteger 最小值BigInteger = 数值解析<BigInteger>(最小值);
				BigInteger 最大值BigInteger = 数值解析<BigInteger>(最大值);
				if (最小值BigInteger >= int.MinValue && 最大值BigInteger <= int.MaxValue && 最大值BigInteger - 最小值BigInteger + 1 <= int.MaxValue)
				{
					int 最小值int = (int)最小值BigInteger;
					int 最大值int = (int)最大值BigInteger;
					IEnumerable<int> 整数值;
					if (平均值CheckBox.IsChecked || 标准差CheckBox.IsChecked)
						整数值 = from double a in 浮点随机数() select (int)Math.Round(a);
					else
					{
						int 生成个数int = (int)数值解析<uint>(生成个数);
						if (无重复.IsChecked)
							整数值 = from int a in MathNet.Numerics.Combinatorics.GenerateVariation(最大值int - 最小值int + 1, 生成个数int) select a + 最小值int;
						else
							整数值 = 随机生成器.NextInt32Sequence(最小值int, 最大值int + 1).Take(生成个数int);
					}
					生成结果string = 枚举随机数(整数值);
				}
				else
				{
					IEnumerable<BigInteger> 整数值;
					if (平均值CheckBox.IsChecked || 标准差CheckBox.IsChecked)
						整数值 = from double a in 浮点随机数() select (BigInteger)Math.Round(a);
					else
					{
						int 生成个数int = (int)数值解析<uint>(生成个数);
						if (无重复.IsChecked)
							整数值 = from BigInteger a in MathNet.Numerics.Combinatorics.GenerateVariation(最大值BigInteger - 最小值BigInteger + 1, 生成个数int) select a + 最小值BigInteger;
						else
							整数值 = 随机生成器.NextBigIntegerSequence(最小值BigInteger, 最大值BigInteger + 1).Take(生成个数int);
					}
					生成结果string = 枚举随机数(整数值);
				}
			}
		}
		catch (Exception ex)
		{
			生成结果string = ex.Message;
		}
		if (自动清除记录.IsChecked)
			记录文本 = 生成结果string;
		else
			记录文本 += "【" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "】" + 生成结果string + Environment.NewLine;
		生成结果.Html = 记录文本.HtmlPre();
		Default.Set("数值.记录文本", 记录文本);
	}

	private void 无重复_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		if(无重复.IsChecked)
			浮点.IsChecked = 平均值CheckBox.IsChecked = 标准差CheckBox.IsChecked = false;
		Default.Set("数值.无重复", 无重复.IsChecked);
	}

	private void 浮点_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		无重复.IsChecked &= !浮点.IsChecked;
		Default.Set("数值.浮点", 浮点.IsChecked);
	}

	private void 平均值CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		无重复.IsChecked &= !平均值CheckBox.IsChecked;
		Default.Set("数值.平均值CheckBox", 平均值CheckBox.IsChecked);
	}

	private void 标准差CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		无重复.IsChecked &= !标准差CheckBox.IsChecked;
		Default.Set("数值.标准差CheckBox", 标准差CheckBox.IsChecked);
	}

	private void 复制到剪贴板_Clicked(object sender, EventArgs e)
	{
		Clipboard.SetTextAsync(记录文本);
	}

	private void 清除记录_Clicked(object sender, EventArgs e)
	{
		Default.Set("数值.记录文本", 生成结果.Html = 记录文本 = "");
	}

	private void 自动清除记录_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		Default.Set("数值.自动清除记录", 自动清除记录.IsChecked);
	}
}