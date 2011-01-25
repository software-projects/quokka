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
	/// <summary>
	/// 	Fluent interface for building <see cref = "UINode" /> objects for a <see cref = "UITask" />
	/// </summary>
	/// <typeparam name = "TPresenter">
	/// 	The type of presenter that is to be created for this node.
	/// </typeparam>
	public interface IPresenterNodeBuilder<TPresenter> : INodeBuilder
	{
		/// <summary>
		/// 	Specify a navigation from the presenter.
		/// </summary>
		/// <param name = "converter">
		/// 	Specifies a delegate that converts the presenter into a <see cref = "NavigateCommand" /> object. Typically
		/// 	this will be a closure of the form <c>p => p.MyNavigation</c>, where <c>MyNavigation</c> is a read-only field
		/// 	(or property) of the presenter and is of type <see cref = "INavigateCommand" />.
		/// </param>
		/// <param name = "node">
		/// 	Specifies a <see cref = "UINode" /> to transation to when the <see cref = "NavigateCommand" /> is navigated.
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "IPresenterNodeBuilder{TPresenter}" /> object, which can be used
		/// 	for further defining properties for the presenter, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		IPresenterNodeBuilder<TPresenter> NavigateTo(Converter<TPresenter, INavigateCommand> converter, INodeBuilder node);

		/// <summary>
		/// 	Specify an action to perform on the presenter after it is constructed.
		/// </summary>
		/// <param name = "action">
		/// 	An action to perform on the presenter after it has been constructed. Typically this will
		/// 	be used to set a property that will alter the behaviour of the presenter.
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "IPresenterNodeBuilder{T}" /> object, which can be used
		/// 	for further defining properties for the presenter, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		IPresenterNodeBuilder<TPresenter> With(Action<TPresenter> action);

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

		/// <summary>
		/// 	Specify that the view and presenter should stay open when the <see cref = "UITask" /> transitions
		/// 	to another node.
		/// </summary>
		/// <returns>
		/// 	Returns an <see cref = "IPresenterNodeBuilder{T}" /> object, which can be used
		/// 	for further defining properties for the presenter, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		IPresenterNodeBuilder<TPresenter> StayOpen();
	}
}