using System;
using System.Windows.Forms;

namespace NautilusExtensions.All {
    public partial class TestResetForm : Form {
        public TestResetForm() {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            lboxTests.ClearSelected();
            this.Close();
        }
    }
}