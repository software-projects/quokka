using System;
using Quokka.Diagnostics;

namespace Quokka.Sandbox
{
	/// <summary>
	/// Specifies a timeout for a response to a message.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class MessageTimeoutAttribute : Attribute
	{
		public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

		public TimeSpan Timeout { get; private set; }

		public MessageTimeoutAttribute()
		{
			Timeout = DefaultTimeout;
		}

		public double Seconds
		{
			get { return Timeout.TotalSeconds; }
			set { Timeout = TimeSpan.FromSeconds(value); }
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class RequestMessageAttribute : Attribute
	{
		/// <summary>
		/// Timeout in seconds
		/// </summary>
		public int Timeout { get; set; }
	}

	/// <summary>
	/// Indicates that a message of the specified type is an
	/// expected response message. There can be more than one
	/// expected reponse message type -- use multiple attributes
	/// in this case.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class ExpectedResponseMessage : Attribute
	{
		public Type ResponseType { get; private set; }

		public ExpectedResponseMessage(Type type)
		{
			ResponseType = Verify.ArgumentNotNull(type, "type");
		}
	}
}
