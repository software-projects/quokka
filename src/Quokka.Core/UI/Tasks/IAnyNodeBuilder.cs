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
		///	Returns a <see cref = "ITaskNodeBuilder{TTask}" /> object, which can be used for further defining
		///	properties of the nested task, and configuring navigations to other nodes within
		///	the <see cref = "UITask" />.
		///</returns>
		ITaskNodeBuilder<TTask> SetNestedTask<TTask>() where TTask : UITask;
	}
}