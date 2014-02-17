using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace MarkdownPad2.Converters
{
	internal class ColorToBrushConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return value;
			}
			Color color = (Color)ColorConverter.ConvertFromString(value.ToString());
			return new SolidColorBrush(color);
		}
		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return Colors.White;
			}
			return (Color)ColorConverter.ConvertFromString(value.ToString());
		}
	}
}
