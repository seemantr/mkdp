using MarkdownPad2.Licensing;
using MarkdownPad2.Properties;
using MarkdownPad2.Utilities;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
namespace MarkdownPad2.Core
{
	public class AboutWindow : Window, System.Windows.Markup.IComponentConnector, IStyleConnector
	{
		internal StackPanel StackPanel_Title;
		internal TextBlock TextBlock_VersionNumber;
		internal TextBlock TextBlock_LicensedTo;
		internal Button Button_Ok;
		private bool _contentLoaded;
		public string PathToOpen
		{
			get;
			private set;
		}
		public AboutWindow()
		{
			this.InitializeComponent();
		}
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			string absoluteUri = e.Uri.AbsoluteUri;
			absoluteUri.TryStartDefaultProcess();
			e.Handled = true;
		}
		private void AboutWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			Settings @default = Settings.Default;
			this.TextBlock_VersionNumber.Text = AssemblyUtilities.Version;
			if (!string.IsNullOrEmpty(@default.App_LicenseEmail) && !string.IsNullOrEmpty(@default.App_LicenseKey))
			{
				LicenseEngine licenseEngine = new LicenseEngine();
				licenseEngine.VerifyLicense(@default.App_LicenseKey, @default.App_LicenseEmail);
				this.TextBlock_LicensedTo.Text = licenseEngine.LicensedToMessage;
			}
		}
		private void Hyperlink_Licenses_OnClick(object sender, RoutedEventArgs e)
		{
			string assemblyPath = AssemblyUtilities.AssemblyPath;
			this.PathToOpen = System.IO.Path.GetDirectoryName(assemblyPath) + "\\Docs\\licenses.md";
			base.Close();
		}
		private void Button_Ok_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
		private void Hyperlink_Eula_OnClick(object sender, RoutedEventArgs e)
		{
			string assemblyPath = AssemblyUtilities.AssemblyPath;
			this.PathToOpen = System.IO.Path.GetDirectoryName(assemblyPath) + "\\Docs\\EULA.md";
			base.Close();
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/ui/aboutwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((AboutWindow)target).Loaded += new RoutedEventHandler(this.AboutWindow_OnLoaded);
				return;
			case 3:
				this.StackPanel_Title = (StackPanel)target;
				return;
			case 4:
				this.TextBlock_VersionNumber = (TextBlock)target;
				return;
			case 5:
				this.TextBlock_LicensedTo = (TextBlock)target;
				return;
			case 6:
				((Hyperlink)target).Click += new RoutedEventHandler(this.Hyperlink_Licenses_OnClick);
				return;
			case 7:
				((Hyperlink)target).Click += new RoutedEventHandler(this.Hyperlink_Eula_OnClick);
				return;
			case 8:
				this.Button_Ok = (Button)target;
				this.Button_Ok.Click += new RoutedEventHandler(this.Button_Ok_Click);
				return;
			}
			this._contentLoaded = true;
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId != 2)
			{
				return;
			}
			EventSetter eventSetter = new EventSetter();
			eventSetter.Event = Hyperlink.RequestNavigateEvent;
			eventSetter.Handler = new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
			((Style)target).Setters.Add(eventSetter);
		}
	}
}
