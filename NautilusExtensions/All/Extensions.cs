using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NautilusExtensions {
    public static class Extensions {
        public static object Parameterize(this object value) {
            if (value == System.DBNull.Value || value == null || string.IsNullOrWhiteSpace(value.ToString())) {
                return System.DBNull.Value;
            } else {
                return value.ToString();
            }
        }


        public static DataColumn GetColumnWithExPropNautilusTestName(this DataTable dt, string testName)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ExtendedProperties["nautilustestname"].ToString() == testName) return dc;
            }

            return null;
        }


        public static bool HasMatchingColumn(this DataTable dt, string testName, string testDescription)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.GetExPropNautilusTestDescription() == testDescription && dc.GetExPropNautilusTestName() == testName)
                {
                    return true;
                }
            }

            return false;
        }


        public static void SetExPropNautilusTestName(this DataColumn dc, string testName)
        {
            dc.ExtendedProperties["nautilustestname"] = testName;
        }


        public static string GetExPropNautilusTestName(this DataColumn dc)
        {
            return dc.ExtendedProperties["nautilustestname"] == null ? string.Empty : dc.ExtendedProperties["nautilustestname"].ToString();
        }


        public static void SetExPropNautilusTestDescription(this DataColumn dc, string testName)
        {
            dc.ExtendedProperties["nautilustestdescription"] = testName;
        }


        public static string GetExPropNautilusTestDescription(this DataColumn dc)
        {
            return dc.ExtendedProperties["nautilustestdescription"] == null ? string.Empty : dc.ExtendedProperties["nautilustestdescription"].ToString();
        }
    }
}
