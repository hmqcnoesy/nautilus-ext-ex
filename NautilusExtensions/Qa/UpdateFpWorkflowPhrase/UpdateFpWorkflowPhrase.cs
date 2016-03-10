using System;
using System.Data;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("65FF6ABB-51C4-4F99-B181-F473E948004B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _UpdateFpWorkflowPhrase : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("B8DB8528-2C04-444B-963C-CD14AF8E6666")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.UpdateFpWorkflowPhrase")]
    public class UpdateFpWorkflowPhrase : _UpdateFpWorkflowPhrase {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        #region IGenericExtension Members

        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            string operatorName = Parameters["OPERATOR_NAME"].ToString();
            
            //create a connection string using lims_read instead of current user
            //using lims_read because the username and password must match remote db (ops) when executing lims_app_is.update_fp_workflow_phrase
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=lims_read"
                + ";Password=lims_read_prod7"
                + ";Unicode=True;";

            try {
                OracleConnection connection = new OracleConnection(connString);
                connection.Open();

                OracleCommand command = new OracleCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "lims_app_is.update_fp_workflows_phrase";
                command.ExecuteNonQuery();

                connection.Close();
                MessageBox.Show("FP Workflows phrase procedure execution complete.");

            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "UpdateFpWorkflowPhrase", "Error updating FP Workflows phrase:" + Environment.NewLine + ex.Message);
            }
        }

        #endregion

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
