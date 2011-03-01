using System;

namespace Quokka.Stomp
{
	public interface IStompListener : IDisposable
	{
		event EventHandler ClientConnected;
		event EventHandler<ExceptionEventArgs> ListenException;

		void StartListening();
		IStompTransport GetNextTransport();
	}
}