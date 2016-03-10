using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using NautilusExtensions.All;

namespace NautilusExtensions {
    public static class Common {

        internal static OracleConnection GetOracleConnection(LSEXT.LSExtensionParameters parameters) {
            OracleConnectionStringBuilder csb = new OracleConnectionStringBuilder();
            csb.DataSource = parameters["SERVER_INFO"].ToString();
            csb.PersistSecurityInfo = true;
            csb.UserID = parameters["USERNAME"].ToString();
            csb.Password = parameters["PASSWORD"].ToString();
            csb.Unicode = true;

            var connection = new OracleConnection(csb.ToString());

            try {
                connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError("Error connecting to Nautilus database:\r\n" + ex.Message);
            }

            return connection;
        }

        internal static string GetOperatorName(LSEXT.LSExtensionParameters parameters)
        {
            return parameters["OPERATOR_NAME"].ToString();
        }

        internal static int GetOperatorId(LSEXT.LSExtensionParameters parameters)
        {
            return (int)parameters["OPERATOR_ID"];
        }

        internal static int GetSingleEntityId(LSEXT.LSExtensionParameters parameters)
        {
            if (((ADODB.Recordset)parameters["RECORDS"]).RecordCount != 1) 
                throw new ApplicationException("Called GetSingleEntityId on recordset with more than one element.");

            return int.Parse(((ADODB.Recordset)parameters["RECORDS"]).Fields[0].Value);
        }

        internal static bool IsInstanceOfEntity(LSEXT.LSExtensionParameters parameters, OracleConnection connection, string entityName)
        {
            var sql = "select name from lims_sys.schema_entity where schema_entity_id = :in_schema_entity_id";
            var cmd = new OracleCommand(sql, connection);
            cmd.Parameters.Add(new OracleParameter(":in_schema_entity_id", (int)parameters["ENTITY_ID"]));

            var selectedEntity = (string)cmd.ExecuteScalar();
            return (selectedEntity.ToUpper() == entityName.ToUpper());
        }

        internal static int GetSelectedEntityCount(LSEXT.LSExtensionParameters parameters)
        {
            return (parameters["RECORDS"] as ADODB.Recordset).RecordCount;
        }
    }
}
