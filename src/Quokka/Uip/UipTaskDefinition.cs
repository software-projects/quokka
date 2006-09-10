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
        private readonly UipNode startNode;

        internal UipTaskDefinition(TaskConfig taskConfig, IEnumerable<Assembly> assemblies) {
            if (taskConfig == null)
                throw new ArgumentNullException("taskConfig");

            this.name = taskConfig.Name;
            this.assemblies = new List<Assembly>(assemblies).AsReadOnly();
            this.namespaces = CreateNamespaces(taskConfig);
            this.stateType = TypeUtil.FindType(taskConfig.State.TypeName, namespaces, assemblies, true);
            this.stateProperties = new PropertyCollection(taskConfig.State.Properties);
            this.nodes = CreateNodes(taskConfig.NavigationGraph.Nodes);
            CreateTransitions(taskConfig);
            this.startNode = FindNode(taskConfig.NavigationGraph.StartNodeName, true);
        }

        private void CreateTransitions(TaskConfig taskConfig) {
            foreach (NodeConfig nodeConfig in taskConfig.NavigationGraph.Nodes) {
                UipNode node = FindNode(nodeConfig.Name, true);
                foreach (NavigateToConfig transitionConfig in nodeConfig.NavigateTos) {
                    UipNode nextNode = FindNode(transitionConfig.NodeName, true);
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
            get { return startNode; }
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

        public void Start() {
            throw new NotImplementedException();
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

        private void CreateNode(NodeConfig nodeConfig) {
            Type viewType = TypeUtil.FindType(nodeConfig.View.TypeName, namespaces, assemblies);
            Type controllerType = TypeUtil.FindType(nodeConfig.Controller.TypeName, namespaces, assemblies);
        }
    }
}
