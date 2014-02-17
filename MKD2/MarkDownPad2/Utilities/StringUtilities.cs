using System;
using System.Text;
namespace MarkdownPad2.Utilities
{
	public static class StringUtilities
	{
		public static string ConvertTextToUnixEol(this string content)
		{
			return content.Replace("\r\n", "\n");
		}
		public static string[] SplitLines(this string source)
		{
			return source.Split(new string[]
			{
				"\r\n",
				"\n"
			}, System.StringSplitOptions.None);
		}
		public static string GetNewLines(int quantity)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			for (int i = 0; i < quantity; i++)
			{
				stringBuilder.Append(System.Environment.NewLine);
			}
			return stringBuilder.ToString();
		}
		public static string ToLower(this bool input)
		{
			return input.ToString().ToLowerInvariant();
		}
	}
}
