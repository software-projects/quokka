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

using NUnit.Framework;
using Quokka.ServiceLocation;
using Quokka.Castle;
using Quokka.UI.Fakes;

// disable obsolete warning
#pragma warning disable 612,618

// ReSharper disable CheckNamespace
namespace Quokka.Uip
{
    [TestFixture]
    public class FakeAppTests2
    {
    	private IServiceLocator _serviceLocator;

		[SetUp]
		public void SetUp()
		{
			_serviceLocator = ServiceContainerFactory.CreateContainer().Locator;
			ServiceLocator.SetLocatorProvider(() => _serviceLocator);
		}

        // This test was created to diagnose a bug with the following characteristics:
        // 1. Controller created and transitions in its constructor, so its associated view is never created
        // 2. Next transition is back to the first node.
        // 3. The original controller was not disposed of, and was reused. This is wrong -- a new controller
        //    should be created.
        [Test]
        public void DoesNotReuseControllersAndViews()
        {
            var viewManager = new FakeViewDeck();
            var task = new FakeTask();
            task.Start(viewManager);

            // should have transitioned directly to Node2
            var controller2 = task.CurrentController as FakeController2;
            Assert.IsNotNull(controller2);

            controller2.Back();

            var controller = task.CurrentNode.Controller as FakeController1;
            Assert.IsNotNull(controller);

            // check that we were issued with a brand new controller, not the controller
            // recycled from the first transition
            Assert.AreNotSame(controller, task.State.FirstController);
        }

        public class FakeTask : UipTask<FakeState>
        {
            public readonly UipNode Node1 = new UipNode();
            public readonly UipNode Node2 = new UipNode();

            public FakeTask()
            {
                Node1
                    .SetControllerType(typeof(FakeController1))
                    .SetViewType(typeof(FakeController2))
                    .NavigateTo("Next", Node2);

                Node2
                    .SetControllerType(typeof(FakeController2))
                    .SetViewType(typeof(FakeView2))
                    .NavigateTo("Back", Node1);
            }
        }

        public class FakeState
        {
            public object FirstController;
        }

        public class FakeView1 { }

        public class FakeController1
        {
            public FakeController1(IUipNavigator navigator, FakeState state)
            {
                // remember this controller and navigate to the next node
                // if this is the first time through
                if (state.FirstController == null)
                {
                    state.FirstController = this;
                    navigator.Navigate("Next");
                }
            }
        }

        public class FakeView2 {}

        public class FakeController2
        {
            private readonly IUipNavigator _navigator;

            public FakeController2(IUipNavigator navigator)
            {
                Assert.IsNotNull(navigator);
                _navigator = navigator;
            }

            public void Back()
            {
                _navigator.Navigate("Back");
            }
        }
    }
}
