using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {

    [Guid("F17BA896-627E-403D-AB20-B8A54411D712")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _SelectLabelPrinter : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("A91AA5E1-B44A-404B-B83D-E9C47D18566E")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Ops.SelectLabelPrinter")]
    public class SelectLabelPrinter : _SelectLabelPrinter {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;

        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            //Connection string
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            _connection = new OracleConnection(connString);

            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "SelectLabelPrinter", "Error connecting to database:\r\n" + ex.Message);
                return;
            }
            
            SelectLabelPrinterForm slpf = new SelectLabelPrinterForm(_connection);
            slpf.ShowDialog();

            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "SelectLabelPrinter", "Closing connection:\r\n" + ex.Message);
            }
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
