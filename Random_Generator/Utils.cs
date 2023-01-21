using MathNet.Numerics.Random;
using System.Web;
using System.Runtime.CompilerServices;
namespace Random_Generator
{
	internal static class Combinatorics
	{
		public static void SelectPermutationInplace<T>(this IList<T> data, int startIndex, int endIndex, Random randomSource = null)
		{
			var random = randomSource ?? SystemRandomSource.Default;

			// Fisher-Yates Shuffling
			for (int i = endIndex; i > startIndex; i--)
			{
				int swapIndex = random.Next(startIndex, i + 1);
				(data[i], data[swapIndex]) = (data[swapIndex], data[i]);
			}
		}
	}
	internal static class Utils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string HtmlPre(this string 输入)
		{
			return "<style>pre{white-space:pre-wrap;word-wrap: break-word;}</style><pre>" + Environment.NewLine + HttpUtility.HtmlEncode(输入) + "</pre>";
		}
	}
}
