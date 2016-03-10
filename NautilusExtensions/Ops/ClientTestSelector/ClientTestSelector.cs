using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OracleClient;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {

    [ProgId("NautilusExtensions.Ops.ClientTestSelector")]
    [ComVisible(true)]
    public partial class ClientTestSelector : UserControl, LSExtensionControlLib.IExtensionControl, LSEXT.IVersion {

        protected LSExtensionControlLib.IExtensionControlSite extSite = null;
        private int _clientId;

        private const int VERSION = 4092;  // increment this value when you make changes to prevent users from running old code

        private OracleConnection _connection;
        private string _connectionString;

        private List<string> _testTemplateList;
        private DataTable _testsDataTable;

        private BindingSource _bindingSource;

        public ClientTestSelector() {
            InitializeComponent();
        }
        
        void LSExtensionControlLib.IExtensionControl.EnterPage() {
        }

        void LSExtensionControlLib.IExtensionControl.ExitPage() {
        }

        void LSExtensionControlLib.IExtensionControl.Internationalise() {
            extSite.SetPageName("Tests");
        }

        void LSExtensionControlLib.IExtensionControl.PreDisplay() {
            try {
                _connection = new OracleConnection(_connectionString);
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError("ClientTestSelector", "Updates will not be possible, there was a problem connecting:\r\n" + ex.Message);
            }
        }

        void LSExtensionControlLib.IExtensionControl.RestoreSettings(int hKey) {
        }

        void LSExtensionControlLib.IExtensionControl.SaveData() {

            dgvTests.CommitEdit(0);
            dgvTests.EndEdit();
            Update();

        }

        void LSExtensionControlLib.IExtensionControl.SaveSettings(int hKey) {
        }

        void LSExtensionControlLib.IExtensionControl.SetReadOnly(bool readOnly) {
            readOnly = false;
        }

        void LSExtensionControlLib.IExtensionControl.SetServiceProvider(object serviceProvider) {
            // need to convert the ADO connection string provided into one the .net oracle client likes
            try {
                OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
                ocsb.Unicode = true;
                ocsb.PersistSecurityInfo = true;

                string adoConnectionString =
                    ((LSSERVICEPROVIDERLib.NautilusServiceProvider)serviceProvider).QueryServiceProvider("DBConnection").GetADOConnectionString();
                string[] parts = adoConnectionString.Split(";".ToCharArray());

                foreach (string s in parts) {
                    if (s.StartsWith("User ID=")) {
                        ocsb.UserID = s.Substring(s.IndexOf("User ID=") + 8);
                    }

                    if (s.StartsWith("Password=")) {
                        ocsb.Password = s.Substring(s.IndexOf("Password=") + 9);
                    }

                    if (s.StartsWith("Data Source=")) {
                        ocsb.DataSource = s.Substring(s.IndexOf("Data Source=") + 12);
                    }
                }
                _connectionString = ocsb.ToString();
            } catch (Exception ex) {
                ErrorHandler.LogError("Error obtaining suitable connection string from NautilusServiceProvider object:\r\n" + ex.Message);
            }
        }

        void LSExtensionControlLib.IExtensionControl.SetSite(object site) 
        {
            if (site != null) 
            {
                extSite = site as LSExtensionControlLib.IExtensionControlSite;
            }
        }

        void LSExtensionControlLib.IExtensionControl.SetupData()
        {
            string sqlString = "set role lims_user";
            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.ExecuteNonQuery();
            
            double clientId = 0;
            bool throwAwayBool;
            extSite.GetDoubleValue("CLIENT_ID", out clientId, out throwAwayBool);
            _clientId = (int)clientId;

            // get a table of associated test templates and whether they're primary
            sqlString = "select decode(c.PRIMARY_TEST, 'T', 'Yes', '') \"Primary\", tt.NAME \"Test\" "
                + "from lims_sys.u_client_test_template c, lims_sys.test_template tt "
                + "where c.test_template_id = tt.test_template_id "
                + "and c.client_id = :in_client_id "
                + "order by tt.name";

            OracleDataAdapter adapter = new OracleDataAdapter(sqlString, _connection);
            adapter.SelectCommand.Parameters.Add(new OracleParameter(":in_client_id", clientId));

            _testsDataTable = new DataTable();
            adapter.Fill(_testsDataTable);

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _testsDataTable;
            dgvTests.DataSource = _bindingSource;

            FillTestsComboBox();
        }



        private void FillTestsComboBox()
        {
            var cmd = new OracleCommand();
            var paramstring = string.Empty;
            var paramlist = new List<OracleParameter>();

            for (var i = 0; i < _testsDataTable.Rows.Count; i++)
            {
                var p = ":in_param_" + i.ToString();
                paramstring += p + ",";
                paramlist.Add(new OracleParameter(p, _testsDataTable.Rows[i][1]));
            }

            var wheresql = _testsDataTable.Rows.Count == 0 
                ? string.Empty 
                : string.Format("where name not in ({0}) ", paramstring.TrimEnd(",".ToCharArray()));
            var sql = string.Format("select name from lims_sys.test_template {0} order by name", wheresql);
            cmd.Connection = _connection;
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(paramlist.ToArray());

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                cmbTestsToAdd.Items.Add(reader[0].ToString());
            }

            reader.Close();
        }


        private void Update() {
            _testsDataTable.AcceptChanges();
            var sqlDelete = "delete from lims_sys.u_client_test_template where client_id = :in_client_id";
            var sqlInsert = "insert into lims_sys.u_client_test_template values (:in_client_id, (select test_template_id from lims_sys.test_template where name = :in_test_template_name), :in_primary)";

            var tx = _connection.BeginTransaction();

            try
            {
                var cmdDelete = new OracleCommand(sqlDelete, _connection, tx);
                cmdDelete.Parameters.Add(new OracleParameter(":in_client_id", _clientId));
                cmdDelete.ExecuteNonQuery();

                foreach (DataRow row in _testsDataTable.Rows)
                {
                    var cmdInsert = new OracleCommand(sqlInsert, _connection, tx);
                    cmdInsert.Parameters.Add(new OracleParameter(":in_client_id", _clientId));
                    cmdInsert.Parameters.Add(new OracleParameter(":in_test_template_name", row["Test"]));
                    cmdInsert.Parameters.Add(new OracleParameter(":in_primary", row["Primary"].ToString() == "Yes" ? "T" : "F"));
                    cmdInsert.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError("Couldn't save changes: " + ex.Message);
                tx.Rollback();
            }
        }

        public int GetVersion() {
            return VERSION;
        }

        private void btnAddTest_Click(object sender, EventArgs e) {
            if (cmbTestsToAdd.SelectedIndex < 0) return;

            var row = _testsDataTable.NewRow();
            row["Test"] = cmbTestsToAdd.Text;
            row["Primary"] = chkPrimary.Checked ? "Yes" : string.Empty;
            _testsDataTable.Rows.Add(row);
            extSite.SetModifiedFlag();

            cmbTestsToAdd.Items.RemoveAt(cmbTestsToAdd.SelectedIndex);
        }


        private List<string> GetCurrentlySelectedTests()
        {
            var tests = new List<string>();

            foreach (DataGridViewRow row in dgvTests.Rows)
            {
                tests.Add(row.Cells["Test"].Value.ToString());
            }

            return tests;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var row = dgvTests.CurrentRow;
            if (row == null) return;

            int? rowIndexToDelete = null;

            for (var i = 0; i < _testsDataTable.Rows.Count; i++)
            {
                if (_testsDataTable.Rows[i].RowState == DataRowState.Deleted) continue;
                if (_testsDataTable.Rows[i]["Test"] == row.Cells["Test"].Value)
                {
                    rowIndexToDelete = i;
                }
            }

            if (rowIndexToDelete.HasValue)
            {
                _testsDataTable.Rows[rowIndexToDelete.Value].Delete();
                _bindingSource.ResetBindings(false);
                extSite.SetModifiedFlag();
            }
        }
    }
}
