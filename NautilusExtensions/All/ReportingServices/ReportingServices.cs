using System;
using System.Data;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Text;

namespace NautilusExtensions.All {
    [Guid("3D72CA54-6187-42D4-841E-2CBA76DDE578")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _ReportingServices : LSEXT.IGenericExtension, LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("E1064A9F-3FF9-4C26-9A22-3486E30C71F7")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.All.ReportingServices")]
    public class ReportingServices : _ReportingServices {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string operatorName;

        #region IGenericExtension Members

        void LSEXT.IGenericExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            operatorName = Parameters["OPERATOR_NAME"].ToString();

            string sqlString;
            OracleConnection connection;
            OracleCommand command;
            OracleDataReader reader;
            DataTable dt;
            object[] rowToAdd;
            ReportingServicesForm rsf;
            
            // connection string
            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = Parameters["SERVER_INFO"].ToString();
            ocsb.PersistSecurityInfo = true;
            ocsb.Unicode = true;
            ocsb.UserID = Parameters["USERNAME"].ToString();
            ocsb.Password = Parameters["PASSWORD"].ToString(); ;

            //query to retrieve reporting services with no entity assigned
            sqlString = "select r.name, r.description, nvl(u.u_url, '#') u_url, u.u_icon "
                + "from lims_sys.u_reporting_service r, lims_sys.u_reporting_service_user u "
                + "where r.u_reporting_service_id = u.u_reporting_service_id "
                + "and u.u_schema_entity_id is null "
                + "order by name ";

            dt = new DataTable();
            dt.Columns.Add("name", typeof(System.String));
            dt.Columns.Add("description", typeof(System.String));
            dt.Columns.Add("url", typeof(System.String));
            dt.Columns.Add("icon", typeof(System.String));

            try {
                connection = new OracleConnection(ocsb.ToString());
                connection.Open();
                command = new OracleCommand(sqlString, connection);
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    rowToAdd = new object[4];
                    rowToAdd[0] = reader["name"].ToString();
                    rowToAdd[1] = reader["description"].ToString();
                    rowToAdd[2] = reader["u_url"].ToString();
                    rowToAdd[3] = reader["u_icon"].ToString();
                    dt.Rows.Add(rowToAdd);
                }

                reader.Close();
                connection.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ReportingServices", "Error while accessing database:\r\n" + ex.Message);
                return;
            }


            rsf = new ReportingServicesForm(dt, string.Empty);
            rsf.ShowDialog();
        }

        #endregion

        #region IEntityExtension Members

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {
            return LSEXT.ExecuteExtension.exEnabled;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {

            operatorName = Parameters["OPERATOR_NAME"].ToString();

            string sqlString;
            OracleConnection connection;
            OracleCommand command;
            OracleDataReader reader;
            DataTable dt;
            object[] rowToAdd;
            ReportingServicesForm rsf;

            // connection string
            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = Parameters["SERVER_INFO"].ToString();
            ocsb.PersistSecurityInfo = true;
            ocsb.Unicode = true;
            ocsb.UserID = Parameters["USERNAME"].ToString();
            ocsb.Password = Parameters["PASSWORD"].ToString();

            //query to retrieve reporting services with no entity assigned
            sqlString = "select r.name, r.description, nvl(u.u_url, '#') u_url, u.u_icon "
                + "from lims_sys.u_reporting_service r, lims_sys.u_reporting_service_user u "
                + "where r.u_reporting_service_id = u.u_reporting_service_id "
                + "and u.u_schema_entity_id = :in_entity_id "
                + "order by name ";

            //concatenate a string of all the entity ids passed in (for use in url argument sent to report page)
            //the "&q=" portion before each id is there to identify each id as part of a multivalue parameter
            ADODB.Recordset records = (ADODB.Recordset)Parameters["RECORDS"];
            StringBuilder entityIdList = new StringBuilder();
            while (!records.EOF) {
                entityIdList.Append("&q=" + records.Fields[0].Value.ToString());
                records.MoveNext();
            }

            dt = new DataTable();
            dt.Columns.Add("name", typeof(System.String));
            dt.Columns.Add("description", typeof(System.String));
            dt.Columns.Add("url", typeof(System.String));
            dt.Columns.Add("icon", typeof(System.String));

            try {
                connection = new OracleConnection(ocsb.ToString());
                connection.Open();
                command = new OracleCommand(sqlString, connection);
                command.Parameters.Add(new OracleParameter(":in_entity_id", (int)Parameters["ENTITY_ID"]));
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    rowToAdd = new object[4];
                    rowToAdd[0] = reader["name"].ToString();
                    rowToAdd[1] = reader["description"].ToString();
                    rowToAdd[2] = reader["u_url"].ToString();
                    rowToAdd[3] = reader["u_icon"].ToString();
                    dt.Rows.Add(rowToAdd);
                }

                reader.Close();
                connection.Close();

            } catch (Exception ex) {
                ErrorHandler.LogError(operatorName, "ReportingServices", "Error while accessing database:\r\n" + ex.Message);
                return;
            }

            rsf = new ReportingServicesForm(dt, entityIdList.ToString());
            rsf.ShowDialog();
        }

        #endregion

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
