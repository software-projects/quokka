using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Contains all of the information required to initialize a <see cref="UITask"/>
	/// for a given type.
	/// </summary>
	internal class TaskTemplate
	{
		private readonly Type _taskType;
		private readonly List<NodeBuilder> _nodes = new List<NodeBuilder>();

		public TaskTemplate(Type taskType)
		{
			_taskType = taskType;
		}

		public Type TaskType { 
			get { return _taskType;}
		}

		public List<NodeBuilder> Nodes
		{
			get { return _nodes; }
		}
	}
}
