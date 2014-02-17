using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using MarkdownPad2.Core;
using MarkdownPad2.EditorResources;
using MarkdownPad2.i18n;
using MarkdownPad2.i18n.Search;
using MarkdownPad2.Markdown;
using MarkdownPad2.Properties;
using MarkdownPad2.SpellCheck;
using MarkdownPad2.SyntaxRules;
using MarkdownPad2.Utilities;
using NLog;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
namespace MarkdownPad2.UserControls
{
	public class MarkdownEditor : UserControl, System.IDisposable, INotifyPropertyChanged, System.Windows.Markup.IComponentConnector
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		private Settings _settings = Settings.Default;
		private SearchPanel _searchPanel;
		private System.Windows.Threading.DispatcherTimer AutoSaveTimer;
		private string _statusMessage;
		public SpellCheckProvider SpellCheckProvider;
		private SpellingService SpellingService;
		private readonly Regex wordSeparatorRegex = new Regex("-[^\\w]+|^'[^\\w]+|[^\\w]+'[^\\w]+|[^\\w]+-[^\\w]+|[^\\w]+'$|[^\\w]+-$|^-$|^'$|[^\\w'-]", RegexOptions.Compiled);
		private readonly Regex wordCountRegex = new Regex("[\\S]+", RegexOptions.Compiled);
		private int wordCount;
		internal TextEditor Editor;
		private bool _contentLoaded;
		public event System.EventHandler TextChanged;
		public event PropertyChangedEventHandler PropertyChanged;
		public IMarkdownProcessor MarkdownProcessor
		{
			get;
			set;
		}
		public string Filename
		{
			get;
			private set;
		}
		public System.DateTime FileLastModified
		{
			get;
			set;
		}
		public string StatusMessage
		{
			get
			{
				return this._statusMessage;
			}
			set
			{
				this._statusMessage = value;
				this.OnPropertyChanged("StatusMessage");
			}
		}
		private bool SpellCheckKeyActivated
		{
			get;
			set;
		}
		public int WordCount
		{
			get
			{
				return this.wordCount;
			}
			private set
			{
				this.wordCount = value;
				this.OnPropertyChanged("WordCount");
			}
		}
		public int CharacterCount
		{
			get
			{
				return this.Editor.Text.Length;
			}
		}
		public int SelectedWordCount
		{
			get
			{
				return this.wordCountRegex.Matches(this.Editor.SelectedText).Count;
			}
		}
		public int SelectedCharacterCount
		{
			get
			{
				return this.Editor.SelectionLength;
			}
		}
		public string OriginalDocument
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
		private bool IsEditorLoaded
		{
			get;
			set;
		}
		public TextEditor TextEditor
		{
			get
			{
				return this.Editor;
			}
		}
		public string Text
		{
			get
			{
				return this.Editor.Text;
			}
			set
			{
				this.Editor.Text = value;
			}
		}
		public TextArea TextArea
		{
			get
			{
				return this.Editor.TextArea;
			}
		}
		public TextView TextView
		{
			get
			{
				return this.Editor.TextArea.TextView;
			}
		}
		public TextDocument TextDocument
		{
			get
			{
				return this.Editor.Document;
			}
		}
		public string MarkdownText
		{
			get
			{
				string result = string.Empty;
				if (this._settings.Markdown_EnableHighPerformanceRendering)
				{
					string arg_1B_0 = string.Empty;
					TextView textView = this.TextView;
					TextViewPosition? position = textView.GetPosition(new System.Windows.Point(0.0, 0.0) + textView.ScrollOffset);
					TextViewPosition? position2 = textView.GetPosition(new System.Windows.Point(textView.ActualWidth, textView.ActualHeight) + textView.ScrollOffset);
					TextDocument textDocument = this.TextDocument;
					int num = position.HasValue ? textDocument.GetOffset(position.Value.Location) : textDocument.TextLength;
					int num2 = position2.HasValue ? textDocument.GetOffset(position2.Value.Location) : textDocument.TextLength;
					result = textDocument.GetText(num, System.Math.Abs(num2 - num));
				}
				else
				{
					result = this.Editor.Text;
				}
				return result;
			}
		}
		public int SelectionStart
		{
			get
			{
				return this.Editor.SelectionStart;
			}
			set
			{
				this.Editor.SelectionStart = value;
			}
		}
		public int SelectionLength
		{
			get
			{
				return this.Editor.SelectionLength;
			}
			set
			{
				this.Editor.SelectionLength = value;
			}
		}
		public string SelectedText
		{
			get
			{
				return this.Editor.SelectedText;
			}
			set
			{
				this.Editor.SelectedText = value;
			}
		}
		public DocumentLine CurrentLine
		{
			get
			{
				return this.Editor.Document.GetLineByOffset(this.Editor.CaretOffset);
			}
		}
		public bool IsCaretOnLastLine
		{
			get
			{
				return this.CurrentLine.LineNumber == this.TotalNumberOfLines;
			}
		}
		public bool IsCaretAtLastOffset
		{
			get
			{
				return this.Editor.CaretOffset == this.Editor.Document.TextLength;
			}
		}
		public int TotalNumberOfLines
		{
			get
			{
				return this.Editor.Document.LineCount;
			}
		}
		private ContextMenu EditorContextMenu
		{
			get
			{
				ContextMenu contextMenu = new ContextMenu();
				MenuItem menuItem = new MenuItem();
				menuItem.Command = MarkdownPadCommands.Undo;
				contextMenu.Items.Add(menuItem);
				MenuItem menuItem2 = new MenuItem();
				menuItem2.Command = MarkdownPadCommands.Redo;
				contextMenu.Items.Add(menuItem2);
				Separator newItem = new Separator();
				contextMenu.Items.Add(newItem);
				MenuItem menuItem3 = new MenuItem();
				menuItem3.Command = MarkdownPadCommands.Cut;
				contextMenu.Items.Add(menuItem3);
				MenuItem menuItem4 = new MenuItem();
				menuItem4.Command = MarkdownPadCommands.Copy;
				contextMenu.Items.Add(menuItem4);
				MenuItem menuItem5 = new MenuItem();
				menuItem5.Command = MarkdownPadCommands.CopyHTML;
				contextMenu.Items.Add(menuItem5);
				MenuItem menuItem6 = new MenuItem();
				menuItem6.Command = MarkdownPadCommands.Paste;
				contextMenu.Items.Add(menuItem6);
				MenuItem menuItem7 = new MenuItem();
				menuItem7.Command = MarkdownPadCommands.Delete;
				contextMenu.Items.Add(menuItem7);
				Separator newItem2 = new Separator();
				contextMenu.Items.Add(newItem2);
				MenuItem menuItem8 = new MenuItem();
				menuItem8.Command = MarkdownPadCommands.SelectAll;
				contextMenu.Items.Add(menuItem8);
				return contextMenu;
			}
		}
		public TextViewPosition? GetPositionFromPoint(System.Windows.Point point)
		{
			return this.Editor.GetPositionFromPoint(point);
		}
		public MarkdownEditor()
		{
			this.InitializeComponent();
			System.Collections.Generic.ICollection<CommandBinding> commandBindings = this.Editor.TextArea.DefaultInputHandler.Editing.CommandBindings;
			CommandBinding item = commandBindings.Single((CommandBinding cb) => cb.Command == AvalonEditCommands.IndentSelection);
			commandBindings.Remove(item);
			this.OriginalDocument = string.Empty;
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}
			this.LoadEditorSettings();
			this.InitializeCommands();
			this.Editor.Loaded += new RoutedEventHandler(this.Editor_Loaded);
			this.Editor.TextArea.SelectionChanged += new System.EventHandler(this.TextArea_SelectionChanged);
		}
		private void TextArea_SelectionChanged(object sender, System.EventArgs e)
		{
			this.OnPropertyChanged("SelectedWordCount");
			this.OnPropertyChanged("SelectedCharacterCount");
		}
		public FileModificationType CheckIfFileModifiedOrDeletedOnDisk()
		{
			if (string.IsNullOrEmpty(this.Filename))
			{
				return FileModificationType.None;
			}
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(this.Filename);
			if (!fileInfo.Exists)
			{
				return FileModificationType.Deleted;
			}
			System.TimeSpan timeSpan = fileInfo.LastWriteTime - this.FileLastModified;
			MarkdownEditor._logger.Trace("File modification time gap: " + timeSpan);
			if (timeSpan > System.TimeSpan.FromSeconds(1.0))
			{
				return FileModificationType.Modified;
			}
			return FileModificationType.None;
		}
		public void ClearOriginalDocument()
		{
			this.OriginalDocument = string.Empty;
			this.OnPropertyChanged("HasChanges");
		}
		public void ClearPendingChanges()
		{
			this.OriginalDocument = this.Editor.Text;
			this.OnPropertyChanged("HasChanges");
		}
		public void SetColumnGuide(bool enable, int position)
		{
			this.Editor.Options.ShowColumnRuler = enable;
			this.Editor.Options.ColumnRulerPosition = position;
		}
		public void SetMarkdownProcessor(MarkdownProcessorType processor)
		{
			MarkdownEditor._logger.Debug("Setting markdown processor to: " + processor);
			if (!MarkdownProcessorProvider.MarkdownProcessorMap.ContainsKey(processor))
			{
				throw new System.Exception("Unable to find the Markdown processor: " + processor);
			}
			this.MarkdownProcessor = MarkdownProcessorProvider.MarkdownProcessorMap[processor];
		}
		private void InitializeCommands()
		{
			CommandBinding commandBinding = new CommandBinding(MarkdownPadCommands.CopyHTML, new ExecutedRoutedEventHandler(this.CommandExecuted_CopyHtml), new CanExecuteRoutedEventHandler(this.CommandBinding_CanExecute_CommandsThatRequireSelectedText));
			base.CommandBindings.Add(commandBinding);
		}
		private void CommandExecuted_CopyHtml(object sender, ExecutedRoutedEventArgs e)
		{
			if (this.SelectionLength == 0)
			{
				return;
			}
			string textToCopy = this.MarkdownProcessor.ConvertMarkdownToHTML(this.SelectedText);
			SystemUtilities.CopyStringToClipboard(textToCopy);
		}
		private void CommandBinding_CanExecute_EditingCommandsThatRequireText(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (this.Editor.Text.Length > 0);
		}
		private void CommandBinding_CanExecute_CommandsThatRequireSelectedText(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (this.SelectionLength > 0);
		}
		private void TextView_VisualLinesChanged(object sender, System.EventArgs e)
		{
			if (this.SpellCheckProvider == null)
			{
				return;
			}
			if (this.IsCaretAtLastOffset && !this.SpellCheckKeyActivated)
			{
				return;
			}
			this.SpellCheckProvider.DoSpellCheck();
		}
		private void Editor_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			this.Editor.ContextMenu = this.EditorContextMenu;
			if (this.SpellCheckProvider == null)
			{
				return;
			}
			TextViewPosition? positionFromPoint = this.Editor.GetPositionFromPoint(Mouse.GetPosition(this.Editor));
			if (!positionFromPoint.HasValue)
			{
				return;
			}
			int offset = this.Editor.Document.GetOffset(positionFromPoint.Value.Line, positionFromPoint.Value.Column);
			System.Collections.Generic.IEnumerable<TextSegment> spellCheckErrors = this.SpellCheckProvider.GetSpellCheckErrors();
			TextSegment textSegment = spellCheckErrors.FirstOrDefault((TextSegment segment) => segment.StartOffset <= offset && segment.EndOffset >= offset);
			if (textSegment == null)
			{
				return;
			}
			DocumentLine lineByOffset = this.Editor.Document.GetLineByOffset(offset);
			if (offset == lineByOffset.Offset || offset == lineByOffset.EndOffset)
			{
				return;
			}
			System.Collections.Generic.IEnumerable<string> spellcheckSuggestions = this.SpellCheckProvider.GetSpellcheckSuggestions(this.Editor.Document.GetText(textSegment));
			int num = 0;
			if (spellcheckSuggestions.Any<string>())
			{
				foreach (string current in spellcheckSuggestions)
				{
					MenuItem menuItem = new MenuItem
					{
						Header = current,
						FontWeight = FontWeights.Bold,
						Tag = textSegment
					};
					menuItem.Click += new RoutedEventHandler(this.SuggestionItem_Click);
					this.Editor.ContextMenu.Items.Insert(num, menuItem);
					num++;
				}
			}
			MenuItem menuItem2 = new MenuItem
			{
				Header = LocalizationProvider.GetLocalizedString("MenuItem_AddToDictionary", false, "MarkdownPadStrings"),
				Tag = textSegment
			};
			menuItem2.Click += delegate(object o, RoutedEventArgs args)
			{
				MenuItem menuItem3 = args.OriginalSource as MenuItem;
				TextSegment segment = (TextSegment)menuItem3.Tag;
				string text = this.Editor.Document.GetText(segment);
				this.SpellingService.AddWordToCustomDictionary(text);
				this.SpellCheckProvider.DoSpellCheck();
			};
			this.Editor.ContextMenu.Items.Insert(num, menuItem2);
			num++;
			Separator insertItem = new Separator();
			this.Editor.ContextMenu.Items.Insert(num, insertItem);
		}
		private void SuggestionItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = e.OriginalSource as MenuItem;
			TextSegment segment = (TextSegment)menuItem.Tag;
			string text = (string)menuItem.Header;
			this.Editor.Document.Replace(segment, text);
			this.SpellCheckProvider.DoSpellCheck();
		}
		private void Editor_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.IsEditorLoaded)
			{
				return;
			}
			this.IsEditorLoaded = true;
			MarkdownEditor._logger.Debug("MarkdownEditor loaded.");
			this.Editor.TextChanged += new System.EventHandler(this.Editor_TextChanged);
			this.Editor.TextArea.TextEntered += new TextCompositionEventHandler(this.TextArea_TextEntered);
			this.Editor.ContextMenuOpening += new ContextMenuEventHandler(this.Editor_ContextMenuOpening);
			this.Editor.TextArea.TextView.VisualLinesChanged += new System.EventHandler(this.TextView_VisualLinesChanged);
			this.Editor.PreviewKeyDown += new KeyEventHandler(this.Editor_PreviewKeyDown);
			this.Editor.ContextMenu = this.EditorContextMenu;
			this.Editor.Focus();
		}
		private void Editor_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (this._settings.Editor_AutoContinueLists)
			{
				AutoContinueLists autoContinueLists = new AutoContinueLists();
				autoContinueLists.Handle(new EditorPreviewKeyDownEvent(this.Editor, e));
			}
		}
		public void LoadEditorSettings()
		{
			this.Editor.TextArea.IndentationStrategy = null;
			this.ToggleMarkdownSyntaxHighlighting(this._settings.Editor_MarkdownSyntaxHighlightingEnabled);
			this.ToggleLineNumbers(this._settings.Editor_LineNumbersEnabled);
			this.SetWordWrap(this._settings.Editor_WordWrap);
			if (Fonts.SystemFontFamilies.Contains(this._settings.Editor_FontFamily))
			{
				this.Editor.FontFamily = this._settings.Editor_FontFamily;
			}
			else
			{
				if (Fonts.SystemFontFamilies.Contains(new FontFamily("Courier New")))
				{
					this.Editor.FontFamily = new FontFamily("Courier New");
				}
			}
			this.Editor.FontSize = this._settings.Editor_FontSize;
			this.Editor.FontWeight = this._settings.Editor_FontWeight;
			this.Editor.Foreground = this._settings.Editor_ForegroundColor.ToBrush();
			this.Editor.Background = this._settings.Editor_BackgroundColor.ToBrush();
			this.Editor.TextArea.TextView.LinkTextForegroundBrush = this._settings.Editor_HyperlinkForegroundColor.ToBrush();
			MarkdownEditor._logger.Trace("Setting spell check dictionary language: " + this._settings.Editor_SpellCheckLanguage);
			this.SpellingService = new SpellingService();
			this.SpellingService.SetLanguage(this._settings.Editor_SpellCheckLanguage);
			this.ToggleSpellCheck(this._settings.Editor_SpellCheckEnabled);
			this.Editor.Options = new TextEditorOptions
			{
				AllowScrollBelowDocument = true,
				CutCopyWholeLine = false,
				EnableEmailHyperlinks = this._settings.Editor_EnableHyperlinks,
				EnableHyperlinks = this._settings.Editor_EnableHyperlinks,
				EnableImeSupport = true,
				ConvertTabsToSpaces = this._settings.Editor_UseSpacesAsTabs,
				ShowColumnRuler = this._settings.Editor_ShowColumnGuide,
				ColumnRulerPosition = this._settings.CodeEditor_ColumnGuidePosition,
				ShowEndOfLine = this._settings.Editor_DisplayFormattingMarks,
				ShowSpaces = this._settings.Editor_DisplayFormattingMarks,
				ShowTabs = this._settings.Editor_DisplayFormattingMarks
			};
			this.SetMarkdownProcessor(this._settings.Markdown_MarkdownProcessor);
			this.ToggleAutoSave(this._settings.IO_AutoSaveEnabled);
		}
		public void LoadDocument(string filename)
		{
			if (!System.IO.File.Exists(filename))
			{
				return;
			}
			this.Filename = filename;
			this.FileLastModified = System.DateTime.Now;
			try
			{
				this.Editor.Load(filename);
				this.OriginalDocument = this.Editor.Text;
				if (this._settings.IO_ScrollToEndOfDocumentOnLoad)
				{
					this.ScrollToEndOfDocument();
				}
				this.UpdateWordCount();
			}
			catch (System.Exception exception)
			{
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("ErrorWhileOpeningFileMessage", true, "MarkdownPadStrings") + filename, LocalizationProvider.GetLocalizedString("ErrorWhileOpeningFileTitle", false, "MarkdownPadStrings"), exception, "");
			}
		}
		public void ReloadCurrentDocument()
		{
			if (string.IsNullOrEmpty(this.Filename))
			{
				return;
			}
			this.LoadDocument(this.Filename);
			this.OnPropertyChanged("HasChanges");
		}
		public void ResetFileLastModifiedTime()
		{
			this.FileLastModified = System.DateTime.Now;
		}
		public void ScrollToEndOfDocument()
		{
			this.Editor.ScrollToEnd();
			this.Editor.CaretOffset = this.Editor.Document.TextLength;
		}
		public void SaveDocument(string fileName)
		{
			System.Text.UTF8Encoding encoding = this._settings.IO_ExportTextWithBom ? new System.Text.UTF8Encoding(true) : new System.Text.UTF8Encoding(false);
			using (System.IO.TextWriter textWriter = new System.IO.StreamWriter(fileName, false, encoding))
			{
				string text = this.Editor.Text;
				if (this._settings.IO_UseUnixStyleEol)
				{
					textWriter.NewLine = "\n";
					text = text.ConvertTextToUnixEol();
				}
				else
				{
					textWriter.NewLine = "\r\n";
				}
				try
				{
					textWriter.Write(text);
					this.Filename = fileName;
					this.OnPropertyChanged("Filename");
					this.FileLastModified = System.DateTime.Now;
					MarkdownEditor._logger.Trace("Saved file: " + fileName);
					this.OriginalDocument = this.Editor.Text;
					this.OnPropertyChanged("HasChanges");
					this.StatusMessage = string.Format(LocalizationProvider.GetLocalizedString("FileSavedNotification", false, "MarkdownPadStrings"), System.IO.Path.GetFileName(fileName), System.DateTime.Now);
				}
				catch (System.Exception exception)
				{
					MarkdownEditor._logger.ErrorException("Exception while saving text", exception);
					throw;
				}
			}
		}
		public void Find()
		{
			if (this._searchPanel == null || this._searchPanel.IsClosed)
			{
				this._searchPanel = new SearchPanel
				{
					Localization = new SearchLocalization()
				};
				this._searchPanel.Attach(this.TextArea);
			}
			this._searchPanel.Open();
			if (!this.TextArea.Selection.IsEmpty && !this.TextArea.Selection.IsMultiline)
			{
				this._searchPanel.SearchPattern = this.TextArea.Selection.GetText();
			}
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
		public void GoToLine(int lineNumber)
		{
			if (lineNumber < 1)
			{
				return;
			}
			DocumentLine lineByNumber = this.Editor.Document.GetLineByNumber(lineNumber);
			this.Editor.ScrollToLine(lineNumber);
			this.Editor.CaretOffset = lineByNumber.Offset;
		}
		public void InsertUnorderedListItem()
		{
			string markdownSyntaxToApply = MarkdownSyntaxProvider.UnorderedListSyntaxMap[this._settings.Markdown_UnorderedListStyle];
			this.ToggleAsymmetricMarkdownFormatting(markdownSyntaxToApply);
		}
		public void InsertOrderedListItem()
		{
			this.ToggleAsymmetricMarkdownFormatting("1. ");
		}
		public void ToggleBold()
		{
			this.ToggleSymmetricalMarkdownFormatting(MarkdownSyntax.Bold);
		}
		public void ToggleItalic()
		{
			this.ToggleSymmetricalMarkdownFormatting(MarkdownSyntax.Italic);
		}
		public void ToggleCode()
		{
			bool flag = this.Editor.SelectedText.Contains(System.Environment.NewLine);
			if (this.Editor.SelectedText == this.Editor.Document.GetText(this.CurrentLine) || flag)
			{
				this.ToggleAsymmetricMarkdownFormatting(MarkdownSyntax.CodeBlock);
				return;
			}
			this.ToggleSymmetricalMarkdownFormatting(MarkdownSyntax.CodeInline);
		}
		public void ToggleQuotes()
		{
			this.ToggleAsymmetricMarkdownFormatting(MarkdownSyntax.Quote);
		}
		public void ConvertToUppercase()
		{
			if (this.SelectionLength <= 0)
			{
				return;
			}
			this.SelectedText = this.SelectedText.ToUpper();
		}
		public void ConvertToLowercase()
		{
			if (this.SelectionLength <= 0)
			{
				return;
			}
			this.SelectedText = this.SelectedText.ToLower();
		}
		public void InsertTimestamp(string format)
		{
			string text;
			try
			{
				text = System.DateTime.Now.ToString(format);
			}
			catch (System.Exception exception)
			{
				MarkdownEditor._logger.ErrorException("Exception while inserting timestamp", exception);
				string localizedString = LocalizationProvider.GetLocalizedString("Error_InsertTimestampMessage", false, "MarkdownPadStrings");
				string localizedString2 = LocalizationProvider.GetLocalizedString("Error_InsertTimestampTitle", false, "MarkdownPadStrings");
				MessageBoxHelper.ShowErrorMessageBox(localizedString, localizedString2, exception, "");
				return;
			}
			text += " ";
			this.SelectedText = text;
			this.SelectionLength = 0;
			this.SelectionStart += text.Length;
		}
		public void ToggleHeading(HeadingType headingType, bool useUnderlineStyleHeadings)
		{
			string text = string.Empty;
			switch (headingType)
			{
			case HeadingType.Heading1:
				text = ((useUnderlineStyleHeadings && this.SelectionLength > 0) ? MarkdownSyntax.Heading1Alternative : MarkdownSyntax.Heading1);
				break;
			case HeadingType.Heading2:
				text = ((useUnderlineStyleHeadings && this.SelectionLength > 0) ? MarkdownSyntax.Heading2Alternative : MarkdownSyntax.Heading2);
				break;
			case HeadingType.Heading3:
				text = MarkdownSyntax.Heading3;
				break;
			case HeadingType.Heading4:
				text = MarkdownSyntax.Heading4;
				break;
			case HeadingType.Heading5:
				text = MarkdownSyntax.Heading5;
				break;
			case HeadingType.Heading6:
				text = MarkdownSyntax.Heading6;
				break;
			}
			if (useUnderlineStyleHeadings && this.SelectionLength > 0 && (headingType == HeadingType.Heading1 || headingType == HeadingType.Heading2))
			{
				this.UnderlineSelection(text);
				return;
			}
			this.ToggleSymmetricalMarkdownFormatting(text);
		}
		private void UnderlineSelection(string characterToUseForUnderline)
		{
			int selectionLength = this.SelectionLength;
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			for (int i = 0; i < this.SelectionLength; i++)
			{
				stringBuilder.Append(characterToUseForUnderline);
			}
			this.SelectedText = this.SelectedText + System.Environment.NewLine + stringBuilder.ToString();
			this.SelectionLength = selectionLength;
		}
		public void ToggleLineNumbers(bool enable)
		{
			this.Editor.ShowLineNumbers = enable;
		}
		public void SetWordWrap(bool enable)
		{
			this.Editor.WordWrap = enable;
		}
		public void ToggleAutoSave(bool enable)
		{
			if (enable)
			{
				MarkdownEditor._logger.Trace("Setting auto save timer for minutes: " + this._settings.IO_AutoSaveInterval);
				if (this.AutoSaveTimer != null)
				{
					this.AutoSaveTimer.Stop();
					this.AutoSaveTimer.Tick -= new System.EventHandler(this.AutoSaveTimer_Tick);
				}
				this.AutoSaveTimer = new System.Windows.Threading.DispatcherTimer
				{
					Interval = new System.TimeSpan(0, 0, this._settings.IO_AutoSaveInterval, 0)
				};
				this.AutoSaveTimer.Tick += new System.EventHandler(this.AutoSaveTimer_Tick);
				this.AutoSaveTimer.Start();
				return;
			}
			if (this.AutoSaveTimer != null)
			{
				this.AutoSaveTimer.Stop();
				this.AutoSaveTimer.Tick -= new System.EventHandler(this.AutoSaveTimer_Tick);
				this.AutoSaveTimer = null;
			}
		}
		private void AutoSaveTimer_Tick(object sender, System.EventArgs e)
		{
			MarkdownEditor._logger.Trace("Auto save timer TICK!");
			if (!string.IsNullOrEmpty(this.Filename) && this.HasChanges)
			{
				MarkdownEditor._logger.Trace("Auto save initiating");
				this.SaveDocument(this.Filename);
			}
		}
		public void ToggleSpellCheck(bool enable)
		{
			if (this.SpellCheckProvider != null)
			{
				this.SpellCheckProvider.ClearSpellCheckErrorsAndInvalidateLayer();
			}
			if (enable)
			{
				this.SpellCheckProvider = new SpellCheckProvider(this.SpellingService);
				this.SpellCheckProvider.Connect(this.Editor.TextArea.TextView);
				this.SpellCheckProvider.DoSpellCheck();
				return;
			}
			if (this.SpellCheckProvider != null)
			{
				this.SpellCheckProvider.Disconnect();
				this.SpellCheckProvider = null;
			}
		}
		public void PrintMarkdown()
		{
			PrintDialog printDialog = new PrintDialog();
			if (printDialog.ShowDialog() != true)
			{
				return;
			}
			FlowDocument flowDocument = this.CreateFlowDocument(this.Editor);
			flowDocument.PageHeight = printDialog.PrintableAreaHeight;
			flowDocument.PageWidth = printDialog.PrintableAreaWidth;
			flowDocument.PagePadding = new Thickness(72.0);
			flowDocument.ColumnGap = 0.0;
			flowDocument.ColumnWidth = flowDocument.PageWidth - flowDocument.ColumnGap - flowDocument.PagePadding.Left - flowDocument.PagePadding.Right;
			IDocumentPaginatorSource documentPaginatorSource = flowDocument;
			try
			{
				printDialog.PrintDocument(documentPaginatorSource.DocumentPaginator, "MarkdownPad Document");
			}
			catch (System.Exception exception)
			{
				MarkdownEditor._logger.ErrorException("Exception while attempting to print Markdown plain text", exception);
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_PrintMarkdownMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_PrintMarkdownTitle", false, "MarkdownPadStrings"), exception, "");
			}
		}
		private FlowDocument CreateFlowDocument(TextEditor editor)
		{
			IHighlighter highlighter = editor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;
			return new FlowDocument(this.ConvertTextDocumentToBlock(editor.Document, highlighter))
			{
				FontFamily = editor.FontFamily,
				FontSize = editor.FontSize
			};
		}
		private Block ConvertTextDocumentToBlock(TextDocument document, IHighlighter highlighter)
		{
			if (document == null)
			{
				throw new System.ArgumentNullException("document");
			}
			Paragraph paragraph = new Paragraph();
			foreach (DocumentLine current in document.Lines)
			{
				int lineNumber = current.LineNumber;
				HighlightedInlineBuilder highlightedInlineBuilder = new HighlightedInlineBuilder(document.GetText(current));
				if (highlighter != null)
				{
					HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
					int offset = current.Offset;
					foreach (HighlightedSection current2 in highlightedLine.Sections)
					{
						highlightedInlineBuilder.SetHighlighting(current2.Offset - offset, current2.Length, current2.Color);
					}
				}
				paragraph.Inlines.AddRange(highlightedInlineBuilder.CreateRuns());
				paragraph.Inlines.Add(new LineBreak());
			}
			return paragraph;
		}
		public void ToggleMarkdownSyntaxHighlighting(bool enable)
		{
			if (!enable)
			{
				this.Editor.SyntaxHighlighting = null;
				return;
			}
			SyntaxRuleCollection defaultMarkownSyntaxRules = SyntaxRuleProvider.DefaultMarkownSyntaxRules;
			try
			{
				SyntaxRuleSerializer syntaxRuleSerializer = new SyntaxRuleSerializer();
				XmlReader reader = syntaxRuleSerializer.SerializeToXml(defaultMarkownSyntaxRules);
				this.Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
			}
			catch (System.Exception exception)
			{
				this.Editor.SyntaxHighlighting = null;
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_LoadingSyntaxHighlighterMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_LoadingSyntaxHighlighterTitle", false, "MarkdownPadStrings"), exception, "");
			}
		}
		public void InsertHyperlink(string hyperlink, string title, bool insertAsFootnote)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(title))
			{
				text = " \"" + title + "\"";
			}
			if (this.SelectionLength == 0)
			{
				this.SelectedText = string.Format("[{0}]({0}{1})", hyperlink, text);
				return;
			}
			this.SelectedText = string.Format("[{0}]({1}{2})", this.SelectedText, hyperlink, text);
		}
		public void InsertImage(string imageUrl, string alt)
		{
			this.SelectedText = string.Format("![{1}]({0})", imageUrl, alt);
		}
		public void InsertHorizontalRule()
		{
			if (this.SelectionLength == 0)
			{
				string text = System.Environment.NewLine;
				if (!this.CurrentLine.IsBlankLine())
				{
					text += System.Environment.NewLine;
				}
				this.SelectedText = text + MarkdownSyntax.HorizontalRule + System.Environment.NewLine;
				this.SelectionLength = 0;
				this.SelectionStart += MarkdownSyntax.HorizontalRule.Length + text.Length + System.Environment.NewLine.Length;
				return;
			}
			this.SelectedText = MarkdownSyntax.HorizontalRule;
		}
		public void Cut()
		{
			if (this.SelectionLength == 0)
			{
				return;
			}
			if (!SystemUtilities.CopyStringToClipboard(this.SelectedText))
			{
				return;
			}
			this.SelectedText = string.Empty;
		}
		public void Copy()
		{
			if (this.SelectionLength == 0)
			{
				return;
			}
			SystemUtilities.CopyStringToClipboard(this.SelectedText);
		}
		public void CopyDocumentAsHtml()
		{
			string textToCopy = this.MarkdownProcessor.ConvertMarkdownToHTML(this.Editor.Text);
			SystemUtilities.CopyStringToClipboard(textToCopy);
		}
		public void Paste()
		{
			if (!Clipboard.ContainsText())
			{
				return;
			}
			string text = Clipboard.GetText();
			this.SelectedText = text;
			this.SelectionLength = 0;
			this.SelectionStart += text.Length;
		}
		public void Undo()
		{
			this.Editor.Document.UndoStack.Undo();
		}
		public void Redo()
		{
			this.Editor.Document.UndoStack.Redo();
		}
		public void SelectAll()
		{
			this.Editor.SelectAll();
		}
		public void Delete()
		{
			this.Editor.SelectedText = string.Empty;
		}
		public void ToggleSymmetricalMarkdownFormatting(string syntax)
		{
			int selectionLength = this.Editor.SelectionLength;
			int selectionStart = this.Editor.SelectionStart;
			if (selectionLength == 0 && selectionStart + syntax.Length <= this.Editor.Text.Length)
			{
				string text = this.Editor.Document.GetText(selectionStart, syntax.Length);
				if (text == syntax)
				{
					this.Editor.SelectionStart += syntax.Length;
					return;
				}
			}
			char[] array = syntax.ToCharArray();
			System.Array.Reverse(array);
			string text2 = new string(array);
			int num = this.Editor.SelectionLength;
			int num2 = this.Editor.SelectionStart;
			if (num2 >= syntax.Length)
			{
				num2 -= syntax.Length;
				num += syntax.Length;
			}
			DocumentLine lineByOffset = this.Editor.Document.GetLineByOffset(this.Editor.CaretOffset);
			if (num2 + num + syntax.Length <= lineByOffset.EndOffset)
			{
				num += syntax.Length;
			}
			string text3 = "";
			if (num > 0)
			{
				text3 = this.Editor.Document.GetText(num2, num);
			}
			Match match = Regex.Match(text3, string.Concat(new string[]
			{
				"^",
				Regex.Escape(syntax),
				"(.*)",
				Regex.Escape(text2),
				"$"
			}), RegexOptions.Singleline);
			bool success = match.Success;
			if (success)
			{
				text3 = match.Groups[1].Value;
				this.Editor.SelectionStart = num2;
				this.Editor.SelectionLength = num;
				this.Editor.SelectedText = text3;
				return;
			}
			text3 = syntax + this.Editor.SelectedText + text2;
			this.Editor.SelectedText = text3;
			this.Editor.SelectionLength -= syntax.Length * 2;
			this.Editor.SelectionStart += syntax.Length;
		}
		public void ToggleAsymmetricMarkdownFormatting(string markdownSyntaxToApply)
		{
			bool flag = this.Editor.SelectedText == this.Editor.Document.GetText(this.CurrentLine);
			bool flag2 = this.Editor.SelectedText.Contains(System.Environment.NewLine);
			bool flag3 = this.Editor.CaretOffset == this.CurrentLine.Offset;
			if ((!flag3 || !flag) && !flag2)
			{
				this.Editor.SelectedText = System.Environment.NewLine + this.Editor.SelectedText;
				this.Editor.SelectionLength -= System.Environment.NewLine.Length;
				this.Editor.SelectionStart += System.Environment.NewLine.Length;
			}
			if (this.Editor.SelectionLength > 0)
			{
				string selectedText = this.Editor.SelectedText;
				string selectedText2;
				if (selectedText.Contains(markdownSyntaxToApply))
				{
					selectedText2 = selectedText.Replace(markdownSyntaxToApply, "");
				}
				else
				{
					selectedText2 = markdownSyntaxToApply + selectedText.Replace(System.Environment.NewLine, System.Environment.NewLine + markdownSyntaxToApply);
				}
				this.Editor.SelectedText = selectedText2;
				return;
			}
			DocumentLine lineByOffset = this.Editor.Document.GetLineByOffset(this.Editor.CaretOffset);
			string text = string.Empty;
			if (!lineByOffset.IsBlankLine())
			{
				text = System.Environment.NewLine;
			}
			this.Editor.SelectedText = text + markdownSyntaxToApply;
			this.Editor.SelectionLength = 0;
			this.Editor.SelectionStart += markdownSyntaxToApply.Length + text.Length;
		}
		private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
		{
			this.OnPropertyChanged("CharacterCount");
			string text = e.Text;
			if (this.wordSeparatorRegex.IsMatch(text))
			{
				this.SpellCheckKeyActivated = true;
				this.UpdateWordCount();
				return;
			}
			this.SpellCheckKeyActivated = false;
		}
		private void Editor_TextChanged(object sender, System.EventArgs e)
		{
			this.OnPropertyChanged("HasChanges");
			if (this.TextChanged != null)
			{
				this.TextChanged(sender, e);
			}
		}
		private void UpdateWordCount()
		{
			this.WordCount = this.wordCountRegex.Matches(this.Editor.Text).Count;
		}
		public void Dispose()
		{
			MarkdownEditor._logger.Trace("Disposing MarkdownEditor control");
			if (this.SpellCheckProvider != null)
			{
				this.SpellCheckProvider.Disconnect();
			}
			if (this.AutoSaveTimer != null)
			{
				this.ToggleAutoSave(false);
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
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/usercontrols/markdowneditor.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				this.Editor = (TextEditor)target;
				return;
			}
			this._contentLoaded = true;
		}
	}
}
