using System.Windows.Forms;
using Quokka.UI;
using Quokka.WinForms.Workspaces;

namespace Example2
{
	public partial class MainForm : Form
	{
		private readonly IWorkspace _mainWorkspace;

		public MainForm()
		{
			InitializeComponent();
			tabControl.TabPages.Clear();
			_mainWorkspace = new TabControlWorkspace(tabControl);
		}

		public IWorkspace MainWorkspace
		{
			get { return _mainWorkspace; }
		}
	}
}