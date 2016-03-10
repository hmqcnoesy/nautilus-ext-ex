using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NautilusExtensions.All {

    static class FileHelper {

        /// <summary>
        /// Overwrites an existing file of the specified name, or creates if it doesn't exist.
        /// </summary>
        /// <param name="fileName">Path to file, including filename</param>
        /// <param name="fileContents">Contents to write to new/overwriting file</param>
        public static void OverwriteOrCreateFile(string fileName, string fileContents) {
            try {
                //create the file using the streamwriter class
                using (StreamWriter sw = File.CreateText(fileName)) {
                    sw.Write(fileContents);
                    sw.Flush();
                    sw.Close();
                }
            } catch (Exception ex) {
                ErrorHandler.LogError("Could not create or overwrite file at '" + fileName + "':\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// Returns a string containing the first line of the contents of the file specified.
        /// </summary>
        /// <param name="fileName">The complete path to the file to be read.</param>
        /// <returns>string containing first line of contents.</returns>
        public static string GetFirstLineFileContents(string fileName) {
            if (File.Exists(fileName)) {
                string firstLine = string.Empty;
                using (StreamReader sr = File.OpenText(fileName)) {
                    firstLine = sr.ReadLine();
                }
                return firstLine;
            } else {
                return string.Empty;
            }
        }

        /// <summary>
        /// Appends fileContents to a file if it exists, creates the file with fileContents if it doesn't.
        /// </summary>
        /// <param name="fileName">Full path to file.</param>
        /// <param name="fileContents">Contents to append or create.</param>
        public static void AppendOrCreateFile(string fileName, string fileContents) {
            //write to or create the file using the streamwriter class
            try {
                using (StreamWriter sw = File.AppendText(fileName)) {
                    sw.Write(fileContents);
                    sw.Flush();
                    sw.Close();
                }
            } catch (Exception ex) {
                ErrorHandler.LogError("Could not create or append to file at '" + fileName + "':\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// Creates a file with specified path/name and contents.
        /// </summary>
        /// <param name="fileName">The full path to the file.</param>
        /// <param name="fileContents">The contents of the file to be created.</param>
        public static void CreateFile(string fileName, string fileContents) {
            //write to or create the file using the streamwriter class
            try {
                using (StreamWriter sw = File.CreateText(fileName)) {
                    sw.Write(fileContents);
                    sw.Flush();
                    sw.Close();
                }
            } catch (Exception ex) {
                ErrorHandler.LogError("Could not create file at '" + fileName + "':\r\n" + ex.Message);
            }
        }
    }
}
