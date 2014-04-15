#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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
		public int Index { get; private set; }
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
		public IList<ConditionalNodeTransitionBuilder> NestedTaskTransitions = new List<ConditionalNodeTransitionBuilder>();

		private bool _isValidated;

		public NodeBuilder(TaskBuilder task, string name, int index)
		{
			Task = task;
			Name = name;
			Index = index;
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

		public INestedTaskNodeBuilder<TNestedTask> SetNestedTask<TNestedTask>() where TNestedTask : UITask
		{
			NestedTaskType = typeof (TNestedTask);
			return new NestedNestedTaskNodeBuilder<TNestedTask>(this);
		}

		private void StayOpen()
		{
			Options |= UINodeOptions.StayOpen;
		}

		private void ShowModal()
		{
			Options |= UINodeOptions.ShowModal;
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
			string inferredName = null;
			const string nodeSuffix = "Node";

			if (PresenterType != null)
			{
				const string presenterSuffix = "Presenter";
				var presenterName = PresenterType.Name;
				if (presenterName.EndsWith(presenterSuffix))
				{
					inferredName = presenterName.Substring(0, presenterName.Length - presenterSuffix.Length) + nodeSuffix;
				}
				else
				{
					inferredName = presenterName + nodeSuffix;
				}
			}
			else if (DeclaredViewType != null)
			{
				const string viewSuffix = "View";
				var viewName = ViewType.Name;
				if (viewName.EndsWith(viewSuffix))
				{
					inferredName = viewName.Substring(0, viewName.Length - viewSuffix.Length) + nodeSuffix;
				}
				else
				{
					inferredName = viewName + nodeSuffix;
				}
			}
			else if (NestedTaskType != null)
			{
				const string taskSuffix = "Task";
				var taskName = NestedTaskType.Name;
				if (taskName.EndsWith(taskSuffix))
				{
					inferredName = taskName.Substring(0, taskName.Length - taskSuffix.Length) + nodeSuffix;
				}
				else
				{
					inferredName = taskName + nodeSuffix;
				}
			}
			else
			{
				inferredName = string.Format("Node[{0}]", Index);
			}

			return inferredName;
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

			public IViewNodeBuilder<TView> ShowModal()
			{
				InnerNodeBuilder.ShowModal();
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

			public IPresenterNodeBuilder<TPresenter> ShowModal()
			{
				InnerNodeBuilder.ShowModal();
				return this;
			}
		}

		internal class NestedNestedTaskNodeBuilder<TNestedTask> : INestedTaskNodeBuilder<TNestedTask> where TNestedTask : UITask
		{
			protected readonly NodeBuilder InnerNodeBuilder;

			public NestedNestedTaskNodeBuilder(NodeBuilder nodeBuilder)
			{
				InnerNodeBuilder = Verify.ArgumentNotNull(nodeBuilder, "nodeBuilder");
			}

			public INestedTaskNodeBuilder<TNestedTask> NavigateTo(Converter<TNestedTask, bool> converter, INodeBuilder node)
			{
				Verify.ArgumentNotNull(converter, "converter");
				Converter<object, bool> c = obj => converter((TNestedTask)obj);
				var transition = new ConditionalNodeTransitionBuilder(c, node);
				InnerNodeBuilder.NestedTaskTransitions.Add(transition);
				return this;
			}

			public INestedTaskNodeBuilder<TNestedTask> NavigateTo(INodeBuilder node)
			{
				Verify.ArgumentNotNull(node, "node");
				InnerNodeBuilder.NestedTaskNextNode = node;
				return this;
			}

			public INestedTaskNodeBuilder<TNestedTask> With(Action<TNestedTask> action)
			{
				Verify.ArgumentNotNull(action, "action");
				Action<object> initialization = obj => action((TNestedTask)obj);
				InnerNodeBuilder.NestedTaskInitializations.Add(initialization);
				return this;
			}

			public INestedTaskNodeBuilder<TNestedTask> ShowModal()
			{
				InnerNodeBuilder.ShowModal();
				return this;
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}