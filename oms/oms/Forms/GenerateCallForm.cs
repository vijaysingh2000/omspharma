using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using oms.DataAccessLayer;
using oms.Model;
using System.Diagnostics;

namespace oms.Forms
{
    public partial class GenerateCallForm : Form
    {
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;

        public GenerateCallForm()
        {
            InitializeComponent();
        }

        private void GenerateCallForm_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogText("Call List");

            this.LoadGrid();
            this.LoadData();
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("PatientId", typeof(string));
            this.dataTable.Columns.Add("OrderId", typeof(string));
            this.dataTable.Columns.Add("MRN", typeof(string));
            this.dataTable.Columns.Add("Name", typeof(string));
            this.dataTable.Columns.Add("Date of Birth", typeof(DateTime));
            this.dataTable.Columns.Add("Email", typeof(string));
            this.dataTable.Columns.Add("Insurance", typeof(string));
            this.dataTable.Columns.Add("Next Call Date", typeof(string));
            this.dataTable.Columns.Add("Drug Name", typeof(string));
            this.dataTable.Columns.Add("Last Order Number", typeof(string));
            this.dataTable.Columns.Add("DOS", typeof(DateTime));

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
            this.gridView.Columns["Last Order Number"].ColumnEdit = editLink;
            editLink.Click += new EventHandler(this.Orders_Click);

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["MRN"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Details_Click);

            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView.Columns["PatientId"].Visible = false;
            this.gridView.Columns["OrderId"].Visible = false;
            this.gridView.OptionsBehavior.ReadOnly = true;
        }

        private void Orders_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid patientID = CommonFunctions.GetGuidSafely(dataRow["patientId"].ToString());
            Guid orderId = CommonFunctions.GetGuidSafely(dataRow["OrderId"].ToString());
            OrderDetails orderDetails = new OrderDetails(patientID, orderId);
            orderDetails.ShowDialog();
            this.LoadData();
        }

        private void Details_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid patientID = CommonFunctions.GetGuidSafely(dataRow["patientId"].ToString());

            PatientDetailsForm patientDetails = new PatientDetailsForm(patientID);
            if (patientDetails.ShowDialog() == DialogResult.OK)
            {
                this.LoadData();
            }
        }

        private void txtNumberofdays_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                this.LoadData();

            }
        }

        private void LoadData()
        {
            int value = CommonFunctions.GetIntSafely(txtNumberofdays.Text);

            this.dataTable.Rows.Clear();

            List<OrderWithPatient> orders = OrdersDL.GetCallList(value);
            foreach (OrderWithPatient order in orders)
            {
                this.dataTable.Rows.Add(order.PatientId, order.Id, order.MRN, order.LastName + ", " + order.FirstName, order.DOB, order.Email, order.InsuranceName, order.NextCallDate, order.DrugName, order.OrderNumber, order.DOS);
            }

            this.gridControl.RefreshDataSource();
            this.gridControl.Refresh();
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            string fileName = Path.Combine(Path.GetTempPath(), "sendemail.txt");

            StringBuilder stringBuilder = new StringBuilder();
            string value = string.Empty;

            stringBuilder.Append("Hello Erica" + Environment.NewLine + Environment.NewLine);
            stringBuilder.Append("Here is the call list" + Environment.NewLine + Environment.NewLine);
            foreach (DataRow dataRow in this.dataTable.Rows)
            {
                value = $"{dataRow["MRN"]}\t{dataRow["Name"]}\t{dataRow["Email"]}\t{dataRow["Date of Birth"]}\t{dataRow["Drug Name"]}\t{dataRow["DOS"]}" + Environment.NewLine;
                stringBuilder.Append(value);
            }
            stringBuilder.Append(Environment.NewLine + Environment.NewLine);
            stringBuilder.Append("Thanks" + Environment.NewLine + Environment.NewLine);

            File.WriteAllText(fileName, stringBuilder.ToString());

            ProcessStartInfo pi = new ProcessStartInfo(fileName);
            CommonFunctions.OpenFile(fileName);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
