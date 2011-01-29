using System.Windows.Forms;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Views
{
	public partial class DoSomething2View : UserControl
	{
		public INavigateCommand CloseCommand { get; set; }

		public DoSomething2View()
		{
			InitializeComponent();
			nextButton.Click += (sender, args) => CloseCommand.Navigate();
		}
	}
}