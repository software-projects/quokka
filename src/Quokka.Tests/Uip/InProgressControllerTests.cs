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
