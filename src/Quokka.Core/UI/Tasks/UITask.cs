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
using System.Collections.Generic;
using Castle.Core.Logging;
using Quokka.Collections;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;
using Quokka.UI.Messages;

namespace Quokka.UI.Tasks
{
	public abstract class UITask : IUITask, IDisposable
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private IList<UINode> _nodes;
		private TaskBuilder _taskBuilder;
		private bool _canCreateNode;
		private UINode _nextNode;
		private IViewDeck _viewDeck;
		private readonly IServiceContainer _serviceContainer;
		private bool _inNavigateMethod;
		private bool _endTaskRequested;
		private bool _raiseTaskComplete;
		private bool _raiseTaskStarted;
		protected readonly DisposableCollection Disposables = new DisposableCollection();

		protected UITask()
		{
			// may be overridden by a derived class
			Name = GetType().Name;

			_serviceContainer = CreateServiceContainer();

			// Registers the task instance under both UITask, and the actual type of the task
			_serviceContainer.RegisterInstance(this);
			_serviceContainer.RegisterInstance(GetType(), this);
		}

		~UITask()
		{
			Dispose(false);
		}

		#region Public events

		public event EventHandler TaskStarted;
		public event EventHandler TaskComplete;

		#endregion

		#region Public properties

		public string Name { get; protected set; }

		public IServiceContainer ServiceContainer
		{
			get { return _serviceContainer; }
		}

		public IServiceLocator ServiceLocator
		{
			get { return _serviceContainer.Locator; }
		}

		public IViewDeck ViewDeck
		{
			get { return _viewDeck; }
		}

		public UINode CurrentNode { get; private set; }

		public IList<UINode> Nodes
		{
			get
			{
				if (_nodes == null)
				{
					BuildNodes();
				}
				return _nodes;
			}
		}

		public bool IsRunning
		{
			get { return CurrentNode != null; }
		}

		public bool IsComplete { get; private set; }

		#endregion

		#region Internal properties

		public bool InNavigateMethod
		{
			get { return _inNavigateMethod; }
		}

		#endregion

		#region Public methods

		public void Start(IViewDeck viewDeck)
		{
			Verify.ArgumentNotNull(viewDeck, "viewDeck");
			if (IsRunning)
			{
				string message = string.Format("Task {0} is already running", Name);
				Log.Error(message);
				throw new InvalidOperationException(message);
			}
			if (IsComplete)
			{
				// When a task has completed, there is
				// quite a bit of cleaning up to do to re-use it. At the moment it is easier to
				// just have the restriction that UI tasks cannot be reused.
				const string message = "A task can only be run once.";
				Log.Error(message);
				throw new InvalidOperationException(message);
			}

			// If the nodes have not been built yet, then build them
			if (_nodes == null)
			{
				BuildNodes();
			}

			// throw an exception if the task is not valid
			_taskBuilder.AssertValid();

			// If you have a UI task that is run more than once you
			// are going to have a problem, because each time you might be using a different IViewManager.
			// One way to deal with this is to have (yet another) child container that is created each
			// time the UI task is started.
			//
			// The way this is solved at the moment is to prevent re-use of UI tasks. See test before this
			// for avoiding re-use of tasks.
			_viewDeck = viewDeck;
			_serviceContainer.RegisterInstance(_viewDeck);
			_serviceContainer.RegisterType<UIMessageBox>(ServiceLifecycle.PerRequest);

			CreateState();

			// Now that the task has a view deck, we can inform the nodes and let them initialize.
			foreach (var node in Nodes)
			{
				node.TaskStarting();
			}

			Navigate(Nodes[0]);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Disposables.Dispose();
			Dispose(true);

			if (IsComplete)
			{
				return;
			}

			CurrentNode = null;
			IsComplete = true;

			foreach (var node in Nodes)
			{
				node.CleanupNode();
			}

			CurrentNode = null;
			IsComplete = true;
			_raiseTaskComplete = true;
			RaiseEvents();
		}

		protected virtual void Dispose(bool disposing)
		{
			
		}

		/// <summary>
		/// End the task.
		/// </summary>
		/// <remarks>
		/// TODO: Need to determine whether this method is needed.
		/// </remarks>
		public void EndTask()
		{
			_endTaskRequested = true;
			Navigate(null);
		}

		#endregion

		#region Protected overrides

