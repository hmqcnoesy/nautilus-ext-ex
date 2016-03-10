using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa
{
    public partial class SampleHazardPickerForm : Form
    {
        private OracleConnection _connection;
        private int _sampleId;
        private DataTable _dataTable;
        private Dictionary<string, string> _hazardNamesAndIconFileNames;
        private Color _selectedBackgroundColor = Color.BlanchedAlmond;
        private Color _unselectedBackgroundColor = SystemColors.Window;

        public SampleHazardPickerForm(OracleConnection connection, int sampleId)
        {
            InitializeComponent();
            _connection = connection;
            _sampleId = sampleId;
            DisplaySelectedSampleInfo();
        }


        private void SampleHazardPickerForm_Load(object sender, EventArgs e)
        {
            string sql = "select h.hazard_id, case when shu.u_sample_id is null then 0 else 1 end is_selected, h.name, shu.u_remarks "
                + "from lims_sys.hazard h, lims_sys.u_sample_hazard_user shu "
                + "where h.hazard_id = shu.u_hazard_id(+) "
                + "and shu.u_sample_id(+) = :in_sample_id "
                + "order by h.name";

            var cmd = new OracleCommand(sql, _connection);
            cmd.Parameters.Add(new OracleParameter(":in_sample_id", _sampleId));
            var reader = cmd.ExecuteReader();
            _dataTable = new DataTable();
            _dataTable.Columns.Add("hazard_id", typeof(System.Int32));
            _dataTable.Columns.Add("is_selected", typeof(System.Boolean));
            _dataTable.Columns.Add("icon", typeof(Image));
            _dataTable.Columns.Add("hazard_name", typeof(System.String));
            _dataTable.Columns.Add("remarks", typeof(System.String));

            while (reader.Read())
            {
                var row = _dataTable.NewRow();
                row[0] = reader.GetInt32(0);
                row[1] = reader.GetInt32(1) > 0;
                row[2] = GetImageForHazardName(reader.GetString(2));
                row[3] = reader.GetString(2);
                row[4] = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                _dataTable.Rows.Add(row);
            }
            reader.Close();

            dgvHazards.DataSource = _dataTable;
            AdjustDataGridViewColumns();
            SetRowColors();
        }


        private Image GetImageForHazardName(string hazardName)
        {
            if (_hazardNamesAndIconFileNames == null)
            {
                _hazardNamesAndIconFileNames = GetHazardNamesAndIconFileNames();
            }

            try
            {
                if (_hazardNamesAndIconFileNames.ContainsKey(hazardName))
                {
                    return Image.FromFile("resource\\" + _hazardNamesAndIconFileNames[hazardName]);
                }
                else
                {
                    return Image.FromFile("resource\\hazard.ico");
                }
            }
            catch
            {
                return null;
            }
        }


        private void AdjustDataGridViewColumns()
        {
            dgvHazards.Columns[0].Visible = false;
            dgvHazards.Columns[1].HeaderText = dgvHazards.Columns[2].HeaderText = string.Empty;
            dgvHazards.Columns[1].Resizable = dgvHazards.Columns[2].Resizable = DataGridViewTriState.False;
            dgvHazards.Columns[2].Width = 16;
            ((DataGridViewImageColumn)dgvHazards.Columns[2]).ImageLayout = DataGridViewImageCellLayout.Zoom;
            dgvHazards.Columns[3].HeaderText = "Hazard";
            dgvHazards.Columns[3].ReadOnly = true;
            dgvHazards.Columns[4].HeaderText = "Remarks";
            dgvHazards.AutoResizeColumns();
            dgvHazards.Columns[4].Width = 300;
            dgvHazards.Columns[4].ReadOnly = true;
            foreach (DataGridViewRow dgvr in dgvHazards.Rows)
            {
                if ((bool)dgvr.Cells[1].Value)
                {
                    dgvr.Cells[4].ReadOnly = false;
                }
            }
        }


        private Dictionary<string, string> GetHazardNamesAndIconFileNames()
        {
            Dictionary<string, string> namesAndIcons = new Dictionary<string, string>();

            string sql = "select picture_item_status, selected_icon "
                + "from LIMS_sys.ENTITY_PICTURE "
                + "where schema_entity_id = (select schema_entity_id from lims_sys.schema_entity where name = 'Hazard')";

            var cmd = new OracleCommand(sql, _connection);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                namesAndIcons.Add(reader.GetString(0), reader.GetString(1));
            }

            reader.Close();

            return namesAndIcons;
        }


        private void dgvHazards_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1) return;

            if ((bool)dgvHazards.Rows[e.RowIndex].Cells[1].Value)
            {
                dgvHazards.Rows[e.RowIndex].DefaultCellStyle.BackColor = _selectedBackgroundColor;
                dgvHazards.Rows[e.RowIndex].Cells[4].ReadOnly = false;
            }
            else
            {
                dgvHazards.Rows[e.RowIndex].DefaultCellStyle.BackColor = _unselectedBackgroundColor;
                dgvHazards.Rows[e.RowIndex].Cells[4].ReadOnly = true;
            }
        }


        private void SetRowColors()
        {
            foreach (DataGridViewRow dgvr in dgvHazards.Rows)
            {
                if ((bool)dgvr.Cells[1].Value)
                {
                    dgvr.DefaultCellStyle.BackColor = _selectedBackgroundColor;
                }
            }
        }


        private void dgvHazards_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1) dgvHazards.EndEdit();
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            SetLimsUserRole();
            var tx = _connection.BeginTransaction();
            try
            {
                DeleteAllSampleHazards(tx);
                
                foreach (DataGridViewRow dgvr in dgvHazards.Rows)
                {
                    if ((bool)dgvr.Cells[1].Value)
                    {
                        InsertSampleHazard((int)dgvr.Cells[0].Value, (string)dgvr.Cells[4].Value, tx);
                    }
                }
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                ErrorHandler.LogError("SampleHazardPicker could not execute updates: " + ex.Message);
            }
        }


        private void SetLimsUserRole()
        {
            string sql = "set role lims_user";
            var cmd = new OracleCommand(sql, _connection);
            cmd.ExecuteNonQuery();
        }


        private void InsertSampleHazard(int hazardId, string remarks, OracleTransaction tx)
        {
            string sql = "insert into lims_sys.u_sample_hazard_user (u_sample_id, u_hazard_id, u_remarks) values (:in_sample_id, :in_hazard_id, :in_remarks)";
            var cmd = new OracleCommand(sql, _connection, tx);
            cmd.Parameters.Add(new OracleParameter(":in_sample_id", _sampleId));
            cmd.Parameters.Add(new OracleParameter(":in_hazard_id", hazardId));
            cmd.Parameters.Add(new OracleParameter(":in_remarks", remarks));
            cmd.ExecuteNonQuery();
        }


        private void DeleteAllSampleHazards(OracleTransaction tx)
        {
            string sql = "delete from lims_sys.u_sample_hazard_user where u_sample_id = :in_sample_id";
            var cmd = new OracleCommand(sql, _connection, tx);
            cmd.Parameters.Add(new OracleParameter(":in_sample_id", _sampleId));
            cmd.ExecuteNonQuery();
        }


        private void DisplaySelectedSampleInfo()
        {
            string sql = "select status, name from lims_sys.sample where sample_id = :in_sample_id";
            var cmd = new OracleCommand(sql, _connection);
            cmd.Parameters.Add(new OracleParameter(":in_sample_id", _sampleId));
            var reader = cmd.ExecuteReader();
            reader.Read();
            lblSampleStatus.ImageKey = reader.GetString(0);
            lblSampleName.Text = reader.GetString(1);
            reader.Close();
        }
    }
}
