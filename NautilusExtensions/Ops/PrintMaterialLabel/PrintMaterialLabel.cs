using System;
using System.Data.OracleClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {

    [Guid("67D54D2B-404D-4532-A936-7B85CCB4C343")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _PrintMaterialLabel : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("63455C68-F876-4357-9826-1E5A56E353A1")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Ops.PrintMaterialLabel")]
    public class PrintMaterialLabel : _PrintMaterialLabel {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;
        private string printerPreferenceFile = @"c:\program files\thermo\nautilus\log\extension_printer.txt";
        private string tempFileLocation = @"c:\printlabel.txt";

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            return LSEXT.ExecuteExtension.exEnabled;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            _connection = new OracleConnection(connString);

            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PrintMaterialLabel", "Connection error:\r\n" + ex.Message);
                return;
            }


            //need to check that the selected entity is an aliquot before continuing.
            int entityId = (int)Parameters["ENTITY_ID"];
            int aliquotEntityId = -1;

            string sqlString = "select schema_entity_id from lims_sys.schema_entity where name = 'Aliquot' ";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            try {
                aliquotEntityId = (int)(OracleNumber)command.ExecuteOracleScalar();
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "PrintMaterialLabel", "Error getting aliquot entity id:\r\n" + ex.Message);
                if (_connection != null) {
                    _connection.Close();
                }
            }

            if (entityId != aliquotEntityId) {
                ErrorHandler.LogMessage(_operatorName, "PrintMaterialLabel", "Attempted to execute this extension on wrong entity (" + entityId + ").");
                return;
            }


            //Get the extension printer preference for the current workstation from file.  If not found, prompt to make file.
            if (!File.Exists(printerPreferenceFile)) {
                SelectLabelPrinterForm slpf = new SelectLabelPrinterForm(_connection);
                slpf.ShowDialog();
            }
            string printerDestination = FileHelper.GetFirstLineFileContents(printerPreferenceFile);

            if (string.IsNullOrEmpty(printerDestination)) {
                ErrorHandler.LogError(_operatorName, "PrintMaterialLabel", "No extension printer has been specified, the label will NOT be printed.");
                if (_connection != null) {
                    _connection.Close();
                }
                return;
            }


            //loop through records selected in the Nautilus explorer, print material label for each.
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            while (!records.EOF) {
                PrintLabel(records.Fields[0].Value.ToString(), printerDestination);
                records.MoveNext();
            }

            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PrintMaterialLabel", "Closing connection:\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// Sends the label print job information for specified aliquot to the specified label printer.
        /// </summary>
        /// <param name="aliquotId">The aliquot to print.</param>
        /// <param name="printerDestination">The network address of the label printer destination.</param>
        private void PrintLabel(string aliquotId, string printerDestination) {
            string sqlString = "select mi.name mat, a.name, miu.u_commercial, "
                + "mi.description, bp.u_dot_classification dot, bp.u_reactive_waste_category reactive, "
                + "a.amount, u.name unitname, g.external_reference "
                + "from lims_sys.aliquot a, lims_sys.aliquot_user au, lims_sys.u_material_info_user miu, "
                + "lims_sys.u_material_info mi, lims_sys.u_burn_profile_user bp, "
                + "lims_sys.sample s, lims_sys.sdg g, lims_sys.sample_user su, lims_sys.unit u "
                + "where g.sdg_id = s.sdg_id "
                + "and s.sample_id = su.sample_id "
                + "and s.sample_id = a.sample_id "
                + "and a.aliquot_id = au.aliquot_id "
                + "and su.u_material_info = mi.u_material_info_id "
                + "and mi.u_material_info_id = miu.u_material_info_id "
                + "and miu.u_burn_profile = bp.u_burn_profile_id " 
                + "and a.unit_id = u.unit_id "
                + "and a.aliquot_id = " + aliquotId;

            OracleCommand command = new OracleCommand(sqlString, _connection);
            OracleDataReader reader;
            StringBuilder sb = new StringBuilder();
            try {
                reader = command.ExecuteReader();
                if (reader.HasRows) {
                    reader.Read();

                    //check the datamax printer documentation for info on these strings
                    sb.Append(Convert.ToChar(2) + "L\r\nH10\r\r\nD11\r\r\nPC\r\r\n");
                    sb.Append("1911A3002200050" + reader["mat"] + "\r\r\n");
                    sb.Append("1911A1202050050" + reader["description"] + "\r\r\n");
                    sb.Append("1911A1201800050" + reader["dot"] + " Category " + reader["reactive"] + "\r\r\n");
                    if (reader["u_commercial"].ToString().Equals("T")) sb.Append("1911A1201550050Commercial" + "\r\r\n");
                    sb.Append("1911A1201300050" + reader["external_reference"] + "\r\r\n");
                    sb.Append("1911A1800950050" + reader["amount"] + " " + reader["unitname"] + "\r\r\n");
                    sb.Append("1e0200000500050" + reader["name"] + "\r\r\n");
                    sb.Append("1911A1800150050" + reader["name"] + "\r\r\n");
                    sb.Append("E\r\n");
                }
                reader.Close();

                //need to save file to temp location first, then copy to printer location
                FileHelper.OverwriteOrCreateFile(tempFileLocation, sb.ToString());
                File.Copy(tempFileLocation, printerDestination, true);
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PrintMaterialLabel", "Error getting material information for aliquot " + aliquotId + ":\r\n" + ex.Message);
            }
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
