using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
namespace MarkdownPad2.UI
{
	public class MoveTabWindow : Window, System.Windows.Markup.IComponentConnector
	{
		internal ComboBox ComboBox_TabPositions;
		internal Button Button_OK;
		internal Button Button_Cancel;
		private bool _contentLoaded;
		public int? NewIndex
		{
			get;
			private set;
		}
		private int OriginalIndex
		{
			get;
			set;
		}
		public MoveTabWindow(int totalItems, int selectedIndex)
		{
			this.InitializeComponent();
			this.OriginalIndex = selectedIndex;
			System.Collections.Generic.Dictionary<int, string> dictionary = new System.Collections.Generic.Dictionary<int, string>();
			for (int i = 0; i < totalItems; i++)
			{
				string text = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
				if (i == this.OriginalIndex)
				{
					text += " (current)";
				}
				dictionary.Add(i, text);
			}
			this.ComboBox_TabPositions.ItemsSource = dictionary;
			this.ComboBox_TabPositions.DisplayMemberPath = "Value";
			this.ComboBox_TabPositions.SelectedValuePath = "Key";
			this.ComboBox_TabPositions.SelectedIndex = this.OriginalIndex;
		}
		private void Button_OK_Click(object sender, RoutedEventArgs e)
		{
			if (this.ComboBox_TabPositions.SelectedIndex == this.OriginalIndex)
			{
				this.NewIndex = null;
			}
			else
			{
				this.NewIndex = new int?(this.ComboBox_TabPositions.SelectedIndex);
			}
			base.Close();
		}
		private void Button_Cancel_Click(object sender, RoutedEventArgs e)
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
			Uri resourceLocator = new Uri("/MarkdownPad2;component/ui/movetabwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.ComboBox_TabPositions = (ComboBox)target;
				return;
			case 2:
				this.Button_OK = (Button)target;
				this.Button_OK.Click += new RoutedEventHandler(this.Button_OK_Click);
				return;
			case 3:
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
