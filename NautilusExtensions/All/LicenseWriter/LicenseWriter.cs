using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data.OracleClient;
using Microsoft.Win32;

namespace NautilusExtensions.All {

    [Guid("D06934C7-2AAD-4BD3-96FC-C9EFA4F50EF7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _LicenseWriter : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("47E201FB-4DA0-417C-9038-EDE9E8E42535")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.All.LicenseWriter")]
    public class LicenseWriter : _LicenseWriter {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            string[] serverAndPort = ((string)Parameters["PARAMETER"]).Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string server = string.Empty, port = string.Empty;

            if (serverAndPort.Length == 2) {
                server = serverAndPort[0];
                port = serverAndPort[1];
            }
            
            using (LicenseWriterForm lwf = new LicenseWriterForm(server, port)) {
                lwf.ShowDialog();
            }
        }


        public int GetVersion() {
            return VERSION;
        }
    }
}
