using System;
using System.Collections.Generic;
namespace MarkdownPad2.SessionManager
{
	public class Session
	{
		public string Name
		{
			get;
			set;
		}
		public System.Collections.Generic.List<string> Files
		{
			get;
			set;
		}
		public override string ToString()
		{
			return string.Join(",", this.Files);
		}
	}
}
