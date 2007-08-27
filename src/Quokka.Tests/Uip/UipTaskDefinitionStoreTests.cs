namespace Quokka.Uip
{
	using System;
	using System.IO;
	using System.Reflection;
	using NUnit.Framework;
	using Quokka.Uip.MockApp;

	[TestFixture]
	public class UipTaskDefinitionStoreTests
	{
		private UipTaskDefinitionStore store;

		[SetUp]
		public void SetUp()
		{
			store = new UipTaskDefinitionStore();
		}

		[TearDown]
		public void TearDown()
		{
			store = null;
		}

		[Test]
		public void CanCreateTaskDefinitionFromStream()
		{
			store.Assemblies.Add(Assembly.GetExecutingAssembly());

			Stream stream = GetMockTaskStream();

			UipTaskDefinition taskDefinition = store.CreateTaskDefinitionFromStream(stream);

			Assert.IsNotNull(taskDefinition);
			Assert.AreEqual("MockTask", taskDefinition.Name);
		}

		[Test]
		public void FailsToCreateTaskDefinitionTwiceFromStream() {
			store.Assemblies.Add(Assembly.GetExecutingAssembly());

			Stream stream = GetMockTaskStream();

			Assert.IsInstanceOfType(typeof(UipTaskDefinition), store.CreateTaskDefinitionFromStream(stream));

			stream.Seek(0, SeekOrigin.Begin);
			try {
				store.CreateTaskDefinitionFromStream(stream);
				Assert.Fail("Should have thrown exception");
			}
			catch (UipTaskAlreadyExistsException) {}
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FailsToCreateTaskDefinitionWithNullStream()
		{
			store.CreateTaskDefinitionFromStream(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FailsToCreateTaskDefinitionWithNullName()
		{
			store.CreateTaskDefinition(null, typeof (MockState));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FailsToCreateTaskDefinitionWithNullStateType()
		{
			store.CreateTaskDefinition("TaskName", null);
		}

		[Test]
		public void CreatesEmptyTaskDefinition()
		{
			const string taskName = "MyTaskName";
			Type stateType = typeof (MockState);
			UipTaskDefinition taskDefinition = store.CreateTaskDefinition(taskName, stateType);
			Assert.IsNotNull(taskDefinition);
			Assert.AreEqual(taskName, taskDefinition.Name);
			Assert.AreEqual(stateType, taskDefinition.StateType);
			Assert.AreEqual(0, taskDefinition.Nodes.Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FailsToDefineTaskDefinitionWithNullTaskName()
		{
			store.DefineTask(null, CreateTaskDefinition);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FailsToDefineTaskDefinitionWithNullCallback()
		{
			store.DefineTask("Something", null);
		}

		[Test]
		public void FindTaskDefinition()
		{
			const string name = "xyz";
			UipTaskDefinition taskDefinition = store.CreateTaskDefinition(name, typeof (MockState));
			Assert.AreSame(taskDefinition, store[name]);
		}

		[Test]
		[ExpectedException(typeof(UipUnknownTaskException))]
		public void FailsWithUnknownTaskName()
		{
			UipTaskDefinition taskDefinition = store["SomeSillyTaskName"];
			Assert.Fail(taskDefinition.ToString()); // This line just keeps refactor happy
		}

		[Test]
		public void CreatesTaskDefinitionFromCallback()
		{
			const string taskName = "xyz";
			store.DefineTask(taskName, CreateTaskDefinition);

			UipTaskDefinition taskDefinition = store[taskName];
			Assert.IsNotNull(taskDefinition);
			Assert.AreEqual(taskName, taskDefinition.Name);

			// check that we only create one task definition object
			UipTaskDefinition taskDefinition2 = store[taskName];
			Assert.AreSame(taskDefinition, taskDefinition2);
		}

		[Test]
		[ExpectedException(typeof(UipTaskDefinitionNameMismatchException))]
		public void FailsWhenTaskDefinitionNameDoesNotMatch()
		{
			const string taskName = "xxx";
			store.DefineTask(taskName, CreateTaskDefinitionWithIncorrectName);

			UipTaskDefinition taskDefinition = store[taskName];
			Assert.Fail(taskDefinition.ToString()); // This line just keeps refactor happy
		}

		private static UipTaskDefinition CreateTaskDefinition(string taskName)
		{
			UipTaskDefinition taskDefinition = new UipTaskDefinition(taskName, typeof (MockState));
			// probably a good idea to add some nodes here.
			return taskDefinition;
		}

		private static UipTaskDefinition CreateTaskDefinitionWithIncorrectName(string taskName)
		{
			UipTaskDefinition taskDefinition = new UipTaskDefinition(taskName + "XXX", typeof(MockState));
			// probably a good idea to add some nodes here.
			return taskDefinition;
		}

		private static Stream GetMockTaskStream()
		{
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(MockState), "MockTask.xml");
			Assert.IsNotNull(stream);
			return stream;
		}
	}
}
