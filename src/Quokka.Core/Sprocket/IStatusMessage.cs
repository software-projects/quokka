using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quokka.Sprocket
{
	/// <summary>
	/// Interface implemented by messages that indicate the current status
	/// of something.
	/// </summary>
	public interface IStatusMessage
	{
		/// <summary>
		/// Returns a string that identifies the object that this message
		/// is reporting.
		/// </summary>
		/// <returns>
		/// A string representation of the unique identifier for the object
		/// that this message is reporting on.
		/// </returns>
		string GetStatusId();
	}
}
