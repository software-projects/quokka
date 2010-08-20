using System;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;
using Quokka.Uip;
using Quokka.Util;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Represents a single node in the <see cref="UITask"/> transition graph.
	/// </summary>
	/// <remarks>
	/// A <see cref="UINode"/> can specify a presenter with or without
	/// specifying a view, and can specify a view node with or without specifying
	/// a presenter. A <see cref="UINode"/> does need to have either a 
	/// view or a presenter, however.
	/// </para>
	/// </remarks>
	public sealed class UINode
	{
		/// <summary>
		/// The <see cref="UITask"/> that this node belongs to.
		/// </summary>
		public UITask Task { get; internal set; }

		/// <summary>
		/// The type of presenter associated with this node. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// This type will usually be a subtype of <see cref="Presenter"/> 
		/// or <see cref="Presenter{TView}"/>, but it does not need to be.		
		/// </para>
		/// </remarks>
		public Type PresenterType { get; internal set; }

		/// <summary>
		/// The type of view specified for this node. If the view type was
		/// not explicitly specified via <see cref="INodeBuilder.SetView{TView}"/>,
		/// then this property will be <c>null</c>, and the view type will be
		/// determined from <see cref="PresenterViewType"/>.
		/// </summary>
		public Type ViewType { get; internal set; }

		/// <summary>
		/// If the presenter is a subtype of <see cref="Presenter{TView}"/>,
		/// then this type is the type passed as the generic parameter <c>TView</c>.
		/// </summary>
		public Type PresenterViewType { get; internal set; }

		/// <summary>
		/// Options for this node.
		/// </summary>
		public UINodeOptions Options { get; internal set; }

		/// <summary>
		/// The view object for this node, or <c>null</c>
		/// </summary>
		public object View { get; internal set; }

		/// <summary>
		/// The current presenter object for this node, or <c>null</c>.
		/// </summary>
		public object Presenter { get; internal set; }

		public bool IsViewModal { get; internal set; }

		public bool StayOpen { get; internal set; }

		public IServiceContainer Container { get; internal set; }

		public IViewDeck ViewDeck { get; private set; }

		// TODO: probably better to have an event for this
		internal void TaskStarting()
		{
			ViewDeck = Task.ViewDeck;
			ViewDeck.ViewClosed += ViewDeck_ViewClosed;
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

				// Both controller and view are singletons. The container only lasts as long as the
				// node is current (or as long as the task runs in the case of a StayOpen node).
				if (PresenterType != null && !PresenterType.IsInterface)
				{
					nodeContainer.RegisterType(PresenterType, null, null, ServiceLifecycle.Singleton);
				}
				if (ViewType == null)
				{
					if (PresenterViewType != null && !PresenterViewType.IsInterface)
					{
						nodeContainer.RegisterType(PresenterViewType, ServiceLifecycle.Singleton);
					}
				}
				else
				{
					if (!ViewType.IsInterface)
					{
						if (PresenterViewType != null && PresenterViewType.IsInterface)
						{
							// The view type is concrete, and the presenter type is an interface
							nodeContainer.RegisterType(PresenterViewType, ViewType, ServiceLifecycle.Singleton);
						}
						else
						{
							// Use the concrete view type by itself.
							nodeContainer.RegisterType(ViewType, ServiceLifecycle.Singleton);
						}
					}
				}

				// node container is now initialised, assign to the member variable
				Container = nodeContainer;
			}
		}

		internal void CreatePresenterAndView()
		{
			if (Presenter == null && PresenterType != null)
			{
				Presenter = Container.Locator.GetInstance(PresenterType);
			}

			if (View == null && ViewType != null)
			{
				
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

			// The presenter might have created the view through a construction injection or property injection
			var p = Presenter as PresenterBase;
			if (p != null && View == null)
			{
				// The view was created when the presenter was created
				View = p.ViewObject;
			}
		}

		internal void CreateView()
		{
			if (View != null)
			{
				// the view has already been created
				return;
			}

			if (ViewType == null && PresenterViewType == null)
			{
				// don't have a view for this node
				return;
			}

			View = ViewType != null ? Container.Locator.GetInstance(ViewType) : Container.Locator.GetInstance(PresenterViewType);

			var p = Presenter as PresenterBase;
			if (p != null && p.ViewObject == null)
			{
				// the presenter does not know about the view yet, so tell it
				p.ViewObject = View;
			}
		}


		// TODO: probably better to have an event for this
		internal void CleanupNode()
		{
			bool hideView = false;
			bool removeView = false;
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

					// The task is not re-usable, but the view manager might be. Unregister
					// event to avoid a memory leak.
					if (ViewDeck != null)
					{
						ViewDeck.ViewClosed -= ViewDeck_ViewClosed;
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
							}
						}
					}
				}
			}

			if (hideView || removeView)
			{
				if (ViewDeck != null)
				{
					ViewDeck.HideView(View);
				}
			}

			if (removeView)
			{
				DisposeView();
				DisposePresenter();
			}

			if (disposeContainer)
			{
				DisposeView();
				DisposePresenter();
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

		private void DisposeContainer()
		{
			if (Container != null)
			{
				DisposeUtils.DisposeOf(Container);
				Container = null;
			}
		}

		private void ViewDeck_ViewClosed(object sender, ViewClosedEventArgs e)
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