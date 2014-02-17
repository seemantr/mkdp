using System;
namespace MarkdownPad2.ImageUploader
{
	public class ImgurUploadErrorEventArgs : System.EventArgs
	{
		public System.Exception Exception
		{
			get;
			set;
		}
		public ImgurUploadErrorEventArgs(System.Exception ex)
		{
			this.Exception = ex;
		}
	}
}
