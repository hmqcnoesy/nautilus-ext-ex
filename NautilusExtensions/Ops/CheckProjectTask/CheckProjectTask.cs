using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {

    [Guid("F70A9E44-25C9-4B15-8029-3BC0BBF26C84")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _CheckProjectTask : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("FF817E55-1D8F-4E97-8310-71768B5F335A")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Ops.CheckProjectTask")]
    public class CheckProjectTask : _CheckProjectTask {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            return LSEXT.ExecuteExtension.exEnabled;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = Parameters["SERVER_INFO"].ToString();
            ocsb.Unicode = true;
            ocsb.PersistSecurityInfo = true;
            ocsb.UserID = Parameters["USERNAME"].ToString();
            ocsb.Password = Parameters["PASSWORD"].ToString();
            
            _connection = new OracleConnection(ocsb.ToString());

            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "CheckProjectTask", "Error connecting to Nautilus database:\r\n" + ex.Message);
                return;
            }

            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            string sqlString;
            string sdgName = string.Empty;
            string sdgProject = string.Empty;
            string sdgTask = string.Empty;
            OracleCommand command;
            OracleParameter parameter;
            OracleDataReader reader;

            //get ready a separate database connection to MRPprod for checking project/task
            string mrpConnString = "Data Source=MRPprod;Persist Security Info=True;User Id=trs_tc_app;Password=g9oe82eo;Unicode=True;";
            OracleConnection mrpConnection = new OracleConnection(mrpConnString);
            OracleCommand mrpCommand;
            OracleParameter taskParameter, projectParameter;

            try {
                mrpConnection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "CheckProjectTask", "Error connecting to remote database:\r\n" + ex.Message);
            }

            //loop through all sdg records selected in the Nautilus explorer.
            while (!records.EOF) {

                //get the project and task from the sdg_user table
                sqlString = "select s.name, u.u_project PROJECT, u.u_task TASK  "
                    + "from lims_sys.sdg_user u, lims_sys.sdg s "
                    + "where s.sdg_id = u.sdg_id and s.sdg_id = :in_sdg_id";

                try {
                    command = new OracleCommand(sqlString, _connection);
                    parameter = new OracleParameter(":in_sdg_id", records.Fields[0].Value.ToString());
                    command.Parameters.Add(parameter);

                    reader = command.ExecuteReader();
                    sdgProject = string.Empty;
                    sdgTask = string.Empty;

                    while (reader.Read()) {
                        sdgName = reader["name"].ToString();
                        sdgProject = reader["PROJECT"].ToString();
                        sdgTask = reader["TASK"].ToString();
                    }

                    reader.Close();

                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "CheckProjectTask", "Error getting SDG Project:\r\n" + ex.Message);
                }

                //skip the SDG if either project or task field was left empty. 
                if (string.IsNullOrEmpty(sdgProject)) {
                    MessageBox.Show("Cannot check Project/Task for SDG " + sdgName + ".\r\nSDG Project or Task field is empty!");
                    records.MoveNext();
                    continue;
                } else {

                    //check the remote database
                    sqlString = "select count(project) from jit.pj_master "
                        + "where project = :in_project "
                        + "and task = :in_task "
                        + "and plant = '03' and status = 'O' ";

                    try {
                        mrpCommand = new OracleCommand(sqlString, mrpConnection);
                        projectParameter = new OracleParameter(":in_project", sdgProject);
                        taskParameter = new OracleParameter(":in_task", sdgTask);
                        mrpCommand.Parameters.Add(projectParameter);
                        mrpCommand.Parameters.Add(taskParameter);

                        //the scalar result will be a count of the number of records.  
                        //if any records are returned, the project/task is open, if no records returned, it must be closed.
                        if (((int)(OracleNumber)mrpCommand.ExecuteOracleScalar()) > 0) {
                            MessageBox.Show("Project/task (" + sdgProject + "/" + sdgTask + ") for SDG " + sdgName + " is open.");
                        } else {
                            MessageBox.Show(
                                "The project/task (" + sdgProject + "/" + sdgTask + ") for SDG " + sdgName
                                + " is either not open, or is incorrect.  Correct the problem before beginning or proceeding with work.");
                        }

                    } catch (Exception ex) {
                        ErrorHandler.LogError(_operatorName, "CheckProjectTask", "Error getting code status:\r\n" + ex.Message);
                    }
                }

                records.MoveNext();
            }

            mrpConnection.Close();
            _connection.Close();
        }


        public int GetVersion() {
            return VERSION;
        }
    }
}

