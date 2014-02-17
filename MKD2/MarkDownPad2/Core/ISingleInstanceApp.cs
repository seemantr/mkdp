using System;
using System.Collections.Generic;
namespace MarkdownPad2.Core
{
	public interface ISingleInstanceApp
	{
		bool SignalExternalCommandLineArgs(System.Collections.Generic.IList<string> args);
	}
}
