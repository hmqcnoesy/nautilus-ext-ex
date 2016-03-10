namespace NautilusExtensions.Qa {
    partial class WorkflowFinderForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkflowFinderForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkIncludeAliquot = new System.Windows.Forms.CheckBox();
            this.chkIncludeTrashed = new System.Windows.Forms.CheckBox();
            this.btnFindWorkflows = new System.Windows.Forms.Button();
            this.btnClearFields = new System.Windows.Forms.Button();
            this.txtWorkflowName = new System.Windows.Forms.TextBox();
            this.txtWorkflowDescription = new System.Windows.Forms.TextBox();
            this.lstTests = new System.Windows.Forms.CheckedListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tvWorkflowTree = new System.Windows.Forms.TreeView();
            this.lblWorkflowId = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lvMatchingWorkflows = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.imageListWorkflows = new System.Windows.Forms.ImageList(this.components);
            this.imageListNodes = new System.Windows.Forms.ImageList(this.components);
            this.btnExpandNodes = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(267, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input one or more of the fields below to find a workflow.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Workflow name contains:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Workflow description contains:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(167, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Workflow has these tests:";
            // 
            // chkIncludeAliquot
            // 
            this.chkIncludeAliquot.AutoSize = true;
            this.chkIncludeAliquot.Location = new System.Drawing.Point(15, 138);
            this.chkIncludeAliquot.Name = "chkIncludeAliquot";
            this.chkIncludeAliquot.Size = new System.Drawing.Size(145, 17);
            this.chkIncludeAliquot.TabIndex = 3;
            this.chkIncludeAliquot.Text = "Include aliquot workflows";
            this.chkIncludeAliquot.UseVisualStyleBackColor = true;
            // 
            // chkIncludeTrashed
            // 
            this.chkIncludeTrashed.AutoSize = true;
            this.chkIncludeTrashed.Location = new System.Drawing.Point(15, 161);
            this.chkIncludeTrashed.Name = "chkIncludeTrashed";
            this.chkIncludeTrashed.Size = new System.Drawing.Size(149, 17);
            this.chkIncludeTrashed.TabIndex = 4;
            this.chkIncludeTrashed.Text = "Include trashed workflows";
            this.chkIncludeTrashed.UseVisualStyleBackColor = true;
            // 
            // btnFindWorkflows
            // 
            this.btnFindWorkflows.Location = new System.Drawing.Point(12, 184);
            this.btnFindWorkflows.Name = "btnFindWorkflows";
            this.btnFindWorkflows.Size = new System.Drawing.Size(89, 37);
            this.btnFindWorkflows.TabIndex = 5;
            this.btnFindWorkflows.Text = "Find Matching Workflows";
            this.btnFindWorkflows.UseVisualStyleBackColor = true;
            this.btnFindWorkflows.Click += new System.EventHandler(this.btnFindWorkflows_Click);
            // 
            // btnClearFields
            // 
            this.btnClearFields.Location = new System.Drawing.Point(107, 184);
            this.btnClearFields.Name = "btnClearFields";
            this.btnClearFields.Size = new System.Drawing.Size(50, 37);
            this.btnClearFields.TabIndex = 6;
            this.btnClearFields.Text = "Clear Fields";
            this.btnClearFields.UseVisualStyleBackColor = true;
            this.btnClearFields.Click += new System.EventHandler(this.btnClearFields_Click);
            // 
            // txtWorkflowName
            // 
            this.txtWorkflowName.Location = new System.Drawing.Point(15, 63);
            this.txtWorkflowName.Name = "txtWorkflowName";
            this.txtWorkflowName.Size = new System.Drawing.Size(149, 20);
            this.txtWorkflowName.TabIndex = 1;
            // 
            // txtWorkflowDescription
            // 
            this.txtWorkflowDescription.Location = new System.Drawing.Point(15, 112);
            this.txtWorkflowDescription.Name = "txtWorkflowDescription";
            this.txtWorkflowDescription.Size = new System.Drawing.Size(149, 20);
            this.txtWorkflowDescription.TabIndex = 2;
            // 
            // lstTests
            // 
            this.lstTests.CheckOnClick = true;
            this.lstTests.FormattingEnabled = true;
            this.lstTests.Location = new System.Drawing.Point(170, 63);
            this.lstTests.Name = "lstTests";
            this.lstTests.Size = new System.Drawing.Size(257, 154);
            this.lstTests.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 240);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(172, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Matching workflows (click to view):";
            // 
            // tvWorkflowTree
            // 
            this.tvWorkflowTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvWorkflowTree.ImageIndex = 73;
            this.tvWorkflowTree.ImageList = this.imageListNodes;
            this.tvWorkflowTree.Location = new System.Drawing.Point(433, 63);
            this.tvWorkflowTree.Name = "tvWorkflowTree";
            this.tvWorkflowTree.SelectedImageIndex = 73;
            this.tvWorkflowTree.Size = new System.Drawing.Size(307, 378);
            this.tvWorkflowTree.TabIndex = 9;
            this.tvWorkflowTree.TabStop = false;
            // 
            // lblWorkflowId
            // 
            this.lblWorkflowId.Location = new System.Drawing.Point(430, 13);
            this.lblWorkflowId.Name = "lblWorkflowId";
            this.lblWorkflowId.Size = new System.Drawing.Size(66, 13);
            this.lblWorkflowId.TabIndex = 8;
            this.lblWorkflowId.Text = "Workflow ID";
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(430, 26);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(281, 31);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "Description";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(502, 13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 8;
            this.lblName.Text = "Name";
            // 
            // lvMatchingWorkflows
            // 
            this.lvMatchingWorkflows.AllowColumnReorder = true;
            this.lvMatchingWorkflows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvMatchingWorkflows.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvMatchingWorkflows.FullRowSelect = true;
            this.lvMatchingWorkflows.HideSelection = false;
            this.lvMatchingWorkflows.LargeImageList = this.imageListWorkflows;
            this.lvMatchingWorkflows.Location = new System.Drawing.Point(13, 257);
            this.lvMatchingWorkflows.MultiSelect = false;
            this.lvMatchingWorkflows.Name = "lvMatchingWorkflows";
            this.lvMatchingWorkflows.Size = new System.Drawing.Size(414, 184);
            this.lvMatchingWorkflows.SmallImageList = this.imageListWorkflows;
            this.lvMatchingWorkflows.TabIndex = 8;
            this.lvMatchingWorkflows.TabStop = false;
            this.lvMatchingWorkflows.UseCompatibleStateImageBehavior = false;
            this.lvMatchingWorkflows.View = System.Windows.Forms.View.Details;
            this.lvMatchingWorkflows.SelectedIndexChanged += new System.EventHandler(this.lvMatchingWorkflows_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 148;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ID";
            this.columnHeader2.Width = 66;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Description";
            this.columnHeader3.Width = 155;
            // 
            // imageListWorkflows
            // 
            this.imageListWorkflows.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListWorkflows.ImageStream")));
            this.imageListWorkflows.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListWorkflows.Images.SetKeyName(0, "1");
            this.imageListWorkflows.Images.SetKeyName(1, "34");
            // 
            // imageListNodes
            // 
            this.imageListNodes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListNodes.ImageStream")));
            this.imageListNodes.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListNodes.Images.SetKeyName(0, "aliquot_analyte.ico");
            this.imageListNodes.Images.SetKeyName(1, "analyte.ico");
            this.imageListNodes.Images.SetKeyName(2, "argument.ico");
            this.imageListNodes.Images.SetKeyName(3, "argument_aliquot.ico");
            this.imageListNodes.Images.SetKeyName(4, "argument_av.ico");
            this.imageListNodes.Images.SetKeyName(5, "argument_chemical.ico");
            this.imageListNodes.Images.SetKeyName(6, "argument_rlp.ico");
            this.imageListNodes.Images.SetKeyName(7, "argument_rsa.ico");
            this.imageListNodes.Images.SetKeyName(8, "argument_rsp.ico");
            this.imageListNodes.Images.SetKeyName(9, "argument_rst.ico");
            this.imageListNodes.Images.SetKeyName(10, "calculation.ico");
            this.imageListNodes.Images.SetKeyName(11, "client.ico");
            this.imageListNodes.Images.SetKeyName(12, "comment.ico");
            this.imageListNodes.Images.SetKeyName(13, "debug.ico");
            this.imageListNodes.Images.SetKeyName(14, "destination.ico");
            this.imageListNodes.Images.SetKeyName(15, "extension.ico");
            this.imageListNodes.Images.SetKeyName(16, "extensionchoice.ico");
            this.imageListNodes.Images.SetKeyName(17, "field_argument.ico");
            this.imageListNodes.Images.SetKeyName(18, "field_argument_field.ico");
            this.imageListNodes.Images.SetKeyName(19, "filter.ico");
            this.imageListNodes.Images.SetKeyName(20, "instrument.ico");
            this.imageListNodes.Images.SetKeyName(21, "order_plate_template.ico");
            this.imageListNodes.Images.SetKeyName(22, "plate cherryp.ico");
            this.imageListNodes.Images.SetKeyName(23, "plate compress.ico");
            this.imageListNodes.Images.SetKeyName(24, "plate fill mode.ico");
            this.imageListNodes.Images.SetKeyName(25, "plate merge.ico");
            this.imageListNodes.Images.SetKeyName(26, "plate probe.ico");
            this.imageListNodes.Images.SetKeyName(27, "plate replicate.ico");
            this.imageListNodes.Images.SetKeyName(28, "plate split.ico");
            this.imageListNodes.Images.SetKeyName(29, "product.ico");
            this.imageListNodes.Images.SetKeyName(30, "report.ico");
            this.imageListNodes.Images.SetKeyName(31, "study.ico");
            this.imageListNodes.Images.SetKeyName(32, "workflow.ico");
            this.imageListNodes.Images.SetKeyName(33, "workflow_aliquot.ico");
            this.imageListNodes.Images.SetKeyName(34, "workflow_aliquot_list.ico");
            this.imageListNodes.Images.SetKeyName(35, "workflow_assign_date.ico");
            this.imageListNodes.Images.SetKeyName(36, "workflow_assign_operator.ico");
            this.imageListNodes.Images.SetKeyName(37, "workflow_assign_role.ico");
            this.imageListNodes.Images.SetKeyName(38, "workflow_broadcast.ico");
            this.imageListNodes.Images.SetKeyName(39, "workflow_choice.ico");
            this.imageListNodes.Images.SetKeyName(40, "workflow_else.ico");
            this.imageListNodes.Images.SetKeyName(41, "workflow_event.ico");
            this.imageListNodes.Images.SetKeyName(42, "workflow_execution.ico");
            this.imageListNodes.Images.SetKeyName(43, "workflow_false.ico");
            this.imageListNodes.Images.SetKeyName(44, "workflow_field_assignment.ico");
            this.imageListNodes.Images.SetKeyName(45, "workflow_field_calculation.ico");
            this.imageListNodes.Images.SetKeyName(46, "workflow_field_copy.ico");
            this.imageListNodes.Images.SetKeyName(47, "workflow_message.ico");
            this.imageListNodes.Images.SetKeyName(48, "workflow_outcome.ico");
            this.imageListNodes.Images.SetKeyName(49, "workflow_plate.ico");
            this.imageListNodes.Images.SetKeyName(50, "workflow_plate_filter.ico");
            this.imageListNodes.Images.SetKeyName(51, "workflow_plate_plan.ico");
            this.imageListNodes.Images.SetKeyName(52, "workflow_prompted_choice.ico");
            this.imageListNodes.Images.SetKeyName(53, "workflow_prompted_selection.ico");
            this.imageListNodes.Images.SetKeyName(54, "workflow_propagate_authorisation.ico");
            this.imageListNodes.Images.SetKeyName(55, "workflow_repeat.ico");
            this.imageListNodes.Images.SetKeyName(56, "workflow_result.ico");
            this.imageListNodes.Images.SetKeyName(57, "workflow_result_list.ico");
            this.imageListNodes.Images.SetKeyName(58, "workflow_sample.ico");
            this.imageListNodes.Images.SetKeyName(59, "workflow_sample_list.ico");
            this.imageListNodes.Images.SetKeyName(60, "workflow_schedule.ico");
            this.imageListNodes.Images.SetKeyName(61, "workflow_sdg.ico");
            this.imageListNodes.Images.SetKeyName(62, "workflow_selection.ico");
            this.imageListNodes.Images.SetKeyName(63, "workflow_setup.ico");
            this.imageListNodes.Images.SetKeyName(64, "workflow_specification.ico");
            this.imageListNodes.Images.SetKeyName(65, "workflow_split.ico");
            this.imageListNodes.Images.SetKeyName(66, "workflow_step.ico");
            this.imageListNodes.Images.SetKeyName(67, "workflow_test.ico");
            this.imageListNodes.Images.SetKeyName(68, "workflow_test_list.ico");
            this.imageListNodes.Images.SetKeyName(69, "workflow_trigger.ico");
            this.imageListNodes.Images.SetKeyName(70, "workflow_true.ico");
            this.imageListNodes.Images.SetKeyName(71, "workflow_validation_fail.ico");
            this.imageListNodes.Images.SetKeyName(72, "workflow_validation_pass.ico");
            this.imageListNodes.Images.SetKeyName(73, "empty.ico");
            // 
            // btnExpandNodes
            // 
            this.btnExpandNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExpandNodes.AutoSize = true;
            this.btnExpandNodes.Location = new System.Drawing.Point(717, 34);
            this.btnExpandNodes.Name = "btnExpandNodes";
            this.btnExpandNodes.Size = new System.Drawing.Size(23, 23);
            this.btnExpandNodes.TabIndex = 10;
            this.btnExpandNodes.Text = "+";
            this.btnExpandNodes.UseVisualStyleBackColor = true;
            this.btnExpandNodes.Click += new System.EventHandler(this.btnExpandNodes_Click);
            // 
            // WorkflowFinderForm
            // 
            this.AcceptButton = this.btnFindWorkflows;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 453);
            this.Controls.Add(this.btnExpandNodes);
            this.Controls.Add(this.lvMatchingWorkflows);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblWorkflowId);
            this.Controls.Add(this.tvWorkflowTree);
            this.Controls.Add(this.lstTests);
            this.Controls.Add(this.txtWorkflowDescription);
            this.Controls.Add(this.txtWorkflowName);
            this.Controls.Add(this.btnClearFields);
            this.Controls.Add(this.btnFindWorkflows);
            this.Controls.Add(this.chkIncludeTrashed);
            this.Controls.Add(this.chkIncludeAliquot);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "WorkflowFinderForm";
            this.Text = "Workflow Finder Tool";
            this.Load += new System.EventHandler(this.WorkflowFinderForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkIncludeAliquot;
        private System.Windows.Forms.CheckBox chkIncludeTrashed;
        private System.Windows.Forms.Button btnFindWorkflows;
        private System.Windows.Forms.Button btnClearFields;
        private System.Windows.Forms.TextBox txtWorkflowName;
        private System.Windows.Forms.TextBox txtWorkflowDescription;
        private System.Windows.Forms.CheckedListBox lstTests;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TreeView tvWorkflowTree;
        private System.Windows.Forms.Label lblWorkflowId;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ListView lvMatchingWorkflows;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ImageList imageListWorkflows;
        private System.Windows.Forms.ImageList imageListNodes;
        private System.Windows.Forms.Button btnExpandNodes;
    }
}