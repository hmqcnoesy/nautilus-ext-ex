using System;
using System.Data.OracleClient;
using System.Windows.Forms;
using System.Drawing;

namespace NautilusExtensions.All {
    public partial class ConfigureColumnsForm : Form {
        OracleConnection connection;
        string operatorName;

        private int sourceOperatorId;
        public int SourceOperatorId {
            get {
                return sourceOperatorId;
            }

            set {
                sourceOperatorId = value;
            }
        }

        public ConfigureColumnsForm(OracleConnection connection, string operatorName) {
            InitializeComponent();
            this.connection = connection;
            this.operatorName = operatorName;
            sourceOperatorId = 0;
            txtTargetOperator.Text = this.operatorName;
        }

        private void ConfigureColumnsForm_Load(object sender, EventArgs e) {
            string sqlString = "select name, operator_id from lims_sys.operator "
                + "where upper(name) not like 'LIMS%' "
                + "and upper(name) != upper(:in_operator_name) "
                + "and allow_login = 'T' "
                + "order by name ";

            OracleCommand command;
            OracleDataReader reader;

            try {
                command = new OracleCommand(sqlString, connection);
                command.Parameters.Add(new OracleParameter(":in_operator_name", operatorName));
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    cmbSourceOperator.Items.Add(reader["name"].ToString());
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ConfigureColumns", "Error reading operator names:\r\n" + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            sourceOperatorId = 0;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e) {

            if (cmbSourceOperator.Text.Equals(string.Empty)) {
                return;
            }

            string sqlString = "select operator_id from lims_sys.operator where name = :operator_name ";
            OracleParameter parameter = new OracleParameter(":operator_name", cmbSourceOperator.Text);

            try {
                OracleCommand command = new OracleCommand(sqlString, connection);
                command.Parameters.Add(parameter);
                sourceOperatorId = (int)(OracleNumber)command.ExecuteOracleScalar();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ConfigureColumns", "Error getting ID for '" + cmbSourceOperator.Text + "':\r\n" + ex.Message);
            }

            this.Close();
        }
    }
}
