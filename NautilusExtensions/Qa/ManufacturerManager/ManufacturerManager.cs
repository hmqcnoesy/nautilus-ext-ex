using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Text;
using System.Data;
using System.Windows.Forms;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [Guid("F0DD000F-D866-4023-8201-709F1ADA4D2C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _ManufacturerManager : LSEXT.IGenericExtension, LSEXT.IVersion {
    }

    [Guid("0354E9CE-BFD6-46B2-AE27-89AA2C1C622B")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.ManufacturerManager")]
    public class ManufacturerManager : _ManufacturerManager {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;
        private OracleConnection _connection;

        public void Execute(ref LSEXT.LSExtensionParameters Parameters) {

            _operatorName = (string)Parameters["OPERATOR_NAME"];

            //only allow 8123 Technician role to exectute
            if (!Parameters["ROLE_NAME"].ToString().Equals("8123 Technician") && !Parameters["ROLE_NAME"].ToString().Equals("System")) {
                ErrorHandler.LogError(_operatorName, "ManufacturerManager",
                    string.Format("Your current role is {0}.  Only the '8123 Technician' role can execute this extension.", (string)Parameters["ROLE_NAME"]));
                return;
            }

            //setup the database connection
            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = Parameters["SERVER_INFO"].ToString();
            ocsb.PersistSecurityInfo = true;
            ocsb.UserID = Parameters["USERNAME"].ToString();
            ocsb.Password = Parameters["PASSWORD"].ToString();
            ocsb.Unicode = true;

            using (_connection = new OracleConnection(ocsb.ToString())) {

                string sqlString;

                try {
                    _connection.Open();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "ManufacturerManager", "Error connecting to database:\r\n" + ex.Message);
                    return;
                }

                // get a list of manufacturers available
                List<string> manufacturerNames = new List<string>();
                string doNothingString = "(Do nothing)";
                manufacturerNames.Add(doNothingString);
                string addString = "(Add this value to manufacturer table)";
                manufacturerNames.Add(addString);

                sqlString = "select name from lims_sys.u_manufacturer order by name";
                OracleCommand command = new OracleCommand(sqlString, _connection);
                OracleDataReader reader;

                try {
                    reader = command.ExecuteReader();

                    while (reader.Read()) {
                        manufacturerNames.Add(reader[0].ToString());
                    }

                    reader.Close();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "ManufacturerManager", "Error getting all manufacturer names:\r\n" + ex.Message);
                    return;
                }

                // set role to allow updating
                sqlString = "set role lims_user";
                command = new OracleCommand(sqlString, _connection);

                try {
                    command.ExecuteNonQuery();
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "ManufacturerManager", "Error setting Oracle role:\r\n" + ex.Message);
                    return;
                }

                // get table of samples with non-standard manufacturers/suppliers
                DataTable manufacturersTable = new DataTable();

                sqlString = " select s.sample_id, s.name \"Sample Name\", 'Manufacturer' \"Field\", su.u_manufacturer \"Value\" "
                    + "from lims_sys.sample s, lims_sys.sample_user su "
                    + "where s.sample_id = su.sample_id "
                    + "and trim(upper(su.u_manufacturer)) not in (select name from lims_sys.u_manufacturer) "
                    + "and s.status != 'X' "
                    + "union "
                    + "select s.sample_id, s.name, 'Supplier', su.u_supplier "
                    + "from lims_sys.sample s, lims_sys.sample_user su "
                    + "where s.sample_id = su.sample_id "
                    + "and trim(upper(su.u_supplier)) not in (select name from lims_sys.u_manufacturer) "
                    + "and s.status != 'X' "
                    + "order by 1";

                OracleDataAdapter adapter = new OracleDataAdapter(sqlString, _connection);

                try {
                    adapter.Fill(manufacturersTable);
                    manufacturersTable.Columns.Add("NewValue", typeof(System.String));
                } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "ManufacturerManager", "Error retrieving samples with non-standard manufacturer/supplier:\r\n" + ex.Message);
                    return;
                }

                // display the form using the data table
                using (ManufacturerManagerForm mmf = new ManufacturerManagerForm(manufacturerNames, manufacturersTable)) {
                    if (mmf.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                        // the form put the new owners names in the table
                        foreach (DataRow dr in manufacturersTable.Rows) {
                            if (dr["NewValue"].ToString().Equals(addString)) {
                                // add the selected value to the manufacturer table
                                InsertNewManufacturer((string)dr["Value"]);
                            } else if (!((string)dr["NewValue"]).Equals(doNothingString)) {
                                // change the manufacturer value in the sample user table to the selected value
                                UpdateSampleManufacturerValue((int)(decimal)dr["SAMPLE_ID"], (string)dr["Field"], (string)dr["NewValue"]);
                            }
                        }
                    }
                }
            }
        }


        private void UpdateSampleManufacturerValue(int sampleId, string fieldName, string newValue) {

            string sqlString = string.Empty;

            if (fieldName.Equals("Supplier") || fieldName.Equals("Manufacturer")) {
                sqlString = string.Format("update lims_sys.sample_user set u_{0} = :in_new_value where sample_id = :in_sample_id", fieldName);
            } else {
                ErrorHandler.LogError(_operatorName, "ManufacturerManager",
                    string.Format("Error attempting to update sample '{0}' value.  Must be 'Supplier' or 'Manufacturer'.", fieldName));
                return;
            }

            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_new_value", newValue));
            command.Parameters.Add(new OracleParameter(":in_sample_id", sampleId));

            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "ManufacturerManager",
                    string.Format("Error updating {0} value of sample {1} to {2}:\r\n{3}", fieldName, sampleId, newValue, ex.Message));
            }
        }


        private void InsertNewManufacturer(string newValue) {
            OracleTransaction trans = _connection.BeginTransaction();

            string sqlStringSequence = "select lims.sq_u_manufacturer.nextval from dual";
            string sqlStringManuf = "insert into lims_sys.u_manufacturer values(:in_manufacturer_id, trim(upper(:in_name)), trim(upper(:in_name)), 1, 'A', null, null)";
            string sqlStringUser = "insert into lims_sys.u_manufacturer_user values(:in_manufacturer_id)";

            try {
                int newId = (int)(decimal)(new OracleCommand(sqlStringSequence, _connection, trans)).ExecuteScalar();
                OracleCommand command = new OracleCommand(sqlStringManuf, _connection, trans);
                command.Parameters.Add(new OracleParameter(":in_manufacturer_id", newId));
                command.Parameters.Add(new OracleParameter(":in_name", newValue));
                command.ExecuteNonQuery();

                command = new OracleCommand(sqlStringUser, _connection, trans);
                command.Parameters.Add(new OracleParameter(":in_manufacturer_id", newId));
                command.ExecuteNonQuery();

                trans.Commit();
            } catch (OracleException oex) {
                // skip any unique constraint errors (user can select to insert same value multiple times)
                if (!oex.Message.StartsWith("ORA-00001")) {
                    ErrorHandler.LogError(_operatorName, "ManufacturerManager", string.Format("Error updating manufacturer table with value {0}:\r\n{1}", newValue, oex.Message));
                    trans.Rollback();
                }
                trans.Rollback();
            } catch (Exception ex) {
                    ErrorHandler.LogError(_operatorName, "ManufacturerManager", string.Format("Error updating manufacturer table with value {0}:\r\n{1}", newValue, ex.Message));
                    trans.Rollback();
            }
        }


        public int GetVersion() {
            return VERSION;
        }
    }
}
