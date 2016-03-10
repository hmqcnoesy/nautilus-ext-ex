using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("596FC75B-6630-4235-A30C-20E47F6BFF8C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _AddPartDescription : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("7670F5F9-11C0-4935-88BE-4A928830A3D2")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.AddPartDescription")]
    public class AddPartDescription : _AddPartDescription {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;

        #region IEntityExtension Members

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            return LSEXT.ExecuteExtension.exEnabled;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            if (Parameters["ENTITY_ID"].ToString() != "88") {
                ErrorHandler.LogError(_operatorName, "AddPartDescription", "This extension is for sample workflows only.");
                return;
            }

            string partDescription = string.Empty;
            OracleConnection connection;
            OracleCommand command;
            OracleCommand updateCommand;
            OracleDataReader reader;
            string sqlString;

            // apparently the hard-coded connection string is required to make use of the explicit dblink to ADCAR
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=lims_app_is"
                + ";Password=mdm3_rj"
                + ";Unicode=True;";


            try {
                connection = new OracleConnection(connString);
                connection.Open();
                command = new OracleCommand();
                command.Connection = connection;

                updateCommand = new OracleCommand();
                updateCommand.Connection = connection;
                updateCommand.CommandText = "set role lims_user";
                updateCommand.ExecuteNonQuery();

                ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];

                while (!records.EOF) {
                    sqlString = "select trim(part_dsc) part_dsc "
                        + "from ecms.item_master@adcar "
                        + "where part_nbr in (select rpad(parameter_2, 32)"
                        + "from lims_sys.workflow_node "
                        + "where workflow_id = " + records.Fields[0].Value.ToString() + " "
                        + "and workflow_prompt_column = 'U_PART_NUMBER' "
                        + "and parameter_3 = '160') ";
                    command.CommandText = sqlString;
                    reader = command.ExecuteReader();

                    if (reader.Read()) {
                        partDescription = reader["part_dsc"].ToString();
                        //first update the description field assignment node in the selected workflow
                        sqlString = "update lims_sys.workflow_node set parameter_2 = '" + partDescription + "' "
                            + "where workflow_id = " + records.Fields[0].Value.ToString() + " "
                            + "and workflow_prompt_column = 'DESCRIPTION' "
                            + "and parameter_3 = '28' "
                            + "and parameter_5 = 'SAMPLE' ";
                        updateCommand.CommandText = sqlString;
                        updateCommand.ExecuteNonQuery();

                        //also update the description of the workflow itself
                        sqlString = "update lims_sys.workflow set description = '" + partDescription + "' "
                            + "where workflow_id = " + records.Fields[0].Value.ToString();
                        updateCommand.CommandText = sqlString;
                        updateCommand.ExecuteNonQuery();

                        MessageBox.Show("Workflow " + records.Fields[0].Value.ToString() + " was updated with description:" + Environment.NewLine + partDescription);
                    } else {
                        ErrorHandler.LogError(_operatorName, "AddPartDescription", "WARNING!  Workflow " + records.Fields[0].Value.ToString() + " did not get an updated description.  The are two possible causes:  "
                            + "\r\n1)  There is no is no part number field assignment node in this workflow. "
                            + "\r\n2)  The value in this workflow's part number field assignment node does not match any part in the ITEM_MASTER. "
                            + "\r\nCheck that the workflow has a field assignment node and a correct part number value assigned."
                            + "\r\nIf you are sure the workflow is correct, the part number may need to be added to ECMS/ADCAR.");
                    }

                    reader.Close();
                    records.MoveNext();
                }

                connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "AddPartDescription", "Error updating descriptions:" + Environment.NewLine + ex.Message);
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
