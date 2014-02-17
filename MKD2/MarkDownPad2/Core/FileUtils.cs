using MarkdownPad2.i18n;
using MarkdownPad2.Utilities;
using NLog;
using System;
using System.IO;
namespace MarkdownPad2.Core
{
	public static class FileUtils
	{
		public static Logger _logger = LogManager.GetCurrentClassLogger();
		public static void MoveFile(string file, string destination)
		{
			try
			{
				if (System.IO.File.Exists(destination))
				{
					FileUtils._logger.Info("Overwriting existing file: " + destination);
					System.IO.File.Delete(destination);
				}
				System.IO.File.Move(file, destination);
			}
			catch (System.Exception exception)
			{
				FileUtils._logger.ErrorException(string.Format("Unable to move file from source ({0}) to destination ({1})", file, destination), exception);
				MessageBoxHelper.ShowErrorMessageBox(string.Concat(new string[]
				{
					LocalizationProvider.GetLocalizedString("Error_MovingFile_Message", false, "MarkdownPadStrings"),
					StringUtilities.GetNewLines(2),
					LocalizationProvider.GetLocalizedString("Error_MovingFileSource", true, "MarkdownPadStrings"),
					file,
					StringUtilities.GetNewLines(2),
					LocalizationProvider.GetLocalizedString("Error_MovingFileDestination", true, "MarkdownPadStrings"),
					destination
				}), LocalizationProvider.GetLocalizedString("Error_MovingFile_Title", false, "MarkdownPadStrings"), exception, "");
			}
		}
		public static void MoveFilesFromDirectory(string sourceDirectory, string destinationDirectory, string filter = "*.*")
		{
			if (!System.IO.Directory.Exists(sourceDirectory))
			{
				return;
			}
			if (!System.IO.Directory.Exists(destinationDirectory))
			{
				try
				{
					FileUtils._logger.Info("Destination directory doesn't exist, creating it: " + destinationDirectory);
					System.IO.Directory.CreateDirectory(destinationDirectory);
				}
				catch (System.Exception exception)
				{
					FileUtils._logger.ErrorException("Unable to create directory: " + destinationDirectory, exception);
					MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_CreateDirectory", false, "MarkdownPadStrings") + ": " + destinationDirectory, LocalizationProvider.GetLocalizedString("Error_CreateDirectory", false, "MarkdownPadStrings"), exception, "");
				}
			}
			string[] files = System.IO.Directory.GetFiles(sourceDirectory, filter);
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string text2 = destinationDirectory + System.IO.Path.GetFileName(text);
				FileUtils._logger.Info(string.Format("Moving file ({0}) to destination ({1})", text, text2));
				FileUtils.MoveFile(text, text2);
			}
			if (System.IO.Directory.GetFiles(sourceDirectory).Length != 0)
			{
				return;
			}
			try
			{
				System.IO.Directory.Delete(sourceDirectory);
			}
			catch (System.Exception exception2)
			{
				FileUtils._logger.ErrorException("Unable to delete directory: " + sourceDirectory, exception2);
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_DeleteDirectory", false, "MarkdownPadStrings") + ": " + sourceDirectory, LocalizationProvider.GetLocalizedString("Error_DeleteDirectory", false, "MarkdownPadStrings"), exception2, "");
			}
		}
	}
}
