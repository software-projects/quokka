using Quokka.Uip;

namespace Example1
{
	internal class WelcomeTask : UipTask<WelcomeState>
	{
		public readonly UipNode StartNode = new UipNode();

		public WelcomeTask()
		{
			StartNode
				.SetView<WelcomeForm>()
				.SetPresenter<WelcomePresenter>();
		}
	}
}