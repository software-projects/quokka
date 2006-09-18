using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Quokka.Uip.MockApp
{
    public class MockController1
    {
        protected readonly IUipNavigator navigator;
        protected readonly IState state;

        public interface IState
        {
            string StringProperty { get; }
        }

        public MockController1(IUipNavigator navigator, IState state) {
            Assert.IsNotNull(navigator);
            Assert.IsNotNull(state);

            this.navigator = navigator;
            this.state = state;
        }

        public void Next() {
            navigator.Navigate("Next");
        }

        public void Back() {
            navigator.Navigate("Back");
        }
    }

    public class MockController2 : MockController1
    {
        public MockController2(IUipNavigator navigator, IState state)
            : base(navigator, state) {
        }

        public void Error() {
            navigator.Navigate("Error");
        }
    }

    public class MockController3
    {
        protected readonly IUipNavigator navigator;
        protected readonly MockState state;
        protected readonly IUipViewManager viewManager;

        public MockController3(MockState state, IUipNavigator navigator, UipTask task, IUipViewManager viewManager) {
            Assert.IsNotNull(state);
            Assert.IsNotNull(navigator);
            Assert.IsNotNull(task);
            this.state = state;
            this.navigator = navigator;

            // the view manager does not get used, just checking that it is passed if it is needed
            Assert.IsNotNull(viewManager);
            this.viewManager = viewManager;

            if (task.CurrentNode.Name == "NoViewNode") {
                state.SetByController3 = true;
                navigator.Navigate("Next");
            }
        }

        public void Next() {
            navigator.Navigate("Next");
        }
    }
}
