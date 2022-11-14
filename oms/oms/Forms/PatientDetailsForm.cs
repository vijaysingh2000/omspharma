using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using oms.DataAccessLayer;
using oms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace oms.Forms
{
    public partial class PatientDetailsForm : Form
    {
        public Patient WorkingPatient = null;
        public bool newPatient = false;
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;

        public PatientDetailsForm(Guid patiendId)
        {
            this.InitializeComponent();

            this.WorkingPatient = PatientsDL.Get(patiendId);

            if (this.WorkingPatient == null)
            {
                this.newPatient = true;
                this.WorkingPatient = new Patient() { Id = Guid.NewGuid(), DefaultAddressType = 1 };
            }
        }

        public PatientDetailsForm()
        {
            InitializeComponent();

            this.newPatient = true;
            this.WorkingPatient = new Patient() { Id = Guid.NewGuid(), DefaultAddressType = 1 };
        }

        private void PatientDetailsForm_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogTextWithUser("Patient Details");
            this.LoadGrid();
            this.Set();
            this.LoadData();
        }

        private void Set()
        {
            List<BasicModel> insurances = ListDL.GetActive(Constants.TableName_Insurance);

            txtMRN.Text = this.WorkingPatient.MRN;
            txtDOB.Value = this.WorkingPatient.DOB;
            txtFirstName.Text = this.WorkingPatient.FirstName;
            txtLastName.Text = this.WorkingPatient.LastName;
            txtEmail.Text = this.WorkingPatient.Email;
            txtPhone1.Text = this.WorkingPatient.Phone1;
            txtPhone2.Text = this.WorkingPatient.Phone2;
            txtInsurance.DataSource = insurances;
            txtInsurance.DisplayMember = "Name";
            txtInsurance.SelectedItem = insurances.FirstOrDefault(x => x.Id.Equals(this.WorkingPatient.InsuranceId));
            radAddress1.Checked = this.WorkingPatient.DefaultAddressType == 1;
            radAddress2.Checked = this.WorkingPatient.DefaultAddressType == 2;
            radAddress3.Checked = this.WorkingPatient.DefaultAddressType == 3;
            txtAddress1.Text = this.WorkingPatient.Address1;
            txtAddress2.Text = this.WorkingPatient.Address2;
            txtAddress3.Text = this.WorkingPatient.Address3;
            txtGuardian.Text = this.WorkingPatient.GuardianDetails;
        }

        private void Get()
        {
            string patientPath = PathDL.GetPatientPath(this.WorkingPatient.Id, true);
            List<string> files = new List<string>();
            XElement items = new XElement("notes");
            foreach (DataRow dataRow in this.dataTable.Rows)
            {
                XElement item = new XElement("item");
                string fileName = CommonFunctions.GetStringSafely(dataRow["FileName"]);
                string path = CommonFunctions.GetStringSafely(dataRow["Path"]);
                string notes = CommonFunctions.GetStringSafely(dataRow["Notes"]);
                DateTime dateTime = CommonFunctions.GetDateTimeSafely(dataRow["Date"]);

                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(fileName))
                {
                    string newPath = Path.Combine(patientPath, fileName);

                    if (!path.IsStringEqual(newPath))
                        File.Copy(path, newPath, true);

                    path = newPath;
                    files.Add(newPath.Trim().ToLower());
                }

                item.SetAttributeValue("date", dateTime.ToString());
                item.SetAttributeValue("filename", fileName);
                item.SetAttributeValue("path", path);
                item.SetAttributeValue("notes", notes);

                items.Add(item);
            }

            CommonFunctions.DeleteUnwantedFiles(patientPath, files);

            this.WorkingPatient.MRN = txtMRN.Text;
            this.WorkingPatient.DOB = txtDOB.Value;
            this.WorkingPatient.FirstName = txtFirstName.Text;
            this.WorkingPatient.LastName = txtLastName.Text;
            this.WorkingPatient.Email = txtEmail.Text;
            this.WorkingPatient.Phone1 = txtPhone1.Text;
            this.WorkingPatient.Phone2 = txtPhone2.Text;
            this.WorkingPatient.InsuranceId = txtInsurance.SelectedItem != null ? ((BasicModel)txtInsurance.SelectedItem).Id : 0;
            this.WorkingPatient.DefaultAddressType = radAddress1.Checked ? 1 : this.WorkingPatient.DefaultAddressType;
            this.WorkingPatient.DefaultAddressType = radAddress2.Checked ? 2 : this.WorkingPatient.DefaultAddressType;
            this.WorkingPatient.DefaultAddressType = radAddress3.Checked ? 3 : this.WorkingPatient.DefaultAddressType;
            this.WorkingPatient.Address1 = txtAddress1.Text;
            this.WorkingPatient.Address2 = txtAddress2.Text;
            this.WorkingPatient.Address3 = txtAddress3.Text;
            this.WorkingPatient.GuardianDetails = txtGuardian.Text;
            this.WorkingPatient.Notes = items.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtInsurance.SelectedItem == null)
            {
                CommonFunctions.ShowErrorMessage("Insurance need to selected to add patient !!!");
                txtInsurance.Focus();
                return;
            }

            Patient mrnExists = PatientsDL.Get(txtMRN.Text);

            if(mrnExists != null)
            {
                if (newPatient || !this.WorkingPatient.Id.Equals(mrnExists.Id))
                {
                    CommonFunctions.ShowErrorMessage("Patient with same MRN already exists.");
                    return;
                }
            }

            this.Get();

            int result = PatientsDL.AddOrUpdate(new List<Patient>() { this.WorkingPatient });
            if (result > 0)
                CommonFunctions.ShowInfomationMessage("Patient Updated Successfully.");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Date", typeof(DateTime));
            this.dataTable.Columns.Add("Notes", typeof(string));
            this.dataTable.Columns.Add("Path", typeof(string));
            this.dataTable.Columns.Add("FileName", typeof(string));
            this.dataTable.Columns.Add("Attach", typeof(string));
            this.dataTable.Columns.Add("Delete", typeof(string));

            this.gridControl = new GridControl();
            this.splitContainer1.Panel2.Controls.Add(this.gridControl);
            this.gridControl.DataSource = this.dataTable;
            this.gridControl.Dock = DockStyle.Fill;
            this.gridControl.Visible = true;

            this.gridView = new GridView();
            this.gridControl.MainView = this.gridView;
            BaseView[] views = new BaseView[] { this.gridView };
            this.gridControl.ViewCollection.AddRange(views);

            this.gridView.GridControl = this.gridControl;
            this.gridView.PopulateColumns();

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Attach"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Attach_Click);

            RepositoryItemHyperLinkEdit editLink2 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["FileName"].ColumnEdit = editLink2;
            editLink2.Click += new EventHandler(this.View_Click);

            RepositoryItemHyperLinkEdit editLink3 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Delete"].ColumnEdit = editLink3;
            editLink3.Click += new EventHandler(this.Delete_Click);

            this.gridView.Columns["Path"].Visible = false;
            this.gridView.Columns["Path"].OptionsColumn.ReadOnly = true;

            this.gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

            this.gridView.CellValueChanged += ((s, e) =>
            {
                if (e.Column.GetTextCaption().IsStringEqual("Amount"))
                {
                    this.gridView.UpdateCurrentRow();
                }
            });

            this.gridView.InitNewRow += ((s, e) =>
            {
                GridView view = s as GridView;
                view.SetRowCellValue(e.RowHandle, view.Columns["Date"], DateTime.Now);
                view.SetRowCellValue(e.RowHandle, view.Columns["Attach"], "Attach");
                view.SetRowCellValue(e.RowHandle, view.Columns["Delete"], "Delete");
            });
        }

        private void View_Click(object sender, EventArgs e)
        {
            DataRow row = this.gridView.GetFocusedDataRow();

            if (row != null && row["Path"] != null && !string.IsNullOrEmpty(row["Path"].ToString()))
            {
                string fileName = row["Path"].ToString();
                CommonFunctions.OpenFile(fileName);
            }
        }

        private void Attach_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Title = "Attachment";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DataRow row = this.gridView.GetFocusedDataRow();
                if (row == null)
                {
                    row = this.dataTable.Rows.Add(DateTime.Now, string.Empty, string.Empty, string.Empty, "Attach", "Delete");
                    this.gridControl.RefreshDataSource();
                }

                row["FileName"] = Path.GetFileName(dialog.FileName);
                row["Path"] = dialog.FileName;
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = this.gridView.GetFocusedDataRow();
                if (row != null)
                {
                    this.dataTable.Rows.Remove(row);
                    this.gridControl.RefreshDataSource();
                    this.gridControl.Refresh();
                }
            }
            catch
            {
                this.gridControl.RefreshDataSource();
                this.gridControl.Refresh();
            }
        }

        public void LoadData()
        {
            XElement elements = CommonFunctions.GetXmlSafely(this.WorkingPatient.Notes);

            IEnumerable<XElement> items = elements.Elements("item");
            this.dataTable.Rows.Clear();
            foreach (XElement item in items)
            {
                this.dataTable.Rows.Add(
                    CommonFunctions.GetDateTimeSafely(item.GetAttribute("date")),
                    item.GetAttribute("notes"),
                    item.GetAttribute("path"),
                    item.GetAttribute("filename"),
                    "Attach",
                    "Delete"
                    );
            }

            this.gridControl.RefreshDataSource();
            this.gridControl.Refresh();
        }
    }
}
