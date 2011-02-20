using Quokka.UI.Messages.Internal;
using Quokka.UI.Tasks;

namespace Quokka.UI.Messages
{
	/// <summary>
	/// 	Used for displaying modal message boxes.
	/// </summary>
	public class UIMessageBox
	{
		public IViewDeck ViewDeck { get; set; }

		public virtual UIAnswer Show(UIMessage message)
		{
			using (var modalWindow = ViewDeck.CreateModalWindow())
			{
				using (var task = new MessageBoxTask(message))
				{
					task.TaskComplete += (o, e) => modalWindow.Dispose();
					task.Start(modalWindow.ViewDeck);
					modalWindow.ShowModal(true);
				}
			}
			return message.SelectedAnswer;
		}
	}
}