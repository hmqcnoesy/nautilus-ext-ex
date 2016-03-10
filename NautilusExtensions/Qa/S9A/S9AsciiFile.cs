using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using Excel;

namespace NautilusExtensions.Qa {
    public class S9AsciiFile {
        public string Dimension1Name { get { return "Dim 1"; } }
        public string Dimension2Name { get { return "Dim 2"; } }
        public string Dimension3Name { get { return "Dim 3"; } }
        public string Dimension4Name { get { return "Dim 4"; } }

        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string RawDataFileLocation { get; private set; }
        public string AliquotName { get; private set; }
        public string EmployeeId { get; private set; }
        public string InstrumentName { get; private set; }
        public string CrossheadSpeed { get; private set; }
        public string Temperature { get; private set; }
        public string Humidity { get; private set; }
        public string UserField2Key { get; set; }
        public string UserField2Value { get; set; }
        public string UserField3Key { get; set; }
        public string UserField3Value { get; set; }
        public string UserField4Key { get; set; }
        public string UserField4Value { get; set; }
        public int ValidSpecimenCount { get; private set; }
        public int TotalSpecimenCount { get; private set; }
        public System.Data.DataTable Results { get; private set; }
        public bool IsFileValid { get; set; }
        public List<string> ValidationErrors { get; private set; }
        public System.Data.DataTable Tools { get; set; }

        public bool IsValid { get { return ValidationErrors.Count == 0; } }

        private Dictionary<string, string> _statNames = new Dictionary<string, string>() {
            {"1", "Average"},
            {"2", "Standard Deviation"},
            {"3", "Minimum"},
            {"4", "Maximum"},
            {"5", "Mean - Standard Deviation"},
            {"6", "Mean + Standard Deviation"},
            {"7", "Coefficient of Variation"},
            {"8", "Median"}
        };


        public override string ToString() {
            return string.Format("{0} ({1})", Name.ToUpper().Replace(".ASC", string.Empty), AliquotName);
        }


        /// <summary>
        /// Constructor populates some variables, including AliquotName and SettingsName.  To populate all variables, the Parse() method must be called.
        /// </summary>
        /// <param name="fi"></param>
        public S9AsciiFile(FileInfo fi) {
            Name = fi.Name;
            FullName = fi.FullName;
            IsFileValid = true;  //this can be set to false if the ascii file is crappy, to prevent being displayed in gui

            //read the first line of text only
            using (StreamReader sr = File.OpenText(fi.FullName)) {
                string firstLine;
                firstLine = sr.ReadLine();


                //the header line must be at least 584 characters, otherwise there will be trouble
                if (firstLine == null || firstLine.Length < 584) {
                    MessageBox.Show(string.Format("The Series IX ascii file '{0}' is corrupt.  The header line does not have required info.", Name));
                    IsFileValid = false;
                    return;
                }


                //get specimen count
                int specimenCount;
                if (!int.TryParse(firstLine.Substring(13, 3).Trim(), out specimenCount)) {
                    MessageBox.Show(string.Format("The number of specimens in '{0}' could not be determined.  Please correct the file and try again.", Name));
                    IsFileValid = false;
                    return;
                }


                //Get header information
                EmployeeId = firstLine.Substring(37, 20).Trim();
                CrossheadSpeed = firstLine.Substring(124, 14).Trim();
                Humidity = firstLine.Substring(169, 5).Trim();
                Temperature = firstLine.Substring(175, 5).Trim();
                AliquotName = firstLine.Substring(218, 40).Trim();
                UserField2Key = firstLine.Substring(259, 20).Trim();
                UserField2Value = firstLine.Substring(279, 40).Trim();
                UserField3Key = firstLine.Substring(320, 20).Trim();
                UserField3Value = firstLine.Substring(340, 40).Trim();
                UserField4Key = firstLine.Substring(381, 20).Trim();
                UserField4Value = firstLine.Substring(401, 40).Trim();
                InstrumentName = firstLine.Substring(442, 20).Replace("Instron", string.Empty).Trim();
            }
        }


        /// <summary>
        /// Parses the remainder of a file, after using constructor.  IsFileValid should be checked before and after parsing.
        /// The list of strings with test names passed in should come from the file's specified setting.
        /// </summary>
        /// <param name="calculations"></param>
        public void Parse(S9Config config) {

            Tools = config.GetUserFieldsTable(UserField2Key, UserField2Value, UserField3Key, UserField3Value, UserField4Key, UserField4Value);
            Results = SetupResultsTable(config);
            DataTable dtDrim = config.IncludeDrim ? GetDRimResults() : null;

            using (StreamReader sr = File.OpenText(FullName)) 
            {
                
                string currentLine;
                currentLine = sr.ReadLine(); // first line was read in constructor, ingnored this time around.

                object[] rowToAdd;
                string specimenId, statSummaryId;

                while ((currentLine = sr.ReadLine()) != null) 
                {
                    if (currentLine.Length < 666) continue;    // this keeps any substring operations on the line safe up through dimension 4's position.

                    specimenId = currentLine.Substring(13, 3).Trim();  //specimen id is 1 through some number for specimens, blank for statistics
                    statSummaryId = currentLine.Substring(21, 1).Trim();      //stat id is blank for specimens, 1 through 8 for stats, indicates stat type (mean, median, cofv, etc.)

                    rowToAdd = new object[Results.Columns.Count];

                    if (!string.IsNullOrEmpty(specimenId)) 
                    {
                        rowToAdd[0] = specimenId;
                        rowToAdd[1] = "Actual " + specimenId;

                        //keep track of number of valid specimens.  This position in file indicates an invalidated test specimen
                        if (string.IsNullOrEmpty(currentLine.Substring(17, 1).Trim())) ValidSpecimenCount++;

                        // keep track of total number of specimens.
                        TotalSpecimenCount++;
                    } 
                    else 
                    {
                        rowToAdd[0] = string.Empty;
                        try 
                        {
                            rowToAdd[1] = _statNames[statSummaryId];
                        } 
                        catch 
                        {
                            IsFileValid = false;
                            MessageBox.Show(string.Format("The Series IX ascii file '{0}' is corrupt.  Invalid summary result ID ({0}).", Name, statSummaryId));
                            return;
                        }
                    }


                    // the R at this position indicates invalidated result
                    rowToAdd[2] = (!currentLine.Substring(17, 1).Trim().Equals("R"));

                    // specimen dimensions
                    int col = 3;
                    if (config.CheckDimension1) rowToAdd[col++] = currentLine.Substring(615, 12).Trim();
                    if (config.CheckDimension2) rowToAdd[col++] = currentLine.Substring(628, 12).Trim();
                    if (config.CheckDimension3) rowToAdd[col++] = currentLine.Substring(641, 12).Trim();
                    if (config.CheckDimension4) rowToAdd[col++] = currentLine.Substring(654, 12).Trim();

                    //add columns from settings here
                    int asciiColumnPosition;
                    foreach (KeyValuePair<string, S9Calculation> calculation in config.Calculations) 
                    {
                        int columnId = 0;

                        if (int.TryParse(calculation.Key, out columnId))
                        {

                            asciiColumnPosition = (columnId * 37) + 652;
                            if (currentLine.Length <= asciiColumnPosition + 13)
                            {
                                MessageBox.Show(string.Format("The settings for file '{0}' specify more calculation columns than the file contains."
                                    + "\r\nAttempted to access column '{1}'\r\nCorrect the file and try again.", Name, asciiColumnPosition.ToString()));
                                IsFileValid = false;
                                return;
                            }
                            rowToAdd[col++] = currentLine.Substring((columnId * 37) + 652, 12).Trim();
                        }
                        else if (dtDrim != null && Regex.IsMatch(calculation.Key, "[A-Z]"))
                        {
                            rowToAdd[col++] = GetDRimValue(dtDrim, rowToAdd[1].ToString(), calculation.Key);
                        }
                    }


                    // specimen label ???
                    rowToAdd[col] = currentLine.Substring(604, 10).Trim();
                    rowToAdd[col + 1] = string.Empty;    // this is the remarks column.

                    Results.Rows.Add(rowToAdd);
                }
            }
        }


        private string GetDRimValue(DataTable dtDrim, string replicate, string columnId)
        {
            // dtDrim.Select() won't work because we need to compare a string to doubles
            foreach (DataRow dr in dtDrim.Rows)
            {
                string specimenRowId = string.Empty;

                if (dr[0] is System.Double) specimenRowId = "Actual " + ((double)dr[0]).ToString("0");
                else if (dr[0].ToString() == "Average:") specimenRowId = "Average";
                else if (dr[0].ToString() == "Std Dev:") specimenRowId = "Standard Deviation";
                else if (dr[0].ToString() == "C.V.:") specimenRowId = "Coefficient of Variation";

                if (specimenRowId == replicate)
                {
                    return dr[columnId].ToString();
                }
            }

            return string.Empty;
        }


        private DataTable GetDRimResults()
        {
            var dt = GetDRimFileUsedRange();

            // delete unnecessary rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() == "No.")
                {
                    dt.Rows[i].Delete();
                    break;
                }

                dt.Rows[i].Delete();
            }

            // rename columns
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].ColumnName = ((char)(i + 65)).ToString();
            }

            dt.AcceptChanges();

            return dt;
        }


        private DataTable GetDRimFileUsedRange()
        {
            DataTable dt = null;
            var pathToXls = this.FullName.ToLower().Replace(".asc", "i.xls");

            if (!File.Exists(pathToXls))
            {
                IsFileValid = false;
                MessageBox.Show(string.Format("DRIM File '{0}' was not added because it cannot be found.", pathToXls));
                return null;
            }

            while (true)
            {
                try
                {
                    // excel data reader
                    FileStream stream = File.Open(pathToXls, FileMode.Open, FileAccess.Read);

                    IExcelDataReader xlReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    dt = xlReader.AsDataSet().Tables[0];

                    xlReader.Close();
                    xlReader.Dispose();
                    break;
                }
                catch
                {
                    MessageBox.Show("DRIM file could not be opened.  Please close any DRIM files you have opened, then click OK.");
                }
            }

            return dt;
        }

        /// <summary>
        /// Validates file's results against the settings passed in.
        /// </summary>
        /// <param name="setting"></param>
        public void Validate(S9Config config) {
            ValidationErrors = new List<string>();

            // check validation items
            if (config.CheckCrossheadSpeed) CheckConformance("Crosshead speed", CrossheadSpeed, config.CrossheadSpeedLower, config.CrossheadSpeedUpper);
            if (config.CheckTemperature) CheckConformance("Temperature", Temperature, config.TemperatureLower, config.TemperatureUpper);
            if (config.CheckHumidity) CheckConformance("Humidity", Humidity, config.HumidityLower, config.HumidityUpper);

            // check valid specimen count
            if (config.CheckValidSpecimenCount) {
                if (config.ValidSpecimenLower != null && ValidSpecimenCount < config.ValidSpecimenLower) {
                    ValidationErrors.Add(string.Format("Valid specimen count '{0}' doesn't meet min '{1}'.", ValidSpecimenCount, config.ValidSpecimenLower));
                } else if (config.ValidSpecimenUpper != null && ValidSpecimenCount > config.ValidSpecimenUpper) {
                    ValidationErrors.Add(string.Format("Valid specimen count '{0}' doesn't meet max '{1}'.", ValidSpecimenCount, config.ValidSpecimenUpper));
                }
            }

            // check total specimen count
            if (config.CheckTotalSpecimenCount) {
                if (config.TotalSpecimenLower != null && TotalSpecimenCount < config.TotalSpecimenLower) {
                    ValidationErrors.Add(string.Format("Total specimen count '{0}' doesn't meet min '{1}'.", TotalSpecimenCount, config.TotalSpecimenLower));
                } else if (config.TotalSpecimenUpper != null && TotalSpecimenCount > config.TotalSpecimenUpper) {
                    ValidationErrors.Add(string.Format("Total specimen count '{0}' doesn't meet max '{1}'.", TotalSpecimenCount, config.TotalSpecimenUpper));
                }
            }
            

            // check for corresponding raw data file if required
            if (config.RequireRawDataFile) {
                if (File.Exists((FullName.ToLower()).Replace(".asc", "." + config.RawFileExtension))) {
                    RawDataFileLocation = FullName.ToLower().Replace(".asc", "." + config.RawFileExtension);
                } else {
                    RawDataFileLocation = string.Empty;
                    ValidationErrors.Add(string.Format("File '{0}' requires a similarly named raw data file named with extension '{1}' in the same folder.",
                        this.Name, config.RawFileExtension));
                }
            }

            // check dimensions and results, failure modes
            int failureModeColumnIndex = Results.Columns.Count - 2;
            string failureMode;
            List<string> validCodes = new List<string>(config.FailureModes.Split(@"+- ,;/\".ToCharArray()));  // to allow most delimiters

            foreach (DataRow dr in Results.Rows) {
                if ((bool)dr[2]) {
                    // dimensions (only apply to actuals, not statistic results)
                    if (Results.Columns.Contains(Dimension1Name) && config.CheckDimension1 && !string.IsNullOrEmpty((string)dr[0])) {
                        CheckConformance("Dimension 1", (string)dr[Dimension1Name], config.Dimension1Lower, config.Dimension1Upper);
                    }

                    if (Results.Columns.Contains(Dimension2Name) && config.CheckDimension1 && !string.IsNullOrEmpty((string)dr[0])) {
                        CheckConformance("Dimension 2", (string)dr[Dimension2Name], config.Dimension2Lower, config.Dimension2Upper);
                    }

                    if (Results.Columns.Contains(Dimension3Name) && config.CheckDimension1 && !string.IsNullOrEmpty((string)dr[0])) {
                        CheckConformance("Dimension 3", (string)dr[Dimension3Name], config.Dimension3Lower, config.Dimension3Upper);
                    }

                    if (Results.Columns.Contains(Dimension4Name) && config.CheckDimension1 && !string.IsNullOrEmpty((string)dr[0])) {
                        CheckConformance("Dimension 4", (string)dr[Dimension4Name], config.Dimension4Lower, config.Dimension4Upper);
                    }

                    // results
                    foreach (S9Calculation calc in config.Calculations.Values) 
                    {
                        // actuals spec limits
                        if (Results.Columns.Contains(calc.DataTableColumnName) && !string.IsNullOrEmpty((string)dr[0]) && calc.CheckSpecLimits && config.CheckActuals) {
                            CheckConformance(calc.DataTableColumnName, (string)dr[calc.DataTableColumnName], calc.SpecLimitLower, calc.SpecLimitUpper);
                        }

                        // actuals stat limits
                        if (Results.Columns.Contains(calc.DataTableColumnName) && !string.IsNullOrEmpty((string)dr[0]) && calc.CheckStatLimits && config.CheckActuals)
                        {
                            CheckConformance(calc.DataTableColumnName, (string)dr[calc.DataTableColumnName], calc.StatLimitLower, calc.StatLimitUpper);
                        }

                        // average spec limits
                        if (Results.Columns.Contains(calc.DataTableColumnName) && (string)dr[1] == "Average" && calc.CheckSpecLimits && config.CheckAverage)
                        {
                            CheckConformance(calc.DataTableColumnName, (string)dr[calc.DataTableColumnName], calc.SpecLimitLower, calc.SpecLimitUpper);
                        }

                        // actuals stat limits
                        if (Results.Columns.Contains(calc.DataTableColumnName) && (string)dr[1] == "Average" && calc.CheckStatLimits && config.CheckAverage)
                        {
                            CheckConformance(calc.DataTableColumnName, (string)dr[calc.DataTableColumnName], calc.StatLimitLower, calc.StatLimitUpper);
                        }

                        // median spec limits
                        if (Results.Columns.Contains(calc.DataTableColumnName) && (string)dr[1] == "Median" && calc.CheckSpecLimits && config.CheckMedian)
                        {
                            CheckConformance(calc.DataTableColumnName, (string)dr[calc.DataTableColumnName], calc.SpecLimitLower, calc.SpecLimitUpper);
                        }

                        // median stat limits
                        if (Results.Columns.Contains(calc.DataTableColumnName) && (string)dr[1] == "Median" && calc.CheckStatLimits && config.CheckMedian)
                        {
                            CheckConformance(calc.DataTableColumnName, (string)dr[calc.DataTableColumnName], calc.StatLimitLower, calc.StatLimitUpper);
                        }


                    }

                    if (config.CheckFailureModes && !string.IsNullOrEmpty(dr[0].ToString())) {
                        // two valid schemes for failure modes.
                        // First is %CA%CB...  where % is the number between 1 and 100, CA is code A, CB is code B, etc.
                        // Second is CA/%,CB/%,... this one is used for Bacchus which has special codes.
                        // The First cannot be used in every situation, because some of the Bacchus failure mode codes are numbers only(so the / and , delimiters are needed to set off these codes).
                        
                        failureMode = dr[failureModeColumnIndex].ToString();

                        // failure mode must not be empty
                        if (string.IsNullOrEmpty(failureMode)) {
                            ValidationErrors.Add(string.Format("No failure mode specified for result '{0}'.", dr[1].ToString()));
                            continue;
                        }

                        if (config.FailureModesConciseFormat) {

                            // failure mode must start with a char between 1 and 9
                            if (!Regex.IsMatch(failureMode.Substring(0, 1), "^[1-9]$")) {
                                ValidationErrors.Add(string.Format("Failure mode '{0}' for result '{1}' does not begin with numeric.", failureMode, dr[1].ToString()));
                                continue;
                            }

                            // parse the failure mode value, ensure numbers add up to 100, and each code is in validCodes array
                            string temp = string.Empty;
                            List<string> usedCodes = new List<string>();
                            int sum = 0;
                            bool isPrevCharInt = true;
                            foreach (char c in dr[failureModeColumnIndex].ToString()) {
                                if (Regex.IsMatch(c.ToString(), "^[0-9]$")) {
                                    if (isPrevCharInt) {
                                        temp += c;
                                    } else {
                                        usedCodes.Add(temp);
                                        temp = c.ToString();
                                    }
                                    isPrevCharInt = true;
                                } else {
                                    if (isPrevCharInt) {
                                        sum += int.Parse(temp);
                                        temp = c.ToString();
                                    } else {
                                        temp += c;
                                    }
                                    isPrevCharInt = false;
                                }
                            }

                            // append the last code of failure mode to list of used codes.  Not done in loop.
                            usedCodes.Add(temp);

                            if (sum != 100) {
                                ValidationErrors.Add(string.Format("Failure mode '{0}' for result '{1}' does not add up to 100.", failureMode, dr[1].ToString()));
                            }

                            foreach (string code in usedCodes) {
                                if (!validCodes.Contains(code)) {
                                    ValidationErrors.Add(string.Format("Failure mode '{0}' for result '{1}' contains invalid code '{2}'.", failureMode, dr[1].ToString(), code));
                                }
                            }

                        } else {
                            // this should make an even-lengthed array of strings, where the values in the odd elements are codes, and values in even elements are percentages.
                            failureMode = failureMode.Replace(" ", string.Empty);
                            string[] codeValuePairs = failureMode.Split("/,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                            if (codeValuePairs.Length < 2 || codeValuePairs.Length % 2 != 0) {
                                ValidationErrors.Add(string.Format("Failure mode '{0}' for result '{1}' does not have proper delimiters.", failureMode, dr[1].ToString()));
                                continue;
                            }

                            // loop through elements, odd elements checked against valid codes, even elements summed up
                            int sum = 0, parsedInt = 0;
                            List<string> usedCodes = new List<string>();

                            for (int i = 0; i < codeValuePairs.Length; i++) {
                                if (i % 2 == 0) {
                                    if (!validCodes.Contains(codeValuePairs[i])) {
                                        ValidationErrors.Add(string.Format("Failure mode '{0}' for result '{1}' contains invalid code '{2}'.", failureMode, dr[1].ToString(), codeValuePairs[i]));
                                    }
                                } else {
                                    if (int.TryParse(codeValuePairs[i], out parsedInt)) {
                                        sum += parsedInt;
                                    } else {
                                        ValidationErrors.Add(string.Format("Failure mode '{0}' for result '{1}' has a non-numeric value in a 'percentage' position.", failureMode, dr[1].ToString()));
                                        break;
                                    }
                                }
                            }

                            if (sum != 100) {
                                ValidationErrors.Add(string.Format("Failure mode '{0}' for result '{1}' does not add up to 100.", failureMode, dr[1].ToString()));
                            }
                        }
                    }
                }
            }
        }


        private void CheckConformance(string propertyName, string valueToCheck, decimal? lowerLimit, decimal? upperLimit) {
            decimal parsedDecimal;
            CultureInfo cultureInfo = new CultureInfo("en-US");  // necessary to use tryparse overload that allows "E" for sci notation
            if (decimal.TryParse(valueToCheck, System.Globalization.NumberStyles.Float, cultureInfo, out parsedDecimal)) {
                if (lowerLimit != null && parsedDecimal < lowerLimit) {
                    ValidationErrors.Add(string.Format("{0} value '{1}' doesn't meet min '{2}'.", propertyName, valueToCheck, lowerLimit));
                } else if (upperLimit != null && parsedDecimal > upperLimit) {
                    ValidationErrors.Add(string.Format("{0} value '{1}' doesn't meet max '{2}'.", propertyName, valueToCheck, upperLimit));
                }
            } else {
                ValidationErrors.Add(string.Format("{0} value '{1}' is not numeric.", propertyName, valueToCheck));
            }
        }


        private System.Data.DataTable SetupResultsTable(S9Config config) {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add(new DataColumn("Specimen", typeof(string)) { ReadOnly = false });
            dt.Columns.Add(new DataColumn("Replicate", typeof(string)) { ReadOnly = true });
            dt.Columns.Add(new DataColumn("Valid", typeof(bool)) { ReadOnly = true });

            if (config.CheckDimension1) dt.Columns.Add(new DataColumn(Dimension1Name, typeof(string)) { ReadOnly = true });
            if (config.CheckDimension2) dt.Columns.Add(new DataColumn(Dimension2Name, typeof(string)) { ReadOnly = true });
            if (config.CheckDimension3) dt.Columns.Add(new DataColumn(Dimension3Name, typeof(string)) { ReadOnly = true });
            if (config.CheckDimension4) dt.Columns.Add(new DataColumn(Dimension4Name, typeof(string)) { ReadOnly = true });
            
            //each calculation string returned from the database needs a separate column
            foreach (S9Calculation c in config.Calculations.Values) {
                DataColumn dc = new DataColumn(c.DataTableColumnName, typeof(string)) { ReadOnly = true };
                dc.SetExPropNautilusTestName(c.NautilusTestName);
                dc.SetExPropNautilusTestDescription(c.NautilusTestDescription);
                dt.Columns.Add(dc);
            }

            dt.Columns.Add("Failure Mode", typeof(string));
            dt.Columns.Add("Remarks", typeof(string));

            return dt;
        }
    }
}
