using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NautilusExtensions.Qa {
    public partial class S9ConfigSelectionForm : Form {
        public string SelectedConfig { get; private set; }

        public S9ConfigSelectionForm(List<string> configNames) {
            InitializeComponent();
            foreach (string s in configNames) {
                lboxConfigNames.Items.Add(s);
            }
        }

        private void btnOk_Click(object sender, EventArgs e) {
            SelectedConfig = lboxConfigNames.SelectedItem as string;
        }
    }
}
