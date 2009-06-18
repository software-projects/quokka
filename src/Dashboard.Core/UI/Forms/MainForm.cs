using ComponentFactory.Krypton.Toolkit;
using Quokka.UI.Regions;
using Quokka.WinForms.Regions;

namespace Dashboard.UI.Forms
{
	public partial class MainForm : KryptonForm
	{
		private readonly IRegion _mainRegion;

		public MainForm()
		{
			InitializeComponent();
			_mainRegion = new DeckRegion(kryptonPanel);
		}

		public IRegion MainRegion
		{
			get { return _mainRegion; }
		}
	}
}