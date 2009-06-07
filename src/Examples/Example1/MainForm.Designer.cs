namespace Example1
{
	partial class MainForm
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
			this.viewManagerPanel1 = new Quokka.WinForms.ViewManagerPanel();
			this.SuspendLayout();
			// 
			// viewManagerPanel1
			// 
			this.viewManagerPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewManagerPanel1.Location = new System.Drawing.Point(0, 0);
			this.viewManagerPanel1.Name = "viewManagerPanel1";
			this.viewManagerPanel1.Size = new System.Drawing.Size(292, 266);
			this.viewManagerPanel1.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.viewManagerPanel1);
			this.Name = "MainForm";
			this.Text = "Example 1";
			this.ResumeLayout(false);

		}

		#endregion

		private Quokka.WinForms.ViewManagerPanel viewManagerPanel1;
	}
}

