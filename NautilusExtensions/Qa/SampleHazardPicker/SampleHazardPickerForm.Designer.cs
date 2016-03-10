namespace NautilusExtensions.Qa
{
    partial class SampleHazardPickerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SampleHazardPickerForm));
            this.dgvHazards = new System.Windows.Forms.DataGridView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblSampleName = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblSampleStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHazards)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvHazards
            // 
            this.dgvHazards.AllowUserToAddRows = false;
            this.dgvHazards.AllowUserToDeleteRows = false;
            this.dgvHazards.AllowUserToResizeRows = false;
            this.dgvHazards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHazards.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHazards.Location = new System.Drawing.Point(13, 32);
            this.dgvHazards.Name = "dgvHazards";
            this.dgvHazards.RowHeadersVisible = false;
            this.dgvHazards.ShowEditingIcon = false;
            this.dgvHazards.Size = new System.Drawing.Size(623, 373);
            this.dgvHazards.TabIndex = 0;
            this.dgvHazards.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHazards_CellContentClick);
            this.dgvHazards.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHazards_CellEndEdit);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(561, 411);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(480, 411);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblSampleName
            // 
            this.lblSampleName.AutoSize = true;
            this.lblSampleName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSampleName.ImageKey = "(none)";
            this.lblSampleName.Location = new System.Drawing.Point(34, 9);
            this.lblSampleName.Name = "lblSampleName";
            this.lblSampleName.Size = new System.Drawing.Size(0, 13);
            this.lblSampleName.TabIndex = 3;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "A");
            this.imageList1.Images.SetKeyName(1, "C");
            this.imageList1.Images.SetKeyName(2, "I");
            this.imageList1.Images.SetKeyName(3, "P");
            this.imageList1.Images.SetKeyName(4, "R");
            this.imageList1.Images.SetKeyName(5, "S");
            this.imageList1.Images.SetKeyName(6, "U");
            this.imageList1.Images.SetKeyName(7, "V");
            this.imageList1.Images.SetKeyName(8, "W");
            this.imageList1.Images.SetKeyName(9, "X");
            // 
            // lblSampleStatus
            // 
            this.lblSampleStatus.ImageKey = "(none)";
            this.lblSampleStatus.ImageList = this.imageList1;
            this.lblSampleStatus.Location = new System.Drawing.Point(12, 9);
            this.lblSampleStatus.Name = "lblSampleStatus";
            this.lblSampleStatus.Size = new System.Drawing.Size(16, 16);
            this.lblSampleStatus.TabIndex = 4;
            // 
            // SampleHazardPickerForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(648, 446);
            this.Controls.Add(this.lblSampleStatus);
            this.Controls.Add(this.lblSampleName);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dgvHazards);
            this.Name = "SampleHazardPickerForm";
            this.Text = "Sample Hazard Picker";
            this.Load += new System.EventHandler(this.SampleHazardPickerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHazards)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvHazards;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblSampleName;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lblSampleStatus;
    }
}