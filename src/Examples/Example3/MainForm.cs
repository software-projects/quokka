using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Quokka.Diagnostics;
using Quokka.Krypton;
using Quokka.UI.Regions;

namespace Example3
{
	public partial class MainForm : KryptonForm
	{
		private readonly IRegion _navigatorRegion;
		private readonly IRegion _modalRegion;

		public MainForm()
		{
			InitializeComponent();
			_navigatorRegion = new KryptonNavigatorRegion(navigator) {Name = RegionNames.NavigatorRegion};
			_modalRegion = new KryptonModalRegion() {Name = RegionNames.ModalRegion, ParentWindow = this};
			navigator.Dock = DockStyle.Fill;
			navigator.Pages.Clear();

			KryptonCommand cmd = new KryptonCommand();
		}

		public MainForm(IRegionManager regionManager) : this()
		{
			Verify.ArgumentNotNull(regionManager, "regionManager");
			regionManager.Regions.Add(_navigatorRegion);
			regionManager.Regions.Add(_modalRegion);
		}

		public IRegion NavigatorRegion
		{
			get { return _navigatorRegion; }
		}
	}
}