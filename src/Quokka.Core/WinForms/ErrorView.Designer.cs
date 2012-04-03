namespace Quokka.WinForms
{
	partial class ErrorView
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
			this.label1 = new System.Windows.Forms.Label();
			this.errorDetailTextBox = new System.Windows.Forms.TextBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.retryButton = new System.Windows.Forms.Button();
			this.abortButton = new System.Windows.Forms.Button();
			this.buttonPanel = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1.SuspendLayout();
			this.buttonPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(10, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(238, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Error Condition Encountered";
			// 
			// errorDetailTextBox
			// 
			this.errorDetailTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.errorDetailTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.errorDetailTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.errorDetailTextBox.Location = new System.Drawing.Point(0, 39);
			this.errorDetailTextBox.Multiline = true;
			this.errorDetailTextBox.Name = "errorDetailTextBox";
			this.errorDetailTextBox.ReadOnly = true;
			this.errorDetailTextBox.Size = new System.Drawing.Size(436, 307);
			this.errorDetailTextBox.TabIndex = 5;
			this.errorDetailTextBox.Text = "Error details go here";
			// 
			// cancelButton
			// 
			this.cancelButton.Enabled = false;
			this.cancelButton.Location = new System.Drawing.Point(335, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(90, 25);
			this.cancelButton.TabIndex = 0;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Visible = false;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
			this.flowLayoutPanel1.Controls.Add(this.cancelButton);
			this.flowLayoutPanel1.Controls.Add(this.retryButton);
			this.flowLayoutPanel1.Controls.Add(this.abortButton);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 8);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(428, 40);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// retryButton
			// 
			this.retryButton.Enabled = false;
			this.retryButton.Location = new System.Drawing.Point(239, 3);
			this.retryButton.Name = "retryButton";
			this.retryButton.Size = new System.Drawing.Size(90, 25);
			this.retryButton.TabIndex = 1;
			this.retryButton.Text = "Retry";
			this.retryButton.Visible = false;
			// 
			// abortButton
			// 
			this.abortButton.Enabled = false;
			this.abortButton.Location = new System.Drawing.Point(143, 3);
			this.abortButton.Name = "abortButton";
			this.abortButton.Size = new System.Drawing.Size(90, 25);
			this.abortButton.TabIndex = 2;
			this.abortButton.Text = "Abort";
			this.abortButton.Visible = false;
			// 
			// buttonPanel
			// 
			this.buttonPanel.Controls.Add(this.flowLayoutPanel1);
			this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonPanel.Location = new System.Drawing.Point(0, 346);
			this.buttonPanel.Name = "buttonPanel";
			this.buttonPanel.Padding = new System.Windows.Forms.Padding(0, 8, 8, 0);
			this.buttonPanel.Size = new System.Drawing.Size(436, 48);
			this.buttonPanel.TabIndex = 4;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(436, 39);
			this.panel1.TabIndex = 6;
			// 
			// ErrorView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.errorDetailTextBox);
			this.Controls.Add(this.buttonPanel);
			this.Controls.Add(this.panel1);
			this.Name = "ErrorView";
			this.Size = new System.Drawing.Size(436, 394);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.buttonPanel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox errorDetailTextBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button retryButton;
		private System.Windows.Forms.Button abortButton;
		private System.Windows.Forms.Panel buttonPanel;
		private System.Windows.Forms.Panel panel1;
	}
}
