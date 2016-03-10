namespace NautilusExtensions.Ops {
    partial class ClientTestSelector {
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
            this.dgvTests = new System.Windows.Forms.DataGridView();
            this.btnAddTest = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.cmbTestsToAdd = new System.Windows.Forms.ComboBox();
            this.chkPrimary = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTests)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTests
            // 
            this.dgvTests.AllowUserToAddRows = false;
            this.dgvTests.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTests.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvTests.Location = new System.Drawing.Point(0, 38);
            this.dgvTests.Name = "dgvTests";
            this.dgvTests.ReadOnly = true;
            this.dgvTests.Size = new System.Drawing.Size(344, 390);
            this.dgvTests.TabIndex = 0;
            // 
            // btnAddTest
            // 
            this.btnAddTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddTest.AutoSize = true;
            this.btnAddTest.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddTest.Location = new System.Drawing.Point(284, 9);
            this.btnAddTest.Name = "btnAddTest";
            this.btnAddTest.Size = new System.Drawing.Size(60, 23);
            this.btnAddTest.TabIndex = 1;
            this.btnAddTest.Text = "Add Test";
            this.btnAddTest.UseVisualStyleBackColor = true;
            this.btnAddTest.Click += new System.EventHandler(this.btnAddTest_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.Location = new System.Drawing.Point(3, 434);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Delete Selected";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // cmbTestsToAdd
            // 
            this.cmbTestsToAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTestsToAdd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTestsToAdd.FormattingEnabled = true;
            this.cmbTestsToAdd.Location = new System.Drawing.Point(3, 11);
            this.cmbTestsToAdd.Name = "cmbTestsToAdd";
            this.cmbTestsToAdd.Size = new System.Drawing.Size(209, 21);
            this.cmbTestsToAdd.TabIndex = 2;
            // 
            // chkPrimary
            // 
            this.chkPrimary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPrimary.AutoSize = true;
            this.chkPrimary.Location = new System.Drawing.Point(219, 14);
            this.chkPrimary.Name = "chkPrimary";
            this.chkPrimary.Size = new System.Drawing.Size(60, 17);
            this.chkPrimary.TabIndex = 3;
            this.chkPrimary.Text = "Primary";
            this.chkPrimary.UseVisualStyleBackColor = true;
            // 
            // ClientTestSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkPrimary);
            this.Controls.Add(this.cmbTestsToAdd);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnAddTest);
            this.Controls.Add(this.dgvTests);
            this.Name = "ClientTestSelector";
            this.Size = new System.Drawing.Size(344, 460);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTests)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTests;
        private System.Windows.Forms.Button btnAddTest;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cmbTestsToAdd;
        private System.Windows.Forms.CheckBox chkPrimary;
    }
}
