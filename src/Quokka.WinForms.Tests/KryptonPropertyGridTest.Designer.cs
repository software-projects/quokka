namespace Quokka.WinForms.Tests
{
	partial class KryptonPropertyGridTest
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
			this.components = new System.ComponentModel.Container();
			this.kryptonPropertyGrid1 = new Quokka.Krypton.KryptonPropertyGrid();
			this.kryptonManager1 = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
			this.SuspendLayout();
			// 
			// kryptonPropertyGrid1
			// 
			this.kryptonPropertyGrid1.CategoryForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
			this.kryptonPropertyGrid1.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
			this.kryptonPropertyGrid1.HelpForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
			this.kryptonPropertyGrid1.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
			this.kryptonPropertyGrid1.Location = new System.Drawing.Point(39, 21);
			this.kryptonPropertyGrid1.Name = "kryptonPropertyGrid1";
			this.kryptonPropertyGrid1.Size = new System.Drawing.Size(199, 218);
			this.kryptonPropertyGrid1.TabIndex = 0;
			// 
			// KryptonPropertyGridTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.kryptonPropertyGrid1);
			this.Name = "KryptonPropertyGridTest";
			this.Text = "KryptonPropertyGridTest";
			this.ResumeLayout(false);

		}

		#endregion

		private Quokka.Krypton.KryptonPropertyGrid kryptonPropertyGrid1;
		private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager1;
	}
}