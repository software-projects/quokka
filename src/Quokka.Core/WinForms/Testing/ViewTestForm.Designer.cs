namespace Quokka.WinForms.Testing
{
	partial class ViewTestForm
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
			this.currentNodeDetailsPanel = new System.Windows.Forms.Panel();
			this.clearButton = new System.Windows.Forms.Button();
			this.refreshButton = new System.Windows.Forms.Button();
			this.changeButton = new System.Windows.Forms.Button();
			this.viewNameLabel = new System.Windows.Forms.Label();
			this.controllerNameLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.viewManagerPanel = new System.Windows.Forms.Panel();
			this.currentNodeDetailsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// currentNodeDetailsPanel
			// 
			this.currentNodeDetailsPanel.BackColor = System.Drawing.SystemColors.ControlLight;
			this.currentNodeDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.currentNodeDetailsPanel.Controls.Add(this.clearButton);
			this.currentNodeDetailsPanel.Controls.Add(this.refreshButton);
			this.currentNodeDetailsPanel.Controls.Add(this.changeButton);
			this.currentNodeDetailsPanel.Controls.Add(this.viewNameLabel);
			this.currentNodeDetailsPanel.Controls.Add(this.controllerNameLabel);
			this.currentNodeDetailsPanel.Controls.Add(this.label2);
			this.currentNodeDetailsPanel.Controls.Add(this.label1);
			this.currentNodeDetailsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.currentNodeDetailsPanel.Location = new System.Drawing.Point(0, 0);
			this.currentNodeDetailsPanel.Name = "currentNodeDetailsPanel";
			this.currentNodeDetailsPanel.Size = new System.Drawing.Size(573, 62);
			this.currentNodeDetailsPanel.TabIndex = 1;
			// 
			// clearButton
			// 
			this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.clearButton.Location = new System.Drawing.Point(488, 4);
			this.clearButton.Name = "clearButton";
			this.clearButton.Size = new System.Drawing.Size(75, 23);
			this.clearButton.TabIndex = 6;
			this.clearButton.Text = "Clear";
			this.clearButton.UseVisualStyleBackColor = true;
			this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
			// 
			// refreshButton
			// 
			this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.refreshButton.Location = new System.Drawing.Point(407, 4);
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(75, 23);
			this.refreshButton.TabIndex = 5;
			this.refreshButton.Text = "Refresh";
			this.refreshButton.UseVisualStyleBackColor = true;
			this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
			// 
			// changeButton
			// 
			this.changeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.changeButton.Location = new System.Drawing.Point(326, 4);
			this.changeButton.Name = "changeButton";
			this.changeButton.Size = new System.Drawing.Size(75, 23);
			this.changeButton.TabIndex = 4;
			this.changeButton.Text = "Change";
			this.changeButton.UseVisualStyleBackColor = true;
			this.changeButton.Click += new System.EventHandler(this.changeButton_Click);
			// 
			// viewNameLabel
			// 
			this.viewNameLabel.AutoSize = true;
			this.viewNameLabel.Location = new System.Drawing.Point(72, 9);
			this.viewNameLabel.Name = "viewNameLabel";
			this.viewNameLabel.Size = new System.Drawing.Size(89, 13);
			this.viewNameLabel.TabIndex = 3;
			this.viewNameLabel.Text = "(View name here)";
			// 
			// controllerNameLabel
			// 
			this.controllerNameLabel.AutoSize = true;
			this.controllerNameLabel.Location = new System.Drawing.Point(72, 31);
			this.controllerNameLabel.Name = "controllerNameLabel";
			this.controllerNameLabel.Size = new System.Drawing.Size(110, 13);
			this.controllerNameLabel.TabIndex = 2;
			this.controllerNameLabel.Text = "(Controller name here)";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Controller:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "View:";
			// 
			// viewManagerPanel
			// 
			this.viewManagerPanel.Location = new System.Drawing.Point(109, 170);
			this.viewManagerPanel.Name = "viewManagerPanel";
			this.viewManagerPanel.Size = new System.Drawing.Size(273, 174);
			this.viewManagerPanel.TabIndex = 2;
			// 
			// ViewTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(573, 506);
			this.Controls.Add(this.viewManagerPanel);
			this.Controls.Add(this.currentNodeDetailsPanel);
			this.MinimumSize = new System.Drawing.Size(360, 300);
			this.Name = "ViewTestForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "View Test Form";
			this.Load += new System.EventHandler(this.ViewTestForm_Load);
			this.currentNodeDetailsPanel.ResumeLayout(false);
			this.currentNodeDetailsPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel currentNodeDetailsPanel;
		private System.Windows.Forms.Button changeButton;
		private System.Windows.Forms.Label viewNameLabel;
		private System.Windows.Forms.Label controllerNameLabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button refreshButton;
		private System.Windows.Forms.Button clearButton;
		private System.Windows.Forms.Panel viewManagerPanel;
	}
}