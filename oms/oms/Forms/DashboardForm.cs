using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.VisualBasic;
using oms.DataAccessLayer;
using oms.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace oms.Forms
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
        }

        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;

        private void btnUsers_Click(object sender, EventArgs e)
        {
            UsersForm usersForm = new UsersForm();
            usersForm.ShowDialog();
        }

        private void btnMyProfile_Click(object sender, EventArgs e)
        {
            UserDetailsForm myProfileForm = new UserDetailsForm(ApplicationVariables.LoggedInUser.Id);
            myProfileForm.ShowDialog();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            this.LoadGrid();
            this.LoadClients();
        }

        private void LoadClients()
        {
            List<BasicModel> clients = ClienttDL.Get();
            this.cmbClients.DataSource = clients;
            this.cmbClients.DisplayMember = "Name";
            this.cmbClients.SelectedIndex = clients.IndexOf(clients.FirstOrDefault(x => x.Id.Equals(ApplicationVariables.WorkingClient.Id)));
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("OrderId", typeof(string));
            this.dataTable.Columns.Add("PatientId", typeof(string));
            this.dataTable.Columns.Add("MRN", typeof(string));
            this.dataTable.Columns.Add("Name", typeof(string));
            this.dataTable.Columns.Add("Order Number", typeof(string));
            this.dataTable.Columns.Add("Drug Name", typeof(string));
            this.dataTable.Columns.Add("Current State", typeof(string));
            this.dataTable.Columns.Add("Date of Service", typeof(DateTime));
            this.dataTable.Columns.Add("Estimated Delivery Date", typeof(DateTime));
            this.dataTable.Columns.Add("Billed", typeof(float));
            this.dataTable.Columns.Add("Payments", typeof(float));
            this.dataTable.Columns.Add("Balance", typeof(float));


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

            this.gridView.OptionsView.ShowFooter = true;

            RepositoryItemTextEdit editLinkBilled = new RepositoryItemTextEdit();
            this.gridView.Columns["Billed"].ColumnEdit = editLinkBilled;
            editLinkBilled.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            editLinkBilled.DisplayFormat.FormatString = "c2";
            this.gridView.Columns["Billed"].AppearanceCell.BackColor = Color.LightYellow;
            this.gridView.Columns["Billed"].AppearanceCell.ForeColor = Color.Black;
            this.gridView.Columns["Billed"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            this.gridView.Columns["Billed"].SummaryItem.DisplayFormat = "{0:c2}";

            RepositoryItemTextEdit editLinkPayments = new RepositoryItemTextEdit();
            this.gridView.Columns["Payments"].ColumnEdit = editLinkPayments;
            editLinkPayments.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            editLinkPayments.DisplayFormat.FormatString = "c2";
            this.gridView.Columns["Payments"].AppearanceCell.BackColor = Color.LightYellow;
            this.gridView.Columns["Payments"].AppearanceCell.ForeColor = Color.Black;
            this.gridView.Columns["Payments"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            this.gridView.Columns["Payments"].SummaryItem.DisplayFormat = "{0:c2}";

            RepositoryItemTextEdit editLinkBalance = new RepositoryItemTextEdit();
            this.gridView.Columns["Balance"].ColumnEdit = editLinkBalance;
            editLinkBalance.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            editLinkBalance.DisplayFormat.FormatString = "c2";
            this.gridView.Columns["Balance"].AppearanceCell.BackColor = Color.LightYellow;
            this.gridView.Columns["Balance"].AppearanceCell.ForeColor = Color.Black;
            this.gridView.Columns["Balance"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            this.gridView.Columns["Balance"].SummaryItem.DisplayFormat = "{0:c2}";

            RepositoryItemHyperLinkEdit editLink0 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Current State"].ColumnEdit = editLink0;
            editLink0.Click += new EventHandler(this.Current_State_Click);

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Order Number"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Order_Details_Click);

            RepositoryItemHyperLinkEdit editLink2 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["MRN"].ColumnEdit = editLink2;
            editLink2.Click += new EventHandler(this.Patient_Details_Click);

            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView.Columns["OrderId"].Visible = false;
            this.gridView.Columns["PatientId"].Visible = false;
            this.gridView.OptionsBehavior.ReadOnly = true;
        }

        private void Current_State_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid orderId = new Guid(dataRow["OrderId"].ToString());

            OrderTasks orderStatus = new OrderTasks(orderId);
            orderStatus.ShowDialog();
            this.LoadData();
        }

        private void Order_Details_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid patientId = new Guid(dataRow["PatientId"].ToString());
            Guid orderId = new Guid(dataRow["OrderId"].ToString());

            OrderDetails orderDetails = new OrderDetails(patientId, orderId);
            orderDetails.ShowDialog();
            this.LoadData();
        }

        private void Patient_Details_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid patientId = new Guid(dataRow["PatientId"].ToString());

            PatientDetailsForm patientDetailsForm = new PatientDetailsForm(patientId);
            patientDetailsForm.ShowDialog();
            this.LoadData();
        }

        private void LoadData()
        {
            List<OrderWithPatient> orders = OrdersDL.GetOrdersInProgress();

            this.dataTable.Rows.Clear();
            foreach (OrderWithPatient order in orders)
            {
                this.dataTable.Rows.Add(
                    order.Id,
                    order.PatientId,
                    order.MRN, 
                    order.LastName + ", " + order.FirstName, 
                    order.OrderNumber,
                    order.DrugName,
                    order.LastState,
                    order.DOS, 
                    order.EstimatedDeliveryDate,
                    order.TotalAmountBilled,
                    order.TotalPayments,
                    order.TotalBalance
                    );
            }

            this.gridControl.DataSource = this.dataTable;
            this.gridControl.RefreshDataSource();
        }

        public void LoadAlerts()
        {

        }

        private void btnNewPatient_Click(object sender, EventArgs e)
        {
            PatientDetailsForm patientDetailsForm = new PatientDetailsForm();
            patientDetailsForm.ShowDialog();
            this.LoadData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.LoadData();
        }

        private void btnPatientList_Click(object sender, EventArgs e)
        {
            PatientsForm patientsForm = new PatientsForm();
            patientsForm.ShowDialog();
            this.LoadData();
        }

        private void btnOrderList_Click(object sender, EventArgs e)
        {
            OrdersForm ordersForm = new OrdersForm();
            ordersForm.ShowDialog();
            this.LoadData();
        }

        private void btnGenerateCallList_Click(object sender, EventArgs e)
        {
            GenerateCallForm genCallForm = new GenerateCallForm();
            genCallForm.ShowDialog();
            this.LoadData();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ReportsForm reportsForm = new ReportsForm();
            reportsForm.ShowDialog();
            this.LoadData();
        }

        private void btnLists_Click(object sender, EventArgs e)
        {
            ListForm listForm = new ListForm();
            listForm.ShowDialog();
            this.LoadData();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            List<Patient> patients = PatientsDL.Get();
            List<BasicModel> drugs = ListDL.Get(Constants.TableName_Drug);
            List<BasicModel> insurances = ListDL.Get(Constants.TableName_Insurance);
            List<BasicModel> manufacturers = ListDL.Get(Constants.TableName_Manufacturer);

            string fileName = Path.Combine(ApplicationVariables.ImportDataFile);

            string[] lines = File.ReadAllLines(fileName);

            foreach (string line in lines)
            {
                string[] columns = line.Split('\t');

                if (columns.Length > 38)
                {
                    string drugName = columns[7];
                    BasicModel drug = drugs.FirstOrDefault(x => x.Name.IsStringEqual(drugName));
                    if (drug == null)
                    {
                        drug = new BasicModel() { Active = true };
                        drugs.Add(drug);
                    }
                    drug.Name = drugName;

                    string insuranceName = columns[13];
                    BasicModel insurance = insurances.FirstOrDefault(x => x.Name.IsStringEqual(insuranceName));
                    if (insurance == null)
                    {
                        insurance = new BasicModel() { Active = true };
                        insurances.Add(insurance);
                    }
                    insurance.Name = insuranceName;

                    string manufacturerName = columns[38];
                    BasicModel manufacturer = manufacturers.FirstOrDefault(x => x.Name.IsStringEqual(manufacturerName));
                    if (manufacturer == null)
                    {
                        manufacturer = new BasicModel() { Active = true };
                        manufacturers.Add(manufacturer);
                    }
                    manufacturer.Name = manufacturerName;

                    string mrn = columns[2].Trim();
                    Patient patient = patients.FirstOrDefault(x => x.MRN.IsStringEqual(mrn));
                    if (patient == null)
                    {
                        patient = new Patient() { Id = Guid.NewGuid(), MRN = mrn, DefaultAddressType = 1 };
                        patients.Add(patient);
                    }

                    string[] fullNameSplit = columns[3].Trim().TrimStart('"').TrimEnd('"').Split(',');
                    patient.LastName = fullNameSplit[0];
                    patient.FirstName = fullNameSplit.Length == 2 ? fullNameSplit[1] : patient.LastName;
                    patient.Address1 = columns[5].Trim().TrimStart('"').TrimEnd('"');
                    patient.InsuranceId = insurance.Id;
                    patient.LastName = patient.LastName.Replace('"', ' ').Trim();
                    patient.FirstName = patient.FirstName.Replace('"', ' ').Trim();
                    patient.InsuranceName = insurance.Name;
                }
            }

            ListDL.AddOrUpdate(Constants.TableName_Drug, drugs);
            ListDL.AddOrUpdate(Constants.TableName_Insurance, insurances);
            ListDL.AddOrUpdate(Constants.TableName_Manufacturer, manufacturers);

            insurances = ListDL.Get(Constants.TableName_Insurance);
            patients.ForEach(x =>
            {
                if(x.InsuranceId == 0)
                {
                    BasicModel insurance = insurances.FirstOrDefault(y => y.Name.IsStringEqual(x.InsuranceName));
                    if (insurance != null)
                        x.InsuranceId = insurance.Id;
                }
            });

            int result = PatientsDL.AddOrUpdate(patients);

            if(result > 0)
            {
                CommonFunctions.ShowInfomationMessage("Import completed.");
                this.LoadData();
            }

        }

        private void btnBatchPayment_Click(object sender, EventArgs e)
        {
            BatchPayments batchPayments = new BatchPayments();
            batchPayments.ShowDialog();
            this.LoadData();
        }

        private void cmbClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplicationVariables.WorkingClient = cmbClients.SelectedItem as BasicModel;
            this.LoadData();

            this.Text = CommonFunctions.GetDialogTextWithUser();
        }
    }
}
