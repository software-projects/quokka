using System;

namespace Quokka.UI.Messages
{
	/// <summary>
	/// 	Interface implemented by a view that can display a modal message box.
	/// </summary>
	public interface IUIMessageBoxView
	{
		/// <summary>
		/// 	Raised when the user selects one of the possible answers.
		/// </summary>
		/// <remarks>
		/// 	The answer selected is stored in the <see cref = "UIMessage.SelectedAnswer" /> property 
		/// 	of the message.
		/// </remarks>
		event EventHandler Answered;

		/// <summary>
		/// 	Contains the message to be asked of the user.
		/// </summary>
		UIMessage Message { get; set; }
	}
}