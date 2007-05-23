namespace Quokka.WinForms.Tests
{
	partial class MessageBoxTestForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.clientPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.mainInstructionTextBox = new System.Windows.Forms.TextBox();
			this.contentTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.messageTypeComboBox = new System.Windows.Forms.ComboBox();
			this.topPanel = new System.Windows.Forms.Panel();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.displayButton = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.clientPanel.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.topPanel.SuspendLayout();
			this.bottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(146, 230);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			// 
			// clientPanel
			// 
			this.clientPanel.Controls.Add(this.tableLayoutPanel1);
			this.clientPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.clientPanel.Location = new System.Drawing.Point(0, 42);
			this.clientPanel.Name = "clientPanel";
			this.clientPanel.Size = new System.Drawing.Size(403, 252);
			this.clientPanel.TabIndex = 2;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.8642F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.1358F));
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.mainInstructionTextBox, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.contentTextBox, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.messageTypeComboBox, 1, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(403, 252);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 3);
			this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Main instruction";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Content";
			// 
			// mainInstructionTextBox
			// 
			this.mainInstructionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainInstructionTextBox.Location = new System.Drawing.Point(127, 3);
			this.mainInstructionTextBox.Name = "mainInstructionTextBox";
			this.mainInstructionTextBox.Size = new System.Drawing.Size(273, 20);
			this.mainInstructionTextBox.TabIndex = 2;
			// 
			// contentTextBox
			// 
			this.contentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentTextBox.Location = new System.Drawing.Point(127, 29);
			this.contentTextBox.Multiline = true;
			this.contentTextBox.Name = "contentTextBox";
			this.contentTextBox.Size = new System.Drawing.Size(273, 107);
			this.contentTextBox.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 142);
			this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(76, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "Message Type";
			// 
			// messageTypeComboBox
			// 
			this.messageTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.messageTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.messageTypeComboBox.FormattingEnabled = true;
			this.messageTypeComboBox.Items.AddRange(new object[] {
            "Information",
            "Success",
            "Question",
            "Warning",
            "Failure",
            "Forbidden",
            "Unauthorized"});
			this.messageTypeComboBox.Location = new System.Drawing.Point(127, 142);
			this.messageTypeComboBox.Name = "messageTypeComboBox";
			this.messageTypeComboBox.Size = new System.Drawing.Size(273, 21);
			this.messageTypeComboBox.TabIndex = 5;
			// 
			// topPanel
			// 
			this.topPanel.Controls.Add(this.label5);
			this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.topPanel.Location = new System.Drawing.Point(0, 0);
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(403, 42);
			this.topPanel.TabIndex = 3;
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.button1);
			this.bottomPanel.Controls.Add(this.displayButton);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point(0, 294);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(403, 50);
			this.bottomPanel.TabIndex = 4;
			// 
			// displayButton
			// 
			this.displayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.displayButton.Location = new System.Drawing.Point(316, 15);
			this.displayButton.Name = "displayButton";
			this.displayButton.Size = new System.Drawing.Size(75, 23);
			this.displayButton.TabIndex = 0;
			this.displayButton.Text = "Display";
			this.displayButton.UseVisualStyleBackColor = true;
			this.displayButton.Click += new System.EventHandler(this.displayButton_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Tahoma", 14F);
			this.label5.Location = new System.Drawing.Point(3, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(176, 23);
			this.label5.TabIndex = 0;
			this.label5.Text = "Message Box Tester";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(23, 15);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(73, 22);
			this.button1.TabIndex = 1;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// MessageBoxTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(403, 344);
			this.Controls.Add(this.clientPanel);
			this.Controls.Add(this.bottomPanel);
			this.Controls.Add(this.topPanel);
			this.Controls.Add(this.label1);
			this.Name = "MessageBoxTestForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Message Box Test Form";
			this.clientPanel.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.topPanel.ResumeLayout(false);
			this.topPanel.PerformLayout();
			this.bottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel clientPanel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox mainInstructionTextBox;
		private System.Windows.Forms.TextBox contentTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox messageTypeComboBox;
		private System.Windows.Forms.Panel topPanel;
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button displayButton;
		private System.Windows.Forms.Button button1;
	}
}

