using ICSharpCode.AvalonEdit;
using System;
using System.Windows.Input;
namespace MarkdownPad2.EditorResources
{
	public class EditorPreviewKeyDownEvent
	{
		public TextEditor Editor
		{
			get;
			private set;
		}
		public KeyEventArgs Args
		{
			get;
			private set;
		}
		public EditorPreviewKeyDownEvent(TextEditor editor, KeyEventArgs args)
		{
			this.Editor = editor;
			this.Args = args;
		}
	}
}
