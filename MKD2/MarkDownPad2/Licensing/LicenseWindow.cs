using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
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
using WPFCustomMessageBox;
namespace MarkdownPad2.Licensing
{
	public class LicenseWindow : Window, System.Windows.Markup.IComponentConnector
	{
		private Settings _settings = Settings.Default;
		internal TextBox TextBox_Email;
		internal TextBox TextBox_LicenseKey;
		internal Button Button_OK;
		internal Button Button_Cancel;
		internal Button Button_Clear;
		private bool _contentLoaded;
		public string LicenseEmail
		{
			get
			{
				return this.TextBox_Email.Text;
			}
			set
			{
				this.TextBox_Email.Text = value;
			}
		}
		public string LicenseKey
		{
			get
			{
				return this.TextBox_LicenseKey.Text;
			}
			set
			{
				this.TextBox_LicenseKey.Text = value;
			}
		}
		private bool IsLicensed
		{
			get;
			set;
		}
		public LicenseWindow()
		{
			this.InitializeComponent();
		}
		private void LicenseWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			this.LicenseEmail = this._settings.App_LicenseEmail;
			this.LicenseKey = this._settings.App_LicenseKey;
			this.TextBox_Email.Focus();
		}
		private void Button_OK_OnClick(object sender, RoutedEventArgs e)
		{
			this.VerifyLicense();
		}
		private void VerifyLicense()
		{
			if (string.IsNullOrEmpty(this.LicenseKey) || string.IsNullOrEmpty(this.LicenseEmail))
			{
				return;
			}
			LicenseEngine licenseEngine = new LicenseEngine();
			bool flag = licenseEngine.VerifyLicense(this.LicenseKey, this.LicenseEmail);
			if (!licenseEngine.LicenseProcessed)
			{
				return;
			}
			if (!flag)
			{
				CustomMessageBox.ShowOK(LocalizationProvider.GetLocalizedString("License_FailedMessage", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("License_PleaseVerify", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("License_FailedTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("OK", false, "MarkdownPadStrings"), MessageBoxImage.Exclamation);
				return;
			}
			CustomMessageBox.ShowOK(LocalizationProvider.GetLocalizedString("License_SuccessMessage", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + licenseEngine.LicensedToMessage, LocalizationProvider.GetLocalizedString("License_SuccessTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("OK", false, "MarkdownPadStrings"), MessageBoxImage.Asterisk);
			this._settings.App_LicenseEmail = this.LicenseEmail;
			this._settings.App_LicenseKey = this.LicenseKey;
			this.IsLicensed = true;
			base.Close();
		}
		private void Button_Cancel_OnClick(object sender, RoutedEventArgs e)
		{
			this.LicenseEmail = string.Empty;
			this.LicenseKey = string.Empty;
			base.Close();
		}
		public bool ShowLicenseDialog()
		{
			base.ShowDialog();
			return this.IsLicensed;
		}
		private void TextBox_Email_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Return && !string.IsNullOrEmpty(this.TextBox_Email.Text))
			{
				this.TextBox_LicenseKey.Focus();
			}
		}
		private void TextBox_LicenseKey_OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Return && string.IsNullOrEmpty(this.TextBox_Email.Text))
			{
				e.Handled = true;
				this.TextBox_Email.Focus();
				return;
			}
			if (e.Key == System.Windows.Input.Key.Return)
			{
				e.Handled = true;
				this.VerifyLicense();
			}
		}
		private void CommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = false;
			e.Handled = true;
		}
		private void Hyperlink_Buy_OnClick(object sender, RoutedEventArgs e)
		{
			Urls.BuyProVersion.TryStartDefaultProcess();
		}
		private void Hyperlink_Paste_OnClick(object sender, RoutedEventArgs e)
		{
			if (Clipboard.ContainsText())
			{
				this.TextBox_LicenseKey.Text = Clipboard.GetText();
			}
		}
		private void Button_Clear_OnClick(object sender, RoutedEventArgs e)
		{
			this.LicenseEmail = (this.LicenseKey = (this._settings.App_LicenseEmail = (this._settings.App_LicenseKey = string.Empty)));
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
			Uri resourceLocator = new Uri("/MarkdownPad2;component/licensing/licensewindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((LicenseWindow)target).Loaded += new RoutedEventHandler(this.LicenseWindow_OnLoaded);
				return;
			case 2:
				((Hyperlink)target).Click += new RoutedEventHandler(this.Hyperlink_Buy_OnClick);
				return;
			case 3:
				this.TextBox_Email = (TextBox)target;
				this.TextBox_Email.KeyDown += new KeyEventHandler(this.TextBox_Email_OnKeyDown);
				return;
			case 4:
				((Hyperlink)target).Click += new RoutedEventHandler(this.Hyperlink_Paste_OnClick);
				return;
			case 5:
				this.TextBox_LicenseKey = (TextBox)target;
				this.TextBox_LicenseKey.PreviewKeyDown += new KeyEventHandler(this.TextBox_LicenseKey_OnPreviewKeyDown);
				return;
			case 6:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanExecute);
				return;
			case 7:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CommandBinding_OnCanExecute);
				return;
			case 8:
				this.Button_OK = (Button)target;
				this.Button_OK.Click += new RoutedEventHandler(this.Button_OK_OnClick);
				return;
			case 9:
				this.Button_Cancel = (Button)target;
				this.Button_Cancel.Click += new RoutedEventHandler(this.Button_Cancel_OnClick);
				return;
			case 10:
				this.Button_Clear = (Button)target;
				this.Button_Clear.Click += new RoutedEventHandler(this.Button_Clear_OnClick);
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}
	}
}
