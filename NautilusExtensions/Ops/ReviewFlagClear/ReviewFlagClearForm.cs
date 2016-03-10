using System;
using System.Data.OracleClient;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {
    public partial class ReviewFlagClearForm : Form {
        OracleConnection connection;
        string worksheetId;
        int sessionId;

        /// <summary>
        /// Instantiates the form for review flag remarks.
        /// </summary>
        /// <param name="connection">The database connection object for retreiving and updating records.</param>
        /// <param name="worksheetId">The worksheet ID for result display.</param>
        /// <param name="sessionId">The current operator's session ID, which will go with the remarks.</param>
        public ReviewFlagClearForm(OracleConnection connection, string worksheetId, int sessionId) {
            InitializeComponent();
            this.connection = connection;
            this.worksheetId = worksheetId;
            this.sessionId = sessionId;
        }

        private void ReviewFlagClearForm_Load(object sender, EventArgs e) {
            string sqlString = "select r.result_id, a.name, r.name, r.formatted_result, r.formatted_unit, "
                + "lims_read.get_stat_score(r.result_id), ru.u_stat_review_remarks "
                + "from lims_sys.result r, lims_sys.result_user ru, lims_sys.test t, lims_sys.aliquot a "
                + "where a.aliquot_id = t.aliquot_id "
                + "and t.test_id = r.test_id "
                + "and r.result_id = ru.result_id "
                + "and ru.u_needs_stat_review = 'T' "
                + "and r.worksheet_id = " + worksheetId; 

            OracleCommand command = new OracleCommand(sqlString, connection);
            OracleDataReader reader;

            try {
                reader = command.ExecuteReader();
                string[] rowToAdd;
                while (reader.Read()) {
                    rowToAdd = new string[7];
                    for (int i = 0; i < rowToAdd.Length; i++) {
                        rowToAdd[i] = reader[i].ToString();
                    }

                    dgvResults.Rows.Add(rowToAdd);
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError("ReviewFlagClearForm", "Error getting worksheet result records:\r\n" + ex.Message);
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e) {

            OracleCommand command = new OracleCommand("set role lims_user", connection);
            OracleParameter parameter;

            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError("ReviewFlagClearForm", "Error setting the lims_user role:\r\n" + ex.Message);
            }

            //loop through the data grid view's rows and do an update on each record iff remarks have been input.
            bool needToShowMessage = true;
            string sqlString;
            foreach (DataGridViewRow dgvr in dgvResults.Rows) {
                if (string.IsNullOrEmpty(dgvr.Cells[6].Value.ToString()) && needToShowMessage) {
                    MessageBox.Show("Result flags will not be cleared where no remarks have been input.");
                    needToShowMessage = false;
                }

                if (!string.IsNullOrEmpty(dgvr.Cells[6].Value.ToString())) {
                    sqlString = "update lims_sys.result_user "
                        + "set u_needs_stat_review = 'F', "
                        + "u_stat_review_remarks = u_stat_review_remarks || '" + sessionId + ": ' || "
                        + ":result_remarks || ';  ' "
                        + "where result_id = " + dgvr.Cells[0].Value.ToString();

                    command.CommandText = sqlString;
                    parameter = new OracleParameter();
                    parameter.ParameterName = ":result_remarks";
                    parameter.Value = dgvr.Cells[6].Value.ToString();
                    command.Parameters.Add(parameter);

                    try {
                        command.ExecuteNonQuery();
                    } catch (Exception ex) {
                        ErrorHandler.LogError("ReviewClearFlagForm",
                            "Error updating result " + dgvr.Cells[0].Value.ToString() + ":\r\n" + ex.Message);
                    }
                }
            }

            this.Close();
        }


    }
}
