#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using Quokka.Diagnostics;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Used for building <see cref = "NodeTransition" /> objects.
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

	internal class ConditionalNodeTransitionBuilder
	{
		public ConditionalNodeTransitionBuilder(Converter<object, bool> converter, INodeBuilder nextNode)
		{
			Converter = Verify.ArgumentNotNull(converter, "converter");
			NextNode = nextNode;
		}

		public Converter<object, bool> Converter { get; private set; }
		public INodeBuilder NextNode { get; private set; }
	}
}