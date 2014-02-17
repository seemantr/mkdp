using Awesomium.Core;
using Awesomium.Windows.Controls;
using ICSharpCode.AvalonEdit;
using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.Licensing;
using MarkdownPad2.Markdown;
using MarkdownPad2.Network;
using MarkdownPad2.Options.OptionsPages;
using MarkdownPad2.Properties;
using MarkdownPad2.Settings;
using MarkdownPad2.Stylesheets;
using MarkdownPad2.UI;
using MarkdownPad2.Updater;
using MarkdownPad2.Utilities;
using Microsoft.Win32;
using NLog;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using WPFCustomMessageBox;
using WPFLocalizeExtension.Engine;
using Xceed.Wpf.Toolkit;
namespace MarkdownPad2.Options
{
	public class OptionsWindow : Window, System.Windows.Markup.IComponentConnector, IStyleConnector
	{
		private Settings _settings = Settings.Default;
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		private System.Collections.ObjectModel.ObservableCollection<System.IO.FileInfo> cssFileList = new System.Collections.ObjectModel.ObservableCollection<System.IO.FileInfo>();
		internal TabControl TabControl_Main;
		internal ColorPicker ColorPicker_BackgroundColor;
		internal ColorPicker ColorPicker_ForegroundColor;
		internal ComboBox ComboBox_FontWeight;
		internal ComboBox ComboBox_FontSize;
		internal ComboBox ComboBox_FontFamily;
		internal TextEditor TextEditor_PreviewText;
		internal MenuItem MenuItem_NewPangram;
		internal CheckBox CheckBox_InsertSpacesInsteadOfTab;
		internal CheckBox CheckBox_AutoContinueLists;
		internal CheckBox CheckBox_DisplayFormattingMarks;
		internal CheckBox CheckBox_EnableClickableLinksInEditor;
		internal ColorPicker ColorPicker_HyperlinkForegroundColor;
		internal Button Button_TimestampInfo;
		internal TextBox TextBox_TimestampFormat;
		internal Grid Grid_Languages;
		internal ComboBox ComboBox_UserInterfaceLanguages;
		internal Hyperlink Hyperlink_HelpTranslate;
		internal ComboBox ComboBox_MarkdownProcessors;
		internal Button Button_RenderingModeTooltip;
		internal ComboBox ComboBox_RenderingMode;
		internal CheckBox CheckBox_MarkdownClassic_AutomaticNewlines;
		internal CheckBox CheckBox_MarkdownClassic_AutomaticHyperlinks;
		internal CheckBox CheckBox_MarkdownClassic_AutomaticEmailHyperlinks;
		internal CheckBox CheckBox_MarkdownClassic_EncodeProblematicUrls;
		internal CheckBox CheckBox_GFM_AnonymousMode;
		internal TextBox TextBox_GithubUsername;
		internal PasswordBox PasswordBox_GithubPassword;
		internal CheckBox CheckBox_OfflineGFM_AutomaticNewlines;
		internal CheckBox CheckBox_OfflineGFM_SmartLists;
		internal CheckBox CheckBox_OfflineGFM_SmartyPants;
		internal ComboBox ComboBox_UnorderedListSyntax;
		internal CheckBox CheckBox_UseUnderlineStyleHeadings;
		internal ListBox listBox_CSS;
		internal Button Button_AddCSS;
		internal Button Button_ModifyCSS;
		internal Button Button_RemoveCSS;
		internal Button Button_RestoreDefaultStylesheets;
		internal WebControl Browser_CSSPreview;
		internal CheckBox CheckBox_EnableAutoSave;
		internal IntegerUpDown IntegerUpDown_AutoSaveFrequency;
		internal CheckBox CheckBox_MonitorFileChanges;
		internal CheckBox CheckBox_AutomaticReloadOnExternalChanges;
		internal CheckBox CheckBox_ScrollToEndOfDocumentOnLoad;
		internal CheckBox CheckBox_OpenHtmlFileAfterExport;
		internal CheckBox CheckBox_OpenPdfFileAfterExport;
		internal CheckBox CheckBox_PdfIncludeBackground;
		internal CheckBox CheckBox_PdfEnableOutlineGeneration;
		internal CheckBox CheckBox_PdfLandscapeMode;
		internal IntegerUpDown IntUpDown_Pdf_MarginLeft;
		internal IntegerUpDown IntUpDown_Pdf_MarginTop;
		internal IntegerUpDown IntUpDown_Pdf_MarginRight;
		internal IntegerUpDown IntUpDown_Pdf_MarginBottom;
		internal ComboBox ComboBox_PaperSize;
		internal CheckBox CheckBox_ExportHtmlWithBom;
		internal CheckBox CheckBox_ExportTextWithBom;
		internal CheckBox CheckBox_UseUnixEol;
		internal CheckBox CheckBox_VerifyInternetConnection;
		internal TextBox TextBox_MarkdownReferenceFile;
		internal Button Button_MarkdownReferenceFile_Browse;
		internal Button Button_MarkdownReferenceFile_Clear;
		internal ComboBox ComboBox_ProxySettings;
		internal TextBox TextBox_ManualProxy;
		internal TextBox TextBox_ManualProxyPort;
		internal CheckBox CheckBox_DisplaySplashScreen;
		internal CheckBox CheckBox_SubmitAnonymousStats;
		internal ComboBox ComboBox_UpdateFrequency;
		internal SyntaxHighlightingOptions OptionsPage_Highlighting;
		internal Button Button_Save;
		internal Button Button_Cancel;
		private bool _contentLoaded;
		public bool SettingsSaved
		{
			get;
			private set;
		}
		public OptionsWindow()
		{
			this.InitializeComponent();
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.LoadSettings();
			System.Windows.DependencyProperty showDurationProperty = ToolTipService.ShowDurationProperty;
			System.Windows.PropertyMetadata metadata = showDurationProperty.GetMetadata(typeof(System.Windows.DependencyObject));
			int num = int.Parse(metadata.DefaultValue.ToString());
			if (num < 24000)
			{
				showDurationProperty.OverrideMetadata(typeof(System.Windows.DependencyObject), new FrameworkPropertyMetadata(24000));
			}
			if (this.listBox_CSS.SelectedItem != null)
			{
				System.IO.FileInfo fileInfo = (System.IO.FileInfo)this.listBox_CSS.SelectedItem;
				this.UpdateCSSPreview(fileInfo.FullName);
			}
		}
		private void Window_Closing(object sender, CancelEventArgs e)
		{
		}
		private void Button_Save_Click(object sender, RoutedEventArgs e)
		{
			if (!this.CheckBox_GFM_AnonymousMode.IsChecked.Value)
			{
				object selectedItem = this.ComboBox_MarkdownProcessors.SelectedItem;
				if (selectedItem is GitHubFlavoredMarkdownProcessor && (string.IsNullOrEmpty(this.TextBox_GithubUsername.Text) || this.PasswordBox_GithubPassword.SecurePassword.Length == 0))
				{
					CustomMessageBox.Show(LocalizationProvider.GetLocalizedString("GithubCredentialsProblemMessage1", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("GithubCredentialsProblemMessage2", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("GithubCredentialsProblemTitle", false, "MarkdownPadStrings"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
			}
			this.SetSettings();
			this.SettingsSaved = true;
			base.Close();
		}
		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.SettingsSaved = false;
			base.Close();
		}
		private void SetSettings()
		{
			this._settings.Editor_BackgroundColor = this.ColorPicker_BackgroundColor.SelectedColor;
			this._settings.Editor_ForegroundColor = this.ColorPicker_ForegroundColor.SelectedColor;
			if (this.ComboBox_FontFamily.SelectedItem != null)
			{
				this._settings.Editor_FontFamily = (FontFamily)this.ComboBox_FontFamily.SelectedItem;
			}
			if (this.ComboBox_FontSize.SelectedItem != null)
			{
				this._settings.Editor_FontSize = (double)this.ComboBox_FontSize.SelectedItem;
			}
			if (this.ComboBox_FontWeight.SelectedItem != null)
			{
				this._settings.Editor_FontWeight = (FontWeight)this.ComboBox_FontWeight.SelectedItem;
			}
			this._settings.Editor_UseSpacesAsTabs = this.CheckBox_InsertSpacesInsteadOfTab.IsChecked.Value;
			this._settings.Editor_AutoContinueLists = this.CheckBox_AutoContinueLists.IsChecked.Value;
			this._settings.Editor_EnableHyperlinks = this.CheckBox_EnableClickableLinksInEditor.IsChecked.Value;
			this._settings.Editor_HyperlinkForegroundColor = this.ColorPicker_HyperlinkForegroundColor.SelectedColor;
			this._settings.Editor_TimestampFormat = this.TextBox_TimestampFormat.Text;
			this._settings.Editor_DisplayFormattingMarks = this.CheckBox_DisplayFormattingMarks.IsChecked.Value;
			if (this.listBox_CSS.SelectedItem != null)
			{
				System.IO.FileInfo fileInfo = (System.IO.FileInfo)this.listBox_CSS.SelectedItem;
				this._settings.CodeEditor_SelectedCssFileName = fileInfo.Name;
			}
			if (this.ComboBox_MarkdownProcessors.SelectedItem != null)
			{
				IMarkdownProcessor processor = this.ComboBox_MarkdownProcessors.SelectedItem as IMarkdownProcessor;
				MarkdownProcessorType markdown_MarkdownProcessor = MarkdownProcessorProvider.ReverseLookupBad(processor);
				this._settings.Markdown_MarkdownProcessor = markdown_MarkdownProcessor;
			}
			this._settings.Markdown_EnableHighPerformanceRendering = System.Convert.ToBoolean(this.ComboBox_RenderingMode.SelectedIndex);
			this._settings.Markdown_Standard_AutoNewlines = this.CheckBox_MarkdownClassic_AutomaticNewlines.IsChecked.Value;
			this._settings.Markdown_Standard_AutoHyperlink = this.CheckBox_MarkdownClassic_AutomaticHyperlinks.IsChecked.Value;
			this._settings.Markdown_Standard_LinkEmails = this.CheckBox_MarkdownClassic_AutomaticEmailHyperlinks.IsChecked.Value;
			this._settings.Markdown_Standard_EncodeProblemUrlCharacters = this.CheckBox_MarkdownClassic_EncodeProblematicUrls.IsChecked.Value;
			this._settings.Markdown_GFM_AnonymousMode = this.CheckBox_GFM_AnonymousMode.IsChecked.Value;
			this._settings.Markdown_GFM_Username = this.TextBox_GithubUsername.Text;
			this._settings.Markdown_GFM_Password = SettingsProvider.EncryptString(this.PasswordBox_GithubPassword.SecurePassword);
			this._settings.Markdown_OfflineGFM_AutoLineBreaks = this.CheckBox_OfflineGFM_AutomaticNewlines.IsChecked.Value;
			this._settings.Markdown_OfflineGFM_SmartLists = this.CheckBox_OfflineGFM_SmartLists.IsChecked.Value;
			this._settings.Markdown_OfflineGFM_SmartyPants = this.CheckBox_OfflineGFM_SmartyPants.IsChecked.Value;
			this._settings.Markdown_UnorderedListStyle = (UnorderedListStyle)this.ComboBox_UnorderedListSyntax.SelectedValue;
			this._settings.Markdown_UseUnderlineStyleHeadings = this.CheckBox_UseUnderlineStyleHeadings.IsChecked.Value;
			if (this.ComboBox_UserInterfaceLanguages.SelectedItem != null)
			{
				this._settings.App_Locale = (this.ComboBox_UserInterfaceLanguages.SelectedItem as System.Globalization.CultureInfo);
			}
			this._settings.IO_AutoSaveEnabled = this.CheckBox_EnableAutoSave.IsChecked.Value;
			if (this.IntegerUpDown_AutoSaveFrequency.Value.HasValue)
			{
				this._settings.IO_AutoSaveInterval = this.IntegerUpDown_AutoSaveFrequency.Value.Value;
			}
			this._settings.IO_MonitorFileChangesOnDisk = this.CheckBox_MonitorFileChanges.IsChecked.Value;
			this._settings.IO_AutomaticReloadOnExternalChanges = this.CheckBox_AutomaticReloadOnExternalChanges.IsChecked.Value;
			this._settings.IO_ScrollToEndOfDocumentOnLoad = this.CheckBox_ScrollToEndOfDocumentOnLoad.IsChecked.Value;
			this._settings.IO_OpenHtmlFileAfterExport = this.CheckBox_OpenHtmlFileAfterExport.IsChecked.Value;
			this._settings.IO_OpenPdfFileAfterExport = this.CheckBox_OpenPdfFileAfterExport.IsChecked.Value;
			this._settings.IO_Pdf_IncludeBackground = this.CheckBox_PdfIncludeBackground.IsChecked.Value;
			this._settings.IO_Pdf_EnableOutlineGeneration = this.CheckBox_PdfEnableOutlineGeneration.IsChecked.Value;
			this._settings.IO_Pdf_LandscapeMode = this.CheckBox_PdfLandscapeMode.IsChecked.Value;
			this._settings.IO_Pdf_MarginLeftInMillimeters = this.IntUpDown_Pdf_MarginLeft.Value.Value;
			this._settings.IO_Pdf_MarginTopInMillimeters = this.IntUpDown_Pdf_MarginTop.Value.Value;
			this._settings.IO_Pdf_MarginRightInMillimeters = this.IntUpDown_Pdf_MarginRight.Value.Value;
			this._settings.IO_Pdf_MarginBottomInMillimeters = this.IntUpDown_Pdf_MarginBottom.Value.Value;
			this._settings.IO_Pdf_PaperSize = (PaperKind)this.ComboBox_PaperSize.SelectedItem;
			this._settings.IO_UseUnixStyleEol = this.CheckBox_UseUnixEol.IsChecked.Value;
			this._settings.IO_ExportHtmlWithBom = this.CheckBox_ExportHtmlWithBom.IsChecked.Value;
			this._settings.IO_ExportTextWithBom = this.CheckBox_ExportTextWithBom.IsChecked.Value;
			this._settings.App_VerifyNetworkConnection = this.CheckBox_VerifyInternetConnection.IsChecked.Value;
			this._settings.App_DisplaySplashScreen = this.CheckBox_DisplaySplashScreen.IsChecked.Value;
			this._settings.App_ProxyType = (ProxyType)this.ComboBox_ProxySettings.SelectedValue;
			this._settings.App_ManualProxy = this.TextBox_ManualProxy.Text;
			int app_ProxyPort = 80;
			int.TryParse(this.TextBox_ManualProxyPort.Text, out app_ProxyPort);
			this._settings.App_ProxyPort = app_ProxyPort;
			this._settings.App_SendAnonymousStatistics = this.CheckBox_SubmitAnonymousStats.IsChecked.Value;
			this._settings.Markdown_ReferenceFile = this.TextBox_MarkdownReferenceFile.Text;
			this.OptionsPage_Highlighting.SaveSettings();
			if (this.ComboBox_UpdateFrequency.SelectedItem != null)
			{
				this._settings.App_UpdateFrequency = (UpdateFrequency)this.ComboBox_UpdateFrequency.SelectedValue;
			}
			try
			{
				this._settings.Save();
			}
			catch (System.Exception ex)
			{
				CustomMessageBox.Show("An error occurred while saving settings:\n\n" + ex.Message + "\n\nIf this error persists, please report it under Help --> Report a Bug", "Error Saving Settings", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
		private void LoadSettings()
		{
			this.ColorPicker_BackgroundColor.SelectedColor = this._settings.Editor_BackgroundColor;
			this.ColorPicker_ForegroundColor.SelectedColor = this._settings.Editor_ForegroundColor;
			this.ComboBox_FontFamily.ItemsSource = 
				from font in Fonts.SystemFontFamilies
				orderby font.ToString()
				select font;
			if (this.ComboBox_FontFamily.Items.Contains(this._settings.Editor_FontFamily))
			{
				this.ComboBox_FontFamily.SelectedItem = this._settings.Editor_FontFamily;
			}
			else
			{
				if (this.ComboBox_FontFamily.Items.Contains(new FontFamily("Courier New")))
				{
					this.ComboBox_FontFamily.SelectedItem = new FontFamily("Courier New");
				}
			}
			this.ComboBox_FontSize.ItemsSource = EditorUtilities.FontSizes;
			if (this.ComboBox_FontSize.Items.Contains(this._settings.Editor_FontSize))
			{
				this.ComboBox_FontSize.SelectedItem = this._settings.Editor_FontSize;
			}
			if (this.ComboBox_FontWeight.Items.Contains(this._settings.Editor_FontWeight))
			{
				this.ComboBox_FontWeight.SelectedItem = this._settings.Editor_FontWeight;
			}
			this.CheckBox_EnableClickableLinksInEditor.IsChecked = new bool?(this._settings.Editor_EnableHyperlinks);
			this.CheckBox_AutoContinueLists.IsChecked = new bool?(this._settings.Editor_AutoContinueLists);
			this.ColorPicker_HyperlinkForegroundColor.SelectedColor = this._settings.Editor_HyperlinkForegroundColor;
			this.CheckBox_InsertSpacesInsteadOfTab.IsChecked = new bool?(this._settings.Editor_UseSpacesAsTabs);
			this.TextBox_TimestampFormat.Text = this._settings.Editor_TimestampFormat;
			this.CheckBox_DisplayFormattingMarks.IsChecked = new bool?(this._settings.Editor_DisplayFormattingMarks);
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(StyleSheetProvider.StylesheetDirectory + this._settings.CodeEditor_SelectedCssFileName);
			this.LoadCSSFiles(fileInfo.FullName);
			this.ComboBox_RenderingMode.SelectedIndex = System.Convert.ToInt32(this._settings.Markdown_EnableHighPerformanceRendering);
			MarkdownProcessorProvider.PopulateComboBoxWithMarkdownProcessors(this.ComboBox_MarkdownProcessors, this._settings.Markdown_MarkdownProcessor, this);
			this.CheckBox_MarkdownClassic_AutomaticNewlines.IsChecked = new bool?(this._settings.Markdown_Standard_AutoNewlines);
			this.CheckBox_MarkdownClassic_AutomaticHyperlinks.IsChecked = new bool?(this._settings.Markdown_Standard_AutoHyperlink);
			this.CheckBox_MarkdownClassic_AutomaticEmailHyperlinks.IsChecked = new bool?(this._settings.Markdown_Standard_LinkEmails);
			this.CheckBox_MarkdownClassic_EncodeProblematicUrls.IsChecked = new bool?(this._settings.Markdown_Standard_EncodeProblemUrlCharacters);
			this.CheckBox_GFM_AnonymousMode.IsChecked = new bool?(this._settings.Markdown_GFM_AnonymousMode);
			this.TextBox_GithubUsername.Text = this._settings.Markdown_GFM_Username;
			this.PasswordBox_GithubPassword.Password = SettingsProvider.DecryptString(this._settings.Markdown_GFM_Password).ToInsecureString();
			this.CheckBox_OfflineGFM_AutomaticNewlines.IsChecked = new bool?(this._settings.Markdown_OfflineGFM_AutoLineBreaks);
			this.CheckBox_OfflineGFM_SmartLists.IsChecked = new bool?(this._settings.Markdown_OfflineGFM_SmartLists);
			this.CheckBox_OfflineGFM_SmartyPants.IsChecked = new bool?(this._settings.Markdown_OfflineGFM_SmartyPants);
			this.ComboBox_UnorderedListSyntax.ItemsSource = MarkdownSyntaxProvider.UnorderedListSyntaxMap;
			this.ComboBox_UnorderedListSyntax.SelectedValue = this._settings.Markdown_UnorderedListStyle;
			this.CheckBox_UseUnderlineStyleHeadings.IsChecked = new bool?(this._settings.Markdown_UseUnderlineStyleHeadings);
			this.ComboBox_UserInterfaceLanguages.ItemsSource = LocalizationProvider.AvailableCultures;
			if (this.ComboBox_UserInterfaceLanguages.Items.Contains(this._settings.App_Locale))
			{
				this.ComboBox_UserInterfaceLanguages.SelectedItem = this._settings.App_Locale;
			}
			this.ComboBox_UserInterfaceLanguages.SelectionChanged += delegate(object sender, SelectionChangedEventArgs args)
			{
				LocalizeDictionary.Instance.Culture = (System.Globalization.CultureInfo)args.AddedItems[0];
				if (args.AddedItems[0] != args.RemovedItems[0])
				{
					this._settings.App_Locale = (args.AddedItems[0] as System.Globalization.CultureInfo);
					CustomMessageBox.ShowOK(LocalizationProvider.GetLocalizedString("Language_PleaseRestartMarkdownPad_Message", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Language_PleaseRestartMarkdownPad_Title", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("OK", false, "MarkdownPadStrings"), MessageBoxImage.Asterisk);
				}
			};
			this.CheckBox_EnableAutoSave.IsChecked = new bool?(this._settings.IO_AutoSaveEnabled);
			this.IntegerUpDown_AutoSaveFrequency.Value = new int?(this._settings.IO_AutoSaveInterval);
			this.CheckBox_MonitorFileChanges.IsChecked = new bool?(this._settings.IO_MonitorFileChangesOnDisk);
			this.CheckBox_AutomaticReloadOnExternalChanges.IsChecked = new bool?(this._settings.IO_AutomaticReloadOnExternalChanges);
			this.CheckBox_OpenHtmlFileAfterExport.IsChecked = new bool?(this._settings.IO_OpenHtmlFileAfterExport);
			this.CheckBox_ScrollToEndOfDocumentOnLoad.IsChecked = new bool?(this._settings.IO_ScrollToEndOfDocumentOnLoad);
			this.CheckBox_OpenPdfFileAfterExport.IsChecked = new bool?(this._settings.IO_OpenPdfFileAfterExport);
			this.CheckBox_PdfIncludeBackground.IsChecked = new bool?(this._settings.IO_Pdf_IncludeBackground);
			this.CheckBox_PdfEnableOutlineGeneration.IsChecked = new bool?(this._settings.IO_Pdf_EnableOutlineGeneration);
			this.CheckBox_PdfLandscapeMode.IsChecked = new bool?(this._settings.IO_Pdf_LandscapeMode);
			this.IntUpDown_Pdf_MarginLeft.Value = new int?(this._settings.IO_Pdf_MarginLeftInMillimeters);
			this.IntUpDown_Pdf_MarginTop.Value = new int?(this._settings.IO_Pdf_MarginTopInMillimeters);
			this.IntUpDown_Pdf_MarginRight.Value = new int?(this._settings.IO_Pdf_MarginRightInMillimeters);
			this.IntUpDown_Pdf_MarginBottom.Value = new int?(this._settings.IO_Pdf_MarginBottomInMillimeters);
			this.ComboBox_PaperSize.ItemsSource = System.Enum.GetValues(typeof(PaperKind)).Cast<PaperKind>();
			if (this.ComboBox_PaperSize.Items.Contains(this._settings.IO_Pdf_PaperSize))
			{
				this.ComboBox_PaperSize.SelectedItem = this._settings.IO_Pdf_PaperSize;
			}
			this.CheckBox_ExportHtmlWithBom.IsChecked = new bool?(this._settings.IO_ExportHtmlWithBom);
			this.CheckBox_ExportTextWithBom.IsChecked = new bool?(this._settings.IO_ExportTextWithBom);
			this.CheckBox_UseUnixEol.IsChecked = new bool?(this._settings.IO_UseUnixStyleEol);
			this.CheckBox_VerifyInternetConnection.IsChecked = new bool?(this._settings.App_VerifyNetworkConnection);
			this.CheckBox_DisplaySplashScreen.IsChecked = new bool?(this._settings.App_DisplaySplashScreen);
			this.ComboBox_ProxySettings.ItemsSource = ProxyProvider.ProxyTypes;
			this.ComboBox_ProxySettings.SelectedItem = this._settings.App_ProxyType;
			this.TextBox_ManualProxyPort.Text = this._settings.App_ProxyPort.ToString(System.Globalization.CultureInfo.InvariantCulture);
			this.TextBox_ManualProxy.Text = this._settings.App_ManualProxy;
			this.CheckBox_SubmitAnonymousStats.IsChecked = new bool?(this._settings.App_SendAnonymousStatistics);
			this.TextBox_MarkdownReferenceFile.Text = this._settings.Markdown_ReferenceFile;
			this.OptionsPage_Highlighting.LoadSettings();
			this.ComboBox_UpdateFrequency.ItemsSource = UpdateProvider.UpdateFrequencyMap;
			this.ComboBox_UpdateFrequency.SelectedValue = this._settings.App_UpdateFrequency;
			this.ComboBox_UpdateFrequency.SelectionChanged += delegate(object sender, SelectionChangedEventArgs args)
			{
				if ((UpdateFrequency)this.ComboBox_UpdateFrequency.SelectedValue != UpdateFrequency.Always)
				{
					string localizedString = LocalizationProvider.GetLocalizedString("Pro_ConfigureUpdateFrequency", false, "MarkdownPadStrings");
					if (!LicenseHelper.ValidateLicense(localizedString, this))
					{
						this.ComboBox_UpdateFrequency.SelectedItem = args.RemovedItems[0];
						args.Handled = true;
						return;
					}
				}
				if ((UpdateFrequency)this.ComboBox_UpdateFrequency.SelectedValue != UpdateFrequency.Never)
				{
					return;
				}
				object selectedItem = args.RemovedItems[0];
				MessageBoxResult messageBoxResult = CustomMessageBox.ShowOKCancel(LocalizationProvider.GetLocalizedString("UpdateFrequency_NeverWarningMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("UpdateFrequency_NeverWarningTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("UpdateFrequency_NeverButton", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Cancel", false, "MarkdownPadStrings"), MessageBoxImage.Exclamation);
				if (messageBoxResult != MessageBoxResult.OK)
				{
					this.ComboBox_UpdateFrequency.SelectedItem = selectedItem;
					args.Handled = true;
				}
			};
		}
		private void LoadCSSFiles(string fileToSelect)
		{
			string[] array = new string[0];
			string stylesheetDirectory = StyleSheetProvider.StylesheetDirectory;
			if (System.IO.Directory.Exists(stylesheetDirectory))
			{
				try
				{
					array = System.IO.Directory.GetFiles(stylesheetDirectory, "*.css");
				}
				catch (System.Exception exception)
				{
					MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_OpeningCssFileMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_OpeningCssFileTitle", false, "MarkdownPadStrings"), exception, "");
				}
			}
			OptionsWindow._logger.Trace(array.Length + " CSS files found in " + stylesheetDirectory);
			if (array.Length > 0)
			{
				this.AddCSSFilesToList(array);
				foreach (System.IO.FileInfo fileInfo in (System.Collections.IEnumerable)this.listBox_CSS.Items)
				{
					if (fileInfo.FullName == fileToSelect)
					{
						this.listBox_CSS.SelectedItem = fileInfo;
					}
				}
			}
		}
		private void AddCSSFilesToList(string[] files)
		{
			this.cssFileList.Clear();
			for (int i = 0; i < files.Length; i++)
			{
				string text = files[i];
				if (System.IO.File.Exists(text))
				{
					System.IO.FileInfo item = new System.IO.FileInfo(text);
					this.cssFileList.Add(item);
				}
			}
			this.listBox_CSS.ItemsSource = this.cssFileList;
			this.listBox_CSS.DisplayMemberPath = "Name";
		}
		private void UpdateCSSPreview(string cssFileLocation)
		{
			string css = string.Empty;
			string html = string.Empty;
			try
			{
				if (System.IO.File.Exists(cssFileLocation))
				{
					using (System.IO.StreamReader streamReader = new System.IO.StreamReader(cssFileLocation))
					{
						css = streamReader.ReadToEnd();
					}
				}
				html = this.GetCSSPreviewSource(css);
				this.Browser_CSSPreview.LoadHTML(html);
			}
			catch (System.Exception exception)
			{
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_OpeningCssFileMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_OpeningCssFileTitle", false, "MarkdownPadStrings"), exception, "");
			}
		}
		private string GetCSSPreviewSource(string css)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.AppendLine("<html><head>");
			stringBuilder.AppendLine("<style type=\"text/css\">");
			stringBuilder.AppendLine(css);
			stringBuilder.AppendLine("</style></head><body>");
			stringBuilder.AppendLine(string.Format("<p>{0}</p>", LocalizationProvider.GetLocalizedString("CssPreview_Paragraph", false, "MarkdownPadStrings")));
			stringBuilder.AppendLine(string.Format("<h1>{0}</h1>", LocalizationProvider.GetLocalizedString("CssPreview_Heading1", false, "MarkdownPadStrings")));
			stringBuilder.AppendLine(string.Format("<h2>{0}</h2>", LocalizationProvider.GetLocalizedString("CssPreview_Heading2", false, "MarkdownPadStrings")));
			stringBuilder.AppendLine(string.Format("<p><code>{0}</code></p>", LocalizationProvider.GetLocalizedString("CssPreview_Code", false, "MarkdownPadStrings")));
			stringBuilder.AppendLine(string.Format("<p><blockquote>{0}</blockquote></p>", LocalizationProvider.GetLocalizedString("CssPreview_Blockquote", false, "MarkdownPadStrings")));
			stringBuilder.AppendLine(string.Format("<p><a href=\"#\">{0}</a>", LocalizationProvider.GetLocalizedString("CssPreview_Hyperlink", false, "MarkdownPadStrings")));
			stringBuilder.AppendLine("</body></html>");
			return stringBuilder.ToString();
		}
		private void listBox_CSS_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
		}
		private void Button_ModifyCSS_Click(object sender, RoutedEventArgs e)
		{
			this.EditFile();
		}
		private void EditFile()
		{
			if (this.listBox_CSS.SelectedItem != null)
			{
				System.IO.FileInfo fileInfo = (System.IO.FileInfo)this.listBox_CSS.SelectedItem;
				string fullName = fileInfo.FullName;
				this.OpenCodeEditor(fullName);
				this.LoadCSSFiles(fullName);
				this.UpdateCSSPreview(fullName);
			}
		}
		private void OpenCodeEditor(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				System.IO.FileInfo file = new System.IO.FileInfo(fileName);
				new CodeEditor(file)
				{
					Owner = this
				}.ShowDialog();
			}
		}
		private void Button_AddCSS_Click(object sender, RoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_UnlimitedStylesheets", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				return;
			}
			string text = string.Empty;
			if (this.listBox_CSS.SelectedItem != null)
			{
				System.IO.FileInfo fileInfo = (System.IO.FileInfo)this.listBox_CSS.SelectedItem;
				text = fileInfo.FullName;
			}
			CodeEditor codeEditor = new CodeEditor();
			codeEditor.Owner = this;
			codeEditor.ShowDialog();
			if (!string.IsNullOrEmpty(codeEditor.FileName))
			{
				text = codeEditor.FileName;
			}
			this.LoadCSSFiles(text);
			this.UpdateCSSPreview(text);
		}
		private void Button_RemoveCSS_Click(object sender, RoutedEventArgs e)
		{
			if (this.listBox_CSS.SelectedItem == null)
			{
				return;
			}
			System.IO.FileInfo fileInfo = (System.IO.FileInfo)this.listBox_CSS.SelectedItem;
			MessageBoxResult messageBoxResult = CustomMessageBox.ShowYesNo(LocalizationProvider.GetLocalizedString("ConfirmDeleteFile_Message", true, "MarkdownPadStrings") + fileInfo.Name, LocalizationProvider.GetLocalizedString("ConfirmDeleteFile_Title", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Yes", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("No", false, "MarkdownPadStrings"), MessageBoxImage.Exclamation);
			if (messageBoxResult != MessageBoxResult.Yes)
			{
				return;
			}
			try
			{
				int num = this.listBox_CSS.SelectedIndex;
				if (num == this.listBox_CSS.Items.Count - 1 && this.listBox_CSS.Items.Count > 1)
				{
					num--;
				}
				fileInfo.Delete();
				this.cssFileList.Remove(fileInfo);
				string text = string.Empty;
				if (this.listBox_CSS.Items.Count > 0 && this.listBox_CSS.Items[num] != null)
				{
					System.IO.FileInfo fileInfo2 = (System.IO.FileInfo)this.listBox_CSS.Items[num];
					text = fileInfo2.FullName;
				}
				this.LoadCSSFiles(text);
				this.UpdateCSSPreview(text);
			}
			catch (System.Exception exception)
			{
				OptionsWindow._logger.ErrorException("Error deleting CSS file", exception);
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("ErrorDeletingFile_Message", true, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("ErrorDeletingFile_Title", false, "MarkdownPadStrings"), exception, "");
			}
		}
		private void listBox_CSS_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (this.listBox_CSS.SelectedItem != null)
			{
				System.IO.FileInfo fileInfo = (System.IO.FileInfo)this.listBox_CSS.SelectedItem;
				this.UpdateCSSPreview(fileInfo.FullName);
			}
		}
		private void MenuItem_NewPangram_Click(object sender, RoutedEventArgs e)
		{
			string[] array = new string[]
			{
				"The quick onyx goblin jumps over the lazy dwarf.",
				"Sphinx of black quartz, judge my vow.",
				"How quickly daft jumping zebras vex.",
				"Two driven jocks help fax my big quiz.",
				"The five boxing wizards jump quickly.",
				"Watch \"Jeopardy!\", Alex Trebek's fun TV quiz game.",
				"Grumpy wizards make a toxic brew for the jovial queen.",
				"Few black taxis drive up major roads on quiet hazy nights.",
				"A quick movement of the enemy will jeopardize six gunboats."
			};
			System.Random random = new System.Random();
			this.TextEditor_PreviewText.Text = array[random.Next(0, array.Length)];
		}
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			string absoluteUri = e.Uri.AbsoluteUri;
			absoluteUri.TryStartDefaultProcess();
			e.Handled = true;
		}
		private void Button_HTML_Head_Editor_Click(object sender, RoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_CustomHtmlHead", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				e.Handled = true;
				return;
			}
			CodeEditor codeEditor = new CodeEditor(this._settings.Renderer_CustomHtmlHeadContent)
			{
				Owner = this
			};
			codeEditor.ShowDialog();
		}
		private void Button_Reset_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult messageBoxResult = CustomMessageBox.ShowOKCancel(LocalizationProvider.GetLocalizedString("ResetSettingsMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("ResetSettingsTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("ResetSettings", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Cancel", false, "MarkdownPadStrings"), MessageBoxImage.Exclamation);
			if (messageBoxResult != MessageBoxResult.OK)
			{
				return;
			}
			OptionsWindow._logger.Info("Resetting settings");
			this._settings.Reset();
			this.SettingsSaved = true;
			base.Close();
		}
		private void Button_RestoreDefaultStylesheets_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult messageBoxResult = CustomMessageBox.ShowYesNo(LocalizationProvider.GetLocalizedString("ConfirmRestoreDefaultStylesheetsMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("ConfirmRestoreDefaultStylesheetsTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Button_RestoreDefaultStylesheets", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Cancel", false, "MarkdownPadStrings"), MessageBoxImage.Question);
			if (messageBoxResult != MessageBoxResult.Yes)
			{
				return;
			}
			string fullName = ((System.IO.FileInfo)this.listBox_CSS.SelectedItem).FullName;
			StyleSheetProvider styleSheetProvider = new StyleSheetProvider();
			styleSheetProvider.WriteBuiltInStylesheetsToAppData(true);
			this.LoadCSSFiles(fullName);
			this.UpdateCSSPreview(fullName);
		}
		private void Browser_CSSPreview_OnShowContextMenu(object sender, Awesomium.Core.ContextMenuEventArgs e)
		{
			e.Handled = true;
		}
		private void Button_MarkdownExtra_OnClick(object sender, RoutedEventArgs e)
		{
			Urls.Doc_PhpMarkdownExtra.TryStartDefaultProcess();
		}
		private void Hyperlink_HelpTranslate_OnClick(object sender, RoutedEventArgs e)
		{
			Urls.MarkdownPad_Translate.TryStartDefaultProcess();
		}
		private void Hyperlink_AddDictionaries_OnClick(object sender, RoutedEventArgs e)
		{
			Urls.MarkdownPad_Faq_AddSpellCheckDictionaries.TryStartDefaultProcess();
		}
		private void CheckBox_EnableAutoSave_OnClick(object sender, RoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_AutoSave", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				this.CheckBox_EnableAutoSave.IsChecked = new bool?(false);
				e.Handled = true;
			}
		}
		private void CheckBox_SubmitAnonymousStats_OnClick(object sender, RoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_DisableStatistics", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				this.CheckBox_SubmitAnonymousStats.IsChecked = new bool?(true);
				e.Handled = true;
			}
		}
		private void Button_TimestampInfo_OnClick(object sender, RoutedEventArgs e)
		{
			string.Format(Urls.Documentation_Timestamp, this._settings.App_Locale.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture)).TryStartDefaultProcess();
		}
		private void Button_RenderingModeTooltip_OnClick(object sender, RoutedEventArgs e)
		{
			Urls.Faq_RenderingMode.TryStartDefaultProcess();
		}
		private void Button_MarkdownReferenceFile_Browse_OnClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = FileDialogHelper.OpenMarkdownFileTypes,
				RestoreDirectory = true,
				Multiselect = false
			};
			if (!openFileDialog.ShowDialog().Value || string.IsNullOrEmpty(openFileDialog.FileName) || !System.IO.File.Exists(openFileDialog.FileName))
			{
				return;
			}
			this.TextBox_MarkdownReferenceFile.Text = openFileDialog.FileName;
		}
		private void Button_MarkdownReferenceFile_Clear_OnClick(object sender, RoutedEventArgs e)
		{
			this.TextBox_MarkdownReferenceFile.Text = string.Empty;
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/options/optionswindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((OptionsWindow)target).Loaded += new RoutedEventHandler(this.Window_Loaded);
				((OptionsWindow)target).Closing += new CancelEventHandler(this.Window_Closing);
				return;
			case 3:
				this.TabControl_Main = (TabControl)target;
				return;
			case 4:
				this.ColorPicker_BackgroundColor = (ColorPicker)target;
				return;
			case 5:
				this.ColorPicker_ForegroundColor = (ColorPicker)target;
				return;
			case 6:
				this.ComboBox_FontWeight = (ComboBox)target;
				return;
			case 7:
				this.ComboBox_FontSize = (ComboBox)target;
				return;
			case 8:
				this.ComboBox_FontFamily = (ComboBox)target;
				return;
			case 9:
				this.TextEditor_PreviewText = (TextEditor)target;
				return;
			case 10:
				this.MenuItem_NewPangram = (MenuItem)target;
				this.MenuItem_NewPangram.Click += new RoutedEventHandler(this.MenuItem_NewPangram_Click);
				return;
			case 11:
				this.CheckBox_InsertSpacesInsteadOfTab = (CheckBox)target;
				return;
			case 12:
				this.CheckBox_AutoContinueLists = (CheckBox)target;
				return;
			case 13:
				this.CheckBox_DisplayFormattingMarks = (CheckBox)target;
				return;
			case 14:
				this.CheckBox_EnableClickableLinksInEditor = (CheckBox)target;
				return;
			case 15:
				this.ColorPicker_HyperlinkForegroundColor = (ColorPicker)target;
				return;
			case 16:
				this.Button_TimestampInfo = (Button)target;
				this.Button_TimestampInfo.Click += new RoutedEventHandler(this.Button_TimestampInfo_OnClick);
				return;
			case 17:
				this.TextBox_TimestampFormat = (TextBox)target;
				return;
			case 18:
				this.Grid_Languages = (Grid)target;
				return;
			case 19:
				this.ComboBox_UserInterfaceLanguages = (ComboBox)target;
				return;
			case 20:
				this.Hyperlink_HelpTranslate = (Hyperlink)target;
				this.Hyperlink_HelpTranslate.Click += new RoutedEventHandler(this.Hyperlink_HelpTranslate_OnClick);
				return;
			case 21:
				this.ComboBox_MarkdownProcessors = (ComboBox)target;
				return;
			case 22:
				this.Button_RenderingModeTooltip = (Button)target;
				this.Button_RenderingModeTooltip.Click += new RoutedEventHandler(this.Button_RenderingModeTooltip_OnClick);
				return;
			case 23:
				this.ComboBox_RenderingMode = (ComboBox)target;
				return;
			case 24:
				this.CheckBox_MarkdownClassic_AutomaticNewlines = (CheckBox)target;
				return;
			case 25:
				this.CheckBox_MarkdownClassic_AutomaticHyperlinks = (CheckBox)target;
				return;
			case 26:
				this.CheckBox_MarkdownClassic_AutomaticEmailHyperlinks = (CheckBox)target;
				return;
			case 27:
				this.CheckBox_MarkdownClassic_EncodeProblematicUrls = (CheckBox)target;
				return;
			case 28:
				this.CheckBox_GFM_AnonymousMode = (CheckBox)target;
				return;
			case 29:
				this.TextBox_GithubUsername = (TextBox)target;
				return;
			case 30:
				this.PasswordBox_GithubPassword = (PasswordBox)target;
				return;
			case 31:
				this.CheckBox_OfflineGFM_AutomaticNewlines = (CheckBox)target;
				return;
			case 32:
				this.CheckBox_OfflineGFM_SmartLists = (CheckBox)target;
				return;
			case 33:
				this.CheckBox_OfflineGFM_SmartyPants = (CheckBox)target;
				return;
			case 34:
				this.ComboBox_UnorderedListSyntax = (ComboBox)target;
				return;
			case 35:
				this.CheckBox_UseUnderlineStyleHeadings = (CheckBox)target;
				return;
			case 36:
				this.listBox_CSS = (ListBox)target;
				this.listBox_CSS.MouseUp += new MouseButtonEventHandler(this.listBox_CSS_MouseUp);
				this.listBox_CSS.MouseDoubleClick += new MouseButtonEventHandler(this.listBox_CSS_MouseDoubleClick);
				return;
			case 37:
				this.Button_AddCSS = (Button)target;
				this.Button_AddCSS.Click += new RoutedEventHandler(this.Button_AddCSS_Click);
				return;
			case 38:
				this.Button_ModifyCSS = (Button)target;
				this.Button_ModifyCSS.Click += new RoutedEventHandler(this.Button_ModifyCSS_Click);
				return;
			case 39:
				this.Button_RemoveCSS = (Button)target;
				this.Button_RemoveCSS.Click += new RoutedEventHandler(this.Button_RemoveCSS_Click);
				return;
			case 40:
				this.Button_RestoreDefaultStylesheets = (Button)target;
				this.Button_RestoreDefaultStylesheets.Click += new RoutedEventHandler(this.Button_RestoreDefaultStylesheets_Click);
				return;
			case 41:
				this.Browser_CSSPreview = (WebControl)target;
				this.Browser_CSSPreview.ShowContextMenu += new ShowContextMenuEventHandler(this.Browser_CSSPreview_OnShowContextMenu);
				return;
			case 42:
				this.CheckBox_EnableAutoSave = (CheckBox)target;
				this.CheckBox_EnableAutoSave.Click += new RoutedEventHandler(this.CheckBox_EnableAutoSave_OnClick);
				return;
			case 43:
				this.IntegerUpDown_AutoSaveFrequency = (IntegerUpDown)target;
				return;
			case 44:
				this.CheckBox_MonitorFileChanges = (CheckBox)target;
				return;
			case 45:
				this.CheckBox_AutomaticReloadOnExternalChanges = (CheckBox)target;
				return;
			case 46:
				this.CheckBox_ScrollToEndOfDocumentOnLoad = (CheckBox)target;
				return;
			case 47:
				this.CheckBox_OpenHtmlFileAfterExport = (CheckBox)target;
				return;
			case 48:
				this.CheckBox_OpenPdfFileAfterExport = (CheckBox)target;
				return;
			case 49:
				this.CheckBox_PdfIncludeBackground = (CheckBox)target;
				return;
			case 50:
				this.CheckBox_PdfEnableOutlineGeneration = (CheckBox)target;
				return;
			case 51:
				this.CheckBox_PdfLandscapeMode = (CheckBox)target;
				return;
			case 52:
				this.IntUpDown_Pdf_MarginLeft = (IntegerUpDown)target;
				return;
			case 53:
				this.IntUpDown_Pdf_MarginTop = (IntegerUpDown)target;
				return;
			case 54:
				this.IntUpDown_Pdf_MarginRight = (IntegerUpDown)target;
				return;
			case 55:
				this.IntUpDown_Pdf_MarginBottom = (IntegerUpDown)target;
				return;
			case 56:
				this.ComboBox_PaperSize = (ComboBox)target;
				return;
			case 57:
				this.CheckBox_ExportHtmlWithBom = (CheckBox)target;
				return;
			case 58:
				this.CheckBox_ExportTextWithBom = (CheckBox)target;
				return;
			case 59:
				this.CheckBox_UseUnixEol = (CheckBox)target;
				return;
			case 60:
				this.CheckBox_VerifyInternetConnection = (CheckBox)target;
				return;
			case 61:
				((Button)target).Click += new RoutedEventHandler(this.Button_HTML_Head_Editor_Click);
				return;
			case 62:
				this.TextBox_MarkdownReferenceFile = (TextBox)target;
				return;
			case 63:
				this.Button_MarkdownReferenceFile_Browse = (Button)target;
				this.Button_MarkdownReferenceFile_Browse.Click += new RoutedEventHandler(this.Button_MarkdownReferenceFile_Browse_OnClick);
				return;
			case 64:
				this.Button_MarkdownReferenceFile_Clear = (Button)target;
				this.Button_MarkdownReferenceFile_Clear.Click += new RoutedEventHandler(this.Button_MarkdownReferenceFile_Clear_OnClick);
				return;
			case 65:
				this.ComboBox_ProxySettings = (ComboBox)target;
				return;
			case 66:
				this.TextBox_ManualProxy = (TextBox)target;
				return;
			case 67:
				this.TextBox_ManualProxyPort = (TextBox)target;
				return;
			case 68:
				this.CheckBox_DisplaySplashScreen = (CheckBox)target;
				return;
			case 69:
				this.CheckBox_SubmitAnonymousStats = (CheckBox)target;
				this.CheckBox_SubmitAnonymousStats.Click += new RoutedEventHandler(this.CheckBox_SubmitAnonymousStats_OnClick);
				return;
			case 70:
				this.ComboBox_UpdateFrequency = (ComboBox)target;
				return;
			case 71:
				((Button)target).Click += new RoutedEventHandler(this.Button_Reset_Click);
				return;
			case 72:
				this.OptionsPage_Highlighting = (SyntaxHighlightingOptions)target;
				return;
			case 73:
				this.Button_Save = (Button)target;
				this.Button_Save.Click += new RoutedEventHandler(this.Button_Save_Click);
				return;
			case 74:
				this.Button_Cancel = (Button)target;
				this.Button_Cancel.Click += new RoutedEventHandler(this.Button_Cancel_Click);
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
