using System;
using System.Collections.Generic;
using System.IO;

namespace NautilusExtensions.Qa {
    class VendorDataFile {

        private string _fileName;

        private Sample _currentSample;
        private List<Sample> _samples;
        public List<Sample> Samples { get { return _samples; } set { _samples = value; } }

        public string PartNumber { get; set; }
        public string SerialNumber { get; set; }

        public bool HasErrors { get { return !string.IsNullOrEmpty(_errors); } }

        private string _errors;
        public string Errors { get { return _errors; } }

        public VendorDataFile(string file) {
            _errors = string.Empty;
            _fileName = file.Substring(file.LastIndexOf('\\') + 1);
            _samples = new List<Sample>();

            string line;

            using (StreamReader sr = new StreamReader(file)) {
                line = sr.ReadLine();
                while (line != null) {
                    if (!string.IsNullOrEmpty(line.Trim())) ParseLineIntoObjectHierarchy(line);
                    line = sr.ReadLine();
                }
            }
        }

        private void ParseLineIntoObjectHierarchy(string line) {
            string[] lineData = line.Split(",".ToCharArray());

            //there must be at least 4 columns.  The third column (an int) will help with total column count.
            if (lineData.Length < 4) {
                throw new ProcessXmlException("Line in selected file is missing required columns:\r\n" + line);
            }

            //Get the part and serial numbers from the first column
            lineData[0] = lineData[0].Replace("\"", string.Empty);
            string[] sampleInfo = lineData[0].Split('/');
            string partNumber = string.Empty, serialNumber = string.Empty;
            if (sampleInfo.Length == 2) {
                partNumber = sampleInfo[0];
                serialNumber = sampleInfo[1];
            } else {
                //try a - separating character
                sampleInfo = lineData[0].Split('-');
                int sampleInfoLength = sampleInfo.Length;
                if (sampleInfoLength >= 2) {
                    serialNumber = sampleInfo[sampleInfoLength - 1];
                    for (int i = 0; i < sampleInfoLength - 1; i++) {
                        partNumber += sampleInfo[i] + "-";
                    }

                    partNumber = partNumber.Trim("-".ToCharArray());
                } else {
                    //give up
                    partNumber = lineData[0];
                }
            }

            //create a sample if new part/serial number, or if the sample hasn't been created yet
            if (_currentSample == null || !_currentSample.PartNumber.Equals(partNumber) || !_currentSample.SerialNumber.Equals(serialNumber)) {
                _currentSample = new Sample() {
                    PartNumber = partNumber,
                    SerialNumber = serialNumber,
                    Description = "ELECTRONIC VENDOR DATA",
                    ReceiverNumber = _fileName
                };
                _currentSample.Aliquots.Add(new Aliquot());
                _samples.Add(_currentSample);
            }

            string actualCountString = lineData[2].Trim("\"".ToCharArray());
            int actualCount;

            if (!int.TryParse(actualCountString, out actualCount)) {
                throw new ProcessXmlException("Line does not have integer in third column:\r\n" + line);
            }

            //Parse out the result names from the fourth column
            string[] resultNames = lineData[3].Trim("\"".ToCharArray()).Split(":".ToCharArray());

            //The total number of columns needs to be 6 plus the actual columns count, plus the summary result columns count
            if (lineData.Length != (6 + actualCount + resultNames.Length)) {
                throw new ProcessXmlException(
                    string.Format("Line does not have correct number of columns ({0}) for specified actual count ({1}) and summary count ({2}):\r\n{3}",
                    (6 + actualCount + resultNames.Length).ToString(),
                    actualCount.ToString(),
                    resultNames.Length.ToString(),
                    line));
            }

            //Matrix ID must be in correct format - asterisk + 7 digits
            if (!System.Text.RegularExpressions.Regex.IsMatch(lineData[1].Trim("\"".ToCharArray()), @"^\*[0-9]{7}$")) {
                _errors += string.Format("Incorrect matrix ID format:  {0}\r\n", line);
                return;
            }


            //Create the test object for this row
            Test t = null;

            //Put actual results into object hierarchy
            Result r;
            int actualNumber;
            for (int i = 0; i < actualCount; i++) {
                //if more than 88 results, restart count with each multiple of 88, creating new test
                //this is because ADCAR is set up to handle actuals up through column_id 89 only ('Actual 1' is column 2, etc.)
                actualNumber = (i % 88) + 1;
                if (actualNumber == 1) {
                    if (t != null) _currentSample.Aliquots[0].Tests.Add(t);
                    t = new Test();
                    t.MatrixId = lineData[1].Trim("\"".ToCharArray());
                    t.IpItemNumber = "0100"; //hard-coded info that is required for ADCAR insert.

                    //add the col 01 result (the row ID)
                    t.RowId = lineData[4].Replace("\"", "") + lineData[5].Replace("\"", "");
                    if (string.IsNullOrEmpty(t.RowId)) t.RowId = "NULL";
                }

                r = new Result() {
                    Name = "Actual " + actualNumber,
                    ResultValue = lineData[6 + i]
                };
                t.Results.Add(r);
            }

            //Put summary results into table
            for (int i = 0; i < resultNames.Length; i++) {
                r = new Result();
                switch (resultNames[i].ToUpper()) {
                    case "AVE":
                        r.Name = "Average";
                        break;
                    case "MIN":
                        r.Name = "Minimum";
                        break;
                    case "MAX":
                        r.Name = "Maximum";
                        break;
                    case "MED":
                        r.Name = "Median";
                        break;
                    default:
                        throw new ProcessXmlException(string.Format("Line contains unknown summary result name ({0}):\r\n{1}",
                            resultNames[i].ToUpper(),
                            line));
                }
                r.ResultValue = lineData[6 + actualCount + i];

                t.Results.Add(r);
            }

            _currentSample.Aliquots[0].Tests.Add(t);

        }
    }
}

