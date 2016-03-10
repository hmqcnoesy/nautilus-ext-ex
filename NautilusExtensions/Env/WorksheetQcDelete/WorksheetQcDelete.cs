using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace EnvCsharpExtensions {

    [Guid("4FCE8D6F-D1D4-4ADE-A71A-DD9E7BAD51E1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _WorksheetQcDelete : LSEXT.IEntityExtension, LSEXT.IWorkflowExtension, LSEXT.IVersion {
    }

    [Guid("11409FE4-2290-45A1-A290-4798D311459C")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Env.WorksheetQcDelete")]
    public class WorksheetQcDelete : _WorksheetQcDelete {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            return LSEXT.ExecuteExtension.exEnabled;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            //make the cursor an hourglass, this could take a while
            Cursor savedCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            //Connection string
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            _connection = new OracleConnection(connString);

            //open the connection
            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcDelete", "Connection error:\r\n" + ex.Message);
                return;
            }

            //set the lims_user role
            OracleCommand command = new OracleCommand("set role lims_user", _connection);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcDelete", "Can't set lims_user role:\r\n" + ex.Message);
                _connection.Close();
            }

            //making sure that the selected entity is a worksheet
            string sqlString = "select schema_entity_id from lims_sys.schema_entity where name = 'Worksheet' ";
            command = new OracleCommand(sqlString, _connection);
            int worksheetEntityId;
            int entityId = (int)Parameters["ENTITY_ID"];

            try {
                worksheetEntityId = (int)(OracleNumber)command.ExecuteOracleScalar();

                if (entityId != worksheetEntityId) {
                    ErrorHandler.LogMessage(_operatorName, "WorksheetQcDelete", "Attempted to execute this extension on wrong entity (" + entityId + ").");
                    _connection.Close();
                    return;
                }
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "WorksheetQcDelete", "Error getting worksheet entity id:\r\n" + ex.Message);
                _connection.Close();
                return;
            }

            //loop through each of the selected worksheets
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];

            while (!records.EOF) {
                PurgeWorksheet(long.Parse(records.Fields[0].Value.ToString()));
                records.MoveNext();
            }

            //close connection
            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcDelete", "Connection close error:\r\n" + ex.Message);
            }

            //put the old cursor back
            Cursor.Current = savedCursor;
        }

        void LSEXT.IWorkflowExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            //make the cursor an hourglass, this could take a while
            Cursor savedCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            //Connection string
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            _connection = new OracleConnection(connString);

            //open the connection
            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate", "Connection error:\r\n" + ex.Message);
                return;
            }

            //set the lims_user role
            OracleCommand command = new OracleCommand("set role lims_user", _connection);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate", "Can't set lims_user role:\r\n" + ex.Message);
                _connection.Close();
            }

            //this extension node may be under a result or a test node.  Create a sql statement to get worksheet_id accordingly.
            string sqlString;
            switch ((string)(Parameters["TABLE_NAME"].ToString())) {
                case "TEST":
                    sqlString = "select distinct worksheet_id from lims_sys.result where worksheet_id is not null and test_id = " + Parameters["PRIMARY_KEY"];
                    break;
                case "RESULT":
                    sqlString = "select worksheet_id from lims_sys.result where result_id = " + Parameters["PRIMARY_KEY"];
                    break;
                default:
                    ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate",
                        "Attempted to run Worksheet QC Calculate extension as a workflow node under a parent node that is not a TEST or RESULT in workflow " + Parameters["WORKFLOW_ID"]);
                    _connection.Close();
                    return;
            }

            //run the command to get the worksheet_id 
            long worksheetId;
            command = new OracleCommand(sqlString, _connection);
            try {
                worksheetId = (long)(OracleNumber)command.ExecuteOracleScalar();
                PurgeWorksheet(worksheetId);
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate", "Connection close error:\r\n" + ex.Message);
                _connection.Close();
            }

            //put the old cursor back
            Cursor.Current = savedCursor;
        }


        /// <summary>
        /// Deletes all QC results that have no matching analyte_id in _any_ SAMP aliquot, 
        /// then deletes any remaining MS/MSD results that have no matching analyte_id in the parent/grandparent SAMP aliquot. 
        /// </summary>
        /// <param name="worksheetId">Worksheet_id of the worksheet to be purged.</param>
        private void PurgeWorksheet(long worksheetId) {

            string sqlString;
            OracleCommand command;
            OracleParameter parameter;
            OracleDataReader reader;


            //query to get a list of worksheet QC results that don't have a corresponding SAMP result (same analyte_id in ANY SAMP aliquot)
            sqlString = "select r.result_id "
                + "from lims_sys.worksheet w, lims_sys.worksheet_entry we, lims_sys.worksheet_aliquot_type wat, "
                + "lims_sys.worksheet_template_test wtt, lims_sys.test t, lims_sys.result r "
                + "where w.worksheet_id = we.worksheet_id "
                + "and wat.worksheet_aliquot_type_id = we.worksheet_aliquot_type_id "
                + "and wtt.worksheet_template_id = w.worksheet_template_id "
                + "and wtt.test_template_id = t.test_template_id "
                + "and we.aliquot_id = t.aliquot_id "
                + "and t.test_id = r.test_id "
                + "and wat.name != 'SAMP' "
                + "and r.analyte_id is not null "
                + "and w.worksheet_id = :in_worksheet_id "
                + "and r.analyte_id not in "
                + "(select distinct r.analyte_id "
                + "from lims_sys.worksheet w, lims_sys.worksheet_entry we, lims_sys.worksheet_aliquot_type wat, "
                + "lims_sys.worksheet_template_test wtt, lims_sys.test t, lims_sys.result r "
                + "where w.worksheet_id = we.worksheet_id "
                + "and wat.worksheet_aliquot_type_id = we.worksheet_aliquot_type_id "
                + "and wtt.worksheet_template_id = w.worksheet_template_id "
                + "and wtt.test_template_id = t.test_template_id "
                + "and we.aliquot_id = t.aliquot_id "
                + "and t.test_id = r.test_id "
                + "and wat.name = 'SAMP' "
                + "and r.analyte_id is not null "
                + "and w.worksheet_id = :in_worksheet_id) ";

            command = new OracleCommand(sqlString, _connection);
            parameter = new OracleParameter(":in_worksheet_id", worksheetId);
            command.Parameters.Add(parameter);


            //delete each result retruned by the query
            try {
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    DeleteResult(long.Parse(reader["result_id"].ToString()));
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcDelete", "Could not get deletable QC results for worksheet " + worksheetId.ToString() + ":\r\n" + ex.Message);
                return;
            }


            //query to get a list of worksheet MS/MSD results that don't have a corresponding SAMP result (same analyte_id in parent/grandparent SAMP aliquot)
            sqlString = "select r.result_id "
                + "from lims_sys.worksheet w, lims_sys.worksheet_entry we, lims_sys.worksheet_aliquot_type wat, lims_sys.worksheet_template_test wtt, "
                + "lims_sys.test t, lims_sys.result r "
                + "where w.worksheet_id = we.worksheet_id "
                + "and we.worksheet_aliquot_type_id = wat.worksheet_aliquot_type_id "
                + "and we.aliquot_id = t.aliquot_id "
                + "and w.worksheet_template_id = wtt.worksheet_template_id "
                + "and wtt.test_template_id = t.test_template_id "
                + "and t.test_id = r.test_id "
                + "and wat.name = 'MS' "
                + "and w.worksheet_id = :in_worksheet_id "
                + "and r.analyte_id not in " 
                + "(select rp.analyte_id "
                + "from lims_sys.worksheet wp, lims_sys.worksheet_entry wep, lims_sys.worksheet_aliquot_type watp, lims_sys.worksheet_template_test wttp, "
                + "lims_sys.test tp, lims_sys.result rp "
                + "where wp.worksheet_id = wep.worksheet_id "
                + "and wep.worksheet_aliquot_type_id = watp.worksheet_aliquot_type_id "
                + "and wep.aliquot_id = tp.aliquot_id "
                + "and wp.worksheet_template_id = wttp.worksheet_template_id "
                + "and wttp.test_template_id = tp.test_template_id "
                + "and tp.test_id = rp.test_id "
                + "and watp.name = 'SAMP' "
                + "and wp.worksheet_id = :in_worksheet_id "
                + "and wep.worksheet_order = we.parent_entry_number) "
                + "union "
                + "select r.result_id "
                + "from lims_sys.worksheet w, lims_sys.worksheet_entry we, lims_sys.worksheet_aliquot_type wat, lims_sys.worksheet_template_test wtt, "
                + "lims_sys.test t, lims_sys.result r "
                + "where w.worksheet_id = we.worksheet_id "
                + "and we.worksheet_aliquot_type_id = wat.worksheet_aliquot_type_id "
                + "and we.aliquot_id = t.aliquot_id "
                + "and w.worksheet_template_id = wtt.worksheet_template_id "
                + "and wtt.test_template_id = t.test_template_id "
                + "and t.test_id = r.test_id "
                + "and wat.name = 'MSD' "
                + "and w.worksheet_id = :in_worksheet_id "
                + "and r.analyte_id not in " 
                + "(select rg.analyte_id "
                + "from lims_sys.worksheet wp, lims_sys.worksheet_entry wep, lims_sys.worksheet_aliquot_type watp, lims_sys.worksheet_template_test wttp, "
                + "lims_sys.worksheet_entry weg, lims_sys.worksheet_aliquot_type watg, "
                + "lims_sys.test tg, lims_sys.result rg "
                + "where wp.worksheet_id = wep.worksheet_id "
                + "and wp.worksheet_id = weg.worksheet_id "
                + "and wep.worksheet_aliquot_type_id = watp.worksheet_aliquot_type_id "
                + "and weg.worksheet_aliquot_type_id = watg.worksheet_aliquot_type_id "
                + "and weg.aliquot_id = tg.aliquot_id "
                + "and wp.worksheet_template_id = wttp.worksheet_template_id "
                + "and wttp.test_template_id = tg.test_template_id "
                + "and tg.test_id = rg.test_id "
                + "and watg.name = 'SAMP' "
                + "and wp.worksheet_id = :in_worksheet_id "
                + "and wep.worksheet_order = we.parent_entry_number "
                + "and weg.worksheet_order = wep.parent_entry_number)";

            command = new OracleCommand(sqlString, _connection);
            parameter = new OracleParameter(":in_worksheet_id", worksheetId);
            command.Parameters.Add(parameter);

            try {
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    DeleteResult(long.Parse(reader["result_id"].ToString()));
                }

                reader.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcDelete", "Could not get deletable MS results for worksheet " + worksheetId.ToString() + ":\r\n" + ex.Message);
                return;
            }

            CompleteWorksheetTests(worksheetId);
        }


        /// <summary>
        /// Deletes specified result record by removing records in aqc_result, result_note, result_user and result tables.
        /// </summary>
        /// <param name="resultId">The result_id of the result to be deleted.</param>
        private void DeleteResult(long resultId) {
            string sqlStringAqc = "delete from lims_sys.aqc_result where result_id = :in_result_id";
            string sqlStringNote = "delete from lims_sys.result_note where result_id = :in_result_id";
            string sqlStringUser = "delete from lims_sys.result_user where result_id = :in_result_id";
            string sqlStringResult = "delete from lims_sys.result where result_id = :in_result_id";

            OracleParameter parameter = new OracleParameter("in_result_id", resultId);
            OracleTransaction transaction = _connection.BeginTransaction();
            OracleCommand command = new OracleCommand();
            command.Connection = _connection;
            command.Parameters.Add(parameter);
            command.Transaction = transaction;

            try {
                command.CommandText = sqlStringAqc;
                command.ExecuteNonQuery();

                command.CommandText = sqlStringNote;
                command.ExecuteNonQuery();

                command.CommandText = sqlStringUser;
                command.ExecuteNonQuery();

                command.CommandText = sqlStringResult;
                command.ExecuteNonQuery();

                transaction.Commit();

            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcDelete", "Error deleting result " + resultId.ToString() + ":\r\n" + ex.Message);
                transaction.Rollback();
            }
        }


        /// <summary>
        /// Sets status of worksheet tests to 'C' if all results under the test (after deletion) are in 'A','R','C','X','I'
        /// </summary>
        /// <param name="worksheetId">The worksheet_id to complete tests</param>
        private void CompleteWorksheetTests(long worksheetId) {
            string sqlString = "update lims_sys.test set status = 'C' "
                + "where status != 'C' and test_id in ("
                + "select t.test_id "
                + "from lims_sys.worksheet w, lims_sys.worksheet_entry we, lims_sys.worksheet_template_test wtt, lims_sys.test t, lims_sys.result r "
                + "where w.worksheet_id = we.worksheet_id "
                + "and we.aliquot_id = t.aliquot_id "
                + "and w.worksheet_template_id = wtt.worksheet_template_id "
                + "and wtt.test_template_id = t.test_template_id "
                + "and t.test_id = r.test_id "
                + "and r.status not in ('V','P','S','M','U','W') "
                + "and w.worksheet_id = :in_worksheet_id )";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            OracleParameter parameter = new OracleParameter(":in_worksheet_id", worksheetId);
            command.Parameters.Add(parameter);

            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcDelete", "Error updating test status(es) to 'C' for worksheet " + worksheetId.ToString() + ":\r\n" + ex.Message);
            }
        }


        public int GetVersion() {
            return VERSION;
        }
    }
}
