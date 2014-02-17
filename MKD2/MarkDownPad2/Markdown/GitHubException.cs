using System;
namespace MarkdownPad2.Markdown
{
	[System.Serializable]
	public class GitHubException : System.Exception
	{
		public GitHubException()
		{
		}
		public GitHubException(string message) : base(message)
		{
		}
	}
}
