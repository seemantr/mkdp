using System;
using System.Collections.Generic;
namespace MarkdownPad2.Markdown
{
	public class MarkdownSyntaxProvider
	{
		public static System.Collections.Generic.Dictionary<UnorderedListStyle, string> UnorderedListSyntaxMap
		{
			get
			{
				return new System.Collections.Generic.Dictionary<UnorderedListStyle, string>
				{

					{
						UnorderedListStyle.Star,
						MarkdownSyntax.UnorderedListStar
					},

					{
						UnorderedListStyle.Dash,
						MarkdownSyntax.UnorderedListDash
					},

					{
						UnorderedListStyle.Plus,
						MarkdownSyntax.UnorderedListPlus
					}
				};
			}
		}
	}
}
