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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.DynamicCodeGeneration;
using Quokka.Reflection;
using Quokka.ServiceLocation;

namespace Quokka.Uip
{
	/// <summary>
	/// Represents a single UI node in the UI task.
	/// </summary>
	public class UipNode
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private string _name;
		private Type _viewType;
		private Type _controllerType;
		private readonly PropertyCollection _viewProperties;
		private readonly PropertyCollection _controllerProperties;
		private readonly List<UipTransition> _transitions;
		private bool _controllerInterfaceValid;
		private Type _controllerInterface;
		private UipNodeOptions _options;

		// fields set once the task has started
		private UipTask _task;
		private IServiceContainer _container;
		private object _controller;
		private object _view;
		private readonly HashSet<Type> _controllerInterfaces = new HashSet<Type>();
		private bool _controllerInterfacesLoaded;


		public UipNode()
		{
			_viewProperties = new PropertyCollection();
			_controllerProperties = new PropertyCollection();
			_transitions = new List<UipTransition>();
		}

		public string Name
		{
			get { return _name; }
			// The name is set when the task starts
			internal set { _name = value; }
		}

		public object Controller
		{
			get { return _controller; }
		}

		public object View
		{
			get { return _view; }
		}

		public Type ViewType
		{
			get { return _viewType; }
		}

		public Type ControllerType
		{
			get { return _controllerType; }
		}

		public Type ControllerInterface
		{
			get
			{
				if (!_controllerInterfaceValid)
				{
					_controllerInterface = GetControllerInterfaceFromViewType(_viewType);
					_controllerInterfaceValid = true;
				}
				return _controllerInterface;
			}
		}

		public PropertyCollection ViewProperties
		{
			get { return _viewProperties; }
		}

		public PropertyCollection ControllerProperties
		{
			get { return _controllerProperties; }
		}

		public UipNodeOptions Options
		{
			get { return _options; }
		}

		public bool IsViewModal
		{
			get { return (_options & UipNodeOptions.ModalView) != 0; }
		}

		public bool StayOpen
		{
			get
			{
				// A modal view can never stay open, regardless of the setting
				if (IsViewModal)
					return false;
				return (_options & UipNodeOptions.StayOpen) != 0;
			}
		}

		public IList<UipTransition> Transitions
		{
			get { return _transitions; }
		}

		public UipNode SetView<TView>()
		{
			return SetViewType(typeof (TView));
		}

		public UipNode SetViewType(Type viewType)
		{
			_viewType = viewType;
			_controllerInterfaceValid = false;
			return this;
		}

		public UipNode SetController<TController>()
		{
			return SetControllerType(typeof (TController));
		}

		public UipNode SetPresenter<TPresenter>()
		{
			return SetControllerType(typeof (TPresenter));
		}

		public UipNode SetControllerType(Type controllerType)
		{
			_controllerType = controllerType;
			return this;
		}

		public UipNode SetOptions(UipNodeOptions options)
		{
			_options = options;
			return this;
		}

		public UipNode SetViewProperty(string propertyName, string propertyValue)
		{
			ViewProperties.Add(propertyName, propertyValue);
			return this;
		}


		public UipNode SetControllerProperty(string propertyName, string propertyValue)
		{
			ControllerProperties.Add(propertyName, propertyValue);
			return this;
		}

		public UipNode NavigateTo(string navigateValue, UipNode node)
		{
			_transitions.Add(new UipTransition(this, navigateValue, node));
			return this;
		}

		public bool GetNextNode(string navigateValue, out UipNode node)
		{
			foreach (UipTransition transition in _transitions)
			{
				if (StringComparer.InvariantCultureIgnoreCase.Compare(transition.NavigateValue, navigateValue) == 0)
				{
					node = transition.NextNode;
					return true;
				}
			}
			node = null;
			return false;
		}

		internal void Initialize(UipTask task)
		{
			Verify.ArgumentNotNull(task, "task", out _task);
			task.ViewManager.ViewClosed += ViewManager_ViewClosed;
		}

