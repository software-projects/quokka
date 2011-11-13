namespace Quokka.Server.Internal
{
	partial class LogView
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.timestampColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.levelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.loggerColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.messageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.stopButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.AllowUserToOrderColumns = true;
			this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.timestampColumn,
            this.levelColumn,
            this.loggerColumn,
            this.messageColumn});
			this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView.Location = new System.Drawing.Point(0, 31);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.RowHeadersVisible = false;
			this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView.Size = new System.Drawing.Size(735, 560);
			this.dataGridView.TabIndex = 0;
			// 
			// timestampColumn
			// 
			dataGridViewCellStyle2.Format = "HH:mm:ss";
			dataGridViewCellStyle2.NullValue = null;
			this.timestampColumn.DefaultCellStyle = dataGridViewCellStyle2;
			this.timestampColumn.HeaderText = "Time";
			this.timestampColumn.Name = "timestampColumn";
			this.timestampColumn.ReadOnly = true;
			// 
			// levelColumn
			// 
			this.levelColumn.HeaderText = "Level";
			this.levelColumn.Name = "levelColumn";
			this.levelColumn.ReadOnly = true;
			// 
			// loggerColumn
			// 
			this.loggerColumn.HeaderText = "Logger";
			this.loggerColumn.Name = "loggerColumn";
			this.loggerColumn.ReadOnly = true;
			// 
			// messageColumn
			// 
			this.messageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.messageColumn.HeaderText = "Message";
			this.messageColumn.Name = "messageColumn";
			this.messageColumn.ReadOnly = true;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.stopButton);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(735, 31);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// stopButton
			// 
			this.stopButton.Location = new System.Drawing.Point(3, 3);
			this.stopButton.Name = "stopButton";
			this.stopButton.Size = new System.Drawing.Size(75, 23);
			this.stopButton.TabIndex = 0;
			this.stopButton.Text = "Stop";
			this.stopButton.UseVisualStyleBackColor = true;
			this.stopButton.Click += new System.EventHandler(this.StopButtonClick);
			// 
			// LogView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "LogView";
			this.Size = new System.Drawing.Size(735, 591);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn timestampColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn levelColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn loggerColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn messageColumn;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button stopButton;
	}
}
