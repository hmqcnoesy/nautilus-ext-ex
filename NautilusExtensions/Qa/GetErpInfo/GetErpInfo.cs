using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data.OracleClient;
using NautilusExtensions.All;
using System.Text.RegularExpressions;
using NautilusExtensions.CostpointReceiptService;

namespace NautilusExtensions.Qa
{

    [Guid("27B191ED-9EB7-4C8A-A412-AB9AE83BB814")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _GetErpInfo : LSEXT.IWorkflowExtension, LSEXT.IEntityExtension, LSEXT.IVersion
    {
    }

    [Guid("B2D44EE7-FC77-46CC-9F50-B32BE157DEF4")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.GetErpInfo")]
    public class GetErpInfo : _GetErpInfo
    {
        private const int VERSION = 4091;
        private string _operatorName;
        private OracleConnection _connection;


        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters)
        {
            return LSEXT.ExecuteExtension.exEnabled;
        }


        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters)
        {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();
            Cursor savedCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            using (_connection = new OracleConnection(GetConnectionString(Parameters)))
            {
                _connection.Open();
                var cmd = new OracleCommand("set role lims_user", _connection);
                cmd.ExecuteNonQuery();

                if (!IsSelectedEntitySample((int)Parameters["ENTITY_ID"])) return;

                ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];

                while (!records.EOF)
                {
                    PopulateSampleWithErpInfo(int.Parse(records.Fields[0].Value.ToString()));
                    records.MoveNext();
                }
            }
            Parameters["REFRESH"] = true;
            Cursor.Current = savedCursor;
        }


        void LSEXT.IWorkflowExtension.Execute(ref LSEXT.LSExtensionParameters Parameters)
        {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();
            Cursor savedCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            using (_connection = new OracleConnection(GetConnectionString(Parameters)))
            {
                _connection.Open();
                var cmd = new OracleCommand("set role lims_user", _connection);
                cmd.ExecuteNonQuery();

                if (Parameters["TABLE_NAME"] != "SAMPLE") 
                {
                    ErrorHandler.LogError(_operatorName, "GetErpInfo",
                        "Attempted to run extension as a workflow node under a parent node that is not a SAMPLE in workflow " + Parameters["WORKFLOW_ID"]);
                    return;
                }

                PopulateSampleWithErpInfo(Parameters["PRIMARY_KEY"]);
            }

            Cursor.Current = savedCursor;
        }


        int LSEXT.IVersion.GetVersion()
        {
            return VERSION;
        }


        private string GetConnectionString(LSEXT.LSExtensionParameters Parameters)
        {
            var ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = Parameters["SERVER_INFO"];
            ocsb.PersistSecurityInfo = true;
            ocsb.UserID = Parameters["USERNAME"];
            ocsb.Password = Parameters["PASSWORD"];
            ocsb.Unicode = true;
            return ocsb.ToString();
        }

        
        private bool IsSelectedEntitySample(int entityId)
        {
            var sql = "select count(*) from lims_sys.schema_entity where name = 'Sample' and schema_entity_id = :in_schema_entity_id ";
            var cmd = new OracleCommand(sql, _connection);
            cmd.Parameters.AddWithValue(":in_schema_entity_id", entityId);

            var count = (int)(OracleNumber)cmd.ExecuteOracleScalar();

            if (count != 1)
            {
                ErrorHandler.LogMessage(_operatorName, "GetErpInfo", "Attempted to execute this extension on wrong entity (" + entityId + ").");
            }

            return (count == 1);
        }


        private void PopulateSampleWithErpInfo(int sampleId)
        {
            var receiverNumber = GetSampleReceiverNumber(sampleId);
            var receiverNumberAndPoLineAndRecLine = SplitReceiverNumberIntoReceiverAndPoLineAndReceiverLine(receiverNumber);
            var receiverNumberAndLineObjects = GetCostpointReceiverLine(receiverNumberAndPoLineAndRecLine.Item1, 
                                                                        receiverNumberAndPoLineAndRecLine.Item2, 
                                                                        receiverNumberAndPoLineAndRecLine.Item3);

            if (receiverNumberAndLineObjects == null) return;

            UpdateSampleErpInfo(sampleId, receiverNumberAndLineObjects.Item1, receiverNumberAndLineObjects.Item2);
            UpdateSampleQmisInfo(sampleId, receiverNumber);
        }


