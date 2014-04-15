#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

// disable obsolete warning
#pragma warning disable 612,618

// ReSharper disable CheckNamespace
namespace Quokka.Uip.MockApp
{
	public class MockTask : UipTask<MockState>
	{
		public readonly UipNode Node1 = new UipNode();
		public readonly UipNode Node2 = new UipNode();
		public readonly UipNode Node3 = new UipNode();
		public readonly UipNode Node5 = new UipNode();
		public readonly UipNode ErrorNode = new UipNode();
		public readonly UipNode NoViewNode = new UipNode();
	    public readonly UipNode NavigateInViewConstructorNode = new UipNode();
	    public readonly UipNode NavigateInViewLoadEvent = new UipNode();

		public MockTask()
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