		protected virtual void OnTaskStarted(EventArgs e)
		{
			if (TaskStarted != null)
			{
				TaskStarted(this, e);
			}
		}

		protected virtual void OnTaskComplete(EventArgs e)
		{
			if (TaskComplete != null)
			{
				TaskComplete(this, e);
			}
		}

		protected abstract void CreateNodes();

		protected virtual void CreateState() {}

		#endregion

		#region Protected methods

		protected void RegisterInstance<T>(T instance)
		{
			ServiceContainer.RegisterInstance(typeof (T), instance);
		}

		protected IAnyNodeBuilder CreateNode()
		{
			return CreateNode(null);
		}

		protected IAnyNodeBuilder CreateNode(string nodeName)
		{
			if (!_canCreateNode)
			{
				throw new InvalidOperationException(
					"Cannot call CreateNode() at this point. Call CreateNode() in the CreateNodes() method only.");
			}

			return _taskBuilder.CreateNode(nodeName);
		}

		#endregion

		#region Private methods

		private void BuildNodes()
		{
			// find the task template for this task type
			_taskBuilder = TaskBuilderStorage.FindForType(GetType());
			if (_taskBuilder == null)
			{
				_taskBuilder = new TaskBuilder(GetType());
				_canCreateNode = true;
				try
				{
					CreateNodes();
				}
				finally
				{
					_canCreateNode = false;
				}
				if (_taskBuilder.IsValid)
				{
					TaskBuilderStorage.Add(_taskBuilder);
				}
			}

			var nodes = new List<UINode>();

			foreach (var nodeBuilder in _taskBuilder.Nodes)
			{
				UINode node = new UINode(this, nodeBuilder);
				nodes.Add(node);
			}
			_nodes = nodes;
		}

		/// <summary>
		/// 	Create a service container that is a child container of the global container.
		/// </summary>
		/// <returns>
		/// 	An <see cref = "IServiceContainer" />.
		/// </returns>
		private static IServiceContainer CreateServiceContainer()
		{
			IServiceLocator serviceLocator;

			try
			{
				serviceLocator = Quokka.ServiceLocation.ServiceLocator.Current;
			}
			catch (NullReferenceException)
			{
				const string message = "ServiceLocator.Current has not been specified";
				Log.Error(message);
				throw new UITaskException(message);
			}

			try
			{
				IServiceContainer parentContainer = serviceLocator.GetInstance<IServiceContainer>();
				return parentContainer.CreateChildContainer();
			}
			catch (ActivationException ex)
			{
				const string message = "Cannot create child service container for UI Task."
				                       + " The most likely cause is that Quokka.ServiceLocation.IServiceContainer"
				                       + " is not implemented.";
				Log.Error(message);
				throw new UITaskException(message, ex);
			}
		}

		internal void Navigating(object sender, EventArgs e)
		{
			var navigateCommand = (NavigateCommand) sender;

			if (IsComplete)
			{
				string message = string.Format("Attempt to navigate from {0} to {1} when task has already completed",
				                               navigateCommand.FromNode,
				                               navigateCommand.ToNode);
				Log.Error(message);
				throw new UITaskException(message);
			}

			if (navigateCommand.FromNode != CurrentNode)
			{
				Log.WarnFormat("Attempt to navigate from {0} to {1} when the current node is {2}",
				               navigateCommand.FromNode,
				               navigateCommand.ToNode,
				               CurrentNode);
				return;
			}

			UINode nextNode = navigateCommand.ToNode;
			Navigate(nextNode);

		}

		internal void Navigate(UINode nextNode)
		{
			if (IsComplete)
			{
				const string message = "Attempt to navigate when task has already completed";
				Log.Error(message);
				throw new UITaskException(message);
			}

			_nextNode = nextNode;

			if (nextNode == null)
			{
				_endTaskRequested = true;
			}

			if (_inNavigateMethod)
			{
				// Recursive calls to this method set the next node for navigation and exit.
				return;
			}


			NavigateHelper();
			while (_nextNode != null && !_endTaskRequested)
			{
				NavigateHelper();
			}

			// Cleanup any views and controllers for nodes that are now no longer 
			// the current node.
			foreach (var node in Nodes)
			{
				node.CleanupNode();
			}

			RaiseEvents();
		}