        private Tuple<Receipt, ReceiptLine> GetCostpointReceiverLine(string receiverNumber, int poLineNumber, int recLineNumber)
        {
            var ws = new CostpointReceiptService.ReceiptServiceClient();
            try
            {
                var receipt = ws.selectReceipt(receiverNumber);
                if (receipt == null)
                {
                    ErrorHandler.LogError("Couldn't find Costpoint receiver: " + receiverNumber);
                    return null;
                }
                
                var throwAway = 0;
                var matchingLines = from r in receipt.receiptLines
                                    where int.TryParse(r.lineNumber, out throwAway)
                                    && int.TryParse(r.poLine, out throwAway)
                                    && int.Parse(r.lineNumber) == recLineNumber
                                    && int.Parse(r.poLine) == poLineNumber
                                    select r;

                if (matchingLines.Count() == 1)
                {
                    return new Tuple<Receipt, ReceiptLine>(receipt, matchingLines.Single());
                }
                else
                {
                    ErrorHandler.LogError(string.Format("Couldn't match up Costpoint receiver, line, and PO line: {0}/{1}/{2}", receiverNumber, recLineNumber, poLineNumber));
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError("Error communicating with Costpoint: " + ex.Message);
                return null;
            }
        }


        private string GetSampleReceiverNumber(int sampleId)
        {
            var sql = "select upper(u_receiver_number) from lims_sys.sample_user where sample_id = :in_sample_id";
            var cmd = new OracleCommand(sql, _connection);
            cmd.Parameters.AddWithValue(":in_sample_id", sampleId);

            return (string)cmd.ExecuteScalar();
        }


        private Tuple<string, int, int> SplitReceiverNumberIntoReceiverAndPoLineAndReceiverLine(string receiverNumber)
        {
            // format of receiver is XXXXXXXXXX-####
            // first three digits of suffix are po line number, last digit is receiver line
            // if a po goes over 999 lines, the dash comes out...hence the following regex
            if (Regex.IsMatch(receiverNumber, @"^[A-Za-z0-9]{10}[\-1-9][0-9]{4}$"))
            {
                string receiver;
                int poLine, recLine;

                if (receiverNumber.Contains('-'))
                {
                    var split = receiverNumber.Split("-".ToCharArray());
                    receiver = split[0];
                    poLine = int.Parse(split[1].Substring(0, 3));
                    recLine = int.Parse(split[1].Substring(3));
                }
                else
                {
                    receiver = receiverNumber.Substring(0, 10);
                    poLine = int.Parse(receiverNumber.Substring(10, 4));
                    recLine = int.Parse(receiverNumber.Substring(14));
                }

                return new Tuple<string,int,int>(receiver, poLine, recLine);
            }
            else
            {
                ErrorHandler.LogError(_operatorName, "GetErpInfo", "Receiver number did not meet criteria for processing: " + receiverNumber);
                return new Tuple<string, int, int>(string.Empty, 0, 0);
            }
        }


        private void UpdateSampleErpInfo(int sampleId, Receipt cpReceiver, ReceiptLine cpReceiverLine)
        {
            var sql = @"update lims_sys.sample_user set 
                u_serial_number = :in_serial_number,
                u_purchase_order = :in_purchase_order,
                u_quantity = :in_quantity,
                u_um = :in_um,
                u_material_location = :in_material_location
                where sample_id = :in_sample_id";
            var cmd = new OracleCommand(sql, _connection);
            var consolidatedSlNumbers = ConsolidateSerialLotNumbers(cpReceiverLine.serialLotNumbers);
            cmd.Parameters.AddWithValue(":in_serial_number", string.IsNullOrEmpty(consolidatedSlNumbers) ? DBNull.Value : (object)consolidatedSlNumbers);
            cmd.Parameters.AddWithValue(":in_purchase_order", string.IsNullOrEmpty(cpReceiver.purchaseOrderNumber) ? DBNull.Value : (object)cpReceiver.purchaseOrderNumber);
            cmd.Parameters.AddWithValue(":in_quantity", string.IsNullOrEmpty(cpReceiverLine.qtyReceived) ? DBNull.Value : (object)cpReceiverLine.qtyReceived);
            cmd.Parameters.AddWithValue(":in_um", string.IsNullOrEmpty(cpReceiverLine.um) ? DBNull.Value : (object)cpReceiverLine.um);
            cmd.Parameters.AddWithValue(":in_material_location", string.IsNullOrEmpty(cpReceiverLine.receiptLocation) ? DBNull.Value : (object)cpReceiverLine.receiptLocation);
            cmd.Parameters.AddWithValue(":in_sample_id", sampleId);
            cmd.ExecuteNonQuery();
        }


        private void UpdateSampleQmisInfo(int sampleId, string qmisReceiverNumber)
        {
            var inspection = GetInspectionNumberFromQmis(qmisReceiverNumber);

            var sql = @"update lims_sys.sample_user set u_inspection = :in_inspection where sample_id = :in_sample_id";
            var cmd = new OracleCommand(sql, _connection);
            cmd.Parameters.AddWithValue(":in_inspection", string.IsNullOrEmpty(inspection) ? DBNull.Value : (object)inspection);
            cmd.Parameters.AddWithValue(":in_sample_id", sampleId);

            cmd.ExecuteNonQuery();
        }


        private string ConsolidateSerialLotNumbers(SerialLotNumbers serialLotNumbers)
        {
            if (serialLotNumbers == null)
            {
                return string.Empty;
            }
            else if (serialLotNumbers.lotNumbers != null && serialLotNumbers.lotNumbers.Count() > 0)
            {
                return string.Join(", ", serialLotNumbers.lotNumbers.Where(n => n.lotNumber != null).Select(n => n.lotNumber));
            }
            else if (serialLotNumbers.serialNumbers != null && serialLotNumbers.serialNumbers.Count() > 0)
            {
                return string.Join(", ", serialLotNumbers.serialNumbers.Where(n => n.serialNumber != null).Select(n => n.serialNumber));
            }
            else
            {
                return string.Empty;
            }
        }


        private string GetInspectionNumberFromQmis(string qmisReceiverNumber)
        {
            var sql = @"select nvl(i.ih_inspection_number, 'Not Found')
                from qmis.IX_INS_RCV_X_REF@qmisp x, qmis.ih_inspection@qmisp i 
                where i.ih_inspection_number = x.ix_inspection_number 
                and x.ix_receiver_number = :in_receiver_number";
            var cmd = new OracleCommand(sql, _connection);
            cmd.Parameters.AddWithValue(":in_receiver_number", qmisReceiverNumber);
            var inspection = cmd.ExecuteScalar();
            return inspection == DBNull.Value ? "Not found" : (string)inspection;
        }
    }
}
