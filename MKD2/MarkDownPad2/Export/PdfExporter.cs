using MarkdownPad2.Core;
using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using MarkdownPad2.Utilities;
using NLog;
using Pechkin;
using Pechkin.EventHandlers;
using Pechkin.Synchronized;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
namespace MarkdownPad2.Export
{
	public class PdfExporter
	{
		private readonly Settings _settings = Settings.Default;
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		public string DocumentTitle
		{
			get;
			private set;
		}
		public string OutputFilename
		{
			get;
			private set;
		}
		public string HtmlContent
		{
			get;
			private set;
		}
		public bool EnableOutlineGeneration
		{
			get;
			set;
		}
		public bool IncludeCssBackground
		{
			get;
			set;
		}
		public PdfExporter(string filename, string content)
		{
			this.DocumentTitle = System.IO.Path.GetFileNameWithoutExtension(filename);
			this.OutputFilename = filename;
			this.HtmlContent = content;
		}
		public void ExportPdf()
		{
			ObjectConfig doc = new ObjectConfig().SetPrintBackground(this.IncludeCssBackground);
			GlobalConfig globalConfig = new GlobalConfig();
			globalConfig.SetDocumentTitle(this.DocumentTitle).SetOutlineGeneration(this.EnableOutlineGeneration).SetPaperOrientation(this._settings.IO_Pdf_LandscapeMode).SetMarginLeft(this._settings.IO_Pdf_MarginLeftInMillimeters).SetMarginTop(this._settings.IO_Pdf_MarginTopInMillimeters).SetMarginRight(this._settings.IO_Pdf_MarginRightInMillimeters).SetMarginBottom(this._settings.IO_Pdf_MarginBottomInMillimeters).SetPaperSize(this._settings.IO_Pdf_PaperSize);
			IPechkin pechkin = new SynchronizedPechkin(globalConfig);
			pechkin.Finished += new FinishEventHandler(this.OnFinished);
			try
			{
				byte[] bytes = pechkin.Convert(doc, this.HtmlContent);
				System.IO.File.WriteAllBytes(this.OutputFilename, bytes);
			}
			catch (System.Exception exception)
			{
				PdfExporter._logger.ErrorException("Error exporting PDF", exception);
				MessageBoxHelper.ShowErrorMessageBox(LocalizationProvider.GetLocalizedString("Error_PdfExportMessage", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("Error_PdfExportTitle", false, "MarkdownPadStrings"), exception, "");
			}
		}
		private void OnFinished(SimplePechkin converter, bool success)
		{
			if (!success)
			{
				PdfExporter._logger.Error("PDF export failed to: " + this.OutputFilename);
				MessageBox.Show(LocalizationProvider.GetLocalizedString("PdfExportFailedMessage", true, "MarkdownPadStrings") + this.OutputFilename, LocalizationProvider.GetLocalizedString("PdfExportFailedTitle", false, "MarkdownPadStrings"), MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			PdfExporter._logger.Trace("Exported PDF document to: " + this.OutputFilename);
			if (!this._settings.IO_OpenPdfFileAfterExport)
			{
				return;
			}
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
			backgroundWorker.DoWork += delegate(object param0, DoWorkEventArgs param1)
			{
				int num = 1;
				while (!System.IO.File.Exists(this.OutputFilename) && num <= 10)
				{
					PdfExporter._logger.Trace(string.Concat(new object[]
					{
						"Try #",
						num,
						" ",
						System.IO.File.Exists(this.OutputFilename)
					}));
					num++;
					System.Threading.Thread.Sleep(300);
				}
			};
			backgroundWorker.RunWorkerAsync();
		}
		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			PdfExporter._logger.Trace("Worker completed! File exists: " + System.IO.File.Exists(this.OutputFilename));
			this.OutputFilename.TryStartDefaultProcess();
		}
		private static void OnPhase(SimplePechkin converter, int phasenumber, string phasedescription)
		{
			throw new System.NotImplementedException();
		}
		private static void OnWarning(SimplePechkin converter, string warningtext)
		{
			throw new System.NotImplementedException();
		}
		private static void OnBegin(SimplePechkin converter, int expectedphasecount)
		{
			throw new System.NotImplementedException();
		}
		private static void OnError(SimplePechkin converter, string errortext)
		{
			throw new System.NotImplementedException();
		}
	}
}
