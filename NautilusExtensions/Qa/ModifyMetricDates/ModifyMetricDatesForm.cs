using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NautilusExtensions.Qa {
    public partial class ModifyMetricDatesForm : Form {

        private DateTime? _authorisedOn;
        public DateTime? AuthorisedOn { get { return _authorisedOn; } set { _authorisedOn = value; } }

        private DateTime? _receivedOn;
        public DateTime? ReceivedOn { get { return _receivedOn; } set { _receivedOn = value; } }

        private string _reasonForChange;
        public string ReasonForChange { get { return _reasonForChange; } set { _reasonForChange = value; } }

        private bool _updatedDates;
        public bool UpdatedDates { get { return _updatedDates && ((_receivedOn != null) || (_authorisedOn != null)); } }

        public ModifyMetricDatesForm(string itemName, DateTime? authorisedOn, DateTime? receivedOn, string reasonForChange) {
            InitializeComponent();

            lblName.Text = itemName;

            _authorisedOn = authorisedOn;
            if (authorisedOn != null) {
                dtpAuthorisedOn.Value = (DateTime)authorisedOn;
            } else {
                dtpAuthorisedOn.Visible = false;
                lblAuthorisedOn.Visible = false;
            }

            _receivedOn = receivedOn;
            if (receivedOn != null) {
                dtpReceivedOn.Value = (DateTime)receivedOn;
            } else {
                dtpReceivedOn.Visible = false;
                lblReceivedOn.Visible = false;
            }

            _reasonForChange = reasonForChange;
            txtReason.Text = reasonForChange;
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            _authorisedOn = null;
            _receivedOn = null;
            _updatedDates = false;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e) {

            if (string.IsNullOrEmpty(txtReason.Text)) {
                MessageBox.Show("You must provide a reason for modifying the authorisation date.");
                return;
            }

            if (dtpReceivedOn.Visible) {
                _receivedOn = dtpReceivedOn.Value;
            } else {
                _receivedOn = null;
            }

            if (dtpAuthorisedOn.Visible) {
                _authorisedOn = dtpAuthorisedOn.Value;
            } else {
                _authorisedOn = null;
            }
            
            _reasonForChange = txtReason.Text;
            _updatedDates = true;

            this.Close();
        }
    }
}
