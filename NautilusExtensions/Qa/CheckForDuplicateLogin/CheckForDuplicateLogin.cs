using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.OracleClient;
using System.Data;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("11A8BE20-5D02-4F50-99A9-CE15A0DCC3FC")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _CheckForDuplicateLogin : LSEXT.IWorkflowExtension, LSEXT.IVersion {
    }

    [Guid("CD5909E4-31E9-4B44-A5DD-558741A1DEC2")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.CheckForDuplicateLogin")]
    public class CheckForDuplicateLogin : _CheckForDuplicateLogin {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        #region IWorkflowExtension Members

        void LSEXT.IWorkflowExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            string operatorName = Parameters["OPERATOR_NAME"].ToString();

            OracleConnection connection;
            OracleDataAdapter adapter;
            DataTable dt = new DataTable();

            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            try {
                connection = new OracleConnection(connString);
                connection.Open();

                string sqlString = "select s.name, s.status, su.u_part_number, su.u_serial_number, su.u_ip_type, "
                    + "o.name created_by, to_char(s.created_on, 'MM/DD/YY') created_on "
                    + "from lims_sys.sample s, lims_sys.sample_user su, lims_sys.operator o, lims_sys.workflow_node wn, lims_sys.workflow w "
                    + "where s.sample_id = su.sample_id "
                    + "and s.created_by = o.operator_id "
                    + "and s.workflow_node_id = wn.workflow_node_id "
                    + "and wn.workflow_id = w.workflow_id "
                    + "and w.workflow_id = " + Parameters["WORKFLOW_ID"].ToString() + " "
                    + "and s.created_on > sysdate-10 "
                    + "order by created_on desc ";

                adapter = new OracleDataAdapter(sqlString, connection);
                adapter.Fill(dt);
                connection.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "CheckForDuplicateLogin", "Error connecting to the database:\r\n" + ex.Message);
            }

            //don't show the form if there are no recent logins
            if (dt.Rows.Count > 0) {
                CheckForDuplicateLoginForm cf = new CheckForDuplicateLoginForm(dt);
                cf.ShowDialog();
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
