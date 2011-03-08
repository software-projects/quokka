using System;

namespace Quokka.Sandbox
{
	public interface IListener<TFrame> : IDisposable
	{
		event EventHandler ClientConnected;
		event EventHandler<ExceptionEventArgs> ListenException;

		void StartListening();
		ITransport<TFrame> GetNextTransport();
	}
}