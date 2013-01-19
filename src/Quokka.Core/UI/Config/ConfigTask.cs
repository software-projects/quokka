using Quokka.UI.Tasks;

namespace Quokka.UI.Config
{
	public class ConfigTask : UITask
	{
		protected override void CreateNodes()
		{
			var listNode = CreateNode();
			var editNode = CreateNode();
			var errorNode = CreateNode();

			listNode
				.SetPresenter<ListConfigPresenter>()
				.NavigateTo(p=> p.EditCommand, editNode)
				.NavigateTo(p => p.ErrorCommand, errorNode);

			editNode
				.SetPresenter<EditConfigPresenter>()
				.NavigateTo(p => p.SaveCommand, listNode)
				.NavigateTo(p => p.CancelCommand, listNode)
				.NavigateTo(p => p.ErrorCommand, errorNode);

			
			errorNode
				.SetPresenter<ErrorReportPresenter>()
				.NavigateTo(p => p.RetryCommand, listNode);
		}

		protected override void CreateState()
		{
			RegisterInstance(new ConfigTaskState());
		}
	}
}