namespace Dashboard.UI.Forms
{
	partial class ShellForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShellForm));
			this.kryptonManager = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.mainPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.kryptonNavigator1 = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
			this.kryptonPage1 = new ComponentFactory.Krypton.Navigator.KryptonPage();
			this.kryptonPage2 = new ComponentFactory.Krypton.Navigator.KryptonPage();
			this.kryptonNavigator2 = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
			this.kryptonPage3 = new ComponentFactory.Krypton.Navigator.KryptonPage();
			this.kryptonPage4 = new ComponentFactory.Krypton.Navigator.KryptonPage();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logoutButton = new System.Windows.Forms.ToolStripButton();
			this.loginNameLabel = new System.Windows.Forms.ToolStripLabel();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.mainPanel)).BeginInit();
			this.mainPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
			this.kryptonSplitContainer1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
			this.kryptonSplitContainer1.Panel2.SuspendLayout();
			this.kryptonSplitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator2)).BeginInit();
			this.kryptonNavigator2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage4)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip
			// 
			this.statusStrip.Font = new System.Drawing.Font("Segoe UI", 8.25F);
			this.statusStrip.Location = new System.Drawing.Point(0, 639);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
			this.statusStrip.Size = new System.Drawing.Size(868, 22);
			this.statusStrip.TabIndex = 0;
			this.statusStrip.Text = "statusStrip1";
			// 
			// mainPanel
			// 
			this.mainPanel.Controls.Add(this.kryptonSplitContainer1);
			this.mainPanel.Controls.Add(this.toolStrip1);
			this.mainPanel.Controls.Add(this.menuStrip1);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Padding = new System.Windows.Forms.Padding(3);
			this.mainPanel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.mainPanel.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			this.mainPanel.Size = new System.Drawing.Size(868, 639);
			this.mainPanel.TabIndex = 1;
			// 
			// kryptonSplitContainer1
			// 
			this.kryptonSplitContainer1.ContainerBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonSplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.kryptonSplitContainer1.Location = new System.Drawing.Point(3, 52);
			this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			this.kryptonSplitContainer1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			// 
			// kryptonSplitContainer1.Panel1
			// 
			this.kryptonSplitContainer1.Panel1.Controls.Add(this.kryptonNavigator1);
			this.kryptonSplitContainer1.Panel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonSplitContainer1.Panel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			// 
			// kryptonSplitContainer1.Panel2
			// 
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonNavigator2);
			this.kryptonSplitContainer1.Panel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonSplitContainer1.Panel2.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient;
			this.kryptonSplitContainer1.SeparatorStyle = ComponentFactory.Krypton.Toolkit.SeparatorStyle.LowProfile;
			this.kryptonSplitContainer1.Size = new System.Drawing.Size(862, 584);
			this.kryptonSplitContainer1.SplitterDistance = 299;
			this.kryptonSplitContainer1.TabIndex = 0;
			// 
			// kryptonNavigator1
			// 
			this.kryptonNavigator1.Button.ButtonDisplayLogic = ComponentFactory.Krypton.Navigator.ButtonDisplayLogic.None;
			// 
			// 
			// 
			this.kryptonNavigator1.Button.CloseButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator1.Button.CloseButton.ExtraText = "";
			this.kryptonNavigator1.Button.CloseButton.Image = null;
			this.kryptonNavigator1.Button.CloseButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator1.Button.CloseButton.Text = "";
			this.kryptonNavigator1.Button.CloseButton.UniqueName = "860F8673EF4A405B860F8673EF4A405B";
			this.kryptonNavigator1.Button.CloseButtonDisplay = ComponentFactory.Krypton.Navigator.ButtonDisplay.Hide;
			// 
			// 
			// 
			this.kryptonNavigator1.Button.ContextButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator1.Button.ContextButton.ExtraText = "";
			this.kryptonNavigator1.Button.ContextButton.Image = null;
			this.kryptonNavigator1.Button.ContextButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator1.Button.ContextButton.Text = "";
			this.kryptonNavigator1.Button.ContextButton.UniqueName = "40F4E6F4EFF04B5E40F4E6F4EFF04B5E";
			// 
			// 
			// 
			this.kryptonNavigator1.Button.NextButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator1.Button.NextButton.ExtraText = "";
			this.kryptonNavigator1.Button.NextButton.Image = null;
			this.kryptonNavigator1.Button.NextButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator1.Button.NextButton.Text = "";
			this.kryptonNavigator1.Button.NextButton.UniqueName = "FC5256AC96AC4676FC5256AC96AC4676";
			// 
			// 
			// 
			this.kryptonNavigator1.Button.PreviousButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator1.Button.PreviousButton.ExtraText = "";
			this.kryptonNavigator1.Button.PreviousButton.Image = null;
			this.kryptonNavigator1.Button.PreviousButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator1.Button.PreviousButton.Text = "";
			this.kryptonNavigator1.Button.PreviousButton.UniqueName = "C32743DA3DF8430BC32743DA3DF8430B";
			this.kryptonNavigator1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonNavigator1.Header.HeaderValuesPrimary.Description = "";
			this.kryptonNavigator1.Header.HeaderValuesPrimary.Heading = "(Empty)";
			this.kryptonNavigator1.Header.HeaderValuesPrimary.Image = null;
			this.kryptonNavigator1.Header.HeaderValuesSecondary.Description = "";
			this.kryptonNavigator1.Header.HeaderValuesSecondary.Heading = " ";
			this.kryptonNavigator1.Header.HeaderValuesSecondary.Image = null;
			this.kryptonNavigator1.Location = new System.Drawing.Point(0, 0);
			this.kryptonNavigator1.Name = "kryptonNavigator1";
			this.kryptonNavigator1.NavigatorMode = ComponentFactory.Krypton.Navigator.NavigatorMode.OutlookFull;
			this.kryptonNavigator1.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
			                                                                                           	this.kryptonPage1,
			                                                                                           	this.kryptonPage2});
			this.kryptonNavigator1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonNavigator1.SelectedIndex = 1;
			this.kryptonNavigator1.Size = new System.Drawing.Size(299, 584);
			this.kryptonNavigator1.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonNavigator1.TabIndex = 0;
			this.kryptonNavigator1.Text = "kryptonNavigator1";
			// 
			// kryptonPage1
			// 
			this.kryptonPage1.LastVisibleSet = true;
			this.kryptonPage1.MinimumSize = new System.Drawing.Size(50, 50);
			this.kryptonPage1.Name = "kryptonPage1";
			this.kryptonPage1.Size = new System.Drawing.Size(223, 318);
			this.kryptonPage1.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonPage1.Text = "kryptonPage1";
			this.kryptonPage1.ToolTipTitle = "Page ToolTip";
			this.kryptonPage1.UniqueName = "D14AC424367B45EDD14AC424367B45ED";
			// 
			// kryptonPage2
			// 
			this.kryptonPage2.LastVisibleSet = true;
			this.kryptonPage2.MinimumSize = new System.Drawing.Size(50, 50);
			this.kryptonPage2.Name = "kryptonPage2";
			this.kryptonPage2.Size = new System.Drawing.Size(297, 483);
			this.kryptonPage2.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonPage2.Text = "kryptonPage2";
			this.kryptonPage2.ToolTipTitle = "Page ToolTip";
			this.kryptonPage2.UniqueName = "14A8E59A6E074C3E14A8E59A6E074C3E";
			// 
			// kryptonNavigator2
			// 
			// 
			// 
			// 
			this.kryptonNavigator2.Button.CloseButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator2.Button.CloseButton.ExtraText = "";
			this.kryptonNavigator2.Button.CloseButton.Image = null;
			this.kryptonNavigator2.Button.CloseButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator2.Button.CloseButton.Text = "";
			this.kryptonNavigator2.Button.CloseButton.UniqueName = "03B2CC7F7045429A03B2CC7F7045429A";
			// 
			// 
			// 
			this.kryptonNavigator2.Button.ContextButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator2.Button.ContextButton.ExtraText = "";
			this.kryptonNavigator2.Button.ContextButton.Image = null;
			this.kryptonNavigator2.Button.ContextButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator2.Button.ContextButton.Text = "";
			this.kryptonNavigator2.Button.ContextButton.UniqueName = "A88299EFE5244468A88299EFE5244468";
			// 
			// 
			// 
			this.kryptonNavigator2.Button.NextButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator2.Button.NextButton.ExtraText = "";
			this.kryptonNavigator2.Button.NextButton.Image = null;
			this.kryptonNavigator2.Button.NextButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator2.Button.NextButton.Text = "";
			this.kryptonNavigator2.Button.NextButton.UniqueName = "B8547A8176A045E3B8547A8176A045E3";
			// 
			// 
			// 
			this.kryptonNavigator2.Button.PreviousButton.Edge = ComponentFactory.Krypton.Toolkit.PaletteRelativeEdgeAlign.Inherit;
			this.kryptonNavigator2.Button.PreviousButton.ExtraText = "";
			this.kryptonNavigator2.Button.PreviousButton.Image = null;
			this.kryptonNavigator2.Button.PreviousButton.Orientation = ComponentFactory.Krypton.Toolkit.PaletteButtonOrientation.Inherit;
			this.kryptonNavigator2.Button.PreviousButton.Text = "";
			this.kryptonNavigator2.Button.PreviousButton.UniqueName = "D6C140ECBC4D4644D6C140ECBC4D4644";
			this.kryptonNavigator2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonNavigator2.Header.HeaderValuesPrimary.Description = "";
			this.kryptonNavigator2.Header.HeaderValuesPrimary.Heading = "(Empty)";
			this.kryptonNavigator2.Header.HeaderValuesPrimary.Image = null;
			this.kryptonNavigator2.Header.HeaderValuesSecondary.Description = "";
			this.kryptonNavigator2.Header.HeaderValuesSecondary.Heading = " ";
			this.kryptonNavigator2.Header.HeaderValuesSecondary.Image = null;
			this.kryptonNavigator2.Location = new System.Drawing.Point(0, 0);
			this.kryptonNavigator2.Name = "kryptonNavigator2";
			this.kryptonNavigator2.NavigatorMode = ComponentFactory.Krypton.Navigator.NavigatorMode.HeaderGroup;
			this.kryptonNavigator2.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
			                                                                                           	this.kryptonPage3,
			                                                                                           	this.kryptonPage4});
			this.kryptonNavigator2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
			this.kryptonNavigator2.SelectedIndex = 0;
			this.kryptonNavigator2.Size = new System.Drawing.Size(558, 584);
			this.kryptonNavigator2.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonNavigator2.TabIndex = 0;
			this.kryptonNavigator2.Text = "kryptonNavigator2";
			// 
			// kryptonPage3
			// 
			this.kryptonPage3.LastVisibleSet = true;
			this.kryptonPage3.MinimumSize = new System.Drawing.Size(50, 50);
			this.kryptonPage3.Name = "kryptonPage3";
			this.kryptonPage3.Size = new System.Drawing.Size(556, 534);
			this.kryptonPage3.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonPage3.Text = "kryptonPage3";
			this.kryptonPage3.ToolTipTitle = "Page ToolTip";
			this.kryptonPage3.UniqueName = "1621E52C8F9942891621E52C8F994289";
			// 
			// kryptonPage4
			// 
			this.kryptonPage4.LastVisibleSet = true;
			this.kryptonPage4.MinimumSize = new System.Drawing.Size(50, 50);
			this.kryptonPage4.Name = "kryptonPage4";
			this.kryptonPage4.Size = new System.Drawing.Size(100, 100);
			this.kryptonPage4.StateCommon.Metrics.PageButtonSpecPadding = new System.Windows.Forms.Padding(-1);
			this.kryptonPage4.Text = "kryptonPage4";
			this.kryptonPage4.ToolTipTitle = "Page ToolTip";
			this.kryptonPage4.UniqueName = "D7F5E9A8FF074F7AD7F5E9A8FF074F7A";
			// 
			// toolStrip1
			// 
			this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			                                                                        	this.logoutButton,
			                                                                        	this.loginNameLabel});
			this.toolStrip1.Location = new System.Drawing.Point(3, 27);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(862, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			                                                                        	this.fileToolStripMenuItem,
			                                                                        	this.editToolStripMenuItem,
			                                                                        	this.viewToolStripMenuItem,
			                                                                        	this.toolsToolStripMenuItem,
			                                                                        	this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(3, 3);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(862, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			                                                                                           	this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			                                                                                           	this.cutToolStripMenuItem,
			                                                                                           	this.copyToolStripMenuItem,
			                                                                                           	this.pasteToolStripMenuItem,
			                                                                                           	this.toolStripSeparator1,
			                                                                                           	this.selectAllToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			                                                                                           	this.refreshToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.toolsToolStripMenuItem.Text = "&Tools";
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			                                                                                           	this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.cutToolStripMenuItem.Text = "Cu&t";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
			this.selectAllToolStripMenuItem.Text = "Select &All";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.aboutToolStripMenuItem.Text = "&About";
			// 
			// logoutButton
			// 
			this.logoutButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.logoutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.logoutButton.Image = ((System.Drawing.Image)(resources.GetObject("logoutButton.Image")));
			this.logoutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.logoutButton.Name = "logoutButton";
			this.logoutButton.Size = new System.Drawing.Size(23, 22);
			this.logoutButton.Text = "Logout";
			// 
			// loginNameLabel
			// 
			this.loginNameLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.loginNameLabel.Name = "loginNameLabel";
			this.loginNameLabel.Size = new System.Drawing.Size(74, 22);
			this.loginNameLabel.Text = "(Login Name)";
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.refreshToolStripMenuItem.Text = "R&efresh";
			// 
			// ShellForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(868, 661);
			this.Controls.Add(this.mainPanel);
			this.Controls.Add(this.statusStrip);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "ShellForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Dashboard";
			((System.ComponentModel.ISupportInitialize)(this.mainPanel)).EndInit();
			this.mainPanel.ResumeLayout(false);
			this.mainPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
			this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
			this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
			this.kryptonSplitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator2)).EndInit();
			this.kryptonNavigator2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPage4)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;
		private System.Windows.Forms.StatusStrip statusStrip;
		private ComponentFactory.Krypton.Toolkit.KryptonPanel mainPanel;
		private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
		private ComponentFactory.Krypton.Navigator.KryptonNavigator kryptonNavigator1;
		private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage1;
		private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage2;
		private ComponentFactory.Krypton.Navigator.KryptonNavigator kryptonNavigator2;
		private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage3;
		private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage4;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton logoutButton;
		private System.Windows.Forms.ToolStripLabel loginNameLabel;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
	}
}