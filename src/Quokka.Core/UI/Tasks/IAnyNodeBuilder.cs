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

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Fluent interface for building <see cref = "UINode" /> objects for a <see cref = "UITask" />
	/// </summary>
	public interface IAnyNodeBuilder : INodeBuilder
	{
		/// <summary>
		/// 	Specifies the presenter type for this node.
		/// </summary>
		/// <typeparam name = "TPresenter">
		/// 	Type of the presenter.
		/// </typeparam>
		/// <returns>
		/// 	Returns an <see cref = "IPresenterNodeBuilder{TPresenter}" /> object, which can be used
		/// 	for further defining properties for the presenter, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		/// <remarks>
		/// 	If the presenter derives from <see cref = "Presenter{TView}" />, then the view type is 
		/// 	inferred, and there may be no need to specify the view type with <see cref = "SetView{TView}" />.
		/// </remarks>
		IPresenterNodeBuilder<TPresenter> SetPresenter<TPresenter>();

		/// <summary>
		/// 	Specifies the view type for this node.
		/// </summary>
		/// <typeparam name = "TView">
		/// 	Type of the view.
		/// </typeparam>
		/// <returns>
		/// 	Returns an <see cref = "IViewNodeBuilder{TView}" /> object, which can be used
		/// 	for further defining properties for the view, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		/// <remarks>
		/// 	If the presenter derives from <see cref = "Presenter{TView}" />, then the view type is 
		/// 	inferred, and there may be no need to specify the view type with <see cref = "SetView{TView}" />.
		/// </remarks>
		IViewNodeBuilder<TView> SetView<TView>();

		///<summary>
		///	Specifies the UI task type for this node.
		///</summary>
		///<typeparam name = "TTask">
		///	Type of the UI task, which is a type inherited from <see cref = "UITask" />
		///</typeparam>
		///<returns>
		///	Returns a <see cref = "INestedTaskNodeBuilder{TTask}" /> object, which can be used for further defining
		///	properties of the nested task, and configuring navigations to other nodes within
		///	the <see cref = "UITask" />.
		///</returns>
		INestedTaskNodeBuilder<TTask> SetNestedTask<TTask>() where TTask : UITask;
	}
}