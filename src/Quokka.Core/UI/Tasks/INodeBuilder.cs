using System;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Fluent interface for building <see cref="UINode"/> objects for a <see cref="UITask"/>
	/// </summary>
	public interface INodeBuilder
	{
		/// <summary>
		/// Specifies the presenter type for this node.
		/// </summary>
		/// <typeparam name="TPresenter">
		/// Type of the presenter.
		/// </typeparam>
		/// <returns>
		/// Returns a <see cref="INodeBuilder{TPresenter}"/> object, which can be used
		/// for further defining properties for the presenter, and configuring navigations to other
		/// <see cref="UINode"/>s within the <see cref="UITask"/>.
		/// </returns>
		/// <remarks>
		/// If the presenter derives from <see cref="Presenter{TView}"/>, then the view type is 
		/// inferred, and there may be no need to specify the view type with <see cref="SetView{TView}"/>.
		/// </remarks>
		INodeBuilder<TPresenter> SetPresenter<TPresenter>();

		/// <summary>
		/// Specifies the view type for this node.
		/// </summary>
		/// <typeparam name="TView">
		/// Type of the view.
		/// </typeparam>
		/// <returns>
		/// Returns a <see cref="INodeBuilder{TView}"/> object, which can be used
		/// for further defining properties for the view, and configuring navigations to other
		/// <see cref="UINode"/>s within the <see cref="UITask"/>.
		/// </returns>
		/// <remarks>
		/// If the presenter derives from <see cref="Presenter{TView}"/>, then the view type is 
		/// inferred, and there may be no need to specify the view type with <see cref="SetView{TView}"/>.
		/// </remarks>
		INodeBuilder<TView> SetView<TView>();

		/// <summary>
		/// Specify that the view and presenter should stay open when the <see cref="UITask"/> transitions
		/// to another node.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="INodeBuilder"/>, which can be used for as part of a fluent API.
		/// </returns>
		INodeBuilder StayOpen();
	}

	/// <summary>
	/// Fluent interface for building <see cref="UINode"/> objects for a <see cref="UITask"/>. This interface
	/// is returned after <see cref="INodeBuilder.SetPresenter{TPresenter}"/> or <see cref="INodeBuilder.SetView{TView}"/>,
	/// and contain methods specific to the type of presenter or view.
	/// </summary>
	/// <typeparam name="T">
	/// The view or presenter type that has been specified, either with a call to <see cref="INodeBuilder.SetPresenter{TPresenter}"/> 
	/// or <see cref="INodeBuilder.SetView{TView}"/>.
	/// </typeparam>
	public interface INodeBuilder<T> : INodeBuilder
	{
		/// <summary>
		/// Specify a navigation from the presenter.
		/// </summary>
		/// <param name="converter">
		/// Specifies a delegate that converts the presenter into a <see cref="NavigateCommand"/> object. Typically
		/// this will be a closure of the form <c>p => p.MyNavigation</c>, where <c>MyNavigation</c> is a read-only field
		/// (or property) of the presenter and is of type <see cref="NavigateCommand"/>.
		/// </param>
		/// <param name="node">
		/// Specifies a <see cref="UINode"/> to transation to when the <see cref="NavigateCommand"/> is navigated.
		/// </param>
		/// <returns>
		/// Returns a <see cref="INodeBuilder{T}"/> object, which can be used
		/// for further defining properties for the presenter, and configuring navigations to other
		/// <see cref="UINode"/>s within the <see cref="UITask"/>.
		/// </returns>
		INodeBuilder<T> NavigateTo(Converter<T, NavigateCommand> converter, INodeBuilder node);

		/// <summary>
		/// Specify an action to perform on the presenter or view after it is constructed.
		/// </summary>
		/// <param name="action">
		/// An action to perform on the presenter or view after it has been constructed. Typically this will
		/// be used to set a property that will alter the behaviour of the view or presenter.
		/// </param>
		/// <returns>
		/// Returns a <see cref="INodeBuilder{T}"/> object, which can be used
		/// for further defining properties for the presenter, and configuring navigations to other
		/// <see cref="UINode"/>s within the <see cref="UITask"/>.
		/// </returns>
		INodeBuilder<T> With(Action<T> action);
	}
}