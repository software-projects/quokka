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

using Quokka.DynamicCodeGeneration;
using Quokka.Reflection;
using Quokka.Uip.Implementation;

namespace Quokka.Uip
{
    /// <summary>
    /// A task defines a discrete unit of work-flow within an application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An application contains one or more UIP tasks.
    /// </para>
    /// <para>TODO: more text</para>
    /// </remarks>
    public sealed class UipTask
    {
        private readonly QuokkaContainer serviceContainer;
        private readonly UipTaskDefinition taskDefinition;
        private readonly object state;
        private UipNode currentNode;
        private object currentController;
        private object currentView;
        private string navigateValue;
        private bool inNavigateMethod;
        private bool endTaskRequested;
        private bool taskFinished;

        public event EventHandler TaskStarted;
        public event EventHandler TaskFinished;

        #region Construction

        internal UipTask(UipTaskDefinition taskDefinition, IServiceProvider serviceProvider, IUipViewManager viewManager) {
            if (taskDefinition == null)
                throw new ArgumentNullException("navigationGraph");
            if (viewManager == null)
                throw new ArgumentNullException("viewManager");
            this.taskDefinition = taskDefinition;
            this.serviceContainer = new QuokkaContainer(serviceProvider);
            this.serviceContainer.AddService(typeof(IUipViewManager), viewManager);
            this.serviceContainer.AddService(typeof(IUipNavigator), new Navigator(this));
            this.serviceContainer.AddService(typeof(IUipEndTask), new EndTaskImpl(this));
            this.state = ObjectFactory.Create(taskDefinition.StateType, serviceProvider, serviceProvider, this);
            PropertyUtil.SetValues(this.state, taskDefinition.StateProperties);
            this.currentNode = null;
            AddNestedStateInterfaces();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Provides services to controller and view objects created while this task is running.
        /// </summary>
        public IServiceProvider ServiceProvider {
            get { return serviceContainer; }
        }

        /// <summary>
        /// Responsible for displaying view objects within the application.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The UIP framework is independent of the display technology used.
        /// In theory, any user interface technology could be used, including:
        /// </para>
        /// <list type="bullet">
        /// <item><term>Windows Forms</term><description>Windows Forms</description></item>
        /// <item><term>GTK#</term><description>GTK#</description></item>
        /// <item><term>Console text</term><description>Console text</description></item>
        /// </list>
        /// </remarks>
        public IUipViewManager ViewManager {
            get { return (IUipViewManager)ServiceProvider.GetService(typeof(IUipViewManager)); }
        }

        public string Name {
            get { return taskDefinition.Name; }
        }

        /// <summary>
        /// The task state object.
        /// </summary>
        /// <remarks>
        /// Every UIP task has a state object, which is made available to controllers
        /// via their constructor arguments. The state object type is specified in the
        /// UIP task definition.
        /// </remarks>
        public object State {
            get { return state; }
        }

        public object CurrentController {
            get { return currentController; }
        }

        public object CurrentView {
            get { return currentView; }
        }

        public UipNode CurrentNode {
            get { return currentNode; }
        }

        public IList<UipNode> Nodes {
            get { return taskDefinition.Nodes; }
        }

        public bool IsRunning {
            get { return currentNode != null; }
        }

        public bool IsFinished {
            get { return taskFinished; }
        }

        #endregion

        #region Public methods

        public UipNode FindNode(string name, bool throwOnError) {
            return taskDefinition.FindNode(name, throwOnError);
        }

        public void Start() {
            if (IsRunning) {
                throw new UipException("Task is already running");
            }

            ViewManager.BeginTask(this);
            Navigate(null);

            if (TaskStarted != null) {
                TaskStarted(this, EventArgs.Empty);
            }
        }

        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }

        #endregion

        #region Private methods

        private void Navigate(string navigateValue) {
            if (this.taskFinished) {
                throw new UipException("Task has finished");
            }

            this.navigateValue = navigateValue;

            if (!inNavigateMethod) {
                inNavigateMethod = true;
                try {
                    UipNode nextNode = GetNextNode();
                    if (nextNode != null) {
                        ViewManager.BeginTransition();
                        try {
                            if (this.currentView != null) {
                                ViewManager.HideView(this.currentView);
                                ViewManager.RemoveView(this.currentView);
                                DisposeOf(ref this.currentView);
                            }

                            do {
                                // Perform the navigation. Note that this call
                                // can result in a recursive call back into this function,
                                // hence the test using the <c>inNavigateMethod</c> variable.
                                DoNavigate(nextNode);

                                // If there was a recursive call to this function, then this
                                // will return non-null.
                                nextNode = GetNextNode();
                            } while (nextNode != null);

                            if (!this.endTaskRequested) {
                                // Finished navigating to a new node, display the new view.
                                CreateView();

                                if (this.currentView != null) {
                                    ViewManager.AddView(this.currentView, this.currentController);
                                    ViewManager.ShowView(this.currentView);
                                }
                            }
                        }
                        finally {
                            ViewManager.EndTransition();
                        }
                    }

                    if (this.endTaskRequested) {
                        this.currentNode = null;
                        this.currentController = null;
                        this.currentView = null;
                        this.navigateValue = null;
                        this.taskFinished = true;
                        this.ViewManager.EndTask(this);
                        if (TaskFinished != null) {
                            TaskFinished(this, EventArgs.Empty);
                        }
                    }
                }
                finally {
                    inNavigateMethod = false;
                }
            }
        }

        private UipNode GetNextNode() {
            UipNode nextNode = null;

            if (!this.endTaskRequested) {
                if (this.currentNode == null) {
                    // Task is just starting
                    nextNode = taskDefinition.StartNode;
                }
                else if (this.navigateValue != null) {
                    bool transitionDefined = this.currentNode.GetNextNode(navigateValue, out nextNode);

                    // forget about the navigate value -- it might get set again when creating the controller
                    string prevNavigateValue = this.navigateValue; // but remember for the error message
                    this.navigateValue = null;

                    if (!transitionDefined) {
                        string message = String.Format("No transition defined: task={0}, node={1}, navigateValue={2}",
                            this.taskDefinition.Name, this.currentNode.Name, prevNavigateValue);
                        throw new UipException(message);
                    }

                    if (nextNode == null) {
                        this.endTaskRequested = true;
                    }
                }
            }

            return nextNode;
        }

        private void DoNavigate(UipNode nextNode) {
            if (nextNode != null) {
                // dispose of the controller and change the current node
                DisposeOf(ref this.currentView);
                DisposeOf(ref this.currentController);
                UipNode prevNode = this.currentNode;
                this.currentNode = nextNode;

                // signal task started
                if (prevNode == null) {
                    // the task has just started 
                    if (TaskStarted != null) {
                        TaskStarted(this, EventArgs.Empty);
                    }
                }

                // Create the controller for the new current node. Note that it is possible for
                // a controller to request navigation inside its constructor, and if this happens
                // the navigateValue variable will be set, and we will continue through this loop again.
                CreateController();
            }
        }

        private void CreateController() {
            DisposeOf(ref currentController);
            if (this.currentNode == null) {
                return;
            }

            Type controllerType = currentNode.ControllerType;
            if (controllerType == null) {
                return;
            }

            currentController = ObjectFactory.Create(currentNode.ControllerType, ServiceProvider, state, ServiceProvider, this, this.currentNode);
            if (currentController == null) {
                throw new UipException("Failed to create controller");
            }
            UipUtil.SetState(currentController, this.state, false);
            PropertyUtil.SetValues(currentController, currentNode.ControllerProperties);
        }

        private void CreateView() {
            DisposeOf(ref currentView);
            if (this.currentNode == null) {
                throw new UipException("Task is not running, no current node");
            }
            if (this.currentController == null) {
                throw new UipException("No controller defined");
            }

            Type viewType = currentNode.ViewType;
            if (viewType == null) {
                // no view defined, finished
                return;
            }

            // Look for a nested controller interface
            object controllerProxy = null;
            if (this.currentNode.ControllerInterface != null) {
                controllerProxy = ProxyFactory.CreateDuckProxy(this.currentNode.ControllerInterface, this.currentController);
            }

            this.currentView = ObjectFactory.Create(viewType, ServiceProvider, controllerProxy, ServiceProvider, this.currentController, this);
            if (this.currentView == null) {
                throw new UipException("Failed to create view");
            }
            UipUtil.SetState(this.currentView, this.state, false);
            UipUtil.SetController(this.currentView, this.currentController, false);
            PropertyUtil.SetValues(this.currentView, this.currentNode.ViewProperties);
        }

        /// <summary>
        /// Create proxies to the state object for nested interfaces called 'IState'
        /// </summary>
        /// <remarks>
        /// <para>
        /// Iterate through each controller and view class looking for nested interfaces called 'IState'.
        /// If a class contains a nested interface called 'IState', assume that this is
        /// a 'duck proxy' reference to the real state object, and create an entry in the service
        /// container that will return a duck proxy to the state object when a service with the nested
        /// interface is requested.
        /// </para>
        /// </remarks>
        private void AddNestedStateInterfaces() {
            // Get the distinct controller types, as a controller type may be present in more than
            // one node.
            Dictionary<Type, Type> typeDict = new Dictionary<Type,Type>();
            foreach (UipNode node in this.Nodes) {
                Type controllerType = node.ControllerType;
                if (controllerType != null) {
                    typeDict[controllerType] = controllerType;
                }
                Type viewType = node.ViewType;
                if (viewType != null) {
                    typeDict[viewType] = viewType;
                }
            }

            // Look for nested interface types called "IState" and assume that they are 
            // duck typing references to the state object.
            foreach (Type controllerType in typeDict.Keys) {
                Type nestedType = controllerType.GetNestedType("IState");
                if (nestedType != null && nestedType.IsInterface) {
                    // create a duck proxy for the state object and add it to the service provider
                    object stateProxy = ProxyFactory.CreateDuckProxy(nestedType, state);
                    serviceContainer.AddService(nestedType, stateProxy);
                }
            }
        }

        private void EndTask() {
            endTaskRequested = true;
            Navigate(null);
        }

        private static void DisposeOf(ref object obj) {
            if (obj != null) {
                IDisposable disposable = obj as IDisposable;
                if (disposable != null) {
                    // Look for a boolean property called 'IsDisposed', and do not dispose if it has a value of true
                    // This is a bit of a hack to avoid any attempt to dispose of Windows Forms controls twice.
                    object isDisposedObject = PropertyUtil.GetValue(disposable, "IsDisposed", false);
                    if (isDisposedObject is bool) {
                        bool isDisposed = (bool)isDisposedObject;
                        if (!isDisposed) {
                            disposable.Dispose();
                        }
                    }
                }
                obj = null;
            }
        }

        #endregion

        #region Nested class Navigator

        private class Navigator : IUipNavigator
        {
            private readonly UipTask task;

            public Navigator(UipTask task) {
                this.task = task;
            }

            public void Navigate(string navigateValue) {
                if (navigateValue == null)
                    throw new ArgumentNullException("navigateValue");
                task.Navigate(navigateValue);
            }

            public bool CanNavigate(string navigateValue) {
                if (navigateValue == null)
                    throw new ArgumentNullException("navigateValue");
                UipNode nextNode;
                return task.CurrentNode.GetNextNode(navigateValue, out nextNode);
            }
        }

        #endregion

        #region Nested class EndTask

        private class EndTaskImpl : IUipEndTask
        {
            private readonly UipTask task;

            public EndTaskImpl(UipTask task) {
                this.task = task;
            }

            #region IUipEndTask Members

            public void EndTask() {
                task.EndTask();
            }

            #endregion
        }

        #endregion
    }
}
