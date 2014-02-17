using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
namespace MarkdownPad2.UI
{
	public class GoToLine : Window, System.Windows.Markup.IComponentConnector
	{
		private int _targetLine;
		private int _currentLine;
		private int _maxLines;
		internal TextBlock TextBlock_CurrentLine;
		internal TextBlock TextBlock_LinesInDocument;
		internal TextBox TextBox_TargetLine;
		internal Button Button_OK;
		internal Button Button_Cancel;
		private bool _contentLoaded;
		public bool IsSet
		{
			get;
			private set;
		}
		public int TargetLine
		{
			get
			{
				if (this._targetLine < 1)
				{
					return 1;
				}
				return this._targetLine;
			}
			set
			{
				if (value > this.MaxLines)
				{
					this._targetLine = this.MaxLines;
					return;
				}
				if (value < 1)
				{
					this._targetLine = 1;
					return;
				}
				this._targetLine = value;
			}
		}
		public int CurrentLine
		{
			get
			{
				return this._currentLine;
			}
			set
			{
				this._currentLine = value;
				this.TextBlock_CurrentLine.Text = this._currentLine.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
		}
		public int MaxLines
		{
			get
			{
				return this._maxLines;
			}
			set
			{
				this._maxLines = value;
				this.TextBlock_LinesInDocument.Text = this._maxLines.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
		}
		public GoToLine()
		{
			this.InitializeComponent();
			this.TextBox_TargetLine.Focus();
		}
		private void Button_OK_OnClick(object sender, RoutedEventArgs e)
		{
			this.SaveTargetAndClose();
		}
		private void SaveTargetAndClose()
		{
			int targetLine = 0;
			int.TryParse(this.TextBox_TargetLine.Text, out targetLine);
			this.TargetLine = targetLine;
			this.IsSet = true;
			base.Close();
		}
		private void Button_Cancel_OnClick(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
		private void TextBox_TargetLine_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Return)
			{
				this.SaveTargetAndClose();
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
			Uri resourceLocator = new Uri("/MarkdownPad2;component/ui/gotoline.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.TextBlock_CurrentLine = (TextBlock)target;
				return;
			case 2:
				this.TextBlock_LinesInDocument = (TextBlock)target;
				return;
			case 3:
				this.TextBox_TargetLine = (TextBox)target;
				this.TextBox_TargetLine.KeyDown += new KeyEventHandler(this.TextBox_TargetLine_OnKeyDown);
				return;
			case 4:
				this.Button_OK = (Button)target;
				this.Button_OK.Click += new RoutedEventHandler(this.Button_OK_OnClick);
				return;
			case 5:
				this.Button_Cancel = (Button)target;
				this.Button_Cancel.Click += new RoutedEventHandler(this.Button_Cancel_OnClick);
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}
	}
}
