using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.OracleClient;

namespace NautilusExtensions.All {

    [Guid("CE64CB99-569F-450C-9FA9-B5D01285379D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _TestReset : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("746C081D-42C3-40EB-8217-229078C0CD24")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ProgId("NautilusExtensions.All.TestReset")]
    public class TestReset : _TestReset {
        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion

        #region IEntityExtension Members

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            bool returnValue = true;
            OracleConnection connection;
            OracleCommand command;
            OracleDataReader reader;
            string operatorName = Parameters["OPERATOR_NAME"].ToString();
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            //only allow execution on an aliquot entity
            if ((int)Parameters["ENTITY_ID"] != 2) {
                ErrorHandler.LogMessage(operatorName, "TestReset", "Attempted to run extension on entity other than aliquot.");
                return LSEXT.ExecuteExtension.exDisabled;
            }

            //get a comma-separated list of selected aliquot ids
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            string entityIds = string.Empty;
            bool firstRecord = true;
            while (!records.EOF) {
                if (firstRecord) {
                    entityIds += records.Fields[0].Value.ToString();
                    firstRecord = false;
                } else {
                    entityIds += ", " + records.Fields[0].Value.ToString();
                }
                records.MoveNext();
            }

            //connect to database and verify the status of selected entities
            connection = new OracleConnection(connString);

            try {
                connection.Open();
                string sqlString = "select distinct a.status "
                    + "from lims_sys.aliquot a, lims_sys.test t "
                    + "where a.aliquot_id = t.aliquot_id "
                    + "and a.aliquot_id in (" + entityIds + ") "
                    + "and t.status = 'C' ";

                command = new OracleCommand(sqlString, connection);
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    if (!reader["status"].ToString().Equals("C") && !reader["status"].ToString().Equals("P")) {
                        returnValue = false;
                    }
                }

                reader.Close();
                connection.Close();

            } catch (Exception ex) {
                ErrorHandler.LogMessage(operatorName, "TestReset", "Database error:\r\n" + ex.Message);
                returnValue = false;
            }

            //if returnValue never got set to false, extension can be enabled
            if (returnValue) {
                return LSEXT.ExecuteExtension.exEnabled;
            } else {
                return LSEXT.ExecuteExtension.exDisabled;
            }

        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            string operatorName = Parameters["OPERATOR_NAME"].ToString();

            //only allow execution on an aliquot entity
            if ((int)Parameters["ENTITY_ID"] != 2) {
                ErrorHandler.LogError(operatorName, "TestReset", "Attempted to run extension on entity other than aliquot.");
                return;
            }

            OracleConnection connection;
            OracleCommand command;
            OracleDataReader reader;
            string sqlString;

            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = Parameters["SERVER_INFO"].ToString();
            ocsb.PersistSecurityInfo = true;
            ocsb.Unicode = true;
            ocsb.UserID = Parameters["USERNAME"].ToString();
            ocsb.Password = Parameters["PASSWORD"].ToString();

            //get a comma-separated list of selected aliquot ids
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            string entityIds = string.Empty;
            bool firstRecord = true;
            while (!records.EOF) {
                if (firstRecord) {
                    entityIds += records.Fields[0].Value.ToString();
                    firstRecord = false;
                } else {
                    entityIds += ", " + records.Fields[0].Value.ToString();
                }
                records.MoveNext();
            }

            //create the form, which will be populated with test names
            TestResetForm trf = new TestResetForm();

            //connect to database and verify that aliquot is in correct status
            using (connection = new OracleConnection(ocsb.ToString())) {

                try {
                    connection.Open();
                    sqlString = "select distinct a.status "
                        + "from lims_sys.aliquot a, lims_sys.test t "
                        + "where a.aliquot_id = t.aliquot_id "
                        + "and a.aliquot_id in (" + entityIds + ") "
                        + "and t.status = 'C' ";

                    command = new OracleCommand(sqlString, connection);
                    reader = command.ExecuteReader();

                    while (reader.Read()) {
                        if (!reader["status"].ToString().Equals("C") && !reader["status"].ToString().Equals("P")) {
                            ErrorHandler.LogError(operatorName, "TestReset", "Invalid status for using this extension: " + reader["status"]);
                            return;
                        }
                    }

                    reader.Close();

                } catch (Exception ex) {
                    ErrorHandler.LogMessage(operatorName, "TestReset", "Database error:\r\n" + ex.Message);
                    return;
                }


                try {
                    sqlString = "select distinct name "
                        + "from lims_sys.test "
                        + "where aliquot_id in (" + entityIds + ") "
                        + "and status = 'C' ";
                    command = new OracleCommand(sqlString, connection);
                    reader = command.ExecuteReader();

                    while (reader.Read()) {
                        trf.lboxTests.Items.Add(reader["name"].ToString());
                    }

                    reader.Close();

                } catch (Exception ex) {
                    ErrorHandler.LogError(operatorName, "TestReset", "Database error:\r\n" + ex.Message);
                    return;
                }

                trf.ShowDialog();
                string selectedTests = string.Empty;

                //if no tests were selected, just leave
                //make a string of the user-selected tests, like: "'Test Name 1', 'Test Name 2', 'Test Name 3'"
                if (trf.lboxTests.CheckedItems.Count == 0) {
                    try {
                        connection.Close();
                    } catch (Exception ex) {
                        ErrorHandler.LogError(operatorName, "TestReset", "Error closing db connection:\r\n" + ex.Message);
                    }

                    return;
                } else {
                    bool firstItem = true;
                    foreach (object o in trf.lboxTests.CheckedItems) {
                        if (firstItem) {
                            selectedTests += "'" + o.ToString() + "'";
                            firstItem = false;
                        } else {
                            selectedTests += " ,'" + o.ToString() + "'";
                        }
                    }
                }
                ErrorHandler.LogMessage(selectedTests);
                //execute the database transactions
                OracleTransaction transaction;
                transaction = connection.BeginTransaction();

                try {
                    command = new OracleCommand("set role lims_user", connection);
                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    //set the result status to 'X'
                    sqlString = "update lims_sys.result set status = 'X' "
                        + "where status = 'C' "
                        + "and test_id in (select t.test_id from lims_sys.test t where t.aliquot_id in (" + entityIds + ") "
                        + "and t.status = 'C' and t.name in (" + selectedTests + ")) ";
                    command = new OracleCommand(sqlString, connection, transaction);
                    command.ExecuteNonQuery();

                    //append 'V' to the old_status
                    sqlString = "update lims_sys.result set old_status = old_status || 'V' "
                        + "where substr(old_status, length(old_status)) = 'C' "
                        + "and status = 'X' "
                        + "and test_id in (select t.test_id from lims_sys.test t where t.aliquot_id in (" + entityIds + ") "
                        + "and t.status = 'C' and t.name in (" + selectedTests + ")) ";
                    command = new OracleCommand(sqlString, connection, transaction);
                    command.ExecuteNonQuery();

                    //set the status back to 'V'
                    sqlString = "update lims_sys.result set status = 'V' "
                        + "where substr(old_status, length(old_status) - 1) = 'CV' "
                        + "and status = 'X' "
                        + "and test_id in (select t.test_id from lims_sys.test t where t.aliquot_id in (" + entityIds + ") "
                        + "and t.status = 'C' and t.name in (" + selectedTests + ")) ";
                    command = new OracleCommand(sqlString, connection, transaction);
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    connection.Close();

                } catch (Exception ex) {
                    transaction.Rollback();
                    ErrorHandler.LogError(operatorName, "TestReset", "Error updating status:\r\n" + ex.Message);
                }
            }

            Parameters["REFRESH"] = true;

        }

        #endregion

    }
}
