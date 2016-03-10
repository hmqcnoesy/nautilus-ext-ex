using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace NautilusExtensions.All {
    public partial class ReportingServicesPreviewForm : Form {
        public ReportingServicesPreviewForm(string url, string parameters) {
            Cursor c = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            InitializeComponent();

            try {
                // the corrected Url has all the params and path to report removed.  Just the "http://example.com/ReportServer" portion
                int serverUrlEndPos = url.IndexOf('/', url.IndexOf('/', url.IndexOf('/', url.IndexOf('/') + 1) + 1) + 1);
                
                reportViewer.ServerReport.ReportServerUrl = new Uri(url.Substring(0, serverUrlEndPos));

                // the report path is the portion that immediately follows the ? up to the &rs
                int pathUrlStartPos = url.IndexOf('?') + 1;
                int pathUrlEndPos = url.IndexOf("&rs");
                
                reportViewer.ServerReport.ReportPath = url.Substring(pathUrlStartPos, pathUrlEndPos - pathUrlStartPos);
                
                // parameters can't be passed in the url
                string[] splitters = new string[] { "&q=", "?q=" };
                string[] splitParams = parameters.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                
                if (splitParams.Length > 0) {
                    ReportParameter p = new ReportParameter("q", splitParams);
                    List<ReportParameter> list = new List<ReportParameter>();
                    list.Add(p);
                    reportViewer.ServerReport.SetParameters(list);
                }

            } catch (Exception ex) {
                ErrorHandler.LogError("Bad report url or parameters:\r\n" + url + "\r\n" + ex.Message);
                return;
            } finally {
                Cursor.Current = c;
            }
        }

        private void ReportingServicesPreviewForm_Load(object sender, EventArgs e)
        {
            Cursor c = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try {
                this.reportViewer.RefreshReport();
            } finally {
                Cursor.Current = c;
            }
        }
    }
}
