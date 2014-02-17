using System;
namespace MarkdownPad2.Utilities
{
	public static class NumericUtilities
	{
		public static double SanitizeDouble(this double input, double fallback)
		{
			if (!double.IsInfinity(input) && !double.IsNaN(input))
			{
				return input;
			}
			return fallback;
		}
		public static bool IsInfinityOrNaN(params double[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				double d = values[i];
				if (double.IsInfinity(d) || double.IsNaN(d))
				{
					return true;
				}
			}
			return false;
		}
	}
}
