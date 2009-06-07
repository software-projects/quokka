using System;

namespace Quokka.Uip
{
	/// <summary>
	/// Options associated with a UI node.
	/// </summary>
	/// <remarks>
	/// These options are flags, in that a UI node can be a combination of these options.
	/// </remarks>
	[Flags]
	public enum UipNodeOptions
	{
		/// <summary>
		/// No options (the default)
		/// </summary>
		None = 0,

		/// <summary>
		/// The view should be displayed modally, ie in a modal dialog box.
		/// </summary>
		ModalView = 1,

		/// <summary>
		/// The view and controller are retained for the lifetime of the UI task. 
		/// If this option is not set, the view and controller are disposed of whenever the
		/// UI task transitions to a different UI node.
		/// </summary>
		StayOpen = 2,
	}
}