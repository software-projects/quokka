namespace Quokka.Krypton
{
	partial class Form1
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
			this.kryptonNavigator1 = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
			this.kryptonPage1 = new ComponentFactory.Krypton.Navigator.KryptonPage();
			this.kryptonPage2 = new ComponentFactory.Krypton.Navigator.KryptonPage();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
			this.kryptonPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).BeginInit();
			this.kryptonNavigator1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).BeginInit();
			this.SuspendLayout();
			// 
			// kryptonPanel
			// 
			this.kryptonPanel.Controls.Add(this.kryptonNavigator1);
			this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel.Name = "kryptonPanel";
			this.kryptonPanel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonPanel.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			this.kryptonPanel.Size = new System.Drawing.Size(292, 266);
			this.kryptonPanel.TabIndex = 0;
			// 
			// kryptonNavigator1
			// 
			// 
			// 
			// 
			this.kryptonNavigator1.Button.CloseButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator1.Button.CloseButton.ExtraText = "";
			this.kryptonNavigator1.Button.CloseButton.Image = null;
			this.kryptonNavigator1.Button.CloseButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator1.Button.CloseButton.Text = "";
			this.kryptonNavigator1.Button.CloseButton.UniqueName = "89A05EDB9758435489A05EDB97584354";
			// 
			// 
			// 
			this.kryptonNavigator1.Button.ContextButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator1.Button.ContextButton.ExtraText = "";
			this.kryptonNavigator1.Button.ContextButton.Image = null;
			this.kryptonNavigator1.Button.ContextButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator1.Button.ContextButton.Text = "";
			this.kryptonNavigator1.Button.ContextButton.UniqueName = "8F0355ED96D5403D8F0355ED96D5403D";
			// 
			// 
			// 
			this.kryptonNavigator1.Button.NextButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator1.Button.NextButton.ExtraText = "";
			this.kryptonNavigator1.Button.NextButton.Image = null;
			this.kryptonNavigator1.Button.NextButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator1.Button.NextButton.Text = "";
			this.kryptonNavigator1.Button.NextButton.UniqueName = "7A8FA798F9604C1F7A8FA798F9604C1F";
			// 
			// 
			// 
			this.kryptonNavigator1.Button.PreviousButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator1.Button.PreviousButton.ExtraText = "";
			this.kryptonNavigator1.Button.PreviousButton.Image = null;
			this.kryptonNavigator1.Button.PreviousButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator1.Button.PreviousButton.Text = "";
			this.kryptonNavigator1.Button.PreviousButton.UniqueName = "09FEF5D366154CBA09FEF5D366154CBA";
			this.kryptonNavigator1.Header.HeaderValuesPrimary.Description = "";
			this.kryptonNavigator1.Header.HeaderValuesPrimary.Heading = "(Empty)";
			this.kryptonNavigator1.Header.HeaderValuesPrimary.Image = null;
			this.kryptonNavigator1.Header.HeaderValuesSecondary.Description = "";
			this.kryptonNavigator1.Header.HeaderValuesSecondary.Heading = " ";
			this.kryptonNavigator1.Header.HeaderValuesSecondary.Image = null;
			this.kryptonNavigator1.Location = new System.Drawing.Point(12, 44);
			this.kryptonNavigator1.Name = "kryptonNavigator1";
			this.kryptonNavigator1.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
            this.kryptonPage1,
            this.kryptonPage2});
			this.kryptonNavigator1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonNavigator1.SelectedIndex = 1;
			this.kryptonNavigator1.Size = new System.Drawing.Size(250, 150);
			this.kryptonNavigator1.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonNavigator1.TabIndex = 0;
			this.kryptonNavigator1.Text = "kryptonNavigator1";
			// 
			// kryptonPage1
			// 
			this.kryptonPage1.LastVisibleSet = true;
			this.kryptonPage1.MinimumSize = new System.Drawing.Size(50, 50);
			this.kryptonPage1.Name = "kryptonPage1";
			this.kryptonPage1.Size = new System.Drawing.Size(248, 124);
			this.kryptonPage1.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonPage1.Text = "kryptonPage1";
			this.kryptonPage1.ToolTipTitle = "Page ToolTip";
			this.kryptonPage1.UniqueName = "13B7EBA1D25A4CED13B7EBA1D25A4CED";
			// 
			// kryptonPage2
			// 
			this.kryptonPage2.LastVisibleSet = true;
			this.kryptonPage2.MinimumSize = new System.Drawing.Size(50, 50);
			this.kryptonPage2.Name = "kryptonPage2";
			this.kryptonPage2.Size = new System.Drawing.Size(248, 124);
			this.kryptonPage2.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonPage2.Text = "kryptonPage2";
			this.kryptonPage2.ToolTipTitle = "Page ToolTip";
			this.kryptonPage2.UniqueName = "0876CE6A88044E380876CE6A88044E38";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.kryptonPanel);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
			this.kryptonPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).EndInit();
			this.kryptonNavigator1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;
		private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
		private ComponentFactory.Krypton.Navigator.KryptonNavigator kryptonNavigator1;
		private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage1;
		private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage2;
	}
}

