using System;
using System.Data.OracleClient;
using System.Text;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {
    public partial class WorkflowFinderForm : Form {

        //By keeping track of the number of times a subtree is called you can ensure that you make each tree node name unique.
        //this is important because some workflows may subtree another workflow more than once.
        //if this situation is not handled by making each node name unique, the subtreed nodes will get added to the first node with the correct parent_id
        private int subtreeCallCount;
        private OracleConnection connection;
        private string operatorName;
        
        public WorkflowFinderForm(string connString, string operatorName) {
            InitializeComponent();

            this.operatorName = operatorName;

            //make database connection and populate list box with tests
            try {
                connection = new OracleConnection(connString);
                connection.Open();
                string sqlString = "select name from lims_sys.test_template where group_id = 7 order by name";
                OracleCommand command = new OracleCommand(sqlString, connection);
                OracleDataReader reader = command.ExecuteReader();

                while (reader.Read()) {
                    lstTests.Items.Add(reader["name"].ToString());
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(this.operatorName, "WorkflowFinderForm", "Error connecting to database:\r\n" + ex.Message);
                this.Close();
                return;
            }
        }

        private void WorkflowFinderForm_Load(object sender, EventArgs e) {
            //Clear the label indicators
            lblWorkflowId.Text = string.Empty;
            lblName.Text = string.Empty;
            lblDescription.Text = string.Empty;
        }

        private void btnClearFields_Click(object sender, EventArgs e) {
            txtWorkflowName.Text = string.Empty;
            txtWorkflowDescription.Text = string.Empty;
            for (int i = 0; i < lstTests.Items.Count; i++) {
                lstTests.SetItemCheckState(i, CheckState.Unchecked);
            }
            chkIncludeAliquot.Checked = false;
            chkIncludeTrashed.Checked = false;
        }

        private void btnFindWorkflows_Click(object sender, EventArgs e) {
            lvMatchingWorkflows.Items.Clear();
            //need to clear the tree view here

            string listOfTestNames = GetTestsString();
            
            if (txtWorkflowName.Text.Equals(string.Empty)
                && txtWorkflowDescription.Text.Equals(string.Empty)
                && listOfTestNames.Equals(string.Empty)) {
                return;
            }

            string sqlString = CreateWorkflowQueryString();
            ListViewItem lvi;

            try {
                OracleCommand command = new OracleCommand(sqlString, connection);
                OracleDataReader reader = command.ExecuteReader();

                while (reader.Read()) {
                    lvi = new ListViewItem(new string[3] {reader["name"].ToString(), reader["workflow_id"].ToString(), 
                        reader["description"].ToString()}, reader["workflow_node_type_id"].ToString() );
                    lvMatchingWorkflows.Items.Add(lvi);
                }

                reader.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "WorkflowFinder", "Error querying for matching workflows:\r\n" + ex.Message);
            }
        }

        private string GetTestsString() {
            StringBuilder sb = new StringBuilder();
            bool isFirstTest = true;

            foreach (object o in lstTests.CheckedItems) {
                if (isFirstTest) {
                    isFirstTest = false;
                } else {
                    sb.Append(", ");
                }
                sb.Append("'" + o.ToString() + "'");
            }

            return sb.ToString();
        }

        private string CreateWorkflowQueryString() {
            StringBuilder sbSql = new StringBuilder("select workflow_id, name, description, workflow_node_type_id from lims_sys.workflow ");

            //include the correct groups, depending on whether user checked "include trashed workflows"
            if (chkIncludeTrashed.Checked) {
                sbSql.Append("where (group_id is null or group_id not in (42, 64)) ");
            } else {
                sbSql.Append("where (group_id is null or group_id not in (21, 42, 64)) ");
            }

            //include correct workflow types, depending on whether user checked "include aliquot workflows"
            if (chkIncludeAliquot.Checked) {
                sbSql.Append("and workflow_node_type_id in (34, 1) ");
            } else {
                sbSql.Append("and workflow_node_type_id = 34 ");
            }

            //check for a workflow name specified
            if (!txtWorkflowName.Text.Equals(string.Empty)) {
                sbSql.Append("and upper(name) like '%" + txtWorkflowName.Text.ToUpper().Trim() + "%' ");
            }

            //check for a workflow description specified
            if (!txtWorkflowDescription.Text.Equals(string.Empty)) {
                sbSql.Append("and upper(description) like '%" + txtWorkflowDescription.Text.ToUpper().Trim() + "%' ");
            }

            //check the test template names
            foreach (object o in lstTests.CheckedItems) {
                sbSql.Append("and workflow_id in (select workflow_id from lims_sys.workflow_node wn, lims_sys.test_template tt "
                    + "where wn.template = tt.test_template_id and wn.workflow_node_type_id = 42 and tt.name = '" + o.ToString() + "') ");
            }

            sbSql.Append("order by name");

            return sbSql.ToString();
        }

        private void lvMatchingWorkflows_SelectedIndexChanged(object sender, EventArgs e) {
            ClearWorkflowTree();
            if (lvMatchingWorkflows.SelectedItems.Count > 0) {
                //Put values into the labels 
                lblWorkflowId.Text = lvMatchingWorkflows.SelectedItems[0].SubItems[1].Text;
                lblName.Text = lvMatchingWorkflows.SelectedItems[0].Text;
                lblDescription.Text = lvMatchingWorkflows.SelectedItems[0].SubItems[2].Text;

                //build the treeview per the workflow
                BuildWorkflowTreeView(lvMatchingWorkflows.SelectedItems[0].SubItems[1].Text);
            }
        }

        private void ClearWorkflowTree() {
            lblWorkflowId.Text = string.Empty;
            lblName.Text = string.Empty;
            lblDescription.Text = string.Empty;
            btnExpandNodes.Text = "+";
            tvWorkflowTree.Nodes.Clear();
        }

        private void BuildWorkflowTreeView(string workflowId) {
            subtreeCallCount = 0;

            //this sql statement gets all nodes in a workflow, and uses decodes to get more info about certain node types
            //if you edit this statement, you should make similar edits (if necessary) to the string in the buildSubTreeNodes Sub
            string sqlString = "select decode(wn.workflow_node_type_id, "
                + "13, wn.long_name || ' [' || decode(wn.parameter_4, 'T', 'Run Multiple', 'Run Once') || ']', "
                + "17, wn.long_name || ' [' || substr(wn.parameter_2, 1, 20) || ']', "
                + "22, wn.long_name || ' [' || substr(wn.parameter_1, 1, 40) || '...]', "
                + "26, wn.long_name || ' [' || substr(wn.parameter_3, 1, 40) || '...]', "
                + "wn.long_name) long_name, w.workflow_id, w.name, w.description, wn.workflow_node_id, "
                + "wn.parent_id, lower(wnt.picture_icon) node_type, wn.template, lower(wnt.name) node_name  "
                + "from lims_sys.workflow_node wn, lims_sys.workflow w, lims_sys.workflow_node_type wnt "
                + "where w.workflow_id = wn.workflow_id "
                + "and wn.workflow_node_type_id = wnt.workflow_node_type_id "
                + "and w.workflow_id = " + workflowId + " "
                + "order by order_number";

            OracleCommand command = new OracleCommand(sqlString, connection);
            OracleDataReader reader = command.ExecuteReader();

            //only the level 0 nodes are part of the treeview's Nodes collection.
            //nodes level 1 or deeper are in their parent node's Nodes collection only.
            TreeNode tn;
            TreeNode tnParent;
            while (reader.Read()) {
                tn = new TreeNode();
                tn.Name = reader["workflow_node_id"].ToString();
                tn.Text = reader["long_name"].ToString();
                tn.ImageKey = reader["node_type"].ToString();
                tn.SelectedImageKey = reader["node_type"].ToString();

                if (string.IsNullOrEmpty(reader["parent_id"].ToString())) {
                    tvWorkflowTree.Nodes.Add(tn);
                } else {
                    tnParent = tvWorkflowTree.Nodes.Find(reader["parent_id"].ToString(), true)[0];
                    tnParent.Nodes.Add(tn);
                }

                //need to get workflow nodes of any subtreed workflows
                if (reader["node_name"].ToString().Equals("subtree")) {
                    subtreeCallCount++;
                    BuildSubTreedWorkflow(reader["template"].ToString(), reader["workflow_node_id"].ToString());
                }

                tn.Expand();
            }
            tvWorkflowTree.ExpandAll();

            reader.Close();
        }

        private void BuildSubTreedWorkflow(string subTreeWorkflowId, string parentNodeId) {
            string sqlString = "select decode(wn.workflow_node_type_id, "
                + "13, wn.long_name || ' [' || decode(wn.parameter_4, 'T', 'Run Multiple', 'Run Once') || ']', "
                + "17, wn.long_name || ' [' || substr(wn.parameter_2, 1, 20) || ']', "
                + "22, wn.long_name || ' [' || substr(wn.parameter_1, 1, 40) || '...]', "
                + "26, wn.long_name || ' [' || substr(wn.parameter_3, 1, 40) || '...]', "
                + "wn.long_name) long_name, '" + subtreeCallCount + "-' || wn.workflow_node_id workflow_node_id, "
                + "decode(wn.parent_id, null, '" + parentNodeId + "', '" + subtreeCallCount + "-' || wn.parent_id) parent_id, "
                + "lower(wnt.picture_icon) node_type, wn.template, lower(wnt.name) node_name "
                + "from lims_sys.workflow_node wn, lims_sys.workflow_node_type wnt "
                + "where wn.workflow_node_type_id = wnt.workflow_node_type_id "
                + "and wn.workflow_id = " + subTreeWorkflowId + " "
                + "order by order_number";

            TreeNode tn;
            TreeNode tnParent;

            try {
                OracleCommand command = new OracleCommand(sqlString, connection);
                OracleDataReader reader = command.ExecuteReader();

                while (reader.Read()) {
                    tn = new TreeNode();
                    tn.Name = reader["workflow_node_id"].ToString();
                    tn.Text = reader["long_name"].ToString();
                    tn.ImageKey = reader["node_type"].ToString();
                    tn.SelectedImageKey = reader["node_type"].ToString();

                    if (string.IsNullOrEmpty(reader["parent_id"].ToString())) {
                        tvWorkflowTree.Nodes.Add(tn);
                    } else {
                        tnParent = tvWorkflowTree.Nodes.Find(reader["parent_id"].ToString(), true)[0];
                        tnParent.Nodes.Add(tn);
                    }

                    //need to get workflow nodes of any subtreed workflows
                    if (reader["node_name"].ToString().Equals("subtree")) {
                        subtreeCallCount++;
                        BuildSubTreedWorkflow(reader["template"].ToString(), reader["workflow_node_id"].ToString());
                    }

                    tn.Expand();
                }

                reader.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "WorkfowFinderForm", "Error building subtreed workflow nodes:\r\n" + ex.Message);
            }
        }

        private void btnExpandNodes_Click(object sender, EventArgs e) {
            if (tvWorkflowTree.Nodes.Count == 0) {
                return;
            }
            
            if (btnExpandNodes.Text.Equals("+")) {
                tvWorkflowTree.ExpandAll();
                tvWorkflowTree.SelectedNode = tvWorkflowTree.Nodes[0];
                btnExpandNodes.Text = "-";
            } else {
                tvWorkflowTree.CollapseAll();
                tvWorkflowTree.SelectedNode = tvWorkflowTree.Nodes[0];
                tvWorkflowTree.Nodes[0].Expand();
                btnExpandNodes.Text = "+";
            }
        }
    }
}