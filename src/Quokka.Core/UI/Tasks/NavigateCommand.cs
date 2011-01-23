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
	///		public readonly NavigateCommand Next = new NavigateCommand();
	///		public readonly NavigateCommand Cancel = new NavigateCommand();
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
	///		public void OnCance(object sender, EventArgs e) {
	///		// Navigate to the cancel node
	///		Cancel.Navigate();
	///		}
	///		}
	///	</code>
	///</example>
	internal sealed class NavigateCommand : INavigateCommand
	{
		public event EventHandler Navigating;

		internal UINode FromNode { get; set; }
		internal UINode ToNode { get; set; }

		public bool CanNavigate
		{
			get { return Navigating != null; }
		}

		public void Navigate()
		{
			if (Navigating == null)
			{
				// TODO: need a better diagnostic message, which includes the name of the
				// current node, and the name of the navigate command.
				throw new InvalidOperationException("No transition is defined for this navigate command.");
			}

			Navigating(this, EventArgs.Empty);
		}
	}
}