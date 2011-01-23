namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Fluent interface for building <see cref = "UINode" /> objects for a <see cref = "UITask" />
	/// </summary>
	public interface INodeBuilder
	{
		/// <summary>
		/// 	Specifies the presenter type for this node.
		/// </summary>
		/// <typeparam name = "TPresenter">
		/// 	Type of the presenter.
		/// </typeparam>
		/// <returns>
		/// 	Returns a <see cref = "INodeBuilder{TPresenter}" /> object, which can be used
		/// 	for further defining properties for the presenter, and configuring navigations to other
		/// 	<see cref = "UINode" />s within the <see cref = "UITask" />.
		/// </returns>
		/// <remarks>
		/// 	If the presenter derives from <see cref = "Presenter{TView}" />, then the view type is 
		/// 	inferred, and there may be no need to specify the view type with <see cref = "SetView{TView}" />.
		/// </remarks>
		INodeBuilder<TPresenter> SetPresenter<TPresenter>();

		/// <summary>
		/// 	Specifies the view type for this node.
		/// </summary>
		/// <typeparam name = "TView">
		/// 	Type of the view.
		/// </typeparam>
		/// <returns>
		/// 	Returns a <see cref = "INodeBuilder{TView}" /> object, which can be used
		/// 	for further defining properties for the view, and configuring navigations to other
		/// 	<see cref = "UINode" />s within the <see cref = "UITask" />.
		/// </returns>
		/// <remarks>
		/// 	If the presenter derives from <see cref = "Presenter{TView}" />, then the view type is 
		/// 	inferred, and there may be no need to specify the view type with <see cref = "SetView{TView}" />.
		/// </remarks>
		INodeBuilder<TView> SetView<TView>();

		/// <summary>
		/// 	Specify that the view and presenter should stay open when the <see cref = "UITask" /> transitions
		/// 	to another node.
		/// </summary>
		/// <returns>
		/// 	Returns a <see cref = "INodeBuilder" />, which can be used for as part of a fluent API.
		/// </returns>
		INodeBuilder StayOpen();
	}
}