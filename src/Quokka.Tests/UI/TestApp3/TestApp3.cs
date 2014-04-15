#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using NUnit.Framework;
using Quokka.Castle;
using Quokka.ServiceLocation;
using Quokka.UI.Fakes;
using Quokka.UI.Tasks;

// ReSharper disable InconsistentNaming

namespace Quokka.UI.TestApp3
{
	[TestFixture]
	public class TestApp3Tests
	{
		private FakeViewDeck _viewDeck;

		[SetUp]
		public void SetUp()
		{
			_viewDeck = new FakeViewDeck();
			var serviceContainer = ServiceContainerFactory.CreateContainer();
			var locator = serviceContainer.Locator;
			ServiceLocator.SetLocatorProvider(() => locator);
		}

		[TearDown]
		public void TearDown()
		{
			_viewDeck = null;
			ServiceLocator.SetLocatorProvider(null);
		}

		[Test]
		public void Presenter_navigates_during_initialization()
		{
			var task = new TestTask3();
			task.Start(_viewDeck);
			Assert.AreEqual("Node2", task.CurrentNode.Name);
		}

		[Test]
		public void Multiple_presenters_navigate_during_initialization()
		{
			var task = new TestTask3();
			task.Start(_viewDeck);
			Assert.AreEqual("Node2", task.CurrentNode.Name);
			var presenter2 = task.CurrentNode.Presenter as Presenter2;
			Assert.IsNotNull(presenter2);
			presenter2.NextCommand.Navigate();
			Assert.AreEqual("Node6", task.CurrentNode.Name);
		}
	}

	public class TestTask3 : UITask
	{
		protected override void CreateNodes()
		{
			var node1 = CreateNode("Node1");
			var node2 = CreateNode("Node2");
			var node3 = CreateNode("Node3");
			var node4 = CreateNode("Node4");
			var node5 = CreateNode("Node5");
			var node6 = CreateNode("Node6");

			node1
				.SetPresenter<Presenter1>()
				.NavigateTo(p => p.NextCommand, node2);

			node2
				.SetPresenter<Presenter2>()
				.NavigateTo(p => p.NextCommand, node3);

			node3
				.SetPresenter<Presenter1>()
				.NavigateTo(p => p.NextCommand, node4);

			node4
				.SetPresenter<Presenter1>()
				.NavigateTo(p => p.NextCommand, node5);

			node5
				.SetPresenter<Presenter1>()
				.NavigateTo(p => p.NextCommand, node6);

			node6
				.SetPresenter<Presenter2>();
		}
	}

	public class Presenter1 : Presenter
	{
		public INavigateCommand NextCommand { get; set; }

		public override void InitializePresenter()
		{
			NextCommand.Navigate();
		}
	}

	public class Presenter2 : Presenter
	{
		public INavigateCommand NextCommand { get; set; }
	}

}