using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient;
using System.Runtime.InteropServices;

namespace NautilusExtensions.Qa {

    [Guid("486143F6-A6E3-4176-A1AF-84882DFFBB4E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface _ModifyAuthorisedData : LSEXT.IEntityExtension, LSEXT.IVersion {
    }

    [Guid("2C0B040A-0E01-462A-84C1-DBF8E4BA8AC8")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("NautilusExtensions.Qa.ModifyAuthorisedData")]
    public class ModifyAuthorisedData : _ModifyAuthorisedData {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code
        private string _operatorName;

        LSEXT.ExecuteExtension LSEXT.IEntityExtension.CanExecute(ref LSEXT.IExtensionParameters Parameters) {

            //only allow system, superuser, or manager role to exectute
            if (!Parameters["ROLE_ID"].ToString().Equals("1") & !Parameters["ROLE_ID"].ToString().Equals("68") & !Parameters["ROLE_ID"].ToString().Equals("2")) {
                return LSEXT.ExecuteExtension.exDisabled;
            }

            //only allow to execute on samples
            if (!Parameters["ENTITY_ID"].ToString().Equals("84")) {
                return LSEXT.ExecuteExtension.exDisabled;
            }

            return LSEXT.ExecuteExtension.exEnabled;
        }

        void LSEXT.IEntityExtension.Execute(ref LSEXT.LSExtensionParameters Parameters) {
            
            _operatorName = Parameters["OPERATOR_NAME"].ToString();
            
            string connString = "Data Source=" + Parameters["SERVER_INFO"]
                + ";Persist Security Info=True"
                + ";User Id=" + Parameters["USERNAME"]
                + ";Password=" + Parameters["PASSWORD"]
                + ";Unicode=True;";

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

            ModifyAuthorisedDataForm madf = new ModifyAuthorisedDataForm(entityIdList.ToString(), _operatorName, connString, Parameters["SESSION_ID"].ToString());
            madf.ShowDialog();
        }

        #region IVersion Members

        public int GetVersion() {
            return VERSION;
        }

        #endregion
    }
}
