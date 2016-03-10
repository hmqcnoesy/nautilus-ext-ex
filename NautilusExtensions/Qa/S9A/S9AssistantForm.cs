using System;
using System.Data.OracleClient;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using LSSERVICEPROVIDERLib;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;

namespace NautilusExtensions.Qa {
    public partial class S9AssistantForm : Form {

        private OracleConnection _connection;
        private S9Config _selectedConfig;
        private int _operatorId;
        private string _operatorName;
        private NautilusProcessXML _processXml;
        private bool _overrideValidationFailures = false;

        public S9AssistantForm(OracleConnection connection, NautilusProcessXML processXml, string operatorName, int operatorId) {
            InitializeComponent();
            _connection = connection;
            _processXml = processXml;
            _operatorId = operatorId;
            _operatorName = operatorName;
            txtAnalystName.Text = operatorName;
        }


        private void OpenFolder_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog()) {
                if (fbd.ShowDialog() == DialogResult.OK) {
                    DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);
                    FileInfo[] files = di.GetFiles("*.asc");

                    Array.Sort(files, (f1, f2) => f1.Name.CompareTo(f2.Name));

                    foreach (FileInfo fi in di.GetFiles("*.asc")) {
                        LoadFile(fi);
                    }
                }
            }
        }


        private void OpenFiles_Click(object sender, EventArgs e) {
            using (OpenFileDialog ofd = new OpenFileDialog()) {
                ofd.Multiselect = true;
                ofd.Filter = "S9A files (*.asc)|*.asc|All files (*.*)|*.*";
                ofd.ShowDialog();
                string[] files = ofd.FileNames;
                Array.Sort(files);
                FileInfo fi;

                for (int i = 0; i < files.Length; i++) {
                    fi = new FileInfo(files[i]);

                    if (fi.Extension.ToUpper().Equals(".ASC")) {
                        LoadFile(fi);
                    }
                }
            }
        }


        private void lboxAliquots_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false)) {
                e.Effect = DragDropEffects.All;
            }
        }


        private void lboxAliquots_DragDrop(object sender, DragEventArgs e) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Array.Sort(files);
            FileInfo fi;

            for (int i = 0; i < files.Length; i++) {
                fi = new FileInfo(files[i]);

                // ascii files - load em up
                if (fi.Extension.ToUpper().Equals(".ASC")) {
                    LoadFile(fi);
                }

                // folders - load any ascii files in them
                if (string.IsNullOrEmpty(fi.Extension)) {
                    if (Directory.Exists(fi.FullName)) {
                        DirectoryInfo di = new DirectoryInfo(fi.FullName);
                        FileInfo[] shortcuttedFolderFiles = di.GetFiles("*.asc", SearchOption.TopDirectoryOnly);

                        foreach (FileInfo fiShortcutted in shortcuttedFolderFiles) {
                            LoadFile(fiShortcutted);
                        }
                    }
                }

                // shortcuts - check to see if they are shortcuts to a directory.  If so, load any ascii files in that directory
                if (fi.Extension.ToUpper().Equals(".LNK")) {
                    IWshRuntimeLibrary.IWshShell shell = new IWshRuntimeLibrary.WshShell();
                    IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(fi.FullName);

                    if (Directory.Exists(shortcut.TargetPath)) {
                        DirectoryInfo di = new DirectoryInfo(shortcut.TargetPath);
                        FileInfo[] shortcuttedFolderFiles = di.GetFiles("*.asc", SearchOption.TopDirectoryOnly);

                        foreach (FileInfo fiShortcutted in shortcuttedFolderFiles) {
                            LoadFile(fiShortcutted);
                        }
                    }
                }
            }
        }


        private bool LoadConfig(S9AsciiFile file) {
            
            List<string> configNames = GetConfigNames(file.AliquotName);

            if (_selectedConfig == null) {

                if (configNames.Count == 1) {
                    SelectConfig(configNames[0]);
                    txtSettings.Text = _selectedConfig.Name;
                } else if (configNames.Count == 0) {
                    MessageBox.Show(string.Format("File '{0}' specifies aliquot '{1}' which has no tests with associated S9Config items.", file.Name, file.AliquotName));
                    return false;
                } else {
                    // prompt user for config selection
                    using (S9ConfigSelectionForm csf = new S9ConfigSelectionForm(configNames)) {
                        DialogResult dr = csf.ShowDialog();
                        if (dr == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(csf.SelectedConfig)) {
                            SelectConfig(csf.SelectedConfig);
                            txtSettings.Text = _selectedConfig.Name;
                        } else {
                            return false;
                        }
                    }
                }

                return true;
            } else {
                // check that this file's aliquot's tests' configs match the selected.  If not, should not load.
                if (configNames.Contains(_selectedConfig.Name)) {
                    return true;
                } else {
                    MessageBox.Show(string.Format("Ignoring file - the configs found for {0} are:\r\n{1}\r\n\r\nMismatch with selected config:\r\n'{2}'",
                        file.Name,
                        string.Join("\r\n", configNames),
                        _selectedConfig.Name));
                    return false;
                }
            }
        }


        private void LoadFile(FileInfo fi) {

            // don't add the file if it is already in the listbox
            foreach (object o in lboxAliquots.Items) {
                if (((S9AsciiFile)o).Name.Equals(fi.Name)) {
                    MessageBox.Show(string.Format("File '{0}' was not added because a file by the same name has already been loaded.", fi.Name));
                    return;
                }
            }

            var file = new S9AsciiFile(fi);
            if (file.IsFileValid) {
                if (!LoadConfig(file)) return;

                file.Parse(_selectedConfig);
                if (file.IsFileValid) {
                    file.Validate(_selectedConfig);
                    lboxAliquots.Items.Add(file);
                }
            }
        }


        private void DeleteItem_Click(object sender, EventArgs e) {
            object o = lboxAliquots.SelectedItem;
            if (o != null) {
                lboxAliquots.Items.Remove(o);
                ClearControls();
            }
        }


        private void lboxAliquots_SelectedIndexChanged(object sender, EventArgs e) {
            ClearControls();

            S9AsciiFile file = lboxAliquots.SelectedItem as S9AsciiFile;

            if (file == null) return;

            txtAliquotName.Text = file.AliquotName;
            txtCrossheadSpeed.Text = file.CrossheadSpeed;
            txtInstrument.Text = file.InstrumentName;
            txtTemperature.Text = file.Temperature;
            txtHumidity.Text = file.Humidity;
            txtValidSpecimens.Text = string.Format("{0} Valid / {1} Total", file.ValidSpecimenCount, file.TotalSpecimenCount);
            
            foreach (string s in file.ValidationErrors) {
                txtValidation.AppendText(s + "\r\n");
            }

            dgvSlNumbers.DataSource = file.Tools;
            dgvSlNumbers.AutoResizeColumns();

            dgvResults.DataSource = file.Results;
            dgvResults.AutoResizeColumns();
        }


        private void ClearControls() {
            txtAliquotName.Text = string.Empty;
            txtCrossheadSpeed.Text = string.Empty;
            txtInstrument.Text = string.Empty;
            txtTemperature.Text = string.Empty;
            txtHumidity.Text = string.Empty;
            txtValidSpecimens.Text = string.Empty;
            txtValidation.Text = string.Empty;
            dgvSlNumbers.DataSource = null;
            dgvResults.DataSource = null;
        }


        private void ViewFile_Click(object sender, EventArgs e) {
            S9AsciiFile file = (S9AsciiFile)lboxAliquots.SelectedItem;

            if (file != null) {
                System.Diagnostics.Process.Start(file.FullName);
            }
        }


        private List<string> GetConfigNames(string aliquotName) {
            List<string> list = new List<string>();

            string sqlString = "select distinct s.name from lims_sys.aliquot a, lims_sys.test t, lims_sys.test_user tu, lims_sys.u_s9_config s "
                + "where a.aliquot_id = t.aliquot_id and t.test_id = tu.test_id and tu.u_s9_config_id = s.u_s9_config_id "
                + "and a.name = :in_name order by s.name ";
            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_name", aliquotName));
            OracleDataReader reader = command.ExecuteReader();

            while (reader.Read()) {
                list.Add(reader[0].ToString());
            }

            reader.Close();
            return list;
        }


        private bool SelectConfig(string configName) {
            bool returnValue = false;
            string sqlString = "select s.*, su.* from lims_sys.u_s9_config s, lims_sys.u_s9_config_user su "
                + "where s.u_s9_config_id = su.u_s9_config_id and s.name = :in_name ";
            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_name", configName));
            OracleDataReader reader = command.ExecuteReader();

            while (reader.Read()) {
                returnValue = true;
                _selectedConfig = new S9Config(reader["u_result_file_locations"].ToString(),
                        reader["u_raw_data_file_locations"].ToString(), reader["u_user_field_additional"].ToString(), reader["u_calculations"].ToString());

                // information
                _selectedConfig.Name = reader["name"].ToString();
                _selectedConfig.Specification = reader["u_specification"].ToString();
                _selectedConfig.Description = reader["description"].ToString();

                // file options
                _selectedConfig.ResultFileExtension = reader["u_result_file_ext"].ToString();
                _selectedConfig.RawFileExtension = reader["u_raw_data_file_ext"].ToString();
                _selectedConfig.CheckCrossheadSpeed = reader["u_check_spec_speed"].ToString().Equals("T");
                _selectedConfig.RequireRawDataFile = reader["u_require_raw_data_file"].ToString().Equals("T");
                _selectedConfig.UseXmlProcessor = reader["u_use_xml"].ToString().Equals("T");
                _selectedConfig.CreateAdHocResults = reader["u_ad_hoc_results"].ToString().Equals("T");

                // validation
                _selectedConfig.CrossheadSpeedUpper = reader.IsDBNull(reader.GetOrdinal("u_upper_spec_speed")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_upper_spec_speed"));
                _selectedConfig.CrossheadSpeedLower = reader.IsDBNull(reader.GetOrdinal("u_lower_spec_speed")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_lower_spec_speed"));
                _selectedConfig.CheckTemperature = reader["u_check_temperature"].ToString().Equals("T");
                _selectedConfig.TemperatureUpper = reader.IsDBNull(reader.GetOrdinal("u_upper_temperature")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_upper_temperature"));
                _selectedConfig.TemperatureLower = reader.IsDBNull(reader.GetOrdinal("u_lower_temperature")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_lower_temperature"));
                _selectedConfig.CheckHumidity = reader["u_check_humidity"].ToString().Equals("T");
                _selectedConfig.HumidityUpper = reader.IsDBNull(reader.GetOrdinal("u_upper_humidity")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_upper_humidity"));
                _selectedConfig.HumidityLower = reader.IsDBNull(reader.GetOrdinal("u_lower_humidity")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_lower_humidity"));
                _selectedConfig.CheckValidSpecimenCount = reader["u_check_specimen_count"].ToString().Equals("T");
                _selectedConfig.ValidSpecimenUpper = reader.IsDBNull(reader.GetOrdinal("u_upper_specimen_count")) ? null : (int?)reader.GetDecimal(reader.GetOrdinal("u_upper_specimen_count"));
                _selectedConfig.ValidSpecimenLower = reader.IsDBNull(reader.GetOrdinal("u_lower_specimen_count")) ? null : (int?)reader.GetDecimal(reader.GetOrdinal("u_lower_specimen_count"));
                _selectedConfig.CheckTotalSpecimenCount = reader["u_check_tot_specimen_count"].ToString().Equals("T");
                _selectedConfig.TotalSpecimenUpper = reader.IsDBNull(reader.GetOrdinal("u_upper_tot_specimen_count")) ? null : (int?)reader.GetDecimal(reader.GetOrdinal("u_upper_tot_specimen_count"));
                _selectedConfig.TotalSpecimenLower = reader.IsDBNull(reader.GetOrdinal("u_lower_tot_specimen_count")) ? null : (int?)reader.GetDecimal(reader.GetOrdinal("u_lower_tot_specimen_count"));
                _selectedConfig.CheckDimension1 = reader["u_check_dimension_1"].ToString().Equals("T");
                _selectedConfig.Dimension1Upper = reader.IsDBNull(reader.GetOrdinal("u_upper_dimension_1")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_upper_dimension_1"));
                _selectedConfig.Dimension1Lower = reader.IsDBNull(reader.GetOrdinal("u_lower_dimension_1")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_lower_dimension_1"));
                _selectedConfig.CheckDimension2 = reader["u_check_dimension_2"].ToString().Equals("T");
                _selectedConfig.Dimension2Upper = reader.IsDBNull(reader.GetOrdinal("u_upper_dimension_2")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_upper_dimension_2"));
                _selectedConfig.Dimension2Lower = reader.IsDBNull(reader.GetOrdinal("u_lower_dimension_2")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_lower_dimension_2"));
                _selectedConfig.CheckDimension3 = reader["u_check_dimension_3"].ToString().Equals("T");
                _selectedConfig.Dimension3Upper = reader.IsDBNull(reader.GetOrdinal("u_upper_dimension_3")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_upper_dimension_3"));
                _selectedConfig.Dimension3Lower = reader.IsDBNull(reader.GetOrdinal("u_lower_dimension_3")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_lower_dimension_3"));
                _selectedConfig.CheckDimension4 = reader["u_check_dimension_4"].ToString().Equals("T");
                _selectedConfig.Dimension4Upper = reader.IsDBNull(reader.GetOrdinal("u_upper_dimension_4")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_upper_dimension_4"));
                _selectedConfig.Dimension4Lower = reader.IsDBNull(reader.GetOrdinal("u_lower_dimension_4")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("u_lower_dimension_4"));
                _selectedConfig.CheckFailureModes = reader["u_check_failure_mode"].ToString().Equals("T");
                _selectedConfig.FailureModes = reader["u_failure_mode"].ToString();
                _selectedConfig.FailureModesConciseFormat = reader["u_failure_mode_concise"].ToString().Equals("T");

                // user defined prompts
                _selectedConfig.UserField2 = reader["u_user_field_2"].ToString().Equals("T");
                _selectedConfig.UserField3 = reader["u_user_field_3"].ToString().Equals("T");
                _selectedConfig.UserField4 = reader["u_user_field_4"].ToString().Equals("T");

                // calculation checks
                _selectedConfig.IncludeDrim = reader["u_include_drim"].ToString().Equals("T");
                _selectedConfig.CheckActuals = reader["u_check_actuals"].ToString().Equals("T");
                _selectedConfig.CheckAverage = reader["u_check_average"].ToString().Equals("T");
                _selectedConfig.CheckMedian = reader["u_check_median"].ToString().Equals("T");
            }
            reader.Close();
            return returnValue;
        }


        private void btnDeleteAll_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Remove all files from list?", "Remove All", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;

            ClearControls();
            _selectedConfig = null;
            txtSettings.Text = string.Empty;
            //dgvSlNumbers.DataSource = null;
            lboxAliquots.Items.Clear();
        }

        private void btnSendData_Click(object sender, EventArgs e) {

            if (lboxAliquots.Items.Count < 1) return;

            Cursor c = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            // The tooling numbers need a separate validation, they apply to the entire group of files
            //if (!_overrideValidationFailures) {
            //    foreach (DataRow dr in (dgvSlNumbers.DataSource as DataTable).Rows) {
            //        if (!Regex.IsMatch(dr[1].ToString().Trim() + " ", @"^(((SL)|(SA)|(GT)|(PT))[0-9]{6}\s)+$")) {
            //            MessageBox.Show(string.Format("Tooling value failed formatting validation:\r\n'{0}'\r\nPlease correct this value and try again.", dr[1].ToString()));
            //            return;
            //        }
            //    }
            //}

            List<S9AsciiFile> invalidFiles = new List<S9AsciiFile>();
            List<int> indexesOfFilesToRemove = new List<int>();

            foreach (S9AsciiFile file in lboxAliquots.Items) {
                // REVALIDATE, if necessary, and if not valid, leave the file alone, keep it loaded (processed files are removed)
                if (!_overrideValidationFailures) {
                    file.Validate(_selectedConfig);
                    if (!file.IsValid) {
                        invalidFiles.Add(file);
                        continue;
                    }
                }

                // calculate an average failure mode if config requires failure mode checking
                if (_selectedConfig.CheckFailureModes) {
                    foreach (DataRow dr in file.Results.Rows) {
                        if (dr[1].ToString().Equals("Average")) {
                            dr[file.Results.Columns.Count - 2] = CalculateAverageFailureMode(file);
                        }
                    }
                }

                // raw data file
                if (!string.IsNullOrEmpty(file.RawDataFileLocation)) {
                    foreach (string path in _selectedConfig.RawFileSaveLocations) {
                        try {
                            FileInfo fi = new FileInfo(file.RawDataFileLocation);
                            File.Copy(fi.FullName, Path.Combine(path, fi.Name), true);
                        } catch (Exception ex) {
                            MessageBox.Show(string.Format("Raw data file could not be copied from\r\n{0}\r\nto\r\n{1}\r\n{2}",
                                file.RawDataFileLocation,
                                Path.Combine(path, file.AliquotName + "." + _selectedConfig.RawFileExtension),
                                ex.Message));
                        }
                    }
                }

                // results file
                foreach (string path in _selectedConfig.ResultFileSaveLocations) {
                    try {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(path,
                                string.Format("{0} ({1}).{2}", file.AliquotName, _selectedConfig.Name, _selectedConfig.ResultFileExtension)))) {
                            sw.WriteLine("Aliquot Name," + file.AliquotName);
                            sw.WriteLine("Operator," + _operatorId);
                            sw.WriteLine("Crosshead Speed," + file.CrossheadSpeed);
                            sw.WriteLine("Instrument," + file.InstrumentName);
                            sw.WriteLine("Humidity," + file.Humidity);
                            sw.WriteLine("Temperature," + file.Temperature);

                            foreach (string pathRawFile in _selectedConfig.RawFileSaveLocations) {
                                sw.WriteLine("Raw Data File," + Path.Combine(pathRawFile, file.Name + "." + _selectedConfig.RawFileExtension));
                            }

                            foreach (DataColumn dc in file.Results.Columns) {
                                sw.Write(dc.ExtendedProperties["nautilustestname"] + ",");
                            }

                            sw.WriteLine();

                            foreach (DataRow dr in file.Results.Rows) {
                                foreach (DataColumn dc in file.Results.Columns) {
                                    if (dc.ColumnName.Equals("Specimen") && string.IsNullOrEmpty(dr[dc.ColumnName].ToString())) {
                                        sw.Write("0,");
                                    } else {
                                        sw.Write(dr[dc.ColumnName].ToString() + ",");
                                    }
                                }
                                sw.WriteLine();
                            }

                            sw.Write("SL Numbers,");
                            foreach (DataRow dr in file.Tools.Rows) {
                                sw.Write(dr[1].ToString().Trim() + " ");
                            }

                            sw.WriteLine();

                            sw.Flush();
                            sw.Close();
                        }
                    } catch (Exception ex) {
                        MessageBox.Show(string.Format("Results file could not be saved at location\r\n{0}\r\n{1}",
                            Path.Combine(path, file.AliquotName + "." + _selectedConfig.ResultFileExtension),
                            ex.Message));
                    }
                }

                // TO DO xml processor
                if (_selectedConfig.UseXmlProcessor) {
                    string results = InputResultsUsingXml(file);
                    if (!string.IsNullOrEmpty(results)) {
                        MessageBox.Show(string.Format("There was a problem processing XML results for aliquot {0}:\r\n{1}", file.AliquotName, results));
                        continue;
                    }
                }

                // this can't be done because an item can't be removed using an enumerator within loop
                //lboxAliquots.Items.Remove(file);
                // need to do it this way
                indexesOfFilesToRemove.Add(lboxAliquots.FindStringExact(file.ToString()));
            }

            for (int i = indexesOfFilesToRemove.Count - 1; i >= 0; i--) {
                lboxAliquots.Items.RemoveAt(indexesOfFilesToRemove[i]);
            }

            if (lboxAliquots.Items.Count == 0) {
                ClearControls();
                _selectedConfig = null;
                txtSettings.Text = string.Empty;
                //dgvSlNumbers.DataSource = null;
            }

            Cursor.Current = c;

            if (invalidFiles.Count > 0) {
                MessageBox.Show("The following files failed validation, and have not been removed from the list:\r\n" + string.Join("\r\n", invalidFiles));
            }
        }


        /// <summary>
        /// Inputs nautilus results for a file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string InputResultsUsingXml(S9AsciiFile file) {
            StringBuilder sbXmlResultEntry, sbXmlResultInfo;
            string errors = string.Empty;

            string findAliquotXml = string.Format("<?xml version=\"1.0\"?><lims-request version=\"1\"><find-request attributes=\"No\">"
                + "<ALIQUOT><find-by-name><name>{0}</name></find-by-name>"
                + "<TEST><return><NAME/><DESCRIPTION/></return>"
                + "<RESULT><return><NAME/></return></RESULT>"
                + "</TEST></ALIQUOT></find-request></lims-request>", file.AliquotName);

            XDocument doc = ProcessXml(findAliquotXml, out errors);
            
            if (!string.IsNullOrEmpty(errors)) return errors;

            // get test names from response document
            IEnumerable<XElement> tests = doc.XPathSelectElements("//TEST");
            IEnumerable<XElement> results;
            string resultValue, resultFailureMode, resultComments, resultDescription, resultDimension, resultSpecimenId;
            bool foundResult;

            Dictionary<string, Tuple<string, string>> columnAndTestNames = new Dictionary<string, Tuple<string, string>>();
            foreach (DataColumn c in file.Results.Columns)
            {
                columnAndTestNames.Add(c.ColumnName, new Tuple<string, string>(c.GetExPropNautilusTestName(), c.GetExPropNautilusTestDescription()));
            }

            foreach (XElement test in tests) {
                var testNameFromXml = test.Element("NAME").Value.ToString();
                var testDescFromXml = test.Element("DESCRIPTION").Value.ToString();
                var matchingColumnName = string.Empty;

                foreach (var kvp in columnAndTestNames)
                {
                    if (kvp.Value.Item1 == testNameFromXml && (string.IsNullOrEmpty(kvp.Value.Item2) || testDescFromXml == kvp.Value.Item2))
                    {
                        matchingColumnName = kvp.Key;
                    }
                }

                if (string.IsNullOrEmpty(matchingColumnName)) continue;

                // make a test info xml request to input test-level info, such as temp, humidity, etc.
                string testXml = string.Format("<?xml version=\"1.0\"?><lims-request><login-request>"
                    + "<TEST><find-by-id>{0}</find-by-id>"
                    + "<U_TESTED_BY>{1}</U_TESTED_BY>"
                    + "<U_INSTRUMENT>{2}</U_INSTRUMENT>"
                    + "<U_SPEED>{3}</U_SPEED>"
                    + "<U_TEMPERATURE>{4}</U_TEMPERATURE>"
                    + "<U_HUMIDITY>{5}</U_HUMIDITY>"
                    + "<U_RAW_DATA_FILE_LOCATION>{6}</U_RAW_DATA_FILE_LOCATION>"
                    + "</TEST></login-request></lims-request>",
                    test.Element("TEST_ID").Value.ToString(),
                    _operatorName,
                    file.InstrumentName,
                    file.CrossheadSpeed,
                    file.Temperature,
                    file.Humidity,
                    file.RawDataFileLocation);

                doc = ProcessXml(testXml, out errors);
                if (!string.IsNullOrEmpty(errors)) return errors;


                // make a result entry xml
                sbXmlResultInfo = new StringBuilder("<?xml version=\"1.0\"?><lims-request version=\"1\"><login-request>");

                sbXmlResultEntry = new StringBuilder("<?xml version=\"1.0\"?><lims-request version=\"1\">");
                sbXmlResultEntry.Append(string.Format("<result-request><load entity=\"TEST\" id=\"{0}\" mode=\"entry\">", test.Element("TEST_ID").Value.ToString()));

                if (_selectedConfig.CreateAdHocResults) {
                    // need to find the result ID of the SL Numbers result
                    results = test.Elements("RESULT");
                    foreach (XElement result in results) {
                        if (result.Element("NAME").Value.ToString().Equals("SL Numbers")) {

                            string slNumber = string.Empty;
                            foreach (DataRow drTool in file.Tools.Rows) {
                                slNumber += drTool[1].ToString() + " ";
                            }

                            sbXmlResultEntry.Append(string.Format("<result-entry result-id=\"{0}\" original-result=\"{1}\" />",
                                result.Element("RESULT_ID").Value.ToString(),
                                slNumber.TrimEnd(", ".ToCharArray())));
                            continue;
                        }
                    }

                    foreach (DataRow dr in file.Results.Rows) {
                        sbXmlResultEntry.Append(string.Format("<result-ad-hoc test-id=\"{0}\" name=\"{1}\" original-result=\"{2}\" template=\"Text\" />",
                            test.Element("TEST_ID").Value.ToString(),
                            dr[1].ToString(),
                            dr[matchingColumnName].ToString()));
                    }
                } else {
                    results = test.Elements("RESULT");

                    foreach (XElement result in results) {

                        // special case for SL Numbers, whose results don't come from results grid, but the tools grid
                        if (result.Element("NAME").Value.ToString().Equals("SL Numbers")) {

                            string slNumber = string.Empty;
                            foreach (DataRow drTool in file.Tools.Rows) {
                                slNumber += drTool[1].ToString() + " ";
                            }

                            sbXmlResultEntry.Append(string.Format("<result-entry result-id=\"{0}\" original-result=\"{1}\" />",
                                result.Element("RESULT_ID").Value.ToString(),
                                slNumber.TrimEnd(", ".ToCharArray())));
                            continue;
                        }

                        foundResult = false;
                        resultValue = string.Empty;
                        resultFailureMode = string.Empty;
                        resultComments = string.Empty;
                        resultDescription = string.Empty;
                        resultDimension = string.Empty;
                        resultSpecimenId = string.Empty;

                        foreach (DataRow dr in file.Results.Rows) {
                            if (dr[1].ToString().Equals(result.Element("NAME").Value.ToString())) {
                                resultValue = dr[matchingColumnName].ToString();
                                resultFailureMode = dr[file.Results.Columns.Count - 2].ToString();
                                resultComments = 
                                resultDescription = dr[2].ToString().ToLower().Equals("false") ? "INVALID" : string.Empty;
                                resultSpecimenId = dr[0].ToString();
                                foundResult = true;

                                // result dimensions
                                if (_selectedConfig.CheckDimension1) resultDimension += dr[file.Dimension1Name].ToString() + ",";
                                if (_selectedConfig.CheckDimension2) resultDimension += dr[file.Dimension2Name].ToString() + ",";
                                if (_selectedConfig.CheckDimension3) resultDimension += dr[file.Dimension3Name].ToString() + ",";
                                if (_selectedConfig.CheckDimension4) resultDimension += dr[file.Dimension4Name].ToString() + ",";
                                resultDimension = resultDimension.TrimEnd(",".ToCharArray());
                                break;
                            }
                        }

                        if (foundResult) {
                            sbXmlResultEntry.Append(string.Format("<result-entry result-id=\"{0}\" original-result=\"{1}\" />",
                                result.Element("RESULT_ID").Value.ToString(),
                                resultValue));
                            sbXmlResultInfo.Append(string.Format("<RESULT><find-by-id>{0}</find-by-id>", result.Element("RESULT_ID").Value.ToString()));
                            sbXmlResultInfo.Append(string.Format("<DESCRIPTION>{0}</DESCRIPTION>", resultDescription));
                            sbXmlResultInfo.Append(string.Format("<U_SPECIMEN_ID>{0}</U_SPECIMEN_ID>", resultSpecimenId));
                            sbXmlResultInfo.Append(string.Format("<U_SPECIMEN_DIMENSION>{0}</U_SPECIMEN_DIMENSION>", resultDimension));
                            sbXmlResultInfo.Append(string.Format("<U_FAILURE_MODE>{0}</U_FAILURE_MODE>", resultFailureMode));
                            sbXmlResultInfo.Append(string.Format("<U_EXTERNAL_REFERENCE>{0}</U_EXTERNAL_REFERENCE>", Guid.NewGuid().ToString()));
                            sbXmlResultInfo.Append(string.Format("<U_COMMENTS>{0}</U_COMMENTS></RESULT>", resultComments));
                        }
                    }
                }


                sbXmlResultEntry.Append("</load></result-request></lims-request>");
                doc = ProcessXml(sbXmlResultEntry.ToString(), out errors);
                if (!string.IsNullOrEmpty(errors)) return errors;

                // if used ad-hoc results, the result info xml hasn't been built, because the result IDs were not known until now.
                // get them out of the response doc, and build up the result info xml
                if (_selectedConfig.CreateAdHocResults) {

                    results = doc.XPathSelectElements("//result-ad-hoc");
                    foreach (XElement result in results) {
                        string resultName = result.Attribute("name").Value.ToString();
                        string resultId = result.Attribute("result-id").Value.ToString();

                        foreach (DataRow dr in file.Results.Rows) {
                            if (dr[1].ToString().Equals(result.Attribute("name").Value.ToString())) {
                                resultDimension = string.Empty;
                                if (_selectedConfig.CheckDimension1) resultDimension += dr[file.Dimension1Name].ToString() + ",";
                                if (_selectedConfig.CheckDimension2) resultDimension += dr[file.Dimension2Name].ToString() + ",";
                                if (_selectedConfig.CheckDimension3) resultDimension += dr[file.Dimension3Name].ToString() + ",";
                                if (_selectedConfig.CheckDimension4) resultDimension += dr[file.Dimension4Name].ToString() + ",";
                                resultDimension = resultDimension.TrimEnd(",".ToCharArray());

                                sbXmlResultInfo.Append(string.Format("<RESULT><find-by-id>{0}</find-by-id>", result.Attribute("result-id").Value.ToString()));
                                sbXmlResultInfo.Append(string.Format("<DESCRIPTION>{0}</DESCRIPTION>", dr[2].ToString().ToLower().Equals("false") ? "INVALID" : string.Empty));
                                sbXmlResultInfo.Append(string.Format("<U_SPECIMEN_ID>{0}</U_SPECIMEN_ID>", dr[0].ToString()));
                                sbXmlResultInfo.Append(string.Format("<U_SPECIMEN_DIMENSION>{0}</U_SPECIMEN_DIMENSION>", resultDimension));
                                sbXmlResultInfo.Append(string.Format("<U_FAILURE_MODE>{0}</U_FAILURE_MODE>", dr[file.Results.Columns.Count - 2].ToString()));
                                sbXmlResultInfo.Append(string.Format("<U_EXTERNAL_REFERENCE>{0}</U_EXTERNAL_REFERENCE>", Guid.NewGuid().ToString()));
                                sbXmlResultInfo.Append(string.Format("<U_COMMENTS>{0}</U_COMMENTS></RESULT>", dr[file.Results.Columns.Count - 1].ToString()));
                                break;
                            }
                        }
                    }
                }

                sbXmlResultInfo.Append(string.Format("</login-request></lims-request>"));
                doc = ProcessXml(sbXmlResultInfo.ToString(), out errors);
                if (!string.IsNullOrEmpty(errors)) return errors;
            }

            return string.Empty;
        }


        private string CalculateAverageFailureMode(S9AsciiFile file) {
            SortedDictionary<string, int> failureModeSummations = new SortedDictionary<string, int>();
            int specimenCount = 0;
            string tempString;
            string tempInt;
            bool isPrevCharInt;
            string[] codesAndValues;

            foreach (DataRow dr in file.Results.Rows) {
                if (_selectedConfig.FailureModesConciseFormat) {
                    isPrevCharInt = true;
                    tempInt = string.Empty;
                    tempString = string.Empty;
                    if (!string.IsNullOrEmpty(dr[0].ToString()) && (bool)dr[2]) {
                        specimenCount++;
                        foreach (char c in dr[file.Results.Columns.Count - 2].ToString()) {
                            if (Regex.IsMatch(c.ToString(), "^[0-9]$")) {
                                if (isPrevCharInt) {
                                    tempInt += c;
                                } else {
                                    if (failureModeSummations.ContainsKey(tempString)) {
                                        failureModeSummations[tempString] += int.Parse(tempInt);
                                    } else {
                                        failureModeSummations.Add(tempString, int.Parse(tempInt));
                                    }
                                    tempInt = c.ToString();
                                    tempString = string.Empty;
                                }
                                isPrevCharInt = true;
                            } else {
                                tempString += c;
                                isPrevCharInt = false;
                            }
                        }

                        if (failureModeSummations.ContainsKey(tempString)) {
                            failureModeSummations[tempString] += int.Parse(tempInt);
                        } else {
                            failureModeSummations.Add(tempString, int.Parse(tempInt));
                        }
                    }
                } else {
                    if (!string.IsNullOrEmpty(dr[0].ToString()) && (bool)dr[2]) {
                        specimenCount++;
                        codesAndValues = dr[file.Results.Columns.Count - 2].ToString().Split(",/ ".ToCharArray());

                        for (int i = 0; i < codesAndValues.Length; i++) {
                            if (failureModeSummations.ContainsKey(codesAndValues[i])) {
                                failureModeSummations[codesAndValues[i]] += int.Parse(codesAndValues[++i]);
                            } else {
                                failureModeSummations.Add(codesAndValues[i], int.Parse(codesAndValues[++i]));
                            }
                        }
                    }
                }
            }

            if (specimenCount == 0) return string.Empty;

            // divide each value by number of specimens
            string returnValue = string.Empty;
            if (_selectedConfig.FailureModesConciseFormat) {
                foreach (KeyValuePair<string, int> kvp in failureModeSummations) {
                    returnValue += ((decimal)kvp.Value / (decimal)specimenCount).ToString("0");
                    returnValue += kvp.Key;
                }
            } else {
                foreach (KeyValuePair<string, int> kvp in failureModeSummations) {
                    returnValue += kvp.Key + "/";
                    returnValue += ((decimal)kvp.Value / (decimal)specimenCount).ToString("0") + ",";
                }
            }

            return returnValue;
        }

        private void btnRevalidate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            S9AsciiFile file = (S9AsciiFile)lboxAliquots.SelectedItem;

            if (file == null) return;

            file.Validate(_selectedConfig);
            txtValidation.Lines = file.ValidationErrors.ToArray();
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

        private void btnOverrideValidationFailures_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            txtOverrideValidationFailuresPassword.Visible = !txtOverrideValidationFailuresPassword.Visible;
            btnOverrideValidationFailuresOk.Visible = !btnOverrideValidationFailuresOk.Visible;
        }

        private void btnOverrideValidationFailuresOk_Click(object sender, EventArgs e) {

            if (_selectedConfig == null) {
                MessageBox.Show("No files are loaded.");
                return;
            }

            // what criminal mastermind can penetrate this fortress of security?
            if (txtOverrideValidationFailuresPassword.Text.Equals(_selectedConfig.Name.PadRight(5).Substring(0, 5))) {
                _overrideValidationFailures = true;
                lblOverrideValidationFailuresStatus.Text = "ON";
                txtOverrideValidationFailuresPassword.Clear();
                txtOverrideValidationFailuresPassword.Visible = false;
                btnOverrideValidationFailuresOk.Visible = false;
            } else {
                _overrideValidationFailures = false;
                lblOverrideValidationFailuresStatus.Text = "OFF";
                MessageBox.Show("Password incorrect.");
            }
        }
    }
}
