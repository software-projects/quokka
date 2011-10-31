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
using System.Text;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Used for initializing <see cref="UITask"/> instances.
	/// </summary>
	internal class TaskBuilder
	{
		private readonly Type _taskType;
		private readonly List<NodeBuilder> _nodes = new List<NodeBuilder>();
		private readonly List<string> _errors = new List<string>();
		private bool _isValidated;

		public TaskBuilder(Type taskType)
		{
			_taskType = taskType;
		}

		public Type TaskType
		{
			get { return _taskType; }
		}

		public IList<NodeBuilder> Nodes
		{
			get { return _nodes.AsReadOnly(); }
		}

		public IList<string> Errors
		{
			get
			{
				Validate();
				return _errors.AsReadOnly();
			}
		}

		public void AssertValid()
		{
			if (IsValid)
			{
				return;
			}

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Task {0} is not valid:", TaskType.FullName);
			sb.AppendLine();
			foreach (var message in Errors)
			{
				sb.AppendLine(message);
			}
			throw new UITaskInvalidException(sb.ToString());
		}

		public NodeBuilder CreateNode(string nodeName)
		{
			var node = new NodeBuilder(this, nodeName);
			_nodes.Add(node);
			return node;
		}

		public bool IsValid
		{
			get
			{
				Validate();
				return _errors.Count == 0;
			}
		}

		public void Validate()
		{
			if (_isValidated)
			{
				return;
			}
			if (Nodes.Count > 0)
			{
				foreach (var node in Nodes)
				{
					node.Validate();
				}
			}
			else
			{
				_errors.Add("At least one UI node must be defined");
			}
			_isValidated = true;
		}

		public void AddError(NodeBuilder node, string reason)
		{
			string errorMessage = node.Name + ": " + reason;
			_errors.Add(errorMessage);
		}
	}
}