using Quokka.Diagnostics;

namespace Quokka.UI.Tasks
{
	public class ErrorReportPresenter : Presenter<IErrorReportView>
	{
		public INavigateCommand CancelCommand { get; set; }
		public INavigateCommand RetryCommand { get; set; }
		public INavigateCommand AbortCommand { get; set; }
		public ErrorReport ErrorReport { get; private set; }


		public ErrorReportPresenter(ErrorReport errorReport)
		{
			ErrorReport = Verify.ArgumentNotNull(errorReport, "errorReport");
		}

		public override void InitializePresenter()
		{
			View.ErrorReport = ErrorReport;

			View.AbortCommand.Execute += (o, e) => AbortCommand.Navigate();
			View.AbortCommand.Enabled = AbortCommand.CanNavigate;

			View.CancelCommand.Execute += (o, e) => RetryCommand.Navigate();
			View.CancelCommand.Enabled = CancelCommand.CanNavigate;

			View.RetryCommand.Execute += (o, e) => RetryCommand.Navigate();
			View.RetryCommand.Enabled = RetryCommand.CanNavigate;
		}
	}
}