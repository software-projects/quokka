using System.Windows.Forms;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Views
{
	public partial class ConfirmLogoutView : UserControl
	{
		public INavigateCommand CancelCommand { get; set; }
		public INavigateCommand LogoutCommand { get; set; }

		public ConfirmLogoutView()
		{
			InitializeComponent();
			logoutButton.Click += (o, e) => LogoutCommand.Navigate();
			cancelButton.Click += (o, e) => CancelCommand.Navigate();
		}
	}
}