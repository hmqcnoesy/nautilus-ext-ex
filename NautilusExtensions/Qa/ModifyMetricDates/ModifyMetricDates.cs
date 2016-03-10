using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("EBA8B1EF-7D84-4BEC-87CF-8E43D46E634C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _ModifyMetricDates : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("CF8BBAE4-5FA1-4058-A10B-69ED709217AB")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.ModifyMetricDates")]
    public class ModifyMetricDates : _ModifyMetricDates {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {

            //only allow system, superuser, or manager role to exectute
            if (!Parameters["ROLE_ID"].ToString().Equals("1") & !Parameters["ROLE_ID"].ToString().Equals("68") & !Parameters["ROLE_ID"].ToString().Equals("2")) {
                return LSEXT.ExecuteExtension.exDisabled;
            }

            //extension can be executed on samples or aliquots only
            if (Parameters["ENTITY_ID"].ToString().Equals("84") || Parameters["ENTITY_ID"].ToString().Equals("2")) {
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
                ErrorHandler.LogError(_operatorName, "ModifyMetricDates", "DB connection error:\r\n" + ex.Message);
                return;
            }

            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            OracleCommand command = new OracleCommand();
            command.Connection = _connection;
            OracleDataReader reader;
            ModifyMetricDatesForm mmdf;
            DateTime? authorisedOn, receivedOn;
            string itemName;
            string sqlString;
            string sqlStringSample, sqlStringSampleUser, sqlStringAliquot, sqlStringAliquotUser, sqlStringTest, sqlStringResult;
            string reasonForChange = string.Empty;

            while (!records.EOF) {

                sqlStringTest = sqlStringResult = string.Empty;

                sqlString = "select name, status, authorised_on, received_on ";

                if (Parameters["ENTITY_ID"].ToString().Equals("84")) {
                    //the item is a sample
                    sqlString += "from lims_sys.sample where sample_id = " + records.Fields[0].Value.ToString();
                } else if (Parameters["ENTITY_ID"].ToString().Equals("2")) {
                    //the item is an aliquot
                    sqlString += "from lims_sys.aliquot where aliquot_id = " + records.Fields[0].Value.ToString();
                }

                //run the query
                try {
                    command.CommandText = sqlString;
                    reader = command.ExecuteReader();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "ModifyMetricDates", "Error reading times - entity " + Parameters["ENTITY_ID"].ToString()
                        + ", ID = " + records.Fields[0].Value.ToString() + ":\r\n" + ex.Message);
                    records.MoveNext();
                    continue;
                }

                try {
                    reader.Read();

                    //if (!reader["status"].ToString().Equals("A")) {
                    //    ErrorHandler.LogError(operatorName, "ModifyMetricDates", reader["name"].ToString() + " is not status A.  Its authorised date cannot be changed.");
                    //    reader.Close();
                    //    records.MoveNext();
                    //    continue;
                    //}

                    //if (!reader["old_status"].ToString().Contains("A")) {
                    //    ErrorHandler.LogError(operatorName, "ModifyMetricDates", reader["name"].ToString() + " has not been unauthorised.  It is not eligible for a change.");
                    //    reader.Close();
                    //    records.MoveNext();
                    //    continue;
                    //}

                    authorisedOn = receivedOn = null;
                    if (!reader.IsDBNull(reader.GetOrdinal("authorised_on"))) {
                        authorisedOn = (DateTime)reader.GetOracleDateTime(reader.GetOrdinal("authorised_on"));
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("received_on"))) {
                        receivedOn = (DateTime)reader.GetOracleDateTime(reader.GetOrdinal("received_on"));
                    }

                    //skip out here if both received_on and authorised_on are null - no change allowed.
                    if (authorisedOn == null && receivedOn == null) {
                        ErrorHandler.LogError(_operatorName, "ModifyMetricDates", "Item " + records.Fields[0].Value.ToString() + " does not have modifiable dates.");
                        records.MoveNext();
                        continue;
                    }

                    itemName = reader["name"].ToString();
                    reader.Close();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "ModifyMetricDates", "Error gettting authorised_on date for item: "
                        + records.Fields[0].Value.ToString() + ":\r\n" + ex.Message);
                    records.MoveNext();
                    continue;
                }

                reader.Close();
                mmdf = new ModifyMetricDatesForm(itemName, authorisedOn, receivedOn, reasonForChange);
                mmdf.ShowDialog();

                //update the record accordingly if a date was selected (property is null if cancel button was clicked).
                authorisedOn = mmdf.AuthorisedOn;
                receivedOn = mmdf.ReceivedOn;
                reasonForChange = mmdf.ReasonForChange;

                if (mmdf.UpdatedDates) {

                    OracleTransaction transaction;
                    transaction = _connection.BeginTransaction();

                    //if a sample is selected, just update sample.  If aliquot, update it and all children
                    if (Parameters["ENTITY_ID"].ToString().Equals("84")) {
                        sqlStringSample = "update lims_sys.sample set ";

                        if (authorisedOn != null) {
                            sqlStringSample += "authorised_on = to_date('" + ((DateTime)authorisedOn).ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') ";
                        }

                        if (receivedOn != null) {
                            if (authorisedOn != null) {
                                sqlStringSample += ", ";
                            }

                            sqlStringSample += "received_on = to_date('" + ((DateTime)receivedOn).ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') ";
                        }

                        sqlStringSample += "where name = '" + itemName + "' ";

                        sqlStringSampleUser = "update lims_sys.sample_user set u_unauthorization = u_unauthorization || '"
                            + Parameters["SESSION_ID"].ToString() + ": " + reasonForChange.Replace("\'", "\'\'") + "; ' where sample_id = " + records.Fields[0].Value.ToString();

                        try {
                            command = new OracleCommand("set role lims_user", _connection, transaction);
                            command.ExecuteNonQuery();

                            command.CommandText = sqlStringSample;
                            command.ExecuteNonQuery();

                            command.CommandText = sqlStringSampleUser;
                            command.ExecuteNonQuery();

                            transaction.Commit();
                        } catch (Exception ex) {
                            ErrorHandler.LogError(_operatorName, "ModifyMetricDates", "Error updating database.  Changes have not been saved:\r\n" + ex.Message);
                            transaction.Rollback();
                            records.MoveNext();
                            continue;
                        }
                    } else if (Parameters["ENTITY_ID"].ToString().Equals("2")) {

                        sqlStringAliquot = "update lims_sys.aliquot set ";

                        if (authorisedOn != null) {
                            sqlStringAliquot += "authorised_on = to_date('" + ((DateTime)authorisedOn).ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') ";

                            sqlStringTest = "update lims_sys.test set "
                                + "authorised_on = to_date('" + ((DateTime)authorisedOn).ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') "
                                + "where aliquot_id = " + records.Fields[0].Value.ToString();

                            sqlStringResult = "update lims_sys.result set "
                                + "authorised_on = to_date('" + ((DateTime)authorisedOn).ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') "
                                + "where test_id in (select t.test_id from lims_sys.test t "
                                + "where t.aliquot_id = " + records.Fields[0].Value.ToString() + ")";
                        }

                        if (receivedOn != null) {
                            if (authorisedOn != null) {
                                sqlStringAliquot += ", ";
                            }

                            sqlStringAliquot += "received_on = to_date('" + ((DateTime)receivedOn).ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS') ";
                        }

                        sqlStringAliquot += "where name = '" + itemName + "' ";

                        sqlStringAliquotUser = "update lims_sys.aliquot_user set u_unauthorization = u_unauthorization || '"
                            + Parameters["SESSION_ID"].ToString() + ": " + reasonForChange.Replace("\'", "\'\'") + "; ' where aliquot_id = " + records.Fields[0].Value.ToString();


                        try {
                            command = new OracleCommand("set role lims_user", _connection, transaction);
                            command.ExecuteNonQuery();

                            command.CommandText = sqlStringAliquot;
                            command.ExecuteNonQuery();

                            command.CommandText = sqlStringAliquotUser;
                            command.ExecuteNonQuery();

                            if (!string.IsNullOrEmpty(sqlStringTest)) {
                                command.CommandText = sqlStringTest;
                                command.ExecuteNonQuery();
                            }

                            if (!string.IsNullOrEmpty(sqlStringResult)) {
                                command.CommandText = sqlStringResult;
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        } catch (Exception ex) {
                            ErrorHandler.LogError(_operatorName, "ModifyMetricDates", "Error updating database.  Changes have not been saved:\r\n" + ex.Message);
                            transaction.Rollback();
                            records.MoveNext();
                            continue;
                        }
                    }
                }

                records.MoveNext();
            }

            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "ModifyMetricDates", "Close connection error:\r\n" + ex.Message);
            }
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
