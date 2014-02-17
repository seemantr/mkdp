using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
namespace MarkdownPad2.UserControls
{
	public class Toolbar : UserControl, System.Windows.Markup.IComponentConnector
	{
		internal ToolBarTray ToolBarTray_Main;
		internal ToolBar ToolBar_Controls;
		internal Button Button_New;
		internal Button Button_Open;
		internal Button Button_Save;
		internal Button Button_SaveAll;
		internal Button Button_Cut;
		internal Button Button_Copy;
		internal Button Button_Paste;
		internal Button Button_Delete;
		internal Button Button_Undo;
		internal Button Button_Redo;
		internal Button Button_Bold;
		internal Button Button_Italic;
		internal Button Button_Quote;
		internal Button Button_Code;
		internal Button Button_H1;
		internal Button Button_H2;
		internal Button Button_Hyperlink;
		internal Button Button_Image;
		internal Button Button_UnorderedList;
		internal Button Button_OrderedList;
		internal Button Button_HorizontalRule;
		internal Button Button_Uppercase;
		internal Button Button_Lowercase;
		internal Button Button_Timestamp;
		internal Button Button_ToggleLayout;
		internal ToggleButton Button_LivePreview;
		internal Button Button_PreviewMarkdown;
		private bool _contentLoaded;
		public Toolbar()
		{
			this.InitializeComponent();
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/usercontrols/toolbar.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.ToolBarTray_Main = (ToolBarTray)target;
				return;
			case 2:
				this.ToolBar_Controls = (ToolBar)target;
				return;
			case 3:
				this.Button_New = (Button)target;
				return;
			case 4:
				this.Button_Open = (Button)target;
				return;
			case 5:
				this.Button_Save = (Button)target;
				return;
			case 6:
				this.Button_SaveAll = (Button)target;
				return;
			case 7:
				this.Button_Cut = (Button)target;
				return;
			case 8:
				this.Button_Copy = (Button)target;
				return;
			case 9:
				this.Button_Paste = (Button)target;
				return;
			case 10:
				this.Button_Delete = (Button)target;
				return;
			case 11:
				this.Button_Undo = (Button)target;
				return;
			case 12:
				this.Button_Redo = (Button)target;
				return;
			case 13:
				this.Button_Bold = (Button)target;
				return;
			case 14:
				this.Button_Italic = (Button)target;
				return;
			case 15:
				this.Button_Quote = (Button)target;
				return;
			case 16:
				this.Button_Code = (Button)target;
				return;
			case 17:
				this.Button_H1 = (Button)target;
				return;
			case 18:
				this.Button_H2 = (Button)target;
				return;
			case 19:
				this.Button_Hyperlink = (Button)target;
				return;
			case 20:
				this.Button_Image = (Button)target;
				return;
			case 21:
				this.Button_UnorderedList = (Button)target;
				return;
			case 22:
				this.Button_OrderedList = (Button)target;
				return;
			case 23:
				this.Button_HorizontalRule = (Button)target;
				return;
			case 24:
				this.Button_Uppercase = (Button)target;
				return;
			case 25:
				this.Button_Lowercase = (Button)target;
				return;
			case 26:
				this.Button_Timestamp = (Button)target;
				return;
			case 27:
				this.Button_ToggleLayout = (Button)target;
				return;
			case 28:
				this.Button_LivePreview = (ToggleButton)target;
				return;
			case 29:
				this.Button_PreviewMarkdown = (Button)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}
	}
}
