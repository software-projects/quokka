using Quokka.Diagnostics;

namespace Quokka.Stomp.Internal
{
	public static class UnitTestHelper
	{
		public static void DisconnectImmediately(StompClient client)
		{
			Verify.ArgumentNotNull(client, "client");
			var message = new StompFrame(StompCommand.Disconnect)
			              	{
			              		Headers =
			              			{
			              				{StompHeader.NonStandard.KeepSession, "true"}
			              			}
			              	};
			client.SendRawMessage(message, false);
		}
	}
}