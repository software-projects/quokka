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
    }

    public class MockController2 : MockController1
    {
        public MockController2(IUipNavigator navigator, IState state)
            : base(navigator, state) {
        }

        public void Back() {
            navigator.Navigate("Back");
        }

        public void Error() {
            navigator.Navigate("Error");
        }
    }

    public class MockController3
    {
        protected readonly IUipNavigator navigator;
        protected readonly MockState state;

        public MockController3(MockState state, IUipNavigator navigator) {
            Assert.IsNotNull(state);
            Assert.IsNotNull(navigator);
            this.state = state;
            this.navigator = navigator;
        }

        public void Next() {
            navigator.Navigate("Next");
        }
    }
}
