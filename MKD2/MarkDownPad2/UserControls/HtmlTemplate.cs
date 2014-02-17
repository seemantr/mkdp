using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.Markdown;
using MarkdownPad2.RendererContent;
using NLog;
using System;
using System.IO;
using System.Text;
namespace MarkdownPad2.UserControls
{
	public class HtmlTemplate
	{
		private Logger _logger = LogManager.GetCurrentClassLogger();
		public MarkdownProcessorType MarkdownProcessor
		{
			get;
			set;
		}
		public string CustomHeadContent
		{
			get;
			set;
		}
		public string Filename
		{
			get;
			set;
		}
		public string BodyContent
		{
			get;
			set;
		}
		public string CssFilePath
		{
			get;
			set;
		}
		public bool UseBaseRelativeImageWorkaround
		{
			get;
			set;
		}
		public string GenerateHtmlTemplate(bool isLivePreview)
		{
			string value = string.Empty;
			if (System.IO.File.Exists(this.CssFilePath))
			{
				this._logger.Trace("Loading CSS file: " + this.CssFilePath);
				try
				{
					value = System.IO.File.ReadAllText(this.CssFilePath);
				}
				catch (System.Exception exception)
				{
					MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_OpeningCssFileMessage", true, "MarkdownPadStrings") + this.CssFilePath, LocalizationProvider.GetLocalizedString("Error_OpeningCssFileTitle", false, "MarkdownPadStrings"), exception, "");
				}
			}
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			string str = string.IsNullOrEmpty(this.Filename) ? "MarkdownPad Document" : System.IO.Path.GetFileNameWithoutExtension(this.Filename);
			if (!isLivePreview)
			{
				stringBuilder.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
			}
			stringBuilder.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">");
			stringBuilder.AppendLine("<head>");
			stringBuilder.AppendLine("<title>" + str + "</title>");
			stringBuilder.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
			stringBuilder.AppendLine("<style type=\"text/css\">");
			stringBuilder.AppendLine(value);
			stringBuilder.AppendLine("</style>");
			if (this.MarkdownProcessor == MarkdownProcessorType.GithubFlavoredMarkdown)
			{
				stringBuilder.AppendLine("<style type=\"text/css\">");
				stringBuilder.AppendLine(WebResources.PygmentsCss);
				stringBuilder.AppendLine("</style>");
			}
			if (isLivePreview)
			{
				stringBuilder.AppendLine("<script type='text/javascript'>function loadMarkdown(input) { document.body.innerHTML = input; var links = document.getElementsByTagName('a');var len = links.length;for(var i=0; i<len; i++){links[i].target = '_blank';} } </script>");
			}
			if (this.UseBaseRelativeImageWorkaround && !string.IsNullOrEmpty(this.Filename))
			{
				try
				{
					stringBuilder.AppendLine("<base href='file:\\\\\\" + System.IO.Path.GetDirectoryName(this.Filename) + "\\'/>");
				}
				catch (System.Exception exception2)
				{
					this._logger.ErrorException("Error setting relative base URL in HTML template", exception2);
				}
			}
			if (!string.IsNullOrEmpty(this.CustomHeadContent))
			{
				stringBuilder.AppendLine(this.CustomHeadContent);
			}
			stringBuilder.AppendLine("</head>");
			stringBuilder.AppendLine("<body>");
			stringBuilder.AppendLine(this.BodyContent);
			stringBuilder.AppendLine("</body>");
			stringBuilder.AppendLine("</html>");
			stringBuilder.AppendLine("<!-- This document was created with MarkdownPad, the Markdown editor for Windows (http://markdownpad.com) -->");
			return stringBuilder.ToString();
		}
	}
}
