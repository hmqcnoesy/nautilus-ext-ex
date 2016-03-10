using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {

    [Guid("FD34FFE1-6244-4F1C-8746-75377CE42919")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _M19aXrfResultFormat : LSEXT.IResultFormat, LSEXT.IVersion {
    }

    [Guid("D748DE61-791E-406D-BF94-E4AC5ABBBF99")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Ops.M19aXrfResultFormat")]
    public class M19aXrfResultFormat : _M19aXrfResultFormat {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        #region IResultFormat Members

        LSEXT.ResultFieldChange LSEXT.IResultFormat.FieldChange(ref LSEXT.LSExtensionParameters Parameters) {
            return LSEXT.ResultFieldChange.rcAllow;
        }

        LSEXT.ResultEntryFormat LSEXT.IResultFormat.Format(ref LSEXT.LSExtensionParameters Parameters, LSEXT.ResultEntryPhase Phase) {
            //only run at the revalidate event
            if (Phase != LSEXT.ResultEntryPhase.reValidate) return LSEXT.ResultEntryFormat.rfDoDefault;

            //don't run on PLD samples, only mix samples
            if (Parameters["name"].ToString().StartsWith("UL - ") ||
                Parameters["name"].ToString().StartsWith("LL - ") ||
                Parameters["name"].ToString().StartsWith("Target - ")) { return LSEXT.ResultEntryFormat.rfDoDefault; }


            //get username
            string operatorName = Parameters["OPERATOR_NAME"].ToString();

            //create a new connection
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

            //get the method_used and the mix number (aliquot.name) from the test_user table
            string methodUsed = string.Empty;
            string mixNumber = string.Empty;
            OracleConnection connection = new OracleConnection(connString);
            try {
                connection.Open();
                string sqlString = "select nvl(tu.u_method_used, 'Not provided') u_method_used, a.name "
                    + "from lims_sys.test_user tu, lims_sys.test t, lims_sys.aliquot a "
                    + "where t.test_id = tu.test_id and a.aliquot_id = t.aliquot_id and t.test_id = " + Parameters["test_id"].ToString();
                OracleCommand command = new OracleCommand(sqlString, connection);
                OracleDataReader reader = command.ExecuteReader();
                int i = 0;
                while (reader.Read()) {
                    methodUsed = reader["u_method_used"].ToString();
                    mixNumber = reader["name"].ToString();
                    i++;
                }

                //the row count (i) better be 1 or there is a problem
                if (i != 1) {
                    ErrorHandler.LogError(operatorName, "M19aXrfResultFormat", i.ToString() + " records returned for method_used/mix_number.\r\n"
                        + "Please contact the programmer.");
                }

                //clean up and close
                reader.Close();
                connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "M19aXrfResultFormat", "Error getting test_user.u_method_used:\r\n" + ex.Message);
            }

            //the method_used needs to be equal to the first three chars of the mix number
            if (!methodUsed.ToUpper().Equals(mixNumber.ToUpper().Substring(0,3))) {
                ErrorHandler.LogError(operatorName, "M19aXrfResultFormat", "The XRF method used for mix " + mixNumber + "  does not match the mix's evaluation.\r\n"
                    + "\r\nMethod Used:  " + methodUsed
                    + "\r\nEvaluation: " + mixNumber.Substring(0, 3)
                    + "\r\n\r\nUse the 'Reset Tests' extension on this aliquot before resubmitting the corrected results.");
            }
            
            return LSEXT.ResultEntryFormat.rfDoDefault;
        }

        #endregion

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
