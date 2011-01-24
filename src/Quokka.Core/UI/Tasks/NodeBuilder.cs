using System;
using System.Collections.Generic;
using Quokka.Diagnostics;

namespace Quokka.UI.Tasks
{
	/// <summary>
	///		Internal class used for building nodes in a <see cref="UITask"/>.
	/// </summary>
	internal class NodeBuilder : IAnyNodeBuilder
	{
		public TaskBuilder Task { get; private set; }
		public string Name { get; private set; }
		public Type PresenterType { get; protected set; }
		public Type DeclaredViewType { get; protected set; }
		public Type InferredViewType { get; protected set; }
		public Type NestedTaskType { get; protected set; }
		public UINodeOptions Options { get; protected set; }
		public INodeBuilder NestedTaskNextNode { get; protected set; }

		public IList<Action<object>> ViewInitializations = new List<Action<object>>();
		public IList<Action<object>> PresenterInitializations = new List<Action<object>>();
		public IList<Action<object>> NestedTaskInitializations = new List<Action<object>>();
		public IList<NodeTransitionBuilder> ViewTransitions = new List<NodeTransitionBuilder>();
		public IList<NodeTransitionBuilder> PresenterTransitions = new List<NodeTransitionBuilder>();
		public IList<NodeTransitionBuilder> NestedTaskTransitions = new List<NodeTransitionBuilder>();

		private bool _isValidated;

		public NodeBuilder(TaskBuilder task, string name)
		{
			Task = task;
			Name = name;
		}

		public void Validate()
		{
			if (_isValidated)
			{
				return;
			}
			_isValidated = true;

			if (string.IsNullOrEmpty(Name))
			{
				Name = InferName();
			}

			if (PresenterType == null && DeclaredViewType == null && NestedTaskType == null)
			{
				AddError("No presenter, view or nested task defined");
				return;
			}

			if (DeclaredViewType != null
				&& InferredViewType != null
				&& !InferredViewType.IsAssignableFrom(DeclaredViewType))
			{
				string message = string.Format("Declared view type {0} is not consistent with the presenter view type {1}",
											   DeclaredViewType.Name, InferredViewType.Name);
				AddError(message);
				return;
			}
		}

		public Type ViewType
		{
			get { return DeclaredViewType ?? InferredViewType; }
		}

		public IPresenterNodeBuilder<TPresenter> SetPresenter<TPresenter>()
		{
			if (PresenterType != null && PresenterType != typeof(TPresenter))
			{
				AddError("SetPresenter<> can only be called once for a node");
			}
			else
			{
				PresenterType = typeof (TPresenter);
				InferredViewType = InferViewType(PresenterType);
			}
			return new PresenterNodeBuilder<TPresenter>(this);
		}

		public IViewNodeBuilder<TView> SetView<TView>()
		{
			if (DeclaredViewType != null && DeclaredViewType != typeof(TView))
			{
				AddError("SetView<> can only be called once for a node");
			}
			else
			{
				DeclaredViewType = typeof (TView);
			}
			return new ViewNodeBuilder<TView>(this);
		}

		public ITaskNodeBuilder<TNestedTask> SetNestedTask<TNestedTask>() where TNestedTask : UITask
		{
			NestedTaskType = typeof (TNestedTask);
			return new NestedTaskNodeBuilder<TNestedTask>(this);
		}

		private void StayOpen()
		{
			Options |= UINodeOptions.StayOpen;
		}

		private static Type InferViewType(Type presenterType)
		{
			if (presenterType == null || presenterType == typeof(object))
			{
				// we have walked all the way up the inheritance hierarchy
				return null;
			}

			if ((presenterType.IsGenericType && !presenterType.IsGenericTypeDefinition)
				&& presenterType.GetGenericTypeDefinition() == typeof(Presenter<>))
			{
				return presenterType.GetGenericArguments()[0];
			}

			// try the base class
			return InferViewType(presenterType.BaseType);
		}

		private string InferName()
		{
			return "TODO";
		}

		private void AddError(string reason)
		{
			Task.AddError(this, reason);
		}


		internal class ViewNodeBuilder<TView> : IViewNodeBuilder<TView>
		{
			protected readonly NodeBuilder InnerNodeBuilder;

			public ViewNodeBuilder(NodeBuilder nodeBuilder)
			{
				InnerNodeBuilder = Verify.ArgumentNotNull(nodeBuilder, "nodeBuilder");
			}

			public IViewNodeBuilder<TView> NavigateTo(Converter<TView, INavigateCommand> converter, INodeBuilder node)
			{
				Verify.ArgumentNotNull(converter, "converter");
				Converter<object, INavigateCommand> c = obj => converter((TView)obj);
				var transition = new NodeTransitionBuilder(c, node);
				InnerNodeBuilder.ViewTransitions.Add(transition);
				return this;
			}

			public IViewNodeBuilder<TView> With(Action<TView> action)
			{
				Verify.ArgumentNotNull(action, "action");
				Action<object> initialization = obj => action((TView)obj);
				InnerNodeBuilder.ViewInitializations.Add(initialization);
				return this;
			}

			public IPresenterNodeBuilder<TPresenter> SetPresenter<TPresenter>()
			{
				return InnerNodeBuilder.SetPresenter<TPresenter>();
			}

			public IViewNodeBuilder<TView> StayOpen()
			{
				InnerNodeBuilder.StayOpen();
				return this;
			}
		}

		internal class PresenterNodeBuilder<TPresenter> : IPresenterNodeBuilder<TPresenter>
		{
			protected readonly NodeBuilder InnerNodeBuilder;

			public PresenterNodeBuilder(NodeBuilder nodeBuilder)
			{
				InnerNodeBuilder = Verify.ArgumentNotNull(nodeBuilder, "nodeBuilder");
			}

			public IPresenterNodeBuilder<TPresenter> NavigateTo(Converter<TPresenter, INavigateCommand> converter, INodeBuilder node)
			{
				Verify.ArgumentNotNull(converter, "converter");
				Converter<object, INavigateCommand> c = obj => converter((TPresenter)obj);
				var transition = new NodeTransitionBuilder(c, node);
				InnerNodeBuilder.PresenterTransitions.Add(transition);
				return this;
			}

			public IPresenterNodeBuilder<TPresenter> With(Action<TPresenter> action)
			{
				Verify.ArgumentNotNull(action, "action");
				Action<object> initialization = obj => action((TPresenter)obj);
				InnerNodeBuilder.PresenterInitializations.Add(initialization);
				return this;
			}

			public IViewNodeBuilder<TView> SetView<TView>()
			{
				return InnerNodeBuilder.SetView<TView>();
			}

			public IPresenterNodeBuilder<TPresenter> StayOpen()
			{
				InnerNodeBuilder.StayOpen();
				return this;
			}
		}

		internal class NestedTaskNodeBuilder<TNestedTask> : ITaskNodeBuilder<TNestedTask> where TNestedTask : UITask
		{
			protected readonly NodeBuilder InnerNodeBuilder;

			public NestedTaskNodeBuilder(NodeBuilder nodeBuilder)
			{
				InnerNodeBuilder = Verify.ArgumentNotNull(nodeBuilder, "nodeBuilder");
			}

			public ITaskNodeBuilder<TNestedTask> NavigateTo(Converter<TNestedTask, INavigateCommand> converter, INodeBuilder node)
			{
				Verify.ArgumentNotNull(converter, "converter");
				Converter<object, INavigateCommand> c = obj => converter((TNestedTask)obj);
				var transition = new NodeTransitionBuilder(c, node);
				InnerNodeBuilder.NestedTaskTransitions.Add(transition);
				return this;
			}

			public ITaskNodeBuilder<TNestedTask> NavigateTo(INodeBuilder node)
			{
				Verify.ArgumentNotNull(node, "node");
				InnerNodeBuilder.NestedTaskNextNode = node;
				return this;
			}


			public ITaskNodeBuilder<TNestedTask> With(Action<TNestedTask> action)
			{
				Verify.ArgumentNotNull(action, "action");
				Action<object> initialization = obj => action((TNestedTask)obj);
				InnerNodeBuilder.NestedTaskInitializations.Add(initialization);
				return this;
			}
		}
	}
}