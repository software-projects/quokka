using System;
using System.Collections.Generic;
using Common.Logging;
using Microsoft.Practices.ServiceLocation;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;
using Quokka.Uip;

namespace Quokka.UI.Tasks
{
	public abstract class UITask
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private IList<UINode> _nodes;
		private TaskBuilder _taskBuilder;
		private bool _canCreateNode;
		private UINode _nextNode;
		private IViewDeck _viewDeck;
		protected readonly IServiceContainer _serviceContainer;
		private bool _inNavigateMethod;
		private bool _endTaskRequested;
		private bool _raiseTaskComplete;
		private bool _raiseTaskStarted;
		private bool _registeredWithViewDeck;

		protected UITask()
		{
			// may be overridden by a derived class
			Name = GetType().Name;

			_serviceContainer = CreateServiceContainer();

			// Registers the task instance under both UipTask, and the actual type of the task
			_serviceContainer.RegisterInstance(this);
			_serviceContainer.RegisterInstance(GetType(), this);
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

			// Now that the task has a view deck, we can inform the nodes and let them initialize.
			foreach (var node in Nodes)
			{
				node.TaskStarting();
			}

			Navigate(Nodes[0]);
		}

		#endregion

		#region Internal methods

		internal void PopNode()
		{
			// TODO: navigate to the previous node. Implies that we keep a stack of nodes. 
			// Normally would not want the stack to go too far. We only want a stack of nodes
			// to backtrack modals.
			throw new NotImplementedException();
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

		#endregion

		#region Protected methods

		protected INodeBuilder CreateNode()
		{
			return CreateNode(null);
		}

		protected INodeBuilder CreateNode(string nodeName)
		{
			if (_canCreateNode)
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
			_taskBuilder = TaskTemplateStorage.FindForType(GetType()) ?? CreateTaskBuilder();

			var nodes = new List<UINode>();

			foreach (var nodeBuilder in _taskBuilder.Nodes)
			{
				UINode node = nodeBuilder.CreateNode();
				node.Task = this;
				nodes.Add(node);
			}
			_nodes = nodes;
		}

		private TaskBuilder CreateTaskBuilder()
		{
			var taskBuilder = new TaskBuilder(GetType());
			_canCreateNode = true;
			try
			{
				CreateNodes();
			}
			finally
			{
				_canCreateNode = false;
			}
			taskBuilder.Validate();
			TaskTemplateStorage.Add(taskBuilder);
			return taskBuilder;
		}

		/// <summary>
		/// Create a service container that is a child container of the global container.
		/// </summary>
		/// <returns>
		/// An <see cref="IServiceContainer"/>.
		/// </returns>
		private static IServiceContainer CreateServiceContainer()
		{
			IServiceLocator serviceLocator;

			try
			{
				serviceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current;
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

		internal void Navigate(UINode nextNode)
		{
			if (IsComplete)
			{
				const string message = "Attempt to navigate when task has already completed";
				Log.Error(message);
				throw new UITaskException(message);
			}

			_nextNode = nextNode;

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
			bool showModalView = false;
			_inNavigateMethod = true;
			try
			{
				var nextNode = _nextNode;

				if (nextNode != null)
				{
					ViewDeck.BeginTransition();
					try
					{
						if (!_registeredWithViewDeck)
						{
							ViewDeck.BeginTask(this);
							_registeredWithViewDeck = true;
						}

						do
						{
							DoNavigate(nextNode);
							nextNode = _nextNode;
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
									ViewDeck.ShowView(CurrentNode.View);
								}
							}
						}
					}
					catch (Exception ex)
					{
						Log.Error("Unexpected error in transition: " + ex.Message, ex);
					}
					finally
					{
						ViewDeck.EndTransition();
					}
				}
				if (_endTaskRequested)
				{
					try
					{
						ViewDeck.BeginTransition();
						ViewDeck.EndTask(this);
					}
					finally
					{
						ViewDeck.EndTransition();
					}

					CurrentNode = null;
					IsComplete = true;
					_raiseTaskComplete = true;
				}
			}
			finally
			{
				_inNavigateMethod = false;
			}
		}

		private void DoNavigate(UINode nextNode)
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

				// Tell the node that it is now the current node.
				CurrentNode.EnterNode();

				// Create the presenter for the new current node. It is possible for
				// a presenter to request navigation inside its constructor, and if this happens
				// the _nextNode variable will be set, and we will continue through this loop again.
				CurrentNode.CreatePresenter();

				//
				if (_nextNode != null)
				{
					// if the controller navigated during its constructor, stop now without
					// creating the current view
					return;
				}

				// Create the view for the new current node. It is possible for a view to
				// request navigation inside its constructor, and if this happens the navigateValue
				// variable will be set, and we will continue through this loop again.
				CurrentNode.CreateView();
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