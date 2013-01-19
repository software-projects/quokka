namespace Quokka.WinForms.Config
{
	partial class DirectoryEditor
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
			this.TextBox = new System.Windows.Forms.TextBox();
			this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.BrowseButton = new System.Windows.Forms.Button();
			this.TableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// TextBox
			// 
			this.TextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.TextBox.Location = new System.Drawing.Point(3, 4);
			this.TextBox.Name = "TextBox";
			this.TextBox.Size = new System.Drawing.Size(159, 20);
			this.TextBox.TabIndex = 0;
			// 
			// TableLayoutPanel
			// 
			this.TableLayoutPanel.AutoSize = true;
			this.TableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TableLayoutPanel.ColumnCount = 2;
			this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.TableLayoutPanel.Controls.Add(this.BrowseButton, 1, 0);
			this.TableLayoutPanel.Controls.Add(this.TextBox, 0, 0);
			this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.TableLayoutPanel.MinimumSize = new System.Drawing.Size(100, 23);
			this.TableLayoutPanel.Name = "TableLayoutPanel";
			this.TableLayoutPanel.RowCount = 1;
			this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TableLayoutPanel.Size = new System.Drawing.Size(223, 29);
			this.TableLayoutPanel.TabIndex = 1;
			// 
			// BrowseButton
			// 
			this.BrowseButton.AutoSize = true;
			this.BrowseButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BrowseButton.Location = new System.Drawing.Point(168, 3);
			this.BrowseButton.Name = "BrowseButton";
			this.BrowseButton.Size = new System.Drawing.Size(52, 23);
			this.BrowseButton.TabIndex = 1;
			this.BrowseButton.Text = "Browse";
			this.BrowseButton.UseVisualStyleBackColor = true;
			this.BrowseButton.Click += new System.EventHandler(this.BrowseButtonClick);
			// 
			// DirectoryEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.Transparent;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.TableLayoutPanel);
			this.MinimumSize = new System.Drawing.Size(100, 25);
			this.Name = "DirectoryEditor";
			this.Size = new System.Drawing.Size(223, 29);
			this.TableLayoutPanel.ResumeLayout(false);
			this.TableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Button BrowseButton;
		public System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
		public System.Windows.Forms.TextBox TextBox;
	}
}
