using Quokka.Sandbox;

namespace Quokka.Stomp
{
	public class StompListener : SocketListener<StompFrame, StompFrameBuilder>
	{
	}

	public class StompClient : SocketClientTransport<StompFrame, StompFrameBuilder>
	{
	}
}