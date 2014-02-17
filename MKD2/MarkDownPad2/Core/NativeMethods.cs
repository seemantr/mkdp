using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
namespace MarkdownPad2.Core
{
	[System.Security.SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods
	{
		public delegate System.IntPtr MessageHandler(WM uMsg, System.IntPtr wParam, System.IntPtr lParam, out bool handled);
		[System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = CharSet.Unicode, EntryPoint = "CommandLineToArgvW")]
		private static extern System.IntPtr _CommandLineToArgvW([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string cmdLine, out int numArgs);
		[System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "LocalFree", SetLastError = true)]
		private static extern System.IntPtr _LocalFree(System.IntPtr hMem);
		public static string[] CommandLineToArgvW(string cmdLine)
		{
			System.IntPtr intPtr = System.IntPtr.Zero;
			string[] result;
			try
			{
				int num = 0;
				intPtr = NativeMethods._CommandLineToArgvW(cmdLine, out num);
				if (intPtr == System.IntPtr.Zero)
				{
					throw new Win32Exception();
				}
				string[] array = new string[num];
				for (int i = 0; i < num; i++)
				{
					System.IntPtr ptr = System.Runtime.InteropServices.Marshal.ReadIntPtr(intPtr, i * System.Runtime.InteropServices.Marshal.SizeOf(typeof(System.IntPtr)));
					array[i] = System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr);
				}
				result = array;
			}
			finally
			{
				NativeMethods._LocalFree(intPtr);
			}
			return result;
		}
	}
}
