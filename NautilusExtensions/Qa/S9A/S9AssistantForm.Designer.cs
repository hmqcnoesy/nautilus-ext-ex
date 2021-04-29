namespace NautilusExtensions.Qa {
    partial class S9AssistantForm {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lboxAliquots = new System.Windows.Forms.ListBox();
            this.dgvSlNumbers = new System.Windows.Forms.DataGridView();
            this.lblAnalyst = new System.Windows.Forms.Label();
            this.lblInstrumentName = new System.Windows.Forms.Label();
            this.lblSettings = new System.Windows.Forms.Label();
            this.txtValidSpecimens = new System.Windows.Forms.TextBox();
            this.txtAnalystName = new System.Windows.Forms.TextBox();
            this.txtCrossheadSpeed = new System.Windows.Forms.TextBox();
            this.txtInstrument = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCrossheadSpeed = new System.Windows.Forms.Label();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.btnSendData = new System.Windows.Forms.Button();
            this.txtTemperature = new System.Windows.Forms.TextBox();
            this.txtHumidity = new System.Windows.Forms.TextBox();
            this.lblHumidity = new System.Windows.Forms.Label();
            this.txtAliquotName = new System.Windows.Forms.TextBox();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOpenFolder = new System.Windows.Forms.ToolStripButton();
            this.btnOpenFiles = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnViewFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteAll = new System.Windows.Forms.ToolStripButton();
            this.txtSettings = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRevalidate = new System.Windows.Forms.LinkLabel();
            this.txtValidation = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnOverrideValidationFailures = new System.Windows.Forms.LinkLabel();
            this.txtOverrideValidationFailuresPassword = new System.Windows.Forms.TextBox();
            this.btnOverrideValidationFailuresOk = new System.Windows.Forms.Button();
            this.lblOverrideValidationFailuresStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSlNumbers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lboxAliquots
            // 
            this.lboxAliquots.AllowDrop = true;
            this.lboxAliquots.FormattingEnabled = true;
            this.lboxAliquots.Location = new System.Drawing.Point(6, 44);
            this.lboxAliquots.Name = "lboxAliquots";
            this.lboxAliquots.Size = new System.Drawing.Size(230, 121);
            this.lboxAliquots.TabIndex = 0;
            this.lboxAliquots.SelectedIndexChanged += new System.EventHandler(this.lboxAliquots_SelectedIndexChanged);
            this.lboxAliquots.DragDrop += new System.Windows.Forms.DragEventHandler(this.lboxAliquots_DragDrop);
            this.lboxAliquots.DragEnter += new System.Windows.Forms.DragEventHandler(this.lboxAliquots_DragEnter);
            // 
            // dgvSlNumbers
            // 
            this.dgvSlNumbers.AllowUserToAddRows = false;
            this.dgvSlNumbers.AllowUserToDeleteRows = false;
            this.dgvSlNumbers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSlNumbers.Location = new System.Drawing.Point(10, 94);
            this.dgvSlNumbers.Name = "dgvSlNumbers";
            this.dgvSlNumbers.RowHeadersVisible = false;
            this.dgvSlNumbers.RowHeadersWidth = 28;
            this.dgvSlNumbers.Size = new System.Drawing.Size(387, 125);
            this.dgvSlNumbers.TabIndex = 1;
            // 
            // lblAnalyst
            // 
            this.lblAnalyst.AutoSize = true;
            this.lblAnalyst.Location = new System.Drawing.Point(6, 202);
            this.lblAnalyst.Name = "lblAnalyst";
            this.lblAnalyst.Size = new System.Drawing.Size(44, 13);
            this.lblAnalyst.TabIndex = 120;
            this.lblAnalyst.Text = "Analyst:";
            // 
            // lblInstrumentName
            // 
            this.lblInstrumentName.AutoSize = true;
            this.lblInstrumentName.Location = new System.Drawing.Point(338, 26);
            this.lblInstrumentName.Name = "lblInstrumentName";
            this.lblInstrumentName.Size = new System.Drawing.Size(59, 13);
            this.lblInstrumentName.TabIndex = 112;
            this.lblInstrumentName.Text = "Instrument:";
            // 
            // lblSettings
            // 
            this.lblSettings.AutoSize = true;
            this.lblSettings.Location = new System.Drawing.Point(6, 176);
            this.lblSettings.Name = "lblSettings";
            this.lblSettings.Size = new System.Drawing.Size(48, 13);
            this.lblSettings.TabIndex = 137;
            this.lblSettings.Text = "Settings:";
            // 
            // txtValidSpecimens
            // 
            this.txtValidSpecimens.Location = new System.Drawing.Point(405, 49);
            this.txtValidSpecimens.Name = "txtValidSpecimens";
            this.txtValidSpecimens.ReadOnly = true;
            this.txtValidSpecimens.Size = new System.Drawing.Size(160, 20);
            this.txtValidSpecimens.TabIndex = 8;
            // 
            // txtAnalystName
            // 
            this.txtAnalystName.Location = new System.Drawing.Point(60, 199);
            this.txtAnalystName.Name = "txtAnalystName";
            this.txtAnalystName.ReadOnly = true;
            this.txtAnalystName.Size = new System.Drawing.Size(176, 20);
            this.txtAnalystName.TabIndex = 2;
            // 
            // txtCrossheadSpeed
            // 
            this.txtCrossheadSpeed.Location = new System.Drawing.Point(85, 49);
            this.txtCrossheadSpeed.Name = "txtCrossheadSpeed";
            this.txtCrossheadSpeed.ReadOnly = true;
            this.txtCrossheadSpeed.Size = new System.Drawing.Size(160, 20);
            this.txtCrossheadSpeed.TabIndex = 3;
            // 
            // txtInstrument
            // 
            this.txtInstrument.Location = new System.Drawing.Point(406, 23);
            this.txtInstrument.Name = "txtInstrument";
            this.txtInstrument.ReadOnly = true;
            this.txtInstrument.Size = new System.Drawing.Size(160, 20);
            this.txtInstrument.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(337, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 132;
            this.label5.Text = "Specimens:";
            // 
            // lblCrossheadSpeed
            // 
            this.lblCrossheadSpeed.AutoSize = true;
            this.lblCrossheadSpeed.Location = new System.Drawing.Point(7, 52);
            this.lblCrossheadSpeed.Name = "lblCrossheadSpeed";
            this.lblCrossheadSpeed.Size = new System.Drawing.Size(41, 13);
            this.lblCrossheadSpeed.TabIndex = 133;
            this.lblCrossheadSpeed.Text = "Speed:";
            // 
            // lblTemperature
            // 
            this.lblTemperature.AutoSize = true;
            this.lblTemperature.Location = new System.Drawing.Point(686, 52);
            this.lblTemperature.Name = "lblTemperature";
            this.lblTemperature.Size = new System.Drawing.Size(70, 13);
            this.lblTemperature.TabIndex = 118;
            this.lblTemperature.Text = "Temperature:";
            // 
            // btnSendData
            // 
            this.btnSendData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendData.Location = new System.Drawing.Point(974, 609);
            this.btnSendData.Name = "btnSendData";
            this.btnSendData.Size = new System.Drawing.Size(149, 23);
            this.btnSendData.TabIndex = 0;
            this.btnSendData.TabStop = false;
            this.btnSendData.Text = "Save Data to Destinations";
            this.btnSendData.UseVisualStyleBackColor = true;
            this.btnSendData.Click += new System.EventHandler(this.btnSendData_Click);
            // 
            // txtTemperature
            // 
            this.txtTemperature.Location = new System.Drawing.Point(754, 49);
            this.txtTemperature.Name = "txtTemperature";
            this.txtTemperature.ReadOnly = true;
            this.txtTemperature.Size = new System.Drawing.Size(100, 20);
            this.txtTemperature.TabIndex = 6;
            // 
            // txtHumidity
            // 
            this.txtHumidity.Location = new System.Drawing.Point(754, 23);
            this.txtHumidity.Name = "txtHumidity";
            this.txtHumidity.ReadOnly = true;
            this.txtHumidity.Size = new System.Drawing.Size(100, 20);
            this.txtHumidity.TabIndex = 7;
            // 
            // lblHumidity
            // 
            this.lblHumidity.AutoSize = true;
            this.lblHumidity.Location = new System.Drawing.Point(686, 26);
            this.lblHumidity.Name = "lblHumidity";
            this.lblHumidity.Size = new System.Drawing.Size(50, 13);
            this.lblHumidity.TabIndex = 115;
            this.lblHumidity.Text = "Humidity:";
            // 
            // txtAliquotName
            // 
            this.txtAliquotName.Location = new System.Drawing.Point(85, 23);
            this.txtAliquotName.Name = "txtAliquotName";
            this.txtAliquotName.ReadOnly = true;
            this.txtAliquotName.Size = new System.Drawing.Size(160, 20);
            this.txtAliquotName.TabIndex = 1;
            // 
            // dgvResults
            // 
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvResults.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResults.Location = new System.Drawing.Point(3, 16);
            this.dgvResults.Name = "dgvResults";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvResults.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.RowHeadersWidth = 28;
            this.dgvResults.Size = new System.Drawing.Size(1105, 341);
            this.dgvResults.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpenFolder,
            this.btnOpenFiles,
            this.toolStripSeparator1,
            this.btnViewFile,
            this.toolStripSeparator2,
            this.btnDeleteItem,
            this.btnDeleteAll});
            this.toolStrip1.Location = new System.Drawing.Point(3, 16);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(161, 25);
            this.toolStrip1.TabIndex = 141;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenFolder.Image = global::NautilusExtensions.Properties.Resources.Folder;
            this.btnOpenFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(23, 22);
            this.btnOpenFolder.Text = "Load all files in a folder";
            this.btnOpenFolder.Click += new System.EventHandler(this.OpenFolder_Click);
            // 
            // btnOpenFiles
            // 
            this.btnOpenFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenFiles.Image = global::NautilusExtensions.Properties.Resources.AsciiFile;
            this.btnOpenFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenFiles.Name = "btnOpenFiles";
            this.btnOpenFiles.Size = new System.Drawing.Size(23, 22);
            this.btnOpenFiles.Text = "Load one or more files";
            this.btnOpenFiles.Click += new System.EventHandler(this.OpenFiles_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnViewFile
            // 
            this.btnViewFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnViewFile.Image = global::NautilusExtensions.Properties.Resources.viewfile;
            this.btnViewFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnViewFile.Name = "btnViewFile";
            this.btnViewFile.Size = new System.Drawing.Size(23, 22);
            this.btnViewFile.Text = "View selected file\'s contents";
            this.btnViewFile.Click += new System.EventHandler(this.ViewFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnDeleteItem
            // 
            this.btnDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDeleteItem.Image = global::NautilusExtensions.Properties.Resources.Delete;
            this.btnDeleteItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteItem.Name = "btnDeleteItem";
            this.btnDeleteItem.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteItem.Text = "Remove selected file from list";
            this.btnDeleteItem.Click += new System.EventHandler(this.DeleteItem_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDeleteAll.Image = global::NautilusExtensions.Properties.Resources.deleteall;
            this.btnDeleteAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteAll.Text = "Remove all files from list";
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // txtSettings
            // 
            this.txtSettings.Location = new System.Drawing.Point(60, 173);
            this.txtSettings.Name = "txtSettings";
            this.txtSettings.ReadOnly = true;
            this.txtSettings.Size = new System.Drawing.Size(176, 20);
            this.txtSettings.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 137;
            this.label1.Text = "Tools";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgvSlNumbers);
            this.groupBox2.Controls.Add(this.btnRevalidate);
            this.groupBox2.Controls.Add(this.txtValidation);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtAliquotName);
            this.groupBox2.Controls.Add(this.lblHumidity);
            this.groupBox2.Controls.Add(this.txtHumidity);
            this.groupBox2.Controls.Add(this.txtTemperature);
            this.groupBox2.Controls.Add(this.lblTemperature);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lblInstrumentName);
            this.groupBox2.Controls.Add(this.lblCrossheadSpeed);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtValidSpecimens);
            this.groupBox2.Controls.Add(this.txtInstrument);
            this.groupBox2.Controls.Add(this.txtCrossheadSpeed);
            this.groupBox2.Location = new System.Drawing.Point(260, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(860, 225);
            this.groupBox2.TabIndex = 144;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Info for selected file";
            // 
            // btnRevalidate
            // 
            this.btnRevalidate.AutoSize = true;
            this.btnRevalidate.Location = new System.Drawing.Point(513, 77);
            this.btnRevalidate.Name = "btnRevalidate";
            this.btnRevalidate.Size = new System.Drawing.Size(58, 13);
            this.btnRevalidate.TabIndex = 135;
            this.btnRevalidate.TabStop = true;
            this.btnRevalidate.Text = "Revalidate";
            this.btnRevalidate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnRevalidate_LinkClicked);
            // 
            // txtValidation
            // 
            this.txtValidation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValidation.BackColor = System.Drawing.SystemColors.Control;
            this.txtValidation.ForeColor = System.Drawing.Color.DarkRed;
            this.txtValidation.Location = new System.Drawing.Point(406, 94);
            this.txtValidation.Multiline = true;
            this.txtValidation.Name = "txtValidation";
            this.txtValidation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtValidation.Size = new System.Drawing.Size(448, 125);
            this.txtValidation.TabIndex = 134;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 117;
            this.label2.Text = "Aliquot Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(403, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 112;
            this.label3.Text = "Validation messages";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.dgvResults);
            this.groupBox3.Location = new System.Drawing.Point(12, 243);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1111, 360);
            this.groupBox3.TabIndex = 145;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Results for selected file";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lboxAliquots);
            this.groupBox4.Controls.Add(this.txtSettings);
            this.groupBox4.Controls.Add(this.toolStrip1);
            this.groupBox4.Controls.Add(this.lblSettings);
            this.groupBox4.Controls.Add(this.lblAnalyst);
            this.groupBox4.Controls.Add(this.txtAnalystName);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(242, 225);
            this.groupBox4.TabIndex = 145;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Files";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(15, 607);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 147;
            // 
            // btnOverrideValidationFailures
            // 
            this.btnOverrideValidationFailures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOverrideValidationFailures.AutoSize = true;
            this.btnOverrideValidationFailures.Location = new System.Drawing.Point(12, 614);
            this.btnOverrideValidationFailures.Name = "btnOverrideValidationFailures";
            this.btnOverrideValidationFailures.Size = new System.Drawing.Size(135, 13);
            this.btnOverrideValidationFailures.TabIndex = 135;
            this.btnOverrideValidationFailures.TabStop = true;
            this.btnOverrideValidationFailures.Text = "Override Validation Failures";
            this.btnOverrideValidationFailures.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnOverrideValidationFailures_LinkClicked);
            // 
            // txtOverrideValidationFailuresPassword
            // 
            this.txtOverrideValidationFailuresPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtOverrideValidationFailuresPassword.Location = new System.Drawing.Point(186, 611);
            this.txtOverrideValidationFailuresPassword.Name = "txtOverrideValidationFailuresPassword";
            this.txtOverrideValidationFailuresPassword.Size = new System.Drawing.Size(100, 20);
            this.txtOverrideValidationFailuresPassword.TabIndex = 148;
            this.txtOverrideValidationFailuresPassword.UseSystemPasswordChar = true;
            this.txtOverrideValidationFailuresPassword.Visible = false;
            // 
            // btnOverrideValidationFailuresOk
            // 
            this.btnOverrideValidationFailuresOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOverrideValidationFailuresOk.AutoSize = true;
            this.btnOverrideValidationFailuresOk.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOverrideValidationFailuresOk.Location = new System.Drawing.Point(292, 609);
            this.btnOverrideValidationFailuresOk.Name = "btnOverrideValidationFailuresOk";
            this.btnOverrideValidationFailuresOk.Size = new System.Drawing.Size(32, 23);
            this.btnOverrideValidationFailuresOk.TabIndex = 149;
            this.btnOverrideValidationFailuresOk.Text = "OK";
            this.btnOverrideValidationFailuresOk.UseVisualStyleBackColor = true;
            this.btnOverrideValidationFailuresOk.Visible = false;
            this.btnOverrideValidationFailuresOk.Click += new System.EventHandler(this.btnOverrideValidationFailuresOk_Click);
            // 
            // lblOverrideValidationFailuresStatus
            // 
            this.lblOverrideValidationFailuresStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOverrideValidationFailuresStatus.AutoSize = true;
            this.lblOverrideValidationFailuresStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblOverrideValidationFailuresStatus.Location = new System.Drawing.Point(153, 614);
            this.lblOverrideValidationFailuresStatus.Name = "lblOverrideValidationFailuresStatus";
            this.lblOverrideValidationFailuresStatus.Size = new System.Drawing.Size(29, 15);
            this.lblOverrideValidationFailuresStatus.TabIndex = 150;
            this.lblOverrideValidationFailuresStatus.Text = "OFF";
            // 
            // S9AssistantForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 644);
            this.Controls.Add(this.lblOverrideValidationFailuresStatus);
            this.Controls.Add(this.btnOverrideValidationFailuresOk);
            this.Controls.Add(this.txtOverrideValidationFailuresPassword);
            this.Controls.Add(this.btnOverrideValidationFailures);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnSendData);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 560);
            this.Name = "S9AssistantForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "S9Assistant";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSlNumbers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lboxAliquots;
        private System.Windows.Forms.DataGridView dgvSlNumbers;
        private System.Windows.Forms.Label lblAnalyst;
        private System.Windows.Forms.Label lblInstrumentName;
        private System.Windows.Forms.Label lblSettings;
        private System.Windows.Forms.TextBox txtValidSpecimens;
        private System.Windows.Forms.TextBox txtAnalystName;
        private System.Windows.Forms.TextBox txtCrossheadSpeed;
        private System.Windows.Forms.TextBox txtInstrument;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCrossheadSpeed;
        private System.Windows.Forms.Label lblTemperature;
        private System.Windows.Forms.Button btnSendData;
        private System.Windows.Forms.TextBox txtTemperature;
        private System.Windows.Forms.TextBox txtHumidity;
        private System.Windows.Forms.Label lblHumidity;
        private System.Windows.Forms.TextBox txtAliquotName;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnOpenFolder;
        private System.Windows.Forms.ToolStripButton btnOpenFiles;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnDeleteItem;
        private System.Windows.Forms.ToolStripButton btnViewFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ToolStripButton btnDeleteAll;
        private System.Windows.Forms.TextBox txtSettings;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtValidation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel btnRevalidate;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.LinkLabel btnOverrideValidationFailures;
        private System.Windows.Forms.TextBox txtOverrideValidationFailuresPassword;
        private System.Windows.Forms.Button btnOverrideValidationFailuresOk;
        private System.Windows.Forms.Label lblOverrideValidationFailuresStatus;
    }
}