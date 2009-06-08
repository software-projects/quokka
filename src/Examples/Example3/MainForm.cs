using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Quokka.Krypton;
using Quokka.UI;

namespace Example3
{
	public partial class MainForm : KryptonForm
	{
		private readonly IWorkspace _mainWorkspace;

		public MainForm()
		{
			InitializeComponent();
			_mainWorkspace = new KryptonNavigatorWorkspace(navigator);
			navigator.Dock = DockStyle.Fill;
			navigator.Pages.Clear();
		}

		public IWorkspace MainWorkspace
		{
			get { return _mainWorkspace; }
		}
	}
}