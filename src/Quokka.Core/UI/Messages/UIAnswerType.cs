namespace Quokka.UI.Messages
{
	/// <summary>
	/// 	Identifies a set of common answers that are provided in response to UI messages.
	/// </summary>
	/// <remarks>
	/// 	The integer values assigned to these enumerations match the values in the
	/// 	System.Windows.Forms.DialogResult enumeration.
	/// </remarks>
	public enum UIAnswerType
	{
		/// <summary>
		/// 	Custom answer, which indicates the answer is not one of the common
		/// 	answer types.
		/// </summary>
		Custom = 0,
		OK = 1,
		Cancel = 2,
		Abort = 3,
		Retry = 4,
		Ignore = 5,
		Yes = 6,
		No = 7,
	}
}