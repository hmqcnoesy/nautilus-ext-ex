using System;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using LSEXT;
using System.Text;

namespace NautilusExtensions.Ops
{
    [Guid("7FE2094A-D33D-44CD-8330-2246E3F4EF6F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _CheckProject : LSEXT.IEntityExtension, LSEXT.IVersion
    {
    }

    [Guid("561EA6F7-B2A0-418D-820B-E63A28949047")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Ops.CheckProject")]
    public class CheckProject : _CheckProject
    {
        public int GetVersion()
        {
            return 4092; // increment this value when you make changes to prevent users from running old code
        }

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters parameters)
        {
            return LSEXT.ExecuteExtension.exEnabled;
        }


        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters parameters)
        {
            Cursor.Current = Cursors.WaitCursor;
            var sdgIds = new List<int>();
            var records = (ADODB.Recordset)parameters["RECORDS"];
            while (!records.EOF)
            {
                sdgIds.Add(int.Parse(records.Fields[0].Value));
                records.MoveNext();
            }

            var sdgNamesWithProjectIds = GetSdgNamesWithProjectIds(sdgIds, parameters);
            var projectIdsWithMessage = GetProjectIdMessages(sdgNamesWithProjectIds);

            var sb = new StringBuilder();
            foreach (var kvp in sdgNamesWithProjectIds)
            {
                sb.AppendLine(kvp.Key + "  -  " + kvp.Value + "  -  " + projectIdsWithMessage[kvp.Value]);
            }
            Cursor.Current = Cursors.Default;
            MessageBox.Show(sb.ToString());
        }


        private Dictionary<string, string> GetProjectIdMessages(Dictionary<string, string> sdgNamesWithProjectIds)
        {
            var projectIdMessages = new Dictionary<string, string>();
            var projectService = new CostpointProjectService.ProjectServiceClient();

            foreach (var projectId in sdgNamesWithProjectIds.Values.Distinct())
            {
                projectIdMessages.Add(projectId, string.Empty);
                var project = projectService.getProject(projectId);
                if (project == null)
                {
                    projectIdMessages[projectId] = "Not found";
                }
                else if (project.allowCharging == "Y")
                {
                    projectIdMessages[projectId] = "OK";
                }
                else
                {
                    projectIdMessages[projectId] = "Charges not allowed";
                }
            }

            return projectIdMessages;
        }


        private Dictionary<string, string> GetSdgNamesWithProjectIds(List<int> sdgIds, LSExtensionParameters parameters)
        {
            var ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = parameters["SERVER_INFO"].ToString();
            ocsb.Unicode = true;
            ocsb.UserID = parameters["USERNAME"].ToString();
            ocsb.Password = parameters["PASSWORD"].ToString();

            var sdgNamesWithProjectIds = new Dictionary<string, string>();

            using (var connection = new OracleConnection(ocsb.ToString()))
            {
                connection.Open();
                var sql = @"select s.name, u.u_project PROJECT
                        from lims_sys.sdg_user u, lims_sys.sdg s 
                        where s.sdg_id = u.sdg_id 
                        and s.sdg_id in (" + string.Join(",", sdgIds.Select(s => s.ToString())) + ")";
                var cmd = new OracleCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    sdgNamesWithProjectIds.Add(reader[0].ToString(), reader[1].ToString());
                }

                reader.Close();
            }

            return sdgNamesWithProjectIds;
        }
    }
}

