using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using NautilusExtensions.All;

namespace NautilusExtensions.Ops {

    [Guid("DAD7B23F-2047-4b51-89B1-9382182DC009")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _FixTurnaroundTime : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("103834F0-494F-446b-9DE7-898125D730D6")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Ops.FixTurnaroundTime")]
    public class FixTurnaroundTime : _FixTurnaroundTime {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;

        #region IEntityExtension Members

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            //extension can be executed on samples only
            if (Parameters["ENTITY_ID"].ToString().Equals("73")) {
                return LSEXT.ExecuteExtension.exEnabled;
            } else {
                return LSEXT.ExecuteExtension.exDisabled;
            }
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            _operatorName = Parameters["OPERATOR_NAME"].ToString();
            
            //Connection string
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";


            try {
                _connection = new OracleConnection(connString);
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "FixTurnaroundTime", "DB connection error:\r\n" + ex.Message);
                return;
            }

            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            OracleCommand command = new OracleCommand();
            command.Connection = _connection;
            OracleDataReader reader;
            FixTurnaroundTimeForm fttf;
            DateTime receivedOn, authorisedOn;
            string sampleName;

            while (!records.EOF) {

                command.CommandText = "select s.name, to_char(s.received_on, 'MM/DD/YYYY HH24:MI:SS') received_on, "
                    + "to_char(s.authorised_on, 'MM/DD/YYYY HH24:MI:SS') authorised_on, su.u_processing_time "
                    + "from lims_sys.sample s, lims_sys.sample_user su "
                    + "where s.sample_id = su.sample_id "
                    + "and s.sample_id = " + records.Fields[0].Value.ToString();

                //run the query
                try {
                    reader = command.ExecuteReader();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "FixTurnaroundTime", "Error reading times - sample " 
                        + records.Fields[0].Value.ToString() + ":\r\n" + ex.Message);
                    records.MoveNext();
                    continue;
                }

                //make sure we can make datetime objects out of strings returned in query
                try {
                    reader.Read();
                    receivedOn = DateTime.ParseExact(reader["received_on"].ToString(), 
                        "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    authorisedOn = DateTime.ParseExact(reader["authorised_on"].ToString(),
                        "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    sampleName = reader["name"].ToString();
                    reader.Close();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "FixTurnaroundTime", records.Fields[0].Value.ToString() 
                        + " is missing a datetime value and cannot be edited:\r\n" + ex.Message);
                    records.MoveNext();
                    continue;
                }

                fttf = new FixTurnaroundTimeForm(sampleName, receivedOn, authorisedOn, _connection);
                reader.Close();
                fttf.ShowDialog();

                records.MoveNext();
            }

            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "FixTurnaroundTime", "Close connection error:\r\n" + ex.Message);
            }
        }

        #endregion

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
