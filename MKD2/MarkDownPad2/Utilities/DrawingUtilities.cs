using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace MarkdownPad2.Utilities
{
	public static class DrawingUtilities
	{
		public static ImageSource ToImageSource(this Icon icon)
		{
			return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
		}
		public static SolidColorBrush ToBrush(this System.Windows.Media.Color input)
		{
			return (SolidColorBrush)new BrushConverter().ConvertFromString(input.ToString());
		}
	}
}
