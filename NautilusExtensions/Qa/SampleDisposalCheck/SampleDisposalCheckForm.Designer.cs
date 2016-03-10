namespace NautilusExtensions.Qa {
    partial class SampleDisposalCheckForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SampleDisposalCheckForm));
            this.label1 = new System.Windows.Forms.Label();
            this.txtSampleName = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tvSample = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pboxStatus = new System.Windows.Forms.PictureBox();
            this.dgvInfo = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblInstructions = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pboxStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Scan or type sample name:";
            // 
            // txtSampleName
            // 
            this.txtSampleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSampleName.Location = new System.Drawing.Point(153, 14);
            this.txtSampleName.Name = "txtSampleName";
            this.txtSampleName.Size = new System.Drawing.Size(243, 20);
            this.txtSampleName.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(402, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(78, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tvSample
            // 
            this.tvSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSample.ImageIndex = 0;
            this.tvSample.ImageList = this.imageList1;
            this.tvSample.Location = new System.Drawing.Point(0, 0);
            this.tvSample.Name = "tvSample";
            this.tvSample.SelectedImageIndex = 0;
            this.tvSample.Size = new System.Drawing.Size(198, 415);
            this.tvSample.TabIndex = 3;
            this.tvSample.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSample_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "aa.gif");
            this.imageList1.Images.SetKeyName(1, "ac.gif");
            this.imageList1.Images.SetKeyName(2, "ai.gif");
            this.imageList1.Images.SetKeyName(3, "ap.gif");
            this.imageList1.Images.SetKeyName(4, "ar.gif");
            this.imageList1.Images.SetKeyName(5, "as.gif");
            this.imageList1.Images.SetKeyName(6, "au.gif");
            this.imageList1.Images.SetKeyName(7, "av.gif");
            this.imageList1.Images.SetKeyName(8, "aw.gif");
            this.imageList1.Images.SetKeyName(9, "ax.gif");
            this.imageList1.Images.SetKeyName(10, "ra.gif");
            this.imageList1.Images.SetKeyName(11, "rc.gif");
            this.imageList1.Images.SetKeyName(12, "ri.gif");
            this.imageList1.Images.SetKeyName(13, "rp.gif");
            this.imageList1.Images.SetKeyName(14, "rr.gif");
            this.imageList1.Images.SetKeyName(15, "rs.gif");
            this.imageList1.Images.SetKeyName(16, "ru.gif");
            this.imageList1.Images.SetKeyName(17, "rv.gif");
            this.imageList1.Images.SetKeyName(18, "rw.gif");
            this.imageList1.Images.SetKeyName(19, "rx.gif");
            this.imageList1.Images.SetKeyName(20, "sa.gif");
            this.imageList1.Images.SetKeyName(21, "sc.gif");
            this.imageList1.Images.SetKeyName(22, "si.gif");
            this.imageList1.Images.SetKeyName(23, "sp.gif");
            this.imageList1.Images.SetKeyName(24, "sr.gif");
            this.imageList1.Images.SetKeyName(25, "ss.gif");
            this.imageList1.Images.SetKeyName(26, "su.gif");
            this.imageList1.Images.SetKeyName(27, "sv.gif");
            this.imageList1.Images.SetKeyName(28, "sw.gif");
            this.imageList1.Images.SetKeyName(29, "sx.gif");
            this.imageList1.Images.SetKeyName(30, "ta.gif");
            this.imageList1.Images.SetKeyName(31, "tc.gif");
            this.imageList1.Images.SetKeyName(32, "ti.gif");
            this.imageList1.Images.SetKeyName(33, "tp.gif");
            this.imageList1.Images.SetKeyName(34, "tr.gif");
            this.imageList1.Images.SetKeyName(35, "ts.gif");
            this.imageList1.Images.SetKeyName(36, "tu.gif");
            this.imageList1.Images.SetKeyName(37, "tv.gif");
            this.imageList1.Images.SetKeyName(38, "tw.gif");
            this.imageList1.Images.SetKeyName(39, "tx.gif");
            this.imageList1.Images.SetKeyName(40, "green.png");
            this.imageList1.Images.SetKeyName(41, "red.png");
            this.imageList1.Images.SetKeyName(42, "yellow.png");
            // 
            // pboxStatus
            // 
            this.pboxStatus.Location = new System.Drawing.Point(153, 40);
            this.pboxStatus.Name = "pboxStatus";
            this.pboxStatus.Size = new System.Drawing.Size(16, 16);
            this.pboxStatus.TabIndex = 4;
            this.pboxStatus.TabStop = false;
            // 
            // dgvInfo
            // 
            this.dgvInfo.AllowUserToAddRows = false;
            this.dgvInfo.AllowUserToDeleteRows = false;
            this.dgvInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInfo.Location = new System.Drawing.Point(0, 0);
            this.dgvInfo.Name = "dgvInfo";
            this.dgvInfo.ReadOnly = true;
            this.dgvInfo.RowHeadersVisible = false;
            this.dgvInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvInfo.ShowCellErrors = false;
            this.dgvInfo.ShowCellToolTips = false;
            this.dgvInfo.ShowEditingIcon = false;
            this.dgvInfo.ShowRowErrors = false;
            this.dgvInfo.Size = new System.Drawing.Size(266, 415);
            this.dgvInfo.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 62);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvSample);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvInfo);
            this.splitContainer1.Size = new System.Drawing.Size(468, 415);
            this.splitContainer1.SplitterDistance = 198;
            this.splitContainer1.TabIndex = 6;
            // 
            // lblInstructions
            // 
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Location = new System.Drawing.Point(175, 43);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(67, 13);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = "                    ";
            // 
            // SampleDisposalCheckForm
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 489);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pboxStatus);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSampleName);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SampleDisposalCheckForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Sample Disposal Check";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pboxStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInfo)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSampleName;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TreeView tvSample;
        private System.Windows.Forms.PictureBox pboxStatus;
        private System.Windows.Forms.DataGridView dgvInfo;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lblInstructions;
    }
}