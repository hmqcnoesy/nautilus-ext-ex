using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("3D5BE263-70E2-458B-9A14-EDE024473357")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _FixSampleEvents : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("85784B1E-126A-48C4-BE1E-8D8363EB402D")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.FixSampleEvents")]
    public class FixSampleEvents : _FixSampleEvents {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        #region IEntityExtension Members

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            if (Parameters["ENTITY_ID"].ToString() == "84") {
                return LSEXT.ExecuteExtension.exEnabled;
            } else {
                return LSEXT.ExecuteExtension.exDisabled;
            }
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            string operatorName = Parameters["OPERATOR_NAME"].ToString();

            StringBuilder eventString;
            string connString;
            string sqlString;
            OracleConnection connection;
            OracleCommand command = new OracleCommand();
            OracleCommand updateCommand;
            OracleDataReader reader;

            connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            try {
                connection = new OracleConnection(connString);
                connection.Open();
                command.Connection = connection;

                ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];

                while (!records.EOF) {

                    //query the workflow node table for all the events that should be in the event string
                    sqlString = "select '(' || wn.events || '-' || wn.name || ',' || wn.workflow_id || ',' || wn.order_number || ',' || wn.parameter_3 || ',' || parameter_4 || ')' "
                        + "from lims_sys.workflow_node wn, lims_sys.sample s "
                        + "where s.workflow_node_id = wn.parent_id "
                        + "and s.sample_id = " + records.Fields[0].Value.ToString() + " "
                        + "and wn.events is not null ";

                    eventString = new StringBuilder();
                    command.CommandText = sqlString;
                    reader = command.ExecuteReader();

                    while (reader.Read()) {
                        eventString.Append(reader[0].ToString()); 
                    }

                    reader.Close();

                    //update the sample table with the corrected event string
                    updateCommand = new OracleCommand("set role lims_user", connection);
                    updateCommand.ExecuteNonQuery();

                    sqlString = "update lims_sys.sample "
                        + "set events = '" + eventString.ToString().Replace("'", "''") + "' "
                        + "where sample_id = " + records.Fields[0].Value.ToString();
                    updateCommand.CommandText = sqlString;
                    updateCommand.ExecuteNonQuery();

                    records.MoveNext();
                }


            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "FixSampleEvents", "Error fixing sample events:\r\n" + ex.Message);
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
