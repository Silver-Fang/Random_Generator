using System.Numerics;

namespace Random_Generator
{
	internal static class 算法
	{
		internal static readonly Random 随机生成器 = new Random();
		internal static IEnumerable<double> 连续指数分布(double 最小值, double 最大值, double 平均值)
		{
			double 斜率 = 最大值 - 最小值;
			double 指数 = 平均值 == 最小值 ? double.PositiveInfinity : (最大值 - 平均值) / (平均值 - 最小值);
			for (; ; )
				yield return Math.Pow(随机生成器.NextDouble(), 指数) * 斜率 + 最小值;
		}
	}
	internal static class 数值操作
	{
		private static class 特化器<T>
		{
			internal static Func<string, T> Parse;
		}
		internal static T Parse<T>(string 值)
		{
			return 特化器<T>.Parse(值);
		}
		static 数值操作()
		{
			特化器<uint>.Parse = uint.Parse;
			特化器<double>.Parse = double.Parse;
			特化器<BigInteger>.Parse = BigInteger.Parse;
		}
	}
}
