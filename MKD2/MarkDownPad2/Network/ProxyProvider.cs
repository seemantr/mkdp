using System;
using System.Collections.Generic;
namespace MarkdownPad2.Network
{
	internal class ProxyProvider
	{
		public static System.Collections.Generic.List<ProxyType> ProxyTypes
		{
			get
			{
				return new System.Collections.Generic.List<ProxyType>
				{
					ProxyType.Auto,
					ProxyType.None,
					ProxyType.Manual
				};
			}
		}
	}
}
