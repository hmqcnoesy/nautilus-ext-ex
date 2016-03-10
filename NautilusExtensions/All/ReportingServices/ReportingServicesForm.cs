using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace NautilusExtensions.All {
    public partial class ReportingServicesForm : Form {

        private string _entityIdList;

        public ReportingServicesForm(DataTable dt, string entityList) {
            InitializeComponent();
            _entityIdList = entityList;

            ListViewItem lvi;

            foreach (DataRow dr in dt.Rows) {
                lvi = new ListViewItem(dr["name"].ToString());
                lvi.SubItems.Add(dr["description"].ToString());
                lvi.SubItems.Add(dr["url"].ToString());

                try {
                    imageList1.Images.Add(dr["icon"].ToString(), (new Bitmap(dr["icon"].ToString())));
                    lvi.ImageKey = dr["icon"].ToString();
                } catch {
                    lvi.ImageIndex = 0;
                }

                lvReportingServices.Items.Add(lvi);
            }

            // try to populate the list of installed printers
            foreach (string s in PrinterSettings.InstalledPrinters) {
                btnPrint.DropDownItems.Add(s, null, PrinterButton_Click);
            }
        }


        void PrinterButton_Click(object sender, EventArgs e) {

            if (lvReportingServices.SelectedItems.Count == 0) {
                MessageBox.Show("No report has been selected.");
                return;
            }

            Cursor c = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try {
                PrintDocument prtdoc = new PrintDocument();
                string printerName = ((ToolStripItem)sender).Text;
                var url = new Uri(lvReportingServices.SelectedItems[0].SubItems[2].Text);
                string reportServiceUrl = url.Scheme + "://" + url.Host + (url.IsDefaultPort ? string.Empty : url.Port.ToString()) + "/ReportServer/ReportService.asmx";
                string reportPath = System.Web.HttpUtility.UrlDecode(lvReportingServices.SelectedItems[0].SubItems[2].Text);

                MessageBox.Show(reportServiceUrl);
                MessageBox.Show(reportPath);
                //reportPath = reportPath.Substring((reportPath.IndexOf('?') + 1), (reportPath.LastIndexOf("&rs")) - (reportPath.IndexOf('?') + 1));
                ReportingServicesPrinting rsp = new ReportingServicesPrinting("http://ut40svma/ReportServer/ReportService.asmx", reportPath, _entityIdList);
                rsp.PrintReport(printerName);
            } catch (System.Web.Services.Protocols.SoapException se) {
                MessageBox.Show("Error retrieving report.  This report may require parameters -- view the report before printing.\r\n" + se.Message);
            } finally {
                Cursor.Current = c;
            }
        }


        private void btnQuickPrint_Click(object sender, EventArgs e) {

            if (lvReportingServices.SelectedItems.Count == 0) {
                MessageBox.Show("No report has been selected.");
                return;
            }

            Cursor c = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try {
                PrintDocument prtdoc = new PrintDocument();
                string defaultPrinterName = prtdoc.PrinterSettings.PrinterName;
                string ReportPath = System.Web.HttpUtility.UrlDecode(lvReportingServices.SelectedItems[0].SubItems[2].Text);
                ReportPath = ReportPath.Substring((ReportPath.IndexOf('?') + 1), (ReportPath.LastIndexOf("&rs")) - (ReportPath.IndexOf('?') + 1));
                MessageBox.Show("Sending to " + ReportPath);
                ReportingServicesPrinting rsp = new ReportingServicesPrinting("http://ut40svma/ReportServer/ReportService.asmx", ReportPath, _entityIdList);
                rsp.PrintReport(defaultPrinterName);
            } catch (System.Web.Services.Protocols.SoapException se) {
                MessageBox.Show("Error retrieving report.  This report may require parameters -- view the report before printing.\r\n" + se.Message);
            } finally {
                Cursor.Current = c;
            }
        }


        private void btnPreview_Click(object sender, EventArgs e) {

            if (lvReportingServices.SelectedItems.Count == 0) {
                MessageBox.Show("No report has been selected.");
                return;
            }

            using (ReportingServicesPreviewForm rspf = new ReportingServicesPreviewForm(lvReportingServices.SelectedItems[0].SubItems[2].Text, _entityIdList)) {
                rspf.ShowDialog();
            }
        }


        private void btnViewInBrowser_Click(object sender, EventArgs e) {

            if (lvReportingServices.SelectedItems.Count == 0) {
                MessageBox.Show("No report has been selected.");
                return;
            }
            
            System.Diagnostics.Process.Start(lvReportingServices.SelectedItems[0].SubItems[2].Text + _entityIdList);
        }
    }
}