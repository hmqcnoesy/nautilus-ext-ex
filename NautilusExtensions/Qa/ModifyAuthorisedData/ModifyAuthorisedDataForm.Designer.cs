namespace NautilusExtensions.Qa {
    partial class ModifyAuthorisedDataForm {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModifyAuthorisedDataForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvSamples = new System.Windows.Forms.TreeView();
            this.imageListDynamicData = new System.Windows.Forms.ImageList(this.components);
            this.dgvAuthorisedData = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.txtOperatorRemarks = new System.Windows.Forms.TextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuthorisedData)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(805, 538);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(724, 538);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(13, 42);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvSamples);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvAuthorisedData);
            this.splitContainer1.Size = new System.Drawing.Size(867, 490);
            this.splitContainer1.SplitterDistance = 168;
            this.splitContainer1.TabIndex = 2;
            // 
            // tvSamples
            // 
            this.tvSamples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSamples.ImageIndex = 0;
            this.tvSamples.ImageList = this.imageListDynamicData;
            this.tvSamples.Location = new System.Drawing.Point(0, 0);
            this.tvSamples.Name = "tvSamples";
            this.tvSamples.SelectedImageIndex = 0;
            this.tvSamples.Size = new System.Drawing.Size(168, 490);
            this.tvSamples.TabIndex = 0;
            this.tvSamples.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSamples_AfterSelect);
            // 
            // imageListDynamicData
            // 
            this.imageListDynamicData.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListDynamicData.ImageStream")));
            this.imageListDynamicData.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListDynamicData.Images.SetKeyName(0, "aliquota");
            this.imageListDynamicData.Images.SetKeyName(1, "aliquotc");
            this.imageListDynamicData.Images.SetKeyName(2, "aliquoti");
            this.imageListDynamicData.Images.SetKeyName(3, "aliquotp");
            this.imageListDynamicData.Images.SetKeyName(4, "aliquotr");
            this.imageListDynamicData.Images.SetKeyName(5, "aliquots");
            this.imageListDynamicData.Images.SetKeyName(6, "aliquotu");
            this.imageListDynamicData.Images.SetKeyName(7, "aliquotv");
            this.imageListDynamicData.Images.SetKeyName(8, "aliquotw");
            this.imageListDynamicData.Images.SetKeyName(9, "aliquotx");
            this.imageListDynamicData.Images.SetKeyName(10, "resulta");
            this.imageListDynamicData.Images.SetKeyName(11, "resultc");
            this.imageListDynamicData.Images.SetKeyName(12, "resulti");
            this.imageListDynamicData.Images.SetKeyName(13, "resultp");
            this.imageListDynamicData.Images.SetKeyName(14, "resultr");
            this.imageListDynamicData.Images.SetKeyName(15, "results");
            this.imageListDynamicData.Images.SetKeyName(16, "resultu");
            this.imageListDynamicData.Images.SetKeyName(17, "resultv");
            this.imageListDynamicData.Images.SetKeyName(18, "resultw");
            this.imageListDynamicData.Images.SetKeyName(19, "resultx");
            this.imageListDynamicData.Images.SetKeyName(20, "samplea");
            this.imageListDynamicData.Images.SetKeyName(21, "samplec");
            this.imageListDynamicData.Images.SetKeyName(22, "samplei");
            this.imageListDynamicData.Images.SetKeyName(23, "samplep");
            this.imageListDynamicData.Images.SetKeyName(24, "sampler");
            this.imageListDynamicData.Images.SetKeyName(25, "samples");
            this.imageListDynamicData.Images.SetKeyName(26, "sampleu");
            this.imageListDynamicData.Images.SetKeyName(27, "samplev");
            this.imageListDynamicData.Images.SetKeyName(28, "samplew");
            this.imageListDynamicData.Images.SetKeyName(29, "samplex");
            this.imageListDynamicData.Images.SetKeyName(30, "testa");
            this.imageListDynamicData.Images.SetKeyName(31, "testc");
            this.imageListDynamicData.Images.SetKeyName(32, "testi");
            this.imageListDynamicData.Images.SetKeyName(33, "testp");
            this.imageListDynamicData.Images.SetKeyName(34, "testr");
            this.imageListDynamicData.Images.SetKeyName(35, "tests");
            this.imageListDynamicData.Images.SetKeyName(36, "testu");
            this.imageListDynamicData.Images.SetKeyName(37, "testv");
            this.imageListDynamicData.Images.SetKeyName(38, "testw");
            this.imageListDynamicData.Images.SetKeyName(39, "testx");
            // 
            // dgvAuthorisedData
            // 
            this.dgvAuthorisedData.AllowUserToAddRows = false;
            this.dgvAuthorisedData.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAuthorisedData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAuthorisedData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAuthorisedData.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAuthorisedData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAuthorisedData.Location = new System.Drawing.Point(0, 0);
            this.dgvAuthorisedData.MultiSelect = false;
            this.dgvAuthorisedData.Name = "dgvAuthorisedData";
            this.dgvAuthorisedData.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAuthorisedData.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvAuthorisedData.Size = new System.Drawing.Size(695, 490);
            this.dgvAuthorisedData.TabIndex = 0;
            this.dgvAuthorisedData.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAuthorisedData_CellValueChanged);
            this.dgvAuthorisedData.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvAuthorisedData_DataError);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(494, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Your personal session information will be recorded with your remarks when authori" +
                "sed data are modified:";
            // 
            // txtOperatorRemarks
            // 
            this.txtOperatorRemarks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOperatorRemarks.BackColor = System.Drawing.Color.GreenYellow;
            this.txtOperatorRemarks.Location = new System.Drawing.Point(556, 6);
            this.txtOperatorRemarks.MaxLength = 200;
            this.txtOperatorRemarks.Name = "txtOperatorRemarks";
            this.txtOperatorRemarks.Size = new System.Drawing.Size(324, 20);
            this.txtOperatorRemarks.TabIndex = 4;
            this.txtOperatorRemarks.TextChanged += new System.EventHandler(this.txtOperatorRemarks_TextChanged);
            // 
            // ModifyAuthorisedDataForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(892, 573);
            this.Controls.Add(this.txtOperatorRemarks);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Name = "ModifyAuthorisedDataForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Modify Authorised Data";
            this.Load += new System.EventHandler(this.ModifyAuthorisedDataForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ModifyAuthorisedDataForm_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAuthorisedData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvSamples;
        private System.Windows.Forms.DataGridView dgvAuthorisedData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageListDynamicData;
        private System.Windows.Forms.TextBox txtOperatorRemarks;
    }
}