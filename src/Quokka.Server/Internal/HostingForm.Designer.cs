namespace Quokka.Server.Internal
{
	partial class HostingForm
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
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.startingLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// timer
			// 
			this.timer.Interval = 50;
			this.timer.Tick += new System.EventHandler(this.TimerTick);
			// 
			// startingLabel
			// 
			this.startingLabel.AutoSize = true;
			this.startingLabel.Location = new System.Drawing.Point(28, 31);
			this.startingLabel.Name = "startingLabel";
			this.startingLabel.Size = new System.Drawing.Size(55, 13);
			this.startingLabel.TabIndex = 0;
			this.startingLabel.Text = "Starting ...";
			// 
			// HostingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(657, 484);
			this.Controls.Add(this.startingLabel);
			this.Name = "HostingForm";
			this.Text = "HostingForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HostingFormFormClosing);
			this.Load += new System.EventHandler(this.FormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Label startingLabel;

	}
}