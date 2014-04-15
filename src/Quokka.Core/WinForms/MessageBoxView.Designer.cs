namespace Quokka.WinForms
{
	partial class MessageBoxView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.panel1 = new System.Windows.Forms.Panel();
			this.mainInstructionTextBox = new System.Windows.Forms.TextBox();
			this.contentTextBox = new System.Windows.Forms.TextBox();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.buttonFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.buttonFlowPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.mainInstructionTextBox);
			this.panel1.Controls.Add(this.contentTextBox);
			this.panel1.Controls.Add(this.pictureBox);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(449, 206);
			this.panel1.TabIndex = 1;
			// 
			// mainInstructionTextBox
			// 
			this.mainInstructionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.mainInstructionTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.mainInstructionTextBox.Font = new System.Drawing.Font("Tahoma", 14F);
			this.mainInstructionTextBox.ForeColor = System.Drawing.SystemColors.Highlight;
			this.mainInstructionTextBox.Location = new System.Drawing.Point(78, 18);
			this.mainInstructionTextBox.Name = "mainInstructionTextBox";
			this.mainInstructionTextBox.ReadOnly = true;
			this.mainInstructionTextBox.Size = new System.Drawing.Size(359, 30);
			this.mainInstructionTextBox.TabIndex = 3;
			this.mainInstructionTextBox.TabStop = false;
			this.mainInstructionTextBox.Text = "Main instruction goes here";
			// 
			// contentTextBox
			// 
			this.contentTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.contentTextBox.BackColor = System.Drawing.SystemColors.Window;
			this.contentTextBox.Location = new System.Drawing.Point(78, 61);
			this.contentTextBox.Multiline = true;
			this.contentTextBox.Name = "contentTextBox";
			this.contentTextBox.ReadOnly = true;
			this.contentTextBox.Size = new System.Drawing.Size(359, 130);
			this.contentTextBox.TabIndex = 2;
			this.contentTextBox.TabStop = false;
			this.contentTextBox.Text = "Content goes here";
			// 
			// pictureBox
			// 
			this.pictureBox.Image = global::Quokka.Properties.Resources.Information;
			this.pictureBox.Location = new System.Drawing.Point(12, 15);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(48, 48);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// button3
			// 
			this.button3.AutoSize = true;
			this.button3.Location = new System.Drawing.Point(188, 6);
			this.button3.Margin = new System.Windows.Forms.Padding(6);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 2;
			this.button3.Text = "button3";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.AutoSize = true;
			this.button2.Location = new System.Drawing.Point(275, 6);
			this.button2.Margin = new System.Windows.Forms.Padding(6);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "button2";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.AutoSize = true;
			this.button1.Location = new System.Drawing.Point(362, 6);
			this.button1.Margin = new System.Windows.Forms.Padding(6);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// buttonFlowPanel
			// 
			this.buttonFlowPanel.AutoSize = true;
			this.buttonFlowPanel.BackColor = System.Drawing.SystemColors.Control;
			this.buttonFlowPanel.Controls.Add(this.button1);
			this.buttonFlowPanel.Controls.Add(this.button2);
			this.buttonFlowPanel.Controls.Add(this.button3);
			this.buttonFlowPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.buttonFlowPanel.Location = new System.Drawing.Point(0, 206);
			this.buttonFlowPanel.Margin = new System.Windows.Forms.Padding(6);
			this.buttonFlowPanel.Name = "buttonFlowPanel";
			this.buttonFlowPanel.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
			this.buttonFlowPanel.Size = new System.Drawing.Size(449, 35);
			this.buttonFlowPanel.TabIndex = 0;
			this.buttonFlowPanel.WrapContents = false;
			// 
			// MessageBoxForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(449, 241);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.buttonFlowPanel);
			this.MinimumSize = new System.Drawing.Size(330, 210);
			this.Name = "MessageBoxForm";
			this.Text = "Application Title Goes Here";
			this.Load += new System.EventHandler(this.ViewLoad);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.buttonFlowPanel.ResumeLayout(false);
			this.buttonFlowPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.FlowLayoutPanel buttonFlowPanel;
		private System.Windows.Forms.TextBox contentTextBox;
		private System.Windows.Forms.TextBox mainInstructionTextBox;
	}
}