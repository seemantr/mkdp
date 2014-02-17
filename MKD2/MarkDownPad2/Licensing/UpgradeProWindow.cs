using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.Utilities;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using WPFCustomMessageBox;
namespace MarkdownPad2.Licensing
{
	public class UpgradeProWindow : Window, System.Windows.Markup.IComponentConnector
	{
		internal Button Button_Buy;
		internal Button Button_EnterKey;
		internal Button Button_Free;
		private bool _contentLoaded;
		public UpgradeProWindow()
		{
			this.InitializeComponent();
		}
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			string absoluteUri = e.Uri.AbsoluteUri;
			absoluteUri.TryStartDefaultProcess();
			e.Handled = true;
		}
		private void Button_EnterKey_Click(object sender, RoutedEventArgs e)
		{
			LicenseWindow licenseWindow = new LicenseWindow
			{
				Owner = this
			};
			bool flag = licenseWindow.ShowLicenseDialog();
			if (flag)
			{
				base.Close();
			}
		}
		private void Button_Free_Click(object sender, RoutedEventArgs e)
		{
			CustomMessageBox.ShowOK(LocalizationProvider.GetLocalizedString("Pro_UnlockLaterMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Pro_UnlockLaterTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("OK", false, "MarkdownPadStrings"), MessageBoxImage.Asterisk);
			base.Close();
		}
		private void Button_Buy_Click(object sender, RoutedEventArgs e)
		{
			Urls.BuyProVersion.TryStartDefaultProcess();
		}
		private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			base.DragMove();
		}
		private void UnlockProWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			this.Button_Buy.Focus();
		}
		private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
		{
			Urls.MarkdownPad_VersionCompare.TryStartDefaultProcess();
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/licensing/upgradeprowindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((UpgradeProWindow)target).Loaded += new RoutedEventHandler(this.UnlockProWindow_OnLoaded);
				return;
			case 2:
				((Border)target).MouseLeftButtonDown += new MouseButtonEventHandler(this.UIElement_OnMouseLeftButtonDown);
				return;
			case 3:
				((Hyperlink)target).Click += new RoutedEventHandler(this.Hyperlink_OnClick);
				return;
			case 4:
				this.Button_Buy = (Button)target;
				this.Button_Buy.Click += new RoutedEventHandler(this.Button_Buy_Click);
				return;
			case 5:
				this.Button_EnterKey = (Button)target;
				this.Button_EnterKey.Click += new RoutedEventHandler(this.Button_EnterKey_Click);
				return;
			case 6:
				this.Button_Free = (Button)target;
				this.Button_Free.Click += new RoutedEventHandler(this.Button_Free_Click);
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}
	}
}
