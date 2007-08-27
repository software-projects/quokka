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
	using System.IO;
	using System.Reflection;
	using Quokka.Uip.Implementation;

	/// <summary>
    /// Used for configuring, creating, and monitoring UIP tasks.
    /// </summary>
    public static class UipManager
    {
		private static UipTaskDefinitionStore store = new UipTaskDefinitionStore();
        private static QuokkaContainer serviceContainer;
        private static readonly IList<UipTask> activeTasks = new List<UipTask>();

        #region Public events

        public static event EventHandler TaskCreated;
        public static event EventHandler TaskStarted;
        public static event EventHandler TaskComplete;

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
			if (!store.Assemblies.Contains(assembly)) {
				store.Assemblies.Add(assembly);
			}
        }

		/// <summary>
		/// Define a UIP task using a callback delegate
		/// </summary>
		/// <param name="name">Name of the task</param>
		/// <param name="callback">Callback delegate that will create the task definition when required.</param>
		public static void DefineTask(string name, UipTaskDefinitionCreatorCallback callback)
		{
			store.DefineTask(name, callback);
		}

        /// <summary>
        /// Load a task definition from an input stream
        /// </summary>
        /// <param name="stream">Input stream</param>
        public static void DefineTask(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException("stream");
            TaskConfig taskConfig = TaskConfig.Create(stream);
            UipTaskDefinition taskDefinition = new UipTaskDefinition(taskConfig, store.Assemblies);
        	store.Add(taskDefinition);
        }

        /// <summary>
        /// Load a task definition from an embedded resource
        /// </summary>
        /// <param name="type">Type used for locating the embedded resource.</param>
        /// <param name="name">Embedded resource name.</param>
        public static void DefineTask(Type type, string name) {
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");

            using (Stream stream = type.Assembly.GetManifestResourceStream(type, name)) {
                if (stream == null) {
                    throw new UipException("Cannot load task: " + name);
                }
                DefineTask(stream);
            }
        }

        /// <summary>
        /// Define many tasks from embedded resources
        /// </summary>
        /// <param name="type">Type used for locating the embedded resources.</param>
        /// <param name="names">Embedded resource names.</param>
        public static void DefineTasks(Type type, params string[] names) {
            foreach (string name in names) {
                DefineTask(type, name);
            }
		}

		#region obsolete methods

		[Obsolete("Renamed to DefineTask")]
		public static void LoadTaskDefinition(Stream stream) {
			DefineTask(stream);
		}

		[Obsolete("Renamed to DefineTask")]
		public static void LoadTaskDefinition(Type type, string name)
		{
			DefineTask(type, name);
		}

		[Obsolete("Renamed to DefineTask")]
		public static void LoadTaskDefinitions(Type type, params string[] names)
		{
			DefineTasks(type, names);
		}

		#endregion

		/// <summary>
        /// Create a new UIP task.
        /// </summary>
        /// <param name="taskName">Name of the task definition</param>
        /// <param name="viewManager">View manager for controlling view display.</param>
        /// <returns>The <c>UipTask</c> object.</returns>
		public static UipTask CreateTask(string taskName, IUipViewManager viewManager)
        {
        	UipTaskDefinition taskDefinition = store[taskName];
        	UipTask task = CreateTask(taskDefinition, viewManager);
        	return task;
        }

		public static UipTaskDefinition CreateTaskDefinition(string name, Type stateType)
		{
			return store.CreateTaskDefinition(name, stateType);
		}

        public static UipTask CreateTask(UipTaskDefinition taskDefinition, IUipViewManager viewManager) {
            UipTask task = new UipTask(taskDefinition, serviceContainer, viewManager);
            task.TaskStarted += new EventHandler(task_TaskStarted);
            task.TaskComplete += new EventHandler(task_TaskComplete);
            if (TaskCreated != null) {
                TaskCreated(task, EventArgs.Empty);
            }
            return task;
        }

        /// <summary>
        /// Clear out everything. Used for testing.
        /// </summary>
        public static void Clear() {
            serviceContainer = null;
        	store.Clear();
            activeTasks.Clear();
        }

        #endregion

        #region Event handlers

        private static void task_TaskComplete(object sender, EventArgs e) {
            activeTasks.Remove((UipTask)sender);
            if (TaskComplete != null) {
                TaskComplete(sender, e);
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
