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
using Quokka.ServiceLocation;
using Quokka.UI.Fakes;
using Quokka.Uip.MockApp;
using Quokka.Castle;

// disable obsolete warning
#pragma warning disable 612,618

// ReSharper disable CheckNamespace
namespace Quokka.Uip
{
    [TestFixture]
    public class MockAppTests
    {
        bool _taskCompleted;
    	private IServiceLocator _serviceLocator;

        [SetUp]
        public void SetUp() {
            _taskCompleted = false;

        	_serviceLocator = ServiceContainerFactory.CreateContainer().Locator;
        	ServiceLocator.SetLocatorProvider(() => _serviceLocator);
        }

        [TearDown]
        public void TearDown() {
        	ServiceLocator.SetLocatorProvider(null);
        }


        [Test]
        public void RunMockApp() {
        	var viewManager = new FakeViewDeck();
        	var task = new MockTask();
            RunMockAppHelper(task, viewManager);
        }


        private void RunMockAppHelper(UipTask task, FakeViewDeck viewManager) {
            Assert.IsNotNull(task);

            Assert.IsInstanceOfType(typeof(MockState), task.GetStateObject());
            var state = (MockState)task.GetStateObject();

        	task.Start(viewManager);
            task.TaskComplete += task_TaskComplete;

            Assert.AreEqual("Node1", task.CurrentNode.Name);
            Assert.IsNotNull(task.CurrentController);
            Assert.IsInstanceOf<MockController1>(task.CurrentController);
            Assert.IsNotNull(viewManager.VisibleView);
            Assert.IsInstanceOf<MockView1>(viewManager.VisibleView);

            var view1 = (MockView1)viewManager.VisibleView;
            view1.PushNextButton();

            // Should be in Node2

            Assert.AreEqual("Node2", task.CurrentNode.Name);
            Assert.IsNotNull(task.CurrentController);
            Assert.IsInstanceOfType(typeof(MockController2), task.CurrentController);
            Assert.IsNotNull(viewManager.VisibleView);
            Assert.IsInstanceOfType(typeof(MockView2), viewManager.VisibleView);

            var view2 = (MockView2)viewManager.VisibleView;
            view2.PushNextButton();

            // Should be in Node3 

            Assert.AreEqual("Node3", task.CurrentNode.Name);
            Assert.IsNotNull(task.CurrentController);
            Assert.IsInstanceOfType(typeof(MockController2), task.CurrentController);
            Assert.IsNotNull(viewManager.VisibleView);
            Assert.IsInstanceOfType(typeof(MockView1), viewManager.VisibleView);

            view1 = (MockView1)viewManager.VisibleView;
            view1.PushNextButton();

            // Should navigate to NavigateInViewConstructorNode and then straight to Node1

            Assert.AreEqual("Node1", task.CurrentNode.Name);
            Assert.IsNotNull(task.CurrentController);
            Assert.IsInstanceOfType(typeof(MockController1), task.CurrentController);
            Assert.IsNotNull(viewManager.VisibleView);
            Assert.IsInstanceOfType(typeof(MockView1), viewManager.VisibleView);

            view1 = (MockView1)viewManager.VisibleView;
            Assert.IsFalse(state.SetByController3);
            view1.PushBackButton();
            Assert.IsTrue(state.SetByController3);

            Assert.AreEqual("Node3", task.CurrentNode.Name);
            Assert.IsNotNull(task.CurrentController);
            Assert.IsInstanceOfType(typeof(MockController2), task.CurrentController);
            Assert.IsNotNull(viewManager.VisibleView);
            Assert.IsInstanceOfType(typeof(MockView1), viewManager.VisibleView);

            view1.PushNextButton();

            Assert.AreEqual("Node1", task.CurrentNode.Name);
            Assert.IsTrue(task.IsRunning);
            Assert.IsFalse(task.IsComplete);
            Assert.IsFalse(_taskCompleted);

            // Should navigate to NavigateInViewLoadEvent and then straight to Node2

            view1.PushNavigateInViewLoadButton();
            Assert.AreEqual("Node2", task.CurrentNode.Name);

            view2 = (MockView2) viewManager.VisibleView;
            view2.PushBackButton();

			Assert.AreEqual("Node1", task.CurrentNode.Name);
        	view1 = (MockView1) viewManager.VisibleView;
        	view1.PushButtonForView5();

        	Assert.AreEqual("Node5", task.CurrentNode.Name);
        	var view5 = (MockView5) viewManager.VisibleView;
			view5.PushBackButton();

            Assert.AreEqual("Node1", task.CurrentNode.Name);
            view1 = (MockView1)viewManager.VisibleView;
            view1.PushEndButton();
            Assert.IsTrue(_taskCompleted);
            Assert.IsTrue(task.IsComplete);
            Assert.IsFalse(task.IsRunning);
        }

        void task_TaskComplete(object sender, EventArgs e) {
            _taskCompleted = true;
        }
    }
}
