namespace Quokka.WinForms.Testing
{
	partial class SelectViewTestNodeForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.viewTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.controllerTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.commentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.bottomPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.cancelButton);
			this.bottomPanel.Controls.Add(this.okButton);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point(0, 399);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(632, 47);
			this.bottomPanel.TabIndex = 0;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(543, 13);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(462, 13);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(632, 39);
			this.panel1.TabIndex = 1;
			// 
			// dataGridView
			// 
			this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.viewTypeColumn,
            this.controllerTypeColumn,
            this.commentColumn});
			this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView.Location = new System.Drawing.Point(0, 39);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.RowHeadersVisible = false;
			this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView.Size = new System.Drawing.Size(632, 360);
			this.dataGridView.TabIndex = 2;
			this.dataGridView.DoubleClick += new System.EventHandler(this.OKButton_Click);
			// 
			// viewTypeColumn
			// 
			this.viewTypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.viewTypeColumn.DataPropertyName = "ViewText";
			this.viewTypeColumn.Frozen = true;
			this.viewTypeColumn.HeaderText = "View";
			this.viewTypeColumn.Name = "viewTypeColumn";
			this.viewTypeColumn.ReadOnly = true;
			this.viewTypeColumn.Width = 55;
			// 
			// controllerTypeColumn
			// 
			this.controllerTypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.controllerTypeColumn.DataPropertyName = "ControllerText";
			this.controllerTypeColumn.HeaderText = "Controller";
			this.controllerTypeColumn.Name = "controllerTypeColumn";
			this.controllerTypeColumn.ReadOnly = true;
			this.controllerTypeColumn.Width = 76;
			// 
			// commentColumn
			// 
			this.commentColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.commentColumn.DataPropertyName = "Comment";
			this.commentColumn.HeaderText = "Comment";
			this.commentColumn.Name = "commentColumn";
			this.commentColumn.ReadOnly = true;
			// 
			// SelectViewTestNodeForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(632, 446);
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.bottomPanel);
			this.Name = "SelectViewTestNodeForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select View Test";
			this.Load += new System.EventHandler(this.SelectViewTestNodeForm_Load);
			this.SizeChanged += new System.EventHandler(this.SelectViewTestNodeForm_SizeChanged);
			this.bottomPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.DataGridViewTextBoxColumn viewTypeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn controllerTypeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn commentColumn;
	}
}