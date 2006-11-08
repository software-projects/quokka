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
using Quokka.Uip.Controllers;

namespace Quokka.Uip
{
    [TestFixture]
    public class InProgressControllerTests
    {
        private bool progressChanged;

        [Test]
        public void RaiseProgressChanged() {
            Controller controller = new Controller();
            controller.ProgressChanged += new EventHandler(controller_ProgressChanged);

            progressChanged = false;
            controller.ProgressSummary = "Progress Summary";
            AssertProgressChanged();
            controller.ProgressSummary = "Progress Summary";
            AssertProgressUnchanged();
            controller.ProgressSummary = "Progress Summary 1";
            AssertProgressChanged();

            controller.ProgressDetail = "Progress Detail";
            AssertProgressChanged();
            controller.ProgressDetail = "Progress Detail";
            AssertProgressUnchanged();
            controller.ProgressDetail = "Progress Detail 1";
            AssertProgressChanged();

            controller.ProgressMinimum = 10;
            AssertProgressChanged();
            controller.ProgressMinimum = 10;
            AssertProgressUnchanged();
            controller.ProgressMinimum = 0;
            AssertProgressChanged();

            controller.ProgressMaximum = 50;
            AssertProgressChanged();
            controller.ProgressMaximum = 50;
            AssertProgressUnchanged();
            controller.ProgressMaximum = 100;
            AssertProgressChanged();

            controller.ProgressValue = 10;
            AssertProgressChanged();
            controller.ProgressValue = 10;
            AssertProgressUnchanged();
            controller.ProgressValue = 0;
            AssertProgressChanged();

            controller.CanCancel = true;
            AssertProgressChanged();
            controller.CanCancel = true;
            AssertProgressUnchanged();
            controller.CanCancel = false;
            AssertProgressChanged();

            controller.CanCancel = true;
            controller.Cancel();
            AssertProgressChanged();
        }

        [Test]
        public void ProgressMinimumAndMaximum() {
            Controller controller = new Controller();
            controller.ProgressMinimum = 10;
            Assert.IsTrue(controller.ProgressMinimum <= controller.ProgressMaximum);

            controller.ProgressMaximum = 5;
            Assert.IsTrue(controller.ProgressMinimum <= controller.ProgressMaximum);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CancelNotSupported() {
            Controller controller = new Controller();
            controller.Cancel();
        }

        [Test]
        public void CancelSupported() {
            Controller controller = new Controller();
            controller.CanCancel = true;
            Assert.IsFalse(controller.CancelRequested);
            Assert.IsFalse(controller.OnCancelCalled);
            controller.Cancel();
            Assert.IsTrue(controller.CancelRequested);
            Assert.IsTrue(controller.OnCancelCalled);
        }

        void controller_ProgressChanged(object sender, EventArgs e) {
            progressChanged = true;
        }

        void AssertProgressChanged() {
            Assert.IsTrue(progressChanged);
            progressChanged = false;
        }

        void AssertProgressUnchanged() {
            Assert.IsFalse(progressChanged);
        }

        public class Controller : InProgressControllerBase
        {
            private bool onCancelCalled;

            public bool OnCancelCalled {
                get { return onCancelCalled; }
            }

            public override void DoWork() {
                throw new Exception("The method or operation is not implemented.");
            }

            public override void FinishedWork() {
                throw new Exception("The method or operation is not implemented.");
            }

            protected override void OnCancelRequested() {
                onCancelCalled = true;
            }
        }
    }
}
