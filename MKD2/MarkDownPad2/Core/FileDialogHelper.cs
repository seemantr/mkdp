using MarkdownPad2.Properties;
using System;
namespace MarkdownPad2.Core
{
	public class FileDialogHelper
	{
		private static Settings _settings = Settings.Default;
		public static string OpenImageFileTypes
		{
			get
			{
				string[] value = new string[]
				{
					"Image Files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif",
					"All Files (*.*)|*.*"
				};
				return string.Join("|", value);
			}
		}
		public static string OpenMarkdownFileTypes
		{
			get
			{
				string[] value = new string[]
				{
					"Markdown Files (*.md, *.txt, *.markdown, *.mdown)|*.md;*.txt;*.markdown;*.mdown",
					"Text Files (*.txt)|*.txt",
					"All Files (*.*)|*.*"
				};
				return string.Join("|", value);
			}
		}
	}
}
