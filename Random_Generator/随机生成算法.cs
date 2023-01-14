using System.Numerics;

namespace Random_Generator
{
	internal static class 随机生成算法
	{
		private static readonly Random 随机生成器 = new Random();
		internal static IEnumerable<double> 连续指数分布(double 最小值, double 最大值, double 平均值)
		{
			double 斜率 = 最大值 - 最小值;
			double 指数 = 平均值 == 最小值 ? double.PositiveInfinity : (最大值 - 平均值) / (平均值 - 最小值);
			for (; ; )
				yield return Math.Pow(随机生成器.NextDouble(), 指数) * 斜率 + 最小值;
		}
		internal static IEnumerable<BigInteger> 取整(IEnumerable<double> 分数, BigInteger 最小值, BigInteger 最大值, bool 无重复)
		{
			if (无重复)
			{
				分数 = 分数.Order();
				double[] 分数数组 = 分数.ToArray();
				BigInteger[] 整数数组 = (from double a in 分数数组 select (BigInteger)a).ToArray();
				long 个数 = 整数数组.LongLength;
				for (long b = 0; b < 个数;)
				{
					BigInteger 样本值 = 整数数组[b];
					long c;
					for (c = b + 1; c < 个数 && 整数数组[c] == 样本值; c++) ;
					long 左界索引 = b - 1;
					long 右界索引 = c;
					for(long d=b;d<)

				}
			}
			else
				foreach (double a in 分数)
					yield return (BigInteger)a;
		}
	}
}
