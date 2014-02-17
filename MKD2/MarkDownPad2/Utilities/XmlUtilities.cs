using System;
using System.IO;
using System.Reflection;
using System.Xml;
namespace MarkdownPad2.Utilities
{
	public class XmlUtilities
	{
		public static XmlTextReader GetXmlTextReaderFromResourceStream(string resource)
		{
			System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			System.IO.Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(resource);
			return new XmlTextReader(manifestResourceStream);
		}
	}
}
