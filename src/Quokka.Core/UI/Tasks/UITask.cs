﻿#region Copyright notice
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
using Quokka.Util;

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
		private readonly ErrorReport _errorReport = new ErrorReport();
		private readonly Dictionary<string, object> _taskData = new Dictionary<string, object>(); 

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

		/// <summary>
		/// The current <see cref="UITask"/>. This value is non-null only if
		/// a task is currently starting or navigating in the current call context.
		/// </summary>
		/// <remarks>
		/// Used for implementing UITask-aware services, such as NHibernate session management.
		/// </remarks>
		public static UITask Current
		{
			get { return UICurrentTask.CurrentTask; }
		}

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
			using (UICurrentTask.SetCurrentTask(this))
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

				_errorReport.Clear();
				_serviceContainer.RegisterInstance(_errorReport);

				// Setup the ViewDeck property so that views can be displayed by this task.
				//
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

				try
				{
					// If the nodes have not been built yet, then build them
					if (_nodes == null)
					{
						BuildNodes();
					}

					// throw an exception if the task is not valid
					_taskBuilder.AssertValid();


					try
					{
						CreateState();
					}
					catch (Exception ex)
					{
						var msg = string.Format("Exception thrown during CreateState method for UITask {0}", GetType());
						_errorReport.ReportError(msg, ex);
						throw;
					}

					// Now that the task has a view deck, we can inform the nodes and let them initialize.
					foreach (var node in Nodes)
					{
						node.TaskStarting();
					}
				}
				catch (UITaskInvalidException ex)
				{
					_errorReport.ReportError("UITask cannot start because it failed verification checks", ex.Message);
					AddErrorReportPropertiesForTask();
				}
				catch (Exception ex)
				{
					if (!_errorReport.HasErrorOccurred)
					{
						_errorReport.ReportError("Exception thrown during UI task startup", ex);
						AddErrorReportPropertiesForTask();
					}
				}

				if (_errorReport.HasErrorOccurred)
				{
					ShowErrorView();
				}
				else
				{
					Navigate(Nodes[0]);
				}
			}
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

			ClearTaskData();

			CurrentNode = null;
			IsComplete = true;
			_raiseTaskComplete = true;
			RaiseEvents();
		}

		protected virtual void Dispose(bool disposing)
		{
			
		}

		private void ClearTaskData()
		{
			// Dispose of any objects stored in the UITask context that implement IDisposable.
			foreach (var obj in _taskData.Values)
			{
				try
				{
					DisposeUtils.DisposeOf(obj);
				}
				catch (Exception ex)
				{
					var message = string.Format("Unexpected exception disposing of context object of type {0}: {1}",
												obj.GetType(), ex.Message);
					Log.Error(message, ex);
				}
			}

			_taskData.Clear();
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

		/// <summary>
		/// Used for storing arbitrary data against the the UITask. Used by <see cref="UICurrentTask"/>
		/// TODO: not well thought out, internal for now.
		/// </summary>
		internal void SetData(string slotName, object data)
		{
			Verify.ArgumentNotNull(slotName, "slotName");
			lock (_taskData)
			{
				if (data == null)
				{
					_taskData.Remove(slotName);
				}
				else
				{
					_taskData[slotName] = data;
				}
			}
		}

		/// <summary>
		/// Used for storing arbitrary data against the the UITask. Used by <see cref="UICurrentTask"/>.
		/// Will be disposed when the task completes. TODO: not well thought out, internal for now.
		/// </summary>
		internal object GetData(string slotName)
		{
			Verify.ArgumentNotNull(slotName, "slotName");
			object result;
			lock(_taskData)
			{
				_taskData.TryGetValue(slotName, out result);
			}
			return result;
		}


		/// <summary>
		/// Gets the object associated with the current node, or <c>null</c> if not found.
		/// </summary>
		public T GetTaskLifetimeObject<T>(string name) where T : class, IDisposable
		{
			return (T) GetData(SlotName<T>(name));
		}

		/// <summary>
		/// Associates a disposable object with the current node. This object will be 
		/// disposed when the current node changes.
		/// </summary>
		public void SetTaskLifetimeObject<T>(string name, T data) where T : class, IDisposable
		{
			SetData(SlotName<T>(name), data);
		}

		/// <summary>
		/// Gets the object associated with the current node, or <c>null</c> if not found.
		/// </summary>
		public T GetNodeLifetimeObject<T>(string name) where T : class, IDisposable
		{
			if (CurrentNode == null)
			{
				throw new InvalidOperationException("No current node defined for task");
			}
			return (T) CurrentNode.GetData(SlotName<T>(name));
		}

		/// <summary>
		/// Associates a disposable object with the current node. This object will be 
		/// disposed when the current node changes.
		/// </summary>
		public void SetNodeLifetimeObject<T>(string name, T data) where T : class, IDisposable
		{
			if (CurrentNode == null)
			{
				throw new InvalidOperationException("No current node defined for task");
			}
			CurrentNode.SetData(SlotName<T>(name), data);
		}

		private static string SlotName<T>(string name)
		{
			if (name == null)
			{
				return "<NULL>-" + typeof(T).FullName;
			}
			return "(" + name + ")-" + typeof(T).FullName;
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
				catch (Exception ex)
				{
					// Report error message as close to catching the exception as possible.
					// This will appear on the IErrorView view.
					var message = string.Format("Exception thrown in CreateNodes method for UITask {0}", GetType());
					_errorReport.ReportError(message, ex);
					throw;
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

		internal void NavigationNotDefined(object sender, EventArgs e)
		{
			var navigateCommand = (NavigateCommand) sender;
			var context = string.Format("Navigating from node " + navigateCommand.FromNode.Name);
			var detail = string.Format("An attempt has been made to navigate from node {0}"
			                           + " using a NavigateCommand whose transition has not been defined.",
			                           navigateCommand.FromNode.Name);
			var resolution = "Check for an INavigateCommand in your presenter or view that has not had a"
			                 + " transition defined in the UITask.";
			_errorReport.ReportError(context, detail);
			_errorReport.Resolution = resolution;
			AddErrorReportPropertiesForNode(navigateCommand.FromNode);
			ShowErrorView();
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
				_errorReport.ReportError("Navigation attempted on completed UI Task", message);
				ShowErrorView();
				return;
			}

			if (navigateCommand.FromNode != CurrentNode)
			{
				var message = string.Format("Attempt to navigate from {0} to {1} when the current node is {2}",
				               navigateCommand.FromNode,
				               navigateCommand.ToNode,
				               CurrentNode);
				Log.Error(message);
				_errorReport.ReportError("Navigation error", message);
				_errorReport.Properties.Add("UITask", GetType().FullName);
				_errorReport.Properties.Add("FromNode", navigateCommand.FromNode.ToString());
				_errorReport.Properties.Add("ToNode", navigateCommand.ToNode.ToString());
				ShowErrorView();
				return;
			}

			UINode nextNode = navigateCommand.ToNode;
			Navigate(nextNode);

		}

		internal void Navigate(UINode nextNode)
		{
			using (UICurrentTask.SetCurrentTask(this))
			{
				var errorReportedPriorToNavigation = _errorReport.HasErrorOccurred;
				var fromNode = CurrentNode;

				try
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

				}
				catch (Exception ex)
				{
					if (!_errorReport.HasErrorOccurred)
					{
						_errorReport.ReportError("Unexpected exception during UI navigation", ex);
					}

					if (_errorReport.Exception == null)
					{
						_errorReport.Exception = ex;
					}

					if (!_errorReport.Properties.ContainsKey("UITask"))
					{
						_errorReport.Properties.Add("UITask", GetType().FullName);
					}
					if (!_errorReport.Properties.ContainsKey("FromNode"))
					{
						_errorReport.Properties.Add("FromNode", fromNode);
					}
					if (!_errorReport.Properties.ContainsKey("ToNode"))
					{
						_errorReport.Properties.Add("ToNode", nextNode);
					}
				}

				if (_errorReport.HasErrorOccurred && !errorReportedPriorToNavigation)
				{
					// An error report was generated during this method.
					ShowErrorView();
				}
				else
				{
					RaiseEvents();
				}
			}
		}

		// Display an error view. This is the end of the line for the UI task in question.
		private void ShowErrorView()
		{
			// Set the current node to null, and cleanup all nodes. This should get
			// rid of any modal windows being displayed.
			CurrentNode = null;
			foreach (var node in Nodes)
			{
				node.CleanupNode();
			}

			if (_viewDeck != null)
			{
				var errorView = ServiceContainer.Locator.GetInstance<IErrorReportView>();
				errorView.AbortCommand.Enabled = false;
				errorView.CancelCommand.Enabled = false;
				errorView.RetryCommand.Enabled = false;
				errorView.ErrorReport = _errorReport;

				using (var transition = _viewDeck.BeginTransition(this))
				{
					transition.AddView(errorView);
					transition.ShowView(errorView);
				}
			}
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
					using (var transition = nextNode.GetViewDeck(createIfNecessary:true).BeginTransition(this))
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
					ClearTaskData();
					_raiseTaskComplete = true;
				}
			}
			finally
			{
				_inNavigateMethod = false;
			}

			if (showModalView && CurrentNode != null)
			{
				using (var transition = CurrentNode.GetViewDeck(createIfNecessary:true).BeginTransition(this))
				{
					transition.ShowView(CurrentNode.View);
				}
				CurrentNode.GetModalWindow(createIfNecessary:true).ShowModal(false);
			}
		}

		private void AddErrorReportPropertiesForTask()
		{
			_errorReport.Properties["UITask"] = GetType().FullName;
			_errorReport.Properties["CurrentNode"] = CurrentNode;
		}

		private void AddErrorReportPropertiesForNode(UINode node)
		{
			AddErrorReportPropertiesForTask();
			if (node.NestedTaskType != null)
			{
				_errorReport.Properties["Nested UITask"] = node.NestedTaskType.FullName;
			}
			else
			{
				if (node.DeclaredViewType != null)
				{
					_errorReport.Properties["Declared View"] = node.DeclaredViewType.FullName;
				}
				if (node.InferredViewType != null)
				{
					_errorReport.Properties["Inferred View"] = node.InferredViewType.FullName;
				}
				if (node.PresenterType != null)
				{
					_errorReport.Properties["Presenter"] = node.PresenterType.FullName;
				}
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
						try
						{
							CurrentNode.CreateView();
						}
						catch (Exception ex)
						{
							_errorReport.ReportError("Unexpected exception creating view", ex);
							AddErrorReportPropertiesForNode(CurrentNode);
							throw;
						}
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
						try
						{
							p.PerformPresenterInitialization();
						}
						catch (Exception ex)
						{
							// This is a common enough error condition. The presenter has thrown an exception
							// during initialization. Reporting as close as possible to where the exception is
							// thrown greatly assists debugging.
							_errorReport.ReportError("Unexpected exception thrown during presenter initialization", ex);
							AddErrorReportPropertiesForNode(CurrentNode);
							throw;
						}
					}
				}

				if (CurrentNode.NestedTask == null && CurrentNode.NestedTaskType != null)
				{
					try
					{
						CurrentNode.CreateNestedTask();
					}
					catch (Exception ex)
					{
						_errorReport.ReportError("Unexpected exception thrown during nested task initialization", ex);
						AddErrorReportPropertiesForNode(CurrentNode);
						throw;
					}
				}
			}
		}

		private void RaiseEvents()
		{
			string context = "Raising TaskComplete event";
			bool showError = false;

			try
			{
				if (_raiseTaskComplete)
				{
					OnTaskComplete(EventArgs.Empty);
					_raiseTaskComplete = false;
				}

				context = "Raising TaskStarted event";
				if (_raiseTaskStarted)
				{
					OnTaskStarted(EventArgs.Empty);
					_raiseTaskStarted = false;
				}
			}
			catch (Exception ex)
			{
				_errorReport.ReportError(context, ex);
				showError = true;
			}

			if (showError)
			{
				ShowErrorView();
			}
		}

		#endregion
	}
}