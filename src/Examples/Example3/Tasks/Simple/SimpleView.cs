using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Quokka.Diagnostics;

namespace Example3.Tasks.Simple
{
	public partial class SimpleView : KryptonForm
	{
		public SimpleView()
		{
			InitializeComponent();
		}

		public void SetController(SimplePresenter presenter)
		{
			Verify.ArgumentNotNull(presenter, "presenter");
			button.Click += delegate { presenter.CreateNewTask(); };
		}
	}
}