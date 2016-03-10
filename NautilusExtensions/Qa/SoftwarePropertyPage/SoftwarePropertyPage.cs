using System;
using System.Data;
using System.Data.OracleClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LSExtensionControlLib;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {

    [ProgId("NautilusExtensions.Qa.SoftwarePropertyPage")]
    [ComVisible(true)]
    public partial class SoftwareInfoPage : UserControl, LSExtensionControlLib.IExtensionControl, LSEXT.IVersion {

        private const int VERSION = 4091;  // increment this value when you make changes to prevent users from running old code

        private OracleConnection _connection;
        private string _connectionString;

        private bool allowNull = false;
        private bool active, newSoftware, changeNotice;
        private string usedInAreas, name, fileName, softwareType, description;
        private string requestedBy, requesterOrg, programmer, programmerOrg, changeDesc, justification;
        private DateTime requestedOn, estimatedCompletionOn;
        private string proposedTesting, testedBy, testingResults;
        private DateTime testedOn;
        private double programmerIdSignedBy, userIdSignedBy, reviewerIdSignedBy, qeIdSignedBy, supervisorIdSignedBy,
            safetyIdSignedBy, crbIdSignedBy, gatekeeperId;
        private string ocrNumber;
        private DateTime programmerSignedOn, userSignedOn, reviewerSignedOn, qeSignedOn, supervisorSignedOn,
            safetySignedOn, crbSignedOn;
        private string backupLocation, documentationLocation, revision;
        private DateTime releasedOn;

        //there are only certain fields that we need to check if they are null
        private bool isNullRequestedOn, isNullEstimateCompletedOn, isNullTestedOn, isNullReleasedOn;
        private bool isNullProgrammerSignedOn, isNullUserSignedOn, isNullReviewerSignedOn, isNullQeSignedOn,
            isNullSupervisorSignedOn, isNullSafetySignedOn, isNullCrbSignedOn;

        protected LSExtensionControlLib.IExtensionControlSite extSite = null;

        public SoftwareInfoPage() {
            InitializeComponent();
        }

        #region IExtensionControl Members

        void IExtensionControl.EnterPage() {
        }

        void IExtensionControl.ExitPage() {
        }

        void IExtensionControl.Internationalise() {
        }

        void IExtensionControl.PreDisplay() {
            try {
                _connection = new OracleConnection(_connectionString);
                _connection.Open();
            } catch (Exception ex) {
                ErrorHandler.LogError("SoftwarePropertyPage", "Updates will not be possible, there was a problem connecting:\r\n" + ex.Message);
            }
        }

        void IExtensionControl.RestoreSettings(int hKey) {
        }

        void IExtensionControl.SaveData() {

            //top of form
            extSite.SetStringValue("U_USED_IN_AREAS", txtUsedInAreas.Text);
            extSite.SetBooleanValue("U_ACTIVE", chkActive.Checked);
            extSite.SetBooleanValue("U_NEW_SOFTWARE", chkNewSoftware.Checked);
            extSite.SetBooleanValue("U_CHANGE_NOTICE", chkChangeNotice.Checked);

            //info group box
            extSite.SetStringValue("U_FILE_NAME", txtFileName.Text);

            if (rbBusiness.Checked) {
                extSite.SetStringValue("U_TYPE", "Business");
            } else if (rbEngineering.Checked) {
                extSite.SetStringValue("U_TYPE", "Engineering");
            } else if (rbSupport.Checked) {
                extSite.SetStringValue("U_TYPE", "Support");
            } else {
                extSite.SetStringValue("U_TYPE", "Product");
            }

            //request group box
            extSite.SetStringValue("U_REQUESTED_BY", txtRequestedBy.Text);
            extSite.SetStringValue("U_REQUESTING_ORGANIZATION", txtRequesterOrg.Text);
            extSite.SetStringValue("U_PROGRAMMER", txtProgrammer.Text);
            extSite.SetStringValue("U_PROGRAMMING_ORGANIZATION", txtProgrammerOrg.Text);
            extSite.SetStringValue("U_CHANGE_DESCRIPTION", txtChangeDescription.Text);
            extSite.SetStringValue("U_CHANGE_JUSTIFICATION", txtJustification.Text);
            extSite.SetDateValue("U_REQUESTED_ON", dtpRequested.Value);
            if (dtpEstimate.Checked) {
                extSite.SetDateValue("U_ESTIMATED_COMPLETION", dtpEstimate.Value);
            } 

            //testing group box
            extSite.SetStringValue("U_PROPOSED_TESTING", txtProposedTesting.Text);
            extSite.SetStringValue("U_TESTED_BY", txtTestedBy.Text);
            extSite.SetStringValue("U_TESTING_RESULTS", txtResults.Text);
            extSite.SetDateValue("U_TESTED_ON", dtpTestDate.Value);

            //approval group box
            if (!txtSigProgrammer.Text.Equals(string.Empty)) {
                extSite.SetDoubleValue("U_PROGRAMMER_SIGNED_BY", GetUserId(txtSigProgrammer.Text));
                extSite.SetDateValue("U_PROGRAMMER_SIGNED_ON", ConvertStringToDate(txtDateProgrammer.Text));
            }
            if (!txtSigUser.Text.Equals(string.Empty)) {
                extSite.SetDoubleValue("U_USER_SIGNED_BY", GetUserId(txtSigUser.Text));
                extSite.SetDateValue("U_USER_SIGNED_ON", ConvertStringToDate(txtDateUser.Text));
            }
            if (!txtSigReviewer.Text.Equals(string.Empty)) {
                extSite.SetDoubleValue("U_REVIEWER_SIGNED_BY", GetUserId(txtSigReviewer.Text));
                extSite.SetDateValue("U_REVIEWER_SIGNED_ON", ConvertStringToDate(txtDateReviewer.Text));
            }
            if (!txtSigQe.Text.Equals(string.Empty)) {
                extSite.SetDoubleValue("U_QE_SIGNED_BY", GetUserId(txtSigQe.Text));
                extSite.SetDateValue("U_QE_SIGNED_ON", ConvertStringToDate(txtDateQe.Text));
            }
            if (!txtSigSupervisor.Text.Equals(string.Empty)) {
                extSite.SetDoubleValue("U_SUPERVISOR_SIGNED_BY", GetUserId(txtSigSupervisor.Text));
                extSite.SetDateValue("U_SUPERVISOR_SIGNED_ON", ConvertStringToDate(txtDateSupervisor.Text));
            }
            if (!txtSigSafety.Text.Equals(string.Empty)) {
                extSite.SetDoubleValue("U_SAFETY_SIGNED_BY", GetUserId(txtSigSafety.Text));
                extSite.SetDateValue("U_SAFETY_SIGNED_ON", ConvertStringToDate(txtDateSafety.Text));
            }
            if (!txtSigCrb.Text.Equals(string.Empty)) {
                extSite.SetDoubleValue("U_CRB_SIGNED_BY", GetUserId(txtSigCrb.Text));
                extSite.SetDateValue("U_CRB_SIGNED_ON", ConvertStringToDate(txtDateCrb.Text));
            }
            if (!txtGatekeeper.Text.Equals(string.Empty)) {
                extSite.SetDoubleValue("U_GATEKEEPER_SIGNED_BY", GetUserId(txtGatekeeper.Text));
            }

            extSite.SetStringValue("U_OCR_NUMBER", txtOcrNumber.Text);

            //effectivity group box
            extSite.SetStringValue("U_BACKUP_LOCATION", txtBackupLocation.Text);
            extSite.SetStringValue("U_DOCUMENTATION_LOCATION", txtDocumentLocation.Text);
            extSite.SetStringValue("U_REVISION", txtRevision.Text);
            extSite.SetDateValue("U_RELEASED_ON", dtpReleased.Value);

            try {
                _connection.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError("SoftwarePropertyPage", "There was a problem closing the connection:\r\n" + ex.Message);
            }
        }

        void IExtensionControl.SaveSettings(int hKey) {
        }

        void IExtensionControl.SetReadOnly(bool readOnly) {
            readOnly = false;
        }

        void IExtensionControl.SetServiceProvider(object serviceProvider) {
            // need to convert the ADO connection string provided into one the .net oracle client likes
            try {
                OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
                ocsb.Unicode = true;
                ocsb.PersistSecurityInfo = true;

                string adoConnectionString = 
                    ((LSSERVICEPROVIDERLib.NautilusServiceProvider)serviceProvider).QueryServiceProvider("DBConnection").GetADOConnectionString();
                string[] parts = adoConnectionString.Split(";".ToCharArray());

                foreach (string s in parts) {
                    if (s.StartsWith("User ID=")) {
                        ocsb.UserID = s.Substring(s.IndexOf("User ID=") + 8);
                    }

                    if (s.StartsWith("Password=")) {
                        ocsb.Password = s.Substring(s.IndexOf("Password=") + 9);
                    }

                    if (s.StartsWith("Data Source=")) {
                        ocsb.DataSource = s.Substring(s.IndexOf("Data Source=") + 12);
                    }
                }
                _connectionString = ocsb.ToString();
            } catch (Exception ex) {
                ErrorHandler.LogError("Error obtaining suitable connection string from NautilusServiceProvider object:\r\n" + ex.Message);
            }
        }

        void IExtensionControl.SetSite(object site) {
            if (site != null) {
                extSite = site as IExtensionControlSite;
            }
        }

        void IExtensionControl.SetupData() {

            //if the gatekeeper has signed, make all the controls read-only
            extSite.GetDoubleValue("U_GATEKEEPER_SIGNED_BY", out gatekeeperId, out allowNull);
            if (gatekeeperId > 0) {
                MakeAllControlsReadOnly();
            }

            //get the rest of the database values to populate the form
            extSite.GetStringValue("U_USED_IN_AREAS", out usedInAreas, out allowNull);
            extSite.GetBooleanValue("U_ACTIVE", out active, out allowNull);
            extSite.GetBooleanValue("U_NEW_SOFTWARE", out newSoftware, out allowNull);
            extSite.GetBooleanValue("U_CHANGE_NOTICE", out changeNotice, out allowNull);
            extSite.GetStringValue("NAME", out name, out allowNull);
            extSite.GetStringValue("U_FILE_NAME", out fileName, out allowNull);
            extSite.GetStringValue("U_TYPE", out softwareType, out allowNull);
            extSite.GetStringValue("DESCRIPTION", out description, out allowNull);
            extSite.GetStringValue("U_REQUESTED_BY", out requestedBy, out allowNull);
            extSite.GetStringValue("U_REQUESTING_ORGANIZATION", out requesterOrg, out allowNull);
            extSite.GetDateValue("U_REQUESTED_ON", out requestedOn, out isNullRequestedOn);
            extSite.GetStringValue("U_PROGRAMMER", out programmer, out allowNull);
            extSite.GetStringValue("U_PROGRAMMING_ORGANIZATION", out programmerOrg, out allowNull);
            extSite.GetDateValue("U_ESTIMATED_COMPLETION", out estimatedCompletionOn, out isNullEstimateCompletedOn);
            extSite.GetStringValue("U_CHANGE_DESCRIPTION", out changeDesc, out allowNull);
            extSite.GetStringValue("U_CHANGE_JUSTIFICATION", out justification, out allowNull);
            extSite.GetStringValue("U_PROPOSED_TESTING", out proposedTesting, out allowNull);
            extSite.GetStringValue("U_TESTED_BY", out testedBy, out allowNull);
            extSite.GetDateValue("U_TESTED_ON", out testedOn, out isNullTestedOn);
            extSite.GetStringValue("U_TESTING_RESULTS", out testingResults, out allowNull);
            extSite.GetDoubleValue("U_PROGRAMMER_SIGNED_BY", out programmerIdSignedBy, out allowNull);
            extSite.GetDateValue("U_PROGRAMMER_SIGNED_ON", out programmerSignedOn, out isNullProgrammerSignedOn);
            extSite.GetDoubleValue("U_USER_SIGNED_BY", out userIdSignedBy, out allowNull);
            extSite.GetDateValue("U_USER_SIGNED_ON", out userSignedOn, out isNullUserSignedOn);
            extSite.GetDoubleValue("U_REVIEWER_SIGNED_BY", out reviewerIdSignedBy, out allowNull);
            extSite.GetDateValue("U_REVIEWER_SIGNED_ON", out reviewerSignedOn, out isNullReviewerSignedOn);
            extSite.GetDoubleValue("U_QE_SIGNED_BY", out qeIdSignedBy, out allowNull);
            extSite.GetDateValue("U_QE_SIGNED_ON", out qeSignedOn, out isNullQeSignedOn);
            extSite.GetDoubleValue("U_SUPERVISOR_SIGNED_BY", out supervisorIdSignedBy, out allowNull);
            extSite.GetDateValue("U_SUPERVISOR_SIGNED_ON", out supervisorSignedOn, out isNullSupervisorSignedOn);
            extSite.GetDoubleValue("U_SAFETY_SIGNED_BY", out safetyIdSignedBy, out allowNull);
            extSite.GetDateValue("U_SAFETY_SIGNED_ON", out safetySignedOn, out isNullSafetySignedOn);
            extSite.GetDoubleValue("U_CRB_SIGNED_BY", out crbIdSignedBy, out allowNull);
            extSite.GetDateValue("U_CRB_SIGNED_ON", out crbSignedOn, out isNullCrbSignedOn);
            extSite.GetStringValue("U_OCR_NUMBER", out ocrNumber, out allowNull);
            extSite.GetStringValue("U_BACKUP_LOCATION", out backupLocation, out allowNull);
            extSite.GetStringValue("U_DOCUMENTATION_LOCATION", out documentationLocation, out allowNull);
            extSite.GetStringValue("U_REVISION", out revision, out allowNull);
            extSite.GetDateValue("U_RELEASED_ON", out releasedOn, out isNullReleasedOn);
            
            //top of form
            txtUsedInAreas.Text = usedInAreas;
            chkActive.Checked = active;
            chkNewSoftware.Checked = newSoftware;
            chkChangeNotice.Checked = changeNotice;

            //Information group box
            txtName.Text = name;
            txtFileName.Text = fileName;
            txtDescription.Text = description;
            switch (softwareType) {
                case "Support":
                    rbSupport.Checked = true;
                    break;
                case "Engineering":
                    rbEngineering.Checked = true;
                    break;
                case "Business":
                    rbBusiness.Checked = true;
                    break;
                default:
                    rbProduct.Checked = true;
                    break;
            }

            //Request group box
            txtRequestedBy.Text = requestedBy;
            txtRequesterOrg.Text = requesterOrg;
            txtProgrammer.Text = programmer;
            txtProgrammerOrg.Text = programmerOrg;
            txtChangeDescription.Text = changeDesc;
            txtJustification.Text = justification;

            if (isNullRequestedOn) {
                dtpRequested.Value = DateTime.Today;
                dtpRequested.Checked = false;
            } else {
                dtpRequested.Value = requestedOn;
                dtpRequested.Checked = true;
            }

            if (isNullEstimateCompletedOn) {
                dtpEstimate.Value = DateTime.Today;
                dtpEstimate.Checked = false;
            } else {
                dtpEstimate.Value = estimatedCompletionOn;
                dtpEstimate.Checked = true;
            }

            //Testing group box
            txtProposedTesting.Text = proposedTesting;
            txtTestedBy.Text = testedBy;
            txtResults.Text = testingResults;

            if (isNullTestedOn) {
                dtpTestDate.Value = DateTime.Today;
                dtpTestDate.Checked = false;
            } else {
                dtpTestDate.Value = testedOn;
                dtpTestDate.Checked = true;
            }

            //Approval group box
            if (programmerIdSignedBy > 0) {
                txtSigProgrammer.Text = GetUserName(programmerIdSignedBy);
                btnSignProgrammer.Visible = false;
            }
            if (userIdSignedBy > 0) {
                txtSigUser.Text = GetUserName(userIdSignedBy);
                btnSignUser.Visible = false;
            }
            if (reviewerIdSignedBy > 0) {
                txtSigReviewer.Text = GetUserName(reviewerIdSignedBy);
                btnSignReviewer.Visible = false;
            }
            if (qeIdSignedBy > 0) {
                txtSigQe.Text = GetUserName(qeIdSignedBy);
                btnSignQe.Visible = false;
            }
            if (supervisorIdSignedBy > 0) {
                txtSigSupervisor.Text = GetUserName(supervisorIdSignedBy);
                btnSignSupervisor.Visible = false;
            }
            if (safetyIdSignedBy > 0) {
                txtSigSafety.Text = GetUserName(safetyIdSignedBy);
                btnSignSafety.Visible = false;
            }
            if (crbIdSignedBy > 0) {
                txtSigCrb.Text = GetUserName(crbIdSignedBy);
                btnSignCrb.Visible = false;
            }

            if (!isNullProgrammerSignedOn) {
                txtDateProgrammer.Text = programmerSignedOn.ToString("MM/dd/yyyy");
            }
            if (!isNullUserSignedOn) {
                txtDateUser.Text = userSignedOn.ToString("MM/dd/yyyy");
            }
            if (!isNullReviewerSignedOn) {
                txtDateReviewer.Text = reviewerSignedOn.ToString("MM/dd/yyyy");
            }
            if (!isNullQeSignedOn) {
                txtDateQe.Text = qeSignedOn.ToString("MM/dd/yyyy");
            }
            if (!isNullSupervisorSignedOn) {
                txtDateSupervisor.Text = supervisorSignedOn.ToString("MM/dd/yyyy");
            }
            if (!isNullSafetySignedOn) {
                txtDateSafety.Text = safetySignedOn.ToString("MM/dd/yyyy");
            }
            if (!isNullCrbSignedOn) {
                txtDateCrb.Text = crbSignedOn.ToString("MM/dd/yyyy");
            }

            txtOcrNumber.Text = ocrNumber.ToString();
            
            //Effectivity group box
            txtBackupLocation.Text = backupLocation;
            txtDocumentLocation.Text = documentationLocation;
            if (gatekeeperId > 0) {
                txtGatekeeper.Text = GetUserName(gatekeeperId);
                btnSignGatekeeper.Enabled = false;
            }
            txtRevision.Text = revision;

            if (isNullReleasedOn) {
                dtpReleased.Value = DateTime.Today;
                dtpReleased.Checked = false;
            } else {
                dtpReleased.Value = releasedOn;
                dtpReleased.Checked = true;
            }

        }

        #endregion

        /// <summary>
        /// Returns a user's Nautilus full name.
        /// </summary>
        /// <param name="userId">The user's nautilus operator_id.</param>
        /// <returns></returns>
        private string GetUserName(double userId) {
            string userName = string.Empty;
            string sqlString = "select name from lims_sys.operator where operator_id = " + (int)userId;
            OracleCommand command;

            try {
                command = new OracleCommand(sqlString, _connection);
                userName = (string)(OracleString)command.ExecuteOracleScalar();
            } catch (Exception ex) {
                userName = ex.Message;
            }

            return userName;
        }

        /// <summary>
        /// Returns an operator_id based on the Nautilus operator.name 
        /// </summary>
        /// <param name="userName">The Nautilus name of the operator</param>
        /// <returns></returns>
        private int GetUserId(string operatorName) {
            int returnValue = 0;
            string sqlString = "select operator_id from lims_sys.operator where upper(name) = upper(:name)";
            OracleCommand command = new OracleCommand(sqlString, _connection);
            OracleParameter parameter = new OracleParameter(":name", operatorName);
            command.Parameters.Add(parameter);

            try {
                returnValue = (int)(OracleNumber)command.ExecuteOracleScalar();
            } catch (Exception ex) {
                ErrorHandler.LogError("SoftwarePropertyPage", "Error getting operator_id for " + operatorName + ":\r\n" + ex.Message);
                returnValue = 0;
            }

            return returnValue;
        }

        /// <summary>
        /// Converts a MM/dd/yyyy string into a datetime object
        /// </summary>
        /// <param name="dateToConvert">The MM/dd/yyyy string to convert</param>
        /// <returns></returns>
        private DateTime ConvertStringToDate(string dateToConvert) {
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            return DateTime.ParseExact(dateToConvert, "MM/dd/yyyy", provider);
        }

        /// <summary>
        /// Makes all controls on the form read-only
        /// </summary>
        private void MakeAllControlsReadOnly() {
            //buttons
            btnSignCrb.Visible = false;
            btnSignGatekeeper.Visible = false;
            btnSignProgrammer.Visible = false;
            btnSignQe.Visible = false;
            btnSignReviewer.Visible = false;
            btnSignSafety.Visible = false;
            btnSignSupervisor.Visible = false;
            btnSignUser.Visible = false;

            //checkboxes and radio buttons
            //purposely leaving out the 'Active' checkbox, which should always allow changes.
            chkNewSoftware.Enabled = false;
            chkChangeNotice.Enabled = false;
            rbBusiness.Enabled = false;
            rbEngineering.Enabled = false;
            rbProduct.Enabled = false;
            rbSupport.Enabled = false;

            //datetimepickers
            dtpRequested.Enabled = false;
            dtpEstimate.Enabled = false;
            dtpTestDate.Enabled = false;
            dtpReleased.Enabled = false;

            //text box sigs are already readonly
            txtSigCrb.Enabled = false;
            txtSigProgrammer.Enabled = false;
            txtSigQe.Enabled = false;
            txtSigReviewer.Enabled = false;
            txtSigSafety.Enabled = false;
            txtSigSupervisor.Enabled = false;
            txtSigUser.Enabled = false;
            txtGatekeeper.Enabled = false;

            //text box dates are already readonly
            txtDateCrb.Enabled = false;
            txtDateProgrammer.Enabled = false;
            txtDateQe.Enabled = false;
            txtDateReviewer.Enabled = false;
            txtDateSafety.Enabled = false;
            txtDateSupervisor.Enabled = false;
            txtDateUser.Enabled = false;

            //text boxes
            txtUsedInAreas.Enabled = false;
            txtFileName.Enabled = false;
            txtRequestedBy.Enabled = false;
            txtRequesterOrg.Enabled = false;
            txtProgrammer.Enabled = false;
            txtProgrammerOrg.Enabled = false;
            txtChangeDescription.Enabled = false;
            txtJustification.Enabled = false;
            txtProposedTesting.Enabled = false;
            txtTestedBy.Enabled = false;
            txtResults.Enabled = false;
            txtOcrNumber.Enabled = false;
            txtBackupLocation.Enabled = false;
            txtDocumentLocation.Enabled = false;
            txtRevision.Enabled = false;

            

        }

        /// <summary>
        /// event handler for any data changes on the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataChanged(object sender, EventArgs e) {
            extSite.SetModifiedFlag();
        }

        private void btnSign_Click(object sender, EventArgs e) {
            //check for programmer, user, reviewer, and qe sigs before continuing
            if (((Button)sender).Tag.ToString().Equals("Gatekeeper")) {
                if (string.IsNullOrEmpty(txtSigProgrammer.Text) || string.IsNullOrEmpty(txtSigUser.Text)
                    || string.IsNullOrEmpty(txtSigReviewer.Text) || string.IsNullOrEmpty(txtSigQe.Text)) {
                    ErrorHandler.LogError("SoftwarePropertyPage", "Gatekeeper cannot sign until at least programmer, user, reviewer, and qe signatures are complete.");
                    return;
                }
            }

            string todayIs = DateTime.Today.ToString("MM/dd/yyyy");
            string userName, password, role, signatureName;
            signatureName = string.Empty;
            role = ((Button)sender).Tag.ToString();

            SoftwarePropertyCredentialsForm spcf = new SoftwarePropertyCredentialsForm(_connection, role);
            spcf.ShowDialog();
            
            userName = spcf.GetUserName();
            password = spcf.GetPassword();
            
            //if the cancel button on the form is clicked, the username will be string.empty
            if (userName.Equals(string.Empty)) {
                return;
            }
            
            //using the supplied credentials, create a new connection, query for operator_id of username having correct role
            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder(_connectionString);
            ocsb.UserID = userName;
            ocsb.Password = password;
            OracleConnection testConnection = new OracleConnection(ocsb.ToString());
            string sqlString = "select o.name "
                + "from lims_sys.operator o, lims_sys.operator_role j, lims_sys.lims_role r "
                + "where o.operator_id = j.operator_id "
                + "and j.role_id = r.role_id "
                + "and upper(o.database_name) = upper(:username) "
                + "and r.name = :rolename ";
            OracleCommand commandGetId = new OracleCommand(sqlString, testConnection);
            OracleParameter p1 = new OracleParameter(":username", userName);
            OracleParameter p2 = new OracleParameter(":rolename", role);
            commandGetId.Parameters.Add(p1);
            commandGetId.Parameters.Add(p2);

            try {
                testConnection.Open();
                signatureName = (string)(OracleString)commandGetId.ExecuteOracleScalar();
                testConnection.Close();
            } catch (OracleException oex) {
                switch (oex.Code) {
                    case 1017:
                        ErrorHandler.LogError("SoftwarePropertyPage", "Invalid username/password.");
                        break;
                    case 933:
                        ErrorHandler.LogError("SoftwarePropertyPage", "Invalid SQL encountered.  Please contact the programmer.");
                        testConnection.Close();
                        break;
                    case 28001:
                        ErrorHandler.LogError("SoftwarePropertyPage", userName + " cannot sign because the password is expired.");
                        testConnection.Close();
                        break;
                    default:
                        ErrorHandler.LogError("SoftwarePropertyPage", "Oracle exception thrown:\r\n" + oex.Code);
                        break;
                }
                return;

            } catch (NullReferenceException) {
                ErrorHandler.LogError("SoftwarePropertyPage", userName + " does not have the " + role + " role in Nautilus and cannot sign.");
                testConnection.Close();
                return;
            } catch (Exception ex) {
                ErrorHandler.LogError("SoftwarePropertyPage", "Error encountered:\r\n" + ex.Message);
                if (testConnection.State == ConnectionState.Open) {
                    testConnection.Close();
                }
                return;
            }

            //update the appropriate text box with the user's name
            switch (role) {
                case "Programmer/Reviewer":
                    if (((Button)sender).Name == "btnSignProgrammer") {
                        txtSigProgrammer.Text = signatureName;
                        txtDateProgrammer.Text = todayIs;
                        btnSignProgrammer.Visible = false;
                    } else {
                        txtSigReviewer.Text = signatureName;
                        txtDateReviewer.Text = todayIs;
                        btnSignReviewer.Visible = false;
                    }
                    break;
                case "User":
                    txtSigUser.Text = signatureName;
                    txtDateUser.Text = todayIs;
                    btnSignUser.Visible = false;
                    break;
                case "QE":
                    txtSigQe.Text = signatureName;
                    txtDateQe.Text = todayIs;
                    btnSignQe.Visible = false;
                    break;
                case "Supervisor":
                    txtSigSupervisor.Text = signatureName;
                    txtDateSupervisor.Text = todayIs;
                    btnSignSupervisor.Visible = false;
                    break;
                case "Safety":
                    txtSigSafety.Text = signatureName;
                    txtDateSafety.Text = todayIs;
                    btnSignSafety.Visible = false;
                    break;
                case "CRB":
                    txtSigCrb.Text = signatureName;
                    txtDateCrb.Text = todayIs;
                    btnSignCrb.Visible = false;
                    break;
                case "Gatekeeper":
                    txtGatekeeper.Text = signatureName;
                    btnSignGatekeeper.Visible = false;
                    break;
                default:
                    break;
            }
        }


        public int GetVersion() {
            return VERSION;
        }
    }
}
