using MarkdownPad2.Properties;
using System;
using System.Net.NetworkInformation;
namespace MarkdownPad2.Utilities
{
	public class NetworkUtilities
	{
		public static bool IsNetworkAvailable()
		{
			return !Settings.Default.App_VerifyNetworkConnection || NetworkUtilities.IsNetworkAvailable(0L);
		}
		private static bool IsNetworkAvailable(long minimumSpeed)
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				return false;
			}
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			for (int i = 0; i < allNetworkInterfaces.Length; i++)
			{
				NetworkInterface networkInterface = allNetworkInterfaces[i];
				if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel && networkInterface.Speed >= minimumSpeed && networkInterface.Description.IndexOf("virtual", System.StringComparison.OrdinalIgnoreCase) < 0 && networkInterface.Name.IndexOf("virtual", System.StringComparison.OrdinalIgnoreCase) < 0 && !networkInterface.Description.Equals("Microsoft Loopback Adapter", System.StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}
	}
}
