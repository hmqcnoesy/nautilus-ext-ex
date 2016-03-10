using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {
    public partial class AelsbrResultsForm : Form {

        private OracleConnection _connection;
        private string _operatorName;
        private string _testName;

        private string[,] _selectedAliquots;
        public string[,] SelectedAliquots { get { return _selectedAliquots; } }

        public AelsbrResultsForm(OracleConnection connection, string operatorName, string testName) {
            InitializeComponent();
            _connection = connection;
            _operatorName = operatorName;
            _testName = testName;
        }

        private void LsbrResultsForm_Load(object sender, EventArgs e) {
            //show available aliquots in list box
            string sqlString = "select distinct a.aliquot_id, a.name, a.status, a.description, au.u_mix_grind_lwr "
                + "from lims_sys.aliquot a, lims_sys.aliquot_user au, lims_sys.test t "
                + "where a.aliquot_id = au.aliquot_id "
                + "and a.aliquot_id = t.aliquot_id "
                + "and t.name = :in_test_name "
                + "and t.status in ('V','P') "
                + "order by aliquot_id ";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_test_name", _testName));
            OracleDataReader reader;

            try {
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    lvAliquots.Items.Add(new ListViewItem(new string[] { reader["name"].ToString(), reader["description"].ToString(), reader["u_mix_grind_lwr"].ToString() }, reader["status"].ToString()));
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "AelsbrResultsForm", "Error getting available aliquots:\r\n" + ex.Message);
                this.Close();
            }
        }

        private void btnOk_Click(object sender, EventArgs e) {
            _selectedAliquots = new string[lvAliquots.SelectedItems.Count, 2];
            int i = 0;
            foreach (ListViewItem lvi in lvAliquots.SelectedItems) {
                _selectedAliquots[i, 0] = lvi.SubItems[0].Text;
                _selectedAliquots[i, 1] = lvi.SubItems[2].Text;
                i++;
            }

            this.Close();
        }
    }
}
