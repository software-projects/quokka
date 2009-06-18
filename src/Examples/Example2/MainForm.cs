using System.Windows.Forms;
using Quokka.UI.Regions;
using Quokka.WinForms.Regions;

namespace Example2
{
	public partial class MainForm : Form
	{
		private readonly IRegion _mainRegion;

		public MainForm()
		{
			InitializeComponent();
			tabControl.TabPages.Clear();
			_mainRegion = new TabControlRegion(tabControl);
		}

		public IRegion MainRegion
		{
			get { return _mainRegion; }
		}
	}
}