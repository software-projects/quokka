using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using Quokka.Diagnostics;
using Quokka.DynamicCodeGeneration;
using Quokka.Reflection;

namespace Quokka.Uip
{
    public abstract class UipTask
	{
		private static IServiceProvider _parentServiceProvider;
		private static readonly List<UipTask> _runningTasks = new List<UipTask>();
		protected readonly ServiceContainer _serviceContainer;
		private ControllerViewStore _controllersAndViews;
		private readonly Navigator _navigator;
		private IUipViewManager _viewManager;
		private EventHelper _eventHelper;
		private IList<UipNode> _nodes;
		private UipNode _currentNode;
		private object _currentController;
		private object _currentView;
		private string _navigateValue;
		private bool _inNavigateMethod;
		private bool _endTaskRequested;
		private bool _taskComplete;
		private bool _registeredWithViewManager;
		private string _name;
    	private ILogger _logger = LogManager.GetLogger();

		public event EventHandler TaskStarted
		{
			add { _eventHelper.TaskStarted += value; }
			remove { _eventHelper.TaskStarted -= value; }
		}

		public event EventHandler TaskComplete
		{
			add { _eventHelper.TaskComplete += value; }
			remove { _eventHelper.TaskComplete -= value; }
		}

		private class EventHelper
		{
			public event EventHandler TaskStarted;
			public event EventHandler TaskComplete;

			public void RaiseTaskStarted(object sender)
			{
				if (TaskStarted != null) {
					TaskStarted(sender, EventArgs.Empty);
				}
			}

			public void RaiseTaskComplete(object sender)
			{
				if (TaskComplete != null) {
					TaskComplete(sender, EventArgs.Empty);
				}
			}
		}

		#region Construction

		protected UipTask()
		{
			// may be overridden by a derived class
			_name = GetType().Name;
			_navigator = new Navigator(this);
			_serviceContainer = new ServiceContainer(_parentServiceProvider);
			_serviceContainer.AddService(typeof(IUipNavigator), _navigator);
			_currentNode = null;
			_eventHelper = new EventHelper();
			AddNestedNavigatorInterfaces(_navigator);
		}

		#endregion

		#region Public properties

		public static IServiceProvider ParentServiceProvider
		{
			get { return _parentServiceProvider; }
			set { _parentServiceProvider = value; }
		}

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
			if (Nodes.Count == 0) {
				throw new UipException("No nodes defined for task: " + Name);
			}
			return Nodes[0];
		}

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
				if (_nodes == null) {
					List<UipNode> nodes = new List<UipNode>();
					foreach (FieldInfo field in GetType().GetFields()) {
						// only interested in UipNode fields
						if (field.FieldType != typeof(UipNode))
							continue;

						// only interested in static fields
						if (!field.IsStatic)
							continue;

						// only interested in readonly fields
						if (!field.IsInitOnly)
							continue;

						// only interested in public fields (TODO: is public necessary?)
						if (!field.IsPublic)
							continue;

						// found a field that defines a node, get the value
						UipNode node = (UipNode) field.GetValue(null);

						// the first time we create a task the name is not set
						if (node.Name == null) {
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

		#region Public methods

		public UipNode FindNode(string name, bool throwOnError)
		{
			foreach (UipNode node in Nodes) {
				if (node.Name == name) {
					return node;
				}
			}

			if (throwOnError) {
				string message = "Cannot find node: " + name;
				_logger.Error(message);
				throw new UipException(message);
			}

			return null;
		}

		public void Start(IUipViewManager viewManager)
		{
			Verify.ArgumentNotNull(viewManager, "viewManager");
			if (IsRunning) {
				const string message = "Task is already running";
				_logger.Error(message);
				throw new UipException(message);
			}

			_viewManager = viewManager;
			_serviceContainer.AddService(typeof(IUipViewManager), _viewManager);
			_controllersAndViews = new ControllerViewStore(this);
			Navigate(null);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion

		#region Private methods

        private void Navigate(string navigateValue)
        {
            if (_taskComplete)
            {
            	string message = String.Format("Attempt to navigate '{0}', but task has already completed", navigateValue);
            	_logger.Error(message);
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
                    ViewManager.BeginTransition();
                    try
                    {
                        // We wait until now to register with the view manager so that all calls
                        // to the view manager occur withing a BeginTransition/EndTransition pair
                        if (!_registeredWithViewManager)
                        {
                            _viewManager.BeginTask(this);
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

                        // We are now at the node where we are going to stay, so cleanup
                        // any unwanted views and controllers prior to displaying the view for
                        // this node.
                        _controllersAndViews.Cleanup(_currentNode);

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
                                    ViewManager.ShowView(_currentView);
                                }
                            }
                        }
                    }
                    finally
                    {
                        _controllersAndViews.Cleanup(_currentNode);
                        ViewManager.EndTransition();
                    }
                }

                if (_endTaskRequested)
                {
                    try
                    {
                        _viewManager.BeginTransition();
                        _controllersAndViews.Clear();
                        _viewManager.EndTask(this);
                    }
                    finally
                    {
                        _viewManager.EndTransition();
                    }

                    _currentNode = null;
                    _currentController = null;
                    _currentView = null;
                    _navigateValue = null;
                    _taskComplete = true;
                    _runningTasks.Remove(this);
                    _eventHelper.RaiseTaskComplete(this);

                    // After raising the TaskComplete event, remove reference to the event
                    // helper object. This helps avoid any circular memory references, which
                    // would result in a memory leak.
                    _eventHelper = null;
                }
            }
            finally
            {
                _inNavigateMethod = false;
            }

            if (showModalView)
            {
                ViewManager.ShowModalView(_currentView, _currentController);
            }
        }

	    private UipNode GetNextNode()
		{
			UipNode nextNode = null;

			if (!_endTaskRequested) {
				if (_currentNode == null) {
					// Task is just starting
					nextNode = GetStartNode();
				}
				else if (_navigateValue != null) {
					bool transitionDefined = _currentNode.GetNextNode(_navigateValue, out nextNode);

					// forget about the navigate value -- it might get set again when creating the controller
					string prevNavigateValue = _navigateValue; // but remember for the error message
					_navigateValue = null;

					if (!transitionDefined) {
						string message = String.Format("No transition defined: task={0}, node={1}, navigateValue={2}",
						                               Name, _currentNode.Name, prevNavigateValue);
						_logger.Error(message);
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
					_eventHelper.RaiseTaskStarted(this);
					_runningTasks.Add(this);
				}

				// Create the controller for the new current node. It is possible for
				// a controller to request navigation inside its constructor, and if this happens
				// the navigateValue variable will be set, and we will continue through this loop again.
				_currentController = _controllersAndViews.GetController(_currentNode);

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
                _currentView = _controllersAndViews.GetView(_currentNode);
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
			// Get the distinct controller types, as a controller type may be present in more than
			// one node.
			Dictionary<Type, Type> typeDict = new Dictionary<Type, Type>();
			foreach (UipNode node in Nodes) {
				Type controllerType = node.ControllerType;
				if (controllerType != null) {
					typeDict[controllerType] = controllerType;
				}

				Type viewType = node.ViewType;
				if (viewType != null)
				{
					typeDict[viewType] = viewType;
				}
			}

			Dictionary<Type, Type> nestedTypeDict = new Dictionary<Type, Type>();

			// Look for nested interface types called "INavigator" and assume that they are 
			// duck typing references to the state object.
			foreach (Type type in typeDict.Keys) {
				Type nestedType = TypeUtil.FindNestedInterface(type, "INavigator");
				if (nestedType != null && !nestedTypeDict.ContainsKey(nestedType)) {
					// create a proxy for the navigator object and add it to the service provider
					object navigatorProxy = ProxyFactory.CreateNavigatorProxy(nestedType, navigator);
					_serviceContainer.AddService(nestedType, navigatorProxy);
					nestedTypeDict.Add(nestedType, nestedType);
				}
			}
		}

		public void EndTask()
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

		#region Nested class ControllerViewStore

		internal class ControllerViewStore
		{
			private readonly UipTask _task;
			private readonly IDictionary<UipNode, object> _controllers;
			private readonly IDictionary<UipNode, object> _views;

			public ControllerViewStore(UipTask task)
			{
				_task = task;
				_task.ViewManager.ViewClosed += ViewManager_ViewClosed;
				_controllers = new Dictionary<UipNode, object>();
				_views = new Dictionary<UipNode, object>();
			}

			public void Clear()
			{
				IList<object> views = new List<object>(_views.Values);
				IList<object> controllers = new List<object>(_controllers.Values);

				_views.Clear();
				_controllers.Clear();
				foreach (object view in views) {
					_task.ViewManager.RemoveView(view);
				}
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
                        object controller;
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

								if (_controllers.TryGetValue(node, out controller)) {
									_controllers.Remove(node);
									controllersForDisposal.Add(controller);
								}
							}
						}

                        // If there is no view, then the controller will always be disposed of
                        else if (_controllers.TryGetValue(node, out controller))
                        {
                            _controllers.Remove(node);
                            controllersForDisposal.Add(controller);
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
					_task.GetStateObject(),
					_task.ServiceProvider,
					_task,
					node);

				if (controller == null) {
					throw new UipException("Failed to create controller");
				}

				ISupportInitialize initialize = controller as ISupportInitialize;
				if (initialize != null) {
					initialize.BeginInit();
				}

				UipUtil.SetState(controller, _task.GetStateObject(), false);
				UipUtil.SetNavigator(controller, _task._navigator);
				UipUtil.SetViewManager(controller, _task.ViewManager);
				PropertyUtil.SetValues(controller, node.ControllerProperties);

				if (initialize != null) {
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

				// controller for the view
				object controller;
				object controllerProxy = null;

				if (_controllers.TryGetValue(node, out controller))
				{
					// Look for a nested controller interface
					if (node.ControllerInterface != null)
					{
						controllerProxy = ProxyFactory.CreateDuckProxy(node.ControllerInterface, controller);
					}
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

				UipUtil.SetState(view, _task.GetStateObject(), false);
				UipUtil.SetController(view, controller, false);
				PropertyUtil.SetValues(view, node.ViewProperties);

                // Assign the view to the controller if the controller wants it.
				if (controller != null)
				{
					UipUtil.SetView(controller, view, false);
				}

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
					if (ReferenceEquals(view, keyValuePair.Value)) {
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
							bool isDisposed = (bool) isDisposedObject;
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