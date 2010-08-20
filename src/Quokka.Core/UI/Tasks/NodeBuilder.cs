using System;
using System.Collections.Generic;
using Quokka.Diagnostics;

namespace Quokka.UI.Tasks
{
	internal class NodeBuilder : INodeBuilder
	{
		public TaskBuilder Task { get; private set; }
		public string Name { get; private set; }
		public Type PresenterType { get; protected set; }
		public Type DeclaredViewType { get; protected set; }
		public Type InferredViewType { get; protected set; }
		public UINodeOptions Options { get; protected set; }

		public IList<Action<object>> ViewInitializations = new List<Action<object>>();
		public IList<Action<object>> PresenterInitializations = new List<Action<object>>();
		public IList<NodeTransitionBuilder> ViewTransitions = new List<NodeTransitionBuilder>();
		public IList<NodeTransitionBuilder> PresenterTransitions = new List<NodeTransitionBuilder>();

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

			if (PresenterType == null && DeclaredViewType == null)
			{
				AddError("No presenter or view defined");
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

		public UINode CreateNode()
		{
			throw new NotImplementedException();
		}

		public Type ViewType
		{
			get { return DeclaredViewType ?? InferredViewType; }
		}

		public INodeBuilder<TPresenter> SetPresenter<TPresenter>()
		{
			if (PresenterType != null)
			{
				throw new InvalidOperationException("SetPresenter<> can only be called once for a node");
			}
			PresenterType = typeof(TPresenter);
			InferredViewType = InferViewType(PresenterType);
			return new PresenterNodeBuilder<TPresenter>(this);
		}

		public INodeBuilder<TView> SetView<TView>()
		{
			if (DeclaredViewType != null)
			{
				throw new InvalidOperationException("SetView<> can only be called once for a node");
			}
			DeclaredViewType = typeof(TView);
			return new ViewNodeBuilder<TView>(this);
		}

		public INodeBuilder StayOpen()
		{
			Options |= UINodeOptions.StayOpen;
			return this;
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

		internal abstract class GenericNodeBuilder<T> : INodeBuilder<T>
		{
			protected readonly NodeBuilder InnerNodeBuilder;

			internal GenericNodeBuilder(NodeBuilder nodeBuilder)
			{
				Verify.ArgumentNotNull(nodeBuilder, "nodeBuilder");
			}

			public INodeBuilder<T> NavigateTo(Converter<T, NavigateCommand> converter, INodeBuilder node)
			{
				Converter<object, NavigateCommand> c = obj => converter((T)obj);
				var transition = new NodeTransitionBuilder(c, node);
				AssignTransition(transition);
				return this;
			}

			public INodeBuilder<TPresenter> SetPresenter<TPresenter>()
			{
				return InnerNodeBuilder.SetPresenter<TPresenter>();
			}

			public INodeBuilder<TView> SetView<TView>()
			{
				return InnerNodeBuilder.SetView<TView>();
			}

			public INodeBuilder StayOpen()
			{
				return InnerNodeBuilder.StayOpen();
			}

			public INodeBuilder<T> With(Action<T> action)
			{
				// TODO
				return this;
			}

			protected abstract void AssignTransition(NodeTransitionBuilder transition);
			protected abstract void AssignInitialization(Action<object> initialization);
		}

		internal class ViewNodeBuilder<T> : GenericNodeBuilder<T>
		{
			public ViewNodeBuilder(NodeBuilder nodeBuilder) : base(nodeBuilder) { }

			protected override void AssignTransition(NodeTransitionBuilder transition)
			{
				InnerNodeBuilder.ViewTransitions.Add(transition);
			}

			protected override void AssignInitialization(Action<object> initialization)
			{
				InnerNodeBuilder.ViewInitializations.Add(initialization);
			}
		}

		internal class PresenterNodeBuilder<T> : GenericNodeBuilder<T>
		{
			public PresenterNodeBuilder(NodeBuilder nodeBuilder) : base(nodeBuilder) { }

			protected override void AssignTransition(NodeTransitionBuilder transition)
			{
				InnerNodeBuilder.PresenterTransitions.Add(transition);
			}

			protected override void AssignInitialization(Action<object> initialization)
			{
				InnerNodeBuilder.PresenterInitializations.Add(initialization);
			}
		}
	}
}