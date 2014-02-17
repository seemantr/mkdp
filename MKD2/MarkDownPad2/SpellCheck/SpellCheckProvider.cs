using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace MarkdownPad2.SpellCheck
{
	public class SpellCheckProvider
	{
		private readonly Regex wordSeparatorRegex = new Regex("-[^\\w]+|^'[^\\w]+|[^\\w]+'[^\\w]+|[^\\w]+-[^\\w]+|[^\\w]+'$|[^\\w]+-$|^-$|^'$|[^\\w'-]", RegexOptions.Compiled);
		private readonly Regex uriFinderRegex = new Regex("(http|ftp|https|mailto):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?", RegexOptions.Compiled);
		private readonly SpellingService spellingService;
		private readonly SpellCheckBackgroundRenderer spellCheckRenderer;
		private TextView view;
		public SpellCheckProvider(SpellingService spellingService)
		{
			this.spellingService = spellingService;
			this.spellCheckRenderer = new SpellCheckBackgroundRenderer();
		}
		public void Connect(TextView textView)
		{
			this.view = textView;
			textView.BackgroundRenderers.Add(this.spellCheckRenderer);
		}
		public void Disconnect()
		{
			if (this.view == null)
			{
				return;
			}
			this.ClearSpellCheckErrors();
			this.view.BackgroundRenderers.Remove(this.spellCheckRenderer);
			this.view = null;
		}
		public void DoSpellCheck()
		{
			if (this.view == null)
			{
				return;
			}
			if (!this.view.VisualLinesValid)
			{
				return;
			}
			this.spellCheckRenderer.ErrorSegments.Clear();
			System.Collections.Generic.IEnumerable<VisualLine> enumerable = this.view.VisualLines.AsParallel<VisualLine>();
			foreach (VisualLine current in enumerable)
			{
				int startIndex = 0;
				string text = this.view.Document.GetText(current.FirstDocumentLine.Offset, current.LastDocumentLine.EndOffset - current.FirstDocumentLine.Offset);
				if (!string.IsNullOrEmpty(text))
				{
					text = Regex.Replace(text, "[\\u2018\\u2019\\u201A\\u201B\\u2032\\u2035]", "'");
					string input = this.uriFinderRegex.Replace(text, "");
					System.Collections.Generic.IEnumerable<string> enumerable2 = 
						from s in this.wordSeparatorRegex.Split(input)
						where !string.IsNullOrEmpty(s)
						select s;
					foreach (string current2 in enumerable2)
					{
						string text2 = current2.Trim(new char[]
						{
							'\'',
							'_',
							'-'
						});
						int startOffset = current.FirstDocumentLine.Offset + text.IndexOf(text2, startIndex, System.StringComparison.InvariantCultureIgnoreCase);
						if (!this.spellingService.Spell(text2))
						{
							TextSegment item = new TextSegment
							{
								StartOffset = startOffset,
								Length = current2.Length
							};
							this.spellCheckRenderer.ErrorSegments.Add(item);
						}
						startIndex = text.IndexOf(current2, startIndex, System.StringComparison.InvariantCultureIgnoreCase) + current2.Length;
					}
				}
			}
			this.view.InvalidateLayer(this.spellCheckRenderer.Layer);
		}
		private void ClearSpellCheckErrors()
		{
			if (this.spellCheckRenderer == null)
			{
				return;
			}
			this.spellCheckRenderer.ErrorSegments.Clear();
		}
		public void ClearSpellCheckErrorsAndInvalidateLayer()
		{
			this.ClearSpellCheckErrors();
			this.view.InvalidateLayer(this.spellCheckRenderer.Layer);
		}
		public System.Collections.Generic.IEnumerable<TextSegment> GetSpellCheckErrors()
		{
			if (this.spellCheckRenderer == null)
			{
				return Enumerable.Empty<TextSegment>();
			}
			return this.spellCheckRenderer.ErrorSegments;
		}
		public System.Collections.Generic.IEnumerable<string> GetSpellcheckSuggestions(string word)
		{
			if (this.spellCheckRenderer == null)
			{
				return Enumerable.Empty<string>();
			}
			return this.spellingService.Suggestions(word);
		}
	}
}
