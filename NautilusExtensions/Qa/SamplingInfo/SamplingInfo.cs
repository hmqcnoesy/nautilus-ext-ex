using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa.SamplingInfo {

    [Guid("1BF8CE91-BCF5-4778-886D-88BD14B2D0AD")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _SamplingInfo : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("7857F595-427D-42AA-93EB-868ADAD6B033")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.SamplingInfo")]
    public class SamplingInfo : _SamplingInfo {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        public LSEXT.ExecuteExtension CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            if (Parameters["ENTITY_ID"].ToString() == "84") {
                return LSEXT.ExecuteExtension.exEnabled;
            } else {
                return LSEXT.ExecuteExtension.exDisabled;
            }
        }

        public void Execute(ref LSEXT.LSExtensionParameters Parameters) {

            string operatorName = Parameters["OPERATOR_NAME"].ToString();


            OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder();
            builder.DataSource = Parameters["SERVER_INFO"].ToString();
            builder.PersistSecurityInfo = true;
            builder.UserID = Parameters["USERNAME"].ToString();
            builder.Password = Parameters["PASSWORD"].ToString();
            builder.Unicode = true;

            OracleConnection connection = new OracleConnection(builder.ToString());
            OracleCommand command = new OracleCommand("set role lims_user", connection);

            try {
                connection.Open();
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "SamplingInfo", "Error connecting to database:\r\n" + ex.Message);
                return;
            }

            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            int sampleId;
            while (!records.EOF) {
                sampleId = int.Parse(records.Fields[0].Value.ToString());
                SamplingInfoForm sif = new SamplingInfoForm(connection, operatorName, sampleId);
                sif.ShowDialog();
                records.MoveNext();
            }

            try {
                connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "SamplingInfo", "Error disconnecting from database:\r\n" + ex.Message);
            }
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
