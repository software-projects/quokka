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
using System.ComponentModel;
using System.Reflection;
using Castle.Core.Logging;
using Quokka.Diagnostics;
using Quokka.DynamicCodeGeneration;
using Quokka.Reflection;
using Quokka.ServiceLocation;
using Quokka.UI.Tasks;

// ReSharper disable CheckNamespace
namespace Quokka.Uip
// ReSharper restore CheckNamespace
{
	[Obsolete("This will be removed from Quokka in a future release")]
	public abstract class UipTask : IUITask
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private static readonly List<UipTask> RunningTasks = new List<UipTask>();
// ReSharper disable InconsistentNaming
		protected readonly IServiceContainer _serviceContainer;
// ReSharper restore InconsistentNaming
		private readonly UipNavigator _navigator;
		private IViewDeck _viewManager;
		private IList<UipNode> _nodes;
		private UipNode _currentNode;
		private object _currentController;
		private object _currentView;
		private string _navigateValue;
		private bool _inNavigateMethod;
		private bool _endTaskRequested;
		private bool _taskComplete;
		private bool _registeredWithViewManager;
		private bool _taskInitialized;
		private string _name;
		private bool _raiseTaskStarted;
		private bool _raiseTaskComplete;

		public event EventHandler TaskStarted;
		public event EventHandler TaskComplete;

		#region Construction

		protected UipTask()
		{
			// may be overridden by a derived class
			_name = GetType().Name;
			_navigator = new UipNavigator(this);

			_serviceContainer = CreateServiceContainer();
			_serviceContainer.RegisterInstance<IUipNavigator>(_navigator);

			// Registers the task instance under both UipTask, and the actual type of the task
			_serviceContainer.RegisterInstance(this);
			_serviceContainer.RegisterInstance(GetType(), this);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Provides services to controller and view objects created while this task is running.
		/// </summary>
		[Obsolete("Use ServiceContainer or ServiceLocator instead")]
		public IServiceProvider ServiceProvider
		{
			get { return _serviceContainer.Locator; }
		}

		public IServiceContainer ServiceContainer
		{
			get { return _serviceContainer; }
		}

		public IServiceLocator ServiceLocator
		{
			get { return _serviceContainer.Locator; }
		}

		/// <summary>
		/// Responsible for displaying view objects within the application.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The UIP framework is independent of the display technology used.
		/// In theory, any user interface technology could be used, including:
		/// </para>
		/// <list type="bullet">
		/// <item><term>Windows Forms</term><description>Windows Forms</description></item>
		/// <item><term>GTK#</term><description>GTK#</description></item>
		/// <item><term>Console text</term><description>Console text</description></item>
		/// </list>
		/// </remarks>
		public IViewDeck ViewManager
		{
			get { return _viewManager; }
		}

		/// <summary>
		/// Name of the task. Derived from the task definition.
		/// </summary>
		public string Name
		{
			get { return _name; }
			protected set { _name = value ?? String.Empty; }
		}

		/// <summary>
		/// The start node. Override to change from the default.
		/// </summary>
		/// <remarks>
		/// By default, the start node is the first node specified.
		/// </remarks>
		public virtual UipNode GetStartNode()
		{
			if (Nodes.Count == 0)
			{
				throw new UipException("No nodes defined for task: " + Name);
			}
			return Nodes[0];
		}

		/// <summary>
		/// The state object associated with this task.
		/// </summary>
		/// <returns>
		/// The task state as an <see cref="object"/>.
		/// </returns>
		public abstract object GetStateObject();

		public object CurrentController
		{
			get { return _currentController; }
		}

		public object CurrentView
		{
			get { return _currentView; }
		}

		public UipNode CurrentNode
		{
			get { return _currentNode; }
		}

		public IList<UipNode> Nodes
		{
			get
			{
				if (_nodes == null)
				{
					var nodes = new List<UipNode>();
					foreach (FieldInfo field in GetType().GetFields())
					{
						// only interested in UipNode fields
						if (field.FieldType != typeof (UipNode))
							continue;

						// only interested in non-static fields
						if (field.IsStatic)
							continue;

						// only interested in readonly fields
						if (!field.IsInitOnly)
							continue;

						// only interested in public fields
						if (!field.IsPublic)
							continue;

						// found a field that defines a node, get the value
						var node = (UipNode) field.GetValue(this);

						// the first time we create a task the name is not set
						if (node.Name == null)
						{
							node.Name = field.Name;
						}

						nodes.Add(node);
					}

					_nodes = nodes.AsReadOnly();
				}

				return _nodes;
			}
		}

		public bool IsRunning
		{
			get { return _currentNode != null; }
		}

		public bool IsComplete
		{
			get { return _taskComplete; }
		}

		#endregion

		#region Internal properties

		internal IUipNavigator Navigator
		{
			get { return _navigator; }
		}

		internal bool InNavigateMethod
		{
			get { return _inNavigateMethod; }
		}

		#endregion

		#region Public methods

		public UipNode FindNode(string name, bool throwOnError)
		{
			foreach (UipNode node in Nodes)
			{
				if (node.Name == name)
				{
					return node;
				}
			}

			if (throwOnError)
			{
				string message = "Cannot find node: " + name;
				Log.Error(message);
				throw new UipException(message);
			}

			return null;
		}

		/// <summary>
		/// Start the UI task.
		/// </summary>
		/// <param name="viewManager">View manager for the task.</param>
		/// <exception cref="UipException">
		/// Task has already started.
		/// </exception>
		public void Start(IViewDeck viewManager)
		{
			Verify.ArgumentNotNull(viewManager, "viewManager");
			if (IsRunning)
			{
				const string message = "Task is already running";
				Log.Error(message);
				throw new UipException(message);
			}
			if (IsComplete)
			{
				// This is a bit of a restriction, sorry. When a task has completed, there is
				// quite a bit of cleaning up to do to re-use it. At the moment it is easier to
				// just have the restriction that UI tasks cannot be reused.
				const string message = "A task can only be run once.";
				Log.Error(message);
				throw new UipException(message);
			}

			// If you have a UI task that is run more than once you
			// are going to have a problem, because each time you might be using a different IViewManager.
			// One way to deal with this is to have (yet another) child container that is created each
			// time the UI task is started.
			//
			// The way this is solved at the moment is to prevent re-use of UI tasks. See test before this
			// for avoiding re-use of tasks.
			_viewManager = viewManager;
			_serviceContainer.RegisterInstance(_viewManager);

			// Register the nested interfaces one time only. Wait until the first run
			// to do this, as the controller and view types are not available in the constructor.
			if (!_taskInitialized)
			{
				InitializeTask();
				_taskInitialized = true;
			}


			Navigate(null);
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

		#region Private methods

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
				serviceLocator = ServiceLocation.ServiceLocator.Current;
			}
			catch (NullReferenceException)
			{
				const string message = "ServiceLocator.Current has not been specified";
				Log.Error(message);
				throw new QuokkaException(message);
			}

			try
			{
				var parentContainer = serviceLocator.GetInstance<IServiceContainer>();
				return parentContainer.CreateChildContainer();
			}
			catch (ActivationException ex)
			{
				const string message = "Cannot create child service container for UI Task."
				                       + " The most likely cause is that Quokka.ServiceLocation.IServiceContainer"
				                       + " is not implemented.";
				Log.Error(message);
				throw new UipException(message, ex);
			}
		}

		/// <summary>
		/// Initialize task the first time it is started
		/// </summary>
		private void InitializeTask()
		{
			// Initialise each node
			foreach (var node in Nodes)
			{
				node.Initialize(this);
			}

			AddNestedNavigatorInterfaces(_navigator);
			AddNestedStateInterfaces();
		}

		/// <summary>
		/// Navigate to the next node based on the navigation value.
		/// </summary>
		/// <param name="navigateValue"></param>
		internal void Navigate(string navigateValue)
		{
			if (IsComplete)
			{
				string message = String.Format("Attempt to navigate '{0}', but task has already completed", navigateValue);
				Log.Error(message);
				throw new UipException(message);
			}

			_navigateValue = navigateValue;

			// Recursive calls will not perform any actual navigation, they
			// just set the _navigateValue member and exit.
			if (_inNavigateMethod)
				return;

			NavigateHelper();

			// This loop is here to catch the condition where the view navigates
			// while it is in the process of being shown.
			while (!_inNavigateMethod && _navigateValue != null && !_endTaskRequested)
			{
				NavigateHelper();
			}

			// Cleanup any views and controllers for nodes that are now no longer 
			// the current node.
			foreach (UipNode node in Nodes)
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
				UipNode nextNode = GetNextNode();
				if (nextNode != null)
				{
					using (var viewTransition = ViewManager.BeginTransition(this))
					{
						try
						{
							// We wait until now to register with the view manager so that all calls
							// to the view manager occur withing a BeginTransition/EndTransition pair
							if (!_registeredWithViewManager)
							{
								//_viewManager.BeginTask(this);
								_registeredWithViewManager = true;
							}

							do
							{
								// Perform the navigation. This call
								// can result in a recursive call back into this function,
								// hence the test using the <c>inNavigateMethod</c> variable.
								DoNavigate(nextNode);

								// If there was a recursive call to this function, then this
								// will return non-null.
								nextNode = GetNextNode();
							} while (nextNode != null);

							if (!_endTaskRequested && _currentNode != null)
							{
								// Finished navigating to a new node, display the new view.
								if (_currentView != null)
								{
									if (_currentNode.IsViewModal)
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
										// in the process of showing the view. It is a bit difficult
										// to handle this here, as there is cleanup to perform. This
										// code could all do with a good refactor, but for now the way
										// this is handled is by the loop in the Navigate method.
										viewTransition.ShowView(_currentView);
									}
								}
							}
						}
						catch (Exception ex)
						{
							Log.Error("Unexpected exception in transition: " + ex.Message, ex);
							throw;
						}
					}
				}

				if (_endTaskRequested)
				{
					using (_viewManager.BeginTransition(this))
					{
						if (_currentNode != null)
						{
							_currentNode.ExitNode();
						}
					}

					_currentNode = null;
					_currentController = null;
					_currentView = null;
					_navigateValue = null;
					_taskComplete = true;
					RunningTasks.Remove(this);
					_raiseTaskComplete = true;
				}
			}
			finally
			{
				_inNavigateMethod = false;
			}

			if (showModalView)
			{
				using (var transition = CurrentNode.GetViewDeck(createIfNecessary: true).BeginTransition(this))
				{
					transition.ShowView(CurrentNode.View);
				}
				CurrentNode.GetModalWindow(createIfNecessary: true).ShowModal(false);
			}
		}

		/// <summary>
		/// Determine the next node to navigate to.
		/// </summary>
		/// <returns>
		/// The next UI node, or <c>null</c> if no transition is required. (A bit yukky, but also sets 
		/// <see cref="_endTaskRequested"/> if the next node is <c>null</c> and the task needs to terminate).
		/// </returns>
		private UipNode GetNextNode()
		{
			UipNode nextNode = null;

			if (!_endTaskRequested)
			{
				if (_currentNode == null)
				{
					// Task is just starting
					nextNode = GetStartNode();
				}
				else if (_navigateValue != null)
				{
					bool transitionDefined = _currentNode.GetNextNode(_navigateValue, out nextNode);

					// forget about the navigate value -- it might get set again when creating the controller
					string prevNavigateValue = _navigateValue; // but remember for the error message
					_navigateValue = null;

					if (!transitionDefined)
					{
						string message = String.Format("No transition defined: task={0}, node={1}, navigateValue={2}",
						                               Name, _currentNode.Name, prevNavigateValue);
						Log.Error(message);
						throw new UipException(message);
					}

					if (nextNode == null)
					{
						_endTaskRequested = true;
					}
				}
			}

			return nextNode;
		}

		/// <summary>
		/// Perform the navigation to the next UI node
		/// </summary>
		/// <param name="nextNode">The next UI node to navigate to.</param>
		/// <remarks>
		/// This process is slightly complicated by the fact that both the controller and
		/// the view can initiate a navigation inside their constructor and/or their initialization
		/// sequence (for example if they implement <see cref="ISupportInitialize"/>).
		/// </remarks>
		private void DoNavigate(UipNode nextNode)
		{
			if (nextNode != null)
			{
				UipNode prevNode = _currentNode;
				_currentNode = nextNode;

				// signal task started
				if (prevNode == null)
				{
					// the task has just started 
					_raiseTaskStarted = true;
					RunningTasks.Add(this);
				}
				else
				{
					// tell the previous node that it is no longer current
					prevNode.ExitNode();
				}

				// Tell the node that it is now the current node. This needs to happen
				// after calling ExitNode on the previous node, just in case the previous node
				// and the current node are actually the same node.
				_currentNode.EnterNode();

				// Create the controller for the new current node. It is possible for
				// a controller to request navigation inside its constructor, and if this happens
				// the navigateValue variable will be set, and we will continue through this loop again.
				_currentController = _currentNode.GetOrCreateController();

				//
				if (_navigateValue != null)
				{
					// if the controller navigated during its constructor, stop now without
					// creating the current view
					return;
				}

				// Create the view for the new current node. It is possible for a view to
				// request navigation inside its constructor, and if this happens the navigateValue
				// variable will be set, and we will continue through this loop again.
				_currentView = _currentNode.GetOrCreateView();
			}
		}

		/// <summary>
		/// Create proxies to the state object for nested interfaces called 'INavigator'
		/// </summary>
		/// <remarks>
		/// <para>
		/// Iterate through each controller and view class looking for nested interfaces called 'INavigator'.
		/// If a class contains a nested interface called 'INavigator', assume that this is
		/// a 'navigator proxy' reference to the real navigator object, and create an entry in the service
		/// container that will return a proxy to the navigator object when a service with the nested
		/// interface is requested.
		/// </para>
		/// </remarks>
		private void AddNestedNavigatorInterfaces(IUipNavigator navigator)
		{
			var types = new HashSet<Type>();

			// Get the distinct controller types, remembering that 
			// a controller or view type may be present in more than one node.
			foreach (UipNode node in Nodes)
			{
				Type controllerType = node.ControllerType;
				if (controllerType != null)
				{
					types.Add(controllerType);
				}

				Type viewType = node.ViewType;
				if (viewType != null)
				{
					types.Add(viewType);
				}
			}

			// Look for nested interface types called "INavigator" and assume that they are 
			// duck typing references to the state object.
			foreach (Type type in types)
			{
				Type nestedType = TypeUtil.FindNestedInterface(type, "INavigator");
				if (nestedType != null && nestedType.IsInterface)
				{
					// create a proxy for the navigator object and add it to the service provider
					object navigatorProxy = ProxyFactory.CreateNavigatorProxy(nestedType, navigator);
					_serviceContainer.RegisterInstance(nestedType, navigatorProxy);
				}
			}
		}

		/// <summary>
		/// Create proxies to the state object for nested interfaces called 'IState'
		/// </summary>
		/// <remarks>
		/// <para>
		/// Iterate through each controller and view class looking for nested interfaces called 'IState'.
		/// If a class contains a nested interface called 'IState', assume that this is
		/// a 'duck proxy' reference to the real state object, and create an entry in the service
		/// container that will return a duck proxy to the state object when a service with the nested
		/// interface is requested.
		/// </para>
		/// </remarks>
		private void AddNestedStateInterfaces()
		{
			// TODO: This should be merged with AddNestedNavigatorInterfaces()

			// Set of controller and view types in the UI task
			var controllerAndViewTypes = new HashSet<Type>();

			foreach (UipNode node in Nodes)
			{
				Type controllerType = node.ControllerType;
				if (controllerType != null)
				{
					controllerAndViewTypes.Add(controllerType);
				}
				Type viewType = node.ViewType;
				if (viewType != null)
				{
					controllerAndViewTypes.Add(viewType);
				}
			}

			// Look for nested interface types called "IState" and assume that they are 
			// duck typing references to the state object.
			foreach (Type type in controllerAndViewTypes)
			{
				Type nestedType = type.GetNestedType("IState");
				if (nestedType != null && nestedType.IsInterface)
				{
					// create a duck proxy for the state object and add it to the service provider
					object stateProxy = ProxyFactory.CreateDuckProxy(nestedType, GetStateObject());
					_serviceContainer.RegisterInstance(nestedType, stateProxy);
				}
			}
		}

		/// <summary>
		/// Raise any events that need to be raised.
		/// </summary>
		/// <remarks>
		/// Events are remembered and raised only at the very end of a method
		/// to avoid re-entrancy problems.
		/// </remarks>
		private void RaiseEvents()
		{
			if (_raiseTaskComplete && TaskComplete != null)
			{
				TaskComplete(this, EventArgs.Empty);
			}
			_raiseTaskComplete = false;

			if (_raiseTaskStarted && TaskStarted != null)
			{
				TaskStarted(this, EventArgs.Empty);
			}
			_raiseTaskStarted = false;
		}

		#endregion
	}
}