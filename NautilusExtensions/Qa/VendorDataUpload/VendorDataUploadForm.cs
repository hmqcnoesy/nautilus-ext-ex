using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OracleClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using LSSERVICEPROVIDERLib;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {
    public partial class VendorDataUploadForm : Form {

        private string _superSecretCode = "dev";
        private string _nautilusParsingPath;
        private string _operatorName;
        private OracleConnection _connection;
        private NautilusProcessXML _processXml;
        private List<Sample> _samples = new List<Sample>();
        private List<Color> highlightColors = new List<Color> { Color.YellowGreen, Color.Yellow, Color.Violet, Color.Turquoise, Color.Tomato, Color.Thistle, Color.Tan, Color.SlateBlue, Color.SkyBlue, Color.SandyBrown };

        public VendorDataUploadForm(OracleConnection connection, NautilusProcessXML processXml, string operatorName) {
            InitializeComponent();
            _operatorName = operatorName;
            _connection = connection;
            _processXml = processXml;

            //get the program codes into the drop down list
            cmbProgramCode.Items.Add(string.Empty);
            string sqlString = "select phrase_name from lims_sys.phrase_entry where phrase_id = 124 order by phrase_name";
            OracleCommand command = new OracleCommand(sqlString, _connection);

            OracleDataReader reader;

            try {
                reader = command.ExecuteReader();
                while (reader.Read()) {
                    cmbProgramCode.Items.Add(reader["phrase_name"].ToString());
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "VendorDataUploadForm", "Error getting program code list:\r\n" + ex.Message);
            }
        }
        

        /// <summary>
        /// Loads file contents into TreeView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadFile_Click(object sender, EventArgs e) {
            lblProgressStatus.Text = string.Empty;
            pbLoginProgress.Value = 0;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Vendor Data Files (*.krt, *.edf)|*.krt;*.edf";
            ofd.Multiselect = true;
            ofd.Title = "Select Vendor Data File(s)";

            //clicked OK?
            if (ofd.ShowDialog() != DialogResult.OK) return;
            string[] files = ofd.FileNames;
            string[] fileNames = ofd.SafeFileNames;
            ofd.Dispose();

            LoadFiles(files, fileNames);
        }


        private void LoadFiles(string[] files, string[] fileNames) {
            //clear out what's in memory.
            if (_samples.Count > 0) {
                DialogResult dr = MessageBox.Show("Clear existing dynamic data?", "Add Files", MessageBoxButtons.OKCancel);
                if (dr == System.Windows.Forms.DialogResult.Cancel) {
                    return;
                } else {
                    _samples.Clear();
                    tvSample.Nodes.Clear();
                }
            }

            string sampleNames;

            //check each file to ensure it has not already been logged in
            int fileIndex = 0;
            foreach (string file in files) {
                //check that the file has not been uploaded before
                //allow developer to easily test with duplicate logins - just type the super secret code into the combobox
                sampleNames = GetSampleNamesFromPreviousUpload(fileNames[fileIndex]);
                if (!cmbProgramCode.Text.Equals(_superSecretCode) && !string.IsNullOrEmpty(sampleNames)) {
                    MessageBox.Show("This file has been previously uploaded.\r\n"
                        + "If changes to ADCAR data are required, make the changes to sample(s):\r\n" + sampleNames);
                    return;
                }

                fileIndex++;
            }

            //translate each file into a sample object/dynamic data hierarchy in memory, displayed in the treeview
            fileIndex = 0;
            foreach (string file in files) {

                //load the contents of the file into the form.  File must be .krt or .edf
                string ext = file.Substring(file.Length - 3).ToLower();
                if (ext.Equals("krt") || ext.Equals("edf")) 
                {
                    try 
                    {  //the Vendor Data File constructor can throw all manner of exceptions when parsing the file fails.
                        VendorDataFile vdf = new VendorDataFile(file);
                        _samples.AddRange(vdf.Samples);

                        if (vdf.HasErrors) 
                            MessageBox.Show(string.Format("There are non-critical errors in the file {0} that are ignored:\r\n{1}", fileNames[fileIndex], vdf.Errors));

                        //if there is more than one sample (part number / serial number combination) in the file, make sure the user understands it's probably a mistake
                        if (vdf.Samples.Count > 1) 
                        {
                            DialogResult dr = MessageBox.Show(string.Format("There is more than part/serial combination specified in file {0}.\r\n"
                                + "Do NOT continue this login unless you are certain this is correct.\r\n"
                                + "It is highly recommended that you click Cancel and contact the Nautilus admin.", fileNames[fileIndex]),
                                "Multiple Samples", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);

                            if (dr == DialogResult.Cancel) 
                            {
                                _samples.Clear();
                                tvSample.Nodes.Clear();
                            }
                        }
                    } 
                    catch (Exception ex) 
                    {
                        ErrorHandler.LogError(_operatorName, "VendorDataUploadForm", string.Format("Error parsing file {0}.  Please notify the Nautilus admin.\r\n{1}", fileNames[fileIndex], ex.Message));
                        return;
                    }
                } 
                else 
                {
                    ErrorHandler.LogError(_operatorName, "VendorDataUploadForm", string.Format("Unable to determine type of file {0}.  Kirkhill files must end in .krt, Cytec files in .edf", fileNames[fileIndex]));
                    return;
                }
            }

            //check that each sample has not been logged in previously
            //allow developer to easily test with duplicate logins - just type the super secret code into the combobox
            foreach (Sample s in _samples)
            {
                sampleNames = GetSampleNamesFromPreviousUpload(s.PartNumber, s.SerialNumber);
                if (!cmbProgramCode.Text.Equals(_superSecretCode) && !string.IsNullOrEmpty(sampleNames)) 
                {
                    MessageBox.Show("This part/serial/ip type already exists in Nauitlus.\r\n"
                        + "If changes to ADCAR data are required, make the changes to sample(s):\r\n" + sampleNames);
                    return;
                }
            }

            PopulateTreeView(_samples, out List<string> duplicateTests);
            if (duplicateTests.Any())
            {
                MessageBox.Show("Duplicate test entries were detected (see list below).  To prevent duplicate tests, find the highlighted test nodes below, right-click and delete unwanted nodes.\r\n\r\n" + string.Join("\r\n", duplicateTests));
            }
            Application.DoEvents();  //updates the interface with populated tree view before possible message box...

            btnLogin.Text = "Login";
            btnLogin.Enabled = true;

            cmbProgramCode.SelectedIndex = 0;
        }


        /// <summary>
        /// Logs file items into Nautilus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e) 
        {
            if (_samples.Count == 0) return;

            if (btnLogin.Text.Equals("Login")) 
            {

                //program code must be selected
                string programCode = cmbProgramCode.Text;
                if (string.IsNullOrEmpty(programCode)) 
                {
                    MessageBox.Show("Select a program code before attempting login.");
                    return;
                }

                backgroundWorker1.RunWorkerAsync("Login");
            } 
            else if (btnLogin.Text.Equals("Authorise")) 
            {
                backgroundWorker1.RunWorkerAsync("Authorise");
            }
        }


        /// <summary>
        /// Populates the tree view according to the _samples hierarchy.
        /// </summary>
        private void PopulateTreeView(List<Sample> samples, out List<string> duplicateTests) {

            tvSample.Nodes.Clear();
            
            TreeNode tnSample, tnAliquot, tnTest, tnResult;
            var allTestNodes = new List<TreeNode>();
            var testUniqueStrings = new List<string>();
            duplicateTests = new List<string>();

            foreach (Sample s in samples) {
                tnSample = new TreeNode() {
                    Name = s.Name,
                    Text = s.TreeNodeName,
                    ImageKey = "s" + s.Status.ToString().ToLower(),
                    SelectedImageKey = "s" + s.Status.ToString().ToLower()
                };
                tnSample.Expand();

                foreach (Aliquot a in s.Aliquots) {
                    tnAliquot = new TreeNode() {
                        Name = a.Name,
                        Text = a.TreeNodeName,
                        ImageKey = "a" + a.Status.ToString().ToLower(),
                        SelectedImageKey = "a" + a.Status.ToString().ToLower()
                    };
                    if (samples.Count == 1) tnAliquot.Expand();

                    foreach (Test t in a.Tests) {
                        tnTest = new TreeNode() {
                            Name = t.Name,
                            Text = t.TreeNodeName,
                            ImageKey = "t" + t.Status.ToString().ToLower(),
                            SelectedImageKey = "t" + t.Status.ToString().ToLower(),
                            Tag = t.ExternalReference
                        };

                        if (testUniqueStrings.Contains(t.TreeNodeName))
                            duplicateTests.Add(t.TreeNodeName);
                        else
                            testUniqueStrings.Add(t.TreeNodeName);

                        foreach (Result r in t.Results) {
                            tnResult = new TreeNode() {
                                Name = r.Name,
                                Text = r.TreeNodeName,
                                ImageKey = "r" + r.Status.ToString().ToLower(),
                                SelectedImageKey = "r" + r.Status.ToString().ToLower(),
                            };
                            tnTest.Nodes.Add(tnResult);
                        }
                        tnAliquot.Nodes.Add(tnTest);
                        allTestNodes.Add(tnTest);
                    }
                    tnSample.Nodes.Add(tnAliquot);
                }
                tvSample.Nodes.Add(tnSample);
            }

            foreach(var tn in allTestNodes)
            {
                var duplicateEntry = duplicateTests.Select((testInfo, idx) => new { testInfo, idx }).Where(x => x.testInfo == tn.Text).FirstOrDefault();
                if (duplicateEntry == null) continue;

                tn.BackColor = highlightColors[duplicateEntry.idx % highlightColors.Count];
            }
        }

       
        /// <summary>
        /// Returns Nautilus sample name of sample name which has the specified file name in the u_receiver_number field.
        /// Returns empty string if no sample found.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetSampleNamesFromPreviousUpload(string fileName) {
            string returnValue = string.Empty;

            string sqlString = "select s.name, su.u_receiver_number "
                + "from lims_sys.sample s, lims_sys.sample_user su "
                + "where s.sample_id = su.sample_id "
                + "and trim(upper(su.u_receiver_number)) = trim(upper(:in_receiver_number)) "
                + "and s.status != 'X' "
                + "and sample_template_id = (select sample_template_id from lims_sys.sample_template where name = 'Vendor - Electronic')";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_receiver_number", fileName));
            OracleDataReader reader;
            try {
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    returnValue += reader[0].ToString() + "\r\n";
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "VendorDataUploadForm", "Error comparing file name to previously loaded file names:\r\n" + ex.Message);
                return "?";  //return a non-empty value so that the user is prevented from continuing.
            }

            return returnValue;
        }


        private string GetFileParsingPath() {
            string sqlString = "select ic.input_file_directory "
                + "from lims_sys.instrument_control ic, lims_sys.instrument i "
                + "where i.instrument_control_id = ic.instrument_control_id "
                + "and i.name = 'Vendor Data'";

            OracleCommand command = new OracleCommand(sqlString, _connection);

            try {
                return command.ExecuteScalar().ToString();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "VendorDataUploadForm", "Error attempting to retrieve parsing directory for the Vendor Data instrument in Nautilus.\r\n"
                    + "Please choose the parsing directory directory manually.\r\n" + ex.Message);

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Select the path for the Nautilus Vendor Data instrument.";
                fbd.ShowDialog();
                return fbd.SelectedPath;
            }
        }

       
        /// <summary>
        /// Returns Nautilus sample name of sample name which has the same part/serial/ip type.
        /// Returns empty string if no sample found.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetSampleNamesFromPreviousUpload(string partNumber, string serialNumber) {
            string returnValue = string.Empty;

            string sqlString = "select s.name, su.u_receiver_number "
                + "from lims_sys.sample s, lims_sys.sample_user su "
                + "where s.sample_id = su.sample_id "
                + "and trim(upper(su.u_part_number)) = trim(upper(:in_part_number)) and trim(upper(su.u_serial_number)) = trim(upper(:in_serial_number)) and su.u_ip_type = 'V' "
                + "and s.status != 'X' "
                + "and sample_template_id = (select sample_template_id from lims_sys.sample_template where name = 'Vendor - Electronic')";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_part_number", partNumber));
            command.Parameters.Add(new OracleParameter(":in_serial_number", serialNumber));
            OracleDataReader reader;

            try {
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    returnValue += reader[0].ToString() + "\r\n";
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "VendorDataUploadForm", "Error comparing part/serial to previously loaded part/serial values:\r\n" + ex.Message);
                return "?";  //return a non-empty value so that the user is prevented from continuing.
            }

            return returnValue;

        }


        private void UpdateHierarchyFromXml(XDocument doc) {
            IEnumerable<XElement> samples, aliquots, tests, results;
            Sample sampleTemp;
            samples = doc.XPathSelectElements("//SAMPLE");
            foreach (System.Xml.Linq.XElement sample in samples) {
                sampleTemp = (Sample)FindDataObjectByDescription(sample.Element("EXTERNAL_REFERENCE").Value.ToString());
                if (sampleTemp != null) {
                    sampleTemp.Name = sample.Element("NAME").Value.ToString();
                    sampleTemp.Status = sample.Element("STATUS").Value.ToCharArray()[0];
                }
            }

            Aliquot aliquotTemp;
            aliquots = doc.XPathSelectElements("//ALIQUOT");
            foreach (System.Xml.Linq.XElement aliquot in aliquots) {
                aliquotTemp = (Aliquot)FindDataObjectByDescription(aliquot.Element("EXTERNAL_REFERENCE").Value.ToString());
                if (aliquotTemp != null) {
                    aliquotTemp.Name = aliquot.Element("NAME").Value.ToString();
                    aliquotTemp.Status = aliquot.Element("STATUS").Value.ToCharArray()[0];
                }
            }

            Test testTemp;
            tests = doc.XPathSelectElements("//TEST");
            foreach (System.Xml.Linq.XElement test in tests) {
                testTemp = (Test)FindDataObjectByDescription(test.Element("U_EXTERNAL_REFERENCE").Value.ToString());
                if (testTemp != null) {
                    testTemp.Name = test.Element("NAME").Value.ToString();
                    testTemp.Status = test.Element("STATUS").Value.ToCharArray()[0];

                    //get all the results this way - can't use guids because ad-hoc-result doesn't allow specifying values for fields like extref or description
                    results = test.Elements("RESULT");
                    foreach (System.Xml.Linq.XElement result in results) {
                        foreach (Result r in testTemp.Results) {
                            if (r.Name.Equals(result.Element("NAME").Value.ToString())) {
                                r.Status = result.Element("STATUS").Value.ToCharArray()[0];
                                r.ResultValue = result.Element("FORMATTED_RESULT").Value.ToString();
                            }
                        }
                    }
                }
            }
        }


        private void UpdateHierarchyStatusFromXml(XDocument doc) {
            IEnumerable<XElement> samples = doc.XPathSelectElements("//SAMPLE");

            Sample tempSample;
            foreach (System.Xml.Linq.XElement sample in samples) {
                if (sample.Element("STATUS").Value.ToString().Equals("A")) {
                    tempSample = (Sample)FindDataObjectByDescription(sample.Element("EXTERNAL_REFERENCE").Value.ToString());
                    if (tempSample != null) {
                        tempSample.Status = 'A';
                        foreach (Aliquot a in tempSample.Aliquots) {
                            a.Status = 'A';
                            foreach (Test t in a.Tests) {
                                t.Status = 'A';
                                foreach (Result r in t.Results) {
                                    r.Status = 'A';
                                }
                            }
                        }
                    }
                } else {
                    throw new ProcessXmlException(string.Format("There was a problem authorising sample {0}.\r\n"
                        + "Notify the Nautilus admin.  You may need to manually authorise the sample.",
                        sample.Element("NAME").Value.ToString()));
                }
            }
        }


        private object FindDataObjectByDescription(string guid) {
            foreach (Sample s in _samples) {

                if (s.ExternalReference.Equals(guid)) return s;

                foreach (Aliquot a in s.Aliquots) {

                    if (a.ExternalReference.Equals(guid)) return a;

                    foreach (Test t in a.Tests) {

                        if (t.ExternalReference.Equals(guid)) return t;

                    }
                }
            }

            return null;
        }


        private object FindParentDataObjectByDescription(string guid) {
            foreach (Sample s in _samples)
            {
                foreach (Aliquot a in s.Aliquots)
                {
                    if (a.ExternalReference.Equals(guid)) return s;
                    foreach (Test t in a.Tests)
                    {
                        if (t.ExternalReference.Equals(guid)) return a;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// The background processor is flaky - when a file is too large, it fails.  Break the results up into files of no more than 1000 results each.
        /// </summary>
        /// <param name="doc"></param>
        private void CreateResultsFilesAdHoc(XDocument doc) {
            StreamWriter sw = null;
            string file;
            int resultCount = 0, fileCount = 0;
            Test selectedTest = null;
            IEnumerable<XElement> aliquots = doc.XPathSelectElements("//ALIQUOT");
            IEnumerable<XElement> tests;

            foreach (XElement aliquot in aliquots) {

                tests = aliquot.Elements("TEST");
                foreach (XElement test in tests) {
                    selectedTest = (Test)FindDataObjectByDescription(test.Element("U_EXTERNAL_REFERENCE").Value.ToString());

                    if (selectedTest != null) {

                        if (sw == null) {
                            file = Path.Combine(_nautilusParsingPath, string.Format("VendorAdHoc {0}-{1}.txt", DateTime.Now.ToString("yyMMddHHmmss"), (fileCount++).ToString()));
                            sw = new StreamWriter(file);
                        }

                        sw.WriteLine("Aliquot ID:" + aliquot.Element("ALIQUOT_ID").Value.ToString());
                        sw.WriteLine("Test Name:" + selectedTest.MatrixId);

                        foreach (Result r in selectedTest.Results) {
                            sw.WriteLine(string.Format("{0},{1}", r.Name, r.ResultValue));
                            resultCount++;
                        }
                    }
                    sw.WriteLine();  //write a blank line after each test.  Makes it very easy to parse with Nautilus scripts.

                    if (resultCount > 999) {
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();
                        sw = null;
                    }
                }

                sw.Flush();
                sw.Close();
            }
        }


        private void CreateResultsFileRowId(XDocument doc) {

            Test selectedTest = null;

            //the nautilus parsing path has only the path, need to specify a filename here
            string file = Path.Combine(_nautilusParsingPath, "VendorRowIds " + DateTime.Now.ToString("yyMMddHHmmss") + ".txt");

            using (StreamWriter sw = new StreamWriter(file)) {
                IEnumerable<XElement> aliquots = doc.XPathSelectElements("//ALIQUOT");
                IEnumerable<XElement> tests, results;
                foreach (XElement aliquot in aliquots) {
                    tests = aliquot.Elements("TEST");

                    foreach (XElement test in tests) {
                        selectedTest = (Test)FindDataObjectByDescription(test.Element("U_EXTERNAL_REFERENCE").Value.ToString());

                        if (selectedTest != null) {
                            sw.WriteLine("Aliquot ID:" + aliquot.Element("ALIQUOT_ID").Value.ToString());
                            sw.WriteLine("Test Name:" + selectedTest.MatrixId);
                            results = test.Elements("RESULT");

                            foreach (XElement result in results) {
                                sw.WriteLine("Row ID," + selectedTest.RowId);
                            }
                        }
                        sw.WriteLine();  //write a blank line after each test.  Makes it very easy to parse with Nautilus scripts.
                    }
                }

                sw.Flush();
                sw.Close();
            }
        }


        private bool AreAllItemsComplete() {
            foreach (Sample s in _samples) {
                if (!s.Status.Equals('C')) return false;
                foreach (Aliquot a in s.Aliquots) {
                    if (!a.Status.Equals('C')) return false;
                    foreach (Test t in a.Tests) {
                        if (!t.Status.Equals('C')) return false;
                        foreach (Result r in t.Results) {
                            if (!r.Status.Equals('C')) return false;

                        }
                    }
                }
            }
            return true;
        }


        private void Login() {

            btnLogin.Enabled = false;

            //process the XML, get errors/response
            //using the response, go through the tests, getting the test_id values
            backgroundWorker1.ReportProgress(5, "Requesting sample/aliquot/test login.");
            string loginXml = CreateLoginXml(cmbProgramCode.Text);
            string errors = string.Empty;
            XDocument doc = ProcessXml(loginXml, out errors);


            //Get the IDs and external references of the tests just logged in.  Will use to complete/add results.
            backgroundWorker1.ReportProgress(25, "Getting IDs of tests logged in.");
            IEnumerable<XElement> aliquots = doc.XPathSelectElements("//ALIQUOT");
            List<string> aliquotIds = new List<string>();
            foreach (XElement aliquot in aliquots) {
                aliquotIds.Add(aliquot.Element("ALIQUOT_ID").Value.ToString());
            }
            doc = ProcessXml(CreateTestFindRequestXml(aliquotIds), out errors);


            //This is a little messed up.  There is a bug in the Nautilus XML processor that sometimes gets results out of order
            //when submitting xml with <result-ad-hoc>.  Workaround (each <result-ad-hoc> has own <result-request> and <load> nodes)
            //causes XML processor to take far too long.
            //complete the 'Row ID' results and add the ad-hoc results
            backgroundWorker1.ReportProgress(50, "Completing and adding results.");
            doc = ProcessXml(CreateResultEntryXml(doc), out errors);


            //Repopulate the tree view with the logged-in data
            backgroundWorker1.ReportProgress(75, "Getting updated names/statuses.");
            doc = ProcessXml(CreateFindAllByGuidXml(), out errors);


            //dig through xml response, update dynamic data hierarchy accordingly
            backgroundWorker1.ReportProgress(95, "Updating hierarchy info.");
            UpdateHierarchyFromXml(doc);

            if (AreAllItemsComplete()) {
                btnLogin.Text = "Authorise";
                btnLogin.Enabled = true;
            } else {
                MessageBox.Show("Not all items were correctly logged in or completed.  Please notify the Nautilus admin, and make corrections to these items as necessary.");
            }
        }


        private void LoginWithFile() {

            btnLogin.Enabled = false;

            //process the XML, get errors/response
            //using the response, go through the tests, getting the test_id values
            backgroundWorker1.ReportProgress(5, "Requesting sample/aliquot/test login.");
            string loginXml = CreateLoginXml(cmbProgramCode.Text);
            string errors = string.Empty;
            XDocument doc = ProcessXml(loginXml, out errors);


            //Get the IDs and external references of the tests just logged in.  Will use to complete/add results.
            backgroundWorker1.ReportProgress(25, "Getting IDs of tests logged in.");
            IEnumerable<XElement> aliquots = doc.XPathSelectElements("//ALIQUOT");
            List<string> aliquotIds = new List<string>();
            foreach (XElement aliquot in aliquots) {
                aliquotIds.Add(aliquot.Element("ALIQUOT_ID").Value.ToString());
            }
            doc = ProcessXml(CreateTestFindRequestXml(aliquotIds), out errors);


            //The fact that two files are used for results is important, as is the order they are made.
            //The existing results in Nauitlus should NOT be completed first, so the sample will show as incomplete to the user.
            //Only after all ad-hoc results are finished and committed should we save a separate file to complete the existing 'Row ID' results.
            backgroundWorker1.ReportProgress(60, "Writing ad-hoc result file.");
            CreateResultsFilesAdHoc(doc);
            backgroundWorker1.ReportProgress(90, "Writing Row ID result file.");
            CreateResultsFileRowId(doc);
            

            //Repopulate the tree view with the logged-in data
            backgroundWorker1.ReportProgress(95, "Getting updated names/statuses.");
            doc = ProcessXml(CreateFindAllByGuidXml(), out errors);


            //dig through xml response, update dynamic data hierarchy accordingly
            backgroundWorker1.ReportProgress(95, "Updating hierarchy info.");
            UpdateHierarchyFromXml(doc);

            btnLogin.Enabled = false;
            MessageBox.Show("Manually authorise the data when complete in Nautilus.");
        }


        /// <summary>
        /// Authorises all samples in memory (and authorisation propagates down hierarchy)
        /// </summary>
        private void Authorise() {
            string errors = string.Empty;
            backgroundWorker1.ReportProgress(5, "Creating sample authorisation XML request.");
            string xmlSampleAuthoriseRequest = CreateSampleAuthoriseRequestXml();

            backgroundWorker1.ReportProgress(45, "Processing sample authorisation XML request.");
            XDocument doc = ProcessXml(xmlSampleAuthoriseRequest, out errors);

            backgroundWorker1.ReportProgress(55, "Creating sample find by ID XML request.");
            string xmlSampleFindRequest = CreateSampleFindRequestXml(doc);

            backgroundWorker1.ReportProgress(85, "Processing sample find by ID XML request.");
            doc = ProcessXml(xmlSampleFindRequest, out errors);

            backgroundWorker1.ReportProgress(99, "Updating hierarchy status display.");
            UpdateHierarchyStatusFromXml(doc);

            btnLogin.Enabled = false;
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            if (e.Argument.Equals("Login")) {
                Login();
            } else if (e.Argument.Equals("Login File")) {
                LoginWithFile();
            } else if (e.Argument.Equals("Authorise")) {
                Authorise();
            }
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            lblProgressStatus.Text = e.UserState.ToString();
            lblProgressStatus.Invalidate();
            lblProgressStatus.Update();    //using invalidate and update ensure this label gets repainted on the form
            pbLoginProgress.Value = e.ProgressPercentage;
        }


        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error == null) {
                pbLoginProgress.Value = 0;
                lblProgressStatus.Text = string.Empty;
                PopulateTreeView(_samples, out List<string> duplicateTests);
            } else {
                ErrorHandler.LogError(_operatorName, "VendorDataUploadForm", "There was an error processing the request.  Please notify the Nautilus admin.\r\n" + e.Error.Message);
            }
        }


        #region XmlProcessingMethods


        private string CreateLoginXml(string programCode) {
            StringBuilder sbXml = new StringBuilder("<?xml version=\"1.0\"?><lims-request version=\"1\"><login-request version=\"1\">");

            foreach (Sample s in _samples) {
                sbXml.Append("<SAMPLE><create-by-workflow><workflow-name>Vendor IP - Electronic</workflow-name></create-by-workflow>");
                sbXml.Append(string.Format("<U_PART_NUMBER>{0}</U_PART_NUMBER>", s.PartNumber));
                sbXml.Append(string.Format("<U_PROGRAM_CODE>{0}</U_PROGRAM_CODE>", programCode));
                sbXml.Append(string.Format("<U_SERIAL_NUMBER>{0}</U_SERIAL_NUMBER>", s.SerialNumber));
                sbXml.Append(string.Format("<EXTERNAL_REFERENCE>{0}</EXTERNAL_REFERENCE>", s.ExternalReference));
                sbXml.Append(string.Format("<DESCRIPTION>{0}</DESCRIPTION>", s.Description));
                sbXml.Append(string.Format("<U_RECEIVER_NUMBER>{0}</U_RECEIVER_NUMBER>", s.ReceiverNumber));

                foreach (Aliquot a in s.Aliquots) {
                    sbXml.Append("<ALIQUOT><create-by-workflow><workflow-name>Data Center</workflow-name></create-by-workflow>");
                    sbXml.Append(string.Format("<EXTERNAL_REFERENCE>{0}</EXTERNAL_REFERENCE>", a.ExternalReference));

                    foreach (Test t in a.Tests) {
                        sbXml.Append(string.Format("<TEST><create-by-workflow><workflow-name>{0}</workflow-name></create-by-workflow>", t.MatrixId));
                        sbXml.Append(string.Format("<U_MATRIX_ID>{0}</U_MATRIX_ID>", t.MatrixId));
                        sbXml.Append(string.Format("<U_IP_ITEM_NUMBER>{0}</U_IP_ITEM_NUMBER>", t.IpItemNumber));
                        sbXml.Append(string.Format("<U_EXTERNAL_REFERENCE>{0}</U_EXTERNAL_REFERENCE>", t.ExternalReference));
                        sbXml.Append("</TEST>");
                    }
                    sbXml.Append("</ALIQUOT>");
                }
                sbXml.Append("</SAMPLE>");
            }

            sbXml.Append("</login-request></lims-request>");
            return (sbXml.ToString());
        }


        private string CreateTestFindRequestXml(List<string> aliquotIds) {
            StringBuilder sbXml = new StringBuilder("<?xml version=\"1.0\"?><lims-request version=\"1\"><find-request attributes=\"No\"><ALIQUOT><find-by-id>");
            foreach (string aliquotId in aliquotIds) {
                sbXml.Append(string.Format("<id>{0}</id>", aliquotId));
            }
            sbXml.Append("</find-by-id><TEST><return><U_EXTERNAL_REFERENCE/></return>");
            sbXml.Append("<RESULT><find-by-name><name>Row ID</name></find-by-name></RESULT>");
            sbXml.Append("</TEST></ALIQUOT></find-request></lims-request>");
            return sbXml.ToString();
        }


        private string CreateResultEntryXml(XDocument doc) {
            StringBuilder sbXml = new StringBuilder("<?xml version=\"1.0\"?><lims-request version=\"1\">");

            IEnumerable<XElement> tests = doc.XPathSelectElements("//TEST");

            foreach (XElement test in tests) {
                sbXml.Append(string.Format("<result-request><load entity=\"TEST\" id=\"{0}\" mode=\"entry\">", test.Element("TEST_ID").Value.ToString()));



                //get the corresponding Test object in memory, which has the list of ad-hoc results needed
                Test selectedTest = (Test)FindDataObjectByDescription(test.Element("U_EXTERNAL_REFERENCE").Value.ToString());

                if (selectedTest != null) {
                    //the 'Row ID' results are there because the Nautilus xml processor has a bug that prevents ad-hoc results from being created
                    //under a test that has no results yet.  So the each workflow has this result logged in by the workflow rather than added ad-hoc.
                    IEnumerable<XElement> results = test.Elements("RESULT");
                    foreach (XElement result in results) {
                        sbXml.Append(string.Format("<result-entry result-id=\"{0}\" original-result=\"{1}\" />", result.Element("RESULT_ID").Value.ToString(), selectedTest.RowId));
                    }

                    foreach (Result r in selectedTest.Results) {
                        sbXml.Append(string.Format("<result-ad-hoc test-id=\"{0}\" name=\"{1}\" original-result=\"{2}\" template=\"{3}\"/>",
                            test.Element("TEST_ID").Value.ToString(),
                            r.Name,
                            r.ResultValue,
                            r.Template));
                    }
                }

                sbXml.Append("</load></result-request>");
            }
            sbXml.Append("</lims-request>");

            return sbXml.ToString();
        }


        private string CreateFindAllByGuidXml() {
            StringBuilder sbXml = new StringBuilder("<?xml version=\"1.0\"?><lims-request version=\"1\"><find-request attributes=\"No\"><SAMPLE><find-by-external-ref>");
            foreach (Sample s in _samples) {
                sbXml.Append(string.Format("<external-ref>{0}</external-ref>", s.ExternalReference));
            }
            sbXml.Append("</find-by-external-ref><return><external_reference/><name/><status/></return>");
            sbXml.Append("<ALIQUOT><return><external_reference/><name/><status/></return>");
            sbXml.Append("<TEST><return><u_external_reference/><name/><status/></return>");
            sbXml.Append("<RESULT><return><u_external_reference/><name/><status/><formatted_result/></return>");
            sbXml.Append("</RESULT></TEST></ALIQUOT></SAMPLE></find-request></lims-request>");

            return sbXml.ToString();
        }


        private string CreateSampleAuthoriseRequestXml() {
            StringBuilder sbXml = new StringBuilder("<?xml version=\"1.0\"?><lims-request version=\"1\"><login-request>");

            foreach (Sample s in _samples) {
                sbXml.Append(string.Format("<SAMPLE><find-by-external-ref>{0}</find-by-external-ref><STATUS>A</STATUS></SAMPLE>", s.ExternalReference));
            }

            sbXml.Append("</login-request></lims-request>");

            return sbXml.ToString();
        }


        private string CreateSampleFindRequestXml(XDocument doc) {
            IEnumerable<System.Xml.Linq.XElement> samples = doc.XPathSelectElements("//SAMPLE");

            StringBuilder sbXml = new StringBuilder("<?xml version=\"1.0\"?><lims-request version=\"1\"><find-request><SAMPLE><find-by-id>");

            foreach (XElement sample in samples) {
                sbXml.Append(string.Format("<id>{0}</id>", sample.Element("SAMPLE_ID").Value.ToString()));
            }

            sbXml.Append("</find-by-id><return><external_reference/><status/><name/></return></SAMPLE></find-request></lims-request>");

            return sbXml.ToString();
        }


        private XDocument ProcessXml(string xmlInput, out string errors) {
            MSXML2.DOMDocument60 request = new MSXML2.DOMDocument60();
            MSXML2.DOMDocument60 response = new MSXML2.DOMDocument60();
            request.loadXML(xmlInput);
            errors = _processXml.ProcessXMLWithResponse(request, response);
            if (!string.IsNullOrEmpty(errors)) {
                throw new ProcessXmlException(errors);
            }
            return XDocument.Parse(response.xml.ToString());
        }


        #endregion

        private void tvSample_DragDrop(object sender, DragEventArgs e) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> validFiles = new List<string>();
            List<string> validFileNames = new List<string>();
            FileInfo fi;
            
            for (int i = 0; i < files.Length; i++) {
                fi = new FileInfo(files[i]);

                if (fi.Extension.ToLower().Equals(".krt") || fi.Extension.ToLower().Equals(".edf")) {
                    validFiles.Add(fi.FullName);
                    validFileNames.Add(fi.Name);
                } else {
                    MessageBox.Show(string.Format("File {0} is not a vendor data file.  Files must be of type .krt or .edf.", fi.Name));
                }
            }

            LoadFiles(validFiles.ToArray(), validFileNames.ToArray());
        }

        private void tvSample_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false)) {
                e.Effect = DragDropEffects.All;
            }
        }

        private void tsmiRemove_Click(object sender, EventArgs e)
        {
            var cms = (ContextMenuStrip)((ToolStripMenuItem)sender).Owner;
            var tn = tvSample.GetNodeAt(tvSample.PointToClient(cms.Location));
            if (tn == null || !tn.ImageKey.StartsWith("t")) return;
            var aliquot = (Aliquot)FindParentDataObjectByDescription((string)tn.Tag);
            var test = (Test)FindDataObjectByDescription((string)tn.Tag);
            if (aliquot == null || test == null) return;
            aliquot.Tests.Remove(test);
            tn.Remove();
        }
    }

    public class ProcessXmlException : System.Exception {
        public ProcessXmlException(string message) : base(message) {
        }
    }
}
