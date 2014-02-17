using MarkdownPad2.i18n;
using MarkdownPad2.UI;
using System;
using System.Windows.Input;
namespace MarkdownPad2.Core
{
	public static class MarkdownPadCommands
	{
		public static readonly RoutedUICommand New = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_New", false, "MarkdownPadStrings"), "New", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.N, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand NewWindow = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_NewWindow", false, "MarkdownPadStrings"), "NewWindow", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.N, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand Open = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Open", false, "MarkdownPadStrings"), "Open", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.O, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand ReloadFromDisk = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_ReloadFromDisk", false, "MarkdownPadStrings"), "ReloadFromDisk", typeof(MainWindow));
		public static readonly RoutedUICommand Save = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Save", false, "MarkdownPadStrings"), "Save", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.S, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand SaveAs = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_SaveAs", false, "MarkdownPadStrings"), "SaveAs", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand Close = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Close", false, "MarkdownPadStrings"), "Close", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.W, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand CloseTab = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Close", false, "MarkdownPadStrings"), "CloseTab", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F4, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand CloseMiddleClick = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Close", false, "MarkdownPadStrings"), "CloseMiddleClick", typeof(MainWindow));
		public static readonly RoutedUICommand Undo = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Undo", false, "MarkdownPadStrings"), "Undo", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.Z, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Redo = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Redo", false, "MarkdownPadStrings"), "Redo", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.Y, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Cut = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Cut", false, "MarkdownPadStrings"), "Cut", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.X, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Copy = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Copy", false, "MarkdownPadStrings"), "Copy", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.C, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Paste = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Paste", false, "MarkdownPadStrings"), "Paste", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.V, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Delete = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Delete", false, "MarkdownPadStrings"), "Delete", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.Delete, System.Windows.Input.ModifierKeys.None)
		});
		public static readonly RoutedUICommand SelectAll = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_SelectAll", false, "MarkdownPadStrings"), "SelectAll", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Find = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Find", false, "MarkdownPadStrings"), "Find", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand EnableLivePreview = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_EnableLivePreview", false, "MarkdownPadStrings"), "EnableLivePreview", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F5, System.Windows.Input.ModifierKeys.None)
		});
		public static readonly RoutedUICommand Bold = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Bold", false, "MarkdownPadStrings"), "Bold", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.B, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Italic = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Italic", false, "MarkdownPadStrings"), "Italic", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.I, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Code = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Code", false, "MarkdownPadStrings"), "Code", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.K, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Quote = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Quote", false, "MarkdownPadStrings"), "Quote", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.Q, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Heading1 = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Heading1", false, "MarkdownPadStrings"), "Heading1", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.D1, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Heading2 = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Heading2", false, "MarkdownPadStrings"), "Heading2", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.D2, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Heading3 = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Heading3", false, "MarkdownPadStrings"), "Heading3", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.D3, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Heading4 = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Heading4", false, "MarkdownPadStrings"), "Heading4", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.D4, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Heading5 = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Heading5", false, "MarkdownPadStrings"), "Heading5", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.D5, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand Heading6 = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Heading6", false, "MarkdownPadStrings"), "Heading6", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.D6, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand ReportBug = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_ReportBug", false, "MarkdownPadStrings"), "ReportBug", typeof(MainWindow));
		public static readonly RoutedUICommand MarkdownSyntaxHighlighting = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_MarkdownSyntaxHighlighting", false, "MarkdownPadStrings"), "MarkdownSyntaxHighlighting", typeof(MainWindow));
		public static readonly RoutedUICommand LineNumbers = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_DisplayLineNumbers", false, "MarkdownPadStrings"), "DisplayLineNumbers", typeof(MainWindow));
		public static readonly RoutedUICommand WordWrap = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_WordWrap", false, "MarkdownPadStrings"), "WordWrap", typeof(MainWindow));
		public static readonly RoutedUICommand SpellCheck = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_SpellCheck", false, "MarkdownPadStrings"), "SpellCheck", typeof(MainWindow));
		public static readonly RoutedUICommand CloseAll = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_CloseAll", false, "MarkdownPadStrings"), "CloseAll", typeof(MainWindow));
		public static readonly RoutedUICommand CopyHTML = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_CopyHTML", false, "MarkdownPadStrings"), "CopyHTML", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.C, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand CopyDocumentAsHtml = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_CopyDocumentAsHtml", false, "MarkdownPadStrings"), "CopyDocumentAsHTML", typeof(MainWindow));
		public static readonly RoutedUICommand SaveAll = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_SaveAll", false, "MarkdownPadStrings"), "SaveAll", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.S, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand PrintHTML = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_PrintHTML", false, "MarkdownPadStrings"), "PrintHTML", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.P, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand PrintMarkdown = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_PrintMarkdown", false, "MarkdownPadStrings"), "PrintMarkdown", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.P, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand ExportHtml = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_ExportHtml", false, "MarkdownPadStrings"), "ExportHTML", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.D1, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand ExportPdf = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_ExportPdf", false, "MarkdownPadStrings"), "ExportPdf", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.D2, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand Quit = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_Quit", false, "MarkdownPadStrings"), "Quit", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F4, System.Windows.Input.ModifierKeys.Alt)
		});
		public static readonly RoutedUICommand PreviewMarkdown = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_PreviewMarkdown", false, "MarkdownPadStrings"), "PreviewMarkdown", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F6)
		});
		public static readonly RoutedUICommand InsertHyperlink = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_InsertHyperlink", false, "MarkdownPadStrings"), "InsertHyperlink", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.L, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand FindNext = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_FindNext", false, "MarkdownPadStrings"), "FindNext", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F3)
		});
		public static readonly RoutedUICommand FindPrevious = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_FindPrevious", false, "MarkdownPadStrings"), "FindPrevious", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F3, System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand CloseSearchPanel = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_CloseSearchPanel", false, "MarkdownPadStrings"), "CloseSearchPanel", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.Escape)
		});
		public static readonly RoutedUICommand UnorderedList = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_UnorderedList", false, "MarkdownPadStrings"), "UnorderedList", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.U, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand OrderedList = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_OrderedList", false, "MarkdownPadStrings"), "OrderedList", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.O, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand InsertImage = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_InsertImage", false, "MarkdownPadStrings"), "InsertImage", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.G, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand EnableHorizontalLayout = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_EnableHorizontalLayout", false, "MarkdownPadStrings"), "EnableHorizontalLayout", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F4, System.Windows.Input.ModifierKeys.None)
		});
		public static readonly RoutedUICommand OpenOptionsWindow = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_OpenOptions", false, "MarkdownPadStrings"), "OpenOptionsWindow", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F7, System.Windows.Input.ModifierKeys.None)
		});
		public static readonly RoutedUICommand InsertHorizontalRule = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_InsertHorizontalRule", false, "MarkdownPadStrings"), "InsertHorizontalRule", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.R, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand EnableAutoSave = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_EnableAutoSave", false, "MarkdownPadStrings"), "ToggleAutoSave", typeof(MainWindow));
		public static readonly RoutedUICommand DistractionFreeMode = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_DistractionFreeMode", false, "MarkdownPadStrings"), "DistractionFreeMode", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.F11, System.Windows.Input.ModifierKeys.None)
		});
		public static readonly RoutedUICommand InsertTimestamp = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_InsertTimestamp", false, "MarkdownPadStrings"), "InsertTimestamp", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.T, System.Windows.Input.ModifierKeys.Control)
		});
		public static readonly RoutedUICommand ShowAboutWindow = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_AboutMarkdownPad", false, "MarkdownPadStrings"), "ShowAboutWindow", typeof(MainWindow));
		public static readonly RoutedUICommand OpenStandardMarkdownSyntaxGuide = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_StandardMarkdownSyntaxGuide", false, "MarkdownPadStrings"), "OpenStandardMarkdownSyntaxGuide", typeof(MainWindow));
		public static readonly RoutedUICommand OpenMarkdownExtraSyntaxGuide = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_MarkdownExtraSyntaxGuide", false, "MarkdownPadStrings"), "OpenMarkdownExtraSyntaxGuide", typeof(MainWindow));
		public static readonly RoutedUICommand OpenGfmSyntaxGuide = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_GitHubFlavoredMarkdownSyntaxGuide", false, "MarkdownPadStrings"), "OpenGfmSyntaxGuide", typeof(MainWindow));
		public static readonly RoutedUICommand OpenMarkdownPadWebsite = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_MarkdownPadWebsite", false, "MarkdownPadStrings"), "OpenMarkdownPadWebsite", typeof(MainWindow));
		public static readonly RoutedUICommand CheckForUpdates = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_CheckForUpdates", false, "MarkdownPadStrings"), "CheckForUpdates", typeof(MainWindow));
		public static readonly RoutedUICommand ConvertToUppercase = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_ConvertToUppercase", false, "MarkdownPadStrings"), "ConvertToUppercase", typeof(MainWindow));
		public static readonly RoutedUICommand ConvertToLowercase = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_ConvertToLowercase", false, "MarkdownPadStrings"), "ConvertToLowercase", typeof(MainWindow));
		public static readonly RoutedUICommand GoToLine = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_GoToLine", false, "MarkdownPadStrings"), "GoToLine", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.G, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand DisplayToolbar = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_DisplayToolbar", false, "MarkdownPadStrings"), "DisplayToolbar", typeof(MainWindow));
		public static readonly RoutedUICommand DisplayStatusBar = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_DisplayStatusBar", false, "MarkdownPadStrings"), "DisplayStatusBar", typeof(MainWindow));
		public static readonly RoutedUICommand UpgradePro = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_UpgradePro", false, "MarkdownPadStrings"), "UpgradePro", typeof(MainWindow));
		public static readonly RoutedUICommand MoveTab = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_MoveTab", false, "MarkdownPadStrings"), "MoveTab", typeof(MainWindow));
		public static readonly RoutedUICommand HelpTranslateMarkdownPad = new RoutedUICommand(LocalizationProvider.GetLocalizedString("HelpTranslateMarkdownPad", false, "MarkdownPadStrings"), "HelpTranslateMarkdownPad", typeof(MainWindow));
		public static readonly RoutedUICommand AutoSessionRestore = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_AutoSessionRestore", false, "MarkdownPadStrings"), "AutoSessionRestore", typeof(MainWindow));
		public static readonly RoutedUICommand MarkdownPadSupport = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_MarkdownPadSupport", false, "MarkdownPadStrings"), "MarkdownPadSupport", typeof(MainWindow));
		public static readonly RoutedUICommand SetColumnGuide = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_SetColumnGuide", false, "MarkdownPadStrings"), "SetColumnGuide", typeof(MainWindow));
		public static readonly RoutedUICommand OpenFileInExplorer = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_OpenFileInExplorer", false, "MarkdownPadStrings"), "OpenFileInExplorer", typeof(MainWindow));
		public static readonly RoutedUICommand CopyFilePath = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_CopyFilePath", false, "MarkdownPadStrings"), "CopyFilePath", typeof(MainWindow));
		public static readonly RoutedUICommand CopyLivePreviewContent = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_CopyLivePreviewContent", false, "MarkdownPadStrings"), "CopyLivePreviewContent", typeof(MainWindow), new InputGestureCollection
		{
			new KeyGesture(System.Windows.Input.Key.L, System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift)
		});
		public static readonly RoutedUICommand OpenFileInNewWindow = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_OpenFileInNewWindow", false, "MarkdownPadStrings"), "OpenFileInNewWindow", typeof(MainWindow));
		public static readonly RoutedUICommand OpenWelcomeDocument = new RoutedUICommand(LocalizationProvider.GetLocalizedString("Command_OpenWelcomeDocument", false, "MarkdownPadStrings"), "OpenWelcomeDocument", typeof(MainWindow));
	}
}
