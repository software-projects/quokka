using System.Windows.Forms;
using Quokka.Diagnostics;

namespace Example2.Tasks.Simple
{
	public partial class SimpleView : Form
	{
		public SimpleView()
		{
			InitializeComponent();
		}

		public void SetController(SimplePresenter presenter)
		{
			Verify.ArgumentNotNull(presenter, "presenter");
			button.Click += delegate { presenter.CreateNewTask(); };
			closeButton.Click += delegate { presenter.Close(); };
		}
	}
}