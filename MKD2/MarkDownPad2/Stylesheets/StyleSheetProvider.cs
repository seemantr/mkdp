using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace MarkdownPad2.Stylesheets
{
	public class StyleSheetProvider
	{
		private const string StyleSheetResourcePath = "MarkdownPad2.Stylesheets.";
		private const string StyleSheetAppDataPath = "\\MarkdownPad 2\\styles\\";
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		public static string StylesheetDirectory
		{
			get
			{
				return System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\MarkdownPad 2\\styles\\";
			}
		}
		public void WriteBuiltInStylesheetsToAppData(bool overwriteExisting = false)
		{
			System.Collections.Generic.List<string> builtInStylesheets = StyleSheetProvider.GetBuiltInStylesheets();
			foreach (string current in builtInStylesheets)
			{
				string str = current.Replace("MarkdownPad2.Stylesheets.", "");
				if (!overwriteExisting && System.IO.File.Exists(StyleSheetProvider.StylesheetDirectory + str))
				{
					StyleSheetProvider._logger.Trace("Style already exists in AppData, skipping: " + str);
				}
				else
				{
					System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
					using (System.IO.StreamReader streamReader = new System.IO.StreamReader(executingAssembly.GetManifestResourceStream(current)))
					{
						StyleSheetProvider._logger.Trace("Writing file to " + str);
						try
						{
							if (!System.IO.Directory.Exists(StyleSheetProvider.StylesheetDirectory))
							{
								StyleSheetProvider._logger.Trace("Stylesheet directory doesn't exist, creating it");
								System.IO.Directory.CreateDirectory(StyleSheetProvider.StylesheetDirectory);
							}
							System.IO.File.WriteAllText(StyleSheetProvider.StylesheetDirectory + str, streamReader.ReadToEnd());
						}
						catch (System.Exception exception)
						{
							StyleSheetProvider._logger.ErrorException("Error writing stylesheet to AppData: " + current, exception);
							MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_WritingStylesheetToFileMessage", true, "MarkdownPadStrings") + str, LocalizationProvider.GetLocalizedString("Error_WritingStylesheetToFileTitle", false, "MarkdownPadStrings"), exception, "");
						}
					}
				}
			}
		}
		public static System.Collections.Generic.List<string> GetBuiltInStylesheets()
		{
			System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			return (
				from n in executingAssembly.GetManifestResourceNames()
				where n.StartsWith("MarkdownPad2.Stylesheets.")
				select n).ToList<string>();
		}
	}
}
