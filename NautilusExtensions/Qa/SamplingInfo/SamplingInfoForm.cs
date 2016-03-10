using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;
using NautilusExtensions.All;

namespace NautilusExtensions.Qa {
    public partial class SamplingInfoForm : Form {

        OracleConnection _connection;
        string _operatorName;
        int _sampleId;
        char _sampleStatus;
        bool _changesMade;

        public SamplingInfoForm(OracleConnection connection, string operatorName, int sampleId) {
            InitializeComponent();
            _connection = connection;
            _operatorName = operatorName;
            _sampleId = sampleId;
        }


        /// <summary>
        /// When the form loads, populates with sampling info data for given sample.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SamplingInfoForm_Load(object sender, EventArgs e) {

            string sqlString = "select status, name from lims_sys.sample where sample_id = :in_sample_id ";
            OracleCommand command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_sample_id", _sampleId));
            OracleDataReader reader;

            try {
                reader = command.ExecuteReader();

                while (reader.Read()) {
                    _sampleStatus = reader["status"].ToString()[0];
                    pbStatus.ImageLocation = @"\\tpapps\apps\nautilus\custom\images\s" + _sampleStatus + ".gif";
                    lblSampleName.Text = reader["name"].ToString();
                    this.Text = "Sampling Info: " + reader["name"].ToString();
                }

                reader.Close();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "SamplingInfoForm", "Error getting sample name/status for " + _sampleId + ":\r\n" + ex.Message);
                return;
            }

            sqlString = "select * from lims_sys.u_sampling_info_user where u_sample_id = :in_sample_id";
            command = new OracleCommand(sqlString, _connection);
            command.Parameters.Add(new OracleParameter(":in_sample_id", _sampleId));
            int samplingInfoId;
            string[] rowToAdd;

            try {
                reader = command.ExecuteReader();

                if (reader.HasRows) {
                    //disable create new button
                    btnCreateRecord.Visible = false;

                    reader.Read();
                    chkNeedsAttention.Checked = reader["u_needs_attention"].ToString().Equals("T");
                    txtSl.Text = reader["u_sl_number"].ToString();
                    txtColor.Text = reader["u_color"].ToString();
                    txtComments.Text = reader["u_comments"].ToString();
                    txtContainerType.Text = reader["u_container_type"].ToString();
                    txtDamage.Text = reader["u_damage"].ToString();
                    txtDoM.Text = reader["u_dom"].ToString();
                    txtLabLocation.Text = reader["u_lab_location"].ToString();
                    txtMaterialName.Text = reader["u_material_name"].ToString();
                    txtMiscNotes.Text = reader["u_misc_notes"].ToString();
                    txtPdlNoRev.Text = reader["u_pdl_rev"].ToString();
                    txtPoNumber.Text = reader["u_po_number"].ToString();
                    txtQtyUm.Text = reader["u_um"].ToString();
                    txtReceiver.Text = reader["u_receiver_number"].ToString();
                    txtScnCn.Text = reader["u_scn_cn"].ToString();
                    txtSpecRev.Text = reader["u_revision"].ToString();
                    txtStoresLocation.Text = reader["u_stores_location"].ToString();
                    txtType.Text = reader["u_type"].ToString();
                    txtVendor.Text = reader["u_vendor"].ToString();
                    txtVendorLot.Text = reader["u_vendor_lot"].ToString();
                    txtWeight.Text = reader["u_weight"].ToString();

                    samplingInfoId = reader.GetInt32(reader.GetOrdinal("u_sampling_info_id"));

                    reader.Close();

                    //new query for grid items (containers)
                    sqlString = "select * from lims_sys.u_sampling_container_user where u_sampling_info_id = :in_sampling_info_id ";

                    command = new OracleCommand(sqlString, _connection);
                    command.Parameters.Add(new OracleParameter(":in_sampling_info_id", samplingInfoId));
                    reader = command.ExecuteReader();

                    while (reader.Read()) {
                        rowToAdd = new string[4] {reader["u_sampling_container_id"].ToString(),
                                    reader["u_container_description"].ToString(),
                                    reader["u_qty"].ToString(),
                                    reader["u_location"].ToString()};
                        dgvContainers.Rows.Add(rowToAdd);
                    }

                    reader.Close();

                    //verify the sample status.  If A, R, X, or S, controls must be disabled except the check box and misc/notes
                    if (_sampleStatus.Equals('A') || _sampleStatus.Equals('R') || _sampleStatus.Equals('X') || _sampleStatus.Equals('S')) {
                        EnableControls(false);
                        btnCreateRecord.Visible = false;
                        dgvContainers.Enabled = false;
                        dgvContainers.DefaultCellStyle.BackColor = Color.LightGray;

                        //re-enable the needs attention and misc/notes controls.  Per the lab, these should be updateable fields, regardless of status.
                        chkNeedsAttention.Enabled = true;
                        txtMiscNotes.Enabled = true;
                    }
                } else {
                    //if there are no rows returned, disable everything.
                    EnableControls(false);
                    dgvContainers.Enabled = false;
                    dgvContainers.DefaultCellStyle.BackColor = Color.LightGray;

                    //verify the sample status.  If A, R, or S, do not allow creation of sampling info record.
                    if (_sampleStatus.Equals('A') || _sampleStatus.Equals('R') || _sampleStatus.Equals('X') || _sampleStatus.Equals('S')) {
                        btnCreateRecord.Enabled = false;
                    }
                }

            } catch (Exception ex) {
                ErrorHandler.LogError("SamplingInfoPage", "Error getting data for sample" + _sampleId + ":\r\n" + ex.Message);
                this.Close();
                return;
            }

