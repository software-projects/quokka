using System;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Fluent interface for building <see cref = "UINode" /> objects for a <see cref = "UITask" />. This interface
	/// 	is returned after <see cref = "INodeBuilder.SetPresenter{TPresenter}" /> or <see cref = "INodeBuilder.SetView{TView}" />,
	/// 	and contain methods specific to the type of presenter or view.
	/// </summary>
	/// <typeparam name = "T">
	/// 	The view or presenter type that has been specified, either with a call to <see cref = "INodeBuilder.SetPresenter{TPresenter}" /> 
	/// 	or <see cref = "INodeBuilder.SetView{TView}" />.
	/// </typeparam>
	public interface INodeBuilder<T> : INodeBuilder
	{
		/// <summary>
		/// 	Specify a navigation from the presenter.
		/// </summary>
		/// <param name = "converter">
		/// 	Specifies a delegate that converts the presenter into a <see cref = "NavigateCommand" /> object. Typically
		/// 	this will be a closure of the form <c>p => p.MyNavigation</c>, where <c>MyNavigation</c> is a read-only field
		/// 	(or property) of the presenter and is of type <see cref = "NavigateCommand" />.
		/// </param>
		/// <param name = "node">
		/// 	Specifies a <see cref = "UINode" /> to transation to when the <see cref = "NavigateCommand" /> is navigated.
		/// </param>
		/// <returns>
		/// 	Returns a <see cref = "INodeBuilder{T}" /> object, which can be used
		/// 	for further defining properties for the presenter, and configuring navigations to other
		/// 	<see cref = "UINode" />s within the <see cref = "UITask" />.
		/// </returns>
		INodeBuilder<T> NavigateTo(Converter<T, INavigateCommand> converter, INodeBuilder node);

		/// <summary>
		/// 	Specify an action to perform on the presenter or view after it is constructed.
		/// </summary>
		/// <param name = "action">
		/// 	An action to perform on the presenter or view after it has been constructed. Typically this will
		/// 	be used to set a property that will alter the behaviour of the view or presenter.
		/// </param>
		/// <returns>
		/// 	Returns a <see cref = "INodeBuilder{T}" /> object, which can be used
		/// 	for further defining properties for the presenter, and configuring navigations to other
		/// 	<see cref = "UINode" />s within the <see cref = "UITask" />.
		/// </returns>
		INodeBuilder<T> With(Action<T> action);
	}
}