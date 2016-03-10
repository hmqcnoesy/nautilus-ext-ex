using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.OracleClient;
using System.Data;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa
{

    [Guid("8FE07680-9A7E-4EC8-85DD-672A19362370")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _SampleHazardPicker : LSEXT.IWorkflowExtension, LSEXT.IVersion
    {
    }

    [Guid("E2E3D585-9D77-463F-9DCF-75858D19C9CD")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.SampleHazardPicker")]
    public class SampleHazardPicker : _SampleHazardPicker
    {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        #region IWorkflowExtension Members

        void LSEXT.IWorkflowExtension.Execute(ref LSEXT.LSExtensionParameters Parameters)
        {
            string operatorName = Parameters["OPERATOR_NAME"].ToString();

            OracleConnection connection;

            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            try
            {
                using (connection = new OracleConnection(connString))
                {
                    connection.Open();

                    // check that the parent entity is a sample
                    if (Parameters["TABLE_NAME"].ToString() != "SAMPLE")
                    {
                        ErrorHandler.LogError(operatorName, "SampleHazardPicker", "Attempted to execute extension on entity other than SAMPLE");
                        return;
                    }

                    // sample ID
                    int sampleId = int.Parse(Parameters["PRIMARY_KEY"].ToString());

                    using (SampleHazardPickerForm form = new SampleHazardPickerForm(connection, sampleId))
                    {
                        form.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(operatorName, "SampleHazardPicker", "Error connecting to the database:\r\n" + ex.Message);
                return;
            }
        }

        #endregion

        #region IVersion Members

        public int GetVersion()
        {
            return VERSION;
        }

        #endregion
    }
}
