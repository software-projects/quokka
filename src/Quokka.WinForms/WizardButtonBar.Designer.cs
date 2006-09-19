namespace Quokka.WinForms
{
    partial class WizardButtonBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.cancelButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.topBorder = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.topBorder)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cancelButton.Enabled = false;
            this.cancelButton.Location = new System.Drawing.Point(333, 13);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nextButton.Enabled = false;
            this.nextButton.Location = new System.Drawing.Point(244, 13);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 1;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // backButton
            // 
            this.backButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.backButton.Enabled = false;
            this.backButton.Location = new System.Drawing.Point(166, 13);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 2;
            this.backButton.Text = "Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // topBorder
            // 
            this.topBorder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.topBorder.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBorder.Location = new System.Drawing.Point(0, 0);
            this.topBorder.Name = "topBorder";
            this.topBorder.Size = new System.Drawing.Size(421, 4);
            this.topBorder.TabIndex = 3;
            this.topBorder.TabStop = false;
            // 
            // WizardButtonBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.topBorder);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.cancelButton);
            this.Name = "WizardButtonBar";
            this.Size = new System.Drawing.Size(421, 48);
            this.Load += new System.EventHandler(this.WizardButtonBar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.topBorder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.PictureBox topBorder;
    }
}
