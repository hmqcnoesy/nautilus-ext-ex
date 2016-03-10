using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NautilusExtensions.All {
    public partial class TransferFolderOwnershipForm : Form {

        public DataTable Folders { get; set; }
        private List<string> _operatorNames;

        public TransferFolderOwnershipForm(DataTable folders, List<string> operatorNames) {
            InitializeComponent();
            Folders = folders;
            _operatorNames = operatorNames;
        }


        private void TransferFolderOwnershipForm_Load(object sender, EventArgs e) {

            dgvFolders.DataSource = Folders;
            dgvFolders.Columns[0].Visible = false;
            dgvFolders.Columns[1].ReadOnly = true;
            dgvFolders.Columns[2].ReadOnly = true;

            // remove last column and add it again as a drop down list
            DataGridViewComboBoxColumn columnNewOperatorName = new DataGridViewComboBoxColumn();
            columnNewOperatorName.Name = "Transfer To";
            columnNewOperatorName.HeaderText = "Transfer To";
            columnNewOperatorName.DataSource = _operatorNames;
            dgvFolders.Columns.Add(columnNewOperatorName);
            
            // clean up the grid view
            dgvFolders.Columns.Remove("New Owner");
            dgvFolders.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvFolders.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            dgvFolders.Columns[1].DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvFolders.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvFolders.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            dgvFolders.Columns[2].DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvFolders.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvFolders.Columns[3].Width = 160;
            this.Width = dgvFolders.Columns[1].Width + dgvFolders.Columns[2].Width + dgvFolders.Columns[3].Width + 60;

            
            // set the correct value in the combobox column for each row
            foreach (DataGridViewRow dgvr in dgvFolders.Rows) {
                dgvr.Cells["Transfer To"].Value = ((DataRowView)dgvr.DataBoundItem)["New Owner"];
            }
        }


        private void btnCopyDown_Click(object sender, EventArgs e) {
            if (dgvFolders.SelectedCells.Count != 1) return;

            int rowIndex = dgvFolders.SelectedCells[0].RowIndex;
            string valueToCopy = dgvFolders.Rows[rowIndex].Cells["Transfer To"].Value.ToString();

            for (int i = rowIndex + 1; i < dgvFolders.Rows.Count; i++) {
                dgvFolders.Rows[i].Cells["Transfer To"].Value = valueToCopy;
            }
        }


        private void btnOk_Click(object sender, EventArgs e) {
            // transfer the combobox selections into the New Owner column of the data table
            string newOwnerValue;
            foreach (DataGridViewRow dgvr in dgvFolders.Rows) {
                newOwnerValue = dgvr.Cells["Transfer To"].Value.ToString();
                ((DataRowView)dgvr.DataBoundItem)["New Owner"] = newOwnerValue;
            }
        }
    }
}
