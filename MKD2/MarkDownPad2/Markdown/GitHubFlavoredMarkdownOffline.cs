using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using MarkdownPad2.Utilities;
using Microsoft.ClearScript.Windows;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Runtime.CompilerServices;
namespace MarkdownPad2.Markdown
{
	public class GitHubFlavoredMarkdownOffline : IMarkdownProcessor
	{
		[System.Runtime.CompilerServices.CompilerGenerated]
		private static class <ConvertMarkdownToHTML>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, string>> <>p__Site1;
			public static CallSite<Func<CallSite, object, string, object>> <>p__Site2;
		}
		private Settings _settings = Settings.Default;
		public string ProcessorName
		{
			get
			{
				return LocalizationProvider.GetLocalizedString("OfflineGFM_Name", false, "MarkdownPadStrings");
			}
		}
		public string Description
		{
			get
			{
				return LocalizationProvider.GetLocalizedString("OffineGFM_Description", false, "MarkdownPadStrings");
			}
		}
		public bool BackgroundRenderingAllowed
		{
			get
			{
				return false;
			}
		}
		public System.TimeSpan RenderDelay
		{
			get;
			private set;
		}
		private JScriptEngine Engine
		{
			get;
			set;
		}
		public bool RenderWhileScrollingAllowed
		{
			get
			{
				return true;
			}
		}
		public GitHubFlavoredMarkdownOffline()
		{
			this.Engine = new JScriptEngine();
			this.ApplySettings();
		}
		public string ConvertMarkdownToHTML(string markdown)
		{
			if (GitHubFlavoredMarkdownOffline.<ConvertMarkdownToHTML>o__SiteContainer0.<>p__Site1 == null)
			{
				GitHubFlavoredMarkdownOffline.<ConvertMarkdownToHTML>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(GitHubFlavoredMarkdownOffline)));
			}
			Func<CallSite, object, string> arg_9D_0 = GitHubFlavoredMarkdownOffline.<ConvertMarkdownToHTML>o__SiteContainer0.<>p__Site1.Target;
			CallSite arg_9D_1 = GitHubFlavoredMarkdownOffline.<ConvertMarkdownToHTML>o__SiteContainer0.<>p__Site1;
			if (GitHubFlavoredMarkdownOffline.<ConvertMarkdownToHTML>o__SiteContainer0.<>p__Site2 == null)
			{
				GitHubFlavoredMarkdownOffline.<ConvertMarkdownToHTML>o__SiteContainer0.<>p__Site2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "marked", null, typeof(GitHubFlavoredMarkdownOffline), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			return arg_9D_0(arg_9D_1, GitHubFlavoredMarkdownOffline.<ConvertMarkdownToHTML>o__SiteContainer0.<>p__Site2.Target(GitHubFlavoredMarkdownOffline.<ConvertMarkdownToHTML>o__SiteContainer0.<>p__Site2, this.Engine.Script, markdown));
		}
		public void ApplySettings()
		{
			this.RenderDelay = System.TimeSpan.FromSeconds(0.1);
			this.Engine.Execute(Resources.marked);
			string str = string.Concat(new string[]
			{
				"{gfm: true, tables: true, breaks: ",
				this._settings.Markdown_OfflineGFM_AutoLineBreaks.ToLower(),
				", sanitize: false, smartLists: ",
				this._settings.Markdown_OfflineGFM_SmartLists.ToLower(),
				", smartypants: ",
				this._settings.Markdown_OfflineGFM_SmartyPants.ToLower(),
				"}"
			});
			this.Engine.Execute("marked.setOptions(" + str + ")");
		}
		public override bool Equals(object obj)
		{
			return obj.GetType() == base.GetType();
		}
	}
}
