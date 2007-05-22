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

namespace Quokka.Uip
{
	using System;
	using NUnit.Framework;
	using Quokka.Uip.MockApp;

	[TestFixture]
    public class MockAppTests
    {
        bool taskCompleted;

        [SetUp]
        public void SetUp() {
            UipManager.Clear();
            taskCompleted = false;
        }

        [TearDown]
        public void TearDown() {
            UipManager.Clear();
        }

        [Test]
        public void RunMockAppFromXmlDefinition() {
            UipManager.AddAssembly(this.GetType().Assembly);
            UipManager.LoadTaskDefinition(typeof(MockState), "MockTask.xml");
            MockViewManager viewManager = new MockViewManager();
            UipTask task = UipManager.CreateTask("MockTask", viewManager);
            Assert.IsNotNull(task);

            RunMockAppHelper(task, viewManager);
        }

        [Test]
        public void RunMockAppFromCodeDefinition() {
            UipTaskDefinition taskDefinition = new UipTaskDefinition("MockTask", typeof(MockState));
            taskDefinition.StateProperties.Add("StringProperty", "Set from config");
            UipNode node1 = taskDefinition.AddNode("Node1", typeof(MockView1), typeof(MockController1))
                .NavigateTo("Next", "Node2")
                .NavigateTo("Back", "NoViewNode")
                .NavigateTo("End", "__end__");
            UipNode node2 = taskDefinition.AddNode("Node2", typeof(MockView2), typeof(MockController2))
                .NavigateTo("Back", node1)
                .NavigateTo("Next", "Node3")
                .NavigateTo("Error", "ErrorNode");
            UipNode node3 = taskDefinition.AddNode("Node3", typeof(MockView1), typeof(MockController2))
                .NavigateTo("Next", node1);
            UipNode errorNode = taskDefinition.AddNode("ErrorNode", typeof(MockView2), typeof(MockController2))
                .NavigateTo("Next", node1);
            UipNode noViewNode = taskDefinition.AddNode("NoViewNode", null, typeof(MockController3))
                .NavigateTo("Next", node3);

            MockViewManager viewManager = new MockViewManager();
            UipTask task = UipManager.CreateTask(taskDefinition, viewManager);
            Assert.IsNotNull(task);

            RunMockAppHelper(task, viewManager);
        }

        private void RunMockAppHelper(UipTask task, MockViewManager viewManager) {
            Assert.IsNotNull(task);

            Assert.IsInstanceOfType(typeof(MockState), task.State);
            MockState state = (MockState)task.State;

            // check that the state property was set from the configuration file
            Assert.AreEqual("Set from config", state.StringProperty);

            task.Start();
            task.TaskComplete += new EventHandler(task_TaskComplete);

            Assert.AreEqual("Node1", task.CurrentNode.Name);
            Assert.IsNotNull(task.CurrentController);
            Assert.IsInstanceOfType(typeof(MockController1), task.CurrentController);
            Assert.IsNotNull(viewManager.VisibleView);
            Assert.IsInstanceOfType(typeof(MockView1), viewManager.VisibleView);

            MockView1 view1 = (MockView1)viewManager.VisibleView;
            view1.PushNextButton();

            Assert.AreEqual("Node2", task.CurrentNode.Name);
            Assert.IsNotNull(task.CurrentController);
            Assert.IsInstanceOfType(typeof(MockController2), task.CurrentController);
            Assert.IsNotNull(viewManager.VisibleView);
            Assert.IsInstanceOfType(typeof(MockView2), viewManager.VisibleView);

            MockView2 view2 = (MockView2)viewManager.VisibleView;
            view2.PushNextButton();

            Assert.AreEqual("Node3", task.CurrentNode.Name);
            Assert.IsNotNull(task.CurrentController);
            Assert.IsInstanceOfType(typeof(MockController2), task.CurrentController);
            Assert.IsNotNull(viewManager.VisibleView);
            Assert.IsInstanceOfType(typeof(MockView1), viewManager.VisibleView);

            view1 = (MockView1)viewManager.VisibleView;
            view1.PushNextButton();

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
            Assert.IsFalse(taskCompleted);
            view1.PushEndButton();
            Assert.IsTrue(taskCompleted);
            Assert.IsTrue(task.IsComplete);
            Assert.IsFalse(task.IsRunning);
        }

        void task_TaskComplete(object sender, EventArgs e) {
            taskCompleted = true;
        }
    }
}
