using System;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Used by <see cref="Presenter{T}"/> classes to navigate between nodes (ie <see cref="UINode"/> objects)
	/// in the <see cref="UITask"/>.
	/// </summary>
	/// <example>
	/// <code>
	/// class MyPresenter : Presenter&lt;IMyView&gt; {
	///		public readonly NavigateCommand Next = new NavigateCommand();
	///		public readonly NavigateCommand Cancel = new NavigateCommand();
	///
	///		//
	///		// ... skip code here
	///		//
	/// 
	///		public void OnNext(object sender, EventArgs e) {
	///			// ... do some processing here
	/// 
	///			// Navigate to the next node, wherever that will be
	///			Next.Navigate();
	///		}
	/// 
	///		public void OnCance(object sender, EventArgs e) {
	///			// Navigate to the cancel node
	///			Cancel.Navigate();
	///		}
	/// }
	/// </code>
	/// </example>
	public sealed class NavigateCommand
	{
		public event EventHandler Navigating;

		public bool CanNavigate
		{
			get { return Navigating != null; }
		}

		public void Navigate()
		{
			if (Navigating != null)
			{
				Navigating(this, EventArgs.Empty);
			}
		}
	}
}