namespace Random_Generator;
using static 随机生成算法;
using static Preferences;
using MathNet.Numerics.Distributions;
using System.Numerics;

public partial class 数值 : ContentPage
{
	public 数值()
	{
		InitializeComponent();
		最小值.Text = Default.Get("数值.最小值", "");
		最大值.Text = Default.Get("数值.最大值", "");
		浮点.IsChecked = Default.Get("数值.浮点", false);
		平均值CheckBox.IsChecked = Default.Get("数值.均值CheckBox", false);
		平均值Entry.Text = Default.Get("数值.均值", "");
		标准差CheckBox.IsChecked = Default.Get("数值.标准差CheckBox", false);
		标准差Entry.Text = Default.Get("数值.标准差", "");
		无重复.IsChecked = Default.Get("数值.无重复", true);
		生成个数.Text = Default.Get("数值.生成个数", "1");
	}
	//返回解析是否失败
	private bool 尝试解析<T>(Entry 条目, Func<string, T> 解析器, ref T 返回值)
	{
		try
		{
			返回值 = 解析器(条目.Text);
		}
		catch (Exception ex)
		{
			生成结果.Text = 条目.Placeholder + ex.Message;
			return true;
		}
		Default.Set("数值." + 条目.Placeholder, 条目.Text);
		return false;
	}
	private void 枚举随机数<T>(IEnumerable<T> 枚举器)
	{
		System.Text.StringBuilder 拼接器 = new System.Text.StringBuilder();
		foreach (T a in 枚举器)
			拼接器.Append(a.ToString()).Append(' ');
		生成结果.Text = 拼接器.ToString();
	}
	private void 生成_Clicked(object sender, EventArgs e)
	{
		uint 生成个数uint = 0;
		if (尝试解析(生成个数, uint.Parse, ref 生成个数uint))
			return;
		double 均值 = 0;
		if (平均值CheckBox.IsChecked && 尝试解析(平均值Entry, double.Parse, ref 均值))
			return;
		double 方差 = 0;
		if (平均值CheckBox.IsChecked && 尝试解析(平均值Entry, double.Parse, ref 方差))
			return;
		if (浮点.IsChecked)
		{
			double 最小值double = 0;
			if (尝试解析(最小值, double.Parse, ref 最小值double))
				return;
			double 最大值double = 0;
			if (尝试解析(最大值, double.Parse, ref 最大值double))
				return;
			double 范围 = 最大值double - 最小值double;
			if (标准差CheckBox.IsChecked)
				{
					方差 /= 范围;
					方差 *= 方差;
				}
			if (平均值CheckBox.IsChecked)
				if (标准差CheckBox.IsChecked)
				{
					均值 = (均值 - 最小值double) / 范围;
					double 共轭均值 = 1 - 均值;
					方差 = 均值 * 共轭均值 / 方差 - 1;
					均值 *= 方差;
					共轭均值 *= 方差;
					if (均值 < 0 || 共轭均值 < 0)
						生成结果.Text = "指定的范围和平均值条件下，不可能实现如此大的标准差";
					else
						枚举随机数(from double a in Beta.Samples(均值, 共轭均值).Take((int)生成个数uint) select a * 范围 + 最小值double);
				}
				else
					枚举随机数(连续指数分布(最小值double, 最大值double, 均值).Take((int)生成个数uint));
			else if (标准差CheckBox.IsChecked)
			{
				方差 = 1 / (8 * 方差) - 0.5;
				if (方差 < 0)
					生成结果.Text = "不可能实现如此大的标准差";
				else
					枚举随机数(from double a in Beta.Samples(方差, 方差).Take((int)生成个数uint) select a * 范围 + 最小值double);
			}
			else
				枚举随机数(ContinuousUniform.Samples(最小值double, 最大值double).Take((int)生成个数uint));
		}
		else
		{
			BigInteger 最小值BigInteger = 0;
			if (尝试解析(最小值, BigInteger.Parse, ref 最小值BigInteger))
				return;
			BigInteger 最大值BigInteger = 0;
			if (尝试解析(最大值, BigInteger.Parse, ref 最大值BigInteger))
				return;
			double 范围 = 0;
			if (标准差CheckBox.IsChecked)
			{
				范围 = (double)(最大值BigInteger - 最小值BigInteger);
				方差 /= 范围;
				方差 *= 方差;
			}
			if (平均值CheckBox.IsChecked)
			{
				double 最小值double = (double)最小值BigInteger;
				double 最大值double = (double)最大值BigInteger;
				if (标准差CheckBox.IsChecked)
				{
					均值 = (均值 - 最小值double) / 范围;
					double 共轭均值 = 1 - 均值;
					方差 = 均值 * 共轭均值 / 方差 - 1;
					均值 *= 方差;
					共轭均值 *= 方差;
					if (均值 < 0 || 共轭均值 < 0)
						生成结果.Text = "指定的范围和平均值条件下，不可能实现如此大的标准差";
					else
						枚举随机数(from double a in Beta.Samples(均值, 共轭均值).Take((int)生成个数uint) select a * 范围 + 最小值double);
				}
				else
					枚举随机数(连续指数分布(最小值double, 最大值double, 均值).Take((int)生成个数uint));
			}
			else if (标准差CheckBox.IsChecked)
			{
				方差 = 1 / (8 * 方差) - 0.5;
				if (方差 < 0)
					生成结果.Text = "不可能实现如此大的标准差";
				else
					枚举随机数(from double a in Beta.Samples(方差, 方差).Take((int)生成个数uint) select a * 范围 + 最小值double);
			}
			else
				枚举随机数(ContinuousUniform.Samples(最小值double, 最大值double).Take((int)生成个数uint));
		}
	}

	private void 浮点_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		无重复.IsEnabled = !浮点.IsChecked;
	}

	private void 无重复_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		浮点.IsEnabled = !无重复.IsChecked;
	}
}