using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using LSSERVICEPROVIDERLib;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("E1632FB0-5C76-47E9-B089-5CE63C84ED19")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _VendorUploadExtension : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("38BCD1F6-97AF-4083-894A-445DD08C41D3")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.VendorDataUpload")]
    public class VendorDataUpload : _VendorUploadExtension {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;

        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            //create a connection to the Nautilus database
            OracleConnectionStringBuilder csb = new OracleConnectionStringBuilder();
            csb.DataSource = Parameters["SERVER_INFO"].ToString();
            csb.PersistSecurityInfo = true;
            csb.Unicode = true;
            csb.UserID = Parameters["USERNAME"].ToString();
            csb.Password = Parameters["PASSWORD"].ToString();

            OracleConnection connection = new OracleConnection(csb.ConnectionString);

            try {
                connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "VendorDataUpload", "Error opening Nautilus database connection:\r\n" + ex.Message);
                return;
            }


            //set up the nautilus xml processor objects
            NautilusServiceProvider sp = (NautilusServiceProvider)Parameters["SERVICE_PROVIDER"];
            NautilusProcessXML processXml = (NautilusProcessXML)sp.QueryServiceProvider("ProcessXML");
            processXml.SetImportOption(ProcessXMLOption.pxmloShowProgress, ProcessXMLSetting.pxmlNo);


            //show the main form.  Note that using an OpenFileDialog without a parent form will crash Nautilus probably due to a threading bug.
            VendorDataUploadForm vduf = new VendorDataUploadForm(connection, processXml, _operatorName);
            vduf.ShowDialog();
            return;
        }


        public int GetVersion() {
            return VERSION;
        }

    }
}
