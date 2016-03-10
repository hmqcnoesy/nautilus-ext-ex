using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace NautilusExtensions.Qa {
    public class S9Config {

        // information
        public string Name { get; set; }
        public string Specification { get; set; }
        public string Description { get; set; }

        // file options
        public string ResultFileExtension { get; set; }
        public List<string> ResultFileSaveLocations { get; set; }
        public string RawFileExtension { get; set; }
        public List<string> RawFileSaveLocations { get; set; }
        public bool RequireRawDataFile { get; set; }
        public bool UseXmlProcessor { get; set; }
        public bool CreateAdHocResults { get; set; }

        // validation
        public bool CheckCrossheadSpeed { get; set; }
        public decimal? CrossheadSpeedUpper { get; set; }
        public decimal? CrossheadSpeedLower { get; set; }
        public bool CheckTemperature { get; set; }
        public decimal? TemperatureUpper { get; set; }
        public decimal? TemperatureLower { get; set; }
        public bool CheckHumidity { get; set; }
        public decimal? HumidityUpper { get; set; }
        public decimal? HumidityLower { get; set; }
        public bool CheckValidSpecimenCount { get; set; }
        public int? ValidSpecimenUpper { get; set; }
        public int? ValidSpecimenLower { get; set; }
        public bool CheckTotalSpecimenCount { get; set; }
        public int? TotalSpecimenUpper { get; set; }
        public int? TotalSpecimenLower { get; set; }
        public bool CheckDimension1 { get; set; }
        public decimal? Dimension1Lower { get; set; }
        public decimal? Dimension1Upper { get; set; }
        public bool CheckDimension2 { get; set; }
        public decimal? Dimension2Lower { get; set; }
        public decimal? Dimension2Upper { get; set; }
        public bool CheckDimension3 { get; set; }
        public decimal? Dimension3Lower { get; set; }
        public decimal? Dimension3Upper { get; set; }
        public bool CheckDimension4 { get; set; }
        public decimal? Dimension4Lower { get; set; }
        public decimal? Dimension4Upper { get; set; }
        public bool CheckFailureModes { get; set; }
        public string FailureModes { get; set; }
        public bool FailureModesConciseFormat { get; set; }

        // user defined prompts
        public bool UserField2 { get; set; }
        public bool UserField3 { get; set; }
        public bool UserField4 { get; set; }
        public List<string> UserFieldsAdditional { get; set; }

        // calculations
        public bool IncludeDrim { get; set; }
        public bool CheckActuals { get; set; }
        public bool CheckAverage { get; set; }
        public bool CheckMedian { get; set; }
        public Dictionary<string, S9Calculation> Calculations { get; set; }

        public S9Config(string resultFileSaveLocations, string rawFileSaveLocations, string userFieldsAdditional, string calculations) {

            ResultFileSaveLocations = new List<string>();
            ResultFileSaveLocations.AddRange(resultFileSaveLocations.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            RawFileSaveLocations = new List<string>();
            RawFileSaveLocations.AddRange(rawFileSaveLocations.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            UserFieldsAdditional = new List<string>();
            UserFieldsAdditional.AddRange(userFieldsAdditional.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            Calculations = new Dictionary<string, S9Calculation>();
            S9Calculation calc;
            string[] calcStrings = calculations.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] parsedValues;

            try {
                for (int i = 0; i < calcStrings.Length; i++) {
                    if (!string.IsNullOrEmpty(calcStrings[i])) {
                        parsedValues = calcStrings[i].Split(";".ToCharArray());

                        if (parsedValues.Length != 10) continue;

                        calc = new S9Calculation() {
                            ColumnId = parsedValues[0],
                            NautilusTestName = parsedValues[1],
                            NautilusTestDescription = parsedValues[2],
                            CheckSpecLimits = parsedValues[3].Equals("T"),
                            SpecLimitUpper = string.IsNullOrEmpty(parsedValues[4]) ? null : (decimal?)decimal.Parse(parsedValues[4]),
                            SpecLimitLower = string.IsNullOrEmpty(parsedValues[5]) ? null : (decimal?)decimal.Parse(parsedValues[5]),
                            CheckStatLimits = parsedValues[6].Equals("T"),
                            StatLimitUpper = string.IsNullOrEmpty(parsedValues[7]) ? null : (decimal?)decimal.Parse(parsedValues[7]),
                            StatLimitLower = string.IsNullOrEmpty(parsedValues[8]) ? null : (decimal?)decimal.Parse(parsedValues[8])
                        };
                        Calculations.Add(calc.ColumnId, calc);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public override string ToString() {
            return Name;
        }

        public DataTable GetUserFieldsTable(string u2Key, string u2Val, string u3Key, string u3Val, string u4Key, string u4Val) {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Item", typeof(string)) { ReadOnly = true });
            dt.Columns.Add(new DataColumn("ID", typeof(string)) { ReadOnly = false });
            DataRow dr;

            if (UserField2) {
                dr = dt.NewRow();
                dr[0] = u2Key;
                dr[1] = u2Val;
                if (!string.IsNullOrEmpty((string)dr[0])) dt.Rows.Add(dr);
            }

            if (UserField3) {
                dr = dt.NewRow();
                dr[0] = u3Key;
                dr[1] = u3Val;
                if (!string.IsNullOrEmpty((string)dr[0])) dt.Rows.Add(dr);
            }

            if (UserField4) {
                dr = dt.NewRow();
                dr[0] = u4Key;
                dr[1] = u4Val;
                if (!string.IsNullOrEmpty((string)dr[0])) dt.Rows.Add(dr);
            }

            foreach (string s in UserFieldsAdditional) {
                dr = dt.NewRow();
                dr[0] = s;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }

    public class S9Calculation {
        public string ColumnId { get; set; }
        public string DataTableColumnName { get { return "(" + ColumnId + ") " + NautilusTestName; } }
        public string NautilusTestName { get; set; }
        public string NautilusTestDescription { get; set; }
        public bool CheckSpecLimits { get; set; }
        public decimal? SpecLimitUpper { get; set; }
        public decimal? SpecLimitLower { get; set; }
        public bool CheckStatLimits { get; set; }
        public decimal? StatLimitUpper { get; set; }
        public decimal? StatLimitLower { get; set; }
    }
}
