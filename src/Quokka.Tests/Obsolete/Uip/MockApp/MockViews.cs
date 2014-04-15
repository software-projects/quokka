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

using Quokka.UI.Fakes;

namespace Quokka.Uip.MockApp
{
	using NUnit.Framework;

    public class MockViewBase : IFakeViewThatLoads
    {
        public virtual void OnLoad() {}
    }

	public class MockView1 : MockViewBase
    {
        private readonly IController controller;

        public interface IController
        {
            void Next();
            void Back();
            void End();
            void NavigateInViewLoad();
        	void View5();
        }

        // Similar to a Windows Form, where there is always a default
        // constructor. Good for design view, but the other constructor
        // is used when the app is running, as it has more arguments.
        public MockView1() { }

        public MockView1(IController controller)
            : this() {
            Assert.IsNotNull(controller);
            this.controller = controller;
        }

        // this method is called directly from Controller1
        public bool DoSomething()
        {
            return true;
        }

        public void PushNextButton() {
            controller.Next();
        }

        public void PushBackButton() {
            controller.Back();
        }

        public void PushNavigateInViewLoadButton()
        {
            controller.NavigateInViewLoad();
        }

        public void PushEndButton() {
            controller.End();
        }

		public void PushButtonForView5()
		{
			controller.View5();
		}

        public IController Controller {
            get { return controller; }
        }
    }

    public class MockView2 : MockViewBase
    {
        private readonly MockController2 controller;

        public MockView2() { }

        public MockView2(MockController2 controller)
            : this() {
            Assert.IsNotNull(controller);
            this.controller = controller;
        }

        public void PushNextButton() {
            controller.Next();
        }

        public void PushBackButton() {
            controller.Back();
        }

        public void ErrorCondition() {
            controller.Error();
        }
    }

    /// <summary>
    /// This view navigates inside its constructor
    /// </summary>
    public class MockView3 : MockViewBase
    {
        public MockView3() {}

        public MockView3(MockController3 controller) : this()
        {
            Assert.IsNotNull(controller);
            controller.Next();
        }
    }

    /// <summary>
    /// This view navigates while it is being loaded
    /// </summary>
    public class MockView4 : MockViewBase
    {
        private readonly MockController3 controller;

        public MockView4() { }

        public MockView4(MockController3 controller)
            : this()
        {
            Assert.IsNotNull(controller);
            this.controller = controller;
        }

        public override void OnLoad()
        {
            controller.Next();
        }
    }

	public class MockView5 : MockViewBase
	{
		private readonly INavigator _navigator;

		public interface INavigator
		{
			void Back();
		}

		public MockView5() {}

		public MockView5(INavigator navigator)
		{
			Assert.IsNotNull(navigator);
			_navigator = navigator;
		}

		public void PushBackButton()
		{
			_navigator.Back();
		}
	}
}
