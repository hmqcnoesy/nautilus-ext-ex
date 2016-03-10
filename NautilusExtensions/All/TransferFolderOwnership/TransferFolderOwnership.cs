using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace NautilusExtensions.All {

    [Guid("FB4C30A9-04E9-4D76-88DC-C0CADBB878D7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _TransferFolderOwnership : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("8BBB8AB0-341A-405B-8C8A-3ADF169975EF")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.All.TransferFolderOwnership")]
    public class TransferFolderOwnership : _TransferFolderOwnership {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;

        public LSEXT.ExecuteExtension CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            //only allow system role to exectute
            if (!Parameters["ROLE_ID"].ToString().Equals("1")) {
                return LSEXT.ExecuteExtension.exDisabled;
            } else {
                return LSEXT.ExecuteExtension.exEnabled;
            }
        }

        public void Execute(ref LSEXT.LSExtensionParameters Parameters) {

            _operatorName = (string)Parameters["OPERATOR_NAME"];

            //setup the database connection
            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = Parameters["SERVER_INFO"].ToString();
            ocsb.PersistSecurityInfo = true;
            ocsb.UserID = Parameters["USERNAME"].ToString();
            ocsb.Password = Parameters["PASSWORD"].ToString();
            ocsb.Unicode = true;

            using (_connection = new OracleConnection(ocsb.ToString())) {

                string sqlString;

                try {
                    _connection.Open();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "TransferFolderOwnership", "Error connecting to database:\r\n" + ex.Message);
                    return;
                }

                // allow execute only on folder entity
                sqlString = "select schema_entity_id from lims_sys.schema_entity where name = 'Folder'";
                string folderEntityId = string.Empty;
                OracleCommand command = new OracleCommand(sqlString, _connection);

                try {
                    folderEntityId = command.ExecuteScalar().ToString();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "TransferFolderOwnership", "Error getting schema_entity_id of folder entity:\r\n" + ex.Message);
                    return;
                }

                if (!folderEntityId.Equals(Parameters["ENTITY_ID"].ToString())) {
                    ErrorHandler.LogError(_operatorName, "TransferFolderOwnership", "Attempted to execute extension on entity other than folder.");
                    return;
                }

                // get a list of operators available to transfer ownership to
                List<string> operatorNames = new List<string>();
                sqlString = "select name from lims_sys.operator where allow_login = 'T' order by name";
                command = new OracleCommand(sqlString, _connection);
                OracleDataReader reader;

                try {
                    reader = command.ExecuteReader();

                    while (reader.Read()) {
                        operatorNames.Add(reader[0].ToString());
                    }

                    reader.Close();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "TransferFolderOwnership", "Error getting all operator names:\r\n" + ex.Message);
                    return;
                }

                // set role to allow updating
                sqlString = "set role lims_user";
                command = new OracleCommand(sqlString, _connection);

                try {
                    command.ExecuteNonQuery();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "TransferFolderOwnership", "Error setting Oracle role:\r\n" + ex.Message);
                    return;
                }

                // create table of selected folders, with old and new owners

                ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
                string selectedFolders = string.Empty;
                while (!records.EOF) {
                    selectedFolders += records.Fields[0].Value.ToString() + ",";
                    records.MoveNext();
                }

                DataTable foldersTable = new DataTable();

                sqlString = string.Format("select f.folder_id, f.name \"Folder\", o.name \"Owner\", :in_new_owner \"New Owner\" "
                    + "from lims_sys.folder f, lims_sys.operator o "
                    + "where f.operator_id = o.operator_id and f.folder_id in ({0}) order by \"Folder\"", selectedFolders.TrimEnd(",".ToCharArray()));

                OracleDataAdapter adapter = new OracleDataAdapter(sqlString, _connection);
                adapter.SelectCommand.Parameters.Add(new OracleParameter(":in_new_owner", _operatorName));

                try {
                    adapter.Fill(foldersTable);
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "TransferFolderOwnership", "Error retrieving selected folders' info:\r\n" + ex.Message);
                    return;
                }

                // display the form using the data table
                using (TransferFolderOwnershipForm tfofg = new TransferFolderOwnershipForm(foldersTable, operatorNames)) {
                    if (tfofg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                        // the form put the new owners names in the table
                        foreach (DataRow dr in foldersTable.Rows) {
                            UpdateFolderOwnership((int)(decimal)dr["folder_id"], dr["New Owner"].ToString());
                        }
                    }
                }
            }
        }

        private void UpdateFolderOwnership(int folderId, string newOwnerName) {
            string sqlString = "update lims_sys.folder "
                + "set operator_id = (select operator_id from lims_sys.operator where name = :in_operator_name)"
                + "where folder_id = :in_folder_id";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_operator_name", newOwnerName));
            command.Parameters.Add(new OracleParameter(":in_folder_id", folderId));

            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "TransferFolderOwnership",
                    string.Format("Could not update folder {0} to operator {1}:\r\n{2}", folderId, newOwnerName, ex.Message));
            }
        }


        public int GetVersion() {
            return VERSION;
        }
    }
}
