using MarkdownPad2.Core;
using MarkdownPad2.Properties;
using MarkdownPad2.UI;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
namespace MarkdownPad2
{
	public class App : Application, ISingleInstanceApp, System.Windows.Markup.IComponentConnector
	{
		private const string Unique = "Not all of us can do great things. But we can do small things with great love.";
		private static readonly Settings _settings = Settings.Default;
		private bool _contentLoaded;
		[System.STAThread]
		public static void Main()
		{
			if (App._settings.App_UpgradeRequired)
			{
				App._settings.Upgrade();
				App._settings.App_UpgradeRequired = false;
			}
			string directoryName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			if (!string.IsNullOrEmpty(directoryName) && System.IO.Directory.GetCurrentDirectory() != directoryName)
			{
				System.IO.Directory.SetCurrentDirectory(directoryName);
			}
			if (SingleInstance<App>.InitializeAsFirstInstance("Not all of us can do great things. But we can do small things with great love."))
			{
				App app = new App();
				app.InitializeComponent();
				app.Run();
				SingleInstance<App>.Cleanup();
			}
		}
		public bool SignalExternalCommandLineArgs(System.Collections.Generic.IList<string> args)
		{
			return ((MainWindow)base.MainWindow).ProcessCommandLineArgs(args);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			base.StartupUri = new Uri("UI/MainWindow.xaml", UriKind.Relative);
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/app.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			this._contentLoaded = true;
		}
	}
}
