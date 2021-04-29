using LSEXT;
using LSSERVICEPROVIDERLib;
using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NautilusExtensions.All.OpenWorkflow
{
    public class OpenWorkflow : IEntityExtension
    {
        ExecuteExtension IEntityExtension.CanExecute(ref IExtensionParameters parameters)
        {
            var records = (ADODB.Recordset)parameters["RECORDS"];
            if (records.RecordCount == 1)
                return ExecuteExtension.exEnabled;
            else
                return ExecuteExtension.exDisabled;
        }


        void IEntityExtension.Execute(ref LSExtensionParameters parameters)
        {
            var serviceProvider = (NautilusServiceProvider)parameters["SERVICE_PROVIDER"];
            var schemaServiceProvider = (NautilusSchema)serviceProvider.QueryServiceProvider("Schema");
            var menuServiceProvider = (NautilusPopupMenu)serviceProvider.QueryServiceProvider("PopupMenu");
            var records = (ADODB.Recordset)parameters["RECORDS"];
            var recordId = (string)records.Fields[0].Value;
            var entityId = (int)parameters["ENTITY_ID"];

            var ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = (string)parameters["SERVER_INFO"];
            ocsb.UserID = (string)parameters["USERNAME"];
            ocsb.Password = (string)parameters["PASSWORD"];

            using (var connection = new OracleConnection(ocsb.ConnectionString))
            {
                connection.Open();
                var sql = "select st.database_name table_name, sf.database_name column_name, replace(se.name, 'Stock ', '') "
                    + "from lims_sys.schema_entity se "
                    + "join lims_sys.schema_table st on st.schema_table_id = se.schema_table_id "
                    + "join lims_sys.schema_field sf on sf.schema_table_id = st.schema_table_id "
                    + "where se.schema_entity_id = :entity_id "
                    + "and sf.unique_key = 'T'";

                var cmd = new OracleCommand(sql, connection);
                cmd.Parameters.AddWithValue(":entity_id", entityId);
                var reader = cmd.ExecuteReader();
                reader.Read();

                sql = $"select workflow_id from lims_sys.workflow_node where workflow_node_id = (select workflow_node_id from lims_sys.{reader[0]} where {reader[1]} = :id)";
                cmd = new OracleCommand(sql, connection);
                cmd.Parameters.AddWithValue(":id", recordId);
                var workflowId = cmd.ExecuteScalar().ToString();
                
                menuServiceProvider.PopupMenuSelectDefault(schemaServiceProvider.EntityIdFromName((string)reader[2] + " Workflow"), workflowId);
            }
        }
    }
}
