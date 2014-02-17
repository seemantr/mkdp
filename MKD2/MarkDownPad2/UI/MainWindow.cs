using MarkdownPad2.Core;
using MarkdownPad2.Core.Extensions;
using MarkdownPad2.i18n;
using MarkdownPad2.Licensing;
using MarkdownPad2.Markdown;
using MarkdownPad2.Options;
using MarkdownPad2.Properties;
using MarkdownPad2.SessionManager;
using MarkdownPad2.SpellCheck;
using MarkdownPad2.Stylesheets;
using MarkdownPad2.Updater;
using MarkdownPad2.UserControls;
using MarkdownPad2.Utilities;
using Microsoft.Win32;
using NLog;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFCustomMessageBox;
using WPFLocalizeExtension.Engine;
using wyDay.Controls;
namespace MarkdownPad2.UI
{
	public class MainWindow : Window, INotifyPropertyChanged, System.Windows.Markup.IComponentConnector
	{
		private Settings _settings = Settings.Default;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private System.Collections.ArrayList _recentFiles = new System.Collections.ArrayList();
		private bool _distractionFreeModeEnabled;
		internal Grid Grid_Main;
		internal MenuItem MenuItem_Recent;
		internal MenuItem MenuItem_LivePreview;
		internal MenuItem MenuItem_Layout;
		internal MenuItem MenuItem_DisplayLineNumbers;
		internal MenuItem MenuItem_WordWrap;
		internal MenuItem MenuItem_MarkdownSyntax;
		internal MenuItem MenuItem_DisplayToolbar;
		internal MenuItem MenuItem_DisplayStatusBar;
		internal MenuItem MenuItem_SessionManager;
		internal MenuItem MenuItem_UpgradePro;
		internal MenuItem MenuItem_ProUnlocked;
		internal Toolbar Toolbar_Main;
		internal DragAndDrop DragAndDrop;
		internal TabbedDocumentInterface TDI;
		internal StatusBar StatusBar_Main;
		internal MenuItem MenuItem_QuickMarkdownSettingsPlaceholder;
		internal Image Image_QuickMarkdownSettings;
		internal ComboBox ComboBox_QuickMarkdownSettings;
		internal MenuItem MenuItem_SpellCheck;
		internal MenuItem MenuItem_SpellCheckEnabled;
		internal StatusBarItem StatusBarItem_WordCount;
		internal TextBlock TextBlock_WordCount;
		internal StatusBarItem StatusBarItem_SelectedWordCount;
		internal TextBlock TextBlock_SelectedWordCount;
		internal StatusBarItem StatusBarItem_CharacterCount;
		internal TextBlock TextBlock_CharacterCount;
		internal StatusBarItem StatusBarItem_SelectedCharacterCount;
		internal TextBlock TextBlock_SelectedCharacterCount;
		internal TextBlock TextBlock_StatusMessage;
		internal AutomaticUpdater Updater;
		private bool _contentLoaded;
		public event PropertyChangedEventHandler PropertyChanged;
		public bool DistractionFreeModeEnabled
		{
			get
			{
				return this._distractionFreeModeEnabled;
			}
			set
			{
				this._distractionFreeModeEnabled = value;
				this.OnPropertyChanged("DistractionFreeModeEnabled");
			}
		}
		public TabControl MainTabControl
		{
			get
			{
				return this.TDI.tabControl;
			}
		}
		public MainWindow()
		{
			Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(this.Current_DispatcherUnhandledException);
			System.Windows.SplashScreen splashScreen = null;
			if (this._settings.App_DisplaySplashScreen)
			{
				splashScreen = new System.Windows.SplashScreen("Images/markdownpad2-logo-border-256.png");
				splashScreen.Show(false);
			}
			if (this._settings.App_IsFirstRun)
			{
				if (LocalizationProvider.AvailableCultures.Contains(System.Globalization.CultureInfo.CurrentCulture))
				{
					MainWindow._logger.Trace("Current system culture is available for UI culture: " + System.Globalization.CultureInfo.CurrentCulture);
					this._settings.App_Locale = System.Globalization.CultureInfo.CurrentCulture;
				}
				else
				{
					MainWindow._logger.Trace("System culture not supported for UI culture, defaulting to en-US: " + System.Globalization.CultureInfo.CurrentCulture);
					this._settings.App_Locale = System.Globalization.CultureInfo.GetCultureInfo("en-US");
				}
				if (SpellingService.CultureLookup.Contains(System.Globalization.CultureInfo.CurrentCulture))
				{
					MainWindow._logger.Trace("Current system culture is available for spell check: " + System.Globalization.CultureInfo.CurrentCulture);
					this._settings.Editor_SpellCheckLanguage = System.Globalization.CultureInfo.CurrentCulture;
				}
				else
				{
					MainWindow._logger.Warn("System culture not supported in spell check, disabling and defauting to en-US: " + System.Globalization.CultureInfo.CurrentCulture);
					this._settings.Editor_SpellCheckEnabled = false;
					this._settings.Editor_SpellCheckLanguage = System.Globalization.CultureInfo.GetCultureInfo("en-US");
				}
			}
			this.SetApplicationLocale(this._settings.App_Locale);
			string str = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "\\MarkdownPad2\\";
			string text = str + "styles\\";
			string sourceDirectory = str + "dictionaries\\";
			if (System.IO.Directory.Exists(text))
			{
				string str2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\MarkdownPad 2\\";
				string destinationDirectory = str2 + "styles\\";
				string destinationDirectory2 = str2 + "dictionaries\\";
				FileUtils.MoveFilesFromDirectory(text, destinationDirectory, "*.css");
				FileUtils.MoveFilesFromDirectory(sourceDirectory, destinationDirectory2, "*.dic");
			}
			StyleSheetProvider styleSheetProvider = new StyleSheetProvider();
			styleSheetProvider.WriteBuiltInStylesheetsToAppData(false);
			this.InitializeComponent();
			this.InitializeControlEventsAndCommands();
			base.DataContext = this;
			if (this._settings.App_DisplaySplashScreen)
			{
				splashScreen.Close(System.TimeSpan.FromSeconds(1.0));
			}
		}
		private void SetApplicationLocale(System.Globalization.CultureInfo culture)
		{
			LocalizeDictionary.Instance.Culture = culture;
		}
		private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			System.Exception exception = e.Exception;
			MainWindow._logger.ErrorException("Unhandled exception occurred", exception);
			if (exception.GetType() == typeof(System.OutOfMemoryException))
			{
				MessageBox.Show("out of memory");
				Application.Current.Shutdown();
			}
			MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_UnexpectedErrorMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_UnexpectedErrorTitle", false, "MarkdownPadStrings"), exception, "");
			e.Handled = true;
		}
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (this._settings.App_IsFirstRun)
			{
				this._settings.App_IsFirstRun = false;
			}
			if (this._settings.App_AutoSessionRestore)
			{
				this.SaveSession(this.TDI.OpenDocumentsThatExist);
			}
			while (this.MainTabControl.SelectedIndex > -1)
			{
				MessageBoxResult messageBoxResult = this.TDI.CloseTab(this.TDI.CurrentTab, false);
				if (messageBoxResult == MessageBoxResult.Cancel)
				{
					e.Cancel = true;
					break;
				}
			}
			this._settings.App_RecentlyFiles = this._recentFiles;
			this.SaveWindowState();
			try
			{
				this._settings.Save();
			}
			catch (ConfigurationException exception)
			{
				MainWindow._logger.WarnException("Exception while seaving settings during Window_Closing event", exception);
			}
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			string[] commandLineArgs = System.Environment.GetCommandLineArgs();
			this.ProcessCommandLineArgs(commandLineArgs);
			this.CheckLicenseAndModifyMenuItems();
		}
		private void PopulateSpellCheckDictionaryMenu()
		{
			foreach (System.Globalization.CultureInfo current in SpellingService.CultureLookup)
			{
				MenuItem dictionaryMenu = new MenuItem
				{
					Header = current.NativeName,
					Tag = current
				};
				dictionaryMenu.Click += delegate(object sender, RoutedEventArgs args)
				{
					this._settings.Editor_SpellCheckLanguage = (dictionaryMenu.Tag as System.Globalization.CultureInfo);
					this.ReloadSettingsInAllEditorRenderers();
					foreach (object current2 in (System.Collections.IEnumerable)this.MenuItem_SpellCheck.Items)
					{
						System.Type type = current2.GetType();
						if (!(type != typeof(MenuItem)))
						{
							MenuItem menuItem2 = current2 as MenuItem;
							if (!object.Equals(menuItem2, this.MenuItem_SpellCheckEnabled) && menuItem2.IsChecked)
							{
								menuItem2.IsChecked = false;
							}
						}
					}
					dictionaryMenu.IsChecked = true;
				};
				if (this._settings.Editor_SpellCheckLanguage.Equals(current))
				{
					dictionaryMenu.IsChecked = true;
				}
				this.MenuItem_SpellCheck.Items.Add(dictionaryMenu);
			}
			Separator newItem = new Separator();
			this.MenuItem_SpellCheck.Items.Add(newItem);
			MenuItem menuItem = new MenuItem
			{
				Header = LocalizationProvider.GetLocalizedString("AddNewDicionaries", false, "MarkdownPadStrings")
			};
			menuItem.Click += delegate(object sender, RoutedEventArgs args)
			{
				Urls.MarkdownPad_Faq_AddSpellCheckDictionaries.TryStartDefaultProcess();
			};
			this.MenuItem_SpellCheck.Items.Add(menuItem);
		}
		private void ToggleMainToolbarVisibility(bool enable)
		{
			if (enable)
			{
				this.Toolbar_Main.Visibility = Visibility.Visible;
				this.MenuItem_DisplayToolbar.IsChecked = true;
				return;
			}
			this.Toolbar_Main.Visibility = Visibility.Collapsed;
			this.MenuItem_DisplayToolbar.IsChecked = false;
		}
		private void ToggleStatusBarVisibility(bool enable)
		{
			if (enable)
			{
				this.StatusBar_Main.Visibility = Visibility.Visible;
				this.MenuItem_DisplayStatusBar.IsChecked = true;
				base.ResizeMode = ResizeMode.CanResizeWithGrip;
				return;
			}
			this.StatusBar_Main.Visibility = Visibility.Collapsed;
			this.MenuItem_DisplayStatusBar.IsChecked = false;
			base.ResizeMode = ResizeMode.CanResize;
		}
		private void Window_Initialized(object sender, System.EventArgs e)
		{
			if (this._settings.App_IsBeta)
			{
				string title = base.Title;
				base.Title = string.Concat(new string[]
				{
					title,
					" - ",
					LocalizationProvider.GetLocalizedString("Beta", false, "MarkdownPadStrings"),
					" ",
					AssemblyUtilities.Version
				});
			}
			try
			{
				this.RestoreWindowState();
			}
			catch (System.Exception exception)
			{
				string localizedString = LocalizationProvider.GetLocalizedString("Error_RestoringWindowStateMessage", false, "MarkdownPadStrings");
				string localizedString2 = LocalizationProvider.GetLocalizedString("Error_RestoringWindowStateTitle", false, "MarkdownPadStrings");
				MessageBoxHelper.ShowErrorMessageBox(localizedString, localizedString2, exception, "");
			}
			this.UpdateLivePreviewCheckedButtons(this._settings.App_LivePreviewEnabled);
			this.ToggleMainToolbarVisibility(this._settings.App_ShowMainToolbar);
			this.ToggleStatusBarVisibility(this._settings.App_ShowStatusBar);
			if (this._settings.App_RecentlyFiles != null)
			{
				this._recentFiles.AddRange(this._settings.App_RecentlyFiles);
			}
			this.AddRecentDocumentsToMenu();
			this.PopulateSessionRestoreMenuItems();
			this.PopulateSpellCheckDictionaryMenu();
			MarkdownProcessorProvider.PopulateComboBoxWithMarkdownProcessors(this.ComboBox_QuickMarkdownSettings, this._settings.Markdown_MarkdownProcessor, this);
			this.QuickMarkdown_SetTooltip();
			this.ComboBox_QuickMarkdownSettings.SelectionChanged += new SelectionChangedEventHandler(this.ComboBox_QuickMarkdownSettings_SelectionChanged);
		}
		private void ComboBox_QuickMarkdownSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			IMarkdownProcessor processor = e.AddedItems[0] as IMarkdownProcessor;
			MarkdownProcessorType markdown_MarkdownProcessor = MarkdownProcessorProvider.ReverseLookupBad(processor);
			this._settings.Markdown_MarkdownProcessor = markdown_MarkdownProcessor;
			this.QuickMarkdown_SetTooltip();
			this.ReloadSettingsInAllEditorRenderers();
		}
		private void QuickMarkdown_SetTooltip()
		{
			IMarkdownProcessor markdownProcessor = this.ComboBox_QuickMarkdownSettings.SelectedItem as IMarkdownProcessor;
			if (markdownProcessor == null)
			{
				return;
			}
			this.MenuItem_QuickMarkdownSettingsPlaceholder.ToolTip = LocalizationProvider.GetLocalizedString("QuickMarkdownSelector", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("SelectedMarkdownProcessor", true, "MarkdownPadStrings") + markdownProcessor.ProcessorName;
		}
		private void SelectQuickMarkdownProcessor(MarkdownProcessorType processorType, bool reloadSettings)
		{
			IMarkdownProcessor markdownProcessor = MarkdownProcessorProvider.MarkdownProcessorMap[processorType];
			if (this.ComboBox_QuickMarkdownSettings.Items.Contains(markdownProcessor))
			{
				this.ComboBox_QuickMarkdownSettings.SelectionChanged -= new SelectionChangedEventHandler(this.ComboBox_QuickMarkdownSettings_SelectionChanged);
				this.ComboBox_QuickMarkdownSettings.SelectedItem = markdownProcessor;
				this.ComboBox_QuickMarkdownSettings.SelectionChanged += new SelectionChangedEventHandler(this.ComboBox_QuickMarkdownSettings_SelectionChanged);
				if (reloadSettings)
				{
					this.ReloadSettingsInAllEditorRenderers();
				}
			}
		}
		private void PopulateSessionRestoreMenuItems()
		{
			this.MenuItem_SessionManager.Items.Clear();
			MenuItem menuItem = new MenuItem
			{
				Header = LocalizationProvider.GetLocalizedString("MenuItem_SaveCurrentSession", false, "MarkdownPadStrings"),
				Icon = new Image
				{
					Source = new BitmapImage(new Uri("pack://application:,,,/MarkdownPad2;component/Images/document_save.png")),
					Width = 16.0,
					Height = 16.0
				},
				Name = "MenuItem_SaveCurrentSession",
				VerticalContentAlignment = VerticalAlignment.Center
			};
			menuItem.Click += new RoutedEventHandler(this.SaveSessionItemClick);
			this.MenuItem_SessionManager.Items.Add(menuItem);
			Binding binding = new Binding("App_AutoSessionRestore")
			{
				Mode = BindingMode.TwoWay,
				Source = Settings.Default
			};
			MenuItem menuItem2 = new MenuItem
			{
				Command = MarkdownPadCommands.AutoSessionRestore,
				Name = "MenuItem_ToggleAutoSessionRestore",
				VerticalContentAlignment = VerticalAlignment.Center
			};
			menuItem2.SetBinding(MenuItem.IsCheckedProperty, binding);
			this.MenuItem_SessionManager.Items.Add(menuItem2);
			SessionCollection app_Sessions = this._settings.App_Sessions;
			if (app_Sessions == null || app_Sessions.Sessions.Count == 0)
			{
				return;
			}
			MainWindow._logger.Trace("Populating all previous sessions in menu: " + app_Sessions.Sessions.Count);
			this.MenuItem_SessionManager.Items.Add(new Separator());
			foreach (Session current in app_Sessions.Sessions)
			{
				MenuItem newItem = new MenuItem
				{
					Header = current.Name,
					Tag = current.Files
				};
				this.MenuItem_SessionManager.Items.Add(newItem);
			}
			this.MenuItem_SessionManager.Items.Add(new Separator());
			MenuItem menuItem3 = new MenuItem
			{
				Header = LocalizationProvider.GetLocalizedString("MenuItem_ClearSessions", false, "MarkdownPadStrings"),
				Icon = new Image
				{
					Source = new BitmapImage(new Uri("pack://application:,,,/MarkdownPad2;component/Images/x.png"))
				},
				Name = "MenuItem_ClearSessions"
			};
			menuItem3.Click += delegate(object sender, RoutedEventArgs args)
			{
				this._settings.App_Sessions = new SessionCollection();
				this.PopulateSessionRestoreMenuItems();
			};
			this.MenuItem_SessionManager.Items.Add(menuItem3);
		}
		private void SaveSessionItemClick(object sender, RoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_SaveSession", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				e.Handled = true;
				return;
			}
			this.SaveSession(this.TDI.OpenDocumentsThatExist);
		}
		private void SaveSession(System.Collections.Generic.List<string> openDocuments)
		{
			if (openDocuments.Count == 0)
			{
				return;
			}
			MainWindow._logger.Trace("Saving current session: " + openDocuments.Count);
			Session session = new Session
			{
				Files = openDocuments
			};
			string arg = (openDocuments.Count > 1) ? LocalizationProvider.GetLocalizedString("SessionManager_DocumentPlural", false, "MarkdownPadStrings") : LocalizationProvider.GetLocalizedString("SessionManager_DocumentSingular", false, "MarkdownPadStrings");
			session.Name = System.DateTime.Now + string.Format(" ({0} {1})", openDocuments.Count, arg);
			if (this._settings.App_Sessions == null)
			{
				this._settings.App_Sessions = new SessionCollection();
			}
			this._settings.App_Sessions.AddToCollection(session);
			this.PopulateSessionRestoreMenuItems();
		}
		private void AddRecentDocumentsToMenu()
		{
			this.MenuItem_Recent.Items.Clear();
			System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
			arrayList.AddRange(this._recentFiles);
			arrayList.Reverse();
			if (arrayList.Count == 0)
			{
				this.MenuItem_Recent.IsEnabled = false;
				return;
			}
			this.MenuItem_Recent.IsEnabled = true;
			foreach (string text in arrayList)
			{
				MenuItem menuItem = new MenuItem();
				TextBlock header = new TextBlock
				{
					Text = text
				};
				menuItem.Header = header;
				menuItem.Tag = text;
				this.MenuItem_Recent.Items.Add(menuItem);
			}
			MenuItem menuItem2 = new MenuItem
			{
				Header = LocalizationProvider.GetLocalizedString("MenuItem_ClearRecentDocuments", false, "MarkdownPadStrings"),
				Icon = new Image
				{
					Source = new BitmapImage(new Uri("pack://application:,,,/MarkdownPad2;component/Images/x.png"))
				},
				Name = "MenuItem_Clear"
			};
			Separator newItem = new Separator();
			this.MenuItem_Recent.Items.Add(newItem);
			this.MenuItem_Recent.Items.Add(menuItem2);
			menuItem2.Click += new RoutedEventHandler(this.ClearMenuItem_Click);
		}
		private void ClearMenuItem_Click(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			this.ClearRecentlyOpenedFiles();
		}
		private void ClearRecentlyOpenedFiles()
		{
			this._recentFiles.Clear();
			this.AddRecentDocumentsToMenu();
			this._settings.App_RecentlyFiles = this._recentFiles;
		}
		private void UpdateLivePreviewCheckedButtons(bool isEnabled)
		{
			this.Toolbar_Main.Button_LivePreview.IsChecked = new bool?(isEnabled);
		}
		private void InitializeControlEventsAndCommands()
		{
			System.Collections.Generic.Dictionary<ButtonBase, RoutedUICommand> toolbarButtonCommandAndGestureTooltip = new System.Collections.Generic.Dictionary<ButtonBase, RoutedUICommand>
			{

				{
					this.Toolbar_Main.Button_LivePreview,
					MarkdownPadCommands.EnableLivePreview
				},

				{
					this.Toolbar_Main.Button_New,
					MarkdownPadCommands.New
				},

				{
					this.Toolbar_Main.Button_Open,
					MarkdownPadCommands.Open
				},

				{
					this.Toolbar_Main.Button_Save,
					MarkdownPadCommands.Save
				},

				{
					this.Toolbar_Main.Button_Bold,
					MarkdownPadCommands.Bold
				},

				{
					this.Toolbar_Main.Button_Italic,
					MarkdownPadCommands.Italic
				},

				{
					this.Toolbar_Main.Button_Code,
					MarkdownPadCommands.Code
				},

				{
					this.Toolbar_Main.Button_Quote,
					MarkdownPadCommands.Quote
				},

				{
					this.Toolbar_Main.Button_H1,
					MarkdownPadCommands.Heading1
				},

				{
					this.Toolbar_Main.Button_H2,
					MarkdownPadCommands.Heading2
				},

				{
					this.Toolbar_Main.Button_UnorderedList,
					MarkdownPadCommands.UnorderedList
				},

				{
					this.Toolbar_Main.Button_OrderedList,
					MarkdownPadCommands.OrderedList
				},

				{
					this.Toolbar_Main.Button_Hyperlink,
					MarkdownPadCommands.InsertHyperlink
				},

				{
					this.Toolbar_Main.Button_Image,
					MarkdownPadCommands.InsertImage
				},

				{
					this.Toolbar_Main.Button_HorizontalRule,
					MarkdownPadCommands.InsertHorizontalRule
				},

				{
					this.Toolbar_Main.Button_Timestamp,
					MarkdownPadCommands.InsertTimestamp
				},

				{
					this.Toolbar_Main.Button_Cut,
					MarkdownPadCommands.Cut
				},

				{
					this.Toolbar_Main.Button_Copy,
					MarkdownPadCommands.Copy
				},

				{
					this.Toolbar_Main.Button_Paste,
					MarkdownPadCommands.Paste
				},

				{
					this.Toolbar_Main.Button_SaveAll,
					MarkdownPadCommands.SaveAll
				},

				{
					this.Toolbar_Main.Button_Delete,
					MarkdownPadCommands.Delete
				},

				{
					this.Toolbar_Main.Button_Undo,
					MarkdownPadCommands.Undo
				},

				{
					this.Toolbar_Main.Button_Redo,
					MarkdownPadCommands.Redo
				},

				{
					this.Toolbar_Main.Button_Uppercase,
					MarkdownPadCommands.ConvertToUppercase
				},

				{
					this.Toolbar_Main.Button_Lowercase,
					MarkdownPadCommands.ConvertToLowercase
				},

				{
					this.Toolbar_Main.Button_ToggleLayout,
					MarkdownPadCommands.EnableHorizontalLayout
				},

				{
					this.Toolbar_Main.Button_PreviewMarkdown,
					MarkdownPadCommands.PreviewMarkdown
				}
			};
			this.SetToolbarButtonCommandAndGestureTooltip(toolbarButtonCommandAndGestureTooltip);
		}
		private void MainWindow_Activated(object sender, System.EventArgs e)
		{
			this.CheckIfOpenFilesModifiedOrDeletedOnDisk();
		}
		private void CheckIfOpenFilesModifiedOrDeletedOnDisk()
		{
			if (!this._settings.IO_MonitorFileChangesOnDisk)
			{
				return;
			}
			this.ToggleActivationEventSubscription(false);
			for (int i = 0; i < this.MainTabControl.Items.Count; i++)
			{
				TabItem tabItem = this.MainTabControl.Items.GetItemAt(i) as TabItem;
				EditorRenderer editorRenderer = tabItem.Content as EditorRenderer;
				if (editorRenderer != null)
				{
					switch (editorRenderer.Editor.CheckIfFileModifiedOrDeletedOnDisk())
					{
					case FileModificationType.Modified:
					{
						tabItem.Focus();
						MessageBoxResult messageBoxResult = MessageBoxResult.Yes;
						if (!this._settings.IO_AutomaticReloadOnExternalChanges)
						{
							messageBoxResult = CustomMessageBox.ShowYesNo(LocalizationProvider.GetLocalizedString("FileModified_Message", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("FileModified_Title", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("FileModified_Reload", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("No", false, "MarkdownPadStrings"), MessageBoxImage.Asterisk);
						}
						switch (messageBoxResult)
						{
						case MessageBoxResult.Yes:
							editorRenderer.Editor.ReloadCurrentDocument();
							break;
						case MessageBoxResult.No:
							editorRenderer.Editor.ResetFileLastModifiedTime();
							break;
						}
						break;
					}
					case FileModificationType.Deleted:
						tabItem.Focus();
						switch (CustomMessageBox.ShowYesNo(LocalizationProvider.GetLocalizedString("FileDeleted_Message", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("FileDeleted_Title", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Yes", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("No", false, "MarkdownPadStrings"), MessageBoxImage.Asterisk))
						{
						case MessageBoxResult.Yes:
							editorRenderer.Editor.ClearOriginalDocument();
							break;
						case MessageBoxResult.No:
							this.TDI.CloseTab(tabItem, true);
							break;
						}
						break;
					}
				}
			}
			this.ToggleActivationEventSubscription(true);
		}
		private void SetToolbarButtonCommandAndGestureTooltip(System.Collections.Generic.Dictionary<ButtonBase, RoutedUICommand> inputs)
		{
			foreach (System.Collections.Generic.KeyValuePair<ButtonBase, RoutedUICommand> current in inputs)
			{
				ButtonBase key = current.Key;
				RoutedUICommand value = current.Value;
				key.Command = value;
				key.ToolTip = value.Text.Replace("_", "");
				if (value.InputGestures.Count > 0)
				{
					KeyGesture keyGesture = (KeyGesture)value.InputGestures[0];
					string arg = (keyGesture.Modifiers == System.Windows.Input.ModifierKeys.None) ? string.Empty : (keyGesture.Modifiers + "+");
					System.Windows.Input.KeyConverter keyConverter = new System.Windows.Input.KeyConverter();
					ButtonBase expr_95 = key;
					expr_95.ToolTip += string.Format(" ({0}{1})", arg, keyConverter.ConvertToString(keyGesture.Key));
				}
			}
		}
		private void ConnectStatusBarDataContexts()
		{
			if (this.TDI.CurrentEditorRenderer != null)
			{
				this.TextBlock_WordCount.DataContext = this.TDI.CurrentEditorRenderer.Editor;
				this.TextBlock_CharacterCount.DataContext = this.TDI.CurrentEditorRenderer.Editor;
				this.TextBlock_SelectedWordCount.DataContext = this.TDI.CurrentEditorRenderer.Editor;
				this.TextBlock_SelectedCharacterCount.DataContext = this.TDI.CurrentEditorRenderer.Editor;
				this.TextBlock_StatusMessage.DataContext = this.TDI.CurrentEditorRenderer.Editor;
			}
		}
		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!(e.Source is TabControl))
			{
				return;
			}
			this.ConnectStatusBarDataContexts();
		}
		private void CommandBinding_EnableLivePreview(object sender, ExecutedRoutedEventArgs e)
		{
			this._settings.App_LivePreviewEnabled = !this._settings.App_LivePreviewEnabled;
			this.TDI.CurrentEditorRenderer.ToggleLivePreview(this._settings.App_LivePreviewEnabled, this._settings.App_VerticalLayoutEnabled);
			this.UpdateLivePreviewCheckedButtons(this._settings.App_LivePreviewEnabled);
		}
		private void RestoreWindowState()
		{
			double virtualScreenWidth = SystemParameters.VirtualScreenWidth;
			double virtualScreenHeight = SystemParameters.VirtualScreenHeight;
			bool flag = SystemUtilities.IsWindowWithinScreenBounds(this._settings.App_WindowState_Left, this._settings.App_WindowState_Width, this._settings.App_WindowState_Top, this._settings.App_WindowState_Height, virtualScreenWidth, virtualScreenHeight);
			if (this._settings.App_IsFirstRun || !flag)
			{
				base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			}
			else
			{
				base.Top = this._settings.App_WindowState_Top.SanitizeDouble(0.0);
				base.Left = this._settings.App_WindowState_Left.SanitizeDouble(0.0);
				base.Height = this._settings.App_WindowState_Height.SanitizeDouble(768.0);
				base.Width = this._settings.App_WindowState_Width.SanitizeDouble(1024.0);
				base.WindowState = this._settings.App_WindowState;
			}
			if (base.Width >= virtualScreenWidth)
			{
				base.Width *= 0.8;
			}
			if (base.Height >= virtualScreenHeight)
			{
				base.Height *= 0.8;
			}
		}
		private void SaveWindowState()
		{
			if (base.WindowState == WindowState.Maximized)
			{
				bool flag = NumericUtilities.IsInfinityOrNaN(new double[]
				{
					base.RestoreBounds.Top,
					base.RestoreBounds.Left,
					base.RestoreBounds.Height,
					base.RestoreBounds.Width
				});
				if (flag)
				{
					return;
				}
				this._settings.App_WindowState_Top = base.RestoreBounds.Top;
				this._settings.App_WindowState_Left = base.RestoreBounds.Left;
				this._settings.App_WindowState_Height = base.RestoreBounds.Height;
				this._settings.App_WindowState_Width = base.RestoreBounds.Width;
				this._settings.App_WindowState = base.WindowState;
				return;
			}
			else
			{
				bool flag2 = NumericUtilities.IsInfinityOrNaN(new double[]
				{
					base.Top,
					base.Left,
					base.Height,
					base.Width
				});
				if (flag2)
				{
					return;
				}
				this._settings.App_WindowState_Top = base.Top;
				this._settings.App_WindowState_Left = base.Left;
				this._settings.App_WindowState_Height = base.Height;
				this._settings.App_WindowState_Width = base.Width;
				this._settings.App_WindowState = base.WindowState;
				return;
			}
		}
		private void CommandBinding_ToggleBold(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleBold();
		}
		private void CommandBinding_New(object sender, ExecutedRoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_UnlimitedDocuments", false, "MarkdownPadStrings");
			if (this.TDI.NumberOfOpenDocuments >= 4 && !LicenseHelper.ValidateLicense(localizedString, this))
			{
				return;
			}
			this.TDI.AddNewTab("");
		}
		private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e)
		{
			this.OpenFile();
		}
		private void OpenFile()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = FileDialogHelper.OpenMarkdownFileTypes,
				RestoreDirectory = true,
				FilterIndex = this._settings.IO_OpenFile_LastUsedFileTypeIndex,
				Multiselect = true
			};
			openFileDialog.ShowDialog();
			this._settings.IO_OpenFile_LastUsedFileTypeIndex = openFileDialog.FilterIndex;
			foreach (string current in openFileDialog.FileNames.Where(new Func<string, bool>(System.IO.File.Exists)))
			{
				this.LoadDocument(current, false);
			}
		}
		private void SaveFileAs()
		{
			EditorRenderer currentEditorRenderer = this.TDI.CurrentEditorRenderer;
			if (!currentEditorRenderer.SaveAsDocument())
			{
				return;
			}
			this.TDI.CurrentTab.Header = currentEditorRenderer.Title;
			this.TDI.CurrentTab.Tag = currentEditorRenderer.Filename;
			this.AddFileToRecentFileList(currentEditorRenderer.Filename);
		}
		public bool ProcessCommandLineArgs(System.Collections.Generic.IList<string> args)
		{
			if (args == null || args.Count == 0)
			{
				return true;
			}
			if (args.Count > 1)
			{
				this.LoadDocument(args[1], false);
				if (base.WindowState == WindowState.Minimized)
				{
					base.WindowState = WindowState.Normal;
				}
				base.Activate();
			}
			return true;
		}
		private void LoadDocument(string filename, bool allow = false)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_UnlimitedDocuments", false, "MarkdownPadStrings");
			if (!allow && this.TDI.NumberOfOpenDocuments >= 4 && !LicenseHelper.ValidateLicense(localizedString, this))
			{
				return;
			}
			if (!System.IO.File.Exists(filename))
			{
				MainWindow._logger.Warn("Tried opening a file that didn't exist: " + filename);
				if (this._recentFiles.Contains(filename))
				{
					this._recentFiles.Remove(filename);
					this.AddRecentDocumentsToMenu();
				}
				MessageBoxResult messageBoxResult = CustomMessageBox.ShowYesNo(string.Concat(new string[]
				{
					LocalizationProvider.GetLocalizedString("FileNotFoundMessage", true, "MarkdownPadStrings"),
					StringUtilities.GetNewLines(1),
					filename,
					StringUtilities.GetNewLines(2),
					LocalizationProvider.GetLocalizedString("SearchForFile", false, "MarkdownPadStrings")
				}), LocalizationProvider.GetLocalizedString("FileNotFoundTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Yes", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("No", false, "MarkdownPadStrings"));
				if (messageBoxResult == MessageBoxResult.Yes)
				{
					this.OpenFile();
				}
				return;
			}
			this.AddFileToRecentFileList(filename);
			this.TDI.AddNewTab(filename);
		}
		private void AddFileToRecentFileList(string filename)
		{
			if (!System.IO.File.Exists(filename))
			{
				MainWindow._logger.Warn("File not found while trying to add to MRU list: " + filename);
				return;
			}
			if (this._recentFiles.Contains(filename))
			{
				MainWindow._logger.Trace("Adding existing file to top of recent files list: " + filename);
				this._recentFiles.Remove(filename);
				this._recentFiles.Add(filename);
			}
			else
			{
				MainWindow._logger.Trace("Adding new file to recent files list: " + filename);
				this._recentFiles.Add(filename);
				while (this._recentFiles.Count > this._settings.App_MaxRecentFiles)
				{
					MainWindow._logger.Trace("Removing old file from MRU: " + this._recentFiles[0]);
					this._recentFiles.RemoveAt(0);
				}
			}
			this.AddRecentDocumentsToMenu();
		}
		private void CommandBinding_ToggleCode(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleCode();
		}
		private void CommandBinding_ToggleQuotes(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleQuotes();
		}
		private void CommandBinding_ToggleItalic(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleItalic();
		}
		private void CommandBinding_Heading1(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleHeading(HeadingType.Heading1, this._settings.Markdown_UseUnderlineStyleHeadings);
		}
		private void CommandBinding_Heading2(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleHeading(HeadingType.Heading2, this._settings.Markdown_UseUnderlineStyleHeadings);
		}
		private void CommandBinding_Heading3(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleHeading(HeadingType.Heading3, this._settings.Markdown_UseUnderlineStyleHeadings);
		}
		private void CommandBinding_Heading4(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleHeading(HeadingType.Heading4, this._settings.Markdown_UseUnderlineStyleHeadings);
		}
		private void CommandBinding_Heading5(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleHeading(HeadingType.Heading5, this._settings.Markdown_UseUnderlineStyleHeadings);
		}
		private void CommandBinding_Heading6(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ToggleHeading(HeadingType.Heading6, this._settings.Markdown_UseUnderlineStyleHeadings);
		}
		private void CommandBinding_ToggleLayout(object sender, ExecutedRoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_HorizontalLayout", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				return;
			}
			this._settings.App_VerticalLayoutEnabled = !this._settings.App_VerticalLayoutEnabled;
			this.TDI.CurrentEditorRenderer.SetLayout(this._settings.App_VerticalLayoutEnabled);
		}
		private void CanExecute_ToggleLayout(object sender, CanExecuteRoutedEventArgs e)
		{
			if (this._settings.App_LivePreviewEnabled && this.TDI != null && this.TDI.NumberOfOpenDocuments > 0)
			{
				e.CanExecute = true;
				return;
			}
			e.CanExecute = false;
		}
		private void CommandBinding_OpenSettings(object sender, ExecutedRoutedEventArgs e)
		{
			this.ToggleActivationEventSubscription(false);
			OptionsWindow optionsWindow = new OptionsWindow
			{
				Owner = this
			};
			optionsWindow.ShowDialog();
			if (!optionsWindow.SettingsSaved)
			{
				return;
			}
			this.ReloadSettingsInAllEditorRenderers();
			this.SelectQuickMarkdownProcessor(this._settings.Markdown_MarkdownProcessor, false);
			this.ToggleActivationEventSubscription(true);
		}
		private void ToggleActivationEventSubscription(bool enable)
		{
			if (enable)
			{
				base.Activated += new System.EventHandler(this.MainWindow_Activated);
				return;
			}
			base.Activated -= new System.EventHandler(this.MainWindow_Activated);
		}
		private void ReloadSettingsInAllEditorRenderers()
		{
			foreach (TabItem tabItem in (System.Collections.IEnumerable)this.MainTabControl.Items)
			{
				EditorRenderer editorRenderer = tabItem.Content as EditorRenderer;
				if (editorRenderer != null)
				{
					editorRenderer.ReloadSettings();
				}
			}
		}
		private void CommandBinding_ReportBug(object sender, ExecutedRoutedEventArgs e)
		{
			BugReportHelper.ShowBugReport(null, "");
		}
		private void CommandBinding_SyntaxHighlight(object sender, ExecutedRoutedEventArgs e)
		{
			this._settings.Editor_MarkdownSyntaxHighlightingEnabled = !this._settings.Editor_MarkdownSyntaxHighlightingEnabled;
			foreach (TabItem tabItem in (System.Collections.IEnumerable)this.MainTabControl.Items)
			{
				EditorRenderer editorRenderer = (EditorRenderer)tabItem.Content;
				editorRenderer.Editor.ToggleMarkdownSyntaxHighlighting(this._settings.Editor_MarkdownSyntaxHighlightingEnabled);
			}
		}
		private void CommandBinding_LineNumbers(object sender, ExecutedRoutedEventArgs e)
		{
			this._settings.Editor_LineNumbersEnabled = !this._settings.Editor_LineNumbersEnabled;
			foreach (TabItem tabItem in (System.Collections.IEnumerable)this.MainTabControl.Items)
			{
				EditorRenderer editorRenderer = (EditorRenderer)tabItem.Content;
				editorRenderer.Editor.ToggleLineNumbers(this._settings.Editor_LineNumbersEnabled);
			}
		}
		private void CommandBinding_EnableAutoSave(object sender, ExecutedRoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_AutoSave", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				return;
			}
			this._settings.IO_AutoSaveEnabled = !this._settings.IO_AutoSaveEnabled;
			foreach (TabItem tabItem in (System.Collections.IEnumerable)this.MainTabControl.Items)
			{
				EditorRenderer editorRenderer = tabItem.Content as EditorRenderer;
				if (editorRenderer != null)
				{
					editorRenderer.Editor.ToggleAutoSave(this._settings.IO_AutoSaveEnabled);
				}
			}
		}
		private void CanExecute_NeedsOpenDocument(object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.TDI != null && this.TDI.NumberOfOpenDocuments > 0)
			{
				e.CanExecute = true;
				return;
			}
			e.CanExecute = false;
		}
		private void CanExecute_NeedsSelectedText(object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.TDI.CurrentEditorRenderer != null && this.TDI.CurrentEditorRenderer.Editor.SelectionLength > 0)
			{
				e.CanExecute = true;
				return;
			}
			e.CanExecute = false;
		}
		private void CommandBinding_Save(object sender, ExecutedRoutedEventArgs e)
		{
			EditorRenderer currentEditorRenderer = this.TDI.CurrentEditorRenderer;
			if (!currentEditorRenderer.SaveDocument())
			{
				return;
			}
			this.TDI.CurrentTab.Header = currentEditorRenderer.Title;
			this.TDI.CurrentTab.Tag = currentEditorRenderer.Filename;
			this.AddFileToRecentFileList(currentEditorRenderer.Filename);
		}
		private void CommandBinding_Close(object sender, ExecutedRoutedEventArgs e)
		{
			if (this.TDI.CurrentTab != null)
			{
				this.TDI.CloseTab(this.TDI.CurrentTab, false);
			}
		}
		private void CommandBinding_CloseAll_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CloseAllTabs();
		}
		private void CommandBinding_SaveAs(object sender, ExecutedRoutedEventArgs e)
		{
			this.SaveFileAs();
		}
		private void CommandBinding_EnableSpellCheck(object sender, ExecutedRoutedEventArgs e)
		{
			this._settings.Editor_SpellCheckEnabled = !this._settings.Editor_SpellCheckEnabled;
			foreach (TabItem tabItem in (System.Collections.IEnumerable)this.MainTabControl.Items)
			{
				EditorRenderer editorRenderer = (EditorRenderer)tabItem.Content;
				editorRenderer.Editor.ToggleSpellCheck(this._settings.Editor_SpellCheckEnabled);
			}
		}
		private void CommandBinding_ToggleMainToolbar(object sender, ExecutedRoutedEventArgs e)
		{
			this._settings.App_ShowMainToolbar = !this._settings.App_ShowMainToolbar;
			this.ToggleMainToolbarVisibility(this._settings.App_ShowMainToolbar);
		}
		private void CommandBinding_ToggleStatusBar(object sender, ExecutedRoutedEventArgs e)
		{
			this._settings.App_ShowStatusBar = !this._settings.App_ShowStatusBar;
			this.ToggleStatusBarVisibility(this._settings.App_ShowStatusBar);
		}
		private void CommandBinding_SaveAll(object sender, ExecutedRoutedEventArgs e)
		{
			foreach (TabItem tabItem in (System.Collections.IEnumerable)this.MainTabControl.Items)
			{
				EditorRenderer editorRenderer = tabItem.Content as EditorRenderer;
				if (editorRenderer != null && editorRenderer.Editor.HasChanges)
				{
					editorRenderer.SaveDocument();
				}
			}
		}
		private void CanExecute_NeedsMultipleOpenDocuments(object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.TDI != null && this.TDI.NumberOfOpenDocuments > 1)
			{
				e.CanExecute = true;
				return;
			}
			e.CanExecute = false;
		}
		private void CanExecute_SaveFile(object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.TDI != null && this.TDI.NumberOfOpenDocuments > 0 && (this.TDI.CurrentEditorRenderer.Editor.HasChanges || string.IsNullOrEmpty(this.TDI.CurrentEditorRenderer.Filename)))
			{
				e.CanExecute = true;
				return;
			}
			e.CanExecute = false;
		}
		private void CommandBinding_PrintHtml(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.PrintHtml();
		}
		private void CommandBinding_PrintMarkdown(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.PrintMarkdown();
		}
		private void MenuItem_Recent_Click(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = e.OriginalSource as MenuItem;
			if (menuItem == null)
			{
				return;
			}
			string filename = menuItem.Tag.ToString();
			this.LoadDocument(filename, false);
		}
		private void LoadWelcomeDocument()
		{
			this.TDI.CurrentEditorRenderer.Editor.Text = (this.TDI.CurrentEditorRenderer.Editor.OriginalDocument = WelcomeDocument.Document);
		}
		private void MainWindow_OnContentRendered(object sender, System.EventArgs e)
		{
			if (this._settings.App_IsFirstRun)
			{
				this.UnlockProVersionPrompt();
				this.LoadWelcomeDocument();
			}
			if (this._settings.App_SendAnonymousStatistics)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += delegate(object o, DoWorkEventArgs args)
				{
					Stats.SendStats();
				};
				backgroundWorker.RunWorkerAsync();
			}
			this.MainTabControl.SelectionChanged += new SelectionChangedEventHandler(this.tabControl_SelectionChanged);
			this.ConnectStatusBarDataContexts();
			if (this._settings.App_AutoSessionRestore)
			{
				SessionCollection app_Sessions = this._settings.App_Sessions;
				if (app_Sessions != null && app_Sessions.Sessions.Count > 0)
				{
					int index = this._settings.App_Sessions.Sessions.Count - 1;
					this.LoadSession(this._settings.App_Sessions.Sessions[index].Files);
					this._settings.App_Sessions.Sessions.RemoveAt(index);
					this.PopulateSessionRestoreMenuItems();
				}
			}
			this.ToggleActivationEventSubscription(true);
			System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer
			{
				Interval = new System.TimeSpan(0, 0, this._settings.App_DelayBeforeUpdateCheck)
			};
			dispatcherTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
			dispatcherTimer.Start();
		}
		private void UpdateTimer_Tick(object sender, System.EventArgs e)
		{
			System.Windows.Threading.DispatcherTimer dispatcherTimer = sender as System.Windows.Threading.DispatcherTimer;
			dispatcherTimer.Stop();
			MainWindow._logger.Trace("Checking for automatic updates: " + this._settings.App_UpdateFrequency);
			if (this._settings.App_UpdateFrequency == UpdateFrequency.Never)
			{
				return;
			}
			int app_UpdateFrequency = (int)this._settings.App_UpdateFrequency;
			System.DateTime dateTime = this.Updater.LastCheckDate + new System.TimeSpan(app_UpdateFrequency, 0, 0);
			bool flag = dateTime <= System.DateTime.Now;
			MainWindow._logger.Trace(string.Format("Update hours: {0}; Date to update by: {1}; Doing it now? {2}", app_UpdateFrequency, dateTime, flag));
			if (flag)
			{
				MainWindow._logger.Trace("Checking for update...");
				this.Updater.UpdateType = UpdateType.Automatic;
				this.Updater.ForceCheckForUpdate();
				this.Updater.BeforeDownloading += delegate(object sender1, BeforeArgs beforeArgs)
				{
					MainWindow._logger.Trace("An update is available, displaying Updater control");
					this.Updater.Visibility = Visibility.Visible;
				};
				this.Updater.BeforeInstalling += delegate(object o, BeforeArgs args)
				{
					while (this.MainTabControl.SelectedIndex > -1)
					{
						MessageBoxResult messageBoxResult = this.TDI.CloseTab(this.TDI.CurrentTab, false);
						if (messageBoxResult == MessageBoxResult.Cancel)
						{
							args.Cancel = true;
							return;
						}
					}
				};
			}
		}
		private void CommandBinding_ExportHtml(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.ExportHtml();
		}
		private void CommandBinding_ExportPdf(object sender, ExecutedRoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_PdfExport", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				return;
			}
			this.TDI.CurrentEditorRenderer.ExportPdf();
		}
		private void CommandBinding_Quit(object sender, ExecutedRoutedEventArgs e)
		{
			base.Close();
		}
		private void CommandBinding_PreviewMarkdown(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.PreviewMarkdownInBrowser();
		}
		private void PreviewDragFile(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.All;
				e.Handled = true;
				this.ToggleDragAndDropPadVisibility(true);
			}
		}
		private void PreviewDropFile(object sender, DragEventArgs e)
		{
			this.ToggleDragAndDropPadVisibility(false);
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					string extension = System.IO.Path.GetExtension(text);
					if (extension != null && FileExtensions.ImageFileExtensions.Contains(extension.ToLowerInvariant()))
					{
						this.ShowInsertImageDialog();
						return;
					}
					this.LoadDocument(text, false);
				}
			}
		}
		private void Window_PreviewDragLeave(object sender, DragEventArgs e)
		{
			this.ToggleDragAndDropPadVisibility(false);
		}
		private void ToggleDragAndDropPadVisibility(bool makeVisible)
		{
			this.DragAndDrop.Visibility = (makeVisible ? Visibility.Visible : Visibility.Collapsed);
		}
		private void CommandBinding_InsertHyperlink(object sender, ExecutedRoutedEventArgs e)
		{
			ModalDialog modalDialog = new ModalDialog
			{
				Owner = this
			};
			if (!modalDialog.ShowUrlModal(LocalizationProvider.GetLocalizedString("Command_InsertHyperlink", false, "MarkdownPadStrings").Replace("_", ""), LocalizationProvider.GetLocalizedString("Url", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("TitleOptional", false, "MarkdownPadStrings")))
			{
				return;
			}
			this.TDI.CurrentEditorRenderer.Editor.InsertHyperlink(modalDialog.UrlValue, modalDialog.DetailValue, this._settings.Editor_InsertHyperlinksAsFootnotes);
		}
		private void CommandBinding_Find(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.Find();
		}
		private void CommandBinding_FindNext(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.FindNext();
		}
		private void CommandBinding_FindPrevious(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.FindPrevious();
		}
		private void CommandBinding_UnorderedList(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.InsertUnorderedListItem();
		}
		private void CommandBinding_InsertImage(object sender, ExecutedRoutedEventArgs e)
		{
			this.ShowInsertImageDialog();
		}
		private void ShowInsertImageDialog()
		{
			ModalDialog modalDialog = new ModalDialog
			{
				Owner = this
			};
			if (!modalDialog.ShowImageModal(LocalizationProvider.GetLocalizedString("Command_InsertImage", false, "MarkdownPadStrings").Replace("_", ""), LocalizationProvider.GetLocalizedString("ImageUrl", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("AltOptional", false, "MarkdownPadStrings")))
			{
				return;
			}
			this.TDI.CurrentEditorRenderer.Editor.InsertImage(modalDialog.UrlValue, modalDialog.DetailValue);
		}
		private void CommandBinding_InsertHorizontalRule(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.InsertHorizontalRule();
		}
		private void CommandBinding_ToggleDistractionFreeMode(object sender, ExecutedRoutedEventArgs e)
		{
			this.DistractionFreeModeEnabled = !this.DistractionFreeModeEnabled;
			this.ToggleDistractionFreeMode(this.DistractionFreeModeEnabled);
		}
		private void ToggleDistractionFreeMode(bool enable)
		{
			if (enable)
			{
				base.Topmost = true;
				base.WindowState = WindowState.Normal;
				base.WindowStyle = WindowStyle.None;
				base.WindowState = WindowState.Maximized;
				return;
			}
			base.Topmost = false;
			base.WindowState = WindowState.Normal;
			base.WindowStyle = WindowStyle.SingleBorderWindow;
		}
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
		private void CommandBinding_InsertTimestamp(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.InsertTimestamp(this._settings.Editor_TimestampFormat);
		}
		private void CommandBinding_ShowAboutWindow(object sender, ExecutedRoutedEventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow
			{
				Owner = this
			};
			aboutWindow.ShowDialog();
			if (!string.IsNullOrEmpty(aboutWindow.PathToOpen) && System.IO.File.Exists(aboutWindow.PathToOpen))
			{
				this.LoadDocument(aboutWindow.PathToOpen, true);
			}
		}
		private void CommandBinding_OpenMarkdownExtraSyntaxGuide(object sender, ExecutedRoutedEventArgs e)
		{
			Urls.Doc_PhpMarkdownExtra.TryStartDefaultProcess();
		}
		private void CommandBinding_OpenStandardMarkdownSyntaxGuide(object sender, ExecutedRoutedEventArgs e)
		{
			Urls.Doc_MarkdownOfficial.TryStartDefaultProcess();
		}
		private void CommandBinding_OpenGfmSyntaxGuide(object sender, ExecutedRoutedEventArgs e)
		{
			Urls.Doc_Gfm.TryStartDefaultProcess();
		}
		private void CommandBinding_OpenMarkdownPadWebsite(object sender, ExecutedRoutedEventArgs e)
		{
			Urls.MarkdownPad_Home.TryStartDefaultProcess();
		}
		private void CommandBinding_CheckForUpdates(object sender, ExecutedRoutedEventArgs e)
		{
			this.CheckForUpdates();
		}
		private void CheckForUpdates()
		{
			this.Updater.Visibility = Visibility.Visible;
			this.Updater.UpdateType = UpdateType.Automatic;
			this.Updater.ForceCheckForUpdate();
		}
		private void CommandBinding_ConvertToLowercase(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ConvertToLowercase();
		}
		private void CommandBinding_ConvertToUppercase(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.ConvertToUppercase();
		}
		private void CommandBinding_OrderedList(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.InsertOrderedListItem();
		}
		private void CommandBinding_GoToLine(object sender, ExecutedRoutedEventArgs e)
		{
			MarkdownEditor editor = this.TDI.CurrentEditorRenderer.Editor;
			GoToLine goToLine = new GoToLine
			{
				CurrentLine = editor.CurrentLine.LineNumber,
				MaxLines = editor.TotalNumberOfLines,
				Owner = this
			};
			goToLine.ShowDialog();
			if (!goToLine.IsSet)
			{
				return;
			}
			this.TDI.CurrentEditorRenderer.Editor.GoToLine(goToLine.TargetLine);
		}
		private void CommandBinding_Cut(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.Cut();
		}
		private void CommandBinding_Copy(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.Copy();
		}
		private void CommandBinding_Paste(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.Paste();
		}
		private void CommandBinding_Delete(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.Delete();
		}
		private void CommandBinding_SelectAll(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.SelectAll();
		}
		private void CanExecute_NeedsOpenDocumentAndClipboardText(object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.TDI != null && this.TDI.NumberOfOpenDocuments > 0 && Clipboard.ContainsText())
			{
				e.CanExecute = true;
				return;
			}
			e.CanExecute = false;
		}
		private void CanExecute_NeedsTextToUndo(object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.TDI != null && this.TDI.NumberOfOpenDocuments > 0 && this.TDI.CurrentEditorRenderer.Editor.Editor.Document.UndoStack.CanUndo)
			{
				e.CanExecute = true;
				return;
			}
			e.CanExecute = false;
		}
		private void CanExecute_NeedsTextToRedo(object sender, CanExecuteRoutedEventArgs e)
		{
			if (this.TDI != null && this.TDI.NumberOfOpenDocuments > 0 && this.TDI.CurrentEditorRenderer.Editor.Editor.Document.UndoStack.CanRedo)
			{
				e.CanExecute = true;
				return;
			}
			e.CanExecute = false;
		}
		private void CommandBinding_Undo(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.Undo();
		}
		private void CommandBinding_Redo(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.Redo();
		}
		private void CommandBinding_UnlockProVersion(object sender, ExecutedRoutedEventArgs e)
		{
			this.UnlockProVersionPrompt();
		}
		private void UnlockProVersionPrompt()
		{
			UpgradeProWindow upgradeProWindow = new UpgradeProWindow
			{
				Owner = this
			};
			upgradeProWindow.ShowDialog();
			this.CheckLicenseAndModifyMenuItems();
		}
		private void CheckLicenseAndModifyMenuItems()
		{
			LicenseEngine licenseEngine = new LicenseEngine();
			bool flag = licenseEngine.VerifyLicense(this._settings.App_LicenseKey, this._settings.App_LicenseEmail);
			if (flag)
			{
				this.MenuItem_UpgradePro.Visibility = Visibility.Collapsed;
				this.MenuItem_ProUnlocked.Visibility = Visibility.Visible;
				return;
			}
			this.MenuItem_UpgradePro.Visibility = Visibility.Visible;
			this.MenuItem_ProUnlocked.Visibility = Visibility.Collapsed;
		}
		private void CommandBinding_CloseMiddleClick(object sender, ExecutedRoutedEventArgs e)
		{
			object selectedItem = this.MainTabControl.SelectedItem;
			for (int i = 0; i < this.MainTabControl.Items.Count; i++)
			{
				TabItem tabItem = this.MainTabControl.Items.GetItemAt(i) as TabItem;
				if (tabItem == null || tabItem.IsMouseOver)
				{
					this.TDI.CloseTab(tabItem, false);
					if (this.MainTabControl.Items.Contains(selectedItem))
					{
						this.MainTabControl.SelectedItem = selectedItem;
					}
					return;
				}
			}
		}
		private void CommandBinding_MoveTab(object sender, ExecutedRoutedEventArgs e)
		{
			this.MainTabControl.MoveTab(this);
		}
		private void CommandBinding_HelpTranslateMarkdownPad(object sender, ExecutedRoutedEventArgs e)
		{
			Urls.MarkdownPad_Translate.TryStartDefaultProcess();
		}
		private void MenuItem_SessionManager_OnClick(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = e.OriginalSource as MenuItem;
			if (menuItem == null)
			{
				return;
			}
			if (menuItem.Tag == null)
			{
				return;
			}
			System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
			list.AddRange((System.Collections.Generic.IEnumerable<string>)menuItem.Tag);
			this.LoadSession(list);
		}
		private void LoadSession(System.Collections.Generic.ICollection<string> session)
		{
			MainWindow._logger.Trace("Loading previous session files: " + session.Count);
			foreach (string current in session)
			{
				this.LoadDocument(current, false);
			}
		}
		private void CommandBinding_ToggleAutoSessionRestore(object sender, ExecutedRoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_AutoSessionRestore", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				e.Handled = true;
				return;
			}
			this._settings.App_AutoSessionRestore = !this._settings.App_AutoSessionRestore;
		}
		private void CommandBinding_OpenMarkdownPadSupportWebsite(object sender, ExecutedRoutedEventArgs e)
		{
			Urls.MarkdownPad_Support.TryStartDefaultProcess();
		}
		private void MenuItem_MarkdownPadProUnlocked_OnClick(object sender, RoutedEventArgs e)
		{
			LicenseWindow licenseWindow = new LicenseWindow
			{
				Owner = this
			};
			licenseWindow.ShowLicenseDialog();
			this.CheckLicenseAndModifyMenuItems();
		}
		private void CommandBinding_SetColumnGuide(object sender, ExecutedRoutedEventArgs e)
		{
			ColumnGuideWindow columnGuideWindow = new ColumnGuideWindow
			{
				Owner = this
			};
			columnGuideWindow.ShowDialog();
			foreach (TabItem tabItem in (System.Collections.IEnumerable)this.MainTabControl.Items)
			{
				EditorRenderer editorRenderer = tabItem.Content as EditorRenderer;
				if (editorRenderer != null)
				{
					editorRenderer.Editor.SetColumnGuide(this._settings.Editor_ShowColumnGuide, this._settings.CodeEditor_ColumnGuidePosition);
				}
			}
		}
		private void CommandBinding_WordWrap(object sender, ExecutedRoutedEventArgs e)
		{
			this._settings.Editor_WordWrap = !this._settings.Editor_WordWrap;
			foreach (TabItem tabItem in (System.Collections.IEnumerable)this.MainTabControl.Items)
			{
				EditorRenderer editorRenderer = tabItem.Content as EditorRenderer;
				if (editorRenderer != null)
				{
					editorRenderer.Editor.SetWordWrap(this._settings.Editor_WordWrap);
				}
			}
		}
		private void CommandBinding_OpenFileInExplorer(object sender, ExecutedRoutedEventArgs e)
		{
			TabItem tabItemUnderMouse = this.MainTabControl.GetTabItemUnderMouse();
			if (tabItemUnderMouse == null)
			{
				return;
			}
			MainWindow._logger.Trace("Open file location initiated for tab with index: " + this.MainTabControl.Items.IndexOf(tabItemUnderMouse));
			string text = tabItemUnderMouse.Tag.ToString();
			if (!string.IsNullOrEmpty(text) && System.IO.File.Exists(text))
			{
				text.TryStartSpecificProcess("explorer.exe", "/select, ");
			}
		}
		private void CommandBinding_CopyLivePreviewContent(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.CopyLivePreviewContent();
		}
		private void CommandBinding_CopyDocumentAsHtml(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.CurrentEditorRenderer.Editor.CopyDocumentAsHtml();
		}
		private void CommandBinding_NewWindow(object sender, ExecutedRoutedEventArgs e)
		{
			new MainWindow
			{
				WindowStartupLocation = WindowStartupLocation.CenterScreen
			}.Show();
		}
		private void CommandBinding_CopyFilePath(object sender, ExecutedRoutedEventArgs e)
		{
			TabItem tabItemUnderMouse = this.MainTabControl.GetTabItemUnderMouse();
			if (tabItemUnderMouse == null)
			{
				return;
			}
			string text = tabItemUnderMouse.Tag.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				MainWindow._logger.Trace("Copying file path to clipboard: " + text);
				SystemUtilities.CopyStringToClipboard(text);
			}
		}
		private void CommandBinding_ReloadFromDisk(object sender, ExecutedRoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(this.TDI.CurrentEditorRenderer.Filename))
			{
				return;
			}
			bool hasChanges = this.TDI.CurrentEditorRenderer.Editor.HasChanges;
			if (hasChanges)
			{
				MessageBoxResult messageBoxResult = CustomMessageBox.ShowOKCancel(LocalizationProvider.GetLocalizedString("ManualReload_UnsavedChanges_Message", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("SaveChangesTitle", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("FileModified_Reload", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Cancel", false, "MarkdownPadStrings"), MessageBoxImage.Exclamation);
				if (messageBoxResult == MessageBoxResult.Cancel)
				{
					return;
				}
			}
			this.TDI.CurrentEditorRenderer.Editor.ReloadCurrentDocument();
		}
		private void CommandBinding_OpenFileInNewWindow(object sender, ExecutedRoutedEventArgs e)
		{
			TabItem tabItemUnderMouse = this.MainTabControl.GetTabItemUnderMouse();
			if (tabItemUnderMouse == null)
			{
				return;
			}
			string text = tabItemUnderMouse.Tag.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				MainWindow._logger.Trace("Opening file in new window: " + text);
				this.TDI.CloseTab(tabItemUnderMouse, false);
				MainWindow mainWindow = new MainWindow();
				mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				mainWindow.LoadDocument(text, false);
				mainWindow.Show();
			}
		}
		private void MenuItem_QuickMarkdownSettingsPlaceholder_OnClick(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = sender as MenuItem;
			menuItem.Visibility = Visibility.Collapsed;
			this.ComboBox_QuickMarkdownSettings.Visibility = Visibility.Visible;
			this.ComboBox_QuickMarkdownSettings.IsDropDownOpen = true;
			System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer
			{
				Interval = System.TimeSpan.FromSeconds(10.0)
			};
			timer.Tick += delegate(object o, System.EventArgs args)
			{
				if (!this.ComboBox_QuickMarkdownSettings.IsMouseOver)
				{
					this.Dispatcher.BeginInvoke(delegate
					{
						this.ComboBox_QuickMarkdownSettings.Visibility = Visibility.Collapsed;
						menuItem.Visibility = Visibility.Visible;
					}, System.Windows.Threading.DispatcherPriority.Normal, new object[0]);
					timer.Stop();
				}
			};
			timer.Start();
		}
		private void CommandBinding_OpenWelcomeDocument(object sender, ExecutedRoutedEventArgs e)
		{
			this.TDI.AddNewTab("");
			this.LoadWelcomeDocument();
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/ui/mainwindow.xaml", UriKind.Relative);
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
				((MainWindow)target).Loaded += new RoutedEventHandler(this.Window_Loaded);
				((MainWindow)target).Closing += new CancelEventHandler(this.Window_Closing);
				((MainWindow)target).Initialized += new System.EventHandler(this.Window_Initialized);
				((MainWindow)target).ContentRendered += new System.EventHandler(this.MainWindow_OnContentRendered);
				((MainWindow)target).PreviewDragEnter += new DragEventHandler(this.PreviewDragFile);
				((MainWindow)target).PreviewDragOver += new DragEventHandler(this.PreviewDragFile);
				((MainWindow)target).PreviewDrop += new DragEventHandler(this.PreviewDropFile);
				((MainWindow)target).PreviewDragLeave += new DragEventHandler(this.Window_PreviewDragLeave);
				return;
			case 2:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_CloseAll_Executed);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 3:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ToggleBold);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 4:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ToggleItalic);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 5:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ToggleCode);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 6:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ToggleQuotes);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 7:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Heading1);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 8:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Heading2);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 9:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Heading3);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 10:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Heading4);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 11:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Heading5);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 12:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Heading6);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 13:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_UnorderedList);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 14:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OrderedList);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 15:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_New);
				return;
			case 16:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_NewWindow);
				return;
			case 17:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Open);
				return;
			case 18:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ReloadFromDisk);
				return;
			case 19:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_SaveFile);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Save);
				return;
			case 20:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_SaveAs);
				return;
			case 21:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsSelectedText);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Cut);
				return;
			case 22:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsSelectedText);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Copy);
				return;
			case 23:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_CopyLivePreviewContent);
				return;
			case 24:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_CopyDocumentAsHtml);
				return;
			case 25:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocumentAndClipboardText);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Paste);
				return;
			case 26:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsSelectedText);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Delete);
				return;
			case 27:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_SelectAll);
				return;
			case 28:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsMultipleOpenDocuments);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_MoveTab);
				return;
			case 29:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OpenFileInExplorer);
				return;
			case 30:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_CopyFilePath);
				return;
			case 31:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OpenFileInNewWindow);
				return;
			case 32:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsTextToUndo);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Undo);
				return;
			case 33:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsTextToRedo);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Redo);
				return;
			case 34:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsMultipleOpenDocuments);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_SaveAll);
				return;
			case 35:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_PrintHtml);
				return;
			case 36:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_PrintMarkdown);
				return;
			case 37:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ExportHtml);
				return;
			case 38:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ExportPdf);
				return;
			case 39:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_PreviewMarkdown);
				return;
			case 40:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_InsertHyperlink);
				return;
			case 41:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_InsertImage);
				return;
			case 42:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_InsertHorizontalRule);
				return;
			case 43:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Close);
				return;
			case 44:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Close);
				return;
			case 45:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_CloseMiddleClick);
				return;
			case 46:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Quit);
				return;
			case 47:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_Find);
				return;
			case 48:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_FindNext);
				return;
			case 49:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_FindPrevious);
				return;
			case 50:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_EnableAutoSave);
				return;
			case 51:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ToggleDistractionFreeMode);
				return;
			case 52:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_InsertTimestamp);
				return;
			case 53:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ShowAboutWindow);
				return;
			case 54:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OpenMarkdownExtraSyntaxGuide);
				return;
			case 55:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OpenStandardMarkdownSyntaxGuide);
				return;
			case 56:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OpenGfmSyntaxGuide);
				return;
			case 57:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OpenMarkdownPadWebsite);
				return;
			case 58:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OpenMarkdownPadSupportWebsite);
				return;
			case 59:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OpenWelcomeDocument);
				return;
			case 60:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_CheckForUpdates);
				return;
			case 61:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsSelectedText);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ConvertToLowercase);
				return;
			case 62:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsSelectedText);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ConvertToUppercase);
				return;
			case 63:
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_GoToLine);
				return;
			case 64:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_UnlockProVersion);
				return;
			case 65:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_HelpTranslateMarkdownPad);
				return;
			case 66:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ToggleAutoSessionRestore);
				return;
			case 67:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ToggleLayout);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_ToggleLayout);
				return;
			case 68:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_OpenSettings);
				return;
			case 69:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ReportBug);
				return;
			case 70:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_SyntaxHighlight);
				return;
			case 71:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_LineNumbers);
				return;
			case 72:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_WordWrap);
				return;
			case 73:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_EnableSpellCheck);
				return;
			case 74:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_EnableLivePreview);
				((CommandBinding)target).CanExecute += new CanExecuteRoutedEventHandler(this.CanExecute_NeedsOpenDocument);
				return;
			case 75:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ToggleMainToolbar);
				return;
			case 76:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_ToggleStatusBar);
				return;
			case 77:
				((CommandBinding)target).Executed += new ExecutedRoutedEventHandler(this.CommandBinding_SetColumnGuide);
				return;
			case 78:
				this.Grid_Main = (Grid)target;
				return;
			case 79:
				this.MenuItem_Recent = (MenuItem)target;
				this.MenuItem_Recent.Click += new RoutedEventHandler(this.MenuItem_Recent_Click);
				return;
			case 80:
				this.MenuItem_LivePreview = (MenuItem)target;
				return;
			case 81:
				this.MenuItem_Layout = (MenuItem)target;
				return;
			case 82:
				this.MenuItem_DisplayLineNumbers = (MenuItem)target;
				return;
			case 83:
				this.MenuItem_WordWrap = (MenuItem)target;
				return;
			case 84:
				this.MenuItem_MarkdownSyntax = (MenuItem)target;
				return;
			case 85:
				this.MenuItem_DisplayToolbar = (MenuItem)target;
				return;
			case 86:
				this.MenuItem_DisplayStatusBar = (MenuItem)target;
				return;
			case 87:
				this.MenuItem_SessionManager = (MenuItem)target;
				this.MenuItem_SessionManager.Click += new RoutedEventHandler(this.MenuItem_SessionManager_OnClick);
				return;
			case 88:
				this.MenuItem_UpgradePro = (MenuItem)target;
				return;
			case 89:
				this.MenuItem_ProUnlocked = (MenuItem)target;
				this.MenuItem_ProUnlocked.Click += new RoutedEventHandler(this.MenuItem_MarkdownPadProUnlocked_OnClick);
				return;
			case 90:
				this.Toolbar_Main = (Toolbar)target;
				return;
			case 91:
				this.DragAndDrop = (DragAndDrop)target;
				return;
			case 92:
				this.TDI = (TabbedDocumentInterface)target;
				return;
			case 93:
				this.StatusBar_Main = (StatusBar)target;
				return;
			case 94:
				this.MenuItem_QuickMarkdownSettingsPlaceholder = (MenuItem)target;
				this.MenuItem_QuickMarkdownSettingsPlaceholder.Click += new RoutedEventHandler(this.MenuItem_QuickMarkdownSettingsPlaceholder_OnClick);
				return;
			case 95:
				this.Image_QuickMarkdownSettings = (Image)target;
				return;
			case 96:
				this.ComboBox_QuickMarkdownSettings = (ComboBox)target;
				return;
			case 97:
				this.MenuItem_SpellCheck = (MenuItem)target;
				return;
			case 98:
				this.MenuItem_SpellCheckEnabled = (MenuItem)target;
				return;
			case 99:
				this.StatusBarItem_WordCount = (StatusBarItem)target;
				return;
			case 100:
				this.TextBlock_WordCount = (TextBlock)target;
				return;
			case 101:
				this.StatusBarItem_SelectedWordCount = (StatusBarItem)target;
				return;
			case 102:
				this.TextBlock_SelectedWordCount = (TextBlock)target;
				return;
			case 103:
				this.StatusBarItem_CharacterCount = (StatusBarItem)target;
				return;
			case 104:
				this.TextBlock_CharacterCount = (TextBlock)target;
				return;
			case 105:
				this.StatusBarItem_SelectedCharacterCount = (StatusBarItem)target;
				return;
			case 106:
				this.TextBlock_SelectedCharacterCount = (TextBlock)target;
				return;
			case 107:
				this.TextBlock_StatusMessage = (TextBlock)target;
				return;
			case 108:
				this.Updater = (AutomaticUpdater)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}
	}
}
