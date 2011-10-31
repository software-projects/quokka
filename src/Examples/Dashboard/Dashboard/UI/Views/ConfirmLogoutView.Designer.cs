namespace Dashboard.UI.Views
{
	partial class ConfirmLogoutView
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
			this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.logoutButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.cancelButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
			this.kryptonPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// kryptonPanel1
			// 
			this.kryptonPanel1.Controls.Add(this.cancelButton);
			this.kryptonPanel1.Controls.Add(this.logoutButton);
			this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
			this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel1.MaximumSize = new System.Drawing.Size(400, 280);
			this.kryptonPanel1.MinimumSize = new System.Drawing.Size(400, 280);
			this.kryptonPanel1.Name = "kryptonPanel1";
			this.kryptonPanel1.Size = new System.Drawing.Size(400, 280);
			this.kryptonPanel1.TabIndex = 0;
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.TitleControl;
			this.kryptonLabel1.Location = new System.Drawing.Point(52, 98);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(303, 29);
			this.kryptonLabel1.TabIndex = 0;
			this.kryptonLabel1.Values.Text = "Are you sure you want to logout?";
			// 
			// logoutButton
			// 
			this.logoutButton.Location = new System.Drawing.Point(93, 192);
			this.logoutButton.Name = "logoutButton";
			this.logoutButton.Size = new System.Drawing.Size(90, 25);
			this.logoutButton.TabIndex = 1;
			this.logoutButton.Values.Text = "Logout";
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(220, 192);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(90, 25);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Values.Text = "Cancel";
			// 
			// SimpleModalView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.kryptonPanel1);
			this.Name = "SimpleModalView";
			this.Size = new System.Drawing.Size(400, 280);
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
			this.kryptonPanel1.ResumeLayout(false);
			this.kryptonPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
		private ComponentFactory.Krypton.Toolkit.KryptonButton cancelButton;
		private ComponentFactory.Krypton.Toolkit.KryptonButton logoutButton;
		private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
	}
}
