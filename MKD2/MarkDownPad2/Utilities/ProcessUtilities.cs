using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using NLog;
using System;
using System.Diagnostics;
namespace MarkdownPad2.Utilities
{
	public static class ProcessUtilities
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		public static void TryStartDefaultProcess(this string fileLocation)
		{
			if (string.IsNullOrEmpty(fileLocation))
			{
				return;
			}
			try
			{
				Process.Start(fileLocation);
			}
			catch (System.Exception exception)
			{
				ProcessUtilities._logger.ErrorException("Exception trying to start process", exception);
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_OpeningDefaultApplicationMessage", true, "MarkdownPadStrings") + fileLocation, LocalizationProvider.GetLocalizedString("Error_OpeningDefaultApplicationTitle", false, "MarkdownPadStrings"), exception, "");
			}
		}
		public static void TryStartSpecificProcess(this string fileLocation, string process, string argument)
		{
			if (string.IsNullOrEmpty(fileLocation))
			{
				return;
			}
			try
			{
				Process.Start(process, argument + fileLocation);
			}
			catch (System.Exception exception)
			{
				ProcessUtilities._logger.ErrorException("Exception trying to start process", exception);
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_OpeningDefaultApplicationMessage", true, "MarkdownPadStrings") + fileLocation, LocalizationProvider.GetLocalizedString("Error_OpeningDefaultApplicationTitle", false, "MarkdownPadStrings"), exception, "");
			}
		}
	}
}
