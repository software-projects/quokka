#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006 John Jeffery. All rights reserved.
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
