using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
namespace MarkdownPad2.Settings
{
	public static class SettingsProvider
	{
		private static readonly byte[] entropy = System.Text.Encoding.Unicode.GetBytes("A Little Salt Goes A Long Way");
		public static string EncryptString(System.Security.SecureString input)
		{
			byte[] inArray = ProtectedData.Protect(System.Text.Encoding.Unicode.GetBytes(input.ToInsecureString()), SettingsProvider.entropy, DataProtectionScope.CurrentUser);
			return System.Convert.ToBase64String(inArray);
		}
		public static System.Security.SecureString DecryptString(string encryptedData)
		{
			if (string.IsNullOrEmpty(encryptedData))
			{
				return new System.Security.SecureString();
			}
			System.Security.SecureString result;
			try
			{
				byte[] bytes = ProtectedData.Unprotect(System.Convert.FromBase64String(encryptedData), SettingsProvider.entropy, DataProtectionScope.CurrentUser);
				result = System.Text.Encoding.Unicode.GetString(bytes).ToSecureString();
			}
			catch
			{
				result = new System.Security.SecureString();
			}
			return result;
		}
		public static System.Security.SecureString ToSecureString(this string input)
		{
			System.Security.SecureString secureString = new System.Security.SecureString();
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				secureString.AppendChar(c);
			}
			secureString.MakeReadOnly();
			return secureString;
		}
		public static string ToInsecureString(this System.Security.SecureString input)
		{
			string result = string.Empty;
			System.IntPtr intPtr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
			try
			{
				result = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(intPtr);
			}
			finally
			{
				System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(intPtr);
			}
			return result;
		}
	}
}
