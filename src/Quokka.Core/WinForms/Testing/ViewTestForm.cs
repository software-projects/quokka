using Quokka.Uip;

namespace Quokka.WinForms.Testing
{
	using System.Windows.Forms;

	public partial class ViewTestForm : Form
	{
		private readonly DisplaySettings _displaySettings;
		private readonly ViewTestManager _viewTestManager;
		private readonly IUipViewManager _viewManager;
		private ViewTestNode _currentNode;

		public ViewTestForm()
		{
			InitializeComponent();
			_displaySettings = new DisplaySettings(this);
			_viewManager = new ViewManager(viewManagerPanel);
			viewManagerPanel.Dock = DockStyle.Fill;
			_viewTestManager = new ViewTestManager(_viewManager);
		}

		private void ShowNode(ViewTestNode node)
		{
			if (node == null) {
				viewNameLabel.Text = string.Empty;
				controllerNameLabel.Text = string.Empty;
				_viewTestManager.Clear();
				_displaySettings.Remove("CurrentNode");
			}
			else {
				viewNameLabel.Text = node.ViewText;
				controllerNameLabel.Text = node.ControllerText;
				_displaySettings.SetString("CurrentNode", node.ToString());
				_viewTestManager.ShowNode(node);
			}
			_currentNode = node;
		}

		private void changeButton_Click(object sender, System.EventArgs e)
		{
			SelectViewTestNodeForm form = new SelectViewTestNodeForm(_viewTestManager);
			if (form.ShowDialog(this) == DialogResult.OK) {
				ShowNode(form.SelectedNode);
			}
		}

		private void ViewTestForm_Load(object sender, System.EventArgs e)
		{
			string nodeName = _displaySettings.GetString("CurrentNode", null);
			foreach (ViewTestNode node in _viewTestManager.ViewTestNodes) {
				if (node.ToString() == nodeName) {
					ShowNode(node);
					return;
				}
			}

			// at this point there is no matching node
			ShowNode(null);
		}

		private void refreshButton_Click(object sender, System.EventArgs e)
		{
			ShowNode(_currentNode);
		}

		private void clearButton_Click(object sender, System.EventArgs e)
		{
			ShowNode(null);
		}
	}
}