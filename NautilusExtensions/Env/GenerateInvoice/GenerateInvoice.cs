using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Env {

    [Guid("C5DDE06C-F205-4E4C-99CE-D3380CB3812B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _GenerateInvoice : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("79C512C1-753D-4EE1-A6DE-98243E43755B")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Env.GenerateInvoice")]
    public class GenerateInvoice : _GenerateInvoice {

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
                ErrorHandler.LogError(_operatorName, "GenerateInvoice", "Connection error:\r\n" + ex.Message);
                return;
            }


            //set the lims_user role
            OracleCommand command = new OracleCommand("set role lims_user", _connection);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "GenerateInvoice", "Can't set lims_user role:\r\n" + ex.Message);
                _connection.Close();
            }


            //making sure that the selected entity is an sdg
            string sqlString = "select schema_entity_id from lims_sys.schema_entity where name = 'SDG' ";
            command = new OracleCommand(sqlString, _connection);
            int sdgEntityId;
            int entityId = (int)Parameters["ENTITY_ID"];

            try {
                sdgEntityId = (int)(OracleNumber)command.ExecuteOracleScalar();

                if (entityId != sdgEntityId) {
                    ErrorHandler.LogMessage(_operatorName, "GenerateInvoice", "Attempted to execute this extension on wrong entity (" + entityId + ").");
                    _connection.Close();
                    return;
                }
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "GenerateInvoice", "Error getting SDG entity id:\r\n" + ex.Message);
                _connection.Close();
                return;
            }


            //loop through each of the selected sdgs
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];

            while (!records.EOF) {
                SdgInvoice(long.Parse(records.Fields[0].Value.ToString()));
                records.MoveNext();
            }

            //close connection
            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "GenerateInvoice", "Connection close error:\r\n" + ex.Message);
            }

            //put the old cursor back
            Cursor.Current = savedCursor;
            
        }
        

        private void SdgInvoice(long sdgId) {
            string sqlString;
            OracleCommand command;
            OracleParameter parameter;


            //query for the test prices
            sqlString = "lims_app_is.generate_invoice";

            command = new OracleCommand(sqlString, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            parameter = new OracleParameter("in_sdg_id", sdgId);
            command.Parameters.Add(parameter);

            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "GenerateInvoice", "Error running generate_invoice procedure for SDG " + sdgId + ":\r\n" + ex.Message);
                return;
            }

            //Matt added 12/10/09 - Kent Bates requested "audio feedback that the invoice has been created."
            System.Media.SystemSounds.Beep.Play();

            return;
        }

        
        public int GetVersion() {
            return VERSION;
        }
    }
}
