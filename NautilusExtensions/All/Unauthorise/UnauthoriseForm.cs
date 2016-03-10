using System;
using System.Windows.Forms;

namespace NautilusExtensions.All {
    public partial class UnauthoriseForm : Form {
        private string unauthorisationReason;

        public UnauthoriseForm() {
            InitializeComponent();
            unauthorisationReason = string.Empty;
        }

        private void btnOk_Click(object sender, EventArgs e) {
            if (txtReason.Text.Equals(string.Empty)) {
                return;
            } else {
                unauthorisationReason = txtReason.Text.Replace("'", "''");
                this.Close();
            }
        }

        public string getUnauthorisationReason() {
            return unauthorisationReason;
        }
    }
}