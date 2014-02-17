using RestSharp;
using System;
using System.Runtime.Serialization;
namespace MarkdownPad2.Rest
{
	[System.Serializable]
	public class RestResponseException : System.Exception
	{
		public IRestResponse Response
		{
			get;
			set;
		}
		public RestResponseException()
		{
		}
		public RestResponseException(string message) : base(message)
		{
		}
		public RestResponseException(string message, System.Exception inner) : base(message, inner)
		{
		}
		protected RestResponseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}
	}
}
