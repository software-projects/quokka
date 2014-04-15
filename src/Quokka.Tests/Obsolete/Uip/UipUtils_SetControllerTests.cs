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

// disable obsolete warning
#pragma warning disable 612,618

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
namespace Quokka.Uip
{
    [TestFixture]
    public class UipUtils_SetControllerTests
    {
        #region Controller classes used for testing

        public class Controller1
        {
            public bool DoneSomething;

            public void DoSomething() {
                DoneSomething = true;
            }
        }

        public class Controller2
        {
            public bool DidSomething;

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
            public IController Controller;
            public interface IController
            {
                void DoSomething();
            }

            public void SetController(IController controller) {
                Controller = controller;
            }

            public void DoSomething() {
                Controller.DoSomething();
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
                Controller = controller;
            }

            public void DoSomething() {
                Controller.DoDifferentThing();
            }
        }

        #endregion

        [Test]
        public void ViewWithProxyController() {
            var view = new View1();
            var controller = new Controller1();

            Assert.IsNull(view.Controller);
            UipUtil.SetController(view, controller, false);
            Assert.IsNotNull(view.Controller);

            Assert.IsFalse(controller.DoneSomething);
            view.DoSomething();
            Assert.IsTrue(controller.DoneSomething);
        }

        [Test]
        public void ViewWithMissingMethod() {
            var view = new ViewWithoutSetController();
            var controller = new Controller1();
            Assert.Throws<QuokkaException>(() => UipUtil.SetController(view, controller, true));
        }

        [Test]
        public void ViewWithTooManyParametersForSetController() {
            var view = new ViewWithIncorrectNumberOfParametersForSetController();
            var controller = new Controller1();
            Assert.Throws<QuokkaException>(() => UipUtil.SetController(view, controller, true));
        }

        [Test]
        public void IncorrectControllerType() {
            var view = new ViewRequiringController2();
            var controller = new Controller1();
            Assert.Throws<QuokkaException>(() => UipUtil.SetController(view, controller, true));
        }

        [Test]
        public void NoProxyRequired() {
            var view = new ViewRequiringController2();
            var controller = new Controller2();

            Assert.IsNull(view.Controller);
            UipUtil.SetController(view, controller, false);
            Assert.AreSame(controller, view.Controller);

            Assert.IsFalse(controller.DidSomething);
            view.DoSomething();
            Assert.IsTrue(controller.DidSomething);
        }

        [Test]
        public void NoProxyRequiredInheritedClass() {
            var view = new ViewRequiringController2();
            var controller = new Controller3();

            Assert.IsNull(view.Controller);
            UipUtil.SetController(view, controller, false);
            Assert.AreSame(controller, view.Controller);

            Assert.IsFalse(controller.DidSomething);
            view.DoSomething();
            Assert.IsTrue(controller.DidSomething);
        }
    }
}
