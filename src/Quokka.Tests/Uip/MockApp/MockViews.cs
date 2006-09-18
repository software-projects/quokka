using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Quokka.Uip.MockApp
{
    public class MockView1
    {
        private readonly IController controller;

        public interface IController
        {
            void Next();
            void Back();
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

        public void PushNextButton() {
            controller.Next();
        }

        public void PushBackButton() {
            controller.Back();
        }

        public IController Controller {
            get { return controller; }
        }
    }

    public class MockView2
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
}
