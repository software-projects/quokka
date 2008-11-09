namespace Quokka.Uip.MockApp
{
	public class MockTask : UipTask<MockState>
	{
		public static readonly UipNode Node1 = new UipNode();
		public static readonly UipNode Node2 = new UipNode();
		public static readonly UipNode Node3 = new UipNode();
		public static readonly UipNode Node5 = new UipNode();
		public static readonly UipNode ErrorNode = new UipNode();
		public static readonly UipNode NoViewNode = new UipNode();
	    public static readonly UipNode NavigateInViewConstructorNode = new UipNode();
	    public static readonly UipNode NavigateInViewLoadEvent = new UipNode();

		static MockTask()
		{
			Node1.SetViewType(typeof (MockView1))
				.SetControllerType(typeof (MockController1))
				.NavigateTo("Next", Node2)
				.NavigateTo("Back", NoViewNode)
				.NavigateTo("End", null)
				.NavigateTo("NavigateInViewLoadEvent", NavigateInViewLoadEvent)
				.NavigateTo("View5", Node5);

			Node2
				.SetViewType(typeof(MockView2))
				.SetControllerType(typeof(MockController2))
				.NavigateTo("Back", Node1)
				.NavigateTo("Next", Node3)
				.NavigateTo("Error", ErrorNode);

		    Node3
		        .SetViewType(typeof (MockView1))
		        .SetControllerType(typeof (MockController2))
		        .NavigateTo("Next", NavigateInViewConstructorNode);

			Node5
				.SetViewType(typeof (MockView5))
				.NavigateTo("Back", Node1);

		    NavigateInViewConstructorNode
		        .SetViewType(typeof (MockView3))
		        .SetControllerType(typeof (MockController3))
		        .NavigateTo("Next", Node1);

		    NavigateInViewLoadEvent
		        .SetViewType(typeof (MockView4))
		        .SetControllerType(typeof (MockController3))
		        .NavigateTo("Next", Node2);

			ErrorNode
				.SetViewType(typeof(MockView2))
				.SetControllerType(typeof(MockController2))
				.NavigateTo("Next", Node1);

			NoViewNode
				.SetControllerType(typeof(MockController3))
				.NavigateTo("Next", Node3);
		}

	}
}
