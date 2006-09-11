using System;
using System.Reflection;
using NUnit.Framework;

using Quokka.Uip.MockApp;

namespace Quokka.Uip
{
    [TestFixture]
    public class UipManagerTests
    {
        [SetUp]
        public void Setup() {
            UipManager.Clear();
        }

        [TearDown]
        public void Teardown() {
            UipManager.Clear();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadTaskDefinition_ArgumentNull_1a() {
            UipManager.LoadTaskDefinition(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadTaskDefinition_ArgumentNull_1b() {
            UipManager.LoadTaskDefinition(null, "task.xml");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadTaskDefinition_ArgumentNull_2() {
            UipManager.LoadTaskDefinition(this.GetType(), null);
        }

        [Test]
        [ExpectedException(typeof(UipException))]
        public void LoadTaskDefinition_NotFound() {
            UipManager.LoadTaskDefinition(this.GetType(), "UndefinedTask.xml");
        }

        [Test]
        public void LoadTaskDefinitions() {
            UipManager.AddAssembly(Assembly.GetExecutingAssembly());
            UipManager.LoadTaskDefinitions(typeof(MockState), "MockTask.xml", "MockTask2.xml");
            IUipViewManager viewManager = new MockViewManager();

            UipTask task1 = UipManager.CreateTask("MockTask", viewManager);
            Assert.IsNotNull(task1);
            UipTask task2 = UipManager.CreateTask("MockTask2", viewManager);
            Assert.IsNotNull(task2);
        }
    }
}
