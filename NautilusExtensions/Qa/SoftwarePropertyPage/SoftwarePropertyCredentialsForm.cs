using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;

namespace NautilusExtensions.Qa {
    public partial class SoftwarePropertyCredentialsForm : Form {
        private OracleConnection connection;
        private string userName, password;
        
        public SoftwarePropertyCredentialsForm(OracleConnection connection, string signAs) {
            InitializeComponent();
            this.connection = connection;
            txtSignAs.Text = signAs;
            txtUserName.Text = System.Environment.UserName;
            userName = string.Empty;
            password = string.Empty;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            userName = txtUserName.Text;
            password = txtPassword.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            userName = string.Empty;
            password = string.Empty;
            this.Close();
        }

        public string GetUserName() {
            return userName;
        }

        public string GetPassword() {
            return password;
        }
    }
}
