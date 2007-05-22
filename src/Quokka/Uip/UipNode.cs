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

namespace Quokka.Uip
{
	using System;
	using System.Collections.Generic;
	using Quokka.Reflection;
	using Quokka.Uip.Implementation;

	[Flags]
	public enum UipNodeOptions
	{
		None = 0,
		ModalView = 1,
		StayOpen = 2,
	}

	public class UipNode
	{
		private readonly UipTaskDefinition _task;
		private readonly string _name;
		private readonly Type _viewType;
		private readonly Type _controllerType;
		private readonly PropertyCollection _viewProperties;
		private readonly PropertyCollection _controllerProperties;
		private readonly List<UipTransition> _transitions;
		private readonly Type _controllerInterface;
		private readonly UipNodeOptions _options;

		internal UipNode(UipTaskDefinition task, NodeConfig nodeConfig)
		{
			_task = task;
			_name = nodeConfig.Name;
			if (nodeConfig.View != null && !String.IsNullOrEmpty(nodeConfig.View.TypeName)) {
				_viewType = TypeUtil.FindType(nodeConfig.View.TypeName, task.Namespaces, task.Assemblies, true);
				_controllerInterface = GetControllerInterfaceFromViewType(_viewType);
				_viewProperties = new PropertyCollection(nodeConfig.View.Properties);
			}
			_controllerType = TypeUtil.FindType(nodeConfig.Controller.TypeName, task.Namespaces, task.Assemblies);
			_controllerProperties = new PropertyCollection(nodeConfig.Controller.Properties);
			_transitions = new List<UipTransition>();
			_options = UipNodeOptions.None;
			if (nodeConfig.View != null) {
				if (nodeConfig.View.OpenModal) {
					_options |= UipNodeOptions.ModalView;
				}
				if (nodeConfig.View.StayOpen) {
					// Cannot stay open if a modal view
					if (!IsViewModal) {
						_options |= UipNodeOptions.StayOpen;
					}
				}
			}
		}

		internal UipNode(UipTaskDefinition taskDefinition,
		                 string name,
		                 Type viewType,
		                 Type controllerType,
		                 UipNodeOptions options)
		{
			Assert.ArgumentNotNull(taskDefinition, "taskDefinition");
			Assert.ArgumentNotNull(name, "name");
			// view type can be null
			Assert.ArgumentNotNull(controllerType, "controllerType");

			_task = taskDefinition;
			_name = name;
			_viewType = viewType;
			_controllerInterface = GetControllerInterfaceFromViewType(viewType);
			_viewProperties = new PropertyCollection();
			_controllerType = controllerType;
			_controllerProperties = new PropertyCollection();
			_transitions = new List<UipTransition>();
			_options = options;
		}

		public string Name
		{
			get { return _name; }
		}

		internal UipTaskDefinition TaskDefinition
		{
			get { return _task; }
		}

		public Type ViewType
		{
			get { return _viewType; }
		}

		public Type ControllerInterface
		{
			get { return _controllerInterface; }
		}

		public PropertyCollection ViewProperties
		{
			get { return _viewProperties; }
		}

		public Type ControllerType
		{
			get { return _controllerType; }
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
			get { return (_options & UipNodeOptions.StayOpen) != 0; }
		}

		public PropertyCollection ControllerProperties
		{
			get { return _controllerProperties; }
		}

		public IList<UipTransition> Transitions
		{
			get { return _transitions; }
		}

		public UipNode NavigateTo(string navigateValue, string nodeName)
		{
			_transitions.Add(new UipTransition(this, navigateValue, nodeName));
			return this;
		}

		public UipNode NavigateTo(string navigateValue, UipNode node)
		{
			_transitions.Add(new UipTransition(this, navigateValue, node));
			return this;
		}

		public bool GetNextNode(string navigateValue, out UipNode node)
		{
			foreach (UipTransition transition in _transitions) {
				if (transition.NavigateValue == navigateValue) {
					node = transition.NextNode;
					return true;
				}
			}
			node = null;
			return false;
		}

		private Type GetControllerInterfaceFromViewType(Type viewType)
		{
			if (viewType != null) {
				// look for a nested interface in the view called 'IController'
				Type nestedType = _viewType.GetNestedType("IController");
				if (nestedType != null && nestedType.IsInterface) {
					return nestedType;
				}
			}

			return null;
		}
	}
}