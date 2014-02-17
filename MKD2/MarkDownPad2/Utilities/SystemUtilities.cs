using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
namespace MarkdownPad2.Utilities
{
	public class SystemUtilities
	{
		[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
		private static extern System.IntPtr GetOpenClipboardWindow();
		[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowThreadProcessId(System.IntPtr hWnd, out int lpdwProcessId);
		public static Process GetProcessLockingClipboard()
		{
			int processId;
			SystemUtilities.GetWindowThreadProcessId(SystemUtilities.GetOpenClipboardWindow(), out processId);
			return Process.GetProcessById(processId);
		}
		public static bool CopyStringToClipboard(string textToCopy)
		{
			for (int i = 1; i <= 3; i++)
			{
				try
				{
					Clipboard.SetText(textToCopy);
					return true;
				}
				catch (System.Runtime.InteropServices.COMException exception)
				{
					if (i == 3)
					{
						Process processLockingClipboard = SystemUtilities.GetProcessLockingClipboard();
						string message = string.Concat(new string[]
						{
							LocalizationProvider.GetLocalizedString("Error_ClipboardLockedMessage", false, "MarkdownPadStrings"),
							StringUtilities.GetNewLines(2),
							LocalizationProvider.GetLocalizedString("Error_ClipboardLockedBy", true, "MarkdownPadStrings"),
							processLockingClipboard.ProcessName,
							" (",
							LocalizationProvider.GetLocalizedString("Error_ClipboardLockedWindowTitle", true, "MarkdownPadStrings"),
							processLockingClipboard.MainWindowTitle,
							")"
						});
						MessageBoxHelper.ShowErrorMessageBox(message, LocalizationProvider.GetLocalizedString("Error_CopyToClipboardTitle", false, "MarkdownPadStrings"), exception, string.Format("Process Name: {0} Process Title: {1}", processLockingClipboard.ProcessName, processLockingClipboard.MainWindowTitle));
					}
				}
				catch (System.Exception exception2)
				{
					if (i == 3)
					{
						MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_CopyToClipboardMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_CopyToClipboardTitle", false, "MarkdownPadStrings"), exception2, "");
					}
				}
			}
			return false;
		}
		public static bool IsWindowWithinScreenBounds(double left, double width, double top, double height, double systemWidth, double systemHeight)
		{
			return left >= 0.0 && top >= 0.0 && left + width <= systemWidth && top + height <= systemHeight;
		}
	}
}
