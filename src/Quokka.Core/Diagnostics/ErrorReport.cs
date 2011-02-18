using System;
using System.Collections.Specialized;

namespace Quokka.Diagnostics
{
	///<summary>
	///	Used for reporting error conditions that occur during processing of a task.
	///</summary>
	///<remarks>
	///	This is just an idea at the moment. Not currently used.
	///</remarks>
	public class ErrorReport
	{
		public ErrorReport()
		{
			Properties = new NameValueCollection();
		}

		///<summary>
		///	Specifies whether an error condition has been reported or not.
		///</summary>
		public bool HasErrorOccurred { get; private set; }

		///<summary>
		///	Describes the context of the error condition: for example what was the
		///	code attempting to do at the time the error condition occurred.
		///</summary>
		public string Context { get; set; }

		/// <summary>
		/// The reporter of the error condition. This is usally set to the full name of the class
		/// that reports the error.
		/// </summary>
		public string Source { get; set; }

		///<summary>
		///	The <see cref = "Exception" /> object associated with the error condition.
		///</summary>
		public Exception Exception { get; set; }

		///<summary>
		///	Describes the error condition in more detail.
		///</summary>
		///<remarks>
		///	If not supplied, then the detail is set to the message associated with the <see cref = "Exception" />,
		///	if it is available.
		///</remarks>
		public string Detail { get; set; }

		///<summary>
		///	Provides information on how to resolve the error condition. This property is optional.
		///</summary>
		public string Resolution { get; set; }

		///<summary>
		///	A collection of name/value pairs that may help with diagnostics.
		///</summary>
		public NameValueCollection Properties { get; private set; }

		///<summary>
		///	Report an error condition
		///</summary>
		///<param name = "context">
		///	Text describing what the code was doing at the time. For example "Contacting server",
		///	or "Saving preferences to the registry".
		///</param>
		///<param name = "exception">
		///	An <see cref = "Exception" /> object, which describes the error condition in more detail.
		///</param>
		public void ReportError(string context, Exception exception)
		{
			ReportError(context, exception, null);
		}

		///<summary>
		///	Report an error condition
		///</summary>
		///<param name = "context">
		///	Text describing what the code was doing at the time. For example "Contacting server",
		///	or "Saving preferences to the registry".
		///</param>
		///<param name = "exception">
		///	An <see cref = "Exception" /> object, which describes the error condition in more detail.
		///</param>
		///<param name = "detail">
		///	Text which describes the error condition in more detail.
		///</param>
		public void ReportError(string context, Exception exception, string detail)
		{
			Clear();
			HasErrorOccurred = true;
			Context = context;
			Exception = exception;
			Detail = detail;
			if (detail == null && exception != null)
			{
				Detail = exception.Message;
			}
		}

		///<summary>
		///	Report an error condition
		///</summary>
		///<param name = "context">
		///	Text describing what the code was doing at the time. For example "Contacting server",
		///	or "Saving preferences to the registry".
		///</param>
		///<param name = "detail">
		///	Text which describes the error condition in more detail.
		///</param>
		public void ReportError(string context, string detail)
		{
			ReportError(context, null, detail);
		}

		///<summary>
		///	Clears the error condition
		///</summary>
		public void Clear()
		{
			HasErrorOccurred = false;
			Exception = null;
			Context = null;
			Detail = null;
			Properties.Clear();
		}
	}
}