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

namespace Quokka.UI.Tasks
{
	///<summary>
	///	Fluent interface for building <see cref = "UINode" /> objects for a <see cref = "UITask" />.
	///	This interface is used for building nodes that contain a nested <see cref = "UITask" />.
	///</summary>
	///<typeparam name = "TNestedTask">
	///	The type of nested <see cref = "UITask" /> that is to be created for this node.
	///</typeparam>
	public interface INestedTaskNodeBuilder<TNestedTask> : INodeBuilder where TNestedTask : UITask
	{
		/// <summary>
		/// 	Specify a navigation for when the nested task ends.
		/// </summary>
		/// <param name = "node">
		/// 	Specifies a node to transation to when the nested task ends..
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "INestedTaskNodeBuilder{TNestedTask}" /> object, which can be used
		/// 	for further defining properties for the nested task, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		INestedTaskNodeBuilder<TNestedTask> NavigateTo(INodeBuilder node);

		/// <summary>
		///		Specify a conditional navigation for when the nested task ends.
		/// </summary>
		/// <param name="condition">
		///		Specify the condition under which this navigation should take place.
		///		This will usually be a condition based on the values of one of the nested task's state.
		/// </param>
		/// <param name="node">
		/// 	Specifies a node to transation to when the nested task ends..
		/// </param>
		/// <returns></returns>
		INestedTaskNodeBuilder<TNestedTask> NavigateTo(Converter<TNestedTask, bool> condition, INodeBuilder node);

		/// <summary>
		/// 	Specify an action to perform on the nested task after it is constructed.
		/// </summary>
		/// <param name = "action">
		/// 	An action to perform on the nested task after it has been constructed. Typically this will
		/// 	be used to set a property that will alter the behaviour of the nested task.
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "INestedTaskNodeBuilder{TNestedTask}" /> object, which can be used
		/// 	for further defining properties for the nested task, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		INestedTaskNodeBuilder<TNestedTask> With(Action<TNestedTask> action);
	}
}