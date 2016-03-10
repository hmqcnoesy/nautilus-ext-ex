using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NautilusExtensions.All
{
    [Guid("F8CBF84D-1D94-41AD-A3B8-ADB3115B89E2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _ImageResultBrowser : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("5785A748-C7D6-4B89-AE6C-A996D44520BF")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.All.ImageResultBrowser")]
    public class ImageResultBrowser : _ImageResultBrowser
    {

        private const int VERSION = 4090;  // increment this value when you make changes to prevent users from running old code

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters)
        {
            return LSEXT.ExecuteExtension.exEnabled;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters)
        {
            var dbName = Parameters["SERVER_INFO"].ToString().ToLower();
            if (dbName != "qa" && dbName != "qad")
            {
                ErrorHandler.LogError("Executing extension on unsupported database, only QA and QAD are allowed: " + dbName);
                return;
            }

            //get a comma-separated list of selected sample ids
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            var queryString = "?j";
            while (!records.EOF)
            {
                queryString += "&id=" + records.Fields[0].Value.ToString();
                records.MoveNext();
            }

            var url = (new UriBuilder(Properties.Settings.Default.ImageResultBrowserUri));
            url.Query = queryString;

            System.Diagnostics.Process.Start(url.ToString());
        }

        int LSEXT.IVersion.GetVersion()
        {
            return VERSION;
        }
    }
}
