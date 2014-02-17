using ICSharpCode.AvalonEdit;
using MarkdownPad2.Editor;
using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
namespace MarkdownPad2.EditorResources
{
	public class AutoContinueLists
	{
		private readonly Regex _unorderedListRegex = new Regex("^[\\s]*[-\\*\\+][\\s]+", RegexOptions.Compiled);
		private readonly Regex _orderedListRegex = new Regex("^[\\s]*[0-9]+[.][\\s]+", RegexOptions.Compiled);
		private readonly Regex _blockquoteRegex = new Regex("^[\\s]*[>][\\s]*", RegexOptions.Compiled);
		public void Handle(EditorPreviewKeyDownEvent e)
		{
			if (Keyboard.Modifiers != System.Windows.Input.ModifierKeys.None)
			{
				return;
			}
			if (e.Args.Key != System.Windows.Input.Key.Return)
			{
				return;
			}
			bool handled = false || this.HandleUnorderedList(e.Editor) || this.HandleOrderedList(e.Editor) || this.HandleBlockquote(e.Editor);
			e.Args.Handled = handled;
		}
		private bool HandleUnorderedList(TextEditor editor)
		{
			Match match = this.MatchUnorderedList(editor.GetTextLeftOfCursor());
			if (!match.Success)
			{
				return false;
			}
			if (match.Value == editor.GetTextLeftOfCursor() && editor.IsCaratAtEndOfLine())
			{
				AutoContinueLists.EndList(editor);
			}
			else
			{
				editor.TextArea.Selection.ReplaceSelectionWithText(System.Environment.NewLine + match.Value);
			}
			return true;
		}
		public Match MatchUnorderedList(string text)
		{
			return this._unorderedListRegex.Match(text);
		}
		private bool HandleOrderedList(TextEditor editor)
		{
			Match match = this.MatchOrderedList(editor.GetTextLeftOfCursor());
			if (!match.Success)
			{
				return false;
			}
			if (match.Value == editor.GetTextLeftOfCursor() && editor.IsCaratAtEndOfLine())
			{
				AutoContinueLists.EndList(editor);
			}
			else
			{
				int num = 1;
				string text = match.Value.Replace(".", "").Trim();
				bool flag = int.TryParse(text, out num);
				string str = match.Value;
				if (flag)
				{
					str = match.Value.Replace(text, (num + 1).ToString());
				}
				editor.TextArea.Selection.ReplaceSelectionWithText(System.Environment.NewLine + str);
			}
			return true;
		}
		public Match MatchOrderedList(string text)
		{
			return this._orderedListRegex.Match(text);
		}
		private bool HandleBlockquote(TextEditor editor)
		{
			Match match = this.MatchBlockQuote(editor.GetTextLeftOfCursor());
			if (!match.Success)
			{
				return false;
			}
			if (match.Value == editor.GetTextLeftOfCursor() && editor.IsCaratAtEndOfLine())
			{
				AutoContinueLists.EndList(editor);
			}
			else
			{
				editor.TextArea.Selection.ReplaceSelectionWithText(System.Environment.NewLine + match.Value);
			}
			return true;
		}
		public Match MatchBlockQuote(string text)
		{
			return this._blockquoteRegex.Match(text);
		}
		private static void EndList(TextEditor editor)
		{
			int offset = editor.TextArea.Caret.Offset;
			editor.SelectionStart = editor.GetCurrentLine().Offset;
			editor.SelectionLength = offset - editor.GetCurrentLine().Offset;
			editor.TextArea.Selection.ReplaceSelectionWithText(System.Environment.NewLine);
		}
	}
}
