using System;
namespace MarkdownPad2.ImageUploader
{
	public class ImgurResponse
	{
		public ImgurData Data
		{
			get;
			set;
		}
		public bool Success
		{
			get;
			set;
		}
		public int Status
		{
			get;
			set;
		}
	}
}
