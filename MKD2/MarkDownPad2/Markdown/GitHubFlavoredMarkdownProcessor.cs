using MarkdownPad2.i18n;
using MarkdownPad2.Properties;
using MarkdownPad2.Rest;
using MarkdownPad2.Settings;
using MarkdownPad2.Utilities;
using NLog;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Threading;
using WPFCustomMessageBox;
namespace MarkdownPad2.Markdown
{
	internal class GitHubFlavoredMarkdownProcessor : IMarkdownProcessor
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly Settings _settings = Settings.Default;
		private int rateLimitRemaining;
		public string ProcessorName
		{
			get
			{
				return LocalizationProvider.GetLocalizedString("GithubFlavoredMarkdown", false, "MarkdownPadStrings");
			}
		}
		public string Description
		{
			get
			{
				return LocalizationProvider.GetLocalizedString("GFM_Description", false, "MarkdownPadStrings");
			}
		}
		public int RateLimit
		{
			get;
			private set;
		}
		public int RateLimitRemainingRemaining
		{
			get
			{
				return this.rateLimitRemaining;
			}
			private set
			{
				this.rateLimitRemaining = value;
				if (this.rateLimitRemaining % 10 == 0)
				{
					GitHubFlavoredMarkdownProcessor._logger.Trace("GFM remaining requests: " + this.rateLimitRemaining);
				}
				if (this.rateLimitRemaining == 10)
				{
					Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, delegate
					{
						CustomMessageBox.Show(LocalizationProvider.GetLocalizedString("GitHub_LowRequestsWarningMessage", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("GitHub_ConsiderLoggingIn", false, "MarkdownPadStrings"), LocalizationProvider.GetLocalizedString("GitHub_LowRequestsWarningTitle", false, "MarkdownPadStrings"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
					});
				}
				if (this.rateLimitRemaining <= 0)
				{
					throw new RestResponseException(LocalizationProvider.GetLocalizedString("Error_GitHubRateLimitReached", false, "MarkdownPadStrings") + StringUtilities.GetNewLines(2) + LocalizationProvider.GetLocalizedString("GitHub_ConsiderLoggingIn", false, "MarkdownPadStrings"));
				}
			}
		}
		public bool RenderWhileScrollingAllowed
		{
			get
			{
				return false;
			}
		}
		public bool BackgroundRenderingAllowed
		{
			get
			{
				return true;
			}
		}
		public System.TimeSpan RenderDelay
		{
			get;
			private set;
		}
		public GitHubFlavoredMarkdownProcessor()
		{
			this.RenderDelay = System.TimeSpan.FromSeconds(0.5);
		}
		public string ConvertMarkdownToHTML(string markdown)
		{
			if (!NetworkUtilities.IsNetworkAvailable())
			{
				return "<strong>" + LocalizationProvider.GetLocalizedString("Error_NoNetworkConnection", false, "MarkdownPadStrings") + "</strong>";
			}
			IAuthenticator authenticator = null;
			if (!this._settings.Markdown_GFM_AnonymousMode)
			{
				authenticator = new HttpBasicAuthenticator(this._settings.Markdown_GFM_Username, SettingsProvider.DecryptString(this._settings.Markdown_GFM_Password).ToInsecureString());
			}
			RestClient restClient = new RestClient
			{
				BaseUrl = "https://api.github.com",
				Authenticator = authenticator,
				Proxy = WebRequest.DefaultWebProxy
			};
			RestRequest restRequest = new RestRequest
			{
				Resource = "markdown",
				RequestFormat = RestSharp.DataFormat.Json,
				Method = Method.POST
			};
			restRequest.AddBody(new
			{
				text = markdown,
				mode = "gfm"
			});
			IRestResponse restResponse = restClient.Execute(restRequest);
			try
			{
				this.CheckResponse(restResponse);
			}
			catch (RestResponseException ex)
			{
				GitHubFlavoredMarkdownProcessor._logger.ErrorException("GFM RestResponseException", ex);
				string text = string.Empty;
				if (ex.Response == null)
				{
					GitHubFlavoredMarkdownProcessor._logger.ErrorException("GFM RestResponseException: Response was null", ex);
					text = ex.Message;
				}
				else
				{
					if (ex.Response.ErrorException != null)
					{
						GitHubFlavoredMarkdownProcessor._logger.ErrorException("GFM RestResponseException: Response was not null, and an Response ErrorException was present.", ex.Response.ErrorException);
						text = ex.Response.ErrorMessage;
					}
					else
					{
						GitHubFlavoredMarkdownProcessor._logger.ErrorException("GFM RestResponseException: Error message received from GFM API, deserializing JSON...\n" + ex.Response, ex);
						try
						{
							JsonDeserializer jsonDeserializer = new JsonDeserializer();
							System.Collections.Generic.Dictionary<string, string> dictionary = jsonDeserializer.Deserialize<System.Collections.Generic.Dictionary<string, string>>(ex.Response);
							if (dictionary.ContainsKey("message"))
							{
								HttpStatusCode statusCode = ex.Response.StatusCode;
								string statusDescription = ex.Response.StatusDescription;
								string arg = dictionary["message"];
								text = string.Format("{0} ({1}): {2}", statusCode, statusDescription, arg);
								string text2 = text;
								text = string.Concat(new string[]
								{
									text2,
									"<br /><br />",
									LocalizationProvider.GetLocalizedString("Error_GitHubFlavoredMarkdownMessage2", false, "MarkdownPadStrings"),
									"<br /><h4>Technical Details</h4><pre><code>",
									ex.Response.Content,
									"</code></pre>"
								});
							}
						}
						catch (System.Runtime.Serialization.SerializationException ex2)
						{
							GitHubFlavoredMarkdownProcessor._logger.ErrorException("GFM: Error deserializing error", ex2);
							text = string.Concat(new object[]
							{
								LocalizationProvider.GetLocalizedString("Gfm_Error_UnexpectedErrorResponse", false, "MarkdownPadStrings"),
								"<br /><br /><h4>Technical Details</h4><pre><code>",
								ex,
								"</code></pre><br /><pre><code>",
								ex.Response.Content,
								"</code></pre><br /><pre><code>",
								ex2,
								"</code></pre>"
							});
						}
					}
				}
				return string.Concat(new string[]
				{
					"<p>",
					LocalizationProvider.GetLocalizedString("Error_GitHubFlavoredMarkdownMessage1", false, "MarkdownPadStrings"),
					"</p><p><strong>",
					text,
					"</strong></p><p>",
					LocalizationProvider.GetLocalizedString("IfBugPersists", false, "MarkdownPadStrings"),
					"</p>"
				});
			}
			return restResponse.Content;
		}
		private void CheckResponse(IRestResponse response)
		{
			RestUtils.CheckResponseStatus(response);
			this.CheckRateLimit(response.Headers);
		}
		private void CheckRateLimit(System.Collections.Generic.IEnumerable<Parameter> headers)
		{
			IQueryable<Parameter> source = 
				from x in headers.AsQueryable<Parameter>()
				where x.Name.StartsWith("X-RateLimit")
				select x;
			Parameter parameter = source.Single((Parameter x) => x.Name.EndsWith("-Limit"));
			Parameter parameter2 = source.Single((Parameter x) => x.Name.EndsWith("-Remaining"));
			this.RateLimit = System.Convert.ToInt32(parameter.Value);
			this.RateLimitRemainingRemaining = System.Convert.ToInt32(parameter2.Value);
		}
		public void ApplySettings()
		{
			throw new System.NotImplementedException();
		}
		public override bool Equals(object obj)
		{
			return obj.GetType() == base.GetType();
		}
	}
}
