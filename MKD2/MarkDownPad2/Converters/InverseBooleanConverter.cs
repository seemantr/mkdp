using System;
using System.Globalization;
using System.Windows.Data;
namespace MarkdownPad2.Converters
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(bool))
			{
				throw new System.InvalidOperationException("The target must be a boolean");
			}
			return !(bool)value;
		}
		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotSupportedException();
		}
	}
}
