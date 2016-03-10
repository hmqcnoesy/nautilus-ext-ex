using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Text;

namespace NautilusExtensions.All {

    [Guid("0C6D165B-3D59-4B9C-BE0E-A303A47EC66C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _Unauthorise : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("9685FFDA-2676-43D2-BAE6-742E6749AFD8")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.All.Unauthorise")]
    public class Unauthorise : _Unauthorise {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            
            //only allow system and superuser role to exectute
            if (!Parameters["ROLE_ID"].ToString().Equals("1") & !Parameters["ROLE_ID"].ToString().Equals("68")) {
                return LSEXT.ExecuteExtension.exDisabled;
            }

            //only allow to execute on sample or aliquot entity
            if (!Parameters["ENTITY_ID"].ToString().Equals("2") & !Parameters["ENTITY_ID"].ToString().Equals("84")) {
                return LSEXT.ExecuteExtension.exDisabled;
            }

            return LSEXT.ExecuteExtension.exEnabled;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            string operatorName = Parameters["OPERATOR_NAME"].ToString();

            //get a reason from the user
            UnauthoriseForm uf = new UnauthoriseForm();
            uf.ShowDialog();
            string unauthorisationReason = uf.getUnauthorisationReason();

            //don't make any updates if no reason is given (input box was canceled)
            if (unauthorisationReason.Equals(string.Empty)) {
                return;
            }

            //concantenate a list of IDs for selected items
            StringBuilder entityIdList = new StringBuilder();
            bool firstRow = true;
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            while (!records.EOF) {
                if (!firstRow) {
                    entityIdList.Append(",");
                } else {
                    firstRow = false;
                }
                entityIdList.Append(records.Fields[0].Value.ToString());
                records.MoveNext();
            }

            //make the database connection
            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = Parameters["SERVER_INFO"].ToString();
            ocsb.PersistSecurityInfo = true;
            ocsb.UserID = Parameters["USERNAME"].ToString();
            ocsb.Password = Parameters["PASSWORD"].ToString();
            ocsb.Unicode = true;

            OracleConnection connection = new OracleConnection(ocsb.ToString());
            OracleTransaction transaction;
            OracleCommand command;
            OracleDataReader reader;
            string sqlString;


            //open connection
            try {
                connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "Unauthorise", "Error opening database:\r\n" + ex.Message);
            }


