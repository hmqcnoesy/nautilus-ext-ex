namespace NautilusExtensions.Ops {
    partial class FixTurnaroundTimeForm {
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
            this.dtpReceivedOn = new System.Windows.Forms.DateTimePicker();
            this.dtpAuthorisedOn = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTurnaroundTime = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSampleName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sample Received:";
            // 
            // dtpReceivedOn
            // 
            this.dtpReceivedOn.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dtpReceivedOn.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceivedOn.Location = new System.Drawing.Point(117, 44);
            this.dtpReceivedOn.Name = "dtpReceivedOn";
            this.dtpReceivedOn.Size = new System.Drawing.Size(148, 20);
            this.dtpReceivedOn.TabIndex = 1;
            this.dtpReceivedOn.Value = new System.DateTime(2008, 11, 25, 18, 24, 0, 0);
            this.dtpReceivedOn.ValueChanged += new System.EventHandler(this.dtp_ValueChanged);
            // 
            // dtpAuthorisedOn
            // 
            this.dtpAuthorisedOn.CustomFormat = "MM/dd/yyyy HH:mm:ss";
            this.dtpAuthorisedOn.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAuthorisedOn.Location = new System.Drawing.Point(117, 70);
            this.dtpAuthorisedOn.Name = "dtpAuthorisedOn";
            this.dtpAuthorisedOn.Size = new System.Drawing.Size(148, 20);
            this.dtpAuthorisedOn.TabIndex = 1;
            this.dtpAuthorisedOn.Value = new System.DateTime(2008, 11, 25, 18, 24, 0, 0);
            this.dtpAuthorisedOn.ValueChanged += new System.EventHandler(this.dtp_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Sample Authorised:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Turnaround Time:";
            // 
            // txtTurnaroundTime
            // 
            this.txtTurnaroundTime.Location = new System.Drawing.Point(117, 96);
            this.txtTurnaroundTime.Name = "txtTurnaroundTime";
            this.txtTurnaroundTime.ReadOnly = true;
            this.txtTurnaroundTime.Size = new System.Drawing.Size(148, 20);
            this.txtTurnaroundTime.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(190, 135);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(109, 135);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Adjusting times for ";
            // 
            // lblSampleName
            // 
            this.lblSampleName.AutoSize = true;
            this.lblSampleName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSampleName.Location = new System.Drawing.Point(114, 9);
            this.lblSampleName.Name = "lblSampleName";
            this.lblSampleName.Size = new System.Drawing.Size(39, 13);
            this.lblSampleName.TabIndex = 0;
            this.lblSampleName.Text = "Name";
            // 
            // FixTurnaroundTimeForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(277, 170);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtTurnaroundTime);
            this.Controls.Add(this.dtpAuthorisedOn);
            this.Controls.Add(this.dtpReceivedOn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblSampleName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FixTurnaroundTimeForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Fix Turnaround Time";
            this.Load += new System.EventHandler(this.FixTurnaroundTimeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpReceivedOn;
        private System.Windows.Forms.DateTimePicker dtpAuthorisedOn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTurnaroundTime;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSampleName;
    }
}