using RestSharp;
using System;
using System.Net;
namespace MarkdownPad2.Rest
{
	public static class RestUtils
	{
		public static void CheckResponseStatus(IRestResponse response)
		{
			if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode != HttpStatusCode.OK)
			{
				string content = response.Content;
				throw new RestResponseException(content)
				{
					Response = response
				};
			}
		}
	}
}
