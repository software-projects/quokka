using System;
using System.ComponentModel;

namespace Quokka.UI.Commands
{
	/// <summary>
	///		Represents a command that can be invoked from a UI control.
	/// </summary>
	/// <remarks>
	///		Acknowledgement to Component Factory Krypton Toolkit for the 
	///		basic idea (http://componentfactory.com/). Implementation differs slightly.
	/// </remarks>
	public interface IUICommand
	{
		/// <summary>
		///		Raised when the command is executed, via the <see cref="PerformExecute"/> method.
		/// </summary>
		event EventHandler Execute;

		/// <summary>
		///		Raised when the value of a property changes.
		/// </summary>
		event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///		Is the user interface control checked. 
		/// </summary>
		/// <remarks>
		///		Not all controls support being checked. If a control does not support being
		///		checked, setting this property has not effect.
		/// </remarks>
		bool Checked { get; set; }

		/// <summary>
		///		Gets or sets whether the user interface control is enabled.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		///		Gets or sets the text associated with the user interface control.
		/// </summary>
		string Text { get; set; }

		/// <summary>
		///		Raises the <see cref="Execute"/> event.
		/// </summary>
		void PerformExecute();
	}
}