using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace NautilusExtensions.All {
    public static class EmailHelper {

        private static MailMessage InitializeMail(List<string> toAddresses) {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.From = new MailAddress("Nautilus-do-not-reply@example.com");
            mail.Body = @"
<html>
    <head>
        <style type=""text/css"">
            div#header { 
                font-family: Arial; 
                font-size: 28px; 
                color: White; 
                background-color: firebrick; 
                border-style: solid; 
                border-width: 2px 3px 1px; 
                border-color: lightgrey lightgrey black; }
            body { font-family:Tahoma; font-size: 8pt;}
            a { font-family:Tahoma; font-size: 8pt; color: firebrick; text-decoration: none; text-decoration: underline; }
            table { border: 1px solid black; }
            tr.even { background-color: Gainsboro; }
            th { text-align: left; border-bottom: 1px solid black; padding: 4px; }
            td { border: 1px dashed Gainsboro; padding: 4px; }
            p.warning { font-weight: bold; color: red; }
        </style>
    </head>
    <body><div id=""header"">Nautilus Authorised Data Modification Notification</div>{0}</body>
</html>";

            foreach (string s in toAddresses) {
                mail.To.Add(s);
            }
            return mail;
        }

        public static void SendModifyAuthorisedDataEmail(List<string> toAddresses, DataTable dtUpdates) {
            SmtpClient client = new SmtpClient(Properties.Settings.Default.SmtpServer, Properties.Settings.Default.SmtpPort);
            MailMessage mail = InitializeMail(toAddresses);

            mail.Subject = "Nautilus: Modify Authorised Data Extension";
            StringBuilder sb = new StringBuilder("<p><strong>Your email address has been designated as a notification destination for the Nautilus 'Modify Authorised Data' extension.</strong></p>");
            
            sb.Append("<p>The following updates have been made:<p>");

            sb.Append("<table><tr>");

            foreach (DataColumn dc in dtUpdates.Columns) {
                sb.Append(string.Format("<th>{0}</th>", dc.ColumnName));
            }

            sb.Append("</tr>");

            bool isEven = true;

            foreach (DataRow dr in dtUpdates.Rows) {

                isEven = !isEven;

                sb.Append(string.Format("<tr{0}>", isEven ? " class=\"even\"" : string.Empty));

                foreach (DataColumn dc in dtUpdates.Columns) {
                    sb.Append(string.Format("<td>{0}</td>", dr[dc].ToString()));
                }

                sb.Append("</tr>");
            }

            sb.Append("</table>");

            mail.Body = mail.Body.Replace("{0}", sb.ToString());  // string.format doesn't work well with user input or with css text
            client.Send(mail);
        }
    }
}