using System;
using System.Windows.Forms;

namespace NautilusExtensions.Ops {
    public partial class PropagateLimitsForm : Form {

        private bool _cancelled;
        private decimal _hbTarget, _ironTarget, _lsbrTarget;

        public bool Cancelled { get { return _cancelled; } }
        public decimal HbTarget { get { return _hbTarget; } }
        public decimal IronTarget { get { return _ironTarget; } }
        public decimal LsbrTarget { get { return _lsbrTarget; } }

        public PropagateLimitsForm(string mixNumber, decimal? hbTarget, decimal? ironTarget, decimal? lsbrTarget) {
            InitializeComponent();
            lblMixNumber.Text = mixNumber;
            txtHbTarget.Text = hbTarget.ToString();
            txtIronTarget.Text = ironTarget.ToString();
            txtLsbrTarget.Text = lsbrTarget.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            _cancelled = true;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e) {
            decimal hbTarget, ironTarget, lsbrTarget;

            if (decimal.TryParse(txtHbTarget.Text, out hbTarget)) {
                _hbTarget = hbTarget;
            } else {
                MessageBox.Show("Provide a numeric value for the HB/ECA target before continuing.");
                txtHbTarget.Focus();
                return;
            }

            if (decimal.TryParse(txtIronTarget.Text, out ironTarget)) {
                _ironTarget = ironTarget;
            } else {
                MessageBox.Show("Provide a numeric value for the iron oxide target before continuing.");
                txtIronTarget.Focus();
                return;
            }

            if (decimal.TryParse(txtLsbrTarget.Text, out lsbrTarget)) {
                _lsbrTarget = lsbrTarget;
            } else {
                MessageBox.Show("Provide a numeric value for the LSBR target before continuing.");
                txtLsbrTarget.Focus();
                return;
            }

            _cancelled = false;
            this.Close();
        }
    }
}
