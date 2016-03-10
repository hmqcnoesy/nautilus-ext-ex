using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.OracleClient;
using System.Windows.Forms;

namespace NautilusExtensions.All {

    [Guid("24085FFF-4ED0-4FDE-889D-457A657B218F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _WorksheetResultReset : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("CCC29B69-1EC2-4C3B-8993-9BE7E2284599")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.All.WorksheetResultReset")]
    public class WorksheetResultReset : _WorksheetResultReset {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private OracleConnection _connection;
        private string _operatorName;
        
        #region IEntityExtension Members

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();
            int worksheetEntityId;
            int entityId = (int)Parameters["ENTITY_ID"];

            //Connection string
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            try {
                _connection = new OracleConnection(connString);
                _connection.Open();
            } catch (Exception ex){
                ErrorHandler.LogMessage(_operatorName, "WorksheetResultReset", "Connection error:\r\n" + ex.Message);
                return LSEXT.ExecuteExtension.exDisabled;
            }

            //making sure that the selected entity is a worksheet
            string sqlString = "select schema_entity_id from lims_sys.schema_entity where name = 'Worksheet' ";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            try {
                worksheetEntityId = (int)(OracleNumber)command.ExecuteOracleScalar();
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "WorksheetResultReset", "Error getting worksheet entity id:\r\n" + ex.Message);
                if (_connection != null) {
                    _connection.Close();
                }
                return LSEXT.ExecuteExtension.exDisabled;
            }

            if (entityId != worksheetEntityId) {
                ErrorHandler.LogMessage(_operatorName, "WorksheetResultReset", "Attempted to execute this extension on wrong entity (" + entityId + ").");
                return LSEXT.ExecuteExtension.exDisabled;
            }

            //making sure that each selected worksheet's status is NOT 'C'
            StringBuilder sb = new StringBuilder();
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            while (!records.EOF) {
                sb.Append(records.Fields[0].Value.ToString());
                records.MoveNext();
                if (!records.EOF) sb.Append(",");
            }

            sqlString = "select distinct status from lims_sys.worksheet where worksheet_id in (" + sb.ToString() + ") ";
            command = new OracleCommand(sqlString, _connection);
            bool isWorksheetComplete = false;

            try {
                OracleDataReader reader = command.ExecuteReader();
                while (reader.Read()) {
                    if (reader["status"].Equals("C")) {
                        isWorksheetComplete = true;
                    }
                }
                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "WorksheetResultReset", "Error getting list of worksheet statuses:\r\n" + ex.Message);
                _connection.Close();
                return LSEXT.ExecuteExtension.exDisabled;
            }

            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "WorksheetResultReset", "Connection close error:\r\n" + ex.Message);
            }

            if (isWorksheetComplete) {
                return LSEXT.ExecuteExtension.exDisabled;
            } else {
                return LSEXT.ExecuteExtension.exEnabled;
            }
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            //Connection string
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            _connection = new OracleConnection(connString);

            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetResultReset", "Connection error:\r\n" + ex.Message);
                return;
            }

            //set the lims_user role
            OracleCommand command = new OracleCommand("set role lims_user", _connection);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetResultReset", "Can't set lims_user role:\r\n" + ex.Message);
                _connection.Close();
            }

            //loop through each of the selected worksheets, and reset all results after prompting for confirmation
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            string sqlString;
            bool isFirstRecord;
            OracleDataReader reader;
            OracleParameter parameter;
            OracleTransaction transaction;
            StringBuilder notCompletedResults;

            while (!records.EOF) {
                
                //prompt for confirmation, if canceled, go to next selected worksheet
                if (MessageBox.Show("Reset status of all results for worksheet " + records.Fields[0].Value.ToString() + "?",
                    "Reset", MessageBoxButtons.OKCancel) == DialogResult.Cancel) {

                    records.MoveNext();
                    continue;
                }

                //construct a list of result ids that need to be left alone when executing the update.
                //this is necessary to avoid uncancelling results that had status = 'X' and old_status like '%V'
                sqlString = "select r.result_id "
                    + "from lims_sys.result r, lims_sys.test t, lims_sys.worksheet_entry we, lims_sys.worksheet w "
                    + "where w.worksheet_id = we.worksheet_id "
                    + "and we.aliquot_id = t.aliquot_id "
                    + "and w.def_test_template_id = t.test_template_id "
                    + "and t.test_id = r.test_id "
                    + "and r.status != 'C' "
                    + "and we.worksheet_id = :in_worksheet_id ";

                command = new OracleCommand(sqlString, _connection);
                parameter = new OracleParameter(":in_worksheet_id", records.Fields[0].Value.ToString());
                command.Parameters.Add(parameter);

                try {
                    //build the comma-separated list of results to leave alone
                    reader = command.ExecuteReader();
                    notCompletedResults = new StringBuilder();
                    isFirstRecord = true;

                    while (reader.Read()) {
                        if (!isFirstRecord) {
                            notCompletedResults.Append(",");
                        } else {
                            isFirstRecord = false;
                        }

                        notCompletedResults.Append(reader["result_id"].ToString());
                    }

                    reader.Close();

                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "WorksheetResultReset", "Error building non-completed result string:\r\n" + ex.Message);
                    continue;
                }

                //First update the status to 'X'
                transaction = _connection.BeginTransaction();
                command.Transaction = transaction;
                try {
                    command.CommandText = "update lims_sys.result set status = 'X' where worksheet_id = :in_worksheet_id ";
                    if (notCompletedResults.ToString().Length > 0) command.CommandText += "and result_id not in (" + notCompletedResults.ToString() + ")";
                    command.ExecuteNonQuery();

                    command.CommandText = "update lims_sys.result set old_status = 'V' where worksheet_id = :in_worksheet_id ";
                    if (notCompletedResults.ToString().Length > 0) command.CommandText += "and result_id not in (" + notCompletedResults.ToString() + ")";
                    command.ExecuteNonQuery();

                    command.CommandText = "update lims_sys.result set status = 'V' where worksheet_id = :in_worksheet_id ";
                    if (notCompletedResults.ToString().Length > 0) command.CommandText += "and result_id not in (" + notCompletedResults.ToString() + ")";
                    command.ExecuteNonQuery();

                    transaction.Commit();

                } catch (Exception ex) {
                    transaction.Rollback();
                    ErrorHandler.LogError(_operatorName, "WorksheetResultReset", "Error updating result statuses:\r\n" + ex.Message);
                }

                records.MoveNext();
            }

            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetResultReset", "Connection close error:\r\n" + ex.Message);
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
