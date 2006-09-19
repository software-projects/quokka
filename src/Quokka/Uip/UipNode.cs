using System;
using System.Collections.Generic;
using System.Text;

using Quokka.Reflection;
using Quokka.Uip.Implementation;


namespace Quokka.Uip
{
    public class UipNode
    {
        private readonly UipTaskDefinition task;
        private readonly string name;
        private readonly Type viewType;
        private readonly Type controllerType;
        private readonly PropertyCollection viewProperties;
        private readonly PropertyCollection controllerProperties;
        private readonly List<UipTransition> transitions;
        private readonly Type controllerInterface;

        internal UipNode(UipTaskDefinition task, NodeConfig nodeConfig) {
            this.task = task;
            this.name = nodeConfig.Name;
            if (nodeConfig.View != null && !String.IsNullOrEmpty(nodeConfig.View.TypeName)) {
                this.viewType = TypeUtil.FindType(nodeConfig.View.TypeName, task.Namespaces, task.Assemblies, true);
                // look for a nested interface in the view called 'IController'
                Type nestedType = this.viewType.GetNestedType("IController");
                if (nestedType != null && nestedType.IsInterface) {
                    this.controllerInterface = nestedType;
                }
                this.viewProperties = new PropertyCollection(nodeConfig.View.Properties);
            }
            this.controllerType = TypeUtil.FindType(nodeConfig.Controller.TypeName, task.Namespaces, task.Assemblies);
            this.controllerProperties = new PropertyCollection(nodeConfig.Controller.Properties);
            this.transitions = new List<UipTransition>();
        }

        public string Name {
            get { return name; }
        }

        public Type ViewType {
            get { return viewType; }
        }

        public Type ControllerInterface {
            get { return controllerInterface; }
        }

        public PropertyCollection ViewProperties {
            get { return viewProperties; }
        }

        public Type ControllerType {
            get { return controllerType; }
        }

        public PropertyCollection ControllerProperties {
            get { return controllerProperties; }
        }

        public IList<UipTransition> Transitions {
            get { return transitions; }
        }

        public bool GetNextNode(string navigateValue, out UipNode node) {
            foreach (UipTransition transition in transitions) {
                if (transition.NavigateValue == navigateValue) {
                    node = transition.NextNode;
                    return true;
                }
            }
            node = null;
            return false;
        }
    }
}