		private void ViewManager_ViewClosed(object sender, UipViewEventArgs e)
		{
			if (e.View != _view)
			{
				// not this view
				return;
			}

			if (IsViewModal && this == _task.CurrentNode && !_task.InNavigateMethod)
			{
				// The current modal view has closed without specifying a navigation.
				// This can happen if the user clicks the close window button.
				// All modal views should specify a navigation for 'Close'.
				const string close = "Close";
				if (_task.Navigator.CanNavigate(close))
				{
					_task.Navigate(close);
				}
			}
		}

		/// <summary>
		/// Called by the <see cref="UipTask"/> when this node becomes the current node.
		/// </summary>
		internal void EnterNode()
		{
			Verify.IsNotNull(_task);

			// If the node is being entered for the first time, its service container will be null
			// and has to be created.
			if (_container == null)
			{
				IServiceContainer parentContainer = _task.ServiceLocator.GetInstance<IServiceContainer>();
				Verify.IsNotNull(parentContainer);

				// Do not assign the container to _container yet -- wait until all the registration is 
				// complete. That way if one of the registrations throws an exception ,we are not left with
				// a node whose container is not fully initialised.
				IServiceContainer nodeContainer = parentContainer.CreateChildContainer();

				// Both controller and view are singletons. The container only lasts as long as the
				// node is current (or as long as the task runs in the case of a StayOpen node).
				if (_controllerType != null)
				{
					nodeContainer.RegisterType(_controllerType, null, null, ServiceLifecycle.Singleton);
				}
				if (_viewType != null)
				{
					nodeContainer.RegisterType(_viewType, null, null, ServiceLifecycle.Singleton);
				}

				// node container is now initialised, assign to the member variable
				_container = nodeContainer;
			}
		}

		/// <summary>
		/// Called by the <see cref="UipTask"/> when this node is no longer the current node.
		/// </summary>
		internal void ExitNode()
		{
			// TODO: This does not do anything at the moment. It turns out to be more reliable to
			// dispose of the views and controllers in CleanupNode. Keep it for now in case there
			// is something to do as soon as the node stops being current.
		}

		/// <summary>
		/// Cleanup the view and controller if necessary. Called for every node at
		/// the end of the navigation process.
		/// </summary>
		internal void CleanupNode()
		{
			bool hideView = false;
			bool removeView = false;
			bool disposeContainer = false;

			if (_task == null)
			{
				disposeContainer = true;
			}
			else 
			{
				if (_task.IsComplete)
				{
					// The task is complete -- all nodes should dispose of their container
					disposeContainer = true;
					removeView = true;

					// The task is not re-usable, but the view manager might be. Unregister
					// event to avoid a memory leak.
					_task.ViewManager.ViewClosed -= ViewManager_ViewClosed;
				}
				else
				{
					if (_task.CurrentNode != this)
					{
						// At this point we know that this node is not the current view
						if (_task.CurrentNode != null && _task.CurrentNode.IsViewModal)
						{
							// if the current node is modal, then do not dispose of anything
						}
						else
						{
							if (StayOpen)
							{
								// Not the current node, but we have been ordered to keep the
								// view and controller. Just ask the view manager to hide the
								// view for now.
								hideView = true;
							}
							else
							{
								// Not the current node, and not ordered to stay open
								disposeContainer = true;
								removeView = true;
							}
						}
					}
				}
			}

			if (hideView)
			{
				_task.ViewManager.HideView(_view);
			}

			if (removeView)
			{
				_task.ViewManager.RemoveView(_view);
				DisposeView();
				DisposeController();
			}

			if (disposeContainer)
			{
				DisposeView();
				DisposeController();
				DisposeContainer();
			}
		}

		private void DisposeContainer()
		{
			DisposeOf(_container);
			_container = null;
		}

		private void DisposeView() {
			DisposeOf(_view);
			_view = null;
		}

		private void DisposeController() {
			DisposeOf(_controller);
			_controller = null;
		}

