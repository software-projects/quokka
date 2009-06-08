namespace Example2.Tasks.Simple
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
			this.button = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button
			// 
			this.button.Location = new System.Drawing.Point(86, 116);
			this.button.Name = "button";
			this.button.Size = new System.Drawing.Size(115, 23);
			this.button.TabIndex = 0;
			this.button.Text = "Create new task";
			this.button.UseVisualStyleBackColor = true;
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.Location = new System.Drawing.Point(205, 12);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			// 
			// SimpleView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.button);
			this.Name = "SimpleView";
			this.Text = "Task1View";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button;
		private System.Windows.Forms.Button closeButton;
	}
}