using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NautilusExtensions.All {
    static class ErrorHandler {

        #region Log Errors methods

        /// <summary>
        /// Writes an error to the extension_errors.txt log file.
        /// If an operator other than lims_bp, a message box is also displayed.
        /// </summary>
        /// <param name="errorMessage">The message to log.  Usually the Extension.Message text.</param>
        public static void LogError(string errorMessage) {
            LogError("Not Specified", "Not Specified", errorMessage);
        }

        /// <summary>
        /// Writes an error to the extension_errors.txt log file.
        /// If an operator other than lims_bp, a message box is also displayed.
        /// </summary>
        /// <param name="extensionName">The name of the extension being executed.  Usually the calling class name.</param>
        /// <param name="errorMessage">The message to log.  Usually the Extension.Message text.</param>
        public static void LogError(string extensionName, string errorMessage) {
            LogError("Not Specified", extensionName, errorMessage);
        }

        /// <summary>
        /// Writes an error to the extension_errors.txt log file.
        /// If an operator other than lims_bp, a message box is also displayed.
        /// </summary>
        /// <param name="operatorName">The name of the current operator.  Usually Parameters["OPERATOR_NAME"].ToString().</param>
        /// <param name="extensionName">The name of the extension being executed.  Usually the calling class name.</param>
        /// <param name="errorMessage">The message to log.  Usually the Extension.Message text.</param>
        public static void LogError(string operatorName, string extensionName, string errorMessage) {

            //If the operator name is NOT lims_bp, show the errors in a message box also
            if (!operatorName.ToLower().Equals("lims_bp")) {
                MessageBox.Show("Error while executing extension '" + extensionName + "'" + Environment.NewLine + errorMessage);
            }

            //write to or create the file using the streamwriter class
            using (StreamWriter sw = File.AppendText(GetLogFilePathAndName())) {
                sw.WriteLine();
                sw.WriteLine("EXTENSION ERROR");
                sw.WriteLine("Date Time:      " + DateTime.Now.ToString());
                sw.WriteLine("Extension:      " + extensionName);
                sw.WriteLine("Operator:       " + operatorName);
                sw.WriteLine("Error Message:  " + errorMessage);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
        }

        #endregion

        #region Log Messages methods

        /// <summary>
        /// Writes a message to the extension_errors.txt file.
        /// </summary>
        /// <param name="extensionMessage">The message to log.</param>
        public static void LogMessage(string extensionMessage) {

            LogMessage("Not Specified", "Not Specified", extensionMessage);
        }

        /// <summary>
        /// Writes a message to the extension_errors.txt file.
        /// </summary>
        /// <param name="extensionName">The name of the extension being executed.  Usually the calling class name.</param>
        /// <param name="extensionMessage">The message to log.</param>
        public static void LogMessage(string extensionName, string extensionMessage) {

            LogMessage("Not Specified", extensionName, extensionMessage);
        }

        /// <summary>
        /// Writes a message to the extension_errors.txt file.
        /// </summary>
        /// <param name="operatorName">The name of the current operator.  Usually Parameters["OPERATOR_NAME"].ToString().</param>
        /// <param name="extensionName">The name of the extension being executed.  Usually the calling class name.</param>
        /// <param name="extensionMessage">The message to log.</param>
        public static void LogMessage(string operatorName, string extensionName, string extensionMessage) {

            //write to or create the file using the streamwriter class
            using (StreamWriter sw = File.AppendText(GetLogFilePathAndName())) {
                sw.WriteLine();
                sw.WriteLine("EXTENSION MESSAGE");
                sw.WriteLine("Date Time:      " + DateTime.Now.ToString());
                sw.WriteLine("Extension:      " + extensionName);
                sw.WriteLine("Operator:       " + operatorName);
                sw.WriteLine("Message:        " + extensionMessage);
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Gets the path with file name of the extension_errors.txt log from the registry.
        /// If not found, it uses c:\temp.
        /// Also creates the directory structure to the file if it doesn't already exist.
        /// </summary>
        /// <returns>string</returns>
        private static string GetLogFilePathAndName() {
            string pathToLogFile;

            //get the pathToLogFile here from the registry
            //use c:\temp if we can't find one
            pathToLogFile = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Thermo\Nautilus\9.3\Directory", "Log", "C:\\temp");

            //make sure the directory exists
            if (!Directory.Exists(pathToLogFile)) {
                Directory.CreateDirectory(pathToLogFile);
            }

            //append the file name to the path
            pathToLogFile += "\\Extension_Errors.txt";

            return pathToLogFile;
        }

        #endregion
    }
}
