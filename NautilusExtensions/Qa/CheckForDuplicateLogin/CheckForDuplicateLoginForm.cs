using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NautilusExtensions.Qa {
    public partial class CheckForDuplicateLoginForm : Form {
        
        public CheckForDuplicateLoginForm(DataTable dt) {
            InitializeComponent();
            ListViewItem lvi;

            foreach (DataRow dr in dt.Rows) {
                lvi = new ListViewItem(new string[6] {dr["name"].ToString(), dr["u_part_number"].ToString(), 
                        dr["u_serial_number"].ToString(), dr["u_ip_type"].ToString(), 
                        dr["created_by"].ToString(), dr["created_on"].ToString()}, dr["status"].ToString());
                lvRecentSamples.Items.Add(lvi);
            }
        }
    }
}