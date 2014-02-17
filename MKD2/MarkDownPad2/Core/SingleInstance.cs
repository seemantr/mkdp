using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
namespace MarkdownPad2.Core
{
	public static class SingleInstance<TApplication> where TApplication : Application, ISingleInstanceApp
	{
		private class IPCRemoteService : System.MarshalByRefObject
		{
			public void InvokeFirstInstance(System.Collections.Generic.IList<string> args)
			{
				if (Application.Current != null)
				{
					Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(SingleInstance<TApplication>.ActivateFirstInstanceCallback), args);
				}
			}
			public override object InitializeLifetimeService()
			{
				return null;
			}
		}
		private const string Delimiter = ":";
		private const string ChannelNameSuffix = "SingeInstanceIPCChannel";
		private const string RemoteServiceName = "SingleInstanceApplicationService";
		private const string IpcProtocol = "ipc://";
		private static System.Threading.Mutex singleInstanceMutex;
		private static IpcServerChannel channel;
		private static System.Collections.Generic.IList<string> commandLineArgs;
		public static System.Collections.Generic.IList<string> CommandLineArgs
		{
			get
			{
				return SingleInstance<TApplication>.commandLineArgs;
			}
		}
		public static bool InitializeAsFirstInstance(string uniqueName)
		{
			SingleInstance<TApplication>.commandLineArgs = SingleInstance<TApplication>.GetCommandLineArgs(uniqueName);
			string text = uniqueName + System.Environment.UserName;
			string channelName = text + ":" + "SingeInstanceIPCChannel";
			bool flag;
			SingleInstance<TApplication>.singleInstanceMutex = new System.Threading.Mutex(true, text, ref flag);
			if (flag)
			{
				SingleInstance<TApplication>.CreateRemoteService(channelName);
			}
			else
			{
				SingleInstance<TApplication>.SignalFirstInstance(channelName, SingleInstance<TApplication>.commandLineArgs);
			}
			return flag;
		}
		public static void Cleanup()
		{
			if (SingleInstance<TApplication>.singleInstanceMutex != null)
			{
				SingleInstance<TApplication>.singleInstanceMutex.Close();
				SingleInstance<TApplication>.singleInstanceMutex = null;
			}
			if (SingleInstance<TApplication>.channel != null)
			{
				System.Runtime.Remoting.Channels.ChannelServices.UnregisterChannel(SingleInstance<TApplication>.channel);
				SingleInstance<TApplication>.channel = null;
			}
		}
		private static System.Collections.Generic.IList<string> GetCommandLineArgs(string uniqueApplicationName)
		{
			string[] array = null;
			if (System.AppDomain.CurrentDomain.ActivationContext == null)
			{
				array = System.Environment.GetCommandLineArgs();
			}
			else
			{
				string path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), uniqueApplicationName);
				string path2 = System.IO.Path.Combine(path, "cmdline.txt");
				if (System.IO.File.Exists(path2))
				{
					try
					{
						using (System.IO.TextReader textReader = new System.IO.StreamReader(path2, System.Text.Encoding.Unicode))
						{
							array = NativeMethods.CommandLineToArgvW(textReader.ReadToEnd());
						}
						System.IO.File.Delete(path2);
					}
					catch (System.IO.IOException)
					{
					}
				}
			}
			if (array == null)
			{
				array = new string[0];
			}
			return new System.Collections.Generic.List<string>(array);
		}
		private static void CreateRemoteService(string channelName)
		{
			BinaryServerFormatterSinkProvider binaryServerFormatterSinkProvider = new BinaryServerFormatterSinkProvider();
			binaryServerFormatterSinkProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
			System.Collections.IDictionary dictionary = new System.Collections.Generic.Dictionary<string, string>();
			dictionary["name"] = channelName;
			dictionary["portName"] = channelName;
			dictionary["exclusiveAddressUse"] = "false";
			SingleInstance<TApplication>.channel = new IpcServerChannel(dictionary, binaryServerFormatterSinkProvider);
			System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(SingleInstance<TApplication>.channel, true);
			SingleInstance<TApplication>.IPCRemoteService obj = new SingleInstance<TApplication>.IPCRemoteService();
			System.Runtime.Remoting.RemotingServices.Marshal(obj, "SingleInstanceApplicationService");
		}
		private static void SignalFirstInstance(string channelName, System.Collections.Generic.IList<string> args)
		{
			IpcClientChannel chnl = new IpcClientChannel();
			System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(chnl, true);
			string url = "ipc://" + channelName + "/SingleInstanceApplicationService";
			SingleInstance<TApplication>.IPCRemoteService iPCRemoteService = (SingleInstance<TApplication>.IPCRemoteService)System.Runtime.Remoting.RemotingServices.Connect(typeof(SingleInstance<TApplication>.IPCRemoteService), url);
			if (iPCRemoteService != null)
			{
				iPCRemoteService.InvokeFirstInstance(args);
			}
		}
		private static object ActivateFirstInstanceCallback(object arg)
		{
			System.Collections.Generic.IList<string> args = arg as System.Collections.Generic.IList<string>;
			SingleInstance<TApplication>.ActivateFirstInstance(args);
			return null;
		}
		private static void ActivateFirstInstance(System.Collections.Generic.IList<string> args)
		{
			if (Application.Current == null)
			{
				return;
			}
			TApplication tApplication = (TApplication)((object)Application.Current);
			tApplication.SignalExternalCommandLineArgs(args);
		}
	}
}
