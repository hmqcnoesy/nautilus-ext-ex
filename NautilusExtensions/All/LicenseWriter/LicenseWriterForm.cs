using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace NautilusExtensions.All {
    public partial class LicenseWriterForm : Form {

        private int _smallWidth = 300;
        private int _smallHeight = 120;
        private int _largeWidth = 640;
        private int _largeHeight = 480;
        private string _contents = "<?xml version =\"1.0\"?>\r\n<configuration>\r\n  <LicensingServer port=\"{0}\">{1}</LicensingServer>\r\n</configuration>";

        public LicenseWriterForm(string server, string port) {
            InitializeComponent();
            txtServer.Text = server;
            txtPort.Text = port;
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.Width = _smallWidth;
            this.Height = _smallHeight;
            gboxAdvanced.Visible = false;
            txtFileName.Text = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath), "Thermo.config");
            txtContents.Text = string.Format(_contents, txtPort.Text, txtServer.Text);
        }

        private void btnAdvanced_Click(object sender, EventArgs e) {
            if (gboxAdvanced.Visible) {
                this.Width = _smallWidth;
                this.Height = _smallHeight;
                btnAdvanced.Text = "Advanced >>";
                gboxAdvanced.Visible = false;
            } else {
                this.Width = _largeWidth;
                this.Height = _largeHeight;
                btnAdvanced.Text = "Advanced <<";
                gboxAdvanced.Visible = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e) {
            // values provided for server and port
            if (string.IsNullOrEmpty(txtServer.Text) || string.IsNullOrEmpty(txtPort.Text)) {
                MessageBox.Show("Please provide a value for Server and Port.");
                return;
            }

            // make sure path is valid
            FileInfo fi = new FileInfo(txtFileName.Text);
            if (!Directory.Exists(fi.DirectoryName)) {
                MessageBox.Show(string.Format("Directory '{0}' does not exist.  Please provide a valid file name and directory.", fi.DirectoryName));
                return;
            }

            // write the file
            try {
                using (StreamWriter sw = new StreamWriter(fi.FullName)) {
                    sw.Write(txtContents.Text);
                    sw.Flush();
                    sw.Close();
                }

                this.Close();
            } catch (Exception ex) {
                MessageBox.Show("The file could not be written:\r\n" + ex.Message);
            }
        }

        private void txt_TextChanged(object sender, EventArgs e) {
            txtContents.Text = string.Format(_contents, txtPort.Text, txtServer.Text);
        }

        private void btnBrowse_Click(object sender, EventArgs e) {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Config files (*.config) | *.config";
            sfd.OverwritePrompt = false;

            // try the currently input path
            FileInfo fi = new FileInfo(txtFileName.Text);
            sfd.FileName = fi.Name;
            if (Directory.Exists(fi.DirectoryName)) {
                sfd.InitialDirectory = fi.DirectoryName;
            }

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                txtFileName.Text = sfd.FileName;
            }
        }
    }
}
