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
using System.Reflection;

using Quokka.Reflection;
using Quokka.Uip.Implementation;

namespace Quokka.Uip
{
    public sealed class UipTaskDefinition
    {
        private readonly string name;
        private readonly IList<Assembly> assemblies;
        private readonly IList<string> namespaces;
        private readonly Type stateType;
        private readonly PropertyCollection stateProperties;
        private readonly IList<UipNode> nodes;
        private UipNode startNode;

        internal UipTaskDefinition(TaskConfig taskConfig, IEnumerable<Assembly> assemblies) {
            Assert.ArgumentNotNull(taskConfig, "taskConfig");

            this.name = taskConfig.Name;
            this.assemblies = new List<Assembly>(assemblies).AsReadOnly();
            this.namespaces = CreateNamespaces(taskConfig);
            this.stateType = TypeUtil.FindType(taskConfig.State.TypeName, namespaces, assemblies, true);
            this.stateProperties = new PropertyCollection(taskConfig.State.Properties);
            this.nodes = CreateNodes(taskConfig.NavigationGraph.Nodes);
            CreateTransitions(taskConfig);
            this.startNode = FindNode(taskConfig.NavigationGraph.StartNodeName, true);
        }

        public UipTaskDefinition(string name, Type stateType) {
            Assert.ArgumentNotNull(name, "name");
            Assert.ArgumentNotNull(stateType, "stateType");

            this.name = name;
            this.stateType = stateType;
            this.assemblies = new List<Assembly>().AsReadOnly();
            this.namespaces = new List<string>().AsReadOnly();
            this.stateProperties = new PropertyCollection();
            this.nodes = new List<UipNode>();
        }

        private void CreateTransitions(TaskConfig taskConfig) {
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

        public string Name {
            get { return this.name; }
        }

        public IUipViewManager ViewManager {
            get { throw new NotImplementedException(); }
        }

        public UipNode StartNode {
            get {
                if (startNode != null) {
                    return startNode;
                }
                if (nodes.Count > 0) {
                    return nodes[0];
                }
                return null;
            }
            set {
                if (value != null && !nodes.Contains(value)) {
                    throw new QuokkaException("Node is not in the Nodes collection");
                }
                startNode = value; 
            }
        }

        public Type StateType {
            get { return stateType; }
        }

        public PropertyCollection StateProperties {
            get { return stateProperties; }
        }

        public IList<Assembly> Assemblies {
            get { return assemblies; }
        }

        public IList<string> Namespaces {
            get { return namespaces; }
        }

        public IList<UipNode> Nodes {
            get { return nodes; }
        }

        public UipNode AddNode(string name, Type viewType, Type controllerType) {
            Assert.ArgumentNotNull(name, "name");
            // view type can be null
            Assert.ArgumentNotNull(controllerType , "controllerType");

            if (FindNode(name, false) != null) {
                throw new UipException("Duplicate node name: " + name);
            }

            UipNode node = new UipNode(this, name, viewType, controllerType);
            nodes.Add(node);

            return node;
        }

        public UipNode FindNode(string name, bool throwOnError) {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (UipNode node in nodes) {
                if (node.Name == name) {
                    return node;
                }
            }

            if (throwOnError) {
                throw new UipException("Cannot find node: " + name);
            }

            return null;
        }

        private List<string> CreateNamespaces(TaskConfig taskConfig) {
            List<string> list = new List<string>();
            foreach (UsingNamespaceConfig usingNamespaceConfig in taskConfig.UsingNamespaces) {
                if (!list.Contains(usingNamespaceConfig.Namespace)) {
                    list.Add(usingNamespaceConfig.Namespace);
                }
            }
            return list;
        }

        private List<UipNode> CreateNodes(IEnumerable<NodeConfig> nodeConfigs) {
            List<UipNode> list = new List<UipNode>();
            foreach (NodeConfig nodeConfig in nodeConfigs) {
                UipNode node = new UipNode(this, nodeConfig);
                list.Add(node);
            }
            return list;
        }
    }
}
