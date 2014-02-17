using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
namespace MarkdownPad2.RendererContent
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode, System.Runtime.CompilerServices.CompilerGenerated]
	internal class WebResources
	{
		private static System.Resources.ResourceManager resourceMan;
		private static System.Globalization.CultureInfo resourceCulture;
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(WebResources.resourceMan, null))
				{
					System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("MarkdownPad2.RendererContent.WebResources", typeof(WebResources).Assembly);
					WebResources.resourceMan = resourceManager;
				}
				return WebResources.resourceMan;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static System.Globalization.CultureInfo Culture
		{
			get
			{
				return WebResources.resourceCulture;
			}
			set
			{
				WebResources.resourceCulture = value;
			}
		}
		internal static string PygmentsCss
		{
			get
			{
				return WebResources.ResourceManager.GetString("PygmentsCss", WebResources.resourceCulture);
			}
		}
		internal WebResources()
		{
		}
	}
}
