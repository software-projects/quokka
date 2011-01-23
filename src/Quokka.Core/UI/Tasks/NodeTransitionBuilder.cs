using System;
using Quokka.Diagnostics;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Used for building <see cref="NodeTransition"/> objects.
	/// </summary>
	internal class NodeTransitionBuilder
	{
		public NodeTransitionBuilder(Converter<object, INavigateCommand> converter, INodeBuilder nextNode)
		{
			Converter = Verify.ArgumentNotNull(converter, "converter");
			NextNode = nextNode;
		}

		public Converter<object, INavigateCommand> Converter { get; private set; }
		public INodeBuilder NextNode { get; private set; }
	}
}