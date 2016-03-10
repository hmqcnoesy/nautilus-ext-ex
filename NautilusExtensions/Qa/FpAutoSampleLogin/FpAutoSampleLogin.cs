using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Data;
using System.Data.OracleClient;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("29AEADE5-FA4C-4EF7-815C-B4444B89682D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _FpAutoSampleLogin : LSEXT.IWorkflowExtension, LSEXT.IVersion {
    }

    [Guid("5E5D0945-9364-48C3-B4F9-B00480B238A1")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.FpAutoSampleLogin")]
    public class FpAutoSampleLogin : _FpAutoSampleLogin {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        //This is the default parsing location, which will be used if the query cannot find the path.
        private string defaultParsingPath = @"\\tpapps\apps\nautilus\parsing\ops\fp_file_login\";
        private string opsConnectionString = "Data Source=ops;Persist Security Info=True;User Id=lims_read;Password=lims_read_prod7;Unicode=True;";
        private string devOpsConnectionString = "Data Source=opsD;Persist Security Info=True;User Id=lims_read;Password=lims_read_prod7;Unicode=True;";

        private string _operatorName;

        #region IWorkflowExtension Members

        void LSEXT.IWorkflowExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            _operatorName = Parameters["OPERATOR_NAME"].ToString();
            
            OracleConnection connection;
            OracleCommand command;
            OracleDataReader reader;
            string sampleName, serialNumber, dueDate, createdBy;

            //it is important to send files to the opsD parser if this is called from qaD to prevent dev data getting into production
            if (!Parameters["SERVER_INFO"].ToString().ToUpper().Trim().Equals("QA")) {
                opsConnectionString = devOpsConnectionString;
            }

            StringBuilder fileText = new StringBuilder("Begin Sample" + Environment.NewLine
                + "\"External_Ref\",\"Workflow_Name\",\"Study_Ref\",\"SDG_Ref\",\"Description\",\"U_LOT_NO\",\"DATE_RESULTS_REQUIRED\"\r\n");

            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            try {
                connection = new OracleConnection(connString);
                connection.Open();

                //get information on current sample being logged into qa
                string sqlString = "select s.name, su.u_serial_number, to_char(s.date_results_required, 'MM/DD/YY') due_date, o.name created_by "
                    + "from lims_sys.sample s, lims_sys.sample_user su, lims_sys.operator o "
                    + "where s.sample_id = su.sample_id "
                    + "and s.created_by = o.operator_id "
                    + "and s.sample_id = " + Parameters["PRIMARY_KEY"].ToString();
                command = new OracleCommand(sqlString, connection);
                reader = command.ExecuteReader();

                if (reader.Read()) {
                    sampleName = reader["name"].ToString();
                    serialNumber = reader["u_serial_number"].ToString();
                    dueDate = reader["due_date"].ToString();
                    createdBy = reader["created_by"].ToString();
                    reader.Close();
                } else {
                    ErrorHandler.LogError(_operatorName, "FpAutoSampleLogin", "No records were returned for current logged sample, sample_id is " + Parameters["PRIMARY_KEY"].ToString());
                    reader.Close();
                    connection.Close();
                    return;
                }

                //get the fingerprinting workflow name(s) from the qa workflow node table
                sqlString = "select parameter_2 from lims_sys.workflow_node "
                    + "where workflow_id = " + Parameters["WORKFLOW_ID"].ToString() + " "
                    + "and workflow_node_type_id = 17 "
                    + "and workflow_prompt_column = 'U_FP_WORKFLOW' ";
                command.CommandText = sqlString;
                reader = command.ExecuteReader();

                if (reader.HasRows) {
                    while (reader.Read()) {
                        fileText.Append("\"" + createdBy + "\",\"" + reader["parameter_2"].ToString() + "\",,,\"" + sampleName + "\",\"" 
                            + serialNumber + "\",\"" + dueDate + "\"\r\n");
                    }

                    fileText.Append("End Sample\r\n");
                } else {
                    ErrorHandler.LogError(_operatorName, "FpAutoSampleLogin", "The workflow has the FP Auto Sample Login Extension but NO FP WORKFLOW FIELD ASSIGNMENT NODES."
                        + "\r\nThe extension cannot auto login the fp sample(s) without these nodes in the workflow.");
                    reader.Close();
                    connection.Close();
                    return;
                }

                reader.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "FpAutoSampleLogin", "Error connecting to database:\r\n" + ex.Message);
                return;
            }

            try {
                //write the file to the file login parsing location
                System.IO.StreamWriter sw = new System.IO.StreamWriter(getFpParsingPath() + sampleName + ".txt");
                sw.Write(fileText);
                sw.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "FpAutoSampleLogin", "Error writing parsing file:\r\n" + ex.Message);
            }

            connection.Close();

            return;
        }

        #endregion

        string getFpParsingPath() {
            
            string sqlString = "select ic.input_file_directory "
                + "from lims_sys.instrument_control ic, lims_sys.instrument i "
                + "where ic.instrument_control_id = i.instrument_control_id "
                + "and i.name = 'File Login' ";
            string returnValue;

            try {
                OracleConnection connection = new OracleConnection(opsConnectionString);
                connection.Open();
                OracleCommand command = new OracleCommand(sqlString, connection);
                OracleDataReader reader = command.ExecuteReader();

                if (reader.Read()) {
                    returnValue = reader["input_file_directory"].ToString();
                } else {
                    ErrorHandler.LogError(_operatorName, "FpAutoSampleLogin", "The parsing directory could not be determined.\r\n" + defaultParsingPath + " will be used.");
                    returnValue = defaultParsingPath;
                }

                reader.Close();
                connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "FpAutoSampleLogin", "Error in determining the parsing directory location:\r\n" + ex.Message);
                returnValue = defaultParsingPath;
            }
            
            return returnValue;
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
