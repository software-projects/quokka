namespace Dashboard.UI.Views
{
	partial class LoginView
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
			this.kryptonManager = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
			this.kryptonPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.loginPanel = new ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup();
			this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.kryptonPanel2 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.passwordTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
			this.loginButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.usernameTextBox = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
			this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
			this.kryptonPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.loginPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.loginPanel.Panel)).BeginInit();
			this.loginPanel.Panel.SuspendLayout();
			this.loginPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
			this.kryptonPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
			this.kryptonPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// kryptonPanel
			// 
			this.kryptonPanel.Controls.Add(this.loginPanel);
			this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel.Name = "kryptonPanel";
			this.kryptonPanel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonPanel.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			this.kryptonPanel.Size = new System.Drawing.Size(527, 367);
			this.kryptonPanel.TabIndex = 0;
			// 
			// loginPanel
			// 
			this.loginPanel.CollapseTarget = ComponentFactory.Krypton.Toolkit.HeaderGroupCollapsedTarget.CollapsedToPrimary;
			this.loginPanel.GroupBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.ControlClient;
			this.loginPanel.GroupBorderStyle = ComponentFactory.Krypton.Toolkit.PaletteBorderStyle.ControlClient;
			this.loginPanel.HeaderStylePrimary = ComponentFactory.Krypton.Toolkit.HeaderStyle.Primary;
			this.loginPanel.HeaderStyleSecondary = ComponentFactory.Krypton.Toolkit.HeaderStyle.Secondary;
			this.loginPanel.HeaderVisibleSecondary = false;
			this.loginPanel.Location = new System.Drawing.Point(123, 92);
			this.loginPanel.Name = "loginPanel";
			this.loginPanel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			// 
			// loginPanel.Panel
			// 
			this.loginPanel.Panel.Controls.Add(this.kryptonPanel1);
			this.loginPanel.Size = new System.Drawing.Size(292, 185);
			this.loginPanel.TabIndex = 0;
			this.loginPanel.Text = "Login";
			this.loginPanel.ValuesPrimary.Description = "";
			this.loginPanel.ValuesPrimary.Heading = "Login";
			this.loginPanel.ValuesPrimary.Image = global::Dashboard.Properties.Resources.IDCard;
			this.loginPanel.ValuesSecondary.Description = "";
			this.loginPanel.ValuesSecondary.Heading = "";
			this.loginPanel.ValuesSecondary.Image = null;
			// 
			// kryptonPanel1
			// 
			this.kryptonPanel1.Controls.Add(this.kryptonPanel2);
			this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel1.Name = "kryptonPanel1";
			this.kryptonPanel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonPanel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			this.kryptonPanel1.Size = new System.Drawing.Size(290, 132);
			this.kryptonPanel1.TabIndex = 0;
			// 
			// kryptonPanel2
			// 
			this.kryptonPanel2.Controls.Add(this.passwordTextBox);
			this.kryptonPanel2.Controls.Add(this.loginButton);
			this.kryptonPanel2.Controls.Add(this.usernameTextBox);
			this.kryptonPanel2.Controls.Add(this.kryptonLabel2);
			this.kryptonPanel2.Controls.Add(this.kryptonLabel1);
			this.kryptonPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonPanel2.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel2.Name = "kryptonPanel2";
			this.kryptonPanel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonPanel2.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelAlternate;
			this.kryptonPanel2.Size = new System.Drawing.Size(290, 132);
			this.kryptonPanel2.TabIndex = 0;
			// 
			// passwordTextBox
			// 
			this.passwordTextBox.InputControlStyle = ComponentFactory.Krypton.Toolkit.InputControlStyle.Standalone;
			this.passwordTextBox.Location = new System.Drawing.Point(109, 53);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.Size = new System.Drawing.Size(120, 22);
			this.passwordTextBox.TabIndex = 3;
			// 
			// loginButton
			// 
			this.loginButton.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.Standalone;
			this.loginButton.Location = new System.Drawing.Point(109, 82);
			this.loginButton.Name = "loginButton";
			this.loginButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.loginButton.Size = new System.Drawing.Size(90, 25);
			this.loginButton.TabIndex = 4;
			this.loginButton.Text = "Login";
			this.loginButton.Values.ExtraText = "";
			this.loginButton.Values.Image = null;
			this.loginButton.Values.ImageStates.ImageCheckedNormal = null;
			this.loginButton.Values.ImageStates.ImageCheckedPressed = null;
			this.loginButton.Values.ImageStates.ImageCheckedTracking = null;
			this.loginButton.Values.Text = "Login";
			// 
			// usernameTextBox
			// 
			this.usernameTextBox.InputControlStyle = ComponentFactory.Krypton.Toolkit.InputControlStyle.Standalone;
			this.usernameTextBox.Location = new System.Drawing.Point(109, 25);
			this.usernameTextBox.Name = "usernameTextBox";
			this.usernameTextBox.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.usernameTextBox.Size = new System.Drawing.Size(120, 22);
			this.usernameTextBox.TabIndex = 1;
			// 
			// kryptonLabel2
			// 
			this.kryptonLabel2.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.kryptonLabel2.Location = new System.Drawing.Point(46, 53);
			this.kryptonLabel2.Name = "kryptonLabel2";
			this.kryptonLabel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonLabel2.Size = new System.Drawing.Size(57, 19);
			this.kryptonLabel2.TabIndex = 2;
			this.kryptonLabel2.Text = "Password";
			this.kryptonLabel2.Values.ExtraText = "";
			this.kryptonLabel2.Values.Image = null;
			this.kryptonLabel2.Values.Text = "Password";
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.kryptonLabel1.Location = new System.Drawing.Point(43, 28);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonLabel1.Size = new System.Drawing.Size(60, 19);
			this.kryptonLabel1.TabIndex = 0;
			this.kryptonLabel1.Text = "Username";
			this.kryptonLabel1.Values.ExtraText = "";
			this.kryptonLabel1.Values.Image = null;
			this.kryptonLabel1.Values.Text = "Username";
			// 
			// errorProvider
			// 
			this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.errorProvider.ContainerControl = this;
			// 
			// LoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(527, 367);
			this.Controls.Add(this.kryptonPanel);
			this.Name = "LoginForm";
			this.Text = "Login";
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
			this.kryptonPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.loginPanel.Panel)).EndInit();
			this.loginPanel.Panel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.loginPanel)).EndInit();
			this.loginPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
			this.kryptonPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
			this.kryptonPanel2.ResumeLayout(false);
			this.kryptonPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;
		private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
		private ComponentFactory.Krypton.Toolkit.KryptonHeaderGroup loginPanel;
		private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
		private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private ComponentFactory.Krypton.Toolkit.KryptonTextBox passwordTextBox;
		private ComponentFactory.Krypton.Toolkit.KryptonTextBox usernameTextBox;
		private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel2;
		private ComponentFactory.Krypton.Toolkit.KryptonButton loginButton;
		private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
		private System.Windows.Forms.ErrorProvider errorProvider;
	}
}