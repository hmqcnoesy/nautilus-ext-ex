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
    public partial class SampleDisposalCheckForm : Form {

        private OracleConnection _connection;
        private string _operatorName;

        public SampleDisposalCheckForm(string operatorName, OracleConnection connection) {
            InitializeComponent();
            _connection = connection;
            _operatorName = operatorName;
        }

        private void btnSearch_Click(object sender, EventArgs e) {

            bool isSampleAuthorised = false, areAllTestsAuthorised = true;

            tvSample.Nodes.Clear();
            TreeNodeWithInfo tnSample = new TreeNodeWithInfo(0, "s", "sw.gif"), tnAliquot = new TreeNodeWithInfo(0, "a", "aw.gif"),
                tnTest = new TreeNodeWithInfo(0, "t", "tw.gif"), tnResult = new TreeNodeWithInfo(0, "r", "rw.gif");
            int previousTestId = 0, previousAliquotId = 0;

            string sqlString = "select s.sample_id, s.status sample_status, s.name sample_name, "
                + "a.aliquot_id, a.status aliquot_status, a.name aliquot_name, "
                + "t.test_id, t.status test_status, t.name test_name, "
                + "r.result_id, r.status result_status, r.name result_name "
                + "from lims_sys.sample s, lims_sys.aliquot a, lims_sys.test t, lims_sys.result r "
                + "where s.sample_id = a.sample_id(+) "
                + "and a.aliquot_id = t.aliquot_id(+) "
                + "and t.test_id = r.test_id(+) "
                + "and s.name = :in_sample_name "
                + "order by a.aliquot_id, t.test_id, r.result_id";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_sample_name", txtSampleName.Text.ToUpper().Trim()));

            OracleDataReader reader;

            try {
                reader = command.ExecuteReader();

                if (reader.HasRows) {

                    reader.Read();

                    //check the sample status
                    isSampleAuthorised = reader["sample_status"].ToString().Equals("A");

                    //add the sample, first aliquot, first test, first result nodes here
                    tnSample = new TreeNodeWithInfo((int)reader.GetInt32(reader.GetOrdinal("sample_id")), reader["sample_name"].ToString(), "s" + reader["sample_status"].ToString() + ".gif");
                    tvSample.Nodes.Add(tnSample);

                    //if there is an aliquot in this first record add it too
                    if (!reader.IsDBNull(reader.GetOrdinal("aliquot_name"))) {
                        tnAliquot = new TreeNodeWithInfo((int)reader.GetInt32(reader.GetOrdinal("aliquot_id")), reader["aliquot_name"].ToString(), "a" + reader["aliquot_status"].ToString() + ".gif");
                        tnSample.Nodes.Add(tnAliquot);
                        if (reader["aliquot_name"].ToString().Contains("-DR-") && !reader["aliquot_status"].ToString().Equals("X")) areAllTestsAuthorised = false;
                        previousAliquotId = (int)reader.GetInt32(reader.GetOrdinal("aliquot_id"));

                        //if there is a test in first row add it
                        if (!reader.IsDBNull(reader.GetOrdinal("test_name"))) {
                            if (reader["test_status"].ToString().Equals("R")) areAllTestsAuthorised = false;
                            tnTest = new TreeNodeWithInfo((int)reader.GetInt32(reader.GetOrdinal("test_id")), reader["test_name"].ToString(), "t" + reader["test_status"].ToString() + ".gif");
                            tnAliquot.Nodes.Add(tnTest);
                            previousTestId = (int)reader.GetInt32(reader.GetOrdinal("test_id"));

                            //if there is a result in first row add it
                            if (!reader.IsDBNull(reader.GetOrdinal("result_name"))) {
                                tnResult = new TreeNodeWithInfo((int)reader.GetInt32(reader.GetOrdinal("result_id")), reader["result_name"].ToString(), "r" + reader["result_status"].ToString() + ".gif");
                                tnTest.Nodes.Add(tnResult);
                            }
                        }
                    }

                    tnSample.Expand();
                } else {
                    //set these variables so that user gets a "determination cannot be made" message, sound, image.
                    isSampleAuthorised = true;
                    areAllTestsAuthorised = false;
                } // if reader.hasrows


                //loop through remaining records, creating nodes as necessary for each record.
                while (reader.Read()) {
                    if (reader["aliquot_name"].ToString().Contains("-DR-") && !reader["aliquot_status"].ToString().Equals("X")) areAllTestsAuthorised = false;
                    if (reader["test_status"].ToString().Equals("R")) areAllTestsAuthorised = false;

                    if (reader.IsDBNull(reader.GetOrdinal("result_name"))) {
                        //The result is null - so check for a null test.  If test is null it must be a new, empty aliquot.  If not null, it is a new, empty test.
                        if (reader.IsDBNull(reader.GetOrdinal("test_id"))) {
                            tnAliquot = new TreeNodeWithInfo((int)reader.GetInt32(reader.GetOrdinal("aliquot_id")), reader["aliquot_name"].ToString(), "a" + reader["aliquot_status"].ToString() + ".gif");
                            tnSample.Nodes.Add(tnAliquot);
                        } else {
                            if (reader["test_status"].ToString().Equals("R")) tnAliquot.Expand();
                            tnTest = new TreeNodeWithInfo((int)reader.GetInt32(reader.GetOrdinal("test_id")), reader["test_name"].ToString(), "t" + reader["test_status"].ToString() + ".gif");
                            tnAliquot.Nodes.Add(tnTest);
                        }
                    } else {
                        tnResult = new TreeNodeWithInfo((int)reader.GetInt32(reader.GetOrdinal("result_id")), reader["result_name"].ToString(), "r" + reader["result_status"].ToString() + ".gif");

                        //check to see if test_id is same as previous record.  If it is, add result to current test.  If not, create new test and then add result.
                        if ((int)reader.GetInt32(reader.GetOrdinal("test_id")) == previousTestId) {
                            tnTest.Nodes.Add(tnResult);
                        } else {
                            tnTest = new TreeNodeWithInfo((int)reader.GetInt32(reader.GetOrdinal("test_id")), reader["test_name"].ToString(), "t" + reader["test_status"].ToString() + ".gif");
                            tnTest.Nodes.Add(tnResult);
                            previousTestId = (int)reader.GetInt32(reader.GetOrdinal("test_id"));

                            //check for new aliquot
                            if (reader.GetInt32(reader.GetOrdinal("aliquot_id")) == previousAliquotId) {
                                tnAliquot.Nodes.Add(tnTest);
                                if (reader["test_status"].ToString().Equals("R")) tnAliquot.Expand();
                            } else {
                                tnAliquot = new TreeNodeWithInfo((int)reader.GetInt32(reader.GetOrdinal("aliquot_id")), reader["aliquot_name"].ToString(), "a" + reader["aliquot_status"].ToString() + ".gif");
                                tnAliquot.Nodes.Add(tnTest);
                                tnSample.Nodes.Add(tnAliquot);
                                previousAliquotId = (int)reader.GetInt32(reader.GetOrdinal("aliquot_id"));
                                if (reader["test_status"].ToString().Equals("R")) tnAliquot.Expand();
                            }
                        }
                    }
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "SampleDisposalCheckForm", "Error getting dynamic data:\r\n" + ex.Message);
                return;
            }

            //display image, instructions, and play sounds as appropriate depending on status
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
            if (isSampleAuthorised) {
                if (areAllTestsAuthorised) {
                    //"go" sound, image, and instructions
                    pboxStatus.Image = imageList1.Images["green.png"];
                    lblInstructions.Text = "Sample and all tests are authorised.";
                    sp.SoundLocation = "\\\\tpapps\\apps\\nautilus\\custom\\sounds\\go.wav";
                    try {
                        sp.Play();
                    } catch {
                    }
                } else {
                    //"alert" sound, image, and instructions
                    pboxStatus.Image = imageList1.Images["yellow.png"];
                    lblInstructions.Text = "Disposal verification cannot be made.";
                    sp.SoundLocation = "\\\\tpapps\\apps\\nautilus\\custom\\sounds\\alert.wav";
                    try {
                        sp.Play();
                    } catch {
                    }
                }
            } else {
                //"no-go" sound, image, and instructions
                pboxStatus.Image = imageList1.Images["red.png"];
                lblInstructions.Text = "Sample is not authorised.  Do NOT dispose.";
                sp.SoundLocation = "\\\\tpapps\\apps\\nautilus\\custom\\sounds\\nogo.wav";
                try {
                    sp.Play();
                } catch {
                }
            }
            txtSampleName.Focus();
            txtSampleName.SelectAll();
        }

        private void tvSample_AfterSelect(object sender, TreeViewEventArgs e) {
            DataTable dt = ((TreeNodeWithInfo)e.Node).InfoTable;
            int id = ((TreeNodeWithInfo)e.Node).Id;

            if (dt == null) {
                string sqlString = string.Empty;

                //the sql depends on the entity type selected
                switch (e.Node.Level) {
                    case 0:  //sample
                        sqlString = "select s.created_on, s.external_reference, s.description, s.received_on, s.completed_on, s.authorised_on, s.date_results_required, su.* "
                            + "from lims_sys.sample s, lims_sys.sample_user su "
                            + "where s.sample_id = su.sample_id "
                            + "and s.sample_id = :in_id";
                        break;
                    case 1:  //aliquot
                        sqlString = "select a.created_on, a.external_reference, a.description, a.received_on, a.completed_on, a.authorised_on, au.* "
                            + "from lims_sys.aliquot a, lims_sys.aliquot_user au "
                            + "where a.aliquot_id = au.aliquot_id "
                            + "and a.aliquot_id = :in_id";
                        break;
                    case 2:  //test
                        sqlString = "select t.created_on, t.description, t.completed_on, t.authorised_on, tu.* "
                            + "from lims_sys.test t, lims_sys.test_user tu "
                            + "where t.test_id = tu.test_id "
                            + "and t.test_id = :in_id";
                        break;
                    case 3:  //result
                        sqlString = "select r.formatted_result, r.original_result, r.created_on, r.description, r.completed_on, r.authorised_on, ru.* "
                            + "from lims_sys.result r, lims_sys.result_user ru "
                            + "where r.result_id = ru.result_id "
                            + "and r.result_id = :in_id";
                        break;
                    default:
                        break;
                }

                OracleCommand command = new OracleCommand(sqlString, _connection);
                command.Parameters.Add(new OracleParameter(":in_id", id));

                OracleDataReader reader;

                try {
                    reader = command.ExecuteReader();

                    if (reader.Read()) {
                        dt = new DataTable();
                        dt.Columns.Add("Field", Type.GetType("System.String"));
                        dt.Columns.Add("Value", Type.GetType("System.String"));

                        for (int i = 0; i < reader.FieldCount; i++) {
                            dt.Rows.Add(new string[] { reader.GetName(i), reader[i].ToString() });
                        }
                    }

                    reader.Close();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "SampleDisposalCheckForm", "Error getting info for selected node:\r\n" + ex.Message);
                }
            }

            ((TreeNodeWithInfo)e.Node).InfoTable = dt;
            dgvInfo.DataSource = dt;
        }
    }

    public class TreeNodeWithInfo : TreeNode {
        public int Id { get; set; }
        public DataTable InfoTable { get; set; }

        public TreeNodeWithInfo(int id, string textDisplay, string imageKey) {
            this.Id = id;
            this.Name = textDisplay;
            this.Text = textDisplay;
            this.ImageKey = imageKey;
            this.SelectedImageKey = imageKey;
        }
    }
}
