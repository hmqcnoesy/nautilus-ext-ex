using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NautilusExtensions.All {

    [Guid("AC23DCB8-2F35-40BB-9168-385AE93CC92B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _WorksheetQcCalculate : LSEXT.IEntityExtension, LSEXT.IWorkflowExtension, LSEXT.IVersion {
    }

    [Guid("A04928E6-4B01-45F8-A7A4-BE793D1877B3")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.All.WorksheetQcCalculate")]
    public class WorksheetQcCalculate : _WorksheetQcCalculate {

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

            //making sure that the selected entity is a worksheet
            string sqlString = "select schema_entity_id from lims_sys.schema_entity where name = 'Worksheet' ";
            command = new OracleCommand(sqlString, _connection);
            int worksheetEntityId;
            int entityId = (int)Parameters["ENTITY_ID"];

            try {
                worksheetEntityId = (int)(OracleNumber)command.ExecuteOracleScalar();

                if (entityId != worksheetEntityId) {
                    ErrorHandler.LogMessage(_operatorName, "WorksheetResultReset", "Attempted to execute this extension on wrong entity (" + entityId + ").");
                    _connection.Close();
                    return;
                }
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "WorksheetQcCalculate", "Error getting worksheet entity id:\r\n" + ex.Message);
                _connection.Close();
                return;
            }

            //loop through each of the selected worksheets
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];

            while (!records.EOF) {
                CalculateWorksheetResults(long.Parse(records.Fields[0].Value.ToString()));
                records.MoveNext();
            }

            //close connection
            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate", "Connection close error:\r\n" + ex.Message);
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
                CalculateWorksheetResults(worksheetId);
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate", "Connection close error:\r\n" + ex.Message);
                _connection.Close();
            }

            //put the old cursor back
            Cursor.Current = savedCursor;
        }

        /// <summary>
        /// Runs the result calculations for every result in a worksheet
        /// </summary>
        /// <param name="worksheetId">Nautilus worksheet_id</param>
        private void CalculateWorksheetResults(long worksheetId) {
            //get all the rpd results on the worksheet
            string sqlString = "select r.result_id, ru.u_plsql_procedure, ru.u_plsql_params "
                + "from lims_sys.worksheet_entry we, lims_sys.worksheet w, "
                + "lims_sys.worksheet_template_test wtt, lims_sys.test t, lims_sys.result r, lims_sys.result_user ru "
                + "where w.worksheet_id = we.worksheet_id "
                + "and wtt.worksheet_template_id = w.worksheet_template_id "
                + "and wtt.test_template_id = t.test_template_id "
                + "and we.aliquot_id = t.aliquot_id "
                + "and t.test_id = r.test_id "
                + "and r.result_id = ru.result_id "
                + "and r.status <> 'X' "
                + "and ru.u_plsql_procedure is not null "
                + "and w.worksheet_id = " + worksheetId + " "
                + "order by result_id";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            OracleDataReader reader;
            try {
                
                reader = command.ExecuteReader();
                while (reader.Read()) {
                    UpdateResult(long.Parse(reader["result_id"].ToString()), reader["u_plsql_procedure"].ToString(), 
                        reader["u_plsql_params"].ToString(), worksheetId);
                }

                reader.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate", "Error retrieving results associated with worksheet " + worksheetId + ":\r\n" + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Calls plsql procedure to calculate the result, formats the returned out value, and updates result table accordingly.
        /// </summary>
        /// <param name="resultId">Nautilus result_id</param>
        /// <param name="resultTemplateId">Nautilus result_template_id</param>
        private void UpdateResult(long resultId, string plSqlProcedureName, string plSqlParameters, long worksheetId) {
            OracleCommand command;
            string formattedResult, sqlString;
            try {

                //set up the parameters required for the procedure.
                command = new OracleCommand("lims_app_is." + plSqlProcedureName, _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add("in_result_id", OracleType.Int32).Value = resultId;
                command.Parameters.Add("out_value", OracleType.Number).Direction = System.Data.ParameterDirection.Output;

                //if there is an additional parameter for the result in the u_plsql_params column, add it also
                if (!string.IsNullOrEmpty(plSqlParameters)) {
                    command.Parameters.Add("other_parameter", OracleType.VarChar).Value = plSqlParameters;
                }

                command.ExecuteNonQuery();


                //for now (3/31/09) do not do any updates in the extension code.  PL/SQL procedure handles everything.
                //update the database with the new formatted_result, etc.
                //Modify the result value returned by the plsql procedure as necessary
                //formattedResult = FormatResultValue((decimal)command.Parameters["out_value"].Value, resultId);
                //sqlString = "update lims_sys.result set formatted_result = '" + formattedResult + "', "
                //    + "completed_on = to_date('" + DateTime.Now.ToString("dd-MMM-yyyy HH:mm") + "','DD-MON-YYYY HH24:MI'), "
                //    + "worksheet_id = " + worksheetId.ToString() + " "
                //    + "where result_id = " + resultId.ToString();
                sqlString = "update lims_sys.result set worksheet_id = " + worksheetId.ToString() + " "
                    + "where result_id = " + resultId.ToString();

                command = new OracleCommand(sqlString, _connection);
                command.ExecuteNonQuery();
            } catch (Exception ex){
                ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate", 
                    string.Format("Error calculating result {0} -- Using procedure {1}\r\n{2}", resultId.ToString(), plSqlProcedureName, ex.Message));
            }
        }

        /// <summary>
        /// Checks a result's limits and format in the result template and modifies the result value accordingly.
        /// </summary>
        /// <param name="resultValue">The result value returned by the plsql procedure</param>
        /// <param name="resultTemplateId">The result's template</param>
        /// <returns></returns>
        private string FormatResultValue(decimal resultValue, long resultId) {

            string formattedResult = string.Empty;

            //get the formatting information from the result template
            //if modified limits exist in the result_user table, they override the result template values.
            string sqlString = "select a.format eql_format, nvl(ru.u_eql_calc, a.numeric_limit) eql_value, "
                + "b.format mdl_format, nvl(ru.u_mdl_calc, b.numeric_limit) mdl_value "
                + "from lims_sys.result r, lims_sys.result_user ru, lims_sys.result_template_limit a, lims_sys.result_template_limit b "
                + "where r.result_id = ru.result_id "
                + "and r.result_template_id = a.result_template_id "
                + "and r.result_template_id = b.result_template_id "
                + "and a.name = 'EQL' "
                + "and b.name = 'MDL' "
                + "and r.result_id = " + resultId;

            OracleCommand command = new OracleCommand(sqlString, _connection);
            OracleDataReader reader;
            decimal mdlValue, eqlValue;
            try {
                //if the reader has any rows, the result value will need to be checked against limits.
                reader = command.ExecuteReader();
                if (reader.HasRows) {
                    reader.Read();
                    mdlValue = (decimal)(OracleNumber)reader["mdl_value"];
                    eqlValue = (decimal)(OracleNumber)reader["eql_value"];

                    //check result value against mdl limit first, then against eql limit
                    if (resultValue < mdlValue) {
                        formattedResult = resultValue.ToString(reader["mdl_format"].ToString());
                    } else if (resultValue < eqlValue) {
                        formattedResult = resultValue.ToString(reader["eql_format"].ToString());
                    }

                }
                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate", "Error getting result template limits:\r\n" + ex.Message);
                return formattedResult;
            }

            //if the formatted result is non-numeric, return it.
            decimal formattedNumericResult;
            if (!decimal.TryParse(formattedResult, out formattedNumericResult)) {
                return formattedResult;
            }

            //the formatted result is still numeric, adjust the significant figures
            sqlString = "select nvl(significant_figures, 0) "
                + "from lims_sys.result_template rt, lims_sys.result r "
                + "where r.result_template_id = rt.result_template_id "
                + "and r.result_id = " + resultId;

            command = new OracleCommand(sqlString, _connection);
            int sigFigs;
            try {
                sigFigs = (int)(OracleNumber)command.ExecuteOracleScalar();
                if (sigFigs < 1) {
                    formattedResult = formattedNumericResult.ToString();
                } else {
                    formattedResult = string.Format("{0:G" + sigFigs.ToString() + "}", formattedNumericResult);
                }
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "WorksheetQcCalculate", "Error getting result template sig fig value:\r\n" + ex.Message);
                return formattedResult;
            }

            return formattedResult;
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
