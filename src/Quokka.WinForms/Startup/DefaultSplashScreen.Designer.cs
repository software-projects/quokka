namespace Quokka.WinForms.Startup
{
	partial class DefaultSplashScreen
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.versionLabel = new System.Windows.Forms.Label();
			this.buildLabel = new System.Windows.Forms.Label();
			this.copyrightLabel = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.titleLabel = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.versionLabel);
			this.panel1.Controls.Add(this.buildLabel);
			this.panel1.Controls.Add(this.copyrightLabel);
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Controls.Add(this.titleLabel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(371, 162);
			this.panel1.TabIndex = 0;
			// 
			// versionLabel
			// 
			this.versionLabel.Location = new System.Drawing.Point(11, 95);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.Size = new System.Drawing.Size(157, 14);
			this.versionLabel.TabIndex = 12;
			this.versionLabel.Text = "(Version)";
			// 
			// buildLabel
			// 
			this.buildLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buildLabel.Location = new System.Drawing.Point(202, 95);
			this.buildLabel.Name = "buildLabel";
			this.buildLabel.Size = new System.Drawing.Size(155, 14);
			this.buildLabel.TabIndex = 11;
			this.buildLabel.Text = "(Build)";
			this.buildLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// copyrightLabel
			// 
			this.copyrightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.copyrightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.copyrightLabel.Location = new System.Drawing.Point(11, 121);
			this.copyrightLabel.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
			this.copyrightLabel.Name = "copyrightLabel";
			this.copyrightLabel.Size = new System.Drawing.Size(346, 29);
			this.copyrightLabel.TabIndex = 9;
			this.copyrightLabel.Text = "(Copyright)";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.BackColor = System.Drawing.Color.Black;
			this.pictureBox1.Location = new System.Drawing.Point(11, 114);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(346, 2);
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			// 
			// titleLabel
			// 
			this.titleLabel.AutoSize = true;
			this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.titleLabel.ForeColor = System.Drawing.SystemColors.Desktop;
			this.titleLabel.Location = new System.Drawing.Point(11, 11);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(91, 36);
			this.titleLabel.TabIndex = 7;
			this.titleLabel.Text = "(Title)";
			this.titleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// DefaultSplashScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(371, 162);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "DefaultSplashScreen";
			this.Text = "DefaultSplashScreen";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label versionLabel;
		private System.Windows.Forms.Label buildLabel;
		private System.Windows.Forms.Label copyrightLabel;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label titleLabel;
	}
}