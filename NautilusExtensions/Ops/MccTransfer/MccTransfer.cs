using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NautilusExtensions.All;
using System.Data.OracleClient;

namespace NautilusExtensions.Ops.MccTransfer
{

    [Guid("DD032729-65A9-4B2F-8D3A-1ACB00EEF4C3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _MccTransfer : LSEXT.IWorkflowExtension, LSEXT.IEntityExtension, LSEXT.IVersion
    {
    }

    [Guid("616C64D2-E854-40A5-A6BC-44A8BF5A98FF")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Ops.MccTransfer")]
    public class MccTransfer : _MccTransfer
    {
        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private string _connectionString;


        void LSEXT.IWorkflowExtension.Execute(ref LSEXT.LSExtensionParameters parameters)
        {
            SetOperatorName(parameters);
            SetConnectionString(parameters);

            if ((string)parameters["TABLE_NAME"] == "TEST")
            {
                ErrorHandler.LogError(_operatorName, "MccTransfer", "Attempted to run extension on workflow node that is not a test.");
                return;
            }

            Transfer((int)parameters["PRIMARY_KEY"]);
        }


        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters parameters)
        {
            return LSEXT.ExecuteExtension.exEnabled;
        }


        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters parameters)
        {
            SetOperatorName(parameters);
            SetConnectionString(parameters);

            if (94 != (int)parameters["ENTITY_ID"])
            {
                ErrorHandler.LogError(_operatorName, "MccTransfer", "Attempted to run extension on entity other than test.");
                return;
            }

            ADODB.Recordset records = (ADODB.Recordset)parameters["RECORDS"];

            while (!records.EOF)
            {
                Transfer(int.Parse(records.Fields[0].Value.ToString()));
                records.MoveNext();
            }
        }


        int LSEXT.IVersion.GetVersion()
        {
            return VERSION;
        }


        private void SetConnectionString(LSEXT.LSExtensionParameters parameters)
        {
            var ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = parameters["SERVER_INFO"].ToString();
            ocsb.PersistSecurityInfo = true;
            ocsb.Unicode = true;
            ocsb.UserID = parameters["USERNAME"].ToString();
            ocsb.Password = parameters["PASSWORD"].ToString();
            _connectionString = ocsb.ToString();
        }


        private void SetOperatorName(LSEXT.LSExtensionParameters parameters)
        {
            _operatorName = parameters["OPERATOR_NAME"].ToString();
        }


        private void Transfer(int testId)
        {
            string mixNumber = GetAliquotName(testId);
            string testName = GetTestName(testId);

            switch (testName)
            {
                case "WET_F1965":
                    UpdateMcc(mixNumber, "SOLIDS", GetResultValue(testId, "Total Solids"));
                    UpdateMcc(mixNumber, "ALFEO", GetResultValue(testId, "Al + Fe"));
                    UpdateMcc(mixNumber, "OXIDIZER", GetResultValue(testId, "Oxidizer"));
                    break;
                case "TL_F1370":
                    UpdateMcc(mixNumber, "LSBR", GetResultValue(testId, "Burn Rate"));
                    break;
                case "FTIR_F0701":
                    UpdateMcc(mixNumber, "HBECA", GetResultValue(testId, "HB (Liquid Fraction)"));
                    break;
                case "XRF_F2302":
                    UpdateMcc(mixNumber, "IRON", GetResultValue(testId, "Iron Oxide"));
                    break;
                case "WET_F1975":
                    UpdateMcc(mixNumber, "VISCOSITY", GetResultValue(testId, "Viscosity"));
                    UpdateMcc(mixNumber, "VISCOSITYTIME", GetResultValue(testId, "Reading Time"));
                    UpdateMcc(mixNumber, "VISCOSITYTEMPERATURE", GetResultValue(testId, "Temperature"));
                    break;
                default:
                    return;
            }

        }


        private string GetAliquotName(int testId)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                string sql = "select a.name from lims_sys.aliquot a, lims_sys.test t "
                    + " where a.aliquot_id = t.aliquot_id "
                    + " and t.test_id = :in_test_id";

                var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter(":in_test_id", testId));
                
                try
                {
                    connection.Open();
                    return (string)command.ExecuteScalar();
                }
                catch
                {
                    ErrorHandler.LogError(_operatorName, "MccTransfer", "Unable to get aliquot name for test " + testId);
                    throw;
                }
            }
        }


        private string GetTestName(int testId)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                string sql = "select t.name from lims_sys.test t where t.test_id = :in_test_id";

                var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter(":in_test_id", testId));

                try
                {
                    connection.Open();
                    return (string)command.ExecuteScalar();
                }
                catch
                {
                    ErrorHandler.LogError(_operatorName, "MccTransfer", "Unable to get aliquot name for test " + testId);
                    throw;
                }
            }
        }


        private object GetResultValue(int testId, string resultName)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                string sql = "select r.calculated_numeric_result from lims_sys.result r where r.test_id = :in_test_id and r.name = :in_result_name";

                var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter(":in_test_id", testId));
                command.Parameters.Add(new OracleParameter(":in_result_name", resultName));

                try
                {
                    connection.Open();
                    return command.ExecuteScalar();
                }
                catch
                {
                    ErrorHandler.LogError(_operatorName, "MccTransfer", "Unable to get aliquot name for test " + testId);
                    throw;
                }
            }

        }


        private void UpdateMcc(string mixNumber, string resultName, object resultValue)
        {
            using (var connection = new OracleConnection("Data Source=prod1;User ID=mcc_app;Password=pqtr0lry;Unicode=True"))
            {
                string sql = string.Format("update mcc set {0} = :in_result_value where casteval = :in_casteval and mixnumber = :in_mixnumber", resultName);
                var command = new OracleCommand(sql, connection);
                command.Parameters.Add(new OracleParameter(":in_result_value", resultValue));
                command.Parameters.Add(new OracleParameter(":in_casteval", mixNumber.Substring(0, 3)));
                command.Parameters.Add(new OracleParameter(":in_mixnumber", int.Parse(mixNumber.Substring(3))));
                
                int rows = command.ExecuteNonQuery();

                if (1 != rows)
                {
                    ErrorHandler.LogError(_operatorName, "MccTransfer", string.Format("Unexpected {0} rows updated: {1}", rows, command.CommandText));
                }
            }
        }
    }
}
