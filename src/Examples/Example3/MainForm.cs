using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Quokka.Krypton;
using Quokka.UI.Regions;

namespace Example3
{
	public partial class MainForm : KryptonForm
	{
		private readonly IRegion _mainWorkspace;

		public MainForm()
		{
			InitializeComponent();
			_mainWorkspace = new KryptonNavigatorRegion(navigator);
			navigator.Dock = DockStyle.Fill;
			navigator.Pages.Clear();

			KryptonCommand cmd = new KryptonCommand();
		}

		public IRegion MainWorkspace
		{
			get { return _mainWorkspace; }
		}
	}
}