namespace Quokka.WinForms.Tests
{
	partial class CommandLinkTestForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.commandLink1 = new Quokka.WinForms.CommandLink();
			this.modalDialogCommandLink = new Quokka.WinForms.CommandLink();
			this.shieldCommandLink = new Quokka.WinForms.CommandLink();
			this.defaultCommandLink = new Quokka.WinForms.CommandLink();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.messageLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.flowLayoutPanel1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(167, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Command Link Test";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.Controls.Add(this.commandLink1);
			this.flowLayoutPanel1.Controls.Add(this.modalDialogCommandLink);
			this.flowLayoutPanel1.Controls.Add(this.shieldCommandLink);
			this.flowLayoutPanel1.Controls.Add(this.defaultCommandLink);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 38);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(561, 453);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// commandLink1
			// 
			this.commandLink1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.commandLink1.BackColor = System.Drawing.Color.White;
			this.commandLink1.Description = "This is the description";
			this.commandLink1.Location = new System.Drawing.Point(3, 3);
			this.commandLink1.Name = "commandLink1";
			this.commandLink1.Size = new System.Drawing.Size(420, 72);
			this.commandLink1.TabIndex = 2;
			this.commandLink1.TabStop = false;
			this.commandLink1.Text = "commandLink1";
			this.commandLink1.UseVisualStyleBackColor = true;
			this.commandLink1.Click += new System.EventHandler(this.commandLink1_Click);
			// 
			// modalDialogCommandLink
			// 
			this.modalDialogCommandLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.modalDialogCommandLink.BackColor = System.Drawing.Color.White;
			this.modalDialogCommandLink.Description = "Tests correct operation when showing a modal dialog.";
			this.modalDialogCommandLink.Location = new System.Drawing.Point(3, 81);
			this.modalDialogCommandLink.Name = "modalDialogCommandLink";
			this.modalDialogCommandLink.Size = new System.Drawing.Size(420, 72);
			this.modalDialogCommandLink.TabIndex = 3;
			this.modalDialogCommandLink.TabStop = false;
			this.modalDialogCommandLink.Text = "Show Modal Dialog";
			this.modalDialogCommandLink.UseVisualStyleBackColor = true;
			this.modalDialogCommandLink.Click += new System.EventHandler(this.modalDialogCommandLink_Click);
			// 
			// shieldCommandLink
			// 
			this.shieldCommandLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.shieldCommandLink.BackColor = System.Drawing.Color.White;
			this.shieldCommandLink.Description = "This has a shield instead of an arrow.";
			this.shieldCommandLink.Location = new System.Drawing.Point(3, 159);
			this.shieldCommandLink.Name = "shieldCommandLink";
			this.shieldCommandLink.ShowShield = true;
			this.shieldCommandLink.Size = new System.Drawing.Size(420, 72);
			this.shieldCommandLink.TabIndex = 4;
			this.shieldCommandLink.TabStop = false;
			this.shieldCommandLink.Text = "Security-related Command";
			this.shieldCommandLink.UseVisualStyleBackColor = true;
			// 
			// defaultCommandLink
			// 
			this.defaultCommandLink.BackColor = System.Drawing.SystemColors.Window;
			this.defaultCommandLink.Description = "I\'m the default button!";
			this.defaultCommandLink.Location = new System.Drawing.Point(3, 237);
			this.defaultCommandLink.Name = "defaultCommandLink";
			this.defaultCommandLink.Size = new System.Drawing.Size(420, 72);
			this.defaultCommandLink.TabIndex = 5;
			this.defaultCommandLink.TabStop = false;
			this.defaultCommandLink.Text = "Default Button";
			this.defaultCommandLink.UseVisualStyleBackColor = true;
			this.defaultCommandLink.Click += new System.EventHandler(this.defaultCommandLink_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.messageLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 535);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(585, 22);
			this.statusStrip1.TabIndex = 3;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// messageLabel
			// 
			this.messageLabel.Name = "messageLabel";
			this.messageLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// CommandLinkTestForm
			// 
			this.AcceptButton = this.defaultCommandLink;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(585, 557);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.label1);
			this.Name = "CommandLinkTestForm";
			this.Text = "CommandLinkTestForm";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private CommandLink commandLink1;
		private CommandLink modalDialogCommandLink;
		private CommandLink shieldCommandLink;
		private CommandLink defaultCommandLink;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel messageLabel;
	}
}