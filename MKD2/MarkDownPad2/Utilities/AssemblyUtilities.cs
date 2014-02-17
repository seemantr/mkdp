using System;
using System.Deployment.Application;
using System.Reflection;
namespace MarkdownPad2.Utilities
{
	public class AssemblyUtilities
	{
		public static string Version
		{
			get
			{
				string result = string.Empty;
				if (ApplicationDeployment.IsNetworkDeployed)
				{
					result = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
				}
				else
				{
					System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
					result = executingAssembly.GetName().Version.ToString();
				}
				return result;
			}
		}
		public static string AssemblyPath
		{
			get
			{
				string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uriBuilder = new UriBuilder(codeBase);
				return Uri.UnescapeDataString(uriBuilder.Path);
			}
		}
	}
}
