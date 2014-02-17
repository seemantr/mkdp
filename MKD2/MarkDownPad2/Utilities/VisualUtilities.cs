using System;
using System.Windows;
using System.Windows.Media;
namespace MarkdownPad2.Utilities
{
	public static class VisualUtilities
	{
		public static T FindVisualChild<T>(this System.Windows.DependencyObject obj) where T : System.Windows.DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				System.Windows.DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
				{
					return (T)((object)child);
				}
				T t = child.FindVisualChild<T>();
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}
	}
}
