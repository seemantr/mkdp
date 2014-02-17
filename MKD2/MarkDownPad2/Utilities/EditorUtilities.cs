using ICSharpCode.AvalonEdit.Document;
using System;
namespace MarkdownPad2.Utilities
{
	public static class EditorUtilities
	{
		public static double[] FontSizes
		{
			get
			{
				return new double[]
				{
					2.0,
					4.0,
					6.0,
					8.0,
					9.0,
					10.0,
					11.0,
					12.0,
					14.0,
					16.0,
					18.0,
					20.0,
					24.0,
					26.0,
					28.0,
					36.0,
					48.0,
					72.0
				};
			}
		}
		public static bool IsBlankLine(this DocumentLine line)
		{
			return line.Length == 0;
		}
	}
}
