using ComponentFactory.Krypton.Toolkit;

namespace Example3.Tasks.Simple
{
	partial class SimpleView
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
			this.button = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonManager1 = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
			this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
			this.kryptonPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button
			// 
			this.button.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.Standalone;
			this.button.Location = new System.Drawing.Point(87, 113);
			this.button.Name = "button";
			this.button.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.button.Size = new System.Drawing.Size(115, 23);
			this.button.TabIndex = 0;
			this.button.Text = "Create new task";
			this.button.Values.ExtraText = "";
			this.button.Values.Image = null;
			this.button.Values.ImageStates.ImageCheckedNormal = null;
			this.button.Values.ImageStates.ImageCheckedPressed = null;
			this.button.Values.ImageStates.ImageCheckedTracking = null;
			this.button.Values.Text = "Create new task";
			// 
			// kryptonPanel1
			// 
			this.kryptonPanel1.Controls.Add(this.button);
			this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel1.Name = "kryptonPanel1";
			this.kryptonPanel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonPanel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			this.kryptonPanel1.Size = new System.Drawing.Size(305, 236);
			this.kryptonPanel1.TabIndex = 2;
			// 
			// SimpleView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(305, 236);
			this.Controls.Add(this.kryptonPanel1);
			this.Name = "SimpleView";
			this.Text = "Task1View";
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
			this.kryptonPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private KryptonManager kryptonManager1;
		private KryptonButton button;
		private KryptonPanel kryptonPanel1;

	}
}