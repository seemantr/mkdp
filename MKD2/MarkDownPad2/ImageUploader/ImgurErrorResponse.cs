using System;
namespace MarkdownPad2.ImageUploader
{
	public class ImgurErrorResponse
	{
		public ImgurErrorData Data
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
