#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

namespace Quokka.Uip.MockApp
{
	using NUnit.Framework;

	public class MockController1
    {
        protected readonly IUipNavigator _navigator;
        protected readonly IState _state;
	    private IView _view;

        public interface IState
        {
            string StringProperty { get; }
        }

        public interface IView
        {
            bool CanDoSomething { get; }
            bool DoSomething();
        }

        public MockController1(IUipNavigator navigator, IState state) {
            Assert.IsNotNull(navigator);
            Assert.IsNotNull(state);

            _navigator = navigator;
            _state = state;
        }

        public void SetView(IView view)
        {
            _view = view;
        }

        public void Next() {
            // This is testing that the UIP framework injects the view when
            // the controller provides a 'SetView' method.
            Assert.IsNotNull(_view);
            if (_view.CanDoSomething)
                Assert.IsTrue(_view.DoSomething());

            _navigator.Navigate("Next");
        }

        public void Back() {
            _navigator.Navigate("Back");
        }

        public void End() {
            _navigator.Navigate("End");
        }

        public void NavigateInViewLoad()
        {
            _navigator.Navigate("NavigateInViewLoadEvent");
        }

		public void View5()
		{
			_navigator.Navigate("View5");
		}
    }

    public class MockController2 : MockController1
    {
        public MockController2(IUipNavigator navigator, IState state)
            : base(navigator, state) {
        }

        public void Error() {
            _navigator.Navigate("Error");
        }
    }

    public class MockController3
    {
		public interface INavigator
		{
			void Next();
		}

        protected readonly INavigator navigator;
        protected readonly MockState state;
        protected readonly IUipViewManager viewManager;

        public MockController3(MockState state, INavigator navigator, UipTask task, IUipViewManager viewManager) {
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
            	navigator.Next();
            }
        }

        public void Next() {
        	navigator.Next();
        }
    }
}
