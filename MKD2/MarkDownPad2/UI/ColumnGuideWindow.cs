using MarkdownPad2.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Xceed.Wpf.Toolkit;
namespace MarkdownPad2.UI
{
	public class ColumnGuideWindow : Window, System.Windows.Markup.IComponentConnector
	{
		private Settings _settings = Settings.Default;
		internal CheckBox CheckBox_EnableColumnGuide;
		internal IntegerUpDown IntUpDown_ColumnGuidePosition;
		internal Button Button_OK;
		internal Button Button_Cancel;
		private bool _contentLoaded;
		public ColumnGuideWindow()
		{
			this.InitializeComponent();
		}
		private void ColumnGuideWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			this.CheckBox_EnableColumnGuide.IsChecked = new bool?(this._settings.Editor_ShowColumnGuide);
			this.IntUpDown_ColumnGuidePosition.Text = this._settings.CodeEditor_ColumnGuidePosition.ToString();
		}
		private void Button_OK_OnClick(object sender, RoutedEventArgs e)
		{
			this._settings.Editor_ShowColumnGuide = this.CheckBox_EnableColumnGuide.IsChecked.Value;
			int codeEditor_ColumnGuidePosition = 80;
			int.TryParse(this.IntUpDown_ColumnGuidePosition.Text, out codeEditor_ColumnGuidePosition);
			this._settings.CodeEditor_ColumnGuidePosition = codeEditor_ColumnGuidePosition;
			base.Close();
		}
		private void Button_Cancel_OnClick(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/ui/columnguidewindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((ColumnGuideWindow)target).Loaded += new RoutedEventHandler(this.ColumnGuideWindow_OnLoaded);
				return;
			case 2:
				this.CheckBox_EnableColumnGuide = (CheckBox)target;
				return;
			case 3:
				this.IntUpDown_ColumnGuidePosition = (IntegerUpDown)target;
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
