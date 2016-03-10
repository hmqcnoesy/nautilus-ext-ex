namespace NautilusExtensions.Qa {
    partial class ModifyMetricDatesForm {
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.dtpAuthorisedOn = new System.Windows.Forms.DateTimePicker();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.lblReceivedOn = new System.Windows.Forms.Label();
            this.dtpReceivedOn = new System.Windows.Forms.DateTimePicker();
            this.lblAuthorisedOn = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Modifying:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(100, 13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(0, 13);
            this.lblName.TabIndex = 1;
            // 
            // dtpAuthorisedOn
            // 
            this.dtpAuthorisedOn.CustomFormat = "dd-MMM-yyyy   HH:mm:ss";
            this.dtpAuthorisedOn.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAuthorisedOn.Location = new System.Drawing.Point(96, 66);
            this.dtpAuthorisedOn.Name = "dtpAuthorisedOn";
            this.dtpAuthorisedOn.Size = new System.Drawing.Size(184, 20);
            this.dtpAuthorisedOn.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(205, 163);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(124, 163);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Session info will be recorded with reason:";
            // 
            // txtReason
            // 
            this.txtReason.Location = new System.Drawing.Point(16, 131);
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(264, 20);
            this.txtReason.TabIndex = 2;
            // 
            // lblReceivedOn
            // 
            this.lblReceivedOn.AutoSize = true;
            this.lblReceivedOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReceivedOn.Location = new System.Drawing.Point(13, 44);
            this.lblReceivedOn.Name = "lblReceivedOn";
            this.lblReceivedOn.Size = new System.Drawing.Size(73, 13);
            this.lblReceivedOn.TabIndex = 0;
            this.lblReceivedOn.Text = "Received On:";
            // 
            // dtpReceivedOn
            // 
            this.dtpReceivedOn.CustomFormat = "dd-MMM-yyyy   HH:mm:ss";
            this.dtpReceivedOn.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceivedOn.Location = new System.Drawing.Point(96, 40);
            this.dtpReceivedOn.Name = "dtpReceivedOn";
            this.dtpReceivedOn.Size = new System.Drawing.Size(184, 20);
            this.dtpReceivedOn.TabIndex = 1;
            // 
            // lblAuthorisedOn
            // 
            this.lblAuthorisedOn.AutoSize = true;
            this.lblAuthorisedOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthorisedOn.Location = new System.Drawing.Point(13, 70);
            this.lblAuthorisedOn.Name = "lblAuthorisedOn";
            this.lblAuthorisedOn.Size = new System.Drawing.Size(77, 13);
            this.lblAuthorisedOn.TabIndex = 0;
            this.lblAuthorisedOn.Text = "Authorised On:";
            // 
            // ModifyMetricDatesForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 198);
            this.ControlBox = false;
            this.Controls.Add(this.txtReason);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dtpReceivedOn);
            this.Controls.Add(this.dtpAuthorisedOn);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblAuthorisedOn);
            this.Controls.Add(this.lblReceivedOn);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModifyMetricDatesForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Modify Metric Dates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.DateTimePicker dtpAuthorisedOn;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.Label lblReceivedOn;
        private System.Windows.Forms.DateTimePicker dtpReceivedOn;
        private System.Windows.Forms.Label lblAuthorisedOn;
    }
}