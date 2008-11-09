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
using Quokka.Reflection;

namespace Quokka.Uip
{
	[Flags]
	public enum UipNodeOptions
	{
		None = 0,
		ModalView = 1,
		StayOpen = 2,
	}

	public class UipNode
	{
		private string _name;
		private Type _viewType;
		private Type _controllerType;
		private readonly PropertyCollection _viewProperties;
		private readonly PropertyCollection _controllerProperties;
		private readonly List<UipTransition> _transitions;
		private bool _controllerInterfaceValid;
		private Type _controllerInterface;
		private UipNodeOptions _options;

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

		public Type ViewType
		{
			get { return _viewType; }
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
			get
			{
				// A modal view can never stay open, regardless of the setting
				if (IsViewModal)
					return false;
				return (_options & UipNodeOptions.StayOpen) != 0;
			}
		}

		public PropertyCollection ControllerProperties
		{
			get { return _controllerProperties; }
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

		private static Type GetControllerInterfaceFromViewType(Type viewType)
		{
			return TypeUtil.FindNestedInterface(viewType, "IController");
		}
	}
}