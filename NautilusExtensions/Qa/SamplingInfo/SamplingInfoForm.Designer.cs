namespace NautilusExtensions.Qa {
    partial class SamplingInfoForm {
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
            this.chkNeedsAttention = new System.Windows.Forms.CheckBox();
            this.Location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvContainers = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Container = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtContainerType = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtQtyUm = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.txtMiscNotes = new System.Windows.Forms.TextBox();
            this.txtStoresLocation = new System.Windows.Forms.TextBox();
            this.txtPoNumber = new System.Windows.Forms.TextBox();
            this.txtSl = new System.Windows.Forms.TextBox();
            this.txtPdlNoRev = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDamage = new System.Windows.Forms.TextBox();
            this.txtVendor = new System.Windows.Forms.TextBox();
            this.txtVendorLot = new System.Windows.Forms.TextBox();
            this.txtColor = new System.Windows.Forms.TextBox();
            this.txtWeight = new System.Windows.Forms.TextBox();
            this.txtType = new System.Windows.Forms.TextBox();
            this.txtDoM = new System.Windows.Forms.TextBox();
            this.txtScnCn = new System.Windows.Forms.TextBox();
            this.txtSpecRev = new System.Windows.Forms.TextBox();
            this.txtLabLocation = new System.Windows.Forms.TextBox();
            this.txtMaterialName = new System.Windows.Forms.TextBox();
            this.txtReceiver = new System.Windows.Forms.TextBox();
            this.btnCreateRecord = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.pbStatus = new System.Windows.Forms.PictureBox();
            this.lblSampleName = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContainers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // chkNeedsAttention
            // 
            this.chkNeedsAttention.AutoSize = true;
            this.chkNeedsAttention.Location = new System.Drawing.Point(355, 252);
            this.chkNeedsAttention.Name = "chkNeedsAttention";
            this.chkNeedsAttention.Size = new System.Drawing.Size(102, 17);
            this.chkNeedsAttention.TabIndex = 59;
            this.chkNeedsAttention.Text = "Needs Attention";
            this.chkNeedsAttention.UseVisualStyleBackColor = true;
            // 
            // Location
            // 
            this.Location.HeaderText = "Location";
            this.Location.Name = "Location";
            this.Location.Width = 120;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dgvContainers);
            this.groupBox1.Controls.Add(this.txtContainerType);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.txtQtyUm);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Location = new System.Drawing.Point(12, 393);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(446, 169);
            this.groupBox1.TabIndex = 58;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Containers";
            // 
            // dgvContainers
            // 
            this.dgvContainers.AllowUserToDeleteRows = false;
            this.dgvContainers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvContainers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvContainers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Container,
            this.Quantity,
            this.Location});
            this.dgvContainers.Location = new System.Drawing.Point(6, 46);
            this.dgvContainers.Name = "dgvContainers";
            this.dgvContainers.Size = new System.Drawing.Size(434, 117);
            this.dgvContainers.TabIndex = 21;
            this.dgvContainers.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvContainers_CellBeginEdit);
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // Container
            // 
            this.Container.HeaderText = "Container";
            this.Container.Name = "Container";
            this.Container.Width = 120;
            // 
            // Quantity
            // 
            this.Quantity.HeaderText = "Quantity";
            this.Quantity.Name = "Quantity";
            this.Quantity.Width = 120;
            // 
            // txtContainerType
            // 
            this.txtContainerType.Location = new System.Drawing.Point(93, 19);
            this.txtContainerType.Name = "txtContainerType";
            this.txtContainerType.Size = new System.Drawing.Size(100, 20);
            this.txtContainerType.TabIndex = 19;
            this.txtContainerType.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 22);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(79, 13);
            this.label19.TabIndex = 2;
            this.label19.Text = "Container Type";
            // 
            // txtQtyUm
            // 
            this.txtQtyUm.Location = new System.Drawing.Point(340, 19);
            this.txtQtyUm.Name = "txtQtyUm";
            this.txtQtyUm.Size = new System.Drawing.Size(100, 20);
            this.txtQtyUm.TabIndex = 20;
            this.txtQtyUm.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(239, 22);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(45, 13);
            this.label20.TabIndex = 2;
            this.label20.Text = "Qty/UM";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(11, 337);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(56, 13);
            this.label18.TabIndex = 37;
            this.label18.Text = "Comments";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(11, 278);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(62, 13);
            this.label17.TabIndex = 40;
            this.label17.Text = "Misc/Notes";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(251, 200);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(81, 13);
            this.label16.TabIndex = 38;
            this.label16.Text = "Stores Location";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(251, 174);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(62, 13);
            this.label15.TabIndex = 39;
            this.label15.Text = "PO Number";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(251, 148);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(25, 13);
            this.label14.TabIndex = 36;
            this.label14.Text = "S/L";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(251, 122);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 33;
            this.label13.Text = "PDL No/Rev";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(251, 96);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 13);
            this.label12.TabIndex = 34;
            this.label12.Text = "SCN/CN";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(251, 70);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 13);
            this.label11.TabIndex = 35;
            this.label11.Text = "Spec/Rev";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(251, 44);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 13);
            this.label10.TabIndex = 41;
            this.label10.Text = "Material Name";
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(105, 334);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(353, 53);
            this.txtComments.TabIndex = 57;
            this.txtComments.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtMiscNotes
            // 
            this.txtMiscNotes.Location = new System.Drawing.Point(105, 275);
            this.txtMiscNotes.Multiline = true;
            this.txtMiscNotes.Name = "txtMiscNotes";
            this.txtMiscNotes.Size = new System.Drawing.Size(353, 53);
            this.txtMiscNotes.TabIndex = 56;
            this.txtMiscNotes.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtStoresLocation
            // 
            this.txtStoresLocation.Location = new System.Drawing.Point(358, 197);
            this.txtStoresLocation.Name = "txtStoresLocation";
            this.txtStoresLocation.Size = new System.Drawing.Size(100, 20);
            this.txtStoresLocation.TabIndex = 55;
            this.txtStoresLocation.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtPoNumber
            // 
            this.txtPoNumber.Location = new System.Drawing.Point(358, 171);
            this.txtPoNumber.Name = "txtPoNumber";
            this.txtPoNumber.Size = new System.Drawing.Size(100, 20);
            this.txtPoNumber.TabIndex = 54;
            this.txtPoNumber.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtSl
            // 
            this.txtSl.Location = new System.Drawing.Point(358, 145);
            this.txtSl.Name = "txtSl";
            this.txtSl.Size = new System.Drawing.Size(100, 20);
            this.txtSl.TabIndex = 53;
            this.txtSl.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtPdlNoRev
            // 
            this.txtPdlNoRev.Location = new System.Drawing.Point(358, 119);
            this.txtPdlNoRev.Name = "txtPdlNoRev";
            this.txtPdlNoRev.Size = new System.Drawing.Size(100, 20);
            this.txtPdlNoRev.TabIndex = 52;
            this.txtPdlNoRev.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 252);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Damage";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 226);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Vendor";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 200);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Vendor Lot";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Color";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Weight";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "DOM";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Lab Location";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Receiver";
            // 
            // txtDamage
            // 
            this.txtDamage.Location = new System.Drawing.Point(105, 249);
            this.txtDamage.Name = "txtDamage";
            this.txtDamage.Size = new System.Drawing.Size(100, 20);
            this.txtDamage.TabIndex = 48;
            this.txtDamage.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtVendor
            // 
            this.txtVendor.Location = new System.Drawing.Point(105, 223);
            this.txtVendor.Name = "txtVendor";
            this.txtVendor.Size = new System.Drawing.Size(100, 20);
            this.txtVendor.TabIndex = 47;
            this.txtVendor.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtVendorLot
            // 
            this.txtVendorLot.Location = new System.Drawing.Point(105, 197);
            this.txtVendorLot.Name = "txtVendorLot";
            this.txtVendorLot.Size = new System.Drawing.Size(100, 20);
            this.txtVendorLot.TabIndex = 46;
            this.txtVendorLot.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtColor
            // 
            this.txtColor.Location = new System.Drawing.Point(105, 171);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(100, 20);
            this.txtColor.TabIndex = 45;
            this.txtColor.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtWeight
            // 
            this.txtWeight.Location = new System.Drawing.Point(105, 145);
            this.txtWeight.Name = "txtWeight";
            this.txtWeight.Size = new System.Drawing.Size(100, 20);
            this.txtWeight.TabIndex = 44;
            this.txtWeight.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtType
            // 
            this.txtType.Location = new System.Drawing.Point(105, 119);
            this.txtType.Name = "txtType";
            this.txtType.Size = new System.Drawing.Size(100, 20);
            this.txtType.TabIndex = 43;
            this.txtType.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtDoM
            // 
            this.txtDoM.Location = new System.Drawing.Point(105, 93);
            this.txtDoM.Name = "txtDoM";
            this.txtDoM.Size = new System.Drawing.Size(100, 20);
            this.txtDoM.TabIndex = 42;
            this.txtDoM.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtScnCn
            // 
            this.txtScnCn.Location = new System.Drawing.Point(358, 93);
            this.txtScnCn.Name = "txtScnCn";
            this.txtScnCn.Size = new System.Drawing.Size(100, 20);
            this.txtScnCn.TabIndex = 51;
            this.txtScnCn.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtSpecRev
            // 
            this.txtSpecRev.Location = new System.Drawing.Point(358, 67);
            this.txtSpecRev.Name = "txtSpecRev";
            this.txtSpecRev.Size = new System.Drawing.Size(100, 20);
            this.txtSpecRev.TabIndex = 50;
            this.txtSpecRev.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtLabLocation
            // 
            this.txtLabLocation.Location = new System.Drawing.Point(105, 67);
            this.txtLabLocation.Name = "txtLabLocation";
            this.txtLabLocation.Size = new System.Drawing.Size(100, 20);
            this.txtLabLocation.TabIndex = 29;
            this.txtLabLocation.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtMaterialName
            // 
            this.txtMaterialName.Location = new System.Drawing.Point(358, 41);
            this.txtMaterialName.Name = "txtMaterialName";
            this.txtMaterialName.Size = new System.Drawing.Size(100, 20);
            this.txtMaterialName.TabIndex = 49;
            this.txtMaterialName.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // txtReceiver
            // 
            this.txtReceiver.Location = new System.Drawing.Point(105, 41);
            this.txtReceiver.Name = "txtReceiver";
            this.txtReceiver.Size = new System.Drawing.Size(100, 20);
            this.txtReceiver.TabIndex = 22;
            this.txtReceiver.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // btnCreateRecord
            // 
            this.btnCreateRecord.AutoSize = true;
            this.btnCreateRecord.Location = new System.Drawing.Point(328, 10);
            this.btnCreateRecord.Name = "btnCreateRecord";
            this.btnCreateRecord.Size = new System.Drawing.Size(132, 23);
            this.btnCreateRecord.TabIndex = 21;
            this.btnCreateRecord.TabStop = false;
            this.btnCreateRecord.Text = "Create Sampling Record";
            this.btnCreateRecord.UseVisualStyleBackColor = true;
            this.btnCreateRecord.Click += new System.EventHandler(this.btnCreateRecord_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(382, 569);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 60;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(301, 569);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 60;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // pbStatus
            // 
            this.pbStatus.ImageLocation = "";
            this.pbStatus.Location = new System.Drawing.Point(12, 12);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(16, 16);
            this.pbStatus.TabIndex = 61;
            this.pbStatus.TabStop = false;
            // 
            // lblSampleName
            // 
            this.lblSampleName.AutoSize = true;
            this.lblSampleName.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lblSampleName.Location = new System.Drawing.Point(34, 15);
            this.lblSampleName.Name = "lblSampleName";
            this.lblSampleName.Size = new System.Drawing.Size(73, 13);
            this.lblSampleName.TabIndex = 62;
            this.lblSampleName.TabStop = true;
            this.lblSampleName.Text = "Sample Name";
            this.lblSampleName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSampleName_LinkClicked);
            // 
            // SamplingInfoForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(472, 604);
            this.Controls.Add(this.lblSampleName);
            this.Controls.Add(this.pbStatus);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.chkNeedsAttention);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.txtMiscNotes);
            this.Controls.Add(this.txtStoresLocation);
            this.Controls.Add(this.txtPoNumber);
            this.Controls.Add(this.txtSl);
            this.Controls.Add(this.txtPdlNoRev);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDamage);
            this.Controls.Add(this.txtVendor);
            this.Controls.Add(this.txtVendorLot);
            this.Controls.Add(this.txtColor);
            this.Controls.Add(this.txtWeight);
            this.Controls.Add(this.txtType);
            this.Controls.Add(this.txtDoM);
            this.Controls.Add(this.txtScnCn);
            this.Controls.Add(this.txtSpecRev);
            this.Controls.Add(this.txtLabLocation);
            this.Controls.Add(this.txtMaterialName);
            this.Controls.Add(this.txtReceiver);
            this.Controls.Add(this.btnCreateRecord);
            this.Name = "SamplingInfoForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SamplingInfoForm";
            this.Load += new System.EventHandler(this.SamplingInfoForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContainers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkNeedsAttention;
        private System.Windows.Forms.DataGridViewTextBoxColumn Location;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvContainers;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Container;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.TextBox txtContainerType;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtQtyUm;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.TextBox txtMiscNotes;
        private System.Windows.Forms.TextBox txtStoresLocation;
        private System.Windows.Forms.TextBox txtPoNumber;
        private System.Windows.Forms.TextBox txtSl;
        private System.Windows.Forms.TextBox txtPdlNoRev;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDamage;
        private System.Windows.Forms.TextBox txtVendor;
        private System.Windows.Forms.TextBox txtVendorLot;
        private System.Windows.Forms.TextBox txtColor;
        private System.Windows.Forms.TextBox txtWeight;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.TextBox txtDoM;
        private System.Windows.Forms.TextBox txtScnCn;
        private System.Windows.Forms.TextBox txtSpecRev;
        private System.Windows.Forms.TextBox txtLabLocation;
        private System.Windows.Forms.TextBox txtMaterialName;
        private System.Windows.Forms.TextBox txtReceiver;
        private System.Windows.Forms.Button btnCreateRecord;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.PictureBox pbStatus;
        private System.Windows.Forms.LinkLabel lblSampleName;
    }
}