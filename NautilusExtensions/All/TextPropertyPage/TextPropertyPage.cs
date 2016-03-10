using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NautilusExtensions.All {

    [ProgId("NautilusExtensions.All.TextPropertyPage")]
    [ComVisible(true)]
    public partial class FpSuccessPage : UserControl, LSExtensionControlLib.IExtensionControl, LSEXT.IVersion {
        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        protected LSExtensionControlLib.IExtensionControlSite site = null;

        public FpSuccessPage() {
            InitializeComponent();
        }

        private void txtInfoText_TextChanged(object sender, EventArgs e) {
            site.SetModifiedFlag();
        }

        #region IExtensionControl Members

        void LSExtensionControlLib.IExtensionControl.EnterPage() {
        }

        void LSExtensionControlLib.IExtensionControl.ExitPage() {
        }

        void LSExtensionControlLib.IExtensionControl.Internationalise() {
        }

        void LSExtensionControlLib.IExtensionControl.PreDisplay() {
        }

        void LSExtensionControlLib.IExtensionControl.RestoreSettings(int hKey) {
        }

        void LSExtensionControlLib.IExtensionControl.SaveData() {
            site.SetStringValue("u_info_text", txtInfoText.Text);
        }

        void LSExtensionControlLib.IExtensionControl.SaveSettings(int hKey) {
        }

        void LSExtensionControlLib.IExtensionControl.SetReadOnly(bool readOnly) {
            readOnly = false;
        }

        private string _connectionString;

        void LSExtensionControlLib.IExtensionControl.SetServiceProvider(object serviceProvider) {
            LSSERVICEPROVIDERLib.NautilusDBConnection connection =
                ((LSSERVICEPROVIDERLib.NautilusServiceProvider)serviceProvider).QueryServiceProvider("DBConnection");
            _connectionString = connection.GetADOConnectionString();
        }

        void LSExtensionControlLib.IExtensionControl.SetSite(object site) {
            if (site != null) {
                this.site = site as LSExtensionControlLib.IExtensionControlSite;
            }
        }

        void LSExtensionControlLib.IExtensionControl.SetupData() {
            string infoText = string.Empty;
            bool throwAway;

            site.GetStringValue("u_info_text", out infoText, out throwAway);

            txtInfoText.Text = infoText;
        }

        #endregion
        

        public int GetVersion() {
            return VERSION;
        }
    }
}
