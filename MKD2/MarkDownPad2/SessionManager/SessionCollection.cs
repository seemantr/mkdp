using MarkdownPad2.Properties;
using System;
using System.Collections.Generic;
namespace MarkdownPad2.SessionManager
{
	public class SessionCollection
	{
		private readonly Settings _settings = Settings.Default;
		public System.Collections.Generic.List<Session> Sessions
		{
			get;
			set;
		}
		public SessionCollection()
		{
			this.Sessions = new System.Collections.Generic.List<Session>();
		}
		public void AddToCollection(Session newSession)
		{
			this.Sessions.Add(newSession);
			if (this.Sessions.Count > this._settings.App_MaxSessions)
			{
				this.Sessions.RemoveAt(0);
			}
		}
	}
}
