using MarkdownPad2.Utilities;
using Microsoft.Win32;
using NLog;
using RestSharp.Contrib;
using System;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
namespace MarkdownPad2.Core
{
	public static class Stats
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		private static bool useSsl = true;
		public static void SendStats()
		{
			if (!NetworkUtilities.IsNetworkAvailable())
			{
				return;
			}
			string text = System.Environment.OSVersion.ToString();
			string text2 = System.Environment.Is64BitOperatingSystem ? "x64" : "x86";
			string version = AssemblyUtilities.Version;
			string text3 = string.Empty;
			RegistryView view = RegistryView.Registry32;
			if (System.Environment.Is64BitOperatingSystem)
			{
				view = RegistryView.Registry64;
			}
			try
			{
				Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, view);
				registryKey = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography");
				if (registryKey != null)
				{
					text3 = registryKey.GetValue("MachineGuid", "keyNotFound").ToString();
				}
			}
			catch (System.Exception exception)
			{
				Stats._logger.ErrorException("Error getting machine GUID", exception);
			}
			string input = string.Concat(new object[]
			{
				text3,
				":",
				text,
				":",
				System.Environment.MachineName,
				":",
				System.Environment.UserName,
				":",
				System.Environment.UserDomainName,
				":",
				text2,
				":",
				System.Environment.ProcessorCount
			});
			string str = Stats.ComputeHash(input, new System.Security.Cryptography.SHA1CryptoServiceProvider());
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append("anonHostId=" + HttpUtility.UrlEncode(str));
			stringBuilder.Append("&os=" + HttpUtility.UrlEncode(text));
			stringBuilder.Append("&cpu=" + HttpUtility.UrlEncode(text2));
			stringBuilder.Append("&version=" + HttpUtility.UrlEncode(version));
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(stringBuilder.ToString());
			string uri = Stats.useSsl ? Urls.MarkdownPad_StatsSsl : Urls.MarkdownPad_StatsNoSsl;
			bool flag = false;
			try
			{
				flag = Stats.CreateAndSendWebRequest(uri, bytes);
			}
			catch (WebException ex)
			{
				Stats._logger.Warn<WebException>("WebException statistics error, retrying once", ex);
				System.Exception baseException = ex.GetBaseException();
				if (baseException.GetType() == typeof(AuthenticationException) && Stats.useSsl)
				{
					Stats.useSsl = false;
					Stats.SendStats();
					return;
				}
			}
			catch (System.Exception exception2)
			{
				Stats._logger.WarnException("Statistics error", exception2);
			}
			Stats._logger.Trace("Statistics success: " + flag);
		}
		private static bool CreateAndSendWebRequest(string uri, byte[] data)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			httpWebRequest.ContentLength = (long)data.Length;
			using (System.IO.Stream requestStream = httpWebRequest.GetRequestStream())
			{
				requestStream.Write(data, 0, data.Length);
			}
			WebResponse response = httpWebRequest.GetResponse();
			return ((HttpWebResponse)response).StatusCode == HttpStatusCode.OK;
		}
		private static string ComputeHash(string input, System.Security.Cryptography.HashAlgorithm algorithm)
		{
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
			byte[] value = algorithm.ComputeHash(bytes);
			return System.BitConverter.ToString(value);
		}
	}
}
