using System.Windows.Forms;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Views
{
	public partial class DoSomething1View : UserControl
	{
		public INavigateCommand NextCommand { get; set; }

		public DoSomething1View()
		{
			InitializeComponent();
			nextButton.Click += (sender, args) => NextCommand.Navigate();
		}
	}
}