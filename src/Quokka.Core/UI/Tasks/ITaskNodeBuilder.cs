using System;

namespace Quokka.UI.Tasks
{
	///<summary>
	///	Fluent interface for building <see cref = "UINode" /> objects for a <see cref = "UITask" />.
	///	This interface is used for building nodes that contain a nested <see cref="UITask"/>.
	///</summary>
	///<typeparam name = "TTask">
	///	The type of nested <see cref="UITask"/> that is to be created for this node.
	///</typeparam>
	public interface ITaskNodeBuilder<TTask> : INodeBuilder where TTask : UITask
	{
		/// <summary>
		/// 	Specify a navigation from the nested task.
		/// </summary>
		/// <param name = "converter">
		/// 	Specifies a delegate that converts the <see cref="UITask"/> into a <see cref = "NavigateCommand" /> object. Typically
		/// 	this will be a closure of the form <c>t => t.MyNavigation</c>, where <c>MyNavigation</c> is a read-only field
		/// 	(or property) of the taskand is of type <see cref = "INavigateCommand" />.
		/// </param>
		/// <param name = "node">
		/// 	Specifies a <see cref = "UINode" /> to transation to when the <see cref = "NavigateCommand" /> is navigated.
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "ITaskNodeBuilder{TTask}" /> object, which can be used
		/// 	for further defining properties for the nested task, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		ITaskNodeBuilder<TTask> NavigateTo(Converter<TTask, INavigateCommand> converter, INodeBuilder node);

		/// <summary>
		/// 	Specify a navigation for when the nested task ends.
		/// </summary>
		/// <param name = "node">
		/// 	Specifies a node to transation to when the nested task ends..
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "ITaskNodeBuilder{TTask}" /> object, which can be used
		/// 	for further defining properties for the nested task, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>		
		ITaskNodeBuilder<TTask> NavigateTo(INodeBuilder node);

		/// <summary>
		/// 	Specify an action to perform on the nested task after it is constructed.
		/// </summary>
		/// <param name = "action">
		/// 	An action to perform on the nested task after it has been constructed. Typically this will
		/// 	be used to set a property that will alter the behaviour of the nested task.
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "ITaskNodeBuilder{TTask}" /> object, which can be used
		/// 	for further defining properties for the nested task, and configuring navigations to other
		/// 	nodes within the <see cref = "UITask" />.
		/// </returns>
		ITaskNodeBuilder<TTask> With(Action<TTask> action);
	}
}