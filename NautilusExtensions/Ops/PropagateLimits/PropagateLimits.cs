using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {

    [Guid("97C257C0-D522-4078-A19A-212359417FBC")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _PropagateLimits : LSEXT.IWorkflowExtension, LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("62188DCE-3571-4AFD-B49F-EF88A8B55065")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Ops.PropagateLimits")]
    public class PropagateLimits : _PropagateLimits {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;

        #region IVersion Members

        int LSEXT.IVersion.GetVersion() {
            return VERSION;
        }

        #endregion

        #region IWorkflowExtension Members

        void LSEXT.IWorkflowExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            //open connections
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            _connection = new OracleConnection(connString);

            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PropagateLimits", "Connection error:\r\n" + ex.Message);
                return;
            }

            //propagate the results
            PropagateResultLimits(long.Parse(Parameters["PRIMARY_KEY"].ToString()), false);

            //close connections
            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PropagateLimits", "Closing connections:\r\n" + ex.Message);
            }
        }

        #endregion

        #region IEntityExtension Members

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            return LSEXT.ExecuteExtension.exEnabled;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();

            //open connections
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            _connection = new OracleConnection(connString);

            try {
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PropagateLimits", "Connection error:\r\n" + ex.Message);
                return;
            }

            //need to verify that sample entity is selected.            
            int entityId = (int)Parameters["ENTITY_ID"];
            int sampleEntityId = -1;
            string sqlString = "select schema_entity_id from lims_sys.schema_entity where name = 'Sample' ";

            OracleCommand command = new OracleCommand(sqlString, _connection);
            try {
                sampleEntityId = (int)(OracleNumber)command.ExecuteOracleScalar();
            } catch (Exception ex) {
                ErrorHandler.LogMessage(_operatorName, "PropagateLimits", "Error getting sample entity id:\r\n" + ex.Message);
                if (_connection != null) {
                    _connection.Close();
                }
                return;
            }

            if (entityId != sampleEntityId) {
                ErrorHandler.LogError(_operatorName, "PropagateLimits", "Attempted to execute this extension on wrong entity (" + entityId + ").");
                return;
            }


            //loop through all samples selected by user in Nautilus explorer window and propagate results for each
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];

            while (!records.EOF) {
                PropagateResultLimits(long.Parse(records.Fields[0].Value.ToString()), true);
                records.MoveNext();
            }

            //close connections
            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PropagateLimits", "Closing connections:\r\n" + ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// Copies limits into a sample's result_user records.  The limits come from the MCC database.
        /// </summary>
        /// <param name="sampleId">The sample_id of the sample to get copy of the PLD limits.</param>
        /// <param name="showPrompt">Show user dialog with limits before copying.</param>
        private void PropagateResultLimits(long sampleId, bool showPrompt) {

            decimal? lsbrTarget = null, lsbrUpper = null, lsbrLower = null;
            decimal? hbEcaTarget = null, hbEcaUpper = null, hbEcaLower = null;
            decimal? ironTarget = null, ironUpper = null, ironLower = null;
            decimal? alfeTarget = null, alfeUpper = null, alfeLower = null;


            //get the sample_id/name/limits of the PLD of the current sample
            string sqlString = "select name from lims_sys.sample where sample_id = :in_sample_id";
            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_sample_id", sampleId));
            OracleCommand updateCommand;
            string mixNumber = string.Empty;
            string mixEvaluation = string.Empty;

            try {
                // set the priveleges first.
                updateCommand = new OracleCommand("set role lims_user", _connection);
                updateCommand.ExecuteNonQuery();

                // get the mix number for currently selected mix.
                mixNumber = (string)(OracleString)command.ExecuteOracleScalar();
                if (mixNumber.Length >= 7) mixEvaluation = mixNumber.Substring(4, 3);
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PropagateLimits", "Error getting mix number for sample " + sampleId + ":\r\n" + ex.Message);
                return;
            }
            
            sqlString = "select u.u_hb_target, u.u_iron_oxide_target, u.u_lsbr_target "
                + "from lims_sys.u_evaluation_user u, lims_sys.u_evaluation e "
                + "where u.u_evaluation_id = e.u_evaluation_id "
                + "and e.name = :in_evaluation ";
            OracleCommand commandMcc = new OracleCommand(sqlString, _connection);
            commandMcc.Parameters.Add(new OracleParameter(":in_evaluation", mixEvaluation));
            OracleDataReader readerMcc;

            try {
                readerMcc = commandMcc.ExecuteReader();

                while (readerMcc.Read()) {
                    lsbrTarget = readerMcc["u_lsbr_target"] as decimal?;
                    hbEcaTarget = readerMcc["u_hb_target"] as decimal?;
                    ironTarget = readerMcc["u_iron_oxide_target"] as decimal?;
                }

                readerMcc.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PropagateLimits", "Error retrieving limits from MCC database for mix " + mixNumber + ":\r\n" + ex.Message);
                return;
            }


            // if the extension was manually run, prompt for corrections before writing to db.
            if (showPrompt) {
                PropagateLimitsForm plf = new PropagateLimitsForm(mixNumber, hbEcaTarget, ironTarget, lsbrTarget);
                plf.ShowDialog();
                hbEcaTarget = plf.HbTarget;
                ironTarget = plf.IronTarget;
                lsbrTarget = plf.LsbrTarget;
            }


            // calculate limits based on the three targets retrieved from Mcc:
            if (lsbrTarget != null && lsbrTarget != 0) {
                lsbrLower = lsbrTarget - 0.023m;
                lsbrUpper = lsbrTarget + 0.023m;
            }

            if (hbEcaTarget != null && hbEcaTarget != 0) {
                hbEcaLower = hbEcaTarget - 0.50m;
                hbEcaUpper = hbEcaTarget + 0.50m;
            }

            if (ironTarget != null && ironTarget != 0) {
                ironLower = ironTarget - 0.030m;
                ironUpper = ironTarget + 0.030m;

                alfeTarget = ironTarget + 16.00m;
                alfeLower = ironTarget + 15.90m;
                alfeUpper = ironTarget + 16.30m;
            }



            //update the Nautilus result_user table to have all the correct limits
            string sqlUpdateNautilus = "update lims_sys.result_user set u_target = :in_target, u_max_below_target = :in_lower, u_max_above_target = :in_upper "
                + "where result_id in (select r.result_id from lims_sys.result r, lims_sys.test t, lims_sys.aliquot a "
                + "where a.aliquot_id = t.aliquot_id "
                + "and t.test_id = r.test_id "
                + "and a.sample_id = :in_sample_id "
                + "and t.name = :in_test_name "
                + "and r.name = :in_result_name) ";

            try {
                command = new OracleCommand(sqlUpdateNautilus, _connection);
                command.Parameters.Add(new OracleParameter(":in_target", ConvertAndRound(lsbrTarget, 3)));
                command.Parameters.Add(new OracleParameter(":in_lower", ConvertAndRound(lsbrLower, 3)));
                command.Parameters.Add(new OracleParameter(":in_upper", ConvertAndRound(lsbrUpper, 3)));
                command.Parameters.Add(new OracleParameter(":in_sample_id", sampleId));
                command.Parameters.Add(new OracleParameter(":in_test_name", "TL_F1370"));
                command.Parameters.Add(new OracleParameter(":in_result_name", "Burn Rate"));
                command.ExecuteNonQuery();

                command = new OracleCommand(sqlUpdateNautilus, _connection);
                command.Parameters.Add(new OracleParameter(":in_target", ConvertAndRound(hbEcaTarget, 2)));
                command.Parameters.Add(new OracleParameter(":in_lower", ConvertAndRound(hbEcaLower, 2)));
                command.Parameters.Add(new OracleParameter(":in_upper", ConvertAndRound(hbEcaUpper, 2)));
                command.Parameters.Add(new OracleParameter(":in_sample_id", sampleId));
                command.Parameters.Add(new OracleParameter(":in_test_name", "FTIR_F0701"));
                command.Parameters.Add(new OracleParameter(":in_result_name", "HB (Liquid Fraction)"));
                command.ExecuteNonQuery();
                
                command = new OracleCommand(sqlUpdateNautilus, _connection);
                command.Parameters.Add(new OracleParameter(":in_target", ConvertAndRound(ironTarget, 3)));
                command.Parameters.Add(new OracleParameter(":in_lower", ConvertAndRound(ironLower, 3)));
                command.Parameters.Add(new OracleParameter(":in_upper", ConvertAndRound(ironUpper, 3)));
                command.Parameters.Add(new OracleParameter(":in_sample_id", sampleId));
                command.Parameters.Add(new OracleParameter(":in_test_name", "XRF_F2302"));
                command.Parameters.Add(new OracleParameter(":in_result_name", "Iron Oxide"));
                command.ExecuteNonQuery();
                
                command = new OracleCommand(sqlUpdateNautilus, _connection);
                command.Parameters.Add(new OracleParameter(":in_target", ConvertAndRound(alfeTarget, 3)));
                command.Parameters.Add(new OracleParameter(":in_lower", ConvertAndRound(alfeLower, 3)));
                command.Parameters.Add(new OracleParameter(":in_upper", ConvertAndRound(alfeUpper, 3)));
                command.Parameters.Add(new OracleParameter(":in_sample_id", sampleId));
                command.Parameters.Add(new OracleParameter(":in_test_name", "WET_F1965"));
                command.Parameters.Add(new OracleParameter(":in_result_name", "Al + Fe"));
                command.ExecuteNonQuery();
                
                command = new OracleCommand(sqlUpdateNautilus, _connection);
                command.Parameters.Add(new OracleParameter(":in_target", (decimal)86.00));
                command.Parameters.Add(new OracleParameter(":in_lower", (decimal)85.50));
                command.Parameters.Add(new OracleParameter(":in_upper", (decimal)86.50));
                command.Parameters.Add(new OracleParameter(":in_sample_id", sampleId));
                command.Parameters.Add(new OracleParameter(":in_test_name", "WET_F1965"));
                command.Parameters.Add(new OracleParameter(":in_result_name", "Total Solids"));
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "PropagateLimits", "Unable to update Nautilus result limits for " + mixNumber + ":\r\n" + ex.Message);
                return;
            }
        }

        private object ConvertAndRound(decimal? originalValue, int decimalPlaces) {
            if (originalValue == null) {
                return DBNull.Value;
            } else {
                return Math.Round((decimal)originalValue, decimalPlaces);
            }       
        }
    }
}
