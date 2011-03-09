using System;
using System.Net;

namespace Quokka.Sandbox
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