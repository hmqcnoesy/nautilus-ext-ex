using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using System.Runtime.InteropServices;

namespace NautilusExtensions.All {

    [Guid("3F1AE786-9C9B-4B19-88D0-263DE8629C9A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _ConfigureColumns : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("48C94574-8730-4E77-AFB0-4733526AD70B")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.All.ConfigureColumns")]
    public class ConfigureColumns : _ConfigureColumns {
        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        string operatorName;
        OracleConnection connection;

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion

        #region IGenericExtension Members

        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            operatorName = Parameters["OPERATOR_NAME"].ToString();

            //Connection string
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            connection = new OracleConnection(connString);

            try {
                connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ConfigureColumns", "Error connecting to database:\r\n" + ex.Message);
                return;
            }

            ConfigureColumnsForm ccf = new ConfigureColumnsForm(connection, operatorName);
            ccf.ShowDialog();

            int sourceOperatorId = ccf.SourceOperatorId;

            //If the cancel button was clicked, the sourceOperatorId will be zero.
            if (sourceOperatorId == 0) {
                connection.Close();
                return;
            }

            string sqlString = "lims_app.configure_all_columns(" + sourceOperatorId + ", " + Parameters["OPERATOR_ID"].ToString() + ")";

            OracleCommand command = new OracleCommand(sqlString, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ConfigureColumns", "Error running pl/sql procedure:\r\n" + ex.Message);
            }

            connection.Close();

        }

        #endregion
    }
}