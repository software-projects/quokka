using Quokka.Diagnostics;
using Quokka.UI.Commands;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// View interface suitable for displaying an error message.
	/// </summary>
	public interface IErrorReportView
	{
		IUICommand AbortCommand { get; }
		IUICommand CancelCommand { get; }
		IUICommand RetryCommand { get; }

		ErrorReport ErrorReport { get; set; }
	}
}
