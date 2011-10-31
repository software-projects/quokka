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

using System;
using NUnit.Framework;

namespace Quokka.Uip
{
    [TestFixture]
    public class UipUtils_SetControllerTests
    {
        #region Controller classes used for testing

        public class Controller1
        {
            public bool DoneSomething = false;

            public void DoSomething() {
                DoneSomething = true;
            }
        }

        public class Controller2
        {
            public bool DidSomething = false;

            public void DoDifferentThing() {
                DidSomething = true;
            }
        }

        public class Controller3 : Controller2
        {
            public void DoNothing() { }
        }

        #endregion

        #region View classes used for testing

        public class View1
        {
            public IController controller;
            public interface IController
            {
                void DoSomething();
            }

            public void SetController(IController controller) {
                this.controller = controller;
            }

            public void DoSomething() {
                controller.DoSomething();
            }
        }

        public class ViewWithoutSetController
        {
            public void NotSetController() {
            }
        }

        public class ViewWithIncorrectNumberOfParametersForSetController
        {
            public void SetController(object p1, object p2) {
            }
        }

        public class ViewRequiringController2
        {
            public Controller2 Controller;

            public void SetController(Controller2 controller) {
                this.Controller = controller;
            }

            public void DoSomething() {
                Controller.DoDifferentThing();
            }
        }

        #endregion

        [Test]
        public void ViewWithProxyController() {
            View1 view = new View1();
            Controller1 controller = new Controller1();

            Assert.IsNull(view.controller);
            UipUtil.SetController(view, controller, false);
            Assert.IsNotNull(view.controller);

            Assert.IsFalse(controller.DoneSomething);
            view.DoSomething();
            Assert.IsTrue(controller.DoneSomething);
        }

        [Test]
        [ExpectedException(typeof(QuokkaException))]
        public void ViewWithMissingMethod() {
            ViewWithoutSetController view = new ViewWithoutSetController();
            Controller1 controller = new Controller1();
            UipUtil.SetController(view, controller, true);
        }

        [Test]
        [ExpectedException(typeof(QuokkaException))]
        public void ViewWithTooManyParametersForSetController() {
            ViewWithIncorrectNumberOfParametersForSetController view = new ViewWithIncorrectNumberOfParametersForSetController();
            Controller1 controller = new Controller1();
            UipUtil.SetController(view, controller, true);
        }

        [Test]
        [ExpectedException(typeof(QuokkaException))]
        public void IncorrectControllerType() {
            ViewRequiringController2 view = new ViewRequiringController2();
            Controller1 controller = new Controller1();
            UipUtil.SetController(view, controller, true);
        }

        [Test]
        public void NoProxyRequired() {
            ViewRequiringController2 view = new ViewRequiringController2();
            Controller2 controller = new Controller2();

            Assert.IsNull(view.Controller);
            UipUtil.SetController(view, controller, false);
            Assert.AreSame(controller, view.Controller);

            Assert.IsFalse(controller.DidSomething);
            view.DoSomething();
            Assert.IsTrue(controller.DidSomething);
        }

        [Test]
        public void NoProxyRequiredInheritedClass() {
            ViewRequiringController2 view = new ViewRequiringController2();
            Controller3 controller = new Controller3();

            Assert.IsNull(view.Controller);
            UipUtil.SetController(view, controller, false);
            Assert.AreSame(controller, view.Controller);

            Assert.IsFalse(controller.DidSomething);
            view.DoSomething();
            Assert.IsTrue(controller.DidSomething);
        }
    }
}
