namespace NautilusExtensions.Qa {
    partial class VendorDataUploadForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VendorDataUploadForm));
            this.lblProgressStatus = new System.Windows.Forms.Label();
            this.pbLoginProgress = new System.Windows.Forms.ProgressBar();
            this.btnLogin = new System.Windows.Forms.Button();
            this.tvSample = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cmbProgramCode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // lblProgressStatus
            // 
            this.lblProgressStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgressStatus.Location = new System.Drawing.Point(93, 414);
            this.lblProgressStatus.Name = "lblProgressStatus";
            this.lblProgressStatus.Size = new System.Drawing.Size(286, 13);
            this.lblProgressStatus.TabIndex = 15;
            // 
            // pbLoginProgress
            // 
            this.pbLoginProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLoginProgress.Location = new System.Drawing.Point(93, 436);
            this.pbLoginProgress.Name = "pbLoginProgress";
            this.pbLoginProgress.Size = new System.Drawing.Size(286, 23);
            this.pbLoginProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbLoginProgress.TabIndex = 14;
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogin.Location = new System.Drawing.Point(11, 436);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // tvSample
            // 
            this.tvSample.AllowDrop = true;
            this.tvSample.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvSample.ImageIndex = 0;
            this.tvSample.ImageList = this.imageList1;
            this.tvSample.Location = new System.Drawing.Point(12, 41);
            this.tvSample.Name = "tvSample";
            this.tvSample.SelectedImageIndex = 0;
            this.tvSample.Size = new System.Drawing.Size(367, 366);
            this.tvSample.TabIndex = 2;
            this.tvSample.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvSample_DragDrop);
            this.tvSample.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvSample_DragEnter);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "an");
            this.imageList1.Images.SetKeyName(1, "aa");
            this.imageList1.Images.SetKeyName(2, "ac");
            this.imageList1.Images.SetKeyName(3, "ai");
            this.imageList1.Images.SetKeyName(4, "ap");
            this.imageList1.Images.SetKeyName(5, "ar");
            this.imageList1.Images.SetKeyName(6, "as");
            this.imageList1.Images.SetKeyName(7, "au");
            this.imageList1.Images.SetKeyName(8, "av");
            this.imageList1.Images.SetKeyName(9, "aw");
            this.imageList1.Images.SetKeyName(10, "ax");
            this.imageList1.Images.SetKeyName(11, "rn");
            this.imageList1.Images.SetKeyName(12, "ra");
            this.imageList1.Images.SetKeyName(13, "rc");
            this.imageList1.Images.SetKeyName(14, "ri");
            this.imageList1.Images.SetKeyName(15, "rp");
            this.imageList1.Images.SetKeyName(16, "rr");
            this.imageList1.Images.SetKeyName(17, "rs");
            this.imageList1.Images.SetKeyName(18, "ru");
            this.imageList1.Images.SetKeyName(19, "rv");
            this.imageList1.Images.SetKeyName(20, "rw");
            this.imageList1.Images.SetKeyName(21, "rx");
            this.imageList1.Images.SetKeyName(22, "sn");
            this.imageList1.Images.SetKeyName(23, "sa");
            this.imageList1.Images.SetKeyName(24, "sc");
            this.imageList1.Images.SetKeyName(25, "si");
            this.imageList1.Images.SetKeyName(26, "sp");
            this.imageList1.Images.SetKeyName(27, "sr");
            this.imageList1.Images.SetKeyName(28, "ss");
            this.imageList1.Images.SetKeyName(29, "su");
            this.imageList1.Images.SetKeyName(30, "sv");
            this.imageList1.Images.SetKeyName(31, "sw");
            this.imageList1.Images.SetKeyName(32, "sx");
            this.imageList1.Images.SetKeyName(33, "tn");
            this.imageList1.Images.SetKeyName(34, "ta");
            this.imageList1.Images.SetKeyName(35, "tc");
            this.imageList1.Images.SetKeyName(36, "ti");
            this.imageList1.Images.SetKeyName(37, "tp");
            this.imageList1.Images.SetKeyName(38, "tr");
            this.imageList1.Images.SetKeyName(39, "ts");
            this.imageList1.Images.SetKeyName(40, "tu");
            this.imageList1.Images.SetKeyName(41, "tv");
            this.imageList1.Images.SetKeyName(42, "tw");
            this.imageList1.Images.SetKeyName(43, "tx");
            // 
            // cmbProgramCode
            // 
            this.cmbProgramCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbProgramCode.FormattingEnabled = true;
            this.cmbProgramCode.Location = new System.Drawing.Point(212, 14);
            this.cmbProgramCode.Name = "cmbProgramCode";
            this.cmbProgramCode.Size = new System.Drawing.Size(167, 21);
            this.cmbProgramCode.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(132, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Program Code";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(104, 17);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(0, 13);
            this.lblFileName.TabIndex = 9;
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.AutoSize = true;
            this.btnLoadFile.Location = new System.Drawing.Point(12, 12);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(86, 23);
            this.btnLoadFile.TabIndex = 0;
            this.btnLoadFile.Text = "Load Data File";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // VendorDataUploadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 472);
            this.Controls.Add(this.lblProgressStatus);
            this.Controls.Add(this.pbLoginProgress);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.tvSample);
            this.Controls.Add(this.cmbProgramCode);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.btnLoadFile);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VendorDataUploadForm";
            this.Text = "Vendor Data Upload";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProgressStatus;
        private System.Windows.Forms.ProgressBar pbLoginProgress;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TreeView tvSample;
        private System.Windows.Forms.ComboBox cmbProgramCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.ImageList imageList1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}