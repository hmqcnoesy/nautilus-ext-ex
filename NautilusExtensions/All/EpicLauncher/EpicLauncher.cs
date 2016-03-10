using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data.OracleClient;
using Microsoft.Win32;
using System.Data;

namespace NautilusExtensions.All {

    [Guid("0B1DB3D7-C230-4116-B94B-A31B90AAC11A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _EpicLauncher : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("C7271FEC-288D-4117-A8AA-35628BE9652A")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.All.EpicLauncher")]
    public class EpicLauncher : _EpicLauncher {

        private const int VERSION = 4094;  // increment this value when you make changes to prevent users from running old code
        
        public int GetVersion() {
            return VERSION;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            
            //get a comma-separated list of selected sdg ids
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            string queryString = "dbName=" + Parameters["SERVER_INFO"];
            while (!records.EOF) {
                queryString += "&ids=" + records.Fields[0].Value.ToString();
                records.MoveNext();
            }
            
            using (var form = new EpicWebLauncherForm(queryString))
            {
                form.ShowDialog();
            }

        }


        public LSEXT.ExecuteExtension CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            return LSEXT.ExecuteExtension.exEnabled;
        }
    }
}
