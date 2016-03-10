using System;
using System.Data.OracleClient;
using System.IO;
using System.Runtime.InteropServices;
using NautilusExtensions.All;
using LSSERVICEPROVIDERLib;

namespace NautilusExtensions.Qa {

    [Guid("51101095-1186-4F83-ADE3-D5F92128D73F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _S9Assistant : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("6EA60943-F2B2-4980-8941-BF7C548F48E0")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.S9Assistant")]
    public class S9Assistant : _S9Assistant {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        public void Execute(ref LSEXT.LSExtensionParameters Parameters) {

            string operatorName = (string)Parameters["OPERATOR_NAME"];
            int operatorId = (int)Parameters["OPERATOR_ID"];
            int roleId = (int)Parameters["ROLE_ID"];
            bool canChangeSettings = (roleId == 1 || roleId == 68 || roleId == 128  || roleId == 2);

            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = (string)Parameters["SERVER_INFO"];
            ocsb.PersistSecurityInfo = true;
            ocsb.UserID = (string)Parameters["USERNAME"];
            ocsb.Password = (string)Parameters["PASSWORD"];
            ocsb.Unicode = true;


            //set up the nautilus xml processor objects
            NautilusServiceProvider sp = (NautilusServiceProvider)Parameters["SERVICE_PROVIDER"];
            NautilusProcessXML processXml = (NautilusProcessXML)sp.QueryServiceProvider("ProcessXML");
            processXml.SetImportOption(ProcessXMLOption.pxmloShowProgress, ProcessXMLSetting.pxmlNo);

            try {
                using (OracleConnection connection = new OracleConnection(ocsb.ToString())) {
                    connection.Open();

                    using (S9AssistantForm sas = new S9AssistantForm(connection, processXml, operatorName, operatorId)) {
                        sas.ShowDialog();
                    }
                }
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "S9Assistant", "Error using S9Assistant:\r\n" + ex.Message);
            }
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
