using NUnit.Framework;
using Quokka.Uip.Fakes;

namespace Quokka.Uip
{
    [TestFixture]
    public class FakeAppTests2
    {
        // This test was created to diagnose a bug with the following characteristics:
        // 1. Controller created and transitions in its constructor, so its associated view is never created
        // 2. Next transition is back to the first node.
        // 3. The original controller was not disposed of, and was reused. This is wrong -- a new controller
        //    should be created.
        [Test]
        public void DoesNotReuseControllersAndViews()
        {
            FakeViewManager viewManager = new FakeViewManager();
            FakeTask task = new FakeTask();
            task.Start(viewManager);

            // should have transitioned directly to Node2
            FakeController2 controller2 = task.CurrentController as FakeController2;
            Assert.IsNotNull(controller2);

            controller2.Back();

            FakeController1 controller = task.CurrentController as FakeController1;
            Assert.IsNotNull(controller);

            // check that we were issued with a brand new controller, not the controller
            // recycled from the first transition
            Assert.AreNotSame(controller, task.State.FirstController);
        }

        public class FakeTask : UipTask<FakeState>
        {
            public static readonly UipNode Node1 = new UipNode();
            public static readonly UipNode Node2 = new UipNode();

            static FakeTask()
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
