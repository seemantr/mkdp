using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using MarkdownSharp;
using NLog;
using System;
using System.Collections.Generic;
namespace MarkdownPad2.Markdown
{
	public class MarkdownClassicProcessor : IMarkdownProcessor
	{
		private Settings _settings = Settings.Default;
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly Markdown _markdownProcessor;
		public bool BackgroundRenderingAllowed
		{
			get
			{
				return true;
			}
		}
		public System.TimeSpan RenderDelay
		{
			get;
			private set;
		}
		public string ProcessorName
		{
			get
			{
				return LocalizationProvider.GetLocalizedString("MarkdownClassic", false, "MarkdownPadStrings");
			}
		}
		public string Description
		{
			get
			{
				return LocalizationProvider.GetLocalizedString("MarkdownClassic_Description", false, "MarkdownPadStrings");
			}
		}
		public bool RenderWhileScrollingAllowed
		{
			get
			{
				return true;
			}
		}
		public MarkdownClassicProcessor()
		{
			this.RenderDelay = System.TimeSpan.FromSeconds(0.1);
			this._markdownProcessor = new Markdown();
			this.ApplySettings();
		}
		public string ConvertMarkdownToHTML(string markdown)
		{
			string result = string.Empty;
			try
			{
				result = this._markdownProcessor.Transform(markdown);
			}
			catch (System.Collections.Generic.KeyNotFoundException exception)
			{
				MarkdownClassicProcessor._logger.WarnException("Known error: MDS key not found, seems to occur when scrolling quickly", exception);
			}
			catch (System.InvalidOperationException exception2)
			{
				MarkdownClassicProcessor._logger.WarnException("Known error: MDS invalid operation, seems to occur when scrolling quickly", exception2);
			}
			return result;
		}
		public void ApplySettings()
		{
			this._markdownProcessor.AutoNewLines = this._settings.Markdown_Standard_AutoNewlines;
			this._markdownProcessor.AutoHyperlink = this._settings.Markdown_Standard_AutoHyperlink;
			this._markdownProcessor.LinkEmails = this._settings.Markdown_Standard_LinkEmails;
			this._markdownProcessor.EncodeProblemUrlCharacters = this._settings.Markdown_Standard_EncodeProblemUrlCharacters;
			this._markdownProcessor.EmptyElementSuffix = " />";
		}
		public override bool Equals(object obj)
		{
			return obj.GetType() == base.GetType();
		}
	}
}
