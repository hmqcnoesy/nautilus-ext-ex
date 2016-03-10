using System;
using System.Data.OracleClient;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {
    public partial class FixTurnaroundTimeForm : Form {
        private string sampleName, turnaroundTime;
        private DateTime receivedOn, authorisedOn;
        private OracleConnection connection;

        public FixTurnaroundTimeForm(string sampleName, DateTime receivedOn, DateTime authorisedOn, OracleConnection connection) {
            InitializeComponent();
            this.sampleName = sampleName;
            this.receivedOn = receivedOn;
            this.authorisedOn = authorisedOn;

            this.connection = connection;

            lblSampleName.Text = sampleName;
            txtTurnaroundTime.Text = turnaroundTime;
            dtpReceivedOn.Value = receivedOn;
            dtpAuthorisedOn.Value = authorisedOn;
        }

        private void dtp_ValueChanged(object sender, EventArgs e) {
            try {
                string tTime = string.Empty;

                if (dtpAuthorisedOn.Value < dtpReceivedOn.Value) {
                    tTime = "-";
                }

                TimeSpan ts = dtpAuthorisedOn.Value - dtpReceivedOn.Value;
                tTime += ts.Days.ToString("000") 
                    + " " + ts.Hours.ToString("00") 
                    + ":" + ts.Minutes.ToString("00") 
                    + ":" + ts.Seconds.ToString("00");

                txtTurnaroundTime.Text = tTime;
            } catch (Exception ex) {
                ErrorHandler.LogError("Error calculating turnaround time on form:\r\n" + ex.Message);
                txtTurnaroundTime.Text = string.Empty;
            }
        }

        private void FixTurnaroundTimeForm_Load(object sender, EventArgs e) {

        }

        private void btnOk_Click(object sender, EventArgs e) {

            string sqlStringSample = "update lims_sys.sample set "
                + "received_on = to_date('" + dtpReceivedOn.Value.ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS'), "
                + "authorised_on = to_date('" + dtpAuthorisedOn.Value.ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') "
                + "where name = '" + sampleName + "' ";
            
            string sqlStringSampleUser = "update lims_sys.sample_user set "
                + "u_processing_time = '" + txtTurnaroundTime.Text + "' "
                + "where sample_id = (select sample_id from lims_sys.sample where name = '" + sampleName + "') ";

            string sqlStringAliquot = "update lims_sys.aliquot set "
                + "received_on = to_date('" + dtpReceivedOn.Value.ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS'), "
                + "authorised_on = to_date('" + dtpAuthorisedOn.Value.ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') "
                + "where sample_id in (select sample_id from lims_sys.sample where name = '" + sampleName + "') ";

            string sqlStringAliquotUser = "update lims_sys.aliquot_user set "
                + "u_mcc_transfer = 'T' "
                + "where aliquot_id in (select a.aliquot_id from lims_sys.sample s, lims_sys.aliquot a where a.sample_id = s.sample_id and s.name = '" + sampleName + "') ";

            string sqlStringTest = "update lims_sys.test set "
                + "authorised_on = to_date('" + dtpAuthorisedOn.Value.ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') "
                + "where aliquot_id in (select a.aliquot_id from lims_sys.aliquot a, lims_sys.sample s where s.sample_id = a.sample_id "
                + "and s.name = '" + sampleName + "') ";

            string sqlStringResult = "update lims_sys.result set "
                + "authorised_on = to_date('" + dtpAuthorisedOn.Value.ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') "
                + "where test_id in (select t.test_id from lims_sys.test t, lims_sys.aliquot a, lims_sys.sample s "
                + "where s.sample_id = a.sample_id and a.aliquot_id = t.aliquot_id and s.name = '" + sampleName + "') ";
            
            OracleTransaction transaction;
            transaction = connection.BeginTransaction();

            try {

                OracleCommand command = new OracleCommand("set role lims_user", connection);
                command.Transaction = transaction;
                command.ExecuteNonQuery();

                command.CommandText = sqlStringSample;
                command.ExecuteNonQuery();

                command.CommandText = sqlStringSampleUser;
                command.ExecuteNonQuery();

                command.CommandText = sqlStringAliquot;
                command.ExecuteNonQuery();

                command.CommandText = sqlStringAliquotUser;
                command.ExecuteNonQuery();

                command.CommandText = sqlStringTest;
                command.ExecuteNonQuery();

                command.CommandText = sqlStringResult;
                command.ExecuteNonQuery();

                transaction.Commit();

            } catch (Exception ex) {
                ErrorHandler.LogError("Error updating database.  Changes have not been saved:\r\n" + ex.Message);
                transaction.Rollback();
                return;
            }

            this.Close();
        }
    }
}
