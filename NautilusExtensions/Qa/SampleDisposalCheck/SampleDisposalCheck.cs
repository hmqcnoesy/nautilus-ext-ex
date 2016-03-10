using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.OracleClient;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("61D910B7-DC64-4AFD-8159-5027F4E6FA42")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _SampleDisposalCheck : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("B4203B5D-0E9A-439F-A691-C6386DC41074")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.SampleDisposalCheck")]
    public class SampleDisposalCheck : _SampleDisposalCheck {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private OracleConnection _connection;
        public OracleConnection Connection { get { return _connection; } }

        private string _operatorName;
        public string OperatorName { get { return _operatorName; } }

        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            //Connection string
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            try {
                _connection = new OracleConnection(connString);
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "Sample Disposal Check", "DB connection error:\r\n" + ex.Message);
                return;
            }

            SampleDisposalCheckForm sdcf = new SampleDisposalCheckForm(_operatorName, _connection);
            sdcf.ShowDialog();

            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "Sample Disposal Check", "Connection close error:\r\n" + ex.Message);
            }
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
