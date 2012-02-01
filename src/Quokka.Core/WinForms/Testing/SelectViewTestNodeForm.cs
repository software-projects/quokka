namespace Quokka.WinForms.Testing
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public partial class SelectViewTestNodeForm : Form
	{
		private readonly DisplaySettings _displaySettings;
		private readonly ViewTestManager _viewTestManager;
		private ViewTestNode _selectedNode;
		private BindingSource _bindingSource;

		public SelectViewTestNodeForm()
		{
            _displaySettings = new DisplaySettings("SelectViewTestNodeForm");
            InitializeComponent();
		}

		public SelectViewTestNodeForm(ViewTestManager viewTestManager) : this()
		{
			_viewTestManager = viewTestManager;
		}

		public ViewTestNode SelectedNode
		{
			get { return _selectedNode; }
		}

		private void SetSelectedNodeAndClose()
		{
			if (dataGridView.SelectedRows.Count == 0)
			{
				MessageBox.Show(this, "Please select an item", Application.ProductName);
				return;
			}

			_selectedNode = (ViewTestNode) dataGridView.SelectedRows[0].DataBoundItem;
			DialogResult = DialogResult.OK;
		}

		private void SelectViewTestNodeForm_Load(object sender, EventArgs e)
		{
			CenterToParent();

			int width = _displaySettings.GetInt("Width", Width);
			int height = _displaySettings.GetInt("Height", Height);

			Size = new Size(width, height);

			if (_viewTestManager != null) {
				_bindingSource = new BindingSource();
				_bindingSource.DataSource = _viewTestManager.ViewTestNodes;
				dataGridView.DataSource = _bindingSource;

				int index = _viewTestManager.ViewTestNodes.IndexOf(_viewTestManager.CurrentNode);
				if (index >= 0) {
					dataGridView.Rows[index].Selected = true;
				}
			}
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			SetSelectedNodeAndClose();
		}

		private void SelectViewTestNodeForm_SizeChanged(object sender, EventArgs e)
		{
			_displaySettings.SetInt("Width", Width);
			_displaySettings.SetInt("Height", Height);
		}
	}
}