namespace Quokka.WinForms.Config
{
	partial class EditConfigView
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
			this.TopPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.HeadingLabel = new System.Windows.Forms.Label();
			this.BottomPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.CancelButton = new System.Windows.Forms.Button();
			this.SaveButton = new System.Windows.Forms.Button();
			this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.NameLabel = new System.Windows.Forms.Label();
			this.TypeLabel = new System.Windows.Forms.Label();
			this.ValueLabel = new System.Windows.Forms.Label();
			this.DescriptionLabel = new System.Windows.Forms.Label();
			this.NameValueLabel = new System.Windows.Forms.Label();
			this.TypeValueLabel = new System.Windows.Forms.Label();
			this.DescriptionTextBox = new System.Windows.Forms.TextBox();
			this.EditValuePanel = new System.Windows.Forms.FlowLayoutPanel();
			this.FillPanel = new System.Windows.Forms.Panel();
			this.TopPanel.SuspendLayout();
			this.BottomPanel.SuspendLayout();
			this.TableLayoutPanel.SuspendLayout();
			this.FillPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// TopPanel
			// 
			this.TopPanel.AutoSize = true;
			this.TopPanel.BackColor = System.Drawing.Color.Transparent;
			this.TopPanel.Controls.Add(this.HeadingLabel);
			this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.TopPanel.Location = new System.Drawing.Point(0, 0);
			this.TopPanel.MinimumSize = new System.Drawing.Size(100, 25);
			this.TopPanel.Name = "TopPanel";
			this.TopPanel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this.TopPanel.Size = new System.Drawing.Size(611, 25);
			this.TopPanel.TabIndex = 0;
			// 
			// HeadingLabel
			// 
			this.HeadingLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.HeadingLabel.AutoSize = true;
			this.HeadingLabel.Location = new System.Drawing.Point(3, 6);
			this.HeadingLabel.Name = "HeadingLabel";
			this.HeadingLabel.Size = new System.Drawing.Size(141, 13);
			this.HeadingLabel.TabIndex = 0;
			this.HeadingLabel.Text = "Edit Configuration Parameter";
			// 
			// BottomPanel
			// 
			this.BottomPanel.AutoSize = true;
			this.BottomPanel.Controls.Add(this.CancelButton);
			this.BottomPanel.Controls.Add(this.SaveButton);
			this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.BottomPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.BottomPanel.Location = new System.Drawing.Point(0, 361);
			this.BottomPanel.MinimumSize = new System.Drawing.Size(100, 25);
			this.BottomPanel.Name = "BottomPanel";
			this.BottomPanel.Padding = new System.Windows.Forms.Padding(6);
			this.BottomPanel.Size = new System.Drawing.Size(611, 41);
			this.BottomPanel.TabIndex = 0;
			// 
			// CancelButton
			// 
			this.CancelButton.Location = new System.Drawing.Point(521, 9);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 23);
			this.CancelButton.TabIndex = 0;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// SaveButton
			// 
			this.SaveButton.Location = new System.Drawing.Point(440, 9);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(75, 23);
			this.SaveButton.TabIndex = 1;
			this.SaveButton.Text = "Save";
			this.SaveButton.UseVisualStyleBackColor = true;
			// 
			// TableLayoutPanel
			// 
			this.TableLayoutPanel.ColumnCount = 2;
			this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TableLayoutPanel.Controls.Add(this.NameLabel, 0, 0);
			this.TableLayoutPanel.Controls.Add(this.TypeLabel, 0, 1);
			this.TableLayoutPanel.Controls.Add(this.ValueLabel, 0, 2);
			this.TableLayoutPanel.Controls.Add(this.DescriptionLabel, 0, 3);
			this.TableLayoutPanel.Controls.Add(this.NameValueLabel, 1, 0);
			this.TableLayoutPanel.Controls.Add(this.TypeValueLabel, 1, 1);
			this.TableLayoutPanel.Controls.Add(this.DescriptionTextBox, 1, 3);
			this.TableLayoutPanel.Controls.Add(this.EditValuePanel, 1, 2);
			this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel.Location = new System.Drawing.Point(12, 12);
			this.TableLayoutPanel.Name = "TableLayoutPanel";
			this.TableLayoutPanel.RowCount = 4;
			this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TableLayoutPanel.Size = new System.Drawing.Size(587, 312);
			this.TableLayoutPanel.TabIndex = 2;
			// 
			// NameLabel
			// 
			this.NameLabel.AutoSize = true;
			this.NameLabel.Location = new System.Drawing.Point(3, 0);
			this.NameLabel.Name = "NameLabel";
			this.NameLabel.Size = new System.Drawing.Size(35, 13);
			this.NameLabel.TabIndex = 0;
			this.NameLabel.Text = "Name";
			// 
			// TypeLabel
			// 
			this.TypeLabel.AutoSize = true;
			this.TypeLabel.Location = new System.Drawing.Point(3, 25);
			this.TypeLabel.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.TypeLabel.Name = "TypeLabel";
			this.TypeLabel.Size = new System.Drawing.Size(31, 13);
			this.TypeLabel.TabIndex = 1;
			this.TypeLabel.Text = "Type";
			// 
			// ValueLabel
			// 
			this.ValueLabel.AutoSize = true;
			this.ValueLabel.Location = new System.Drawing.Point(3, 50);
			this.ValueLabel.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.ValueLabel.Name = "ValueLabel";
			this.ValueLabel.Size = new System.Drawing.Size(34, 13);
			this.ValueLabel.TabIndex = 2;
			this.ValueLabel.Text = "Value";
			// 
			// DescriptionLabel
			// 
			this.DescriptionLabel.AutoSize = true;
			this.DescriptionLabel.Location = new System.Drawing.Point(3, 81);
			this.DescriptionLabel.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.DescriptionLabel.Name = "DescriptionLabel";
			this.DescriptionLabel.Size = new System.Drawing.Size(60, 13);
			this.DescriptionLabel.TabIndex = 3;
			this.DescriptionLabel.Text = "Description";
			// 
			// NameValueLabel
			// 
			this.NameValueLabel.AutoSize = true;
			this.NameValueLabel.Location = new System.Drawing.Point(69, 0);
			this.NameValueLabel.Name = "NameValueLabel";
			this.NameValueLabel.Size = new System.Drawing.Size(91, 13);
			this.NameValueLabel.TabIndex = 4;
			this.NameValueLabel.Text = "(Name goes here)";
			// 
			// TypeValueLabel
			// 
			this.TypeValueLabel.AutoSize = true;
			this.TypeValueLabel.Location = new System.Drawing.Point(69, 25);
			this.TypeValueLabel.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.TypeValueLabel.Name = "TypeValueLabel";
			this.TypeValueLabel.Size = new System.Drawing.Size(87, 13);
			this.TypeValueLabel.TabIndex = 5;
			this.TypeValueLabel.Text = "(Type goes here)";
			// 
			// DescriptionTextBox
			// 
			this.DescriptionTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.DescriptionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.DescriptionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DescriptionTextBox.Location = new System.Drawing.Point(69, 81);
			this.DescriptionTextBox.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
			this.DescriptionTextBox.Multiline = true;
			this.DescriptionTextBox.Name = "DescriptionTextBox";
			this.DescriptionTextBox.ReadOnly = true;
			this.DescriptionTextBox.Size = new System.Drawing.Size(515, 228);
			this.DescriptionTextBox.TabIndex = 6;
			this.DescriptionTextBox.Text = "(Description goes here)";
			// 
			// EditValuePanel
			// 
			this.EditValuePanel.AutoSize = true;
			this.EditValuePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.EditValuePanel.Location = new System.Drawing.Point(69, 41);
			this.EditValuePanel.MinimumSize = new System.Drawing.Size(100, 25);
			this.EditValuePanel.Name = "EditValuePanel";
			this.EditValuePanel.Size = new System.Drawing.Size(515, 25);
			this.EditValuePanel.TabIndex = 7;
			// 
			// FillPanel
			// 
			this.FillPanel.Controls.Add(this.TableLayoutPanel);
			this.FillPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FillPanel.Location = new System.Drawing.Point(0, 25);
			this.FillPanel.Name = "FillPanel";
			this.FillPanel.Padding = new System.Windows.Forms.Padding(12);
			this.FillPanel.Size = new System.Drawing.Size(611, 336);
			this.FillPanel.TabIndex = 3;
			// 
			// EditConfigView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.FillPanel);
			this.Controls.Add(this.BottomPanel);
			this.Controls.Add(this.TopPanel);
			this.Name = "EditConfigView";
			this.Size = new System.Drawing.Size(611, 402);
			this.TopPanel.ResumeLayout(false);
			this.TopPanel.PerformLayout();
			this.BottomPanel.ResumeLayout(false);
			this.TableLayoutPanel.ResumeLayout(false);
			this.TableLayoutPanel.PerformLayout();
			this.FillPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Label HeadingLabel;
		public System.Windows.Forms.Button CancelButton;
		public System.Windows.Forms.Button SaveButton;
		public System.Windows.Forms.TextBox DescriptionTextBox;
		public System.Windows.Forms.Label DescriptionLabel;
		public System.Windows.Forms.FlowLayoutPanel EditValuePanel;
		public System.Windows.Forms.Label TypeLabel;
		public System.Windows.Forms.Label ValueLabel;
		public System.Windows.Forms.Label TypeValueLabel;
		public System.Windows.Forms.Label NameValueLabel;
		public System.Windows.Forms.Label NameLabel;
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
		public System.Windows.Forms.Panel FillPanel;
		public System.Windows.Forms.FlowLayoutPanel TopPanel;
		public System.Windows.Forms.FlowLayoutPanel BottomPanel;
	}
}
