using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NautilusExtensions.Qa {
    public partial class ManufacturerManagerForm : Form {

        private DataTable _samplesTable;
        private List<string> _manufacturersList;

        public ManufacturerManagerForm(List<string> manufacturersList, DataTable samplesTable) {
            InitializeComponent();

            _samplesTable = samplesTable;
            _manufacturersList = manufacturersList;
        }

        private void ManufacturerManagerForm_Load(object sender, EventArgs e) {

            dgvSamples.DataSource = _samplesTable;
            dgvSamples.Columns[0].ReadOnly = true;
            dgvSamples.Columns[1].ReadOnly = true;
            dgvSamples.Columns[2].ReadOnly = true;

            // remove text column and add drop down list column
            dgvSamples.Columns.Remove("NewValue");
            DataGridViewComboBoxColumn columnChangeTo = new DataGridViewComboBoxColumn();
            columnChangeTo.Name = "Change To";
            columnChangeTo.HeaderText = "Change To";
            columnChangeTo.DataSource = _manufacturersList;
            dgvSamples.Columns.Add(columnChangeTo);

            // clean up the grid view
            dgvSamples.Columns[0].Visible = false;
            dgvSamples.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvSamples.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSamples.Columns[0].DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSamples.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvSamples.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSamples.Columns[1].DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSamples.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvSamples.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSamples.Columns[2].DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSamples.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvSamples.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSamples.Columns[3].DefaultCellStyle.BackColor = Color.Gainsboro;
            dgvSamples.Columns[4].Width = 200;
            this.Width = dgvSamples.Columns[1].Width + dgvSamples.Columns[2].Width + dgvSamples.Columns[3].Width +  dgvSamples.Columns[4].Width + 80;


            // set the correct value in the combobox column for each row
            string defaultValue = _manufacturersList[0];
            foreach (DataGridViewRow dgvr in dgvSamples.Rows) {
                dgvr.Cells["Change To"].Value = defaultValue;
            }
        }

        private void btnOk_Click(object sender, EventArgs e) {

            // transfer the combobox selections into the NewValue column of the data table
            string newValue;
            foreach (DataGridViewRow dgvr in dgvSamples.Rows) {
                newValue = dgvr.Cells["Change To"].Value.ToString();
                ((DataRowView)dgvr.DataBoundItem)["NewValue"] = newValue;
            }
        }
    }
}
