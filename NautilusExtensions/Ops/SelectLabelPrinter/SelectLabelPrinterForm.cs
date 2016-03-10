using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {
    public partial class SelectLabelPrinterForm : Form {
        private OracleConnection connection;
        private const string printerPreferenceFileLocation = @"c:\program files\thermo\nautilus\log\extension_printer.txt";

        public SelectLabelPrinterForm(OracleConnection connection) {
            InitializeComponent();
            this.connection = connection;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e) {
            string selectedDestination = string.Empty;
            if (chkCustomDestination.Checked) {
                selectedDestination = txtCustomDestination.Text;
            } else {
                if (lvPrinterDestinations.SelectedItems.Count > 0) {
                    selectedDestination = lvPrinterDestinations.SelectedItems[0].SubItems[2].Text;
                } else {
                    return;
                }
            }

            //write to the file
            if (!string.IsNullOrEmpty(selectedDestination)) {
                FileHelper.OverwriteOrCreateFile(printerPreferenceFileLocation, selectedDestination);
            }

            this.Close();
        }

        private void SelectLabelPrinterForm_Load(object sender, EventArgs e) {

            //get a list of all the printers in the database
            string sqlString = "select name, description, info_text1 "
                + "from lims_sys.destination "
                + "where destination_type = 'P' "
                + "order by name ";
            OracleDataAdapter da = new OracleDataAdapter(sqlString, connection);
            DataTable dt = new DataTable();

            try {
                da.Fill(dt);
                da.Dispose();
            } catch (Exception ex) {
                ErrorHandler.LogError("SelectLabelPrinter", "Error getting list of printer destinations:\r\n" + ex.Message);
            }
            
            ListViewItem lvi;
            foreach (DataRow dr in dt.Rows) {
                lvi = new ListViewItem(new string[3] {dr[0].ToString(), dr[1].ToString(), dr[2].ToString()}, 0);
                lvPrinterDestinations.Items.Add(lvi);
            }
        }

        private void chkCustomDestination_CheckedChanged(object sender, EventArgs e) {
            txtCustomDestination.ReadOnly = (!chkCustomDestination.Checked);
        }
    }
}
