namespace Example3
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
			this.components = new System.ComponentModel.Container();
			this.kryptonManager = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
			this.kryptonPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.navigator = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
			this.kryptonPage1 = new ComponentFactory.Krypton.Navigator.KryptonPage();
			this.kryptonPage2 = new ComponentFactory.Krypton.Navigator.KryptonPage();
			this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
			this.kryptonPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.navigator)).BeginInit();
			this.navigator.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
			this.SuspendLayout();
			// 
			// kryptonPanel
			// 
			this.kryptonPanel.Controls.Add(this.navigator);
			this.kryptonPanel.Controls.Add(this.kryptonPanel1);
			this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel.Name = "kryptonPanel";
			this.kryptonPanel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonPanel.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			this.kryptonPanel.Size = new System.Drawing.Size(472, 371);
			this.kryptonPanel.TabIndex = 0;
			// 
			// navigator
			// 
			this.navigator.Bar.TabBorderStyle = ComponentFactory.Krypton.Toolkit.TabBorderStyle.SquareOutsizeMedium;
			// 
			// 
			// 
			this.navigator.Button.CloseButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.navigator.Button.CloseButton.ExtraText = "";
			this.navigator.Button.CloseButton.Image = null;
			this.navigator.Button.CloseButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.navigator.Button.CloseButton.Text = "";
			this.navigator.Button.CloseButton.UniqueName = "EF38C77F9878439FEF38C77F9878439F";
			// 
			// 
			// 
			this.navigator.Button.ContextButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.navigator.Button.ContextButton.ExtraText = "";
			this.navigator.Button.ContextButton.Image = null;
			this.navigator.Button.ContextButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.navigator.Button.ContextButton.Text = "";
			this.navigator.Button.ContextButton.UniqueName = "65B8D6715287482765B8D67152874827";
			// 
			// 
			// 
			this.navigator.Button.NextButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.navigator.Button.NextButton.ExtraText = "";
			this.navigator.Button.NextButton.Image = null;
			this.navigator.Button.NextButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.navigator.Button.NextButton.Text = "";
			this.navigator.Button.NextButton.UniqueName = "8DC75FE6AF5343EE8DC75FE6AF5343EE";
			// 
			// 
			// 
			this.navigator.Button.PreviousButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.navigator.Button.PreviousButton.ExtraText = "";
			this.navigator.Button.PreviousButton.Image = null;
			this.navigator.Button.PreviousButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.navigator.Button.PreviousButton.Text = "";
			this.navigator.Button.PreviousButton.UniqueName = "1ABB8B5D2A1F43DD1ABB8B5D2A1F43DD";
			this.navigator.Dock = System.Windows.Forms.DockStyle.Fill;
			this.navigator.Header.HeaderValuesPrimary.Description = "";
			this.navigator.Header.HeaderValuesPrimary.Heading = "(Empty)";
			this.navigator.Header.HeaderValuesPrimary.Image = null;
			this.navigator.Header.HeaderValuesSecondary.Description = "";
			this.navigator.Header.HeaderValuesSecondary.Heading = " ";
			this.navigator.Header.HeaderValuesSecondary.Image = null;
			this.navigator.Location = new System.Drawing.Point(0, 34);
			this.navigator.Name = "navigator";
			this.navigator.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
            this.kryptonPage1,
            this.kryptonPage2});
			this.navigator.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.navigator.SelectedIndex = 0;
			this.navigator.Size = new System.Drawing.Size(472, 337);
			this.navigator.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.navigator.TabIndex = 0;
			this.navigator.Text = "kryptonNavigator1";
			// 
			// kryptonPage1
			// 
			this.kryptonPage1.LastVisibleSet = true;
			this.kryptonPage1.MinimumSize = new System.Drawing.Size(50, 50);
			this.kryptonPage1.Name = "kryptonPage1";
			this.kryptonPage1.Size = new System.Drawing.Size(470, 311);
			this.kryptonPage1.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonPage1.Text = "kryptonPage1";
			this.kryptonPage1.ToolTipTitle = "Page ToolTip";
			this.kryptonPage1.UniqueName = "CE3ABF4D24B246AACE3ABF4D24B246AA";
			// 
			// kryptonPage2
			// 
			this.kryptonPage2.LastVisibleSet = true;
			this.kryptonPage2.MinimumSize = new System.Drawing.Size(50, 50);
			this.kryptonPage2.Name = "kryptonPage2";
			this.kryptonPage2.Size = new System.Drawing.Size(470, 300);
			this.kryptonPage2.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonPage2.Text = "kryptonPage2";
			this.kryptonPage2.ToolTipTitle = "Page ToolTip";
			this.kryptonPage2.UniqueName = "E2A797563EF8467CE2A797563EF8467C";
			// 
			// kryptonPanel1
			// 
			this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel1.Name = "kryptonPanel1";
			this.kryptonPanel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonPanel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			this.kryptonPanel1.Size = new System.Drawing.Size(472, 34);
			this.kryptonPanel1.TabIndex = 1;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(472, 371);
			this.Controls.Add(this.kryptonPanel);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
			this.kryptonPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.navigator)).EndInit();
			this.navigator.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;
		private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
		private ComponentFactory.Krypton.Navigator.KryptonNavigator navigator;
		private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage1;
		private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage2;
		private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
	}
}

