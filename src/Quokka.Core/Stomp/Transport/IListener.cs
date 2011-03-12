using System;
using System.Net;
using Quokka.Sandbox;

namespace Quokka.Stomp.Transport
{
	public interface IListener<TFrame> : IDisposable
	{
		event EventHandler ClientConnected;
		event EventHandler<ExceptionEventArgs> ListenException;

		EndPoint ListenEndPoint { get; }
		void StartListening();
		ITransport<TFrame> GetNextTransport();
	}
}