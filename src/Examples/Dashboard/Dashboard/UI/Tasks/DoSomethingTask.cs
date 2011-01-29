using Dashboard.UI.Views;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Tasks
{
	public class DoSomethingTask : UITask
	{
		protected override void CreateNodes()
		{
			var node1 = CreateNode();
			var node2 = CreateNode();

			node1
				.SetView<DoSomething1View>()
				.NavigateTo(v => v.NextCommand, node2);

			node2
				.SetView<DoSomething2View>()
				.NavigateTo(v => v.CloseCommand, null);
		}
	}
}