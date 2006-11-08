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
using System.IO;
using System.Reflection;
using System.Xml;

using Quokka.Uip.Implementation;

namespace Quokka.Uip
{
    /// <summary>
    /// Used for configuring, creating, and monitoring UIP tasks.
    /// </summary>
    public static class UipManager
    {
        private static QuokkaContainer serviceContainer;
        private static readonly List<Assembly> assemblies = new List<Assembly>();
        private static readonly Dictionary<string, UipTaskDefinition> taskDefinitions = new Dictionary<string, UipTaskDefinition>();
        private static readonly IList<UipTask> activeTasks = new List<UipTask>();

        #region Public events

        public static event EventHandler TaskCreated;
        public static event EventHandler TaskStarted;
        public static event EventHandler TaskFinished;

        #endregion

        #region Public properties

        /// <summary>
        /// Service container for services shared between all tasks
        /// </summary>
        /// <remarks>
        /// Add any system-wide services to this container prior to creating any UIP tasks.
        /// </remarks>
        public static QuokkaContainer ServiceContainer {
            get {
                if (serviceContainer == null) {
                    serviceContainer = new QuokkaContainer();
                }
                return serviceContainer;
            }
        }

        #endregion 

        #region Public methods

        /// <summary>
        /// Specify a parent service provider for system-wide services.
        /// </summary>
        /// <remarks>
        /// Call this method prior to accessing the <see cref="ServiceContainer"/> property.
        /// </remarks>
        /// <param name="serviceProvider"></param>
        public static void SetParentProvider(IServiceProvider serviceProvider) {
            if (serviceContainer != null) {
                throw new UipException("Service container has already been defined.");
            }
            serviceContainer = new QuokkaContainer(serviceProvider);
        }

        /// <summary>
        /// Add assemblies that contain UIP classes.
        /// </summary>
        /// <param name="assembly">Assembly containing UIP classes.</param>
        /// <remarks>
        /// Call this method for all relevant assemblies prior to loading task definitions.
        /// </remarks>
        public static void AddAssembly(Assembly assembly) {
            if (!assemblies.Contains(assembly)) {
                assemblies.Add(assembly);
            }
        }

        /// <summary>
        /// Load a task definition from an input stream
        /// </summary>
        /// <param name="stream">Input stream</param>
        public static void LoadTaskDefinition(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException("stream");
            TaskConfig taskConfig = TaskConfig.Create(stream);
            UipTaskDefinition taskDefinition = new UipTaskDefinition(taskConfig, assemblies);
            taskDefinitions[taskConfig.Name] = taskDefinition;
        }

        /// <summary>
        /// Load a task definition from an embedded resource
        /// </summary>
        /// <param name="type">Type used for locating the embedded resource.</param>
        /// <param name="name">Embedded resource name.</param>
        public static void LoadTaskDefinition(Type type, string name) {
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");

            using (Stream stream = type.Assembly.GetManifestResourceStream(type, name)) {
                if (stream == null) {
                    throw new UipException("Cannot load task: " + name);
                }
                LoadTaskDefinition(stream);
            }
        }

        /// <summary>
        /// Load many task definitions from embedded resources
        /// </summary>
        /// <param name="type">Type used for locating the embedded resources.</param>
        /// <param name="names">Embedded resource names.</param>
        public static void LoadTaskDefinitions(Type type, params string[] names) {
            foreach (string name in names) {
                LoadTaskDefinition(type, name);
            }
        }

        /// <summary>
        /// Create a new UIP task.
        /// </summary>
        /// <param name="taskName">Name of the task definition</param>
        /// <param name="viewManager">View manager for controlling view display.</param>
        /// <returns>The <c>UipTask</c> object.</returns>
        public static UipTask CreateTask(string taskName, IUipViewManager viewManager) {
            try {
                UipTaskDefinition taskDefinition = taskDefinitions[taskName];
                UipTask task = new UipTask(taskDefinition, serviceContainer, viewManager);
                task.TaskStarted += new EventHandler(task_TaskStarted);
                task.TaskFinished += new EventHandler(task_TaskFinished);
                if (TaskCreated != null) {
                    TaskCreated(task, EventArgs.Empty);
                }
                return task;
            }
            catch (KeyNotFoundException ex) {
                string message = "Undefined task: " + taskName;
                throw new UipException(message, ex);
            }
        }

        /// <summary>
        /// Clear out everything. Used for testing.
        /// </summary>
        public static void Clear() {
            serviceContainer = null;
            assemblies.Clear();
            taskDefinitions.Clear();
            activeTasks.Clear();
        }

        #endregion

        #region Event handlers

        private static void task_TaskFinished(object sender, EventArgs e) {
            activeTasks.Remove((UipTask)sender);
            if (TaskFinished != null) {
                TaskFinished(sender, e);
            }
        }

        private static void task_TaskStarted(object sender, EventArgs e) {
            activeTasks.Add((UipTask)sender);
            if (TaskStarted != null) {
                TaskStarted(sender, e);
            }
        }

        #endregion
    }
}
