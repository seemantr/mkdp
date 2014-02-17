using System;
using System.Collections.Generic;
namespace MarkdownPad2.Core
{
	internal class FileExtensions
	{
		public static System.Collections.Generic.List<string> ImageFileExtensions
		{
			get
			{
				return new System.Collections.Generic.List<string>
				{
					".jpg",
					".jpeg",
					".png",
					".gif",
					".bmp"
				};
			}
		}
	}
}
