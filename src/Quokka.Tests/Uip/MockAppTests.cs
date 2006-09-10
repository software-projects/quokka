using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Quokka.Uip.MockApp;

namespace Quokka.Uip
{
    [TestFixture]
    public class MockAppTests
    {
        [SetUp]
        public void SetUp() {
            UipManager.Clear();
        }

        [TearDown]
        public void TearDown() {
            UipManager.Clear();
        }

        [Test]
        public void Test1() {
            UipManager.AddAssembly(this.GetType().Assembly);
            UipManager.LoadTaskDefinition(typeof(MockState), "MockTask.xml");
            MockViewManager viewManager = new MockViewManager();
            UipTask task = UipManager.CreateTask("MockTask", viewManager);
            Assert.IsNotNull(task);

            task.Start();

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
        }
    }
}
