using MarkdownDeep;
using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using NLog;
using System;
namespace MarkdownPad2.Markdown
{
	public class MarkdownExtraProcessor : IMarkdownProcessor
	{
		private Settings _settings = Settings.Default;
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly Markdown MarkdownDeep;
		public string ProcessorName
		{
			get
			{
				return LocalizationProvider.GetLocalizedString("MarkdownExtra", false, "MarkdownPadStrings");
			}
		}
		public string Description
		{
			get
			{
				return LocalizationProvider.GetLocalizedString("MarkdownExtra_Description", false, "MarkdownPadStrings");
			}
		}
		public bool RenderWhileScrollingAllowed
		{
			get
			{
				return true;
			}
		}
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
		public MarkdownExtraProcessor()
		{
			this.RenderDelay = System.TimeSpan.FromSeconds(0.1);
			this.MarkdownDeep = new Markdown();
			this.ApplySettings();
		}
		public string ConvertMarkdownToHTML(string markdown)
		{
			string result = string.Empty;
			try
			{
				result = this.MarkdownDeep.Transform(markdown);
			}
			catch (System.ArgumentOutOfRangeException exception)
			{
				MarkdownExtraProcessor._logger.WarnException("MDD known argument range error (empty footnote source?), surpressing", exception);
			}
			catch (System.ArgumentException exception2)
			{
				MarkdownExtraProcessor._logger.WarnException("MDD known argument error, surpressing", exception2);
			}
			catch (System.InvalidCastException argument)
			{
				MarkdownExtraProcessor._logger.Warn<System.InvalidCastException>("MDD known invalid cast error, surpressing", argument);
			}
			catch (System.InvalidOperationException argument2)
			{
				MarkdownExtraProcessor._logger.Warn<System.InvalidOperationException>("MDD known invalidoperationexception, surpressing (scrolling)", argument2);
			}
			catch (System.NullReferenceException argument3)
			{
				MarkdownExtraProcessor._logger.Warn<System.NullReferenceException>("MDD known nullref, surpressing (scrolling)", argument3);
			}
			catch (System.IndexOutOfRangeException argument4)
			{
				MarkdownExtraProcessor._logger.Warn<System.IndexOutOfRangeException>("MDD known index out of range, surpressing (scrolling)", argument4);
			}
			return result;
		}
		public void ApplySettings()
		{
			this.MarkdownDeep.ExtraMode = true;
			this.MarkdownDeep.SafeMode = false;
		}
		public override bool Equals(object obj)
		{
			return obj.GetType() == base.GetType();
		}
	}
}
