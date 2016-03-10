using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("B6C7F168-1E20-4753-BC17-7A1ABDE43AFA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _EmailTagEndStatusChange : LSEXT.IWorkflowExtension, LSEXT.IVersion {
    }

    [Guid("7A0089F5-6C3B-4C53-B1E6-8C52AACA9E50")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.EmailTagEndStatusChange")]
    public class EmailTagEndStatusChange : _EmailTagEndStatusChange {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private OracleConnection _connection;

        #region IWorkflowExtension Members

        void LSEXT.IWorkflowExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            string sqlString, sampleStatus, destinationName;
            OracleCommand command;

            string operatorName = Parameters["OPERATOR_NAME"].ToString();

            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            _connection = new OracleConnection(connString);

            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "EmailTagEndStatusChange", "Error connecting to database:\r\n" + ex.Message);
                return;
            }

            //get the event type (sample status) that is triggering the workflow extension of the sample.
            try {
                sqlString = "select p.events from lims_sys.workflow_node p, lims_sys.workflow_node wn "
                    + "where p.workflow_node_id = wn.parent_id "
                    + "and wn.workflow_node_id = " + Parameters["WORKFLOW_NODE_ID"].ToString();

                command = new OracleCommand(sqlString, _connection);

                sampleStatus = (string)(OracleString)command.ExecuteOracleScalar();

            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "EmailTagEndStatusChange", "Error getting sample status / parent workflow node event type:\r\n" + ex.Message);
                sampleStatus = string.Empty;
            }

            //the sample status / parent workflow node event type determines the group destination of the status change email
            switch (sampleStatus) {
                case "L":
                    destinationName = "'Tag End Samplers'";
                    break;
                case "V":
                    destinationName = "'Tag End Schedulers'";
                    break;
                default:
                    destinationName = "'McNeil, Matt'";
                    break;
            }

            //run the pl/sql procedure:
            try {
                sqlString = "set role lims_user";
                command = new OracleCommand(sqlString, _connection);
                command.ExecuteNonQuery();

                sqlString = "lims_app_is.sample_status_email(" + Parameters["PRIMARY_KEY"].ToString() + "," + destinationName + ",'" + sampleStatus + "')";
                command = new OracleCommand(sqlString, _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.ExecuteNonQuery();

            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "EmailTagEndStatusChange", "Error calling PL/SQL procedure:\r\n" + ex.Message);
            }

            //close
            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "EmailTagEndStatusChange", "Error closing database connection:\r\n" + ex.Message);
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
