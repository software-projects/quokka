namespace Sprocket.Manager
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.mainPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.serverEndPointLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.connectionStatus = new System.Windows.Forms.ToolStripStatusLabel();
			((System.ComponentModel.ISupportInitialize)(this.mainPanel)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(840, 540);
			this.mainPanel.TabIndex = 0;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverEndPointLabel,
            this.connectionStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 540);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
			this.statusStrip1.Size = new System.Drawing.Size(840, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// serverEndPointLabel
			// 
			this.serverEndPointLabel.Name = "serverEndPointLabel";
			this.serverEndPointLabel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
			this.serverEndPointLabel.Size = new System.Drawing.Size(75, 17);
			this.serverEndPointLabel.Text = "(No server)";
			// 
			// connectionStatus
			// 
			this.connectionStatus.Name = "connectionStatus";
			this.connectionStatus.Size = new System.Drawing.Size(86, 17);
			this.connectionStatus.Text = "Not connected";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(840, 562);
			this.Controls.Add(this.mainPanel);
			this.Controls.Add(this.statusStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SPROCKET Management Console";
			this.Load += new System.EventHandler(this.FormLoad);
			((System.ComponentModel.ISupportInitialize)(this.mainPanel)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel mainPanel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatus;
        private System.Windows.Forms.ToolStripStatusLabel serverEndPointLabel;
    }
}

