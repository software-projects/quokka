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
            using (Stream stream = type.Assembly.GetManifestResourceStream(type, name)) {
                if (stream == null) {
                    throw new ArgumentException("Cannot load task from " + name);
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
            UipTaskDefinition taskDefinition = taskDefinitions[taskName];
            UipTask task = new UipTask(taskDefinition, serviceContainer, viewManager);
            task.TaskStarted += new EventHandler(task_TaskStarted);
            task.TaskFinished += new EventHandler(task_TaskFinished);
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
