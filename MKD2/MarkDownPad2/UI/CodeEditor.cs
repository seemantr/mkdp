using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using MarkdownPad2.Stylesheets;
using MarkdownPad2.Utilities;
using NLog;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using WPFCustomMessageBox;
namespace MarkdownPad2.UI
{
	public class CodeEditor : Window, System.Windows.Markup.IComponentConnector
	{
		public enum CodeDocumentType
		{
			Css,
			Html
		}
		private Settings _settings = Settings.Default;
		private Logger _logger = LogManager.GetCurrentClassLogger();
		private SearchPanel _searchPanel;
		private readonly string newStyle = LocalizationProvider.GetLocalizedString("CodeEditor_NewStyle", false, "MarkdownPadStrings");
		internal TextBlock TextBlock_FileNameTitle;
		internal TextBox TextBox_Filename;
		internal TextEditor Editor;
		internal ComboBox ComboBox_Fonts;
		internal ComboBox ComboBox_FontSize;
		internal CheckBox CheckBox_LineNumbersEnabled;
		internal Button Button_Save;
		internal Button Button_Cancel;
		private bool _contentLoaded;
		public string FileName
		{
			get;
			private set;
		}
		private string OriginalDocument
		{
			get;
			set;
		}
		private bool IsNewFile
		{
			get;
			set;
		}
		private CodeEditor.CodeDocumentType CodeType
		{
			get;
			set;
		}
		public bool HasChanges
		{
			get
			{
				return this.OriginalDocument != this.Editor.Text;
			}
		}
		public CodeEditor()
		{
			this.InitializeComponent();
			this.StandardInit();
			this.CodeType = CodeEditor.CodeDocumentType.Css;
			this.IsNewFile = true;
			string stylesheetDirectory = StyleSheetProvider.StylesheetDirectory;
			this.FileName = stylesheetDirectory + this.newStyle + ".css";
			int num = 1;
			while (System.IO.File.Exists(this.FileName))
			{
				this._logger.Trace("File " + this.FileName + " already exists. Incrementing filename.");
				this.FileName = string.Concat(new object[]
				{
					stylesheetDirectory,
					this.newStyle,
					" (",
					num,
					").css"
				});
				num++;
			}
			this.TextBox_Filename.Text = System.IO.Path.GetFileName(this.FileName);
			this.TextBox_Filename.Focus();
			this.TextBox_Filename.SelectAll();
		}
		public CodeEditor(System.IO.FileInfo file)
		{
			this.InitializeComponent();
			this.CodeType = CodeEditor.CodeDocumentType.Css;
			this.FileName = file.ToString();
			this.TextBox_Filename.Text = System.IO.Path.GetFileName(file.ToString());
			this.IsNewFile = false;
			this.TextBox_Filename.IsReadOnly = true;
			this.TextBox_Filename.Background = new SolidColorBrush(Colors.LightGray);
			this.TextBox_Filename.Foreground = new SolidColorBrush(Colors.DimGray);
			this.LoadTextFile(file.ToString());
			this.StandardInit();
			this.Editor.Focus();
		}
		public CodeEditor(string fileContents)
		{
			this.InitializeComponent();
			this.CodeType = CodeEditor.CodeDocumentType.Html;
			this.TextBox_Filename.Visibility = Visibility.Collapsed;
			this.TextBlock_FileNameTitle.Visibility = Visibility.Collapsed;
			this.IsNewFile = false;
			this.Editor.Text = fileContents;
			this.StandardInit();
			this.Editor.Focus();
		}
		private void StandardInit()
		{
			this.LoadSettings();
			this.ComboBox_Fonts.SelectionChanged += new SelectionChangedEventHandler(this.ComboBox_Fonts_SelectionChanged);
			this.ComboBox_FontSize.SelectionChanged += new SelectionChangedEventHandler(this.ComboBox_FontSize_SelectionChanged);
			this.CheckBox_LineNumbersEnabled.Click += new RoutedEventHandler(this.CheckBox_LineNumbersEnabled_Click);
			this.OriginalDocument = this.Editor.Text;
			this.Editor.TextArea.DefaultInputHandler.NestedInputHandlers.Add(new SearchInputHandler(this.Editor.TextArea));
			this.ToggleSyntaxHighlighting(true);
		}
		private void CheckBox_LineNumbersEnabled_Click(object sender, RoutedEventArgs e)
		{
			this.Editor.ShowLineNumbers = this.CheckBox_LineNumbersEnabled.IsChecked.Value;
		}
		private void LoadSettings()
		{
			this.ComboBox_Fonts.ItemsSource = Fonts.SystemFontFamilies;
			if (this.ComboBox_Fonts.Items.Contains(this._settings.CodeEditor_FontFamily))
			{
				this.ComboBox_Fonts.SelectedItem = this._settings.CodeEditor_FontFamily;
			}
			else
			{
				if (this.ComboBox_Fonts.Items.Contains(new FontFamily("Courier New")))
				{
					this.ComboBox_Fonts.SelectedItem = new FontFamily("Courier New");
				}
			}
			if (this.ComboBox_Fonts.SelectedItem != null)
			{
				this.Editor.FontFamily = (FontFamily)this.ComboBox_Fonts.SelectedItem;
			}
			double[] itemsSource = new double[]
			{
				8.0,
				9.0,
				10.0,
				11.0,
				12.0,
				14.0,
				16.0,
				18.0,
				20.0,
				24.0,
				26.0,
				28.0,
				36.0,
				48.0,
				72.0
			};
			this.ComboBox_FontSize.ItemsSource = itemsSource;
			if (this.ComboBox_FontSize.Items.Contains(this._settings.CodeEditor_FontSize))
			{
				this.ComboBox_FontSize.SelectedItem = this._settings.CodeEditor_FontSize;
			}
			if (this.ComboBox_Fonts.SelectedItem != null)
			{
				this.Editor.FontSize = (double)this.ComboBox_FontSize.SelectedItem;
			}
			this.Editor.ShowLineNumbers = this._settings.CodeEditor_DisplayLineNumbers;
			this.CheckBox_LineNumbersEnabled.IsChecked = new bool?(this._settings.CodeEditor_DisplayLineNumbers);
		}
		private void ComboBox_Fonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FontFamily fontFamily = (FontFamily)this.ComboBox_Fonts.SelectedItem;
			this.Editor.FontFamily = fontFamily;
		}
		private void ComboBox_FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			double fontSize = (double)this.ComboBox_FontSize.SelectedItem;
			this.Editor.FontSize = fontSize;
		}
		private void Button_Save_Click(object sender, RoutedEventArgs e)
		{
			this.Save();
			base.Close();
		}
		public void Find()
		{
			if (this._searchPanel == null || this._searchPanel.IsClosed)
			{
				this._searchPanel = new SearchPanel();
				this._searchPanel.Attach(this.Editor.TextArea);
			}
			this._searchPanel.SearchPattern = this.Editor.TextArea.Selection.GetText();
			System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, delegate
			{
				this._searchPanel.Reactivate();
			});
		}
		public void FindNext()
		{
			if (this._searchPanel != null)
			{
				this._searchPanel.FindNext();
			}
		}
		public void FindPrevious()
		{
			if (this._searchPanel != null)
			{
				this._searchPanel.FindPrevious();
			}
		}
		public void CloseSearchPanel()
		{
			if (this._searchPanel != null)
			{
				this._searchPanel.Close();
			}
			this._searchPanel = null;
		}
		private void Save()
		{
			switch (this.CodeType)
			{
			case CodeEditor.CodeDocumentType.Css:
				this.SaveCSSFile();
				break;
			case CodeEditor.CodeDocumentType.Html:
				this.SaveHtmlToSettings();
				break;
			}
			this.SaveSettings();
		}
		private void SaveHtmlToSettings()
		{
			this.OriginalDocument = this.Editor.Text;
			this._settings.Renderer_CustomHtmlHeadContent = this.Editor.Text;
		}
		private void SaveCSSFile()
		{
			string stylesheetDirectory = StyleSheetProvider.StylesheetDirectory;
			if (!string.IsNullOrEmpty(this.TextBox_Filename.Text))
			{
				string extension = System.IO.Path.GetExtension(this.TextBox_Filename.Text);
				if (!extension.Equals(".css", System.StringComparison.OrdinalIgnoreCase))
				{
					TextBox expr_3D = this.TextBox_Filename;
					expr_3D.Text += ".css";
				}
				this.FileName = stylesheetDirectory + this.TextBox_Filename.Text;
			}
			else
			{
				this.FileName = stylesheetDirectory + this.newStyle + ".css";
				int num = 1;
				while (System.IO.File.Exists(this.FileName))
				{
					this._logger.Trace("Saving: File " + this.FileName + " already exists. Incrementing filename.");
					this.FileName = string.Concat(new object[]
					{
						stylesheetDirectory,
						this.newStyle,
						" (",
						num,
						").css"
					});
					num++;
				}
			}
			if (string.IsNullOrEmpty(this.FileName))
			{
				return;
			}
			string directoryName = System.IO.Path.GetDirectoryName(this.FileName);
			if (directoryName != null && !System.IO.Directory.Exists(directoryName))
			{
				try
				{
					System.IO.Directory.CreateDirectory(directoryName);
				}
				catch (System.Exception exception)
				{
					this._logger.ErrorException("Error creating directory for CSS save.", exception);
				}
			}
			try
			{
				this.Editor.Save(this.FileName);
				this.OriginalDocument = this.Editor.Text;
				this.IsNewFile = false;
			}
			catch (System.Exception exception2)
			{
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("CssFileSaveErrorMessage", true, "MarkdownPadStrings") + this.FileName, LocalizationProvider.GetLocalizedString("CssFileSaveErrorTitle", false, "MarkdownPadStrings"), exception2, "");
			}
		}
		private void SaveSettings()
		{
			this._settings.CodeEditor_DisplayLineNumbers = this.CheckBox_LineNumbersEnabled.IsChecked.Value;
			if (this.ComboBox_Fonts.SelectedItem != null)
			{
				this._settings.CodeEditor_FontFamily = (FontFamily)this.ComboBox_Fonts.SelectedItem;
			}
			if (this.ComboBox_FontSize.SelectedItem != null)
			{
				this._settings.CodeEditor_FontSize = (double)this.ComboBox_FontSize.SelectedItem;
			}
			try
			{
				this._settings.Save();
			}
			catch (System.Exception exception)
			{
				this._logger.ErrorException("Error saving settings", exception);
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_SavingSettingsMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_SavingSettingsTitle", false, "MarkdownPadStrings"), exception, "");
			}
		}
		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.FileName = string.Empty;
			base.Close();
		}
		private void LoadTextFile(string fileName)
		{
			try
			{
				this.Editor.Load(fileName);
			}
			catch (System.Exception exception)
			{
				this._logger.ErrorException("Error opening file: " + fileName, exception);
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("ErrorWhileOpeningFileMessage", true, "MarkdownPadStrings") + fileName, LocalizationProvider.GetLocalizedString("ErrorWhileOpeningFileTitle", false, "MarkdownPadStrings"), exception, "");
			}
		}
		private void ToggleSyntaxHighlighting(bool enable)
		{
			if (!enable)
			{
				this.Editor.SyntaxHighlighting = null;
				return;
			}
			IHighlightingDefinition syntaxHighlighting = null;
			switch (this.CodeType)
			{
			case CodeEditor.CodeDocumentType.Css:
				try
				{
					XmlTextReader xmlTextReaderFromResourceStream = XmlUtilities.GetXmlTextReaderFromResourceStream("MarkdownPad2.SyntaxRules.css.xshd");
					syntaxHighlighting = HighlightingLoader.Load(xmlTextReaderFromResourceStream, HighlightingManager.Instance);
				}
				catch (System.Exception exception)
				{
					this._logger.ErrorException("Error loading syntax file: " + this.CodeType, exception);
					MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_LoadingSyntaxHighlighterMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_LoadingSyntaxHighlighterTitle", false, "MarkdownPadStrings"), exception, "");
				}
				break;
			case CodeEditor.CodeDocumentType.Html:
				syntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
				break;
			}
			this.Editor.SyntaxHighlighting = syntaxHighlighting;
		}
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (!this.HasChanges)
			{
				if (this.IsNewFile)
				{
					this.FileName = string.Empty;
				}
				return;
			}
			MessageBoxResult messageBoxResult = CustomMessageBox.ShowYesNoCancel(LocalizationProvider.GetLocalizedString("SaveChangesMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("SaveChangesTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Save", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("DoNotSave", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Cancel", false, "MarkdownPadStrings"), MessageBoxImage.Question);
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				this.Save();
				return;
			}
			if (messageBoxResult == MessageBoxResult.Cancel)
			{
				e.Cancel = true;
				return;
			}
			this.FileName = string.Empty;
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}
		private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			this.Find();
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/ui/codeeditor.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((CodeEditor)target).Loaded += new RoutedEventHandler(this.Window_Loaded);
				((CodeEditor)target).Closing += new CancelEventHandler(this.Window_Closing);
				return;
			case 2:
				this.TextBlock_FileNameTitle = (TextBlock)target;
				return;
			case 3:
				this.TextBox_Filename = (TextBox)target;
				return;
			case 4:
				this.Editor = (TextEditor)target;
				return;
			case 5:
				this.ComboBox_Fonts = (ComboBox)target;
				return;
			case 6:
				this.ComboBox_FontSize = (ComboBox)target;
				return;
			case 7:
				this.CheckBox_LineNumbersEnabled = (CheckBox)target;
				return;
			case 8:
				this.Button_Save = (Button)target;
				this.Button_Save.Click += new RoutedEventHandler(this.Button_Save_Click);
				return;
			case 9:
				this.Button_Cancel = (Button)target;
				this.Button_Cancel.Click += new RoutedEventHandler(this.Button_Cancel_Click);
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}
	}
}
