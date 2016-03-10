using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {
    public partial class ModifyAuthorisedDataForm : Form {
        private string operatorName;
        private string sessionId;
        private string sampleIdList;
        private string connString;
        private OracleConnection connection;
        private enum dynamicData { sample, aliquot, test, result };
        private DataTable dtSample, dtAliquot, dtTest, dtResult, dtUpdates;

        public ModifyAuthorisedDataForm(string sampleIdList, string operatorName, string connString, string sessionId) {
            InitializeComponent();
            this.operatorName = operatorName;
            this.sampleIdList = sampleIdList;
            this.connString = connString;
            this.sessionId = sessionId;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e) {
            if (ExecuteUpdates()) {
                this.Close();
            }
        }

        private void ModifyAuthorisedDataForm_Load(object sender, EventArgs e) {

            connection = new OracleConnection(connString);

            try {
                connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ModifyAuthorisedData", "Error connecting to database:\r\n" + ex.Message);
                this.Close();
            }

            BuildTreeView();
            BuildDataTables();

            tvSamples.SelectedNode = tvSamples.Nodes[0];

            
            //create a datatable for storing updates from changes in datagridview
            dtUpdates = new DataTable();
            dtUpdates.Columns.Add("command_text", Type.GetType("System.String"));
            dtUpdates.Columns.Add("value", Type.GetType("System.String"));
            dtUpdates.Columns.Add("id", Type.GetType("System.String"));

        }

        private void BuildTreeView() {
            string sqlString = "select s.sample_id, s.name sample_name, 'sample' || lower(s.status) sample_image, "
                + "a.aliquot_id, a.name aliquot_name, 'aliquot' || lower(a.status) aliquot_image, "
                + "t.test_id, t.name test_name, 'test' || lower(t.status) test_image, "
                + "r.result_id, r.name result_name, 'result' || lower(r.status) result_image "
                + "from lims_sys.sample s, lims_sys.aliquot a, lims_sys.test t, lims_sys.result r "
                + "where s.sample_id = a.sample_id "
                + "and a.aliquot_id = t.aliquot_id "
                + "and t.test_id = r.test_id "
                + "and s.sample_id in (" + sampleIdList + ") "
                + "order by s.sample_id, a.aliquot_id, t.test_id, r.result_id ";

            OracleCommand command = new OracleCommand(sqlString, connection);
            OracleDataReader reader = command.ExecuteReader();

            //only the level 0 nodes are part of the treeview's Nodes collection.
            //nodes level 1 or deeper are in their parent node's Nodes collection only.
            TreeNode tnSample = new TreeNode();
            TreeNode tnAliquot = new TreeNode();
            TreeNode tnTest = new TreeNode();
            TreeNode tnResult;
            string currentSampleId = string.Empty;
            string currentAliquotId = string.Empty;
            string currentTestId = string.Empty;
            while (reader.Read()) {

                if (!reader["sample_id"].ToString().Equals(currentSampleId)) {
                    tnSample = new TreeNode();
                    tnSample.Name = reader["sample_id"].ToString();
                    tnSample.Text = reader["sample_name"].ToString();
                    tnSample.ImageKey = reader["sample_image"].ToString();
                    tnSample.SelectedImageKey = reader["sample_image"].ToString();
                    currentSampleId = reader["sample_id"].ToString();
                    tvSamples.Nodes.Add(tnSample);
                }

                if (!reader["aliquot_id"].ToString().Equals(currentAliquotId)) {
                    tnAliquot = new TreeNode();
                    tnAliquot.Name = reader["aliquot_id"].ToString();
                    tnAliquot.Text = reader["aliquot_name"].ToString();
                    tnAliquot.ImageKey = reader["aliquot_image"].ToString();
                    tnAliquot.SelectedImageKey = reader["aliquot_image"].ToString();
                    currentAliquotId = reader["aliquot_id"].ToString();
                    tnSample.Nodes.Add(tnAliquot);
                }

                if (!reader["test_id"].ToString().Equals(currentTestId)) {
                    tnTest = new TreeNode();
                    tnTest.Name = reader["test_id"].ToString();
                    tnTest.Text = reader["test_name"].ToString();
                    tnTest.ImageKey = reader["test_image"].ToString();
                    tnTest.SelectedImageKey = reader["test_image"].ToString();
                    currentTestId = reader["test_id"].ToString();
                    tnAliquot.Nodes.Add(tnTest);
                }

                tnResult = new TreeNode();
                tnResult.Name = reader["result_id"].ToString();
                tnResult.Text = reader["result_name"].ToString();
                tnResult.ImageKey = reader["result_image"].ToString();
                tnResult.SelectedImageKey = reader["result_image"].ToString();
                tnTest.Nodes.Add(tnResult);

            }
            //tvSamples.ExpandAll();

            reader.Close();
        }

        /// <summary>
        /// Creates the 4 datatables containing the sample, aliquot, test, and result information
        /// </summary>
        private void BuildDataTables() {
            OracleDataAdapter adapter;
            string sqlString;

            //sample datatable
            sqlString = GetSelectClause(dynamicData.sample)
                + "from lims_sys.sample a, lims_sys.sample_user b "
                + "where a.sample_id = b.sample_id "
                + "and a.sample_id in (" + sampleIdList + ") "
                + "order by a.sample_id ";
            
            try {
                adapter = new OracleDataAdapter(sqlString, connection);
                dtSample = new DataTable();
                adapter.Fill(dtSample);
                dtSample.TableName = "sample";
                dtSample.Columns[0].ReadOnly = true;
                dtSample.Columns[1].ReadOnly = true;
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ModifyAuthorisedData", "Error getting sample info:\r\n" + ex.Message);
            }

            //aliquot datatable
            sqlString = GetSelectClause(dynamicData.aliquot)
                + "from lims_sys.aliquot a, lims_sys.aliquot_user b "
                + "where a.aliquot_id = b.aliquot_id "
                + "and a.sample_id in (" + sampleIdList + ") "
                + "order by a.sample_id, a.aliquot_id ";

            try {
                adapter = new OracleDataAdapter(sqlString, connection);
                dtAliquot = new DataTable();
                adapter.Fill(dtAliquot);
                dtAliquot.TableName = "aliquot";
                dtAliquot.Columns[0].ReadOnly = true;
                dtAliquot.Columns[1].ReadOnly = true;
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ModifyAuthorisedData", "Error getting aliquot info:\r\n" + ex.Message);
            }

            //test datatable
            sqlString = GetSelectClause(dynamicData.test)
                + "from lims_sys.test a, lims_sys.test_user b, lims_sys.aliquot c "
                + "where a.test_id = b.test_id "
                + "and a.aliquot_id = c.aliquot_id "
                + "and c.sample_id in (" + sampleIdList + ") "
                + "order by c.sample_id, c.aliquot_id, a.test_id ";

            try {
                adapter = new OracleDataAdapter(sqlString, connection);
                dtTest = new DataTable();
                adapter.Fill(dtTest);
                dtTest.TableName = "test";
                dtTest.Columns[0].ReadOnly = true;
                dtTest.Columns[1].ReadOnly = true;
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ModifyAuthorisedData", "Error getting test info:\r\n" + ex.Message);
            }

            //result datatable
            sqlString = GetSelectClause(dynamicData.result)
                + "from lims_sys.result a, lims_sys.result_user b, lims_sys.test c, lims_sys.aliquot d "
                + "where a.result_id = b.result_id "
                + "and a.test_id = c.test_id "
                + "and c.aliquot_id = d.aliquot_id "
                + "and d.sample_id in (" + sampleIdList + ") "
                + "order by d.sample_id, d.aliquot_id, c.test_id, a.result_id ";

            try {
                adapter = new OracleDataAdapter(sqlString, connection);
                dtResult = new DataTable();
                adapter.Fill(dtResult);
                dtResult.TableName = "result";
                dtResult.Columns[0].ReadOnly = true;
                dtResult.Columns[1].ReadOnly = true;
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ModifyAuthorisedData", "Error getting result info:\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// Returns a string of all the column names listed in a particular entity's 'Modify Authorised Data' template.
        /// </summary>
        /// <param name="tableName">Enum value of the dynamic data entity</param>
        /// <returns>string</returns>
        private string GetSelectClause(dynamicData tableName) {
            StringBuilder columnListing = new StringBuilder("select a." + tableName + "_id, a.name");
            //string sqlString = "select sf.database_name "
            //    + "from lims_sys." + tableName.ToString() + "_template_field tf, lims_sys." + tableName.ToString() + "_template t, lims_sys.schema_field sf "
            //    + "where tf.schema_field_id = sf.schema_field_id "
            //    + "and tf." + tableName.ToString() + "_template_id = t." + tableName.ToString() + "_template_id "
            //    + "and t.name = 'Modify Authorised Data' "
            //    + "order by tf.order_number ";

            string sqlString = "select sf.database_name "
                + "from lims_sys.schema_field_prompt sfp, lims_sys.schema_field sf, lims_sys.schema_table st "
                + "where sfp.schema_field_id = sf.schema_field_id "
                + "and sf.schema_table_id = st.schema_table_id "
                + "and st.database_name = '" + tableName.ToString().ToUpper() + "_USER' "
                + "and sf.database_name like 'U_%' "
                + "order by sf.database_name ";

            OracleCommand command;
            OracleDataReader reader;

            try {
                command = new OracleCommand(sqlString, connection);
                reader = command.ExecuteReader();

                while (reader.Read()) {

                    columnListing.Append(", b." + reader["database_name"].ToString());

                    //if (reader["database_name"].ToString().Substring(0, 2).ToUpper().Equals("U_")) {
                    //    columnListing.Append(", b." + reader["database_name"].ToString());
                    //} else {
                    //    columnListing.Append(", a." + reader["database_name"].ToString());
                    //}
                }

                reader.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ModifyAuthorisedData", "Error getting " + tableName.ToString() + " columns:\r\n" + ex.Message);
                //ErrorHandler.LogError(operatorName, "ModifyAuthorisedData", "Make sure there is a " + tableName.ToString() + " template named 'Modify Authorised Data'");
                return string.Empty;
            }

            columnListing.Append(" ");

            return columnListing.ToString();
        }

        private void tvSamples_AfterSelect(object sender, TreeViewEventArgs e) {

            try {
                dgvAuthorisedData.Columns[0].Frozen = false;
                dgvAuthorisedData.Columns[1].Frozen = false;
            } catch (Exception) {
            }

            switch (e.Node.ImageKey.Substring(0, 1)) {
                case "s":
                    dgvAuthorisedData.DataSource = dtSample;
                    break;
                case "a":
                    dgvAuthorisedData.DataSource = dtAliquot;
                    break;
                case "t":
                    dgvAuthorisedData.DataSource = dtTest;
                    break;
                case "r":
                    dgvAuthorisedData.DataSource = dtResult;
                    break;
                default:
                    break;
            }

            dgvAuthorisedData.Columns[0].Frozen = true;
            dgvAuthorisedData.Columns[1].Frozen = true;

            //select the dgv row that corresponds to the selected treeview node
            foreach (DataGridViewRow dgvr in dgvAuthorisedData.Rows) {
                if (dgvr.Cells[0].Value.ToString().Equals(e.Node.Name)) {
                    dgvAuthorisedData.CurrentCell = dgvr.Cells[0];
                    break;
                }
            }
        }

        private void SelectCurrentTreeNode(TreeNode tn, string nodeName) {
            if (tn.Name == nodeName) {
                tvSamples.SelectedNode = tn;
            } else {
                foreach (TreeNode tn2 in tn.Nodes) {
                    SelectCurrentTreeNode(tn2, nodeName);
                }
            }
        }


        /// <summary>
        /// Creates two rows in dtUpdates when a user changes value in a grid view cell.
        /// One row for updated value (on main or _user table), second row on _user table for unauthorisation remarks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAuthorisedData_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            //data type of the changed cell is important for creating sql statement
            Type type = ((DataTable)dgvAuthorisedData.DataSource).Columns[e.ColumnIndex].DataType;

            StringBuilder sbCommandText;
            StringBuilder sbUserCommandText;
            string entityId;
            string newValue = string.Empty;
            string unauthorisationValue = sessionId + ": " + txtOperatorRemarks.Text;

            sbCommandText = new StringBuilder("update lims_sys.");
            sbCommandText.Append(dgvAuthorisedData.DataSource.ToString());

            sbUserCommandText = new StringBuilder("update lims_sys.");
            sbUserCommandText.Append(dgvAuthorisedData.DataSource.ToString()
                + "_user set u_unauthorization = u_unauthorization || '  ' || :value "
                + "where " + dgvAuthorisedData.DataSource.ToString() + "_id = :id ");

            if (dgvAuthorisedData.Columns[e.ColumnIndex].Name.StartsWith("U_")) {
                sbCommandText.Append("_user");
            }
            
            sbCommandText.Append(" set " + dgvAuthorisedData.Columns[e.ColumnIndex].Name);

            if (string.IsNullOrEmpty(dgvAuthorisedData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())) {
                sbCommandText.Append(" = null ");
            } else {
                if (type == Type.GetType("System.DateTime")) {
                    sbCommandText.Append(" = to_date(:value, 'MM/DD/YYYY HH24:MI:SS') ");
                    DateTime date = (DateTime)dgvAuthorisedData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    newValue = date.ToString("MM/dd/yyyy HH:mm:ss");
                } else {
                    sbCommandText.Append(" = :value ");
                    newValue = dgvAuthorisedData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                }
            }

            sbCommandText.Append("where " + dgvAuthorisedData.DataSource.ToString() + "_id = :id");

            entityId = dgvAuthorisedData.Rows[e.RowIndex].Cells[0].Value.ToString();

            dtUpdates.Rows.Add(sbCommandText.ToString(), newValue, entityId);
            dtUpdates.Rows.Add(sbUserCommandText.ToString(), sessionId + ": " + txtOperatorRemarks.Text, entityId);

        }

        private void ModifyAuthorisedDataForm_FormClosed(object sender, FormClosedEventArgs e) {
            try {
                connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ModifyAuthorisedDataForm", "Error closing connection:\r\n" + ex.Message);
            }
        }


        /// <summary>
        /// Runs all pending updates stored in the dtUpdates datatable.
        /// </summary>
        /// <returns>True if all updates succeeded.  False if any updates failed.</returns>
        private bool ExecuteUpdates() {
            bool returnValue = true;
            OracleCommand command;
            OracleParameter parameterId, parameterValue;

            //need to set role first
            command = new OracleCommand("set role lims_user", connection);

            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ModifyAuthorisedDataForm", "Failed to set role:\r\n" + ex.Message);
            }

            for (int i = 0; i < dtUpdates.Rows.Count; i++) {
                command = new OracleCommand(dtUpdates.Rows[i]["command_text"].ToString(), connection);
                parameterId = new OracleParameter(":id", dtUpdates.Rows[i]["id"].ToString());
                command.Parameters.Add(parameterId);

                //if the value column in the datatable is string.empty, there will be no :value parameter in the statement
                if (!string.IsNullOrEmpty(dtUpdates.Rows[i]["value"].ToString())) {
                    parameterValue = new OracleParameter(":value", dtUpdates.Rows[i]["value"].ToString());
                    command.Parameters.Add(parameterValue);
                }

                try {
                    command.ExecuteNonQuery();
                } catch (Exception ex) {
                    returnValue = false;
                    ErrorHandler.LogError(operatorName, "ModifyAuthorisedDataForm", "Failed to execute update:\r\n"
                        + dtUpdates.Rows[i]["command_text"].ToString() + ";    " + dtUpdates.Rows[i]["id"].ToString()
                        + ";    " + dtUpdates.Rows[i]["value"].ToString() + ":\r\n" + ex.Message);
                }
            }

            //need to clear out the update table, so if the form stays open and OK is clicked again, the same updates won't be executed again.
            dtUpdates.Rows.Clear();

            return returnValue;
        }

        private void txtOperatorRemarks_TextChanged(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(txtOperatorRemarks.Text)) {
                txtOperatorRemarks.BackColor = Color.GreenYellow;
                dgvAuthorisedData.ReadOnly = true;
            } else {
                txtOperatorRemarks.BackColor = SystemColors.Window;
                dgvAuthorisedData.ReadOnly = false;
            }
        }

        private void dgvAuthorisedData_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            MessageBox.Show("The value input is not valid for this column type (" 
                + ((DataTable)dgvAuthorisedData.DataSource).Columns[e.ColumnIndex].DataType.ToString() + ").");

            e.Cancel = true;
        }
    }
}
