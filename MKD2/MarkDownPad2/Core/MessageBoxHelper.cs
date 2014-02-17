using MarkdownPad2.i18n;
using MarkdownPad2.Utilities;
using System;
using System.Windows;
using WPFCustomMessageBox;
namespace MarkdownPad2.Core
{
	public static class MessageBoxHelper
	{
		public static void ShowErrorMessageBox(string message, string title, System.Exception exception = null, string exceptionDetails = "")
		{
			string localizedString = LocalizationProvider.GetLocalizedString("OK", false, "MarkdownPadStrings");
			string localizedString2 = LocalizationProvider.GetLocalizedString("Button_ReportBug", false, "MarkdownPadStrings");
			if (exception != null)
			{
				message = message + StringUtilities.GetNewLines(2) + exception.Message;
			}
			message = message + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("IfBugPersists", false, "MarkdownPadStrings");
			MessageBoxResult messageBoxResult = CustomMessageBox.ShowYesNo(message, title, localizedString, localizedString2, MessageBoxImage.Hand);
			if (messageBoxResult != MessageBoxResult.No)
			{
				return;
			}
			BugReportHelper.ShowBugReport(exception, exceptionDetails);
		}
		public static void ShowWarningMessageBox(string message, string title)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("OK", false, "MarkdownPadStrings");
			CustomMessageBox.ShowOK(message, title, localizedString, MessageBoxImage.Exclamation);
		}
	}
}