		private static void DisposeOf(object obj)
		{
			IDisposable disposable = obj as IDisposable;
			if (disposable != null)
			{
				try
				{
					disposable.Dispose();
				}
				catch (ObjectDisposedException)
				{
					// should not be thrown, but lots of objects do throw this
				}
			}
		}

		internal object GetOrCreateController()
		{
			if (_controller == null && _controllerType != null)
			{
				if (_container == null)
				{
					const string message = "Cannot get controller when not the current node";
					Log.Error(message);
					throw new InvalidOperationException(message);
				}

				object controller = _container.Locator.GetService(_controllerType);

				ISupportInitialize initialize = controller as ISupportInitialize;
				if (initialize != null)
				{
					initialize.BeginInit();
				}

				UipUtil.SetState(controller, _task.GetStateObject(), false);
				UipUtil.SetNavigator(controller, _task.Navigator);
				UipUtil.SetViewManager(controller, _task.ViewManager);
				PropertyUtil.SetValues(controller, ControllerProperties);

				if (initialize != null)
				{
					initialize.EndInit();
				}

				_controller = controller;

				foreach (Type interfaceType in LoadControllerInterfaces())
				{
					// TODO: Should create a proxy if the controller also implements IDisposable.
					// The proxy should do nothing when Dispose() is called. The reason for this
					// is that the container has the same instance registered multiple times. When
					// the time comes to dispose the container, we cannot be sure that the container
					// is smart enough to recognise that the same instance exists in the container more
					// than once. It might call Dispose() multiple times for the same object. 
					_container.RegisterInstance(interfaceType, controller);
				}
			}
			return _controller;
		}

		internal object GetOrCreateView()
		{
			if (_view == null && _viewType != null)
			{
				object controller = null;

				if (_controllerType != null)
				{
					// If there is a controller then we want to make sure that it is created prior
					// to creating the view.
					controller = GetOrCreateController();

					// If the view has a nested interface for the controller, register an instance of the controller
					// for this interface with the container prior to attempting to create the view.
					if (ControllerInterface != null)
					{
						object controllerProxy = ProxyFactory.CreateDuckProxy(ControllerInterface, controller);
						_container.RegisterInstance(ControllerInterface, controllerProxy);
					}
				}

				object view = _container.Locator.GetService(_viewType);

				UipUtil.SetState(view, _task.GetStateObject(), false);
				PropertyUtil.SetValues(view, ViewProperties);
				if (controller != null)
				{
					UipUtil.SetController(view, controller, false);
					UipUtil.SetView(controller, view, false);
				}

				if (!IsViewModal)
				{
					_task.ViewManager.AddView(view, controller);
				}

				_view = view;
			}
			return _view;
		}

		/// <summary>
		/// Find all of the interfaces that a controller implements that is required by the view
		/// for constructor injection.
		/// </summary>
		/// <returns>List of interface types</returns>
		private IEnumerable<Type> LoadControllerInterfaces()
		{
			if (!_controllerInterfacesLoaded)
			{
				if (_viewType != null)
				{
					// Loop through each constructor for the view (because we do not know
					// which constructor the service container will use). Look for any
					// constructor parameter that requires an interface that the controller
					// implements.
					foreach (ConstructorInfo constructor in _viewType.GetConstructors())
					{
						foreach (ParameterInfo parameter in constructor.GetParameters())
						{
							Type parameterType = parameter.ParameterType;
							if (!parameterType.IsInterface)
							{
								// only interested in parameter types that are interfaces
								continue;
							}

							if (parameterType.IsAssignableFrom(_controllerType))
							{
								_controllerInterfaces.Add(parameterType);
							}
						}
					}
				}

				_controllerInterfacesLoaded = true;
			}
			return _controllerInterfaces;
		}

		private static Type GetControllerInterfaceFromViewType(Type viewType)
		{
			return TypeUtil.FindNestedInterface(viewType, "IController");
		}
	}
}