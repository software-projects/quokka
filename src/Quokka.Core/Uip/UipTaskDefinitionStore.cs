#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2007 John Jeffery. All rights reserved.
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

	public delegate UipTaskDefinition UipTaskDefinitionCreatorCallback(string name);

	/// <summary>
	/// A store for UIP task definitions, accessible by name.
	/// </summary>
	public class UipTaskDefinitionStore
	{
		private readonly Dictionary<string, UipTaskDefinition> taskDefinitions = 
			new Dictionary<string, UipTaskDefinition>();
		private readonly Dictionary<string, UipTaskDefinitionCreatorCallback> callbacks =
			new Dictionary<string, UipTaskDefinitionCreatorCallback>();
		private readonly List<Assembly> assemblies = new List<Assembly>();

		/// <summary>
		/// A Collection of assemblies that are referenced for finding classes
		/// when a task definition is defined by an XML document.
		/// </summary>
		/// <remarks>
		/// This collection is ignored unless there are task definitions defined
		/// using XML.
		/// </remarks>
		public IList<Assembly> Assemblies
		{
			get { return assemblies; }
		}

		/// <summary>
		/// Index to task definition by name.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <returns>Task definition</returns>
		/// <exception cref="UipTaskDefinitionCreateException">
		/// There is no task definition with the specified name.
		/// </exception>
		public UipTaskDefinition this[string name]
		{
			get { return GetTaskDefinition(name); }
		}

		/// <summary>
		/// Creates all of the task definitions that have been defined, but not created.
		/// </summary>
		/// <remarks>
		/// This method is for unit testing, and is unlikely to be used in a production
		/// environment.
		/// </remarks>
		public void CreateAll()
		{
			foreach (string name in callbacks.Keys) {
				GetTaskDefinition(name);
			}
		}

		public void Add(UipTaskDefinition taskDefinition)
		{
			if (taskDefinition == null)
				throw new ArgumentNullException("taskDefinition");
			CheckForDuplicateTaskName(taskDefinition.Name);
			taskDefinitions.Add(taskDefinition.Name, taskDefinition);
		}

		public  void Clear()
		{
			assemblies.Clear();
			taskDefinitions.Clear();
			callbacks.Clear();
		}

		public UipTaskDefinition CreateTaskDefinitionFromStream(Stream stream)
		{
			TaskConfig taskConfig = TaskConfig.Create(stream);
			CheckForDuplicateTaskName(taskConfig.Name);
			UipTaskDefinition taskDefinition = new UipTaskDefinition(taskConfig, assemblies);
			taskDefinitions.Add(taskConfig.Name, taskDefinition);
			return taskDefinition;
		}

		public UipTaskDefinition CreateTaskDefinition(string name, Type stateType)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (stateType == null)
				throw new ArgumentNullException("stateType");
			CheckForDuplicateTaskName(name);

			UipTaskDefinition taskDefinition = new UipTaskDefinition(name, stateType);
			taskDefinitions.Add(name, taskDefinition);
			return taskDefinition;
		}

		public void DefineTask(string name, UipTaskDefinitionCreatorCallback callback)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (callback == null)
				throw new ArgumentNullException("callback");
			CheckForDuplicateTaskName(name);
			callbacks.Add(name, callback);
		}

		public void DefineTaskFromStream(string name, Stream stream)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (stream == null)
				throw new ArgumentNullException("stream");
			CheckForDuplicateTaskName(name);
			DefineTaskFromStreamHelper helper = new DefineTaskFromStreamHelper(stream, this);
			callbacks.Add(name, helper.Create);
		}

		private class DefineTaskFromStreamHelper
		{
			private readonly Stream _stream;
			private readonly UipTaskDefinitionStore _store;

			public DefineTaskFromStreamHelper(Stream stream, UipTaskDefinitionStore store)
			{
				_stream = stream;
				_store = store;
			}

			public UipTaskDefinition Create(string name)
			{
				TaskConfig taskConfig = TaskConfig.Create(_stream);
				return new UipTaskDefinition(taskConfig, _store.Assemblies);
			}
		}

		private UipTaskDefinition GetTaskDefinition(string name)
		{
			UipTaskDefinition result;

			if (taskDefinitions.TryGetValue(name, out result))
			{
				return result;
			}

			UipTaskDefinitionCreatorCallback callback;

			if (callbacks.TryGetValue(name, out callback))
			{
				try
				{
					callbacks.Remove(name);
					result = callback(name);
					if (result.Name != name)
					{
						throw new UipTaskDefinitionNameMismatchException("Expected task definition name: " + name +
																", actual name: " + result.Name);
					}
					taskDefinitions.Add(name, result);
					return result;
				}
				catch (UipTaskDefinitionNameMismatchException)
				{
					throw;
				}
				catch (Exception ex)
				{
					throw new UipTaskDefinitionCreateException("Failed to create task definiton: " + name, ex);
				}
			}

			throw new UipUnknownTaskException("Unknown task: " + name);			
		}

		private void CheckForDuplicateTaskName(string name)
		{
			bool exists = taskDefinitions.ContainsKey(name)
			              || callbacks.ContainsKey(name);
			if (exists)
				throw new UipTaskAlreadyExistsException("Task " + name + " already exists");
		}
	}
}
