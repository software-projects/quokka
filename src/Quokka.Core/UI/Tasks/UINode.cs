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
using System.Linq;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;
using Quokka.Util;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Represents a single node in the <see cref = "UITask" /> transition graph.
	/// </summary>
	/// <remarks>
	/// 	<para>
	/// 		A <see cref = "UINode" /> can specify a presenter with or without
	/// 		specifying a view, and can specify a view node with or without specifying
	/// 		a presenter. A <see cref = "UINode" /> does need to have either a 
	/// 		view or a presenter, however.
	/// 	</para>
	/// </remarks>
	public sealed class UINode
	{
		internal UINode(UITask task, NodeBuilder nodeBuilder)
		{
			Task = Verify.ArgumentNotNull(task, "task");
			NodeBuilder = Verify.ArgumentNotNull(nodeBuilder, "nodeBuilder");
		}

		/// <summary>
		/// 	The <see cref = "UITask" /> that this node belongs to.
		/// </summary>
		public UITask Task { get; private set; }

		/// <summary>
		/// 	The <see cref = "NodeBuilder" /> used to create this node.
		/// </summary>
		internal NodeBuilder NodeBuilder { get; private set; }

		/// <summary>
		/// 	The type of presenter associated with this node.
		/// </summary>
		/// <remarks>
		/// 	<para>
		/// 		This type will usually be a subtype of <see cref = "Presenter" /> 
		/// 		or <see cref = "Presenter{TView}" />, but it does not need to be.		
		/// 	</para>
		/// </remarks>
		public Type PresenterType
		{
			get { return NodeBuilder.PresenterType; }
		}

		/// <summary>
		/// 	The type of the view. This is the type that will be requested from the container, and may
		/// 	or may not be an interface type.
		/// </summary>
		public Type ViewType
		{
			get { return NodeBuilder.ViewType; }
		}

		/// <summary>
		/// 	The type of view specified for this node. If the view type was
		/// 	not explicitly specified via <see cref = "INodeBuilder.SetView{TView}" />,
		/// 	then this property will be <c>null</c>, and the view type will be
		/// 	determined from <see cref = "InferredViewType" />.
		/// </summary>
		public Type DeclaredViewType
		{
			get { return NodeBuilder.DeclaredViewType; }
		}

		/// <summary>
		/// 	If the presenter is a subtype of <see cref = "Presenter{TView}" />,
		/// 	then this type is the type passed as the generic parameter <c>TView</c>.
		/// </summary>
		public Type InferredViewType
		{
			get { return NodeBuilder.InferredViewType; }
		}

		public Type NestedTaskType
		{
			get { return NodeBuilder.NestedTaskType; }
		}

		/// <summary>
		/// 	Options for this node.
		/// </summary>
		public UINodeOptions Options
		{
			get { return NodeBuilder.Options; }
		}

		/// <summary>
		/// 	The view object for this node, or <c>null</c>
		/// </summary>
		public object View { get; internal set; }

		/// <summary>
		/// 	The current presenter object for this node, or <c>null</c>.
		/// </summary>
		public object Presenter { get; internal set; }

		public UITask NestedTask { get; internal set; }

		public bool IsViewModal { get; internal set; }

		/// <summary>
		/// 	Should the view stay open when the node is not the current node.
		/// </summary>
		public bool StayOpen
		{
			get { return (Options & UINodeOptions.StayOpen) != 0; }
		}

		/// <summary>
		/// 	The service container for the node.
		/// </summary>
		public IServiceContainer Container { get; internal set; }

		/// <summary>
		/// 	The view deck for the node
		/// </summary>
		public IViewDeck ViewDeck { get; private set; }

		// TODO: probably better to have an event for this
		internal void TaskStarting()
		{
			ViewDeck = Task.ViewDeck;
			ViewDeck.ViewClosed += ViewDeckViewClosed;
		}

		// TODO: probably better to have as an event for the task?
		internal void EnterNode()
		{
			// If the node is being entered for the first time, its service container will be null
			// and has to be created.
			if (Container == null)
			{
				IServiceContainer parentContainer = Task.ServiceLocator.GetInstance<IServiceContainer>();
				Verify.IsNotNull(parentContainer);

				// Do not assign the container to _container yet -- wait until all the registration is 
				// complete. That way if one of the registrations throws an exception ,we are not left with
				// a node whose container is not fully initialised.
				IServiceContainer nodeContainer = parentContainer.CreateChildContainer();

				nodeContainer.RegisterInstance(this);
				nodeContainer.RegisterType<INavigateCommand, NavigateCommand>(ServiceLifecycle.PerRequest);

				// If the presenter is a concrete type and has not been registered yet, then register it
				// with the container.
				if (PresenterType != null && !PresenterType.IsInterface && !nodeContainer.IsTypeRegistered(PresenterType))
				{
					nodeContainer.RegisterType(PresenterType, null, null, ServiceLifecycle.PerRequest);
				}

				// If the view type has not been declared
				if (DeclaredViewType == null)
				{
					// If the inferred type is a concrete type and has not been registered, then register it
					if (InferredViewType != null && !InferredViewType.IsInterface && !nodeContainer.IsTypeRegistered(InferredViewType))
					{
						nodeContainer.RegisterType(InferredViewType, ServiceLifecycle.PerRequest);
					}
				}
				else
				{
					if (!DeclaredViewType.IsInterface)
					{
						if (InferredViewType != null && InferredViewType.IsInterface)
						{
							// The view type is concrete, and the presenter type is an interface
							nodeContainer.RegisterType(InferredViewType, DeclaredViewType, ServiceLifecycle.PerRequest);
						}
						else
						{
							// Use the concrete view type by itself.
							nodeContainer.RegisterType(DeclaredViewType, ServiceLifecycle.PerRequest);
						}
					}
				}

				if (NestedTaskType != null)
				{
					nodeContainer.RegisterType(NestedTaskType, ServiceLifecycle.PerRequest);
				}

				// node container is now initialised, assign to the member variable
				Container = nodeContainer;
			}
		}

		internal void CreatePresenter()
		{
			if (Presenter != null)
			{
				// presenter has already been created
				return;
			}

			if (PresenterType == null)
			{
				// don't have a presenter for this node
				return;
			}

			Presenter = Container.Locator.GetInstance(PresenterType);

			foreach (var nodeTransitionBuilder in NodeBuilder.PresenterTransitions)
			{
				var navigateCommand = nodeTransitionBuilder.Converter(Presenter) as NavigateCommand;
				if (navigateCommand == null)
				{
					throw new QuokkaException("Expected object of type NavigateCommand");
				}
				NodeTransitionBuilder builder = nodeTransitionBuilder;
				var nextNode = (from n in Task.Nodes
				                where n.NodeBuilder == builder.NextNode
				                select n).FirstOrDefault();

				navigateCommand.FromNode = this;
				navigateCommand.ToNode = nextNode;
				navigateCommand.Navigating += Task.Navigating;
			}

			foreach (var presenterInitialization in NodeBuilder.PresenterInitializations)
			{
				presenterInitialization(Presenter);
			}
		}

		internal void CreateView()
		{
			if (View != null)
			{
				// the view has already been created
				return;
			}

			if (DeclaredViewType == null && InferredViewType == null)
			{
				// don't have a view for this node
				return;
			}

			View = DeclaredViewType != null
			       	? Container.Locator.GetInstance(DeclaredViewType)
			       	: Container.Locator.GetInstance(InferredViewType);

			foreach (var nodeTransitionBuilder in NodeBuilder.ViewTransitions)
			{
				var navigateCommand = nodeTransitionBuilder.Converter(View) as NavigateCommand;
				if (navigateCommand == null)
				{
					throw new QuokkaException("Expected object of type NavigateCommand");
				}
				NodeTransitionBuilder builder = nodeTransitionBuilder;
				var nextNode = (from n in Task.Nodes
								where n.NodeBuilder == builder.NextNode
								select n).FirstOrDefault();

				navigateCommand.FromNode = this;
				navigateCommand.ToNode = nextNode;
				navigateCommand.Navigating += Task.Navigating;
			}

			foreach (var viewInitialization in NodeBuilder.ViewInitializations)
			{
				viewInitialization(View);
			}

			var p = Presenter as PresenterBase;
			if (p != null && p.ViewObject == null)
			{
				// the presenter does not know about the view yet, so tell it
				p.ViewObject = View;
			}
		}

		internal void CreateNestedTask()
		{
			var task = (UITask)Container.Locator.GetInstance(NestedTaskType);
			task.TaskComplete += NestedTaskComplete;
			task.Start(ViewDeck);
			NestedTask = task;
		}

		private void NestedTaskComplete(object sender, EventArgs e)
		{
			var task = (UITask) sender;
			task.TaskComplete -= NestedTaskComplete;
			UINode nextNode = null;

			foreach (var conditionalNodeTransitionBuilder in NodeBuilder.NestedTaskTransitions)
			{
				bool condition = conditionalNodeTransitionBuilder.Converter(task);
				if (condition)
				{
					var c = conditionalNodeTransitionBuilder;

					if (c.NextNode != null)
					{
						nextNode = (from n in Task.Nodes
						            where n.NodeBuilder == c.NextNode
						            select n).FirstOrDefault();
					}
				}
			}

			if (nextNode == null && NodeBuilder.NestedTaskNextNode != null)
			{
				nextNode = (from n in Task.Nodes
				            where n.NodeBuilder == NodeBuilder.NestedTaskNextNode
				            select n).FirstOrDefault();
			}

			if (nextNode != null)
			{
				Task.Navigate(nextNode);
			}
			else
			{
				// At this point the nested task has ended, and there is no indication
				// of where to navigate to. For this reason, the current task will also
				// end.
				Task.EndTask();
			}
		}

		// TODO: probably better to have an event for this
		internal void CleanupNode()
		{
			bool hideView = false;
			bool removeView = false;
			bool removeNestedTask = false;
			bool disposeContainer = false;

			if (Task == null)
			{
				disposeContainer = true;
			}
			else
			{
				if (Task.IsComplete)
				{
					// The task is complete -- all nodes should dispose of their container
					disposeContainer = true;
					removeView = true;
					removeNestedTask = true;

					// The task is not re-usable, but the view manager might be. Unregister
					// event to avoid a memory leak.
					if (ViewDeck != null)
					{
						ViewDeck.ViewClosed -= ViewDeckViewClosed;
					}
				}
				else
				{
					if (Task.CurrentNode != this)
					{
						// At this point we know that this node is not the current view
						if (Task.CurrentNode != null && Task.CurrentNode.IsViewModal)
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
								removeNestedTask = true;
							}
						}
					}
				}
			}

			if (hideView || removeView)
			{
				if (ViewDeck != null && View != null)
				{
					ViewDeck.HideView(View);
				}
			}

			if (removeView)
			{
				DisposeView();
				DisposePresenter();
			}

			if (removeNestedTask)
			{
				DisposeNestedTask();
			}

			if (disposeContainer)
			{
				DisposeView();
				DisposePresenter();
				DisposeNestedTask();
				DisposeContainer();
			}
		}

		private void DisposeView()
		{
			if (View != null)
			{
				if (ViewDeck != null)
				{
					ViewDeck.RemoveView(View);
				}
				DisposeUtils.DisposeOf(View);
				View = null;
			}
		}

		private void DisposePresenter()
		{
			if (Presenter != null)
			{
				DisposeUtils.DisposeOf(Presenter);
				Presenter = null;
			}
		}

		private void DisposeNestedTask()
		{
			if (NestedTask != null)
			{
				if (NestedTask.IsRunning)
				{
					NestedTask.EndTask();
				}
				DisposeUtils.DisposeOf(NestedTask);
				NestedTask = null;
			}
		}

		private void DisposeContainer()
		{
			if (Container != null)
			{
				DisposeUtils.DisposeOf(Container);
				Container = null;
			}
		}

		private void ViewDeckViewClosed(object sender, ViewClosedEventArgs e)
		{
			if (e.View != View)
			{
				// not this view
				return;
			}

			if (IsViewModal && this == Task.CurrentNode && Task.InNavigateMethod)
			{
				Task.PopNode();
			}
		}
	}
}