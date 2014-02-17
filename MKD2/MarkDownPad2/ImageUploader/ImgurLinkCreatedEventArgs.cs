using System;
namespace MarkdownPad2.ImageUploader
{
	public class ImgurLinkCreatedEventArgs : System.EventArgs
	{
		public string ImageUrl
		{
			get;
			set;
		}
		public string DeleteUrl
		{
			get;
			set;
		}
		public ImgurLinkCreatedEventArgs(string imageUrl, string deleteUrl)
		{
			this.ImageUrl = imageUrl;
			this.DeleteUrl = deleteUrl;
		}
	}
}
