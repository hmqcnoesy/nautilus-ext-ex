using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.OracleClient;
using System.Data.Odbc;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("7A374D28-2273-4912-8CE7-18C4F172FF1B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _MicrotracResults : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("A3EB7208-559B-46D0-B181-D20A81C249CD")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.MicrotracResults")]
    public class MicrotracResults : _MicrotracResults {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connectionNautilus;
        private OdbcConnection _connectionMicrotrac;


        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            //get a connection to Nautilus database
            OracleConnectionStringBuilder csb = new OracleConnectionStringBuilder();
            csb.DataSource = Parameters["SERVER_INFO"].ToString();
            csb.PersistSecurityInfo = true;
            csb.UserID = Parameters["USERNAME"].ToString();
            csb.Password = Parameters["PASSWORD"].ToString();
            csb.Unicode = true;

            _connectionNautilus = new OracleConnection(csb.ToString());

            try {
                _connectionNautilus.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "MicrotracResults", "Error connecting to Nautilus database:\r\n" + ex.Message);
                return;
            }

            using (MicrotracResultsForm mrf = new MicrotracResultsForm(_connectionNautilus, _operatorName, (int)Parameters["OPERATOR_ID"])) {
                mrf.ShowDialog();
            }

            try {
                _connectionNautilus.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "MicrotracResults", "Error closing Nautilus database connection:\r\n" + ex.Message);
            }

        }


        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }

    public class MicrotracResult {
        public string AliquotName { get; set; }
        public string ResultName { get; set; }
        public string DbTag1 { get; set; }
        public string DbTag2 { get; set; }
        public string DbTag3 { get; set; }
        public string Note { get; set; }
        public string[] Percentiles { get; set; }
        public string[] PercentileValues { get; set; }
        public string MeanVolume { get; set; }

        public MicrotracResult() {
            // each value indicates the particle size in microns where that corresponding percentage of total particles is equal size or smaller
            Percentiles = new string[10];
            PercentileValues = new string[10];
        }
    }
}
