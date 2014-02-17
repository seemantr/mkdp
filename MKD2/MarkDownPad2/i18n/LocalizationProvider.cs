using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using WPFLocalizeExtension.Extensions;
namespace MarkdownPad2.i18n
{
	public static class LocalizationProvider
	{
		public static System.Collections.Generic.List<System.Globalization.CultureInfo> AvailableCultures
		{
			get
			{
				System.Collections.Generic.List<System.Globalization.CultureInfo> source = new System.Collections.Generic.List<System.Globalization.CultureInfo>
				{
					new System.Globalization.CultureInfo("en-US"),
					new System.Globalization.CultureInfo("pt-PT"),
					new System.Globalization.CultureInfo("zh-CN"),
					new System.Globalization.CultureInfo("de-DE"),
					new System.Globalization.CultureInfo("pl-PL"),
					new System.Globalization.CultureInfo("ru-RU"),
					new System.Globalization.CultureInfo("fi-FI"),
					new System.Globalization.CultureInfo("it-IT"),
					new System.Globalization.CultureInfo("ko-KR"),
					new System.Globalization.CultureInfo("ja-JP"),
					new System.Globalization.CultureInfo("fr-FR"),
					new System.Globalization.CultureInfo("da-DK")
				};
				return (
					from o in source
					orderby o.DisplayName
					select o).ToList<System.Globalization.CultureInfo>();
			}
		}
		public static string GetLocalizedString(string key, bool addSpaceAfter = false, string resourceFileName = "MarkdownPadStrings")
		{
			string text = string.Empty;
			string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
			string key2 = string.Concat(new string[]
			{
				name,
				":",
				resourceFileName,
				":",
				key
			});
			LocExtension locExtension = new LocExtension(key2);
			locExtension.ResolveLocalizedValue<string>(out text);
			if (addSpaceAfter)
			{
				text += " ";
			}
			return text;
		}
		public static void BindLocalizedString(string key, System.Windows.DependencyObject dependencyObject, object targetProperty)
		{
			string arg_05_0 = string.Empty;
			LocExtension locExtension = new LocExtension(key);
			locExtension.SetBinding(dependencyObject, targetProperty);
		}
	}
}
