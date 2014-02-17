using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.ImageUploader;
using MarkdownPad2.Licensing;
using MarkdownPad2.Rest;
using MarkdownPad2.Utilities;
using Microsoft.Win32;
using NLog;
using RestSharp.Deserializers;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;
namespace MarkdownPad2.UI
{
	public class ModalDialog : Window, System.Windows.Markup.IComponentConnector
	{
		public static Logger _logger = LogManager.GetCurrentClassLogger();
		internal Border Border_BusyIndicator;
		internal BusyIndicator BusyIncicator;
		internal TextBlock Label_ImageInfo;
		internal TextBlock TextBlock_Url;
		internal TextBox TextBox_URL;
		internal Button Button_DeleteImage;
		internal Button Button_Browse;
		internal TextBlock TextBlock_Details;
		internal TextBox TextBox_Details;
		internal Button Button_OK;
		internal Button Button_Cancel;
		private bool _contentLoaded;
		public string UrlValue
		{
			get
			{
				if (!this.TextBox_URL.Text.Equals("http://"))
				{
					return this.TextBox_URL.Text;
				}
				return string.Empty;
			}
			set
			{
				this.TextBox_URL.Text = value;
			}
		}
		public string DeleteUrl
		{
			get;
			set;
		}
		public string DetailValue
		{
			get
			{
				return this.TextBox_Details.Text;
			}
			set
			{
				this.TextBox_Details.Text = value;
			}
		}
		private ImgurUpload ImgurUpload
		{
			get;
			set;
		}
		public ModalDialog()
		{
			this.InitializeComponent();
		}
		private void Button_OK_OnClick(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
		private void Button_Cancel_OnClick(object sender, RoutedEventArgs e)
		{
			this.UrlValue = string.Empty;
			this.DetailValue = string.Empty;
			base.Close();
		}
		public bool ShowUrlModal(string windowTitle, string urlLabelText, string detailLabelText)
		{
			base.Title = windowTitle;
			this.TextBlock_Url.Text = urlLabelText;
			this.TextBlock_Details.Text = detailLabelText;
			this.TextBox_URL.Focus();
			this.TextBox_URL.SelectAll();
			this.Button_Browse.Visibility = Visibility.Collapsed;
			this.Label_ImageInfo.Visibility = Visibility.Collapsed;
			base.ShowDialog();
			return !string.IsNullOrEmpty(this.UrlValue);
		}
		public bool ShowImageModal(string windowTitle, string urlLabelText, string detailLabelText)
		{
			base.Title = windowTitle;
			this.TextBlock_Url.Text = urlLabelText;
			this.TextBlock_Details.Text = detailLabelText;
			this.TextBox_URL.Focus();
			this.TextBox_URL.SelectAll();
			this.Button_Browse.Visibility = Visibility.Visible;
			this.Label_ImageInfo.Visibility = Visibility.Visible;
			base.ShowDialog();
			return !string.IsNullOrEmpty(this.UrlValue);
		}
		private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Return)
			{
				base.Close();
			}
		}
		private void Button_Browse_Click(object sender, RoutedEventArgs e)
		{
			string localizedString = LocalizationProvider.GetLocalizedString("Pro_ImageUploader", false, "MarkdownPadStrings");
			if (!LicenseHelper.ValidateLicense(localizedString, this))
			{
				return;
			}
			this.BrowseAndUploadImage();
		}
		private void BrowseAndUploadImage()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = FileDialogHelper.OpenImageFileTypes,
				RestoreDirectory = true,
				Multiselect = false
			};
			if (!openFileDialog.ShowDialog().Value || string.IsNullOrEmpty(openFileDialog.FileName) || !System.IO.File.Exists(openFileDialog.FileName))
			{
				return;
			}
			this.ToggleBusyIndicator(true);
			this.ImgurUpload = new ImgurUpload();
			this.ImgurUpload.ImgurLinkCreated += new ImgurLinkCreated(this.ImgurUpload_ImgurLinkCreated);
			this.ImgurUpload.ImgurUploadError += new ImgurUploadError(this.ImgurUpload_ImgurUploadError);
			this.ImgurUpload.PostToImgur(openFileDialog.FileName);
		}
		private void ImgurUpload_ImgurUploadError(object sender, ImgurUploadErrorEventArgs e)
		{
			ModalDialog._logger.Trace("Imgur upload error event fired");
			if (this.TextBox_URL.Dispatcher.CheckAccess())
			{
				this.ProcessImgurError(e);
				return;
			}
			this.TextBox_URL.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, delegate
			{
				this.ProcessImgurError(e);
			});
		}
		private void ProcessImgurError(ImgurUploadErrorEventArgs e)
		{
			this.ToggleBusyIndicator(false);
			if (e.Exception != null && e.Exception.GetType() == typeof(RestResponseException))
			{
				RestResponseException ex = e.Exception as RestResponseException;
				ModalDialog._logger.ErrorException("Processing Imgur error exception", ex);
				string str = string.Empty;
				if (ex.Response == null)
				{
					str = ex.Message;
				}
				else
				{
					if (ex.Response.ErrorException != null)
					{
						str = ex.Response.ErrorMessage;
					}
					else
					{
						JsonDeserializer jsonDeserializer = new JsonDeserializer();
						MashapeErrorResponse mashapeErrorResponse = jsonDeserializer.Deserialize<MashapeErrorResponse>(ex.Response);
						str = mashapeErrorResponse.Message;
					}
				}
				string message = LocalizationProvider.GetLocalizedString("Error_UploadingImageToImgurMessage", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + str;
				string localizedString = LocalizationProvider.GetLocalizedString("Error_UploadingImageToImgur", false, "MarkdownPadStrings");
				MessageBoxHelper.ShowWarningMessageBox(message, localizedString);
				return;
			}
			System.Exception exception = e.Exception;
			string localizedString2 = LocalizationProvider.GetLocalizedString("Error_UploadingImageToImgurMessage", false, "MarkdownPadStrings");
			string localizedString3 = LocalizationProvider.GetLocalizedString("Error_UploadingImageToImgur", false, "MarkdownPadStrings");
			MessageBoxHelper.ShowErrorMessageBox(localizedString2, localizedString3, exception, "");
		}
		private void ImgurUpload_ImgurLinkCreated(object sender, ImgurLinkCreatedEventArgs e)
		{
			ModalDialog._logger.Trace("Imgur link created event fired, URL: " + e.ImageUrl);
			if (this.TextBox_URL.Dispatcher.CheckAccess())
			{
				this.UpdateUrl(e.ImageUrl, e.DeleteUrl);
				return;
			}
			this.TextBox_URL.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, delegate
			{
				this.UpdateUrl(e.ImageUrl, e.DeleteUrl);
			});
		}
		private void UpdateUrl(string imageUrl, string deleteUrl)
		{
			this.UrlValue = imageUrl;
			this.Button_DeleteImage.Visibility = Visibility.Visible;
			this.DeleteUrl = deleteUrl;
			this.ToggleBusyIndicator(false);
		}
		private void ToggleBusyIndicator(bool enable)
		{
			if (enable)
			{
				this.Border_BusyIndicator.Visibility = Visibility.Visible;
				this.BusyIncicator.IsBusy = true;
				return;
			}
			this.Border_BusyIndicator.Visibility = Visibility.Collapsed;
			this.BusyIncicator.IsBusy = false;
		}
		private void ModalDialog_OnClosing(object sender, CancelEventArgs e)
		{
			if (this.ImgurUpload != null)
			{
				this.ImgurUpload.ImgurLinkCreated -= new ImgurLinkCreated(this.ImgurUpload_ImgurLinkCreated);
				this.ImgurUpload.ImgurUploadError -= new ImgurUploadError(this.ImgurUpload_ImgurUploadError);
				this.ImgurUpload = null;
			}
		}
		private void Button_DeleteImage_OnClick(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.DeleteUrl))
			{
				this.DeleteUrl.TryStartDefaultProcess();
			}
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/MarkdownPad2;component/ui/modaldialog.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), System.Diagnostics.DebuggerNonUserCode]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((ModalDialog)target).Closing += new CancelEventHandler(this.ModalDialog_OnClosing);
				return;
			case 2:
				this.Border_BusyIndicator = (Border)target;
				return;
			case 3:
				this.BusyIncicator = (BusyIndicator)target;
				return;
			case 4:
				this.Label_ImageInfo = (TextBlock)target;
				return;
			case 5:
				this.TextBlock_Url = (TextBlock)target;
				return;
			case 6:
				this.TextBox_URL = (TextBox)target;
				this.TextBox_URL.KeyDown += new KeyEventHandler(this.TextBox_OnKeyDown);
				return;
			case 7:
				this.Button_DeleteImage = (Button)target;
				this.Button_DeleteImage.Click += new RoutedEventHandler(this.Button_DeleteImage_OnClick);
				return;
			case 8:
				this.Button_Browse = (Button)target;
				this.Button_Browse.Click += new RoutedEventHandler(this.Button_Browse_Click);
				return;
			case 9:
				this.TextBlock_Details = (TextBlock)target;
				return;
			case 10:
				this.TextBox_Details = (TextBox)target;
				this.TextBox_Details.KeyDown += new KeyEventHandler(this.TextBox_OnKeyDown);
				return;
			case 11:
				this.Button_OK = (Button)target;
				this.Button_OK.Click += new RoutedEventHandler(this.Button_OK_OnClick);
				return;
			case 12:
				this.Button_Cancel = (Button)target;
				this.Button_Cancel.Click += new RoutedEventHandler(this.Button_Cancel_OnClick);
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}
	}
}
