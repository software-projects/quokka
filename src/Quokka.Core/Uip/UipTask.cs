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

using System.ComponentModel;

namespace Quokka.Uip
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Quokka.DynamicCodeGeneration;
	using Quokka.Reflection;

	/// <summary>
	/// A task defines a discrete unit of work-flow within an application.
	/// </summary>
	/// <remarks>
	/// <para>
	/// An application contains one or more UIP tasks.
	/// </para>
	/// <para>TODO: more text</para>
	/// </remarks>
	public sealed class UipTask
	{
		private readonly QuokkaContainer _serviceContainer;
		private readonly UipTaskDefinition _taskDefinition;
		private readonly ControllerViewStore _controllersAndViews;
		private readonly Navigator _navigator;
		private readonly object state;
		private UipNode _currentNode;
		private object _currentController;
		private object _currentView;
		private string _navigateValue;
		private bool _inNavigateMethod;
		private bool _endTaskRequested;
		private bool _taskComplete;

		public event EventHandler TaskStarted;
		public event EventHandler TaskComplete;

		#region Construction

		internal UipTask(UipTaskDefinition taskDefinition, IServiceProvider serviceProvider, IUipViewManager viewManager)
		{
			if (taskDefinition == null) {
				throw new ArgumentNullException("navigationGraph");
			}
			if (viewManager == null) {
				throw new ArgumentNullException("viewManager");
			}
			_taskDefinition = taskDefinition;
			_navigator = new Navigator(this);
			_serviceContainer = new QuokkaContainer(serviceProvider);
			_serviceContainer.AddService(typeof(IUipViewManager), viewManager);
			_serviceContainer.AddService(typeof(IUipNavigator), _navigator);
			_serviceContainer.AddService(typeof(IUipEndTask), new EndTaskImpl(this));
			state = ObjectFactory.Create(taskDefinition.StateType, serviceProvider, serviceProvider, this);
			PropertyUtil.SetValues(state, taskDefinition.StateProperties);
			_currentNode = null;
			_controllersAndViews = new ControllerViewStore(this);
			AddNestedStateInterfaces();
			AddNestedNavigatorInterfaces(_navigator);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Provides services to controller and view objects created while this task is running.
		/// </summary>
		public IServiceProvider ServiceProvider
		{
			get { return _serviceContainer; }
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
		public IUipViewManager ViewManager
		{
			get { return (IUipViewManager)ServiceProvider.GetService(typeof(IUipViewManager)); }
		}

		/// <summary>
		/// Name of the task. Derived from the task definition.
		/// </summary>
		public string Name
		{
			get { return _taskDefinition.Name; }
		}

		/// <summary>
		/// The task state object.
		/// </summary>
		/// <remarks>
		/// Every UIP task has a state object, which is made available to controllers
		/// via their constructor arguments. The state object type is specified in the
		/// UIP task definition.
		/// </remarks>
		public object State
		{
			get { return state; }
		}

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
			get { return _taskDefinition.Nodes; }
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

		#region Public methods

		public UipNode FindNode(string name, bool throwOnError)
		{
			return _taskDefinition.FindNode(name, throwOnError);
		}

		public void Start()
		{
			if (IsRunning) {
				throw new UipException("Task is already running");
			}

			ViewManager.BeginTask(this);
			Navigate(null);

			if (TaskStarted != null) {
				TaskStarted(this, EventArgs.Empty);
			}
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion

		#region Private methods

		private void Navigate(string navigateValue)
		{
			if (_taskComplete) {
				throw new UipException("Task has completed");
			}

			_navigateValue = navigateValue;

			// Recursive calls will not perform any actual navigation, they
			// just set the _navigateValue member and exit.
			if (!_inNavigateMethod) {
				_inNavigateMethod = true;
				bool showModalView = false;
				try {
					UipNode nextNode = GetNextNode();
					if (nextNode != null) {
						ViewManager.BeginTransition();
						try {
							do {
								// Perform the navigation. Note that this call
								// can result in a recursive call back into this function,
								// hence the test using the <c>inNavigateMethod</c> variable.
								DoNavigate(nextNode);

								// If there was a recursive call to this function, then this
								// will return non-null.
								nextNode = GetNextNode();
							} while (nextNode != null);

							// We are now at the node where we are going to stay, so cleanup
							// any unwanted views and controllers prior to creating the view for
							// this node.
							_controllersAndViews.Cleanup(_currentNode);

							if (!_endTaskRequested && _currentNode != null) {
								// Finished navigating to a new node, display the new view.
								_currentView = _controllersAndViews.GetView(_currentNode);
								if (_currentView != null) {
									if (_currentNode.IsViewModal) {
										// We can't block and show a modal view here
										// because the _inNavigateMethod member is set to 
										// true and this will prevent any navigation from
										// within the modal view. For this reason set a variable
										// to remind us to show the modal view before leaving this
										// method.
										showModalView = true;
									}
									else {
										ViewManager.ShowView(_currentView);
									}
								}
							}
						}
						finally {
							_controllersAndViews.Cleanup(_currentNode);
							ViewManager.EndTransition();
						}
					}

					if (_endTaskRequested) {
						_currentNode = null;
						_currentController = null;
						_currentView = null;
						_navigateValue = null;
						_taskComplete = true;
						_controllersAndViews.Clear();
						ViewManager.EndTask(this);
						if (TaskComplete != null) {
							TaskComplete(this, EventArgs.Empty);
						}
					}
				}
				finally {
					_inNavigateMethod = false;
				}

				if (showModalView) {
					ViewManager.ShowModalView(_currentView, _currentController);
				}
			}
		}

		private UipNode GetNextNode()
		{
			UipNode nextNode = null;

			if (!_endTaskRequested) {
				if (_currentNode == null) {
					// Task is just starting
					nextNode = _taskDefinition.StartNode;
				}
				else if (_navigateValue != null) {
					bool transitionDefined = _currentNode.GetNextNode(_navigateValue, out nextNode);

					// forget about the navigate value -- it might get set again when creating the controller
					string prevNavigateValue = _navigateValue; // but remember for the error message
					_navigateValue = null;

					if (!transitionDefined) {
						string message = String.Format("No transition defined: task={0}, node={1}, navigateValue={2}",
						                               _taskDefinition.Name, _currentNode.Name, prevNavigateValue);
						throw new UipException(message);
					}

					if (nextNode == null) {
						_endTaskRequested = true;
					}
				}
			}

			return nextNode;
		}

		private void DoNavigate(UipNode nextNode)
		{
			if (nextNode != null) {
				UipNode prevNode = _currentNode;
				_currentNode = nextNode;

				// signal task started
				if (prevNode == null) {
					// the task has just started 
					if (TaskStarted != null) {
						TaskStarted(this, EventArgs.Empty);
					}
				}

				// Create the controller for the new current node. Note that it is possible for
				// a controller to request navigation inside its constructor, and if this happens
				// the navigateValue variable will be set, and we will continue through this loop again.
				_currentController = _controllersAndViews.GetController(_currentNode);
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
			// Get the distinct controller types, as a controller type may be present in more than
			// one node.
			Dictionary<Type, Type> typeDict = new Dictionary<Type, Type>();
			foreach (UipNode node in Nodes) {
				Type controllerType = node.ControllerType;
				if (controllerType != null) {
					typeDict[controllerType] = controllerType;
				}
				Type viewType = node.ViewType;
				if (viewType != null) {
					typeDict[viewType] = viewType;
				}
			}

			// Look for nested interface types called "IState" and assume that they are 
			// duck typing references to the state object.
			foreach (Type controllerType in typeDict.Keys) {
				Type nestedType = controllerType.GetNestedType("IState");
				if (nestedType != null && nestedType.IsInterface) {
					// create a duck proxy for the state object and add it to the service provider
					object stateProxy = ProxyFactory.CreateDuckProxy(nestedType, state);
					_serviceContainer.AddService(nestedType, stateProxy);
				}
			}
		}

		/// <summary>
		/// Create proxies to the state object for nested interfaces called 'INavigator'
		/// </summary>
		/// <remarks>
		/// <para>
		/// Iterate through each controller class looking for nested interfaces called 'INavigator'.
		/// If a class contains a nested interface called 'INavigator', assume that this is
		/// a 'navigator proxy' reference to the real navigator object, and create an entry in the service
		/// container that will return a proxy to the navigator object when a service with the nested
		/// interface is requested.
		/// </para>
		/// </remarks>
		private void AddNestedNavigatorInterfaces(IUipNavigator navigator)
		{
			// Get the distinct controller types, as a controller type may be present in more than
			// one node.
			Dictionary<Type, Type> typeDict = new Dictionary<Type, Type>();
			foreach (UipNode node in Nodes) {
				Type controllerType = node.ControllerType;
				if (controllerType != null) {
					typeDict[controllerType] = controllerType;
				}
			}

			// Look for nested interface types called "INavigator" and assume that they are 
			// duck typing references to the state object.
			foreach (Type controllerType in typeDict.Keys) {
				Type nestedType = controllerType.GetNestedType("INavigator");
				if (nestedType != null && nestedType.IsInterface) {
					// create a proxy for the navigator object and add it to the service provider
					object navigatorProxy = ProxyFactory.CreateNavigatorProxy(nestedType, navigator);
					_serviceContainer.AddService(nestedType, navigatorProxy);
				}
			}
		}

		private void EndTask()
		{
			_endTaskRequested = true;
			Navigate(null);
		}

		#endregion

		#region Nested class Navigator

		private class Navigator : IUipNavigator
		{
			private readonly UipTask task;

			public Navigator(UipTask task)
			{
				this.task = task;
			}

			public void Navigate(string navigateValue)
			{
				if (navigateValue == null) {
					throw new ArgumentNullException("navigateValue");
				}
				task.Navigate(navigateValue);
			}

			public bool CanNavigate(string navigateValue)
			{
				if (navigateValue == null) {
					throw new ArgumentNullException("navigateValue");
				}
				UipNode nextNode;
				return task.CurrentNode.GetNextNode(navigateValue, out nextNode);
			}
		}

		#endregion

		#region Nested class EndTask

		private class EndTaskImpl : IUipEndTask
		{
			private readonly UipTask task;

			public EndTaskImpl(UipTask task)
			{
				this.task = task;
			}

			#region IUipEndTask Members

			public void EndTask()
			{
				task.EndTask();
			}

			#endregion
		}

		#endregion

		#region Nested class ControllerViewStore

		internal class ControllerViewStore
		{
			private readonly UipTask _task;
			private readonly IDictionary<UipNode, object> _controllers;
			private readonly IDictionary<UipNode, object> _views;

			public ControllerViewStore(UipTask task)
			{
				_task = task;
				_task.ViewManager.ViewClosed += new EventHandler<UipViewEventArgs>(ViewManager_ViewClosed);
				_controllers = new Dictionary<UipNode, object>(_task.Nodes.Count);
				_views = new Dictionary<UipNode, object>(_task.Nodes.Count);
			}

			public void Clear()
			{
				IList<object> views = new List<object>(_views.Values);
				IList<object> controllers = new List<object>(_controllers.Values);
				_views.Clear();
				_controllers.Clear();
				DisposeOfViews(views);
				DisposeOfControllers(controllers);
			}

			/// <summary>
			/// Cleanup all views and controllers that are no longer needed.
			/// </summary>
			/// <param name="currentNode">The current node.</param>
			public void Cleanup(UipNode currentNode)
			{
				if (currentNode != null && currentNode.IsViewModal) {
					// Never cleanup views or controllers when showing a modal view.
					// This is because view(s) for previous node(s) may be needed as a
					// backdrop to the modal view.
					return;
				}

				// Identify the views and controllers to be disposed and remove them
				// from the dictionary collections first. This is to avoid any side-effects
				// of callbacks during the disposal process.
				List<object> viewsForDisposal = new List<object>();
				List<object> controllersForDisposal = new List<object>();

				foreach (UipNode node in _task.Nodes) {
					if (node != currentNode) {
						object view;
						if (_views.TryGetValue(node, out view)) {
							if (node.StayOpen) {
								// This is not the current node, but it has been marked
								// as a 'stay open' node. Request the view manager to hide it
								// but otherwise leave it alone.
								_task.ViewManager.HideView(view);
							}
							else {
								// This is not the current node and has not been marked
								// as a 'stay open' node, so the view and the controller
								// can be disposed of.
								_views.Remove(node);
								viewsForDisposal.Add(view);

								object controller;
								if (_controllers.TryGetValue(node, out controller)) {
									_controllers.Remove(node);
									controllersForDisposal.Add(controller);
								}
							}
						}
					}
				}

				DisposeOfViews(viewsForDisposal);
				DisposeOfControllers(controllersForDisposal);
			}

			private void DisposeOfViews(IEnumerable views)
			{
				// Dispose of the views
				foreach (object view in views) {
					_task.ViewManager.RemoveView(view);
					DisposeOf(view);
				}
			}

			private static void DisposeOfControllers(IEnumerable controllers)
			{
				foreach (object controller in controllers) {
					DisposeOf(controller);
				}
			}

			public object GetController(UipNode node)
			{
				object controller;
				if (!_controllers.TryGetValue(node, out controller)) {
					controller = CreateController(node);
					_controllers.Add(node, controller);
				}
				return controller;
			}

			private object CreateController(UipNode node)
			{
				Type controllerType = node.ControllerType;
				if (controllerType == null) {
					return null;
				}

				object controller = ObjectFactory.Create(
						node.ControllerType,
						_task.ServiceProvider,
						_task.State,
						_task.ServiceProvider,
						_task,
						node);

				if (controller == null) {
					throw new UipException("Failed to create controller");
				}

				ISupportInitialize initialize = controller as ISupportInitialize;
				if (initialize != null)
				{
					initialize.BeginInit();
				}

				UipUtil.SetState(controller, _task.State, false);
				UipUtil.SetNavigator(controller, _task._navigator);
				UipUtil.SetViewManager(controller, _task.ViewManager);
				PropertyUtil.SetValues(controller, node.ControllerProperties);

				if (initialize != null)
				{
					initialize.EndInit();
				}
				return controller;
			}

			public object GetView(UipNode node)
			{
				object view;
				if (!_views.TryGetValue(node, out view)) {
					view = CreateView(node);
					_views.Add(node, view);
				}
				return view;
			}

			private object CreateView(UipNode node)
			{
				Type viewType = node.ViewType;
				if (viewType == null) {
					// no view defined, finished
					return null;
				}

				// this will throw an exception if there is not controller defined
				object controller = _controllers[node];

				// Look for a nested controller interface
				object controllerProxy = null;
				if (node.ControllerInterface != null) {
					controllerProxy = ProxyFactory.CreateDuckProxy(node.ControllerInterface, controller);
				}

				object view = ObjectFactory.Create(
						viewType,
						_task.ServiceProvider,
						controllerProxy,
						_task.ServiceProvider,
						controller,
						this);

				if (view == null) {
					throw new UipException("Failed to create view");
				}

				UipUtil.SetState(view, _task.State, false);
				UipUtil.SetController(view, controller, false);
				PropertyUtil.SetValues(view, node.ViewProperties);

				// If this view is not shown modally, tell the view manager about it.
				if (!node.IsViewModal) {
					_task.ViewManager.AddView(view, controller);
				}

				return view;
			}

			private void ViewManager_ViewClosed(object sender, UipViewEventArgs e)
			{
				UipNode node = FindNodeForView(e.View);

				if (node == null) {
					// we have already forgotten about this view
					return;
				}

				if (node.IsViewModal && node == _task.CurrentNode && !_task._inNavigateMethod) {
					// The current modal view has closed without specifying a navigation.
					// This can happen if the user clicks the close window button.
					// All modal views should specify a navigation for 'Close'.
					_task.Navigate("Close");
				}

				// Don't try to dispose of the view, or tell the view manager to
				// remove the view as as we assume that the view manager has already
				// taken care of this. Just remove the view from our collection of views
				// as it is no longer useful.
				_views.Remove(node);

				// The view manager will not have disposed of the associated controller, so we
				// look after cleaning up the controller.
				object controller;
				if (_controllers.TryGetValue(node, out controller)) {
					_controllers.Remove(node);
					DisposeOf(controller);
				}
			}

			private UipNode FindNodeForView(object view)
			{
				foreach (KeyValuePair<UipNode, object> keyValuePair in _views) {
					if (Object.ReferenceEquals(view, keyValuePair.Value)) {
						return keyValuePair.Key;
					}
				}
				return null;
			}

			private static void DisposeOf(object obj)
			{
				if (obj != null) {
					IDisposable disposable = obj as IDisposable;
					if (disposable != null) {
						// Look for a boolean property called 'IsDisposed', and do not dispose if it has a value of true
						// This is a bit of a hack to avoid any attempt to dispose of Windows Forms controls twice.
						object isDisposedObject = PropertyUtil.GetValue(disposable, "IsDisposed", false);
						if (isDisposedObject is bool) {
							bool isDisposed = (bool)isDisposedObject;
							if (!isDisposed) {
								disposable.Dispose();
							}
						}
					}
				}
			}
		}

		#endregion
	}
}