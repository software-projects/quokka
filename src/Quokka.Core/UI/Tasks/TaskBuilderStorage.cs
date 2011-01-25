#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
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

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Provides in-memory storage of <see cref = "TaskBuilder" /> objects.
	/// </summary>
	/// <remarks>
	/// 	For each separate sub-type of <see cref = "UITask" />, a single <see cref = "TaskBuilder" /> is
	/// 	created, and re-used each time an instance of that sub-type is initialised.
	/// </remarks>
	internal static class TaskBuilderStorage
	{
		private static readonly Dictionary<Type, TaskBuilder> TaskBuilders = new Dictionary<Type, TaskBuilder>();
		private static readonly object LockObject = new object();

		public static TaskBuilder FindForType(Type taskType)
		{
			TaskBuilder taskBuilder;
			lock (LockObject)
			{
				TaskBuilders.TryGetValue(taskType, out taskBuilder);
			}
			return taskBuilder;
		}

		public static void Add(TaskBuilder taskBuilder)
		{
			lock (LockObject)
			{
				TaskBuilders[taskBuilder.TaskType] = taskBuilder;
			}
		}
	}
}