using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using oms.DataAccessLayer;
using oms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace oms.Forms
{
    public partial class PatientsForm : Form
    {
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;

        public PatientsForm()
        {
            InitializeComponent();
        }

        private void btnNewPatient_Click(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogText("Patients");

            PatientDetailsForm patientDetailsForm = new PatientDetailsForm();

            if (patientDetailsForm.ShowDialog() == DialogResult.OK)
            {
                this.LoadData();
            }
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Id", typeof(string));
            this.dataTable.Columns.Add("MRN", typeof(string));
            this.dataTable.Columns.Add("Name", typeof(string));
            this.dataTable.Columns.Add("Date of Birth", typeof(DateTime));
            this.dataTable.Columns.Add("Email", typeof(string));
            this.dataTable.Columns.Add("Insurance", typeof(string));
            this.dataTable.Columns.Add("Orders In Progress", typeof(string));
            this.dataTable.Columns.Add("Orders Completed", typeof(string));
            this.dataTable.Columns.Add("Orders", typeof(string));


            this.gridControl = new GridControl();
            this.groupBox1.Controls.Add(this.gridControl);
            this.gridControl.DataSource = this.dataTable;
            this.gridControl.Dock = DockStyle.Fill;
            this.gridControl.Visible = true;

            this.gridView = new GridView();
            this.gridControl.MainView = this.gridView;
            BaseView[] views = new BaseView[] { this.gridView };
            this.gridControl.ViewCollection.AddRange(views);

            this.gridView.GridControl = this.gridControl;
            this.gridView.PopulateColumns();

            RepositoryItemHyperLinkEdit editLink = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["MRN"].ColumnEdit = editLink;
            editLink.Click += new EventHandler(this.Details_Click);

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Orders"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Orders_Click);

            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView.Columns["Id"].Visible = false;
            this.gridView.OptionsBehavior.ReadOnly = true;
        }

        private void Details_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid patientId = new Guid(dataRow["Id"].ToString());

            PatientDetailsForm patientDetailsForm = new PatientDetailsForm(patientId);
            if(patientDetailsForm.ShowDialog() == DialogResult.OK)
            {
                this.LoadData();
            }
        }

        private void Orders_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid patientId = new Guid(dataRow["Id"].ToString());

            OrdersForm orders = new OrdersForm(patientId);
            orders.ShowDialog();
            this.LoadData();
        }

        private void LoadData()
        {
            List<Patient> patients = PatientsDL.Get();
            this.dataTable.Rows.Clear();
            foreach (Patient patient in patients)
            {
                dataTable.Rows.Add(patient.Id, patient.MRN, patient.LastName + ", " + patient.FirstName, patient.DOB, patient.Email, patient.InsuranceName, patient.InProgress, patient.Completed, "View");
            }

            this.gridControl.RefreshDataSource();
            this.gridControl.Refresh();
        }

        private void PatientsForm_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogTextWithUser("Patient List");

            this.LoadGrid();
            this.LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