		private void NavigateHelper()
		{
			_inNavigateMethod = true;
			bool showModalView = false;
			try
			{
				var nextNode = _nextNode;
				_nextNode = null;

				if (nextNode != null)
				{
					using (var transition = nextNode.ViewDeck.BeginTransition(this))
					{
						try {
							do
							{
								DoNavigate(nextNode, transition);
								nextNode = _nextNode;
								_nextNode = null;
							} while (nextNode != null);

							if (!_endTaskRequested && CurrentNode != null)
							{
								// Finished navigating to a new node, so display the view
								if (CurrentNode.View != null)
								{
									if (CurrentNode.IsViewModal)
									{
										// We can't block and show a modal view here
										// because the _inNavigateMethod member is set to 
										// true and this will prevent any navigation from
										// within the modal view. For this reason set a variable
										// to remind us to show the modal view before leaving this
										// method.
										showModalView = true;
									}
									else
									{
										// It is possible for the view to navigate while it is
										// in the process of showing itself (for example in its Load method).
										// This is a bit pathalogical, but possible. If it should happen, the
										// view will be cleaned up in the Navigate method.
										transition.ShowView(CurrentNode.View);
									}
								}
								else if (CurrentNode.NestedTask != null)
								{
									if (CurrentNode.IsViewModal)
									{
										showModalView = true;
									}
								}
							}
						}
						catch (Exception ex)
						{
							Log.Error("Unexpected error in transition: " + ex.Message, ex);
							throw;
						}

					}
				}
				if (_endTaskRequested)
				{
					CurrentNode = null;
					IsComplete = true;
					_raiseTaskComplete = true;
				}
			}
			finally
			{
				_inNavigateMethod = false;
			}

			if (showModalView && CurrentNode != null)
			{
				using (var transition = CurrentNode.ViewDeck.BeginTransition(this))
				{
					transition.ShowView(CurrentNode.View);
				}
				CurrentNode.ModalWindow.ShowModal(false);
			}
		}

		private void DoNavigate(UINode nextNode, IViewTransition transition)
		{
			if (nextNode != null)
			{
				UINode prevNode = CurrentNode;
				CurrentNode = nextNode;

				// signal task started
				if (prevNode == null)
				{
					// the task has just started 
					_raiseTaskStarted = true;
				}

				// Tell the node that it is now the current node, and tell it what the previous node was.
				CurrentNode.EnterNode(prevNode);

				bool presenterCreated = false;

				// Create the presenter for the new current node. It is possible for
				// a presenter to request navigation inside its constructor, and if this happens
				// the _nextNode variable will be set, and we will continue through this loop again.
				if (CurrentNode.Presenter == null && CurrentNode.PresenterType != null)
				{
					CurrentNode.CreatePresenter();
					presenterCreated = true;
				}

				//
				if (_nextNode != null)
				{
					// if the controller navigated during its constructor, stop now without
					// creating the current view
					return;
				}

				bool viewCreated = false;

				// Create the view for the new current node. It is possible for a view to
				// request navigation inside its constructor, and if this happens the navigateValue
				// variable will be set, and we will continue through this loop again.
				if (CurrentNode.View == null && (CurrentNode.DeclaredViewType != null || CurrentNode.InferredViewType != null))
				{
					var p = CurrentNode.Presenter as PresenterBase;
					if (p != null && p.ViewObject != null)
					{
						// The presenter has created its own view
						CurrentNode.View = p.ViewObject;
					}
					else
					{
						CurrentNode.CreateView();
						if (p != null)
						{
							p.ViewObject = CurrentNode.View;
						}
					}
					viewCreated = true;
				}

				if (viewCreated)
				{
					transition.AddView(CurrentNode.View);
				}

				if (presenterCreated)
				{
					var p = CurrentNode.Presenter as PresenterBase;
					if (p != null)
					{
						p.PerformPresenterInitialization();
					}
				}

				if (CurrentNode.NestedTask == null && CurrentNode.NestedTaskType != null)
				{
					CurrentNode.CreateNestedTask();
				}
			}
		}

		private void RaiseEvents()
		{
			if (_raiseTaskComplete)
			{
				OnTaskComplete(EventArgs.Empty);
				_raiseTaskComplete = false;
			}

			if (_raiseTaskStarted)
			{
				OnTaskStarted(EventArgs.Empty);
				_raiseTaskStarted = false;
			}
		}

		#endregion
	}
}