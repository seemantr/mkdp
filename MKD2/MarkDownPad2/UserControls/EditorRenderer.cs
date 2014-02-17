using Awesomium.Core;
using Awesomium.Windows.Controls;
using MarkdownPad2.Core;
using MarkdownPad2.Export;
using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using MarkdownPad2.Stylesheets;
using MarkdownPad2.Utilities;
using Microsoft.Win32;
using NLog;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using WPFCustomMessageBox;
namespace MarkdownPad2.UserControls
{
	public class EditorRenderer : UserControl, INotifyPropertyChanged, System.IDisposable, System.Windows.Markup.IComponentConnector
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		private Settings _settings = Settings.Default;
		private System.Windows.Threading.DispatcherTimer RenderTimer;
		internal Grid Grid_Main;
		internal ColumnDefinition Column_Editor;
		internal ColumnDefinition Column_Renderer;
		internal RowDefinition Row_Editor;
		internal RowDefinition Row_Renderer;
		internal Grid Grid_Editor;
		internal MarkdownEditor Editor;
		internal GridSplitter Splitter_V;
		internal GridSplitter Splitter_H;
		internal Grid Grid_Renderer;
		internal WebControl Renderer;
		private bool _contentLoaded;
		public event PropertyChangedEventHandler PropertyChanged;
		private ScrollViewer EditorScrollViewer
		{
			get;
			set;
		}
		public double ScrollViewerPositionPercentage
		{
			get
			{
				if (this.EditorScrollViewer == null)
				{
					EditorRenderer._logger.Warn("ScrollViewer was null, couldn't calculate percentage!");
					return 0.0;
				}
				double num = this.EditorScrollViewer.ExtentHeight - this.EditorScrollViewer.ViewportHeight;
				double result;
				if (num != 0.0)
				{
					result = this.EditorScrollViewer.VerticalOffset / num;
				}
				else
				{
					result = 0.0;
				}
				return result;
			}
		}
		public bool IsRendererLoaded
		{
			get;
			private set;
		}
		public bool IsLoadCompletedCompleted
		{
			get;
			private set;
		}
		public bool IsEditorLoaded
		{
			get;
			private set;
		}
		public string Filename
		{
			get;
			set;
		}
		public string Title
		{
			get
			{
				if (string.IsNullOrEmpty(this.Filename))
				{
					return LocalizationProvider.GetLocalizedString("NewDocument", false, "MarkdownPadStrings");
				}
				return System.IO.Path.GetFileName(this.Filename);
			}
		}
		public string CssFileLocation
		{
			get
			{
				return StyleSheetProvider.StylesheetDirectory + this._settings.CodeEditor_SelectedCssFileName;
			}
		}
		public string SaveHtmlFileTypes
		{
			get
			{
				string[] value = new string[]
				{
					"HTML Files (*.html)|*.html",
					"All Files (*.*)|*.*"
				};
				return string.Join("|", value);
			}
		}
		public string SavePdfFileTypes
		{
			get
			{
				string[] value = new string[]
				{
					"PDF Files (*.pdf)|*.pdf",
					"All Files (*.*)|*.*"
				};
				return string.Join("|", value);
			}
		}
		public string SaveMarkdownFileTypes
		{
			get
			{
				string[] value = new string[]
				{
					"Markdown Files (*.md)|*.md",
					"Markdown Files (*.markdown)|*.markdown",
					"Markdown Files (*.mdown)|*.mdown",
					"Text Files (*.txt)|*.txt",
					"All Files (*.*)|*.*"
				};
				return string.Join("|", value);
			}
		}
		public HtmlTemplate HtmlTemplate
		{
			get
			{
				return new HtmlTemplate
				{
					BodyContent = this.RenderedText,
					CssFilePath = this.CssFileLocation,
					CustomHeadContent = this._settings.Renderer_CustomHtmlHeadContent,
					Filename = this.Filename,
					MarkdownProcessor = this._settings.Markdown_MarkdownProcessor
				};
			}
		}
		public string RenderedText
		{
			get
			{
				string text = this.Editor.Text;
				string text2 = this.LoadSupplementalMarkdownFromFile();
				if (!string.IsNullOrEmpty(text2))
				{
					text = text + StringUtilities.GetNewLines(2) + text2;
				}
				return this.Editor.MarkdownProcessor.ConvertMarkdownToHTML(text);
			}
		}
		public EditorRenderer(string fileToOpen = "")
		{
			this.InitializeComponent();
			this.Editor.Loaded += new RoutedEventHandler(this.Editor_Loaded);
			this.Editor.TextChanged += new System.EventHandler(this.Editor_TextChanged);
			this.Renderer.Loaded += new RoutedEventHandler(this.Renderer_Loaded);
			this.Renderer.LoadCompleted += new System.EventHandler(this.Renderer_LoadCompleted);
			this.Renderer.OpenExternalLink += new OpenExternalLinkEventHandler(this.Renderer_OpenExternalLink);
			this.SetLayout(this._settings.App_VerticalLayoutEnabled);
			this.SetRendererBaseUrl(fileToOpen);
			this.LoadDocument(fileToOpen);
		}
		private void Renderer_OpenExternalLink(object sender, OpenExternalLinkEventArgs e)
		{
			if (e.Url.StartsWith("local://base_request.html/"))
			{
				string str = e.Url.Replace("local://base_request.html/", "");
				EditorRenderer._logger.Trace("Found local resource request: " + str);
				this.Editor.StatusMessage = LocalizationProvider.GetLocalizedString("InternalNavigationDisabledInLivePreview", false, "MarkdownPadStrings");
				return;
			}
			string url = e.Url;
			url.TryStartDefaultProcess();
		}
		private void Renderer_LoadCompleted(object sender, System.EventArgs e)
		{
			if (this.IsLoadCompletedCompleted)
			{
				return;
			}
			this.IsLoadCompletedCompleted = true;
			EditorRenderer._logger.Trace("Renderer_LoadCompleted fired");
			this.RenderTimer = new System.Windows.Threading.DispatcherTimer
			{
				Interval = this.Editor.MarkdownProcessor.RenderDelay
			};
			this.RenderTimer.Tick += new System.EventHandler(this.RenderTimer_Tick);
			if (!string.IsNullOrEmpty(this.Filename) || this.Editor.Text.Length != 0)
			{
				EditorRenderer._logger.Trace("Filename or content exists, kicking off the first Render");
				this.RenderTimer.Start();
				return;
			}
			EditorRenderer._logger.Trace("File doesn't exist (or empty doc), not rendering yet.");
		}
		private void Renderer_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.IsRendererLoaded)
			{
				return;
			}
			this.IsRendererLoaded = true;
			EditorRenderer._logger.Trace("Renderer_Loaded fired");
			this.LoadHtml();
		}
		public void ReloadSettings()
		{
			EditorRenderer._logger.Trace("Reloading settings");
			this.Editor.LoadEditorSettings();
			this.LoadHtml();
		}
		private string LoadSupplementalMarkdownFromFile()
		{
			string markdown_ReferenceFile = this._settings.Markdown_ReferenceFile;
			string result = string.Empty;
			if (!string.IsNullOrEmpty(markdown_ReferenceFile) && System.IO.File.Exists(markdown_ReferenceFile))
			{
				try
				{
					result = System.IO.File.ReadAllText(markdown_ReferenceFile);
				}
				catch (System.Exception exception)
				{
					EditorRenderer._logger.ErrorException("Unable to load supplemental Markdown file", exception);
				}
			}
			return result;
		}
		public void LoadHtml()
		{
			EditorRenderer._logger.Trace("Loading base HTML into renderer");
			this.IsLoadCompletedCompleted = false;
			HtmlTemplate htmlTemplate = new HtmlTemplate
			{
				BodyContent = string.Empty,
				CssFilePath = this.CssFileLocation,
				CustomHeadContent = this._settings.Renderer_CustomHtmlHeadContent,
				Filename = this.Filename,
				MarkdownProcessor = this._settings.Markdown_MarkdownProcessor
			};
			string html = htmlTemplate.GenerateHtmlTemplate(true);
			this.Renderer.LoadHTML(html);
		}
		public void LoadDocument(string filename)
		{
			this.Filename = filename;
			if (System.IO.File.Exists(filename))
			{
				this.Editor.LoadDocument(filename);
			}
		}
		public bool SaveDocument()
		{
			if (string.IsNullOrEmpty(this.Filename))
			{
				return this.SaveAsDocument();
			}
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(this.Filename);
			if (fileInfo.Exists && fileInfo.IsReadOnly)
			{
				MessageBoxResult messageBoxResult = CustomMessageBox.ShowYesNo(LocalizationProvider.GetLocalizedString("FileIsReadOnlyMessage", true, "MarkdownPadStrings") + fileInfo.FullName + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("MakeFileWritable", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("FileIsReadOnlyTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("MakeWritable", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("No", false, "MarkdownPadStrings"), MessageBoxImage.Exclamation);
				if (messageBoxResult != MessageBoxResult.Yes)
				{
					return false;
				}
				try
				{
					fileInfo.IsReadOnly = false;
				}
				catch (System.Exception exception)
				{
					MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_MakingFileWritableMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_MakingFileWritableTitle", false, "MarkdownPadStrings"), exception, "");
					bool result = false;
					return result;
				}
			}
			try
			{
				this.Editor.SaveDocument(this.Filename);
				this.SetRendererBaseUrl(this.Filename);
				bool result = true;
				return result;
			}
			catch (System.Exception exception2)
			{
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_SavingFileMessage", true, "MarkdownPadStrings") + fileInfo.FullName, LocalizationProvider.GetLocalizedString("Error_SavingFileTitle", false, "MarkdownPadStrings"), exception2, "");
			}
			return false;
		}
		private void SetRendererBaseUrl(string filename)
		{
			if (WebCore.IsRunning && System.IO.File.Exists(filename))
			{
				WebCore.BaseDirectory = System.IO.Path.GetDirectoryName(filename);
			}
		}
		public bool SaveAsDocument()
		{
			string text = this.Filename;
			if (string.IsNullOrEmpty(text))
			{
				text = this.Title;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = this.SaveMarkdownFileTypes,
				FilterIndex = this._settings.IO_SaveFile_LastUsedFileTypeIndex,
				RestoreDirectory = true,
				FileName = text
			};
			if (!saveFileDialog.ShowDialog().Value || string.IsNullOrEmpty(saveFileDialog.FileName))
			{
				return false;
			}
			EditorRenderer._logger.Trace("Saving file as: " + saveFileDialog.FileName);
			this._settings.IO_SaveFile_LastUsedFileTypeIndex = saveFileDialog.FilterIndex;
			this.Filename = saveFileDialog.FileName;
			this.SaveDocument();
			return true;
		}
		public void ExportHtml()
		{
			string text = this.Filename;
			if (string.IsNullOrEmpty(text))
			{
				text = this.Title;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = this.SaveHtmlFileTypes,
				FileName = System.IO.Path.GetFileNameWithoutExtension(text),
				RestoreDirectory = true
			};
			bool? flag = saveFileDialog.ShowDialog();
			string fileName = saveFileDialog.FileName;
			if (!flag.Value || string.IsNullOrEmpty(fileName))
			{
				return;
			}
			EditorRenderer._logger.Trace("Exporting HTML document: " + fileName);
			string text2 = this.HtmlTemplate.GenerateHtmlTemplate(false);
			System.Text.Encoding encoding = new System.Text.UTF8Encoding(this._settings.IO_ExportHtmlWithBom);
			using (System.IO.TextWriter textWriter = new System.IO.StreamWriter(fileName, false, encoding))
			{
				if (this._settings.IO_UseUnixStyleEol)
				{
					textWriter.NewLine = "\n";
					text2 = text2.ConvertTextToUnixEol();
				}
				else
				{
					textWriter.NewLine = "\r\n";
				}
				try
				{
					textWriter.Write(text2);
				}
				catch (System.Exception exception)
				{
					EditorRenderer._logger.ErrorException("Exception while exporting HTML", exception);
					MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_ExportingHtmlMessage", true, "MarkdownPadStrings") + fileName, LocalizationProvider.GetLocalizedString("Error_ExportingHtmlTitle", false, "MarkdownPadStrings"), exception, "");
				}
			}
			if (System.IO.File.Exists(fileName) && this._settings.IO_OpenHtmlFileAfterExport)
			{
				fileName.TryStartDefaultProcess();
			}
		}
		public void PreviewMarkdownInBrowser()
		{
			string str = string.Format("{0}.{1}", "MarkdownPadPreview", "html");
			string text = System.IO.Path.GetTempPath() + str;
			HtmlTemplate htmlTemplate = this.HtmlTemplate;
			htmlTemplate.UseBaseRelativeImageWorkaround = true;
			string contents = htmlTemplate.GenerateHtmlTemplate(false);
			try
			{
				System.IO.File.WriteAllText(text, contents);
			}
			catch (System.Exception exception)
			{
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_MarkdownBrowserPreviewMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_MarkdownBrowserPreviewTitle", false, "MarkdownPadStrings"), exception, "");
			}
			if (System.IO.File.Exists(text))
			{
				text.TryStartDefaultProcess();
			}
		}
		private void Editor_Loaded(object sender, RoutedEventArgs e)
		{
			this.ConnectScrollbarListener();
			if (this.IsEditorLoaded)
			{
				this.KickTick();
				return;
			}
			this.IsEditorLoaded = true;
			EditorRenderer._logger.Trace("Editor loaded (in EditorRenderer)");
		}
		private void EditorScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.VerticalChange == 0.0 && e.ViewportHeightChange == 0.0 && e.ViewportWidthChange == 0.0)
			{
				return;
			}
			if (!this.Editor.MarkdownProcessor.RenderWhileScrollingAllowed)
			{
				this.KickTick();
				return;
			}
			this.Render(true);
		}
		private void RenderTimer_Tick(object sender, System.EventArgs e)
		{
			EditorRenderer._logger.Trace("RenderTimer TICK!");
			this.RenderTimer.Stop();
			if (this.Renderer == null || !this.Renderer.IsLoaded)
			{
				EditorRenderer._logger.Warn("RenderTimer could not TICK because null renderer or not loaded renderer");
				return;
			}
			this.Render(false);
		}
		public void ToggleLivePreview(bool enable, bool isVerticalLayout)
		{
			if (!enable)
			{
				if (isVerticalLayout)
				{
					this.Column_Editor.MinWidth = 0.0;
					this.Column_Editor.Width = new GridLength(1.0, GridUnitType.Star);
					this.Column_Renderer.MinWidth = 0.0;
					this.Column_Renderer.Width = new GridLength(0.0, GridUnitType.Star);
					this.Splitter_V.Visibility = Visibility.Collapsed;
				}
				else
				{
					this.Row_Editor.MinHeight = 0.0;
					this.Row_Editor.Height = new GridLength(1.0, GridUnitType.Star);
					this.Row_Renderer.MinHeight = 0.0;
					this.Row_Renderer.Height = new GridLength(0.0, GridUnitType.Star);
					this.Splitter_H.Visibility = Visibility.Collapsed;
				}
				this.Editor.Margin = new Thickness(0.0);
				return;
			}
			if (isVerticalLayout)
			{
				this.Column_Editor.MinWidth = 100.0;
				this.Column_Editor.Width = new GridLength(5.0, GridUnitType.Star);
				this.Column_Renderer.MinWidth = 100.0;
				this.Column_Renderer.Width = new GridLength(5.0, GridUnitType.Star);
				this.Splitter_V.Visibility = Visibility.Visible;
				this.Editor.Margin = new Thickness(0.0, 0.0, 7.0, 0.0);
				return;
			}
			this.Row_Editor.MinHeight = 100.0;
			this.Row_Editor.Height = new GridLength(5.0, GridUnitType.Star);
			this.Row_Renderer.MinHeight = 100.0;
			this.Row_Renderer.Height = new GridLength(5.0, GridUnitType.Star);
			this.Splitter_H.Visibility = Visibility.Visible;
			this.Editor.Margin = new Thickness(0.0, 0.0, 0.0, 7.0);
		}
		internal void SetLayout(bool isVerticalLayout)
		{
			if (isVerticalLayout)
			{
				Grid.SetColumn(this.Grid_Renderer, 1);
				Grid.SetRow(this.Grid_Renderer, 0);
				this.Row_Renderer.MinHeight = 0.0;
				this.Row_Renderer.Height = new GridLength(0.0);
				this.Splitter_H.Visibility = Visibility.Collapsed;
			}
			else
			{
				Grid.SetColumn(this.Grid_Renderer, 0);
				Grid.SetRow(this.Grid_Renderer, 1);
				this.Column_Renderer.MinWidth = 0.0;
				this.Column_Renderer.Width = new GridLength(0.0);
				this.Splitter_V.Visibility = Visibility.Collapsed;
			}
			this.ToggleLivePreview(this._settings.App_LivePreviewEnabled, isVerticalLayout);
		}
		public void PrintHtml()
		{
			WebView view = WebCore.CreateWebView(1024, 768);
			string html = this.HtmlTemplate.GenerateHtmlTemplate(false);
			view.LoadHTML(html);
			view.LoadCompleted += delegate(object param0, System.EventArgs param1)
			{
				view.Print();
			};
		}
		public void ExportPdf()
		{
			string text = this.Filename;
			if (string.IsNullOrEmpty(text))
			{
				text = this.Title;
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = this.SavePdfFileTypes,
				FileName = System.IO.Path.GetFileNameWithoutExtension(text),
				RestoreDirectory = true
			};
			bool? flag = saveFileDialog.ShowDialog();
			string fileName = saveFileDialog.FileName;
			if (!flag.Value || string.IsNullOrEmpty(fileName))
			{
				return;
			}
			EditorRenderer._logger.Trace("Exporting PDF document: " + fileName);
			HtmlTemplate htmlTemplate = this.HtmlTemplate;
			htmlTemplate.UseBaseRelativeImageWorkaround = true;
			string content = htmlTemplate.GenerateHtmlTemplate(false);
			PdfExporter pdfExporter = new PdfExporter(fileName, content)
			{
				IncludeCssBackground = this._settings.IO_Pdf_IncludeBackground,
				EnableOutlineGeneration = this._settings.IO_Pdf_EnableOutlineGeneration
			};
			pdfExporter.ExportPdf();
		}
		public void PrintMarkdown()
		{
			this.Editor.PrintMarkdown();
		}
		private void Editor_TextChanged(object sender, System.EventArgs e)
		{
			this.KickTick();
		}
		public void KickTick()
		{
			if (this._settings.App_LivePreviewEnabled && this.RenderTimer != null)
			{
				this.RenderTimer.Stop();
				this.RenderTimer.Start();
			}
		}
		public void ConnectScrollbarListener()
		{
			if (this.EditorScrollViewer != null)
			{
				EditorRenderer._logger.Trace("ScrollViewer already connected, not going to reconnect");
				return;
			}
			this.EditorScrollViewer = this.Editor.FindVisualChild<ScrollViewer>();
			if (this.EditorScrollViewer != null)
			{
				EditorRenderer._logger.Trace("ScrollViewer listener not connected, connecting now");
				this.EditorScrollViewer.ScrollChanged += new ScrollChangedEventHandler(this.EditorScrollViewer_ScrollChanged);
			}
		}
		public void Render(bool doBlockingRender)
		{
			if (!this._settings.App_LivePreviewEnabled)
			{
				return;
			}
			if (!this.Renderer.IsLoaded)
			{
				EditorRenderer._logger.Warn("Tried to render but Renderer wasn't loaded");
				return;
			}
			if (!this.Editor.MarkdownProcessor.BackgroundRenderingAllowed || doBlockingRender)
			{
				string text = this.Editor.MarkdownText;
				string text2 = this.LoadSupplementalMarkdownFromFile();
				if (!string.IsNullOrEmpty(text2))
				{
					text = text + StringUtilities.GetNewLines(2) + text2;
				}
				string content = this.Editor.MarkdownProcessor.ConvertMarkdownToHTML(text);
				this.InsertHtmlIntoRenderer(content);
				return;
			}
			BackgroundWorker backgroundWorker = new BackgroundWorker
			{
				WorkerSupportsCancellation = true
			};
			backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs args)
			{
				if (args.Argument == null)
				{
					args.Cancel = true;
					return;
				}
				string text3 = args.Argument.ToString();
				string text4 = this.LoadSupplementalMarkdownFromFile();
				if (!string.IsNullOrEmpty(text4))
				{
					text3 = text3 + StringUtilities.GetNewLines(2) + text4;
				}
				string result = this.Editor.MarkdownProcessor.ConvertMarkdownToHTML(text3);
				args.Result = result;
			};
			backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
			{
				if (args.Error != null)
				{
					EditorRenderer._logger.ErrorException("Error processing Markdown - received in RunWorkerCompleted", args.Error);
					MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_ProcessingMarkdownMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_ProcessingMarkdownTitle", false, "MarkdownPadStrings"), args.Error, "");
					return;
				}
				string content2 = args.Result.ToString();
				this.InsertHtmlIntoRenderer(content2);
			};
			string markdownText = this.Editor.MarkdownText;
			backgroundWorker.RunWorkerAsync(markdownText);
		}
		private void InsertHtmlIntoRenderer(string content)
		{
			JSValue jSValue = new JSValue(content);
			try
			{
				this.Renderer.CallJavascriptFunction("", "loadMarkdown", new JSValue[]
				{
					jSValue
				});
				if (!this._settings.Markdown_EnableHighPerformanceRendering)
				{
					string javascript = string.Format("window.scrollTo(0, {0} * (document.body.scrollHeight - document.body.clientHeight));", this.ScrollViewerPositionPercentage.ToString(System.Globalization.CultureInfo.InvariantCulture));
					this.Renderer.ExecuteJavascript(javascript);
				}
			}
			catch (System.InvalidOperationException exception)
			{
				EditorRenderer._logger.ErrorException("Awesomium crash - likely Windows 8? OS: " + System.Environment.OSVersion, exception);
				MessageBoxResult messageBoxResult = CustomMessageBox.ShowYesNoCancel(LocalizationProvider.GetLocalizedString("Error_RendererWindows8Message", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_RendererWindows8Title", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Yes", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("No", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Button_ReportBug", false, "MarkdownPadStrings"));
				MessageBoxResult messageBoxResult2 = messageBoxResult;
				if (messageBoxResult2 != MessageBoxResult.Cancel)
				{
					if (messageBoxResult2 == MessageBoxResult.Yes)
					{
						Urls.MarkdownPad_Windows8RendererBug.TryStartDefaultProcess();
					}
				}
				else
				{
					BugReportHelper.ShowBugReport(exception, "");
				}
			}
		}
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
		public void Dispose()
		{
			EditorRenderer._logger.Trace("Disposing EditorRenderer control");
			if (this.Editor != null)
			{
				this.Editor.Dispose();
			}
			if (this.Renderer != null)
			{
				this.Renderer.Close();
			}
		}
		private void Renderer_OnShowContextMenu(object sender, Awesomium.Core.ContextMenuEventArgs e)
		{
		}
		public void CopyLivePreviewContent()
		{
			this.Renderer.SelectAll();
			this.Renderer.Copy();
			this.Renderer.StopFind(true);
		}
		private void CommandBinding_OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			this._settings.App_LivePreviewEnabled = !this._settings.App_LivePreviewEnabled;
			this.ToggleLivePreview(this._settings.App_LivePreviewEnabled, this._settings.App_VerticalLayoutEnabled);
			e.Handled = true;
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/usercontrols/editorrenderer.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		internal System.Delegate _CreateDelegate(System.Type delegateType, string handler)
		{
			return System.Delegate.CreateDelegate(delegateType, this, handler);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.Grid_Main = (Grid)target;
				return;
			case 2:
				this.Column_Editor = (ColumnDefinition)target;
				return;
			case 3:
				this.Column_Renderer = (ColumnDefinition)target;
				return;
			case 4:
				this.Row_Editor = (RowDefinition)target;
				return;
			case 5:
				this.Row_Renderer = (RowDefinition)target;
				return;
			case 6:
				this.Grid_Editor = (Grid)target;
				return;
			case 7:
				this.Editor = (MarkdownEditor)target;
				return;
			case 8:
				this.Splitter_V = (GridSplitter)target;
				return;
			case 9:
				this.Splitter_H = (GridSplitter)target;
				return;
			case 10:
				this.Grid_Renderer = (Grid)target;
				return;
			case 11:
				this.Renderer = (WebControl)target;
				this.Renderer.ShowContextMenu += new ShowContextMenuEventHandler(this.Renderer_OnShowContextMenu);
				return;
			case 12:
				((CommandBinding)target).PreviewExecuted += new ExecutedRoutedEventHandler(this.CommandBinding_OnPreviewExecuted);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OnPreviewExecuted);
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}
	}
}
