using MarkdownPad2.i18n;
using MarkdownPad2.Rest;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
namespace MarkdownPad2.ImageUploader
{
	public class ImgurUpload
	{
		private IRestResponse _response;
		private int _clientRateLimitRemaining;
		private int _userRateLimitRemaining;
		public event ImgurLinkCreated ImgurLinkCreated;
		public event ImgurUploadError ImgurUploadError;
		private IRestResponse Response
		{
			set
			{
				this._response = value;
				try
				{
					this.CheckResponse(this._response);
				}
				catch (System.Exception ex)
				{
					if (this.ImgurUploadError != null)
					{
						this.ImgurUploadError(this, new ImgurUploadErrorEventArgs(ex));
					}
					return;
				}
				ImgurResponse imgurResponse = this.ProcessContent(this._response);
				string imageUrl = string.Empty;
				string deleteUrl = string.Empty;
				if (imgurResponse.Success)
				{
					imageUrl = imgurResponse.Data.Link;
					deleteUrl = string.Format("http://imgur.com/delete/{0}", imgurResponse.Data.DeleteHash);
					if (this.ImgurLinkCreated != null)
					{
						this.ImgurLinkCreated(this, new ImgurLinkCreatedEventArgs(imageUrl, deleteUrl));
					}
				}
			}
		}
		public int ClientRateLimit
		{
			get;
			private set;
		}
		public int ClientRateRemaining
		{
			get
			{
				return this._clientRateLimitRemaining;
			}
			set
			{
				this._clientRateLimitRemaining = value;
				if (this._clientRateLimitRemaining <= 0)
				{
					throw new RestResponseException(LocalizationProvider.GetLocalizedString("Error_ImgurClientRateLimitReached", false, "MarkdownPadStrings"));
				}
			}
		}
		public int UserRateLimit
		{
			get;
			set;
		}
		public int UserRateRemaining
		{
			get
			{
				return this._userRateLimitRemaining;
			}
			set
			{
				this._userRateLimitRemaining = value;
				if (this._userRateLimitRemaining <= 0)
				{
					throw new RestResponseException(LocalizationProvider.GetLocalizedString("Error_ImgurUserRateLimitReached", false, "MarkdownPadStrings"));
				}
			}
		}
		public void PostToImgur(string fileName)
		{
			if (!System.IO.File.Exists(fileName))
			{
				return;
			}
			RestClient client = new RestClient
			{
				BaseUrl = "https://imgur-apiv3.p.mashape.com/",
				Proxy = WebRequest.DefaultWebProxy
			};
			RestRequest restRequest = new RestRequest
			{
				Resource = "/3/image",
				RequestFormat = DataFormat.Json,
				Method = Method.POST
			};
			restRequest.AddHeader("Authorization", "Client-ID be8d94630e9af0e");
			restRequest.AddHeader("X-Mashape-Authorization", "oa3lIntGfcRcVDud4y2MaPhFlOd31Dx1");
			string image = System.Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
			restRequest.AddBody(new
			{
				image
			});
			client.ExecuteAsync(restRequest, delegate(IRestResponse response)
			{
				this.Response = response;
			});
		}
		private ImgurResponse ProcessContent(IRestResponse content)
		{
			JsonDeserializer jsonDeserializer = new JsonDeserializer();
			return jsonDeserializer.Deserialize<ImgurResponse>(content);
		}
		private void CheckResponse(IRestResponse response)
		{
			RestUtils.CheckResponseStatus(response);
		}
		private void CheckRateLimit(System.Collections.Generic.IEnumerable<Parameter> headers)
		{
			IQueryable<Parameter> source = 
				from x in headers.AsQueryable<Parameter>()
				where x.Name.StartsWith("X-RateLimit")
				select x;
			Parameter parameter = source.Single((Parameter x) => x.Name.EndsWith("-ClientLimit"));
			Parameter parameter2 = source.Single((Parameter x) => x.Name.EndsWith("-ClientRemaining"));
			Parameter parameter3 = source.Single((Parameter x) => x.Name.EndsWith("-UserLimit"));
			Parameter parameter4 = source.Single((Parameter x) => x.Name.EndsWith("-UserRemaining"));
			this.ClientRateLimit = System.Convert.ToInt32(parameter.Value);
			this.ClientRateRemaining = System.Convert.ToInt32(parameter2.Value);
			this.UserRateLimit = System.Convert.ToInt32(parameter3.Value);
			this.UserRateRemaining = System.Convert.ToInt32(parameter4.Value);
		}
	}
}
