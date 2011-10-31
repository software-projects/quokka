namespace Quokka.WinForms.Controls
{
	partial class MainMenuListView
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
			this.components = new System.ComponentModel.Container();
			this.listView = new System.Windows.Forms.ListView();
			this.largeImageList = new System.Windows.Forms.ImageList(this.components);
			this.smallImageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// listView
			// 
			this.listView.Location = new System.Drawing.Point(81, 88);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(194, 175);
			this.listView.TabIndex = 0;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.ItemActivate += new System.EventHandler(this.listView_ItemActivate);
			// 
			// largeImageList
			// 
			this.largeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.largeImageList.ImageSize = new System.Drawing.Size(32, 32);
			this.largeImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// smallImageList
			// 
			this.smallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.smallImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.smallImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// MainMenuListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listView);
			this.Name = "MainMenuListView";
			this.Size = new System.Drawing.Size(360, 328);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ImageList largeImageList;
		private System.Windows.Forms.ImageList smallImageList;
	}
}
