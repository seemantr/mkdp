using MarkdownPad2.Markdown;
using MarkdownPad2.Network;
using MarkdownPad2.SessionManager;
using MarkdownPad2.SyntaxRules;
using MarkdownPad2.Updater;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
namespace MarkdownPad2.Properties
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0"), System.Runtime.CompilerServices.CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool CodeEditor_DisplayLineNumbers
		{
			get
			{
				return (bool)this["CodeEditor_DisplayLineNumbers"];
			}
			set
			{
				this["CodeEditor_DisplayLineNumbers"] = value;
			}
		}
		[DefaultSettingValue("12"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public double CodeEditor_FontSize
		{
			get
			{
				return (double)this["CodeEditor_FontSize"];
			}
			set
			{
				this["CodeEditor_FontSize"] = value;
			}
		}
		[DefaultSettingValue("Consolas"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public FontFamily CodeEditor_FontFamily
		{
			get
			{
				return (FontFamily)this["CodeEditor_FontFamily"];
			}
			set
			{
				this["CodeEditor_FontFamily"] = value;
			}
		}
		[DefaultSettingValue("markdownpad-github.css"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public string CodeEditor_SelectedCssFileName
		{
			get
			{
				return (string)this["CodeEditor_SelectedCssFileName"];
			}
			set
			{
				this["CodeEditor_SelectedCssFileName"] = value;
			}
		}
		[DefaultSettingValue("1"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int IO_OpenFile_LastUsedFileTypeIndex
		{
			get
			{
				return (int)this["IO_OpenFile_LastUsedFileTypeIndex"];
			}
			set
			{
				this["IO_OpenFile_LastUsedFileTypeIndex"] = value;
			}
		}
		[DefaultSettingValue("1"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int IO_SaveFile_LastUsedFileTypeIndex
		{
			get
			{
				return (int)this["IO_SaveFile_LastUsedFileTypeIndex"];
			}
			set
			{
				this["IO_SaveFile_LastUsedFileTypeIndex"] = value;
			}
		}
		[DefaultSettingValue("14"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public double Editor_FontSize
		{
			get
			{
				return (double)this["Editor_FontSize"];
			}
			set
			{
				this["Editor_FontSize"] = value;
			}
		}
		[DefaultSettingValue("Consolas"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public FontFamily Editor_FontFamily
		{
			get
			{
				return (FontFamily)this["Editor_FontFamily"];
			}
			set
			{
				this["Editor_FontFamily"] = value;
			}
		}
		[DefaultSettingValue("#FF000000"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public Color Editor_ForegroundColor
		{
			get
			{
				return (Color)this["Editor_ForegroundColor"];
			}
			set
			{
				this["Editor_ForegroundColor"] = value;
			}
		}
		[DefaultSettingValue("#FFFFFFFF"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public Color Editor_BackgroundColor
		{
			get
			{
				return (Color)this["Editor_BackgroundColor"];
			}
			set
			{
				this["Editor_BackgroundColor"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_LineNumbersEnabled
		{
			get
			{
				return (bool)this["Editor_LineNumbersEnabled"];
			}
			set
			{
				this["Editor_LineNumbersEnabled"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_UseSpacesAsTabs
		{
			get
			{
				return (bool)this["Editor_UseSpacesAsTabs"];
			}
			set
			{
				this["Editor_UseSpacesAsTabs"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_MarkdownSyntaxHighlightingEnabled
		{
			get
			{
				return (bool)this["Editor_MarkdownSyntaxHighlightingEnabled"];
			}
			set
			{
				this["Editor_MarkdownSyntaxHighlightingEnabled"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_UseUnderlineStyleHeadings
		{
			get
			{
				return (bool)this["Markdown_UseUnderlineStyleHeadings"];
			}
			set
			{
				this["Markdown_UseUnderlineStyleHeadings"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_SpellCheckEnabled
		{
			get
			{
				return (bool)this["Editor_SpellCheckEnabled"];
			}
			set
			{
				this["Editor_SpellCheckEnabled"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_EnableHyperlinks
		{
			get
			{
				return (bool)this["Editor_EnableHyperlinks"];
			}
			set
			{
				this["Editor_EnableHyperlinks"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_LivePreviewEnabled
		{
			get
			{
				return (bool)this["App_LivePreviewEnabled"];
			}
			set
			{
				this["App_LivePreviewEnabled"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_IsFirstRun
		{
			get
			{
				return (bool)this["App_IsFirstRun"];
			}
			set
			{
				this["App_IsFirstRun"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_ShowMainToolbar
		{
			get
			{
				return (bool)this["App_ShowMainToolbar"];
			}
			set
			{
				this["App_ShowMainToolbar"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_ShowStatusBar
		{
			get
			{
				return (bool)this["App_ShowStatusBar"];
			}
			set
			{
				this["App_ShowStatusBar"] = value;
			}
		}
		[DefaultSettingValue("0"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public double App_WindowState_Top
		{
			get
			{
				return (double)this["App_WindowState_Top"];
			}
			set
			{
				this["App_WindowState_Top"] = value;
			}
		}
		[DefaultSettingValue("0"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public double App_WindowState_Left
		{
			get
			{
				return (double)this["App_WindowState_Left"];
			}
			set
			{
				this["App_WindowState_Left"] = value;
			}
		}
		[DefaultSettingValue("768"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public double App_WindowState_Height
		{
			get
			{
				return (double)this["App_WindowState_Height"];
			}
			set
			{
				this["App_WindowState_Height"] = value;
			}
		}
		[DefaultSettingValue("1024"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public double App_WindowState_Width
		{
			get
			{
				return (double)this["App_WindowState_Width"];
			}
			set
			{
				this["App_WindowState_Width"] = value;
			}
		}
		[DefaultSettingValue("Normal"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public WindowState App_WindowState
		{
			get
			{
				return (WindowState)this["App_WindowState"];
			}
			set
			{
				this["App_WindowState"] = value;
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("False"), System.Diagnostics.DebuggerNonUserCode]
		public bool App_IsBeta
		{
			get
			{
				return (bool)this["App_IsBeta"];
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_VerticalLayoutEnabled
		{
			get
			{
				return (bool)this["App_VerticalLayoutEnabled"];
			}
			set
			{
				this["App_VerticalLayoutEnabled"] = value;
			}
		}
		[DefaultSettingValue("en-US"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public System.Globalization.CultureInfo App_Locale
		{
			get
			{
				return (System.Globalization.CultureInfo)this["App_Locale"];
			}
			set
			{
				this["App_Locale"] = value;
			}
		}
		[UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public System.Collections.ArrayList App_RecentlyFiles
		{
			get
			{
				return (System.Collections.ArrayList)this["App_RecentlyFiles"];
			}
			set
			{
				this["App_RecentlyFiles"] = value;
			}
		}
		[DefaultSettingValue("10"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int App_MaxRecentFiles
		{
			get
			{
				return (int)this["App_MaxRecentFiles"];
			}
			set
			{
				this["App_MaxRecentFiles"] = value;
			}
		}
		[DefaultSettingValue(""), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public string Renderer_CustomHtmlHeadContent
		{
			get
			{
				return (string)this["Renderer_CustomHtmlHeadContent"];
			}
			set
			{
				this["Renderer_CustomHtmlHeadContent"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_ExportTextWithBom
		{
			get
			{
				return (bool)this["IO_ExportTextWithBom"];
			}
			set
			{
				this["IO_ExportTextWithBom"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_ExportHtmlWithBom
		{
			get
			{
				return (bool)this["IO_ExportHtmlWithBom"];
			}
			set
			{
				this["IO_ExportHtmlWithBom"] = value;
			}
		}
		[DefaultSettingValue("en-US"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public System.Globalization.CultureInfo Editor_SpellCheckLanguage
		{
			get
			{
				return (System.Globalization.CultureInfo)this["Editor_SpellCheckLanguage"];
			}
			set
			{
				this["Editor_SpellCheckLanguage"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_OpenHtmlFileAfterExport
		{
			get
			{
				return (bool)this["IO_OpenHtmlFileAfterExport"];
			}
			set
			{
				this["IO_OpenHtmlFileAfterExport"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_ScrollToEndOfDocumentOnLoad
		{
			get
			{
				return (bool)this["IO_ScrollToEndOfDocumentOnLoad"];
			}
			set
			{
				this["IO_ScrollToEndOfDocumentOnLoad"] = value;
			}
		}
		[DefaultSettingValue("Markdown"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public MarkdownProcessorType Markdown_MarkdownProcessor
		{
			get
			{
				return (MarkdownProcessorType)this["Markdown_MarkdownProcessor"];
			}
			set
			{
				this["Markdown_MarkdownProcessor"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_Standard_AutoHyperlink
		{
			get
			{
				return (bool)this["Markdown_Standard_AutoHyperlink"];
			}
			set
			{
				this["Markdown_Standard_AutoHyperlink"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_Standard_AutoNewlines
		{
			get
			{
				return (bool)this["Markdown_Standard_AutoNewlines"];
			}
			set
			{
				this["Markdown_Standard_AutoNewlines"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_Standard_LinkEmails
		{
			get
			{
				return (bool)this["Markdown_Standard_LinkEmails"];
			}
			set
			{
				this["Markdown_Standard_LinkEmails"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_Standard_EncodeProblemUrlCharacters
		{
			get
			{
				return (bool)this["Markdown_Standard_EncodeProblemUrlCharacters"];
			}
			set
			{
				this["Markdown_Standard_EncodeProblemUrlCharacters"] = value;
			}
		}
		[DefaultSettingValue(""), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public string Markdown_GFM_Username
		{
			get
			{
				return (string)this["Markdown_GFM_Username"];
			}
			set
			{
				this["Markdown_GFM_Username"] = value;
			}
		}
		[DefaultSettingValue(""), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public string Markdown_GFM_Password
		{
			get
			{
				return (string)this["Markdown_GFM_Password"];
			}
			set
			{
				this["Markdown_GFM_Password"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_OpenPdfFileAfterExport
		{
			get
			{
				return (bool)this["IO_OpenPdfFileAfterExport"];
			}
			set
			{
				this["IO_OpenPdfFileAfterExport"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_UpgradeRequired
		{
			get
			{
				return (bool)this["App_UpgradeRequired"];
			}
			set
			{
				this["App_UpgradeRequired"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_GFM_AnonymousMode
		{
			get
			{
				return (bool)this["Markdown_GFM_AnonymousMode"];
			}
			set
			{
				this["Markdown_GFM_AnonymousMode"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_InsertHyperlinksAsFootnotes
		{
			get
			{
				return (bool)this["Editor_InsertHyperlinksAsFootnotes"];
			}
			set
			{
				this["Editor_InsertHyperlinksAsFootnotes"] = value;
			}
		}
		[DefaultSettingValue("Dash"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public UnorderedListStyle Markdown_UnorderedListStyle
		{
			get
			{
				return (UnorderedListStyle)this["Markdown_UnorderedListStyle"];
			}
			set
			{
				this["Markdown_UnorderedListStyle"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_UseUnixStyleEol
		{
			get
			{
				return (bool)this["IO_UseUnixStyleEol"];
			}
			set
			{
				this["IO_UseUnixStyleEol"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_SendAnonymousStatistics
		{
			get
			{
				return (bool)this["App_SendAnonymousStatistics"];
			}
			set
			{
				this["App_SendAnonymousStatistics"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_AutoSaveEnabled
		{
			get
			{
				return (bool)this["IO_AutoSaveEnabled"];
			}
			set
			{
				this["IO_AutoSaveEnabled"] = value;
			}
		}
		[DefaultSettingValue("5"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int IO_AutoSaveInterval
		{
			get
			{
				return (int)this["IO_AutoSaveInterval"];
			}
			set
			{
				this["IO_AutoSaveInterval"] = value;
			}
		}
		[DefaultSettingValue(""), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public string Editor_TimestampFormat
		{
			get
			{
				return (string)this["Editor_TimestampFormat"];
			}
			set
			{
				this["Editor_TimestampFormat"] = value;
			}
		}
		[DefaultSettingValue(""), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public string App_LicenseKey
		{
			get
			{
				return (string)this["App_LicenseKey"];
			}
			set
			{
				this["App_LicenseKey"] = value;
			}
		}
		[DefaultSettingValue(""), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public string App_LicenseEmail
		{
			get
			{
				return (string)this["App_LicenseEmail"];
			}
			set
			{
				this["App_LicenseEmail"] = value;
			}
		}
		[DefaultSettingValue("7"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int App_MaxSessions
		{
			get
			{
				return (int)this["App_MaxSessions"];
			}
			set
			{
				this["App_MaxSessions"] = value;
			}
		}
		[UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public SessionCollection App_Sessions
		{
			get
			{
				return (SessionCollection)this["App_Sessions"];
			}
			set
			{
				this["App_Sessions"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_AutoSessionRestore
		{
			get
			{
				return (bool)this["App_AutoSessionRestore"];
			}
			set
			{
				this["App_AutoSessionRestore"] = value;
			}
		}
		[DefaultSettingValue("Always"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public UpdateFrequency App_UpdateFrequency
		{
			get
			{
				return (UpdateFrequency)this["App_UpdateFrequency"];
			}
			set
			{
				this["App_UpdateFrequency"] = value;
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("10"), System.Diagnostics.DebuggerNonUserCode]
		public int App_DelayBeforeUpdateCheck
		{
			get
			{
				return (int)this["App_DelayBeforeUpdateCheck"];
			}
		}
		[DefaultSettingValue("Auto"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public ProxyType App_ProxyType
		{
			get
			{
				return (ProxyType)this["App_ProxyType"];
			}
			set
			{
				this["App_ProxyType"] = value;
			}
		}
		[DefaultSettingValue(""), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public string App_ManualProxy
		{
			get
			{
				return (string)this["App_ManualProxy"];
			}
			set
			{
				this["App_ManualProxy"] = value;
			}
		}
		[DefaultSettingValue("80"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int App_ProxyPort
		{
			get
			{
				return (int)this["App_ProxyPort"];
			}
			set
			{
				this["App_ProxyPort"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_ShowColumnGuide
		{
			get
			{
				return (bool)this["Editor_ShowColumnGuide"];
			}
			set
			{
				this["Editor_ShowColumnGuide"] = value;
			}
		}
		[DefaultSettingValue("80"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int CodeEditor_ColumnGuidePosition
		{
			get
			{
				return (int)this["CodeEditor_ColumnGuidePosition"];
			}
			set
			{
				this["CodeEditor_ColumnGuidePosition"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_WordWrap
		{
			get
			{
				return (bool)this["Editor_WordWrap"];
			}
			set
			{
				this["Editor_WordWrap"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_Pdf_IncludeBackground
		{
			get
			{
				return (bool)this["IO_Pdf_IncludeBackground"];
			}
			set
			{
				this["IO_Pdf_IncludeBackground"] = value;
			}
		}
		[DefaultSettingValue("Normal"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public FontWeight Editor_FontWeight
		{
			get
			{
				return (FontWeight)this["Editor_FontWeight"];
			}
			set
			{
				this["Editor_FontWeight"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_Pdf_EnableOutlineGeneration
		{
			get
			{
				return (bool)this["IO_Pdf_EnableOutlineGeneration"];
			}
			set
			{
				this["IO_Pdf_EnableOutlineGeneration"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_EnableHighPerformanceRendering
		{
			get
			{
				return (bool)this["Markdown_EnableHighPerformanceRendering"];
			}
			set
			{
				this["Markdown_EnableHighPerformanceRendering"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_MonitorFileChangesOnDisk
		{
			get
			{
				return (bool)this["IO_MonitorFileChangesOnDisk"];
			}
			set
			{
				this["IO_MonitorFileChangesOnDisk"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_DisplayFormattingMarks
		{
			get
			{
				return (bool)this["Editor_DisplayFormattingMarks"];
			}
			set
			{
				this["Editor_DisplayFormattingMarks"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_AutomaticReloadOnExternalChanges
		{
			get
			{
				return (bool)this["IO_AutomaticReloadOnExternalChanges"];
			}
			set
			{
				this["IO_AutomaticReloadOnExternalChanges"] = value;
			}
		}
		[DefaultSettingValue("#FF0000FF"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public Color Editor_HyperlinkForegroundColor
		{
			get
			{
				return (Color)this["Editor_HyperlinkForegroundColor"];
			}
			set
			{
				this["Editor_HyperlinkForegroundColor"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_AutoContinueLists
		{
			get
			{
				return (bool)this["Editor_AutoContinueLists"];
			}
			set
			{
				this["Editor_AutoContinueLists"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_VerifyNetworkConnection
		{
			get
			{
				return (bool)this["App_VerifyNetworkConnection"];
			}
			set
			{
				this["App_VerifyNetworkConnection"] = value;
			}
		}
		[DefaultSettingValue(""), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public string Markdown_ReferenceFile
		{
			get
			{
				return (string)this["Markdown_ReferenceFile"];
			}
			set
			{
				this["Markdown_ReferenceFile"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Editor_MarkdownHighlighting_EnableCustomRules
		{
			get
			{
				return (bool)this["Editor_MarkdownHighlighting_EnableCustomRules"];
			}
			set
			{
				this["Editor_MarkdownHighlighting_EnableCustomRules"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool App_DisplaySplashScreen
		{
			get
			{
				return (bool)this["App_DisplaySplashScreen"];
			}
			set
			{
				this["App_DisplaySplashScreen"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool IO_Pdf_LandscapeMode
		{
			get
			{
				return (bool)this["IO_Pdf_LandscapeMode"];
			}
			set
			{
				this["IO_Pdf_LandscapeMode"] = value;
			}
		}
		[DefaultSettingValue("40"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int IO_Pdf_MarginLeftInMillimeters
		{
			get
			{
				return (int)this["IO_Pdf_MarginLeftInMillimeters"];
			}
			set
			{
				this["IO_Pdf_MarginLeftInMillimeters"] = value;
			}
		}
		[DefaultSettingValue("40"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int IO_Pdf_MarginTopInMillimeters
		{
			get
			{
				return (int)this["IO_Pdf_MarginTopInMillimeters"];
			}
			set
			{
				this["IO_Pdf_MarginTopInMillimeters"] = value;
			}
		}
		[DefaultSettingValue("40"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int IO_Pdf_MarginRightInMillimeters
		{
			get
			{
				return (int)this["IO_Pdf_MarginRightInMillimeters"];
			}
			set
			{
				this["IO_Pdf_MarginRightInMillimeters"] = value;
			}
		}
		[DefaultSettingValue("40"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public int IO_Pdf_MarginBottomInMillimeters
		{
			get
			{
				return (int)this["IO_Pdf_MarginBottomInMillimeters"];
			}
			set
			{
				this["IO_Pdf_MarginBottomInMillimeters"] = value;
			}
		}
		[DefaultSettingValue("Letter"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public PaperKind IO_Pdf_PaperSize
		{
			get
			{
				return (PaperKind)this["IO_Pdf_PaperSize"];
			}
			set
			{
				this["IO_Pdf_PaperSize"] = value;
			}
		}
		[UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public SyntaxRuleCollection Editor_MarkdownHighlighting_CustomRuleCollection
		{
			get
			{
				return (SyntaxRuleCollection)this["Editor_MarkdownHighlighting_CustomRuleCollection"];
			}
			set
			{
				this["Editor_MarkdownHighlighting_CustomRuleCollection"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_OfflineGFM_AutoLineBreaks
		{
			get
			{
				return (bool)this["Markdown_OfflineGFM_AutoLineBreaks"];
			}
			set
			{
				this["Markdown_OfflineGFM_AutoLineBreaks"] = value;
			}
		}
		[DefaultSettingValue("True"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_OfflineGFM_SmartLists
		{
			get
			{
				return (bool)this["Markdown_OfflineGFM_SmartLists"];
			}
			set
			{
				this["Markdown_OfflineGFM_SmartLists"] = value;
			}
		}
		[DefaultSettingValue("False"), UserScopedSetting, System.Diagnostics.DebuggerNonUserCode]
		public bool Markdown_OfflineGFM_SmartyPants
		{
			get
			{
				return (bool)this["Markdown_OfflineGFM_SmartyPants"];
			}
			set
			{
				this["Markdown_OfflineGFM_SmartyPants"] = value;
			}
		}
	}
}
