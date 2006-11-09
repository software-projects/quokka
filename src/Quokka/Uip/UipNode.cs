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
                this.controllerInterface = GetControllerInterfaceFromViewType(this.viewType);
                this.viewProperties = new PropertyCollection(nodeConfig.View.Properties);
            }
            this.controllerType = TypeUtil.FindType(nodeConfig.Controller.TypeName, task.Namespaces, task.Assemblies);
            this.controllerProperties = new PropertyCollection(nodeConfig.Controller.Properties);
            this.transitions = new List<UipTransition>();
        }

        internal UipNode(UipTaskDefinition taskDefinition, string name, Type viewType, Type controllerType) {
            Assert.ArgumentNotNull(taskDefinition, "taskDefinition");
            Assert.ArgumentNotNull(name, "name");
            // view type can be null
            Assert.ArgumentNotNull(controllerType, "controllerType");

            this.task = taskDefinition;
            this.name = name;
            this.viewType = viewType;
            this.controllerInterface = GetControllerInterfaceFromViewType(viewType);
            this.viewProperties = new PropertyCollection();
            this.controllerType = controllerType;
            this.controllerProperties = new PropertyCollection();
            this.transitions = new List<UipTransition>();
        }

        public string Name {
            get { return name; }
        }

        internal UipTaskDefinition TaskDefinition {
            get { return this.task; }
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

        public UipNode NavigateTo(string navigateValue, string nodeName) {
            transitions.Add(new UipTransition(this, navigateValue, nodeName));
            return this;
        }

        public UipNode NavigateTo(string navigateValue, UipNode node) {
            transitions.Add(new UipTransition(this, navigateValue, node));
            return this;
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

        private Type GetControllerInterfaceFromViewType(Type viewType) {
            if (viewType != null) {
                // look for a nested interface in the view called 'IController'
                Type nestedType = this.viewType.GetNestedType("IController");
                if (nestedType != null && nestedType.IsInterface) {
                    return nestedType;
                }
            }

            return null;
        }
    }
}
