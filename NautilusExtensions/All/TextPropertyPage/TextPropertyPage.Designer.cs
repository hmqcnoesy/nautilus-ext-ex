namespace NautilusExtensions.All {
    partial class FpSuccessPage {
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
            this.txtInfoText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtInfoText
            // 
            this.txtInfoText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInfoText.Location = new System.Drawing.Point(4, 4);
            this.txtInfoText.Multiline = true;
            this.txtInfoText.Name = "txtInfoText";
            this.txtInfoText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInfoText.Size = new System.Drawing.Size(293, 293);
            this.txtInfoText.TabIndex = 0;
            this.txtInfoText.TextChanged += new System.EventHandler(this.txtInfoText_TextChanged);
            // 
            // FpSuccessPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtInfoText);
            this.Name = "FpSuccessPage";
            this.Size = new System.Drawing.Size(300, 300);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInfoText;
    }
}
