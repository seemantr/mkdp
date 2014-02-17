using BugSheet;
using MarkdownPad2.Properties;
using MarkdownPad2.Settings;
using MarkdownPad2.Utilities;
using System;
namespace MarkdownPad2.Core
{
	public static class BugReportHelper
	{
		public static void ShowBugReport(System.Exception exception = null, string exceptionDetails = "")
		{
			BugReporter bugReporter = new BugReporter
			{
				SoftwareTitle = "MarkdownPad 2",
				SoftwareVersion = AssemblyUtilities.Version,
				WebBugReportUrl = Urls.MarkdownPad_BugReportWeb,
				Exception = exception,
				ExceptionDetails = exceptionDetails,
				CurrentCulture = Settings.Default.App_Locale,
				User = "YnVnc0BtYXJrZG93bnBhZC5jb20",
				Pass = "Zmpyem91dnV6eXFucXZhbw".ToSecureString()
			};
			bugReporter.Show();
		}
	}
}
