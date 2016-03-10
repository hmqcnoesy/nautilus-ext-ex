using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("8527AD12-54AA-4BDB-872D-8B9EE9D4DF61")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _WorkflowNodeCommenter : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("53630632-6C45-4C40-BBF3-E66C9A74A3A9")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.WorkflowNodeCommenter")]
    public class WorkflowNodeCommenter : _WorkflowNodeCommenter {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private OracleConnection _connection;
        private string _operatorName;

        #region IEntityExtension Members

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            //only allow extension to run on sample, aliquot or test workflow entities
            if (Parameters["ENTITY_ID"].ToString().Equals("88") ||
                Parameters["ENTITY_ID"].ToString().Equals("6") ||
                Parameters["ENTITY_ID"].ToString().Equals("113")) {
                return LSEXT.ExecuteExtension.exEnabled;
            } else {
                return LSEXT.ExecuteExtension.exDisabled;
            }
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            StringBuilder resultsToBeUpdated;
            string sqlString;
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            OracleCommand command;
            OracleDataReader reader;
            _connection = new OracleConnection(connString);

            try {
                _connection.Open();
                command = new OracleCommand("set role lims_user", _connection);
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorkflowNodeCommenter", "Error connecting to the database:\r\n" + ex.Message);
            }


            while (!records.EOF) {
                resultsToBeUpdated = new StringBuilder("The following tests in workflow " + records.Fields[0].Value.ToString().ToString() 
                    + " have SL Numbers result nodes that will be commented out:");
                sqlString = "select long_name, workflow_node_id "
                    + "from lims_sys.workflow_node "
                    + "where workflow_node_type_id = 42 "
                    + "and workflow_node_id in "
                        + "(select parent_id "
                        + "from lims_sys.workflow_node "
                        + "where workflow_id = " + records.Fields[0].Value.ToString() + " "
                        + "and workflow_node_type_id = 28 "
                        + "and template = 39 "
                        + "and long_name = 'SL Numbers') "
                    + "order by order_number ";

                try {
                    command = new OracleCommand(sqlString, _connection);
                    reader = command.ExecuteReader();

                    if (!reader.HasRows) {
                        MessageBox.Show("Workflow " + records.Fields[0].Value.ToString() + " has no SL Number nodes to comment out.\r\n"
                            + "Execute this extension individually on any subtreed aliquot or test workflows.");
                        records.MoveNext();
                        continue;
                    }

                    while (reader.Read()) {
                        resultsToBeUpdated.Append(Environment.NewLine + reader["long_name"].ToString());
                    }

                    reader.Close();
                    resultsToBeUpdated.Append(Environment.NewLine + "Do you want to continue with this update?");

                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "WorkflowNodeCommenter", "Error getting list of workflow nodes to comment out:\r\n" + ex.Message);
                }

                //here prompt user to go ahead with the updates
                if (MessageBox.Show(resultsToBeUpdated.ToString(), "Update Workflow?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    sqlString = "update lims_sys.workflow_node "
                        + "set workflow_node_type_id = 79, "
                            + "name = 'Comment', "
                            + "template = null, "
                            + "long_name = 'SL Number result commented out', "
                            + "parameter_1 = 'SL Number result commented out', "
                            + "parameter_2 = 'This SL Numbers result node was converted to a comment node by " + Parameters["OPERATOR_NAME"] + ", '"
                                + "|| to_char(sysdate, 'MM/dd/yy HH24:MI:SS') || ', using a Nautilus extension.' "
                        + "where workflow_id = " + records.Fields[0].Value.ToString() + " "
                        + "and workflow_node_type_id = 28 "
                        + "and template = 39 "
                        + "and long_name = 'SL Numbers' ";

                    try {
                        command = new OracleCommand(sqlString, _connection);
                        command.ExecuteNonQuery();
                    } catch (Exception ex) {
                        ErrorHandler.LogError(_operatorName, "WorkflowNodeCommenter", "Error updating workflow node table:\r\n" + ex.Message);
                    }
                }

                records.MoveNext();
            }
        }

        #endregion

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