            //set role which allows update statements
            try {
                sqlString = "set role lims_user";
                command = new OracleCommand(sqlString, connection);
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "Unauthorise", "Error setting role:\r\n" + ex.Message);
                return;
            }

            bool canExecute = true;

            //Aliquot - check that all selected aliquots and their parent samples are status A or R
            //then unauthorise the selected aliquots and all their progeny (tests and results)
            if (Parameters["ENTITY_ID"].ToString().Equals("2")) {
                //first ensure that all aliquots selected are authorised
                sqlString = "select distinct a.status, s.status from lims_sys.aliquot a, lims_sys.sample s "
                    + "where s.sample_id = a.sample_id and a.aliquot_id in (" + entityIdList.ToString() + ")";

                command = new OracleCommand(sqlString, connection);

                try {
                    reader = command.ExecuteReader();

                    //extension should be enabled only if all the statuses are A or R
                    while (reader.Read()) {
                        if (!reader[0].ToString().Equals("A") & !reader[0].ToString().Equals("R")) {
                            canExecute = false;
                        }

                        //need to check the status of the parent sample, which cannot be A or R
                        if (reader[1].ToString().Equals("A") || reader[1].ToString().Equals("R")) {
                            canExecute = false;
                        }
                    }

                    reader.Close();
                } catch (Exception ex) {
                    ErrorHandler.LogError(operatorName, "Unauthorise", "Error getting aliquot / sample statuses:\r\n" + ex.Message);
                    return;
                }


                if (canExecute) {
                    transaction = connection.BeginTransaction();
                    command.Transaction = transaction;
                    try {
                        //aliquot
                        sqlString = "update lims_sys.aliquot set status = 'C' where aliquot_id in (" + entityIdList.ToString() + ") ";
                        command.CommandText = sqlString;
                        command.ExecuteNonQuery();

                        //aliquot_user
                        sqlString = "update lims_sys.aliquot_user set u_adcar_transfer=null, u_trace_transfer=null, u_wp_transfer=null, "
                            + "u_unauthorization=u_unauthorization || '" + Parameters["SESSION_ID"].ToString() + ": ' || :in_reason || ';  ' "
                            + "where aliquot_id in (" + entityIdList.ToString() + ") ";
                        command.CommandText = sqlString;
                        command.Parameters.Add(new OracleParameter(":in_reason", unauthorisationReason));
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();

                        //test
                        sqlString = "update lims_sys.test set status = 'C' where status in ('R', 'A') "
                            + "and aliquot_id in (" + entityIdList.ToString() + ") ";
                        command.CommandText = sqlString;
                        command.ExecuteNonQuery();

                        //result
                        sqlString = "update lims_sys.result set status = 'C' where status in ('R', 'A') and test_id in "
                            + "(select t.test_id from lims_sys.test t, lims_sys.aliquot a "
                            + "where a.aliquot_id = t.aliquot_id and a.aliquot_id in (" + entityIdList.ToString() + ")) ";
                        command.CommandText = sqlString;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    } catch (Exception ex) {
                        transaction.Rollback();
                        ErrorHandler.LogError(operatorName, "Unauthorise", "Error updating status of aliquot or progeny:\r\n" + ex.Message);
                        return;
                    }
                } else {
                    ErrorHandler.LogError(operatorName, "Unauthorise", "Attempted to unauthorise aliquot(s) with status not R/A, or invalid parent sample status.");
                }

            } else {
                try {
                    //sample - check that all selected aliquots and their parent samples are status A or R
                    //then unauthorise the selected sample, and adjust values in the sample_user and aliquot_user tables accordingly
                    sqlString = "select distinct status from lims_sys.sample where sample_id in (" + entityIdList.ToString() + ")";

                    command = new OracleCommand(sqlString, connection);
                    reader = command.ExecuteReader();

                    //extension should be enabled only if all the statuses are A or R
                    while (reader.Read()) {
                        if (!reader[0].ToString().Equals("A") & !reader[0].ToString().Equals("R")) {
                            canExecute = false;
                        }
                    }
                    reader.Close();
                } catch (Exception ex) {
                    ErrorHandler.LogError(operatorName, "Unauthorise", "Error getting sample status:\r\n" + ex.Message);
                    return;
                }


                if (canExecute) {
                    transaction = connection.BeginTransaction();
                    command.Transaction = transaction;
                    try {
                        //sample
                        sqlString = "update lims_sys.sample set status = 'C' where sample_id in (" + entityIdList.ToString() + ") ";
                        command.CommandText = sqlString;
                        command.ExecuteNonQuery();

                        //sample_user
                        sqlString = "update lims_sys.sample_user set "
                            + "u_unauthorization=u_unauthorization || '" + Parameters["SESSION_ID"].ToString() + ": ' || :in_reason || ';  ' "
                            + "where sample_id in (" + entityIdList.ToString() + ") ";
                        command.CommandText = sqlString;
                        command.Parameters.Add(new OracleParameter(":in_reason", unauthorisationReason));
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();

                        //aliquot_user
                        sqlString = "update lims_sys.aliquot_user set u_adcar_transfer=null, u_trace_transfer=null, u_wp_transfer=null "
                            + "where aliquot_id in (select aliquot_id from lims_sys.aliquot where sample_id in (" + entityIdList.ToString() + ")) ";
                        command.CommandText = sqlString;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    } catch (Exception ex) {
                        transaction.Rollback();
                        ErrorHandler.LogError(operatorName, "Unauthorise", "Error updating status of sample:\r\n" + ex.Message);
                    }
                }
            }

            try {
                connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "Unauthorise", "Error closing database connection:\r\n" + ex.Message);
            }

            Parameters["REFRESH"] = true;
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
