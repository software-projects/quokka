using Quokka.Uip;

namespace Example2.Tasks.Simple
{
	public class SimpleTask : UipTask<SimpleState>
	{
		public readonly UipNode StartNode = new UipNode();

		public SimpleTask()
		{
			StartNode
				.SetView<SimpleView>()
				.SetPresenter<SimplePresenter>()
				.NavigateTo("end", null);
		}
	}
}