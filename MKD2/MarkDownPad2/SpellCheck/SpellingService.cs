using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using MarkdownPad2.Utilities;
using NHunspell;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
namespace MarkdownPad2.SpellCheck
{
	public class SpellingService
	{
		private static readonly Settings _settings = Settings.Default;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private Hunspell speller;
		private System.Globalization.CultureInfo CurrentCulture
		{
			get;
			set;
		}
		private string CustomDictionaryPath
		{
			get
			{
				return System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + string.Format("\\MarkdownPad 2\\dictionaries\\custom.{0}.dic", this.CurrentCulture.Name);
			}
		}
		private static string DictionaryPath
		{
			get
			{
				string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uriBuilder = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uriBuilder.Path);
				return System.IO.Path.GetDirectoryName(path) + "\\SpellCheck\\dictionaries\\";
			}
		}
		public static System.Collections.Generic.List<System.Globalization.CultureInfo> CultureLookup
		{
			get
			{
				System.Collections.Generic.List<System.Globalization.CultureInfo> list = new System.Collections.Generic.List<System.Globalization.CultureInfo>();
				if (!System.IO.Directory.Exists(SpellingService.DictionaryPath))
				{
					return list;
				}
				string[] array = new string[0];
				try
				{
					array = System.IO.Directory.GetFiles(SpellingService.DictionaryPath, "*.aff");
				}
				catch (System.Exception exception)
				{
					SpellingService._logger.ErrorException("Error occurred while reading dictionary directory", exception);
				}
				SpellingService._logger.Trace("Found dictionary files: " + array.Length);
				if (array.Length <= 0)
				{
					return list;
				}
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					string name = System.IO.Path.GetFileNameWithoutExtension(text).Replace("_", "-");
					System.Globalization.CultureInfo cultureInfo = null;
					try
					{
						cultureInfo = System.Globalization.CultureInfo.GetCultureInfo(name);
					}
					catch (System.Exception exception2)
					{
						SpellingService._logger.ErrorException("Error getting CultureInfo from filename: " + text, exception2);
					}
					if (cultureInfo != null)
					{
						list.Add(cultureInfo);
					}
				}
				return list;
			}
		}
		public bool Spell(string word)
		{
			return this.speller == null || this.speller.IsDisposed || this.speller.Spell(word);
		}
		public System.Collections.Generic.IEnumerable<string> Suggestions(string word)
		{
			return this.speller.Suggest(word);
		}
		public void ClearLanguages()
		{
			this.speller = null;
		}
		public void AddWordToCustomDictionary(string word)
		{
			string text = string.Empty;
			string directoryName = System.IO.Path.GetDirectoryName(this.CustomDictionaryPath);
			if (!System.IO.Directory.Exists(directoryName))
			{
				SpellingService._logger.Trace("No dictionary folder found, I think I'll create one");
				try
				{
					System.IO.Directory.CreateDirectory(directoryName);
				}
				catch (System.Exception exception)
				{
					SpellingService._logger.ErrorException("Unable to create custom spell check dictionary folder", exception);
				}
			}
			if (System.IO.File.Exists(this.CustomDictionaryPath))
			{
				text = System.IO.File.ReadAllText(this.CustomDictionaryPath);
			}
			string[] source = text.Split(new string[]
			{
				System.Environment.NewLine
			}, System.StringSplitOptions.RemoveEmptyEntries);
			if (!source.Contains(word))
			{
				SpellingService._logger.Trace("Adding: " + word + " to: " + this.CustomDictionaryPath);
				try
				{
					System.IO.File.AppendAllText(this.CustomDictionaryPath, word + System.Environment.NewLine);
					goto IL_104;
				}
				catch (System.Exception exception2)
				{
					MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_SavingCustomDictionaryMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_SavingCustomDictionaryTitle", false, "MarkdownPadStrings"), exception2, "");
					goto IL_104;
				}
			}
			SpellingService._logger.Trace("Word: " + word + " already exists in dictionary.");
			IL_104:
			this.LoadCustomWords();
		}
		private void LoadCustomWords()
		{
			string customDictionaryPath = this.CustomDictionaryPath;
			if (!System.IO.File.Exists(customDictionaryPath))
			{
				SpellingService._logger.Info("No custom dictionary found at " + customDictionaryPath);
				return;
			}
			if (this.speller.IsDisposed)
			{
				SpellingService._logger.Warn("Speller was disposed when trying to add custom words to dictionary.");
				return;
			}
			string source = System.IO.File.ReadAllText(customDictionaryPath);
			string[] array = source.SplitLines();
			SpellingService._logger.Trace("Adding " + array.Length + " custom words to the dictionary");
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string word = array2[i];
				this.speller.Add(word);
			}
		}
		public void SetLanguage(System.Globalization.CultureInfo culture)
		{
			this.speller = new Hunspell();
			if (!SpellingService.CultureLookup.Contains(culture))
			{
				SpellingService._logger.Error("Tried to set language that didn't exist: " + culture);
				return;
			}
			this.CurrentCulture = culture;
			string str = culture.Name.Replace("-", "_");
			System.Collections.Generic.Dictionary<string, string> dictionary = new System.Collections.Generic.Dictionary<string, string>
			{

				{
					SpellingService.DictionaryPath + str + ".aff",
					SpellingService.DictionaryPath + str + ".dic"
				}
			};
			foreach (System.Collections.Generic.KeyValuePair<string, string> current in dictionary)
			{
				SpellingService._logger.Trace(string.Format("Loading .aff: {0} and .dic {1}", current.Key, current.Value));
				byte[] array = null;
				byte[] array2 = null;
				try
				{
					array = System.IO.File.ReadAllBytes(current.Key);
					array2 = System.IO.File.ReadAllBytes(current.Value);
				}
				catch (System.Exception exception)
				{
					MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_LoadingSpellCheckDictionaryMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_LoadingSpellCheckDictionaryTitle", false, "MarkdownPadStrings"), exception, "");
				}
				if (array == null || array2 == null)
				{
					return;
				}
				this.speller.Load(array, array2);
			}
			this.LoadCustomWords();
		}
	}
}
