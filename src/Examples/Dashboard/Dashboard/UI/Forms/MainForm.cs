using ComponentFactory.Krypton.Toolkit;
using Quokka.UI.Regions;
using Quokka.UI.Tasks;
using Quokka.WinForms;
using Quokka.WinForms.Regions;

namespace Dashboard.UI.Forms
{
	public partial class MainForm : KryptonForm
	{
		private readonly ViewDeck _viewDeck;

		public MainForm()
		{
			InitializeComponent();
			_viewDeck = new ViewDeck(kryptonPanel);
		}

		public IViewDeck ViewDeck
		{
			get { return _viewDeck; }
		}
	}
}