using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;// For Marshal.Copy
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using NautilusExtensions.ReportingWebService;

namespace NautilusExtensions.All {
    class ReportingServicesPrinting {
        ReportingService _reportingService;
        private byte[][] m_renderedReport;
        private Graphics.EnumerateMetafileProc m_delegate = null;
        private MemoryStream m_currentPageStream;
        private Metafile m_metafile = null;
        int m_numberOfPages;
        private int m_currentPrintingPage;
        private int m_lastPrintingPage;
        private string _path;
        ParameterValue[] _reportParams;
        bool isLandscape = false;

        public ReportingServicesPrinting(string reportingServiceUrl, string path, string parameters)
        {
            _path = path;

            string[] splitters = new string[] { "&q=", "?q=" };
            string[] splitParams = parameters.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

            if (splitParams.Length > 0) {
                _reportParams = new ParameterValue[splitParams.Length];

                for (int i = 0; i < splitParams.Length; i++) {
                    _reportParams[i] = new ParameterValue();
                    _reportParams[i].Name = "q";
                    _reportParams[i].Value = splitParams[i];
                }
            }

            _reportingService = new ReportingService();
            _reportingService.Url = reportingServiceUrl;
            _reportingService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            isLandscape = isReportLandscape(path);
        }

        public byte[][] RenderReport(string reportPath)
        {
            // Private variables for rendering
            string deviceInfo = null;
            string format = "IMAGE";
            Byte[] firstPage = null;
            string encoding;
            string mimeType;
            Warning[] warnings = null;
            ParameterValue[] reportHistoryParameters = null;
            string[] streamIDs = null;
            Byte[][] pages = null;

            // Build device info based on the start page
            deviceInfo =
               String.Format(@"<DeviceInfo><OutputFormat>{0}</OutputFormat></DeviceInfo>", "emf");

            //Exectute the report and get page count.
            // Renders the first page of the report and returns streamIDs for 
            // subsequent pages
            firstPage = _reportingService.Render(
                reportPath,
                format,
                null,
                deviceInfo,
                _reportParams,
                null,
                null,
                out encoding,
                out mimeType,
                out reportHistoryParameters,
                out warnings,
                out streamIDs);
            // The total number of pages of the report is 1 + the streamIDs         
            m_numberOfPages = streamIDs.Length + 1;
            pages = new Byte[m_numberOfPages][];

            // The first page was already rendered
            pages[0] = firstPage;

            for (int pageIndex = 1; pageIndex < m_numberOfPages; pageIndex++)
            {
                // Build device info based on start page
                deviceInfo =
                    String.Format(@"<DeviceInfo><OutputFormat>{0}</OutputFormat><StartPage>{1}</StartPage></DeviceInfo>",
                        "emf", pageIndex + 1);
                pages[pageIndex] = _reportingService.Render(
                    reportPath,
                    format,
                    null,
                    deviceInfo,
                    _reportParams,
                    null,
                    null,
                    out encoding,
                    out mimeType,
                    out reportHistoryParameters,
                    out warnings,
                    out streamIDs);
            }
            return pages;
        }

        public bool PrintReport(string printerName)
        {
            this.RenderedReport = this.RenderReport(_path);
                
            // Wait for the report to completely render.
            if (m_numberOfPages < 1)
                return false;
            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.MaximumPage = m_numberOfPages;
            printerSettings.MinimumPage = 1;
            printerSettings.PrintRange = PrintRange.SomePages;
            printerSettings.FromPage = 1;
            printerSettings.ToPage = m_numberOfPages;
            printerSettings.PrinterName = printerName;
            printerSettings.DefaultPageSettings.Landscape = isLandscape;
            PrintDocument pd = new PrintDocument();
            m_currentPrintingPage = 1;
            m_lastPrintingPage = m_numberOfPages;
            pd.PrinterSettings = printerSettings;
            // Print report
            Console.WriteLine("Printing report...");
            pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
            pd.Print();

            return true;
        }
        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            ev.HasMorePages = false;
            if (m_currentPrintingPage <= m_lastPrintingPage && MoveToPage(m_currentPrintingPage))
            {
                // Draw the page
                ReportDrawPage(ev.Graphics);
                // If the next page is less than or equal to the last page, 
                // print another page.
                if (++m_currentPrintingPage <= m_lastPrintingPage)
                    ev.HasMorePages = true;
            }
        }

        // Method to draw the current emf memory stream 
        private void ReportDrawPage(Graphics g)
        {
            if (null == m_currentPageStream || 0 == m_currentPageStream.Length || null == m_metafile)
                return;
            lock (this)
            {
                Point[] points = new Point[3];
                // Set the metafile delegate.
                m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
                // Draw in the rectangle


                if (isLandscape)
                {
                    //Landscape
                    points[0] = new Point(0, 0);
                    points[1] = new Point(1118, 0);
                    points[2] = new Point(0, 859);
                    isLandscape = true;
                    
                }
                else
                {
                    //Portrait
                    points[0] = new Point(0, 0);
                    points[1] = new Point(859, 0);
                    points[2] = new Point(0, 1118);
                }

                g.EnumerateMetafile(m_metafile, points, m_delegate);
                //g.EnumerateMetafile(m_metafile, destPoint, m_delegate);

                // Clean up
                m_delegate = null;
            }
        }
        private bool MoveToPage(Int32 page)
        {
            // Check to make sure that the current page exists in
            // the array list
            if (null == this.RenderedReport[m_currentPrintingPage - 1])
                return false;
            // Set current page stream equal to the rendered page
            m_currentPageStream = new MemoryStream(this.RenderedReport[m_currentPrintingPage - 1]);
            // Set its postion to start.
            m_currentPageStream.Position = 0;
            // Initialize the metafile
            if (null != m_metafile)
            {
                m_metafile.Dispose();
                m_metafile = null;
            }
            // Load the metafile image for this page
            m_metafile = new Metafile((Stream)m_currentPageStream);
            return true;
        }
        private bool MetafileCallback(
           EmfPlusRecordType recordType,
           int flags,
           int dataSize,
           IntPtr data,
           PlayRecordCallback callbackData)
        {
            byte[] dataArray = null;
            // Dance around unmanaged code.
            if (data != IntPtr.Zero)
            {
                // Copy the unmanaged record to a managed byte buffer 
                // that can be used by PlayRecord.
                dataArray = new byte[dataSize];
                Marshal.Copy(data, dataArray, 0, dataSize);
            }
            // play the record.      
            m_metafile.PlayRecord(recordType, flags, dataSize, dataArray);

            return true;
        }
        public byte[][] RenderedReport
        {
            get
            {
                return m_renderedReport;
            }
            set
            {
                m_renderedReport = value;
            }
        }

        public bool isReportLandscape(string reportName)
        {
            string rdl;
            double doubleH, doubleW;
            byte[] reportDefinition = null;

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            reportDefinition = _reportingService.GetReportDefinition(reportName);
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            rdl = enc.GetString(reportDefinition);
            doc.LoadXml(rdl);
            XmlNode height = doc.GetElementsByTagName("InteractiveHeight")[0];
            XmlNode width = doc.GetElementsByTagName("InteractiveWidth")[0];
            doubleH = double.Parse(height.InnerText.Substring(0,height.InnerText.Length-2));
            doubleW = double.Parse(width.InnerText.Substring(0,width.InnerText.Length-2));

            return (doubleW > doubleH);
        } 
    }
}

