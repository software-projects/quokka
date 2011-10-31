using Quokka.Diagnostics;
using Quokka.UI.Tasks;

namespace Quokka.UI.Messages.Internal
{
	internal class MessageBoxTask : UITask
	{
		public readonly UIMessage Message;

		public MessageBoxTask(UIMessage message)
		{
			Message = Verify.ArgumentNotNull(message, "message");
		}

		protected override void CreateNodes()
		{
			var node = CreateNode();

			node.SetPresenter<MessageBoxPresenter>()
				.NavigateTo(p => p.NextCommand, null);
		}

		protected override void CreateState()
		{
			RegisterInstance(Message);
		}
	}
}