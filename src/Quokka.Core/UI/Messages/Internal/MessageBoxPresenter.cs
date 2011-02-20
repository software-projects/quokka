using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quokka.UI.Tasks;

namespace Quokka.UI.Messages.Internal
{
	internal class MessageBoxPresenter : Presenter<IUIMessageBoxView>
	{
		public INavigateCommand NextCommand { get; set; }
		public UIMessage Message { get; set; }

		protected override void InitializePresenter()
		{
			View.Message = Message;
			View.Answered += AnsweredHandler;
		}

		private void AnsweredHandler(object sender, EventArgs e)
		{
			NextCommand.Navigate();
		}
	}
}
