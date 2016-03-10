using System;
using System.Data;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Env {

    [Guid("6D65B32C-A96F-4067-A853-58AA8ED563D4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _TestPriceCalc : LSEXT.IEntityExtension, LSEXT.IWorkflowExtension, LSEXT.IVersion {
    }

    [Guid("D1639785-1A9D-40E9-9FD2-A45BE8498D4A")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Env.TestPriceCalc")]
    public class TestPriceCalc : _TestPriceCalc {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            //only allow execute on test entity (id is 105 in ENV and ENVD)
            if (Parameters["ENTITY_ID"].ToString().Equals("105")) {
                return LSEXT.ExecuteExtension.exEnabled;
            } else {
                return LSEXT.ExecuteExtension.exDisabled;
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

            //open the connection
            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "TestPriceCalc", "Connection error:\r\n" + ex.Message);
                return;
            }


            //set the lims_user role
            OracleCommand command = new OracleCommand("set role lims_user", _connection);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "TestPriceCalc", "Can't set lims_user role:\r\n" + ex.Message);
                _connection.Close();
            }


            //making sure that the selected entity is a test
            string sqlString = "select schema_entity_id from lims_sys.schema_entity where name = 'Test' ";
            command = new OracleCommand(sqlString, _connection);
            int testEntityId;
            int entityId = (int)Parameters["ENTITY_ID"];

            try {
                testEntityId = (int)(OracleNumber)command.ExecuteOracleScalar();

                if (entityId != testEntityId) {
                    ErrorHandler.LogMessage(_operatorName, "TestPriceCalc", "Attempted to execute this extension on wrong entity (" + entityId + ").");
                    _connection.Close();
                    return;
                }
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "TestPriceCalc", "Error getting test entity id:\r\n" + ex.Message);
                _connection.Close();
                return;
            }


            //loop through selected entities and get test price info
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            while (!records.EOF) {
                UpdateTestPrice(long.Parse(records.Fields[0].Value.ToString()));
                records.MoveNext();
            }


            //close connection
            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "TestPriceCalc", "Close connection error:\r\n" + ex.Message);
            }
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
                ErrorHandler.LogError(_operatorName, "TestPriceCalc", "Connection error:\r\n" + ex.Message);
                return;
            }

            //set the lims_user role
            OracleCommand command = new OracleCommand("set role lims_user", _connection);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "TestPriceCalc", "Can't set lims_user role:\r\n" + ex.Message);
                _connection.Close();
            }


            //Allow execution under test workflow node only
            if (Parameters["TABLE_NAME"].ToString().Equals("TEST")) {
                UpdateTestPrice(long.Parse(Parameters["PRIMARY_KEY"].ToString()));
                _connection.Close();
            } else {
                ErrorHandler.LogError(_operatorName, "TestPriceCalc", "Attempted to execute workflow node extension on wrong entity:\r\n" + Parameters["TABLE_NAME"]);
                _connection.Close();
            }


            //put the old cursor back
            Cursor.Current = savedCursor;
            
        }

        private void UpdateTestPrice(long testId) {
            string sqlString;
            OracleCommand command;
            OracleParameter parameter;


            //query for the test prices
            sqlString = "lims_app_is.calc_test_price";

            command = new OracleCommand(sqlString, _connection);
            command.CommandType = CommandType.StoredProcedure;
            parameter = new OracleParameter("in_test_id", testId);
            command.Parameters.Add(parameter);
        
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "TestPriceCalc", "Error running calc_test_price procedure for test " + testId + ":\r\n" + ex.Message);
                return;
            }

            return;
        }


        public int GetVersion() {
            return VERSION;
        }
    }
}
