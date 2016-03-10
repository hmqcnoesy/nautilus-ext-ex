using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NautilusExtensions.All
{
    public partial class EpicWebLauncherForm : Form
    {
        private Uri _uri;

        public EpicWebLauncherForm(string sdgIdList)
        {
            InitializeComponent();

            var uri = new UriBuilder(Properties.Settings.Default.EpicLauncherUri);
            uri.Query = sdgIdList;
            _uri = uri.Uri;
        }

        private void EpicWebLauncherForm_Load(object sender, EventArgs e)
        {
            webBrowser1.Url = _uri;
        }
    }
}