            _changesMade = false;
        }


        /// <summary>
        /// Close form without saving to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }


        /// <summary>
        /// Save the data to database and close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e) {
            SaveChanges();
            this.Close();
        }


        /// <summary>
        /// Creates records in the sampling info and container tables.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateRecord_Click(object sender, EventArgs e) {

            long newSamplingId;
            string sqlString;
            OracleCommand sequenceCommand;
            OracleCommand insertCommand;
            OracleTransaction transaction;

            //get the next sampling_id number from the oracle sequencer
            sqlString = "select lims.sq_u_sampling_info.nextval from dual ";
            try {
                sequenceCommand = new OracleCommand(sqlString, _connection);
                newSamplingId = (long)(OracleNumber)sequenceCommand.ExecuteOracleScalar();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "SamplingInfoForm", "Error retrieving sequence value for new sampling info record:\r\n" + ex.Message);
                return;
            }

            //two inserts
            transaction = _connection.BeginTransaction();

            try {
                //insert a record into the parent table
                sqlString = "insert into lims_sys.u_sampling_info(u_sampling_info_id, name) values(:in_sampling_info_id, :in_sampling_info_name) ";
                insertCommand = new OracleCommand(sqlString, _connection, transaction);
                insertCommand.Parameters.Add(new OracleParameter(":in_sampling_info_id", newSamplingId));
                insertCommand.Parameters.Add(new OracleParameter(":in_sampling_info_name", newSamplingId.ToString()));
                insertCommand.ExecuteNonQuery();

                //insert a record into the user table
                sqlString = "insert into lims_sys.u_sampling_info_user(u_sampling_info_id, u_sample_id) values(:in_sampling_info_id, :in_sample_id) ";
                insertCommand = new OracleCommand(sqlString, _connection, transaction);
                insertCommand.Parameters.Add(new OracleParameter(":in_sampling_info_id", newSamplingId));
                insertCommand.Parameters.Add(new OracleParameter(":in_sample_id", _sampleId));
                insertCommand.ExecuteNonQuery();

                transaction.Commit();

            } catch (Exception ex) {
                ErrorHandler.LogError("Sampling info record could not be created.  Error inserting records in sampling_info tables:\r\n" + ex.Message);
                transaction.Rollback();
                return;
            }

            //enable all the text boxes
            EnableControls(true);
            dgvContainers.Enabled = true;
            dgvContainers.DefaultCellStyle.BackColor = SystemColors.Window;

            //the button the user just clicked should be disabled now.
            btnCreateRecord.Visible = false;

        }


        /// <summary>
        /// Enables or disables controls depending on value of enable
        /// </summary>
        /// <param name="enable">Enable or disable controls.</param>
        private void EnableControls(bool enable) {
            chkNeedsAttention.Enabled = enable;
            txtSl.Enabled = enable;
            txtColor.Enabled = enable;
            txtComments.Enabled = enable;
            txtContainerType.Enabled = enable;
            txtDamage.Enabled = enable;
            txtDoM.Enabled = enable;
            txtLabLocation.Enabled = enable;
            txtMaterialName.Enabled = enable;
            txtMiscNotes.Enabled = enable;
            txtPdlNoRev.Enabled = enable;
            txtPoNumber.Enabled = enable;
            txtQtyUm.Enabled = enable;
            txtReceiver.Enabled = enable;
            txtScnCn.Enabled = enable;
            txtSpecRev.Enabled = enable;
            txtSpecRev.Enabled = enable;
            txtStoresLocation.Enabled = enable;
            txtType.Enabled = enable;
            txtVendor.Enabled = enable;
            txtVendorLot.Enabled = enable;
            txtWeight.Enabled = enable;
        }


        /// <summary>
        /// Starts browser process at URL of Sampler Final Report, with sample ID passed as a q= parameter.  Prompts to apply changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblSampleName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

            //prompt for saving changes with yes/no/cancel, which will save/not save/cancel the report operation, respectively
            if (_changesMade) {
                DialogResult dr = MessageBox.Show("Do you want to apply changes you've made before viewing the report?", "Apply Changes", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Cancel) {
                    return;
                } else if (dr == DialogResult.Yes) {
                    SaveChanges();
                }
            }

            string sqlString = "select rsu.u_url "
                + "from lims_sys.u_reporting_service rs, lims_sys.u_reporting_service_user rsu "
                + "where rs.u_reporting_service_id = rsu.u_reporting_service_id "
                + "and rs.name = 'Sampler Final Report' ";
            OracleCommand command = new OracleCommand(sqlString, _connection);
            string url;

            try {
                url = command.ExecuteScalar().ToString();
            } catch (Exception ex) {
                ErrorHandler.LogError(_operatorName, "SamplingInfoForm", "Error getting URL for sampler final report:\r\n" + ex.Message);
                return;
            }

            System.Diagnostics.Process.Start(url + "&q=" + _sampleId);

        }


        /// <summary>
        /// Saves changes made on the form to the database.
        /// </summary>
        private void SaveChanges() {

            //end the editing of any datagridviewcells, if they are in edit mode, getting their value will throw an exception.
            try {
                dgvContainers.EndEdit();
            } catch {
                //do nothing
            }

            //sampling_info update
            string sqlUpdate = "update lims_sys.u_sampling_info_user set "
                + "u_color = :in_color, "
                + "u_comments = :in_comments, "
                + "u_damage = :in_damage, "
                + "u_dom = :in_dom, "
                + "u_lab_location = :in_lab_location, "
                + "u_material_name = :in_material_name, "
                + "u_misc_notes = :in_misc_note, "
                + "u_pdl_rev = :in_pdl_rev, "
                + "u_po_number = :in_po_number, "
                + "u_receiver_number = :in_receiver_number, "
                + "u_revision = :in_revision, "
                + "u_scn_cn = :in_scn_cn, "
                + "u_sl_number = :in_sl_number, "
                + "u_stores_location = :in_stor_in_loc, "
                + "u_type = :in_type, "
                + "u_vendor = :in_vendor, "
                + "u_vendor_lot = :in_vendor_lot, "
                + "u_weight = :in_weight, "
                + "u_container_type = :in_container_type, "
                + "u_um = :in_um, "
                + "u_needs_attention = :in_needs_attention "
                + "where u_sample_id = :in_sample_id";

            OracleTransaction transaction = _connection.BeginTransaction();
            OracleCommand command = new OracleCommand(sqlUpdate, _connection, transaction);
            command.Parameters.Add(new OracleParameter(":in_color", txtColor.Text));
            command.Parameters.Add(new OracleParameter(":in_comments", txtComments.Text));
            command.Parameters.Add(new OracleParameter(":in_damage", txtDamage.Text));
            command.Parameters.Add(new OracleParameter(":in_dom", txtDoM.Text));
            command.Parameters.Add(new OracleParameter(":in_lab_location", txtLabLocation.Text));
            command.Parameters.Add(new OracleParameter(":in_material_name", txtMaterialName.Text));
            command.Parameters.Add(new OracleParameter(":in_misc_note", txtMiscNotes.Text));
            command.Parameters.Add(new OracleParameter(":in_pdl_rev", txtPdlNoRev.Text));
            command.Parameters.Add(new OracleParameter(":in_po_number", txtPoNumber.Text));
            command.Parameters.Add(new OracleParameter(":in_receiver_number", txtReceiver.Text));
            command.Parameters.Add(new OracleParameter(":in_revision", txtSpecRev.Text));
            command.Parameters.Add(new OracleParameter(":in_scn_cn", txtScnCn.Text));
            command.Parameters.Add(new OracleParameter(":in_sl_number", txtSl.Text));
            command.Parameters.Add(new OracleParameter(":in_stor_in_loc", txtStoresLocation.Text));
            command.Parameters.Add(new OracleParameter(":in_type", txtType.Text));
            command.Parameters.Add(new OracleParameter(":in_vendor", txtVendor.Text));
            command.Parameters.Add(new OracleParameter(":in_vendor_lot", txtVendorLot.Text));
            command.Parameters.Add(new OracleParameter(":in_weight", txtWeight.Text));
            command.Parameters.Add(new OracleParameter(":in_sample_id", _sampleId));
            command.Parameters.Add(new OracleParameter(":in_container_type", txtContainerType.Text));
            command.Parameters.Add(new OracleParameter(":in_um", txtQtyUm.Text));
            command.Parameters.Add(new OracleParameter(":in_needs_attention", chkNeedsAttention.Checked ? "T" : "F"));

            try {
                command.ExecuteNonQuery();
                transaction.Commit();
            } catch (Exception ex) {
                transaction.Rollback();
                ErrorHandler.LogError("Error updating sampling info table.  Data have not been saved:\r\n" + ex.Message);
                return;
            }

            //container table updates
            OracleCommand commandSequence, commandContainer;
            string sqlSequence, sqlContainer;
            long newId;
            try {
                transaction = _connection.BeginTransaction();
                foreach (DataGridViewRow dgvr in dgvContainers.Rows) {

                    //The "*" new row in the datagridview needs to be explicitly ignored.
                    if (dgvr.IsNewRow) continue;

                    //new rows will have ID set to "0" by the rows_added event handler, those must be inserted
                    //existing rows will have an ID, those must be updated
                    if (dgvr.Cells[0].Value == null) {
                        newId = 0;
                    } else {
                        long.TryParse(dgvr.Cells[0].Value.ToString(), out newId);
                    }

                    if (newId == 0) {
                        sqlSequence = "select lims.sq_u_sampling_container.nextval from dual";
                        commandSequence = new OracleCommand(sqlSequence, _connection, transaction);
                        newId = (long)(OracleNumber)commandSequence.ExecuteOracleScalar();

                        //insert the record in the parent table
                        sqlContainer = "insert into lims_sys.u_sampling_container(u_sampling_container_id, name, version, version_status) "
                            + "values(:in_id, :in_name, 1, 'A') ";
                        commandContainer = new OracleCommand(sqlContainer, _connection, transaction);
                        commandContainer.Parameters.Add(new OracleParameter(":in_id", newId));
                        commandContainer.Parameters.Add(new OracleParameter(":in_name", newId.ToString()));
                        commandContainer.ExecuteNonQuery();

                        //also insert the record in the user table
                        sqlContainer = "insert into lims_sys.u_sampling_container_user(u_sampling_container_id, u_container_description, u_qty, u_location, u_sampling_info_id) "
                            + "values(:in_id, :in_container_description, :in_qty, :in_location, "
                            + "(select u_sampling_info_id from lims_sys.u_sampling_info_user where u_sample_id = :in_sample_id)) ";

                        commandContainer = new OracleCommand(sqlContainer, _connection, transaction);
                        commandContainer.Parameters.Add(new OracleParameter(":in_id", newId));
                        commandContainer.Parameters.Add(new OracleParameter(":in_container_description", dgvr.Cells[1].FormattedValue.ToString()));
                        commandContainer.Parameters.Add(new OracleParameter(":in_qty", dgvr.Cells[2].FormattedValue.ToString()));
                        commandContainer.Parameters.Add(new OracleParameter(":in_location", dgvr.Cells[3].FormattedValue.ToString()));
                        commandContainer.Parameters.Add(new OracleParameter(":in_sample_id", _sampleId));
                        commandContainer.ExecuteNonQuery();

                        //update the existing cell value so that subsequent Apply clicks will do an update now instead of insert
                        dgvr.Cells[0].Value = newId.ToString();
                    } else {
                        //update the container table for rows with an ID already assigned
                        sqlContainer = "update lims_sys.u_sampling_container_user set "
                            + "u_container_description = :in_container_description, "
                            + "u_qty = :in_qty, "
                            + "u_location = :in_location "
                            + "where u_sampling_container_id = :in_sampling_container_id ";

                        commandContainer = new OracleCommand(sqlContainer, _connection, transaction);
                        commandContainer.Parameters.Add(new OracleParameter(":in_container_description", dgvr.Cells[1].FormattedValue.ToString()));
                        commandContainer.Parameters.Add(new OracleParameter(":in_qty", dgvr.Cells[2].FormattedValue.ToString()));
                        commandContainer.Parameters.Add(new OracleParameter(":in_location", dgvr.Cells[3].FormattedValue.ToString()));
                        commandContainer.Parameters.Add(new OracleParameter(":in_sampling_container_id", dgvr.Cells[0].FormattedValue.ToString()));
                        commandContainer.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            } catch (Exception ex) {
                transaction.Rollback();
                ErrorHandler.LogError("Error updating container table.  Container data have not been saved:\r\n" + ex.Message);
                return;
            }
        }


        /// <summary>
        /// Event handler that sets the modified flag when changes are made in the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChanged(object sender, EventArgs e) {
            _changesMade = true;
        }


        /// <summary>
        /// Event handler that sets the modified flag when changes are made in the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvContainers_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e) {
            _changesMade = true;
        }


    }
}
