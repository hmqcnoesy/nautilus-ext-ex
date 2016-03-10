using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {

    [Guid("9799C01D-F0D1-4F92-896C-DE1CE778F401")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _ReviewFlagClear : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("F3608E1C-F9C7-4B4D-8638-C41F34B8AD77")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Ops.ReviewFlagClear")]
    public class ReviewFlagClear : _ReviewFlagClear {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private OracleConnection _connection;
        private string _operatorName;

        #region IEntityExtension Members

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            //we just need to check that the selected entity is a worksheet before enabling the extension.
            int entityId = (int)Parameters["ENTITY_ID"];
            int worksheetEntityId = -1;
            
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            try {
                _connection = new OracleConnection(connString);
                _connection.Open();
            } catch (Exception ex){
                ErrorHandler.LogMessage(_operatorName, "ReviewFlagClear", "Connection error:\r\n" + ex.Message);
                return LSEXT.ExecuteExtension.exDisabled;
            }

            string sqlString = "select schema_entity_id from lims_sys.schema_entity where name = 'Worksheet' ";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            try {
                worksheetEntityId = (int)(OracleNumber)command.ExecuteOracleScalar();
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "ReviewFlagClear", "Error getting worksheet entity id:\r\n" + ex.Message);
                if (_connection != null) {
                    _connection.Close();
                }
                return LSEXT.ExecuteExtension.exDisabled;
            } finally {
                _connection.Close();
            }

            if (entityId == worksheetEntityId) {
                return LSEXT.ExecuteExtension.exEnabled;
            } else {
                ErrorHandler.LogMessage(_operatorName, "ReviewFlagClear", "Attempted to execute this extension on wrong entity (" + entityId + ").");
                return LSEXT.ExecuteExtension.exDisabled;
            }
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            _connection = new OracleConnection(connString);

            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "ReviewFlagClear", "Connection error:\r\n" + ex.Message);
                return;
            }

            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            ReviewFlagClearForm rfcf;
            
            while (!records.EOF) {

                rfcf = new ReviewFlagClearForm(_connection, records.Fields[0].Value.ToString(), (int)Parameters["SESSION_ID"]);
                rfcf.ShowDialog();

                records.MoveNext();
            }

            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "ReviewFlagClear", "Closing connection:\r\n" + ex.Message);
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
