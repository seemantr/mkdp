using System;
namespace MarkdownPad2
{
	public interface IMarkdownProcessor
	{
		string ProcessorName
		{
			get;
		}
		string Description
		{
			get;
		}
		bool RenderWhileScrollingAllowed
		{
			get;
		}
		bool BackgroundRenderingAllowed
		{
			get;
		}
		System.TimeSpan RenderDelay
		{
			get;
		}
		string ConvertMarkdownToHTML(string markdown);
		void ApplySettings();
	}
}
