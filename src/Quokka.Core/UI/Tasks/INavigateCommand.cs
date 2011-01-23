using System;

namespace Quokka.UI.Tasks
{
	///<summary>
	///	Used by <see cref = "Presenter{T}" /> classes to navigate between nodes (ie <see cref = "UINode" /> objects)
	///	in the <see cref = "UITask" />.
	///</summary>
	///<example>
	///	<code>
	///		class MyPresenter : Presenter&lt;IMyView&gt; {
	///		public INavigateCommand Next { get; set; }
	///		public INavigateCommand Cancel { get; set; }
	///
	///		//
	///		// ... skip code here
	///		//
	/// 
	///		public void OnNext(object sender, EventArgs e) {
	///		// ... do some processing here
	/// 
	///		// Navigate to the next node, wherever that will be
	///		Next.Navigate();
	///		}
	/// 
	///		public void OnCancel(object sender, EventArgs e) {
	///		// Navigate to the cancel node, if possible
	///		if (Cancel.CanNavigate) {
	///		Cancel.Navigate();
	///		}
	///		}
	///		}
	///	</code>
	///</example>
	public interface INavigateCommand
	{
		/// <summary>
		/// Navigate to the node defined for this navigate command.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// If no transition is defined for this node, then this exception is thrown.
		/// If there is any doubt over whether a transition is defined for this navigate command,
		/// then test the <see cref="CanNavigate"/> property first.
		/// </exception>
		void Navigate();

		/// <summary>
		/// Is a transition defined for this navigate command.
		/// </summary>
		bool CanNavigate { get; }
	}
}