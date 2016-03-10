using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LSExtensionControlLib;
using System.Text.RegularExpressions;

namespace NautilusExtensions.Qa {
    public partial class S9ConfigForm : UserControl, LSExtensionControlLib.IExtensionControl, LSEXT.IVersion {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        protected LSExtensionControlLib.IExtensionControlSite _site = null;

        public S9ConfigForm() {
            InitializeComponent();
        }

        #region IExtensionControl Members

        void LSExtensionControlLib.IExtensionControl.EnterPage() {
        }

        void LSExtensionControlLib.IExtensionControl.ExitPage() {
        }

        void LSExtensionControlLib.IExtensionControl.Internationalise() {
        }

        void LSExtensionControlLib.IExtensionControl.PreDisplay() {
        }

        void LSExtensionControlLib.IExtensionControl.RestoreSettings(int hKey) {
        }

        void LSExtensionControlLib.IExtensionControl.SaveData() {
            StringBuilder sb;

            //end the editing of any datagridviewcells, if they are in edit mode, getting their value will throw an exception.
            dgvCalculations.EndEdit();
            dgvRawDataFileSaveLocations.EndEdit();
            dgvResultFileSaveLocations.EndEdit();
            dgvResultNames.EndEdit();

            //info group box
            _site.SetStringValue("u_specification", txtSpecification.Text);


            //validation group box
            _site.SetBooleanValue("u_check_spec_speed", chkSpeed.Checked);
            _site.SetBooleanValue("u_check_temperature", chkTemperature.Checked);
            _site.SetBooleanValue("u_check_humidity", chkHumidity.Checked);
            _site.SetBooleanValue("u_check_specimen_count", chkValidSpecimenCount.Checked);
            _site.SetBooleanValue("u_check_tot_specimen_count", chkTotalSpecimenCount.Checked);
            _site.SetBooleanValue("u_check_dimension_1", chkDimension1.Checked);
            _site.SetBooleanValue("u_check_dimension_2", chkDimension2.Checked);
            _site.SetBooleanValue("u_check_dimension_3", chkDimension3.Checked);
            _site.SetBooleanValue("u_check_dimension_4", chkDimension4.Checked);
            _site.SetBooleanValue("u_check_failure_mode", chkFailureModes.Checked);
            SetDoubleValue("u_upper_spec_speed", txtUpperSpeed.Text);
            SetDoubleValue("u_lower_spec_speed", txtLowerSpeed.Text);
            SetDoubleValue("u_upper_temperature", txtUpperTemperature.Text);
            SetDoubleValue("u_lower_temperature", txtLowerTemperature.Text);
            SetDoubleValue("u_upper_humidity", txtUpperHumidity.Text);
            SetDoubleValue("u_lower_humidity", txtLowerHumidity.Text);
            SetDoubleValue("u_upper_specimen_count", txtUpperValidSpecimenCount.Text);
            SetDoubleValue("u_lower_specimen_count", txtLowerValidSpecimenCount.Text);
            SetDoubleValue("u_upper_tot_specimen_count", txtUpperTotalSpecimenCount.Text);
            SetDoubleValue("u_lower_tot_specimen_count", txtLowerTotalSpecimenCount.Text);
            SetDoubleValue("u_lower_dimension_1", txtLowerDimension1.Text);
            SetDoubleValue("u_upper_dimension_1", txtUpperDimension1.Text);
            SetDoubleValue("u_lower_dimension_2", txtLowerDimension2.Text);
            SetDoubleValue("u_upper_dimension_2", txtUpperDimension2.Text);
            SetDoubleValue("u_lower_dimension_3", txtLowerDimension3.Text);
            SetDoubleValue("u_upper_dimension_3", txtUpperDimension3.Text);
            SetDoubleValue("u_lower_dimension_4", txtLowerDimension4.Text);
            SetDoubleValue("u_upper_dimension_4", txtUpperDimension4.Text);
            _site.SetStringValue("u_failure_mode", txtFailureModes.Text);
            _site.SetBooleanValue("u_failure_mode_concise", rbFailureModeConcise.Checked);

            //user defined prompts group box
            _site.SetBooleanValue("u_user_field_2", chkUserField2.Checked);
            _site.SetBooleanValue("u_user_field_3", chkUserField3.Checked);
            _site.SetBooleanValue("u_user_field_4", chkUserField4.Checked);

            sb = new StringBuilder();
            foreach (DataGridViewRow dgvr in dgvResultNames.Rows) {
                //The "*" new row in the datagridview needs to be explicitly ignored.
                if (dgvr.IsNewRow) continue;
                sb.Append((string)dgvr.Cells[0].Value + "\r\n");
            }
            _site.SetStringValue("u_user_field_additional", sb.ToString());


            //file options group box
            _site.SetStringValue("u_result_file_ext", txtResultFileExt.Text);
            sb = new StringBuilder();
            foreach (DataGridViewRow dgvr in dgvResultFileSaveLocations.Rows) {
                if (dgvr.IsNewRow) continue;
                sb.Append((string)dgvr.Cells[0].Value + "\r\n");
            }
            _site.SetStringValue("u_result_file_locations", sb.ToString());

            _site.SetStringValue("u_raw_data_file_ext", txtRawDataFileExt.Text);
            sb = new StringBuilder();
            foreach (DataGridViewRow dgvr in dgvRawDataFileSaveLocations.Rows) {
                if (dgvr.IsNewRow) continue;
                sb.Append((string)dgvr.Cells[0].Value + "\r\n");
            }
            _site.SetStringValue("u_raw_data_file_locations", sb.ToString());

            _site.SetBooleanValue("u_require_raw_data_file", chkRequireRawDataFile.Checked);
            _site.SetBooleanValue("u_use_xml", chkUseXmlProcessor.Checked);
            _site.SetBooleanValue("u_ad_hoc_results", chkAdHocResults.Checked);


            //calculations group box
            _site.SetBooleanValue("u_include_drim", chkIncludeDrim.Checked);
            _site.SetBooleanValue("u_check_actuals", chkActuals.Checked);
            _site.SetBooleanValue("u_check_average", chkAverage.Checked);
            _site.SetBooleanValue("u_check_median", chkMedian.Checked);

            int calcNumber;
            sb = new StringBuilder();

            foreach (DataGridViewRow dgvr in dgvCalculations.Rows) {
                if (dgvr.IsNewRow) continue;

                //if (int.TryParse((string)dgvr.Cells[0].Value, out calcNumber) && calcNumber >= 1 && calcNumber <= 12) {
                if (Regex.IsMatch((string)dgvr.Cells[0].Value, "^([A-Z])|([1-9])$"))
                {
                    sb.Append(dgvr.Cells[0].Value.ToString() + ";");
                    sb.Append((dgvr.Cells[1].Value == null ? string.Empty : dgvr.Cells[1].Value.ToString()).Trim() + ";"); // name
                    sb.Append((dgvr.Cells[2].Value == null ? string.Empty : dgvr.Cells[2].Value.ToString()).Trim() + ";"); // description
                    sb.Append((dgvr.Cells[3].Value ?? false).ToString().ToLower().Equals("true") ? "T;" : "F;");           // check spec
                    sb.Append((dgvr.Cells[4].Value == null ? string.Empty : dgvr.Cells[4].Value.ToString()).Trim() + ";"); // upper spec
                    sb.Append((dgvr.Cells[5].Value == null ? string.Empty : dgvr.Cells[5].Value.ToString()).Trim() + ";"); // lower spec
                    sb.Append((dgvr.Cells[6].Value ?? false).ToString().ToLower().Equals("true") ? "T;" : "F;");           // check stat
                    sb.Append((dgvr.Cells[7].Value == null ? string.Empty : dgvr.Cells[7].Value.ToString()).Trim() + ";"); // upper stat
                    sb.Append((dgvr.Cells[8].Value == null ? string.Empty : dgvr.Cells[8].Value.ToString()).Trim() + ";\r\n"); // lower stat
                } else {
                    MessageBox.Show(string.Format("Bad calculation number '{0}'.  Must be 1-9 (for ascii) or A-Z (for DRIM).  Ignoring row.", 
                        dgvr.Cells[0] == null ? string.Empty : dgvr.Cells[0].Value.ToString()));
                }
            }

            _site.SetStringValue("u_calculations", sb.ToString());
        }


        void LSExtensionControlLib.IExtensionControl.SaveSettings(int hKey) {
        }

        void LSExtensionControlLib.IExtensionControl.SetReadOnly(bool readOnly) {
            readOnly = false;
        }

        void LSExtensionControlLib.IExtensionControl.SetServiceProvider(object serviceProvider) {
        }

        void LSExtensionControlLib.IExtensionControl.SetSite(object site) {
            if (site != null) {
                this._site = site as IExtensionControlSite;
            }

        }

        void LSExtensionControlLib.IExtensionControl.SetupData() {
            bool nullFlag;
            string name, spec, desc;
            bool speed, temp, humidity, validSpecimenCount, totalSpecimenCount, dimension1, dimension2, dimension3, dimension4, failureMode, failureModeConcise, userField2, userField3, userField4, userField5,
                requireRawDataFile, useXml, adHocResuts;
            double lowerSpeed, upperSpeed, lowerTemp, upperTemp, lowerHumidity, upperHumidity,
                lowerValidSpecimenCount, upperValidSpecimenCount, lowerTotalSpecimenCount, upperTotalSpecimenCount, lowerDimension1, upperDimension1, 
                lowerDimension2, upperDimension2, lowerDimension3, upperDimension3, lowerDimension4, upperDimension4;
            bool isnullLowerSpeed, isnullUpperSpeed, isnullLowerTemp, isnullUpperTemp, isnullLowerHumidity, isnullUpperHumidity,
                isnullLowerValidSpecimenCount, isnullUpperValidSpecimenCount, isnullLowerTotalSpecimenCount, isnullUpperTotalSpecimenCount,
                isnullLowerDimension1, isnullUpperDimension1, isnullLowerDimension2, isnullUpperDimension2,
                isnullLowerDimension3, isnullUpperDimension3, isnullLowerDimension4, isnullUpperDimension4;
            string userFieldsAdditional, resultFileExtension, resultFileLocations, rawDataFileExtension, rawDataFileLocations, allowedFailureModes;
            string calculations;
            bool includeDrim, checkActuals, checkAverage, checkMedian;

            //get the database values into variables
            //info group box
            _site.GetStringValue("name", out name, out nullFlag);
            _site.GetStringValue("u_specification", out spec, out nullFlag);
            _site.GetStringValue("description", out desc, out nullFlag);

            //validation group box
            _site.GetBooleanValue("u_check_spec_speed", out speed, out nullFlag);
            _site.GetBooleanValue("u_check_temperature", out temp, out nullFlag);
            _site.GetBooleanValue("u_check_humidity", out humidity, out nullFlag);
            _site.GetBooleanValue("u_check_specimen_count", out validSpecimenCount, out nullFlag);
            _site.GetBooleanValue("u_check_tot_specimen_count", out totalSpecimenCount, out nullFlag);
            _site.GetBooleanValue("u_check_dimension_1", out dimension1, out nullFlag);
            _site.GetBooleanValue("u_check_dimension_2", out dimension2, out nullFlag);
            _site.GetBooleanValue("u_check_dimension_3", out dimension3, out nullFlag);
            _site.GetBooleanValue("u_check_dimension_4", out dimension4, out nullFlag);
            _site.GetBooleanValue("u_check_failure_mode", out failureMode, out nullFlag);
            _site.GetDoubleValue("u_upper_spec_speed", out upperSpeed, out isnullUpperSpeed);
            _site.GetDoubleValue("u_lower_spec_speed", out lowerSpeed, out isnullLowerSpeed);
            _site.GetDoubleValue("u_upper_temperature", out upperTemp, out isnullUpperTemp);
            _site.GetDoubleValue("u_lower_temperature", out lowerTemp, out isnullLowerTemp);
            _site.GetDoubleValue("u_upper_humidity", out upperHumidity, out isnullUpperHumidity);
            _site.GetDoubleValue("u_lower_humidity", out lowerHumidity, out isnullLowerHumidity);
            _site.GetDoubleValue("u_upper_specimen_count", out upperValidSpecimenCount, out isnullUpperValidSpecimenCount);
            _site.GetDoubleValue("u_lower_specimen_count", out lowerValidSpecimenCount, out isnullLowerValidSpecimenCount);
            _site.GetDoubleValue("u_upper_tot_specimen_count", out upperTotalSpecimenCount, out isnullUpperTotalSpecimenCount);
            _site.GetDoubleValue("u_lower_tot_specimen_count", out lowerTotalSpecimenCount, out isnullLowerTotalSpecimenCount);
            _site.GetDoubleValue("u_upper_dimension_1", out upperDimension1, out isnullUpperDimension1);
            _site.GetDoubleValue("u_lower_dimension_1", out lowerDimension1, out isnullLowerDimension1);
            _site.GetDoubleValue("u_upper_dimension_2", out upperDimension2, out isnullUpperDimension2);
            _site.GetDoubleValue("u_lower_dimension_2", out lowerDimension2, out isnullLowerDimension2);
            _site.GetDoubleValue("u_upper_dimension_3", out upperDimension3, out isnullUpperDimension3);
            _site.GetDoubleValue("u_lower_dimension_3", out lowerDimension3, out isnullLowerDimension3);
            _site.GetDoubleValue("u_upper_dimension_4", out upperDimension4, out isnullUpperDimension4);
            _site.GetDoubleValue("u_lower_dimension_4", out lowerDimension4, out isnullLowerDimension4);
            _site.GetStringValue("u_failure_mode", out allowedFailureModes, out nullFlag);
            _site.GetBooleanValue("u_failure_mode_concise", out failureModeConcise, out nullFlag);

            //user defined prompts group box
            _site.GetBooleanValue("u_user_field_2", out userField2, out nullFlag);
            _site.GetBooleanValue("u_user_field_3", out userField3, out nullFlag);
            _site.GetBooleanValue("u_user_field_4", out userField4, out nullFlag);
            _site.GetStringValue("u_user_field_additional", out userFieldsAdditional, out nullFlag);

            //file options group box
            _site.GetStringValue("u_result_file_ext", out resultFileExtension, out nullFlag);
            _site.GetStringValue("u_result_file_locations", out resultFileLocations, out nullFlag);
            _site.GetStringValue("u_raw_data_file_ext", out rawDataFileExtension, out nullFlag);
            _site.GetStringValue("u_raw_data_file_locations", out rawDataFileLocations, out nullFlag);
            _site.GetBooleanValue("u_require_raw_data_file", out requireRawDataFile, out nullFlag);
            _site.GetBooleanValue("u_use_xml", out useXml, out nullFlag);
            _site.GetBooleanValue("u_ad_hoc_results", out adHocResuts, out nullFlag);


            //calculation string
            _site.GetBooleanValue("u_include_drim", out includeDrim, out nullFlag);
            _site.GetBooleanValue("u_check_actuals", out checkActuals, out nullFlag);
            _site.GetBooleanValue("u_check_average", out checkAverage, out nullFlag);
            _site.GetBooleanValue("u_check_median", out checkMedian, out nullFlag);
            _site.GetStringValue("u_calculations", out calculations, out nullFlag);
            

            //populate info group box
            txtName.Text = name;
            txtSpecification.Text = spec;
            txtDescription.Text = desc;

            //populate validation group box
            chkSpeed.Checked = speed;
            chkTemperature.Checked = temp;
            chkHumidity.Checked = humidity;
            chkValidSpecimenCount.Checked = validSpecimenCount;
            chkTotalSpecimenCount.Checked = totalSpecimenCount;
            chkDimension1.Checked = dimension1;
            chkDimension2.Checked = dimension2;
            chkDimension3.Checked = dimension3;
            chkDimension4.Checked = dimension4;
            chkFailureModes.Checked = failureMode;
            txtUpperSpeed.Text = isnullUpperSpeed ? string.Empty : upperSpeed.ToString("0.0000");
            txtLowerSpeed.Text = isnullLowerSpeed ? string.Empty : lowerSpeed.ToString("0.0000");
            txtUpperTemperature.Text = isnullUpperTemp ? string.Empty : upperTemp.ToString("0.0");
            txtLowerTemperature.Text = isnullLowerTemp ? string.Empty : lowerTemp.ToString("0.0");
            txtUpperHumidity.Text = isnullUpperHumidity ? string.Empty : upperHumidity.ToString("0.0");
            txtLowerHumidity.Text = isnullLowerHumidity ? string.Empty : lowerHumidity.ToString("0.0");
            txtUpperValidSpecimenCount.Text = isnullUpperValidSpecimenCount ? string.Empty : upperValidSpecimenCount.ToString("0");
            txtLowerValidSpecimenCount.Text = isnullLowerValidSpecimenCount ? string.Empty : lowerValidSpecimenCount.ToString("0");
            txtUpperTotalSpecimenCount.Text = isnullUpperTotalSpecimenCount ? string.Empty : upperTotalSpecimenCount.ToString("0");
            txtLowerTotalSpecimenCount.Text = isnullLowerTotalSpecimenCount ? string.Empty : lowerTotalSpecimenCount.ToString("0");
            txtUpperDimension1.Text = isnullUpperDimension1 ? string.Empty : upperDimension1.ToString("0.0000");
            txtLowerDimension1.Text = isnullLowerDimension1 ? string.Empty : lowerDimension1.ToString("0.0000");
            txtUpperDimension2.Text = isnullUpperDimension2 ? string.Empty : upperDimension2.ToString("0.0000");
            txtLowerDimension2.Text = isnullLowerDimension2 ? string.Empty : lowerDimension2.ToString("0.0000");
            txtUpperDimension3.Text = isnullUpperDimension3 ? string.Empty : upperDimension3.ToString("0.0000");
            txtLowerDimension3.Text = isnullLowerDimension3 ? string.Empty : lowerDimension3.ToString("0.0000");
            txtUpperDimension4.Text = isnullUpperDimension4 ? string.Empty : upperDimension4.ToString("0.0000");
            txtLowerDimension4.Text = isnullLowerDimension4 ? string.Empty : lowerDimension4.ToString("0.0000");
            txtFailureModes.Text = allowedFailureModes;
            if (failureModeConcise) {
                rbFailureModeConcise.Checked = true;
            } else {
                rbFailureModeVerbose.Checked = true;
            }

            //populate user defined prompts group box
            chkUserField2.Checked = userField2;
            chkUserField3.Checked = userField3;
            chkUserField4.Checked = userField4;
            PopulateDgvRowsFromString(dgvResultNames, userFieldsAdditional);

            //populate file options group box
            txtResultFileExt.Text = resultFileExtension;
            PopulateDgvRowsFromString(dgvResultFileSaveLocations, resultFileLocations);
            txtRawDataFileExt.Text = rawDataFileExtension;
            PopulateDgvRowsFromString(dgvRawDataFileSaveLocations, rawDataFileLocations);
            chkRequireRawDataFile.Checked = requireRawDataFile;
            chkUseXmlProcessor.Checked = useXml;
            chkAdHocResults.Enabled = useXml;
            chkAdHocResults.Checked = adHocResuts;

            //calculations group box
            chkIncludeDrim.Checked = includeDrim;
            chkActuals.Checked = checkActuals;
            chkAverage.Checked = checkAverage;
            chkMedian.Checked = checkMedian;
            PopulateCalculationDgv(calculations);
        }

        #endregion


        /// <summary>
        /// Parses a calculation string passed in, populates calculation datagridview with row for each line containing required 9 ; chars
        /// </summary>
        /// <param name="inputString">String to be parsed, contains semicolon-separated values</param>
        /// <returns>DataGridViewRow with 9 cells</returns>
        private void PopulateCalculationDgv(string inputString) {
            object[] gridArray = { "0", string.Empty, string.Empty, false, null, null, false, null, null };
            string[] linesArray = inputString.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] parsedLineArray;

            foreach (string line in linesArray) {

                parsedLineArray = line.Split(";".ToCharArray());

                if (parsedLineArray.Length == 10) {    // this value should be 10 because the string ends in a ; char (last element is empty string)
                    gridArray[0] = parsedLineArray[0];
                    gridArray[1] = parsedLineArray[1];
                    gridArray[2] = parsedLineArray[2];
                    gridArray[3] = parsedLineArray[3].Equals("T");
                    gridArray[4] = parsedLineArray[4];
                    gridArray[5] = parsedLineArray[5];
                    gridArray[6] = parsedLineArray[6].Equals("T");
                    gridArray[7] = parsedLineArray[7];
                    gridArray[8] = parsedLineArray[8];

                    dgvCalculations.Rows.Add(gridArray);
                }
            }
        }


        /// <summary>
        /// Populates a datagridview with rows that correspond to values in a string, separated by semicolons.  The DGV must have one textbox column only.
        /// </summary>
        /// <param name="dgv">The DataGridView object to be populated.</param>
        /// <param name="inputString">Semi-colon list of values to be parsed.</param>
        private void PopulateDgvRowsFromString(DataGridView dgv, string inputString) {

            if (string.IsNullOrEmpty(inputString)) return;

            foreach (string s in inputString.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)) {
                dgv.Rows.Add(new object[] { s });
            }
        }

        private void SetDoubleValue(string columnName, string value) {

            if (string.IsNullOrEmpty(value.Trim())) {
                _site.SetNullValue(columnName);
                return;
            } else {
                double parsedNumericValue;
                if (double.TryParse(value, out parsedNumericValue)) {
                    _site.SetDoubleValue(columnName, parsedNumericValue);
                } else {
                    MessageBox.Show(string.Format("Could not set column {0} to non-numeric value '{1}'.\r\nSetting to null.", columnName, value));
                    _site.SetNullValue(columnName);
                }
            }
        }

        private void dgv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e) {
            _site.SetModifiedFlag();
        }

        private void textbox_TextChanged(object sender, EventArgs e) {
            _site.SetModifiedFlag();
        }

        private void checkbox_CheckedChanged(object sender, EventArgs e) {
            _site.SetModifiedFlag();

            if (!(sender is CheckBox)) return;

            CheckBox chkSender = (CheckBox)sender;
            if (chkSender.Tag != null) {
                foreach (Control c in chkSender.Parent.Controls) {
                    if (c != chkSender && c.Tag != null && c.Tag.ToString().Equals(chkSender.Tag.ToString() + "Dependent")) {
                        c.Enabled = chkSender.Checked;
                    }
                }
            }
        }

        private void dgvCalculations_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
            _site.SetModifiedFlag();
        }

        private void chkUseXmlProcessor_CheckedChanged(object sender, EventArgs e) {
            _site.SetModifiedFlag();
            chkAdHocResults.Enabled = chkUseXmlProcessor.Checked;
        }


        public int GetVersion() {
            return VERSION;
        }
    }
}