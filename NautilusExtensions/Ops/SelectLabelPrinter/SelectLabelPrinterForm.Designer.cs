namespace NautilusExtensions.Ops {
    partial class SelectLabelPrinterForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectLabelPrinterForm));
            this.lvPrinterDestinations = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.chkCustomDestination = new System.Windows.Forms.CheckBox();
            this.txtCustomDestination = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvPrinterDestinations
            // 
            this.lvPrinterDestinations.AllowColumnReorder = true;
            this.lvPrinterDestinations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvPrinterDestinations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvPrinterDestinations.FullRowSelect = true;
            this.lvPrinterDestinations.LargeImageList = this.imageList1;
            this.lvPrinterDestinations.Location = new System.Drawing.Point(12, 29);
            this.lvPrinterDestinations.MultiSelect = false;
            this.lvPrinterDestinations.Name = "lvPrinterDestinations";
            this.lvPrinterDestinations.Size = new System.Drawing.Size(421, 175);
            this.lvPrinterDestinations.SmallImageList = this.imageList1;
            this.lvPrinterDestinations.TabIndex = 0;
            this.lvPrinterDestinations.UseCompatibleStateImageBehavior = false;
            this.lvPrinterDestinations.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Description";
            this.columnHeader2.Width = 150;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Address";
            this.columnHeader3.Width = 150;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Printer.ico");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select a printer to use from Nautilus destinations:";
            // 
            // chkCustomDestination
            // 
            this.chkCustomDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkCustomDestination.AutoSize = true;
            this.chkCustomDestination.Location = new System.Drawing.Point(13, 220);
            this.chkCustomDestination.Name = "chkCustomDestination";
            this.chkCustomDestination.Size = new System.Drawing.Size(138, 17);
            this.chkCustomDestination.TabIndex = 2;
            this.chkCustomDestination.Text = "Specify custom address";
            this.chkCustomDestination.UseVisualStyleBackColor = true;
            this.chkCustomDestination.CheckedChanged += new System.EventHandler(this.chkCustomDestination_CheckedChanged);
            // 
            // txtCustomDestination
            // 
            this.txtCustomDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtCustomDestination.Location = new System.Drawing.Point(157, 220);
            this.txtCustomDestination.Name = "txtCustomDestination";
            this.txtCustomDestination.ReadOnly = true;
            this.txtCustomDestination.Size = new System.Drawing.Size(112, 20);
            this.txtCustomDestination.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(359, 252);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(278, 252);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // SelectLabelPrinterForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(446, 287);
            this.ControlBox = false;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtCustomDestination);
            this.Controls.Add(this.chkCustomDestination);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvPrinterDestinations);
            this.Name = "SelectLabelPrinterForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Select Label Printer";
            this.Load += new System.EventHandler(this.SelectLabelPrinterForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvPrinterDestinations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkCustomDestination;
        private System.Windows.Forms.TextBox txtCustomDestination;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ImageList imageList1;
    }
}