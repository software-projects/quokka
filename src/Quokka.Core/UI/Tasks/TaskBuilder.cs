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