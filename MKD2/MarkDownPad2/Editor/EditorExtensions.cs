using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System;
namespace MarkdownPad2.Editor
{
	public static class EditorExtensions
	{
		public static bool IsCaratAtEndOfLine(this TextEditor editor)
		{
			return editor.GetCurrentLine().EndOffset == editor.TextArea.Caret.Offset;
		}
		public static DocumentLine GetCurrentLine(this TextEditor editor)
		{
			return editor.Document.GetLineByOffset(editor.TextArea.Caret.Offset);
		}
		public static string GetTextLeftOfCursor(this TextEditor editor)
		{
			DocumentLine currentLine = editor.GetCurrentLine();
			return editor.Document.GetText(currentLine.Offset, editor.CaretOffset - currentLine.Offset);
		}
	}
}
