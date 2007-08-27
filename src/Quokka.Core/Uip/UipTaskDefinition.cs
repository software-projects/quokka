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
	using System.Reflection;
	using Quokka.Reflection;
	using Quokka.Uip.Implementation;

	public sealed class UipTaskDefinition
	{
		private readonly string _name;
		private readonly IList<Assembly> _assemblies;
		private readonly IList<string> _namespaces;
		private readonly Type _stateType;
		private readonly PropertyCollection _stateProperties;
		private readonly IList<UipNode> _nodes;
		private UipNode _startNode;

		internal UipTaskDefinition(TaskConfig taskConfig, IEnumerable<Assembly> assemblies)
		{
			Assert.ArgumentNotNull(taskConfig, "taskConfig");

			_name = taskConfig.Name;
			_assemblies = new List<Assembly>(assemblies).AsReadOnly();
			_namespaces = CreateNamespaces(taskConfig);
			_stateType = TypeUtil.FindType(taskConfig.State.TypeName, _namespaces, assemblies, true);
			_stateProperties = new PropertyCollection(taskConfig.State.Properties);
			_nodes = CreateNodes(taskConfig.NavigationGraph.Nodes);
			CreateTransitions(taskConfig);
			_startNode = FindNode(taskConfig.NavigationGraph.StartNodeName, true);
		}

		public UipTaskDefinition(string name, Type stateType)
		{
			Assert.ArgumentNotNull(name, "name");
			Assert.ArgumentNotNull(stateType, "stateType");

			_name = name;
			_stateType = stateType;
			_assemblies = new List<Assembly>().AsReadOnly();
			_namespaces = new List<string>().AsReadOnly();
			_stateProperties = new PropertyCollection();
			_nodes = new List<UipNode>();
		}

		private void CreateTransitions(TaskConfig taskConfig)
		{
			foreach (NodeConfig nodeConfig in taskConfig.NavigationGraph.Nodes) {
				UipNode node = FindNode(nodeConfig.Name, true);
				foreach (NavigateToConfig transitionConfig in nodeConfig.NavigateTos) {
					UipNode nextNode = null;
					if (transitionConfig.NodeName != "__end__") {
						nextNode = FindNode(transitionConfig.NodeName, true);
					}
					UipTransition transition = new UipTransition(node, transitionConfig.NavigateValue, nextNode);
					node.Transitions.Add(transition);
				}
			}
		}

		public string Name
		{
			get { return _name; }
		}

		public UipNode StartNode
		{
			get
			{
				if (_startNode != null) {
					return _startNode;
				}
				if (_nodes.Count > 0) {
					return _nodes[0];
				}
				return null;
			}
			set
			{
				if (value != null && !_nodes.Contains(value)) {
					throw new UipException("Node is not in the Nodes collection");
				}
				_startNode = value;
			}
		}

		public Type StateType
		{
			get { return _stateType; }
		}

		public PropertyCollection StateProperties
		{
			get { return _stateProperties; }
		}

		public IList<Assembly> Assemblies
		{
			get { return _assemblies; }
		}

		public IList<string> Namespaces
		{
			get { return _namespaces; }
		}

		public IList<UipNode> Nodes
		{
			get { return _nodes; }
		}

		public UipNode AddNode(string name, Type viewType, Type controllerType, UipNodeOptions options)
		{
			Assert.ArgumentNotNull(name, "name");
			// view type can be null
			Assert.ArgumentNotNull(controllerType, "controllerType");
			CheckForDuplicateName(name);
			UipNode node = new UipNode(this, name, viewType, controllerType, options);
			_nodes.Add(node);
			return node;
		}

		public UipNode AddNode(string name)
		{
			Assert.ArgumentNotNull(name, "name");
			CheckForDuplicateName(name);
			UipNode node = new UipNode(this, name);
			_nodes.Add(node);
			return node;
		}

		public UipNode AddNode(string name, Type viewType, Type controllerType)
		{
			return AddNode(name, viewType, controllerType, UipNodeOptions.None);
		}

		public UipNode FindNode(string name, bool throwOnError)
		{
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			foreach (UipNode node in _nodes) {
				if (node.Name == name) {
					return node;
				}
			}

			if (throwOnError) {
				throw new UipException("Cannot find node: " + name);
			}

			return null;
		}

		private void CheckForDuplicateName(string name)
		{
			if (FindNode(name, false) != null) {
				string message = String.Format("Duplicate node name '{0}' in task '{1}'", name, Name);
				throw new UipDuplicateNodeNameException(message);
			}
		}

		private static List<string> CreateNamespaces(TaskConfig taskConfig)
		{
			List<string> list = new List<string>();
			foreach (UsingNamespaceConfig usingNamespaceConfig in taskConfig.UsingNamespaces) {
				if (!list.Contains(usingNamespaceConfig.Namespace)) {
					list.Add(usingNamespaceConfig.Namespace);
				}
			}
			return list;
		}

		private List<UipNode> CreateNodes(IEnumerable<NodeConfig> nodeConfigs)
		{
			List<UipNode> list = new List<UipNode>();
			foreach (NodeConfig nodeConfig in nodeConfigs) {
				UipNode node = new UipNode(this, nodeConfig);
				list.Add(node);
			}
			return list;
		}
	}
}