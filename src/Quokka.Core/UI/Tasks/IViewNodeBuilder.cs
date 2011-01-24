using System;

namespace Quokka.UI.Tasks
{
	///<summary>
	///	Fluent interface for building <see cref = "UINode" /> objects for a <see cref = "UITask" />.
	///	This interface is used for building nodes that contain a view, and may or may not contain a presenter.
	///</summary>
	///<typeparam name = "TView">
	///	The type of view that is to be created for this node.
	///</typeparam>
	public interface IViewNodeBuilder<TView> : INodeBuilder
	{
		/// <summary>
		/// 	Specify a navigation from the view.
		/// </summary>
		/// <param name = "converter">
		/// 	Specifies a delegate that converts the view into a <see cref = "NavigateCommand" /> object. Typically
		/// 	this will be a closure of the form <c>v => v.MyNavigation</c>, where <c>MyNavigation</c> is a read-only field
		/// 	(or property) of the view and is of type <see cref = "INavigateCommand" />.
		/// </param>
		/// <param name = "node">
		/// 	Specifies a <see cref = "UINode" /> to transation to when the <see cref = "NavigateCommand" /> is navigated.
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "IViewNodeBuilder{TView}" /> object, which can be used
		/// 	for further defining properties for the view, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		IViewNodeBuilder<TView> NavigateTo(Converter<TView, INavigateCommand> converter, INodeBuilder node);

		/// <summary>
		/// 	Specify an action to perform on the view after it is constructed.
		/// </summary>
		/// <param name = "action">
		/// 	An action to perform on the view after it has been constructed. Typically this will
		/// 	be used to set a property that will alter the behaviour of the view.
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "IViewNodeBuilder{TView}" /> object, which can be used
		/// 	for further defining properties for the view, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		IViewNodeBuilder<TView> With(Action<TView> action);

		/// <summary>
		/// 	Specifies the presenter type for this node.
		/// </summary>
		/// <typeparam name = "TPresenter">
		/// 	Type of the presenter.
		/// </typeparam>
		/// <returns>
		/// 	Returns an <see cref = "IPresenterNodeBuilder{TView}" /> object, which can be used
		/// 	for further defining properties for the presenter, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		IPresenterNodeBuilder<TPresenter> SetPresenter<TPresenter>();

		/// <summary>
		/// 	Specify that the view and presenter should stay open when the <see cref = "UITask" /> transitions
		/// 	to another node.
		/// </summary>
		/// <returns>
		/// 	Returns an <see cref = "IViewNodeBuilder{T}" /> object, which can be used
		/// 	for further defining properties for the view, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		IViewNodeBuilder<TView> StayOpen();
	}
}