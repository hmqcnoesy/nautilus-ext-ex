using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.OracleClient;
using System.Data.Odbc;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("3C3E9E97-8C08-4533-9396-279A61217DB6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _AelsbrResults : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("1586FC46-7372-402B-9EBE-4395D11040AC")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.AelsbrResults")]
    public class AelsbrResults : _AelsbrResults {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connectionNautilus;
        private OdbcConnection _connectionAelsbr;
        private decimal _strawLengthInInches;
        private string _testName;


        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            if (Parameters["PARAMETER"] == null || string.IsNullOrEmpty(Parameters["PARAMETER"].ToString())) {
                ErrorHandler.LogError(_operatorName, "AelsebrResults", "No parameter was passed to the extension.");
                return;
            } else {
                var parts = Parameters["PARAMETER"].ToString().Split("|".ToCharArray());
                if (parts.Length != 2) {
                    ErrorHandler.LogError(_operatorName, "AelsbrResults", "Parameter does not have correct format.  Use L|T format where L is numeric length of strand in inches, and T is Nautilus test name.");
                    return;
                }

                try {
                    _strawLengthInInches = decimal.Parse(parts[0]);
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "AelsbrResults", "Error converting parameter into number:\r\n" + ex.Message);
                    return;
                }

                _testName = parts[1].Trim();
            }


            _connectionNautilus = Common.GetOracleConnection(Parameters);

            //Get the location of the virtual instrument parsing directory.
            string fileParsingLocation = GetFileParsingPath();

            //Prompting user, get list of aliquots for which the results should be retrieved.
            AelsbrResultsForm lrf = new AelsbrResultsForm(_connectionNautilus, _operatorName, _testName);
            if (lrf.ShowDialog() == DialogResult.Cancel) {
                try {
                    _connectionNautilus.Close();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "AelsbrResults", "Error closing Nautilus database connection:\r\n" + ex.Message);
                }
                return;
            }

            //get a connection to the LSBR .mdb database
            using (_connectionAelsbr = new OdbcConnection(@"DSN=LSBR")) {

                try {
                    _connectionAelsbr.Open();
                } catch (OdbcException ex) {
                    if (ex.Message.Contains("source name not found")) {
                        ErrorHandler.LogError(_operatorName, "AelsbrResults", "To use this extension, you must have an ODBC DSN to the LSBR database (m585.mdb).\r\n" +
                            "Please contact the Nautilus admin for assistance with this setup.");
                        return;
                    }
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "AelsbrResults", "Error connecting to LSBR Access database:\r\n" + ex.Message);
                    return;
                }

                if (lrf.SelectedAliquots != null) {
                    //loop through selected aliquots, creating a parsing file for each one.
                    string mixNumber, aliquotName;
                    List<AelsbrResult> results;
                    for (int i = 0; i < lrf.SelectedAliquots.GetLength(0); i++) {
                        aliquotName = lrf.SelectedAliquots[i, 0];
                        mixNumber = lrf.SelectedAliquots[i, 1];

                        //should warn if selected aliquot doesn't have a mix number in Nautilus db
                        if (string.IsNullOrEmpty(mixNumber)) {
                            ErrorHandler.LogError(_operatorName, "AelsbrResults", "Aliquot " + aliquotName + " has no mix number specified in Nautilus.");
                        } else {
                            results = GetResults(mixNumber);
                            results.Add(GetResultsSlNumbers(mixNumber));

                            WriteResultFile(aliquotName, results, fileParsingLocation, (int)Parameters["OPERATOR_ID"]);
                        }
                    }
                }

                lrf.Dispose();
            }
            
            try {
                _connectionNautilus.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "AelsbrResults", "Error closing Nautilus database connection:\r\n" + ex.Message);
            }
        }


        /// <summary>
        /// Gets a list of results from LSBR access database for specified mix number
        /// </summary>
        /// <param name="mixNumber"></param>
        /// <returns></returns>
        private List<AelsbrResult> GetResults(string mixNumber) {
            List<AelsbrResult> results = new List<AelsbrResult>();
            AelsbrResult result;

            string sqlString = string.Format("select Burn.Id, {0}/(LsbrStat.StopTime-LsbrStat.StartTime), Burn.MinPressure, Burn.BathTemp "
                + "from Mix inner join (Burn inner join LsbrStat on Burn.ID = LsbrStat.BurnID) on Mix.ID = Burn.MixID "
                + "where Mix.MixNumber = ? "
                + "and LsbrStat.Valid = true "
                + "order by 1 ", _strawLengthInInches);

            OdbcCommand command = new OdbcCommand(sqlString, _connectionAelsbr);
            command.Parameters.AddWithValue("MixNumber", mixNumber);
            OdbcDataReader reader;

            try {
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    result = new AelsbrResult() {
                        ResultName = "Actual " + (results.Count + 1),
                        ResultValue = reader[1].ToString(),
                        Pressure = reader[2].ToString(),
                        Temperature = reader[3].ToString()
                    };

                    results.Add(result);
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError("Error retreiving LSBR data for mix " + mixNumber + ":\r\n" + ex.Message);
            }

            return results;
        }


        /// <summary>
        /// Gets a comma separated list of tooling sl numbers used in LSBR for specified mix number
        /// </summary>
        /// <param name="mixNumber"></param>
        /// <returns></returns>
        private AelsbrResult GetResultsSlNumbers(string mixNumber) {
            string slNumbers = string.Empty;
            AelsbrResult result = new AelsbrResult() { ResultName = "SL Numbers" };

            string sqlString = "select distinct SASL.Number "
                + "from SASL, CurrentSLs, Burn, Mix "
                + "where Mix.ID = Burn.MixID "
                + "and Burn.CurrentSLsID = CurrentSLs.ID "
                + "and (CurrentSLs.Temperature1 = SASL.ID "
                + "or CurrentSLs.Temperature2 = SASL.ID "
                + "or CurrentSLs.Pressure1 = SASL.ID "
                + "or CurrentSLs.Pressure2 = SASL.ID "
                + "or CurrentSLs.AcousticTemplate = SASL.ID "
                + "or CurrentSLs.NitrogenTemplate = SASL.ID) "
                + "and Mix.MixNumber = ? "
                + "and SASL.Number IS NOT NULL "
                + "and SASL.Number <> ' ' ";

            OdbcCommand command = new OdbcCommand(sqlString, _connectionAelsbr);
            command.Parameters.AddWithValue("MixNumber", mixNumber);
            OdbcDataReader reader;

            try {
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    slNumbers += reader[0].ToString() + ", ";
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError("Error retreiving SL Numbers data for mix " + mixNumber + ":\r\n" + ex.Message);
            }

            //take off the last comma and space before returning.
            result.ResultValue = slNumbers.Trim(new char[]{',', ' '});
            return result;
        }


        /// <summary>
        /// Writes the file with results to be parsed, in format apropos to the virtual instrument's scripts.
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="results"></param>
        private void WriteResultFile(string aliquotName, List<AelsbrResult> results, string fileLocation, int operatorId) {
            //first get result values into an array for calculating a mean and median
            List<double> resultValues = new List<double>();
            double d, mean = 0, median;
            int count = 0;
            foreach (AelsbrResult r in results) {
                if (double.TryParse(r.ResultValue, out d)) {
                    resultValues.Add(d);
                    mean += d;
                    count++;
                }
            }

            if (count > 0) {
                mean /= (double)count;
                resultValues.Sort();
                median = count % 2 == 0 ? (resultValues[(count / 2) - 1] + resultValues[count / 2]) / 2.0 : resultValues[count / 2];
            } else {
                ErrorHandler.LogError(_operatorName, "AelsbrResults", "No numeric results found in LSBR database for aliquot " + aliquotName);
                return;
            }
            
            //construct the text of the file
            StringBuilder sb = new StringBuilder(string.Format("Test Name,{0}", _testName) + Environment.NewLine);
            sb.Append("Operator," + operatorId + "," + Environment.NewLine);
            
            foreach (AelsbrResult r in results) {
                sb.Append(aliquotName + ",");
                sb.Append(r.ResultName + ",");
                sb.Append(r.Pressure + ",");
                sb.Append(r.Temperature + ",");
                sb.AppendLine(r.ResultValue);
            }

            //append the mean and median
            sb.AppendLine(aliquotName + ",Median,,," + median);
            sb.AppendLine(aliquotName + ",Average,,," + mean);

            //write the file
            try {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(fileLocation, aliquotName + " (AELSBR).csv"));
                sw.Write(sb.ToString());
                sw.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "AelsbrResults", "Error writing parsing file:\r\n" + ex.Message);
            }
        }


        /// <summary>
        /// Returns a string representation of a path to the parsing location of the Nautilus "Virtual Instrument"
        /// </summary>
        /// <returns></returns>
        string GetFileParsingPath() {
            string path = string.Empty;
            string sqlString = "select ic.input_file_directory "
                + "from lims_sys.instrument_control ic, lims_sys.instrument i "
                + "where ic.instrument_control_id = i.instrument_control_id "
                + "and i.name = :in_test_name ";

            OracleCommand command = new OracleCommand(sqlString, _connectionNautilus);
            command.Parameters.Add(new OracleParameter(":in_test_name", _testName));

            try {
                path = command.ExecuteScalar().ToString();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "AelsbrResults", "Error in determining the parsing directory location:\r\n" + ex.Message);
            }

            return path;
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }

    public class AelsbrResult {
        public string ResultName { get; set; }
        public string ResultValue { get; set; }
        public string Pressure { get; set; }
        public string Temperature { get; set; }
    }
}
