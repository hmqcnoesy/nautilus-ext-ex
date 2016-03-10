using System;
using System.Runtime.InteropServices;

namespace NautilusExtensions.Qa {

    [Guid("2514774C-2697-4ECC-A0D6-3FF5DCC5D811")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _WorkflowFinder : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("1208A112-C3E8-4AB1-8813-1B843782524A")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.WorkflowFinder")]
    public class WorkflowFinder : _WorkflowFinder {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        #region IGenericExtension Members

        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            //create a connection string
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            WorkflowFinderForm wff = new WorkflowFinderForm(connString, Parameters["OPERATOR_NAME"].ToString());
            wff.ShowDialog();
        }

        #endregion

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
