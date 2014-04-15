#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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

			View.AbortCommand.Execute += (o, e) => {
				ErrorReport.Clear();
				AbortCommand.Navigate();
			};
			View.AbortCommand.Enabled = AbortCommand.CanNavigate;

			View.CancelCommand.Execute += (o, e) => {
				ErrorReport.Clear();
				CancelCommand.Navigate();
			};
			View.CancelCommand.Enabled = CancelCommand.CanNavigate;

			View.RetryCommand.Execute += (o, e) => {
				ErrorReport.Clear();
				RetryCommand.Navigate();
			};
			View.RetryCommand.Enabled = RetryCommand.CanNavigate;
		}
	}
}