namespace Sprocket.Manager.Views.ShowStatus
{
    partial class StatusView
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
			this.splitContainer = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.sessionDataGridView = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
			this.sessionClientIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.sessionIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.sessionConnectedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.sessionSubscriptionCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.kryptonPanel2 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.messageQueueDataGridView = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
			this.messageQueueNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.messageQueueSubscriptionCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.messageQueuePendingMessageCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.messageQueueTotalMessagesCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).BeginInit();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sessionDataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
			this.kryptonPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.messageQueueDataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
			this.kryptonPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Cursor = System.Windows.Forms.Cursors.Default;
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.sessionDataGridView);
			this.splitContainer.Panel1.Controls.Add(this.kryptonPanel2);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.messageQueueDataGridView);
			this.splitContainer.Panel2.Controls.Add(this.kryptonPanel1);
			this.splitContainer.SeparatorStyle = ComponentFactory.Krypton.Toolkit.SeparatorStyle.HighProfile;
			this.splitContainer.Size = new System.Drawing.Size(725, 618);
			this.splitContainer.SplitterDistance = 404;
			this.splitContainer.TabIndex = 0;
			// 
			// sessionDataGridView
			// 
			this.sessionDataGridView.AllowUserToAddRows = false;
			this.sessionDataGridView.AllowUserToDeleteRows = false;
			this.sessionDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sessionClientIdColumn,
            this.sessionIdColumn,
            this.sessionConnectedColumn,
            this.sessionSubscriptionCountColumn});
			this.sessionDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sessionDataGridView.Location = new System.Drawing.Point(0, 31);
			this.sessionDataGridView.Name = "sessionDataGridView";
			this.sessionDataGridView.ReadOnly = true;
			this.sessionDataGridView.RowHeadersVisible = false;
			this.sessionDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.sessionDataGridView.Size = new System.Drawing.Size(404, 587);
			this.sessionDataGridView.TabIndex = 1;
			// 
			// sessionClientIdColumn
			// 
			this.sessionClientIdColumn.HeaderText = "Client";
			this.sessionClientIdColumn.Name = "sessionClientIdColumn";
			this.sessionClientIdColumn.ReadOnly = true;
			// 
			// sessionIdColumn
			// 
			this.sessionIdColumn.HeaderText = "ID";
			this.sessionIdColumn.Name = "sessionIdColumn";
			this.sessionIdColumn.ReadOnly = true;
			// 
			// sessionConnectedColumn
			// 
			this.sessionConnectedColumn.HeaderText = "Connected";
			this.sessionConnectedColumn.Name = "sessionConnectedColumn";
			this.sessionConnectedColumn.ReadOnly = true;
			// 
			// sessionSubscriptionCountColumn
			// 
			this.sessionSubscriptionCountColumn.HeaderText = "Subscriptions";
			this.sessionSubscriptionCountColumn.Name = "sessionSubscriptionCountColumn";
			this.sessionSubscriptionCountColumn.ReadOnly = true;
			// 
			// kryptonPanel2
			// 
			this.kryptonPanel2.Controls.Add(this.kryptonLabel2);
			this.kryptonPanel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.kryptonPanel2.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel2.Name = "kryptonPanel2";
			this.kryptonPanel2.Size = new System.Drawing.Size(404, 31);
			this.kryptonPanel2.TabIndex = 2;
			// 
			// kryptonLabel2
			// 
			this.kryptonLabel2.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.TitleControl;
			this.kryptonLabel2.Location = new System.Drawing.Point(1, 1);
			this.kryptonLabel2.Name = "kryptonLabel2";
			this.kryptonLabel2.Size = new System.Drawing.Size(85, 29);
			this.kryptonLabel2.TabIndex = 0;
			this.kryptonLabel2.Values.Text = "Sessions";
			// 
			// messageQueueDataGridView
			// 
			this.messageQueueDataGridView.AllowUserToAddRows = false;
			this.messageQueueDataGridView.AllowUserToDeleteRows = false;
			this.messageQueueDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.messageQueueNameColumn,
            this.messageQueueSubscriptionCountColumn,
            this.messageQueuePendingMessageCountColumn,
            this.messageQueueTotalMessagesCountColumn});
			this.messageQueueDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.messageQueueDataGridView.Location = new System.Drawing.Point(0, 31);
			this.messageQueueDataGridView.Name = "messageQueueDataGridView";
			this.messageQueueDataGridView.ReadOnly = true;
			this.messageQueueDataGridView.RowHeadersVisible = false;
			this.messageQueueDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.messageQueueDataGridView.Size = new System.Drawing.Size(316, 587);
			this.messageQueueDataGridView.TabIndex = 1;
			// 
			// messageQueueNameColumn
			// 
			this.messageQueueNameColumn.HeaderText = "Name";
			this.messageQueueNameColumn.Name = "messageQueueNameColumn";
			this.messageQueueNameColumn.ReadOnly = true;
			// 
			// messageQueueSubscriptionCountColumn
			// 
			this.messageQueueSubscriptionCountColumn.HeaderText = "Subscriptions";
			this.messageQueueSubscriptionCountColumn.Name = "messageQueueSubscriptionCountColumn";
			this.messageQueueSubscriptionCountColumn.ReadOnly = true;
			// 
			// messageQueuePendingMessageCountColumn
			// 
			this.messageQueuePendingMessageCountColumn.HeaderText = "Pending";
			this.messageQueuePendingMessageCountColumn.Name = "messageQueuePendingMessageCountColumn";
			this.messageQueuePendingMessageCountColumn.ReadOnly = true;
			// 
			// messageQueueTotalMessagesCountColumn
			// 
			this.messageQueueTotalMessagesCountColumn.HeaderText = "Total";
			this.messageQueueTotalMessagesCountColumn.Name = "messageQueueTotalMessagesCountColumn";
			this.messageQueueTotalMessagesCountColumn.ReadOnly = true;
			// 
			// kryptonPanel1
			// 
			this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
			this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
			this.kryptonPanel1.Name = "kryptonPanel1";
			this.kryptonPanel1.Size = new System.Drawing.Size(316, 31);
			this.kryptonPanel1.TabIndex = 2;
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.TitleControl;
			this.kryptonLabel1.Location = new System.Drawing.Point(1, 1);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(158, 29);
			this.kryptonLabel1.TabIndex = 0;
			this.kryptonLabel1.Values.Text = "Message Queues";
			// 
			// StatusView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer);
			this.Name = "StatusView";
			this.Size = new System.Drawing.Size(725, 618);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).EndInit();
			this.splitContainer.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).EndInit();
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sessionDataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
			this.kryptonPanel2.ResumeLayout(false);
			this.kryptonPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.messageQueueDataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
			this.kryptonPanel1.ResumeLayout(false);
			this.kryptonPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer splitContainer;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView sessionDataGridView;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView messageQueueDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageQueueNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageQueueSubscriptionCountColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageQueuePendingMessageCountColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageQueueTotalMessagesCountColumn;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn sessionClientIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sessionIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sessionConnectedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sessionSubscriptionCountColumn;

    }
}
