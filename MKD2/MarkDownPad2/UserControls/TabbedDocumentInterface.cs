using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using MarkdownPad2.Utilities;
using NLog;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using WPFCustomMessageBox;
namespace MarkdownPad2.UserControls
{
	public class TabbedDocumentInterface : UserControl, INotifyPropertyChanged, System.Windows.Markup.IComponentConnector, IStyleConnector
	{
		private Settings _settings = Settings.Default;
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		internal TabControl tabControl;
		private bool _contentLoaded;
		public event PropertyChangedEventHandler PropertyChanged;
		public System.Collections.Generic.List<string> OpenDocuments
		{
			get
			{
				System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
				foreach (TabItem tabItem in (System.Collections.IEnumerable)this.tabControl.Items)
				{
					list.Add(tabItem.Tag.ToString());
				}
				return list;
			}
		}
		public System.Collections.Generic.List<string> OpenDocumentsThatExist
		{
			get
			{
				System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
				foreach (TabItem tabItem in (System.Collections.IEnumerable)this.tabControl.Items)
				{
					EditorRenderer editorRenderer = tabItem.Content as EditorRenderer;
					if (editorRenderer != null && System.IO.File.Exists(editorRenderer.Filename))
					{
						list.Add(editorRenderer.Filename);
					}
				}
				return list;
			}
		}
		private ContextMenu TabContextMenu
		{
			get
			{
				ContextMenu contextMenu = new ContextMenu
				{
					VerticalContentAlignment = VerticalAlignment.Center
				};
				MenuItem newItem = new MenuItem
				{
					Command = MarkdownPadCommands.MoveTab
				};
				contextMenu.Items.Add(newItem);
				Separator newItem2 = new Separator();
				contextMenu.Items.Add(newItem2);
				MenuItem newItem3 = new MenuItem
				{
					Command = MarkdownPadCommands.OpenFileInExplorer
				};
				contextMenu.Items.Add(newItem3);
				MenuItem newItem4 = new MenuItem
				{
					Command = MarkdownPadCommands.CopyFilePath
				};
				contextMenu.Items.Add(newItem4);
				MenuItem newItem5 = new MenuItem
				{
					Command = MarkdownPadCommands.OpenFileInNewWindow
				};
				contextMenu.Items.Add(newItem5);
				return contextMenu;
			}
		}
		public TabItem CurrentTab
		{
			get
			{
				return this.tabControl.SelectedItem as TabItem;
			}
		}
		public EditorRenderer CurrentEditorRenderer
		{
			get
			{
				if (this.CurrentTab == null)
				{
					return null;
				}
				return this.CurrentTab.Content as EditorRenderer;
			}
		}
		public int NumberOfOpenDocuments
		{
			get
			{
				return this.tabControl.Items.Count;
			}
		}
		public TabbedDocumentInterface()
		{
			this.InitializeComponent();
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				TabItem tabItem = new TabItem();
				Border content = new Border
				{
					Background = Colors.White.ToBrush()
				};
				tabItem.Header = "Tab";
				tabItem.Content = content;
				this.tabControl.Items.Add(tabItem);
				return;
			}
			this.AddNewTab("");
		}
		public void AddNewTab(string fileToOpenOnLoad = "")
		{
			TabItem tabItem = new TabItem();
			EditorRenderer editorRenderer;
			if (!System.IO.File.Exists(fileToOpenOnLoad))
			{
				editorRenderer = new EditorRenderer("");
			}
			else
			{
				foreach (TabItem tabItem2 in (System.Collections.IEnumerable)this.tabControl.Items)
				{
					string text = tabItem2.Tag.ToString();
					if (text.ToUpper(System.Globalization.CultureInfo.InvariantCulture) == fileToOpenOnLoad.ToUpper(System.Globalization.CultureInfo.InvariantCulture))
					{
						TabbedDocumentInterface._logger.Trace("File already open in editor, focusing: " + fileToOpenOnLoad);
						int selectedIndex = this.tabControl.Items.IndexOf(tabItem2);
						this.tabControl.SelectedIndex = selectedIndex;
						return;
					}
				}
				if (this.NumberOfOpenDocuments > 0 && !this.CurrentEditorRenderer.Editor.HasChanges && string.IsNullOrEmpty(this.CurrentEditorRenderer.Filename) && this.tabControl.SelectedIndex == 0)
				{
					this.CloseTab(this.CurrentTab, false);
				}
				editorRenderer = new EditorRenderer(fileToOpenOnLoad);
			}
			tabItem.Header = editorRenderer.Title;
			tabItem.Content = editorRenderer;
			tabItem.Tag = editorRenderer.Filename;
			this.tabControl.Items.Add(tabItem);
			tabItem.ContextMenu = this.TabContextMenu;
			tabItem.Focus();
			tabItem.DataContext = editorRenderer.Editor;
			this.OnPropertyChanged("NumberOfOpenDocuments");
		}
		private void cmdTabItemCloseButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			TabItem tab = (TabItem)button.TemplatedParent;
			this.CloseTab(tab, false);
		}
		public EditorRenderer GetEditorRenderer(int index)
		{
			if (index < this.NumberOfOpenDocuments)
			{
				TabItem tabItem = (TabItem)this.tabControl.Items[index];
				return (EditorRenderer)tabItem.Content;
			}
			TabbedDocumentInterface._logger.Error(string.Concat(new object[]
			{
				"GetEditorRender index was out of range: ",
				index,
				" Total items: ",
				this.NumberOfOpenDocuments
			}));
			return null;
		}
		public void CloseAllTabs()
		{
			for (int i = this.NumberOfOpenDocuments - 1; i >= 0; i--)
			{
				MessageBoxResult messageBoxResult = this.CloseTab(this.tabControl.Items[i], false);
				if (messageBoxResult == MessageBoxResult.Cancel)
				{
					return;
				}
			}
		}
		public MessageBoxResult CloseTab(object tab, bool openNewDocument = false)
		{
			MessageBoxResult messageBoxResult = MessageBoxResult.No;
			TabItem tabItem = tab as TabItem;
			if (tabItem != null)
			{
				tabItem.Focus();
				EditorRenderer currentEditorRenderer = this.CurrentEditorRenderer;
				if (currentEditorRenderer.Editor.HasChanges)
				{
					messageBoxResult = CustomMessageBox.ShowYesNoCancel(LocalizationProvider.GetLocalizedString("SaveChangesMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("SaveChangesTitle", false, "MarkdownPadStrings") + " - " + currentEditorRenderer.Title, LocalizationProvider.GetLocalizedString("Save", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("DoNotSave", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Cancel", false, "MarkdownPadStrings"), MessageBoxImage.Question);
				}
				MessageBoxResult messageBoxResult2 = messageBoxResult;
				if (messageBoxResult2 != MessageBoxResult.Cancel)
				{
					if (messageBoxResult2 == MessageBoxResult.Yes)
					{
						bool flag = this.CurrentEditorRenderer.SaveDocument();
						if (flag)
						{
							currentEditorRenderer.Dispose();
							this.tabControl.Items.Remove(tab);
						}
					}
					else
					{
						currentEditorRenderer.Dispose();
						this.tabControl.Items.Remove(tab);
					}
				}
				if (openNewDocument && this.tabControl.Items.Count == 0)
				{
					this.AddNewTab("");
				}
			}
			this.OnPropertyChanged("NumberOfOpenDocuments");
			return messageBoxResult;
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
			Uri resourceLocator = new Uri("/MarkdownPad2;component/usercontrols/tabbeddocumentinterface.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 2)
			{
				this.tabControl = (TabControl)target;
				return;
			}
			this._contentLoaded = true;
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId != 1)
			{
				return;
			}
			((Button)target).Click += new RoutedEventHandler(this.cmdTabItemCloseButton_Click);
		}
	}
}
