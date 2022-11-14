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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace oms.Forms
{
    public partial class OrdersForm : Form
    {
        private Patient WorkingPatient = null;
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;

        public OrdersForm()
        {
            InitializeComponent();
        }

        public OrdersForm(Guid patientId)
        {
            InitializeComponent();

            this.WorkingPatient = PatientsDL.Get(patientId);
        }

        private void btnNewOrder_Click(object sender, EventArgs e)
        {
            if(this.WorkingPatient != null)
            {
                OrderDetails orderDetails = new OrderDetails(this.WorkingPatient.Id, Guid.Empty);
                orderDetails.ShowDialog();
                this.LoadData();
            }
        }

        private void LoadData()
        {
            this.dataTable.Rows.Clear();
            this.txtMRN.Text = String.Empty;
            this.txtPatientName.Text = String.Empty;

            if (this.WorkingPatient != null)
            {
                this.txtMRN.Text = this.WorkingPatient.MRN;
                this.txtPatientName.Text = this.WorkingPatient.LastName + ", " + this.WorkingPatient.FirstName;
                List<OrderWithPatient> orders = OrdersDL.GetByPatientId(this.WorkingPatient.Id);
                foreach (OrderWithPatient order in orders)
                {
                    this.dataTable.Rows.Add(order.Id, order.OrderNumber, order.DrugName, order.DOS, order.ConfirmedDOS, order.EstimatedDeliveryDate, order.ConfirmedDeliveryDate, order.LastState);
                }
            }

            this.gridControl.RefreshDataSource();
            this.gridControl.Refresh();
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Id", typeof(string));
            this.dataTable.Columns.Add("Order Number", typeof(string));
            this.dataTable.Columns.Add("Drug Name", typeof(string));
            this.dataTable.Columns.Add("Date of Service", typeof(DateTime));
            this.dataTable.Columns.Add("Confirmed DOS", typeof(DateTime));
            this.dataTable.Columns.Add("Estimated Delivery Date", typeof(DateTime));
            this.dataTable.Columns.Add("Confirmed Delivery Date", typeof(DateTime));
            this.dataTable.Columns.Add("Order Status", typeof(string));

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

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Order Status"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Update_Click);

            RepositoryItemHyperLinkEdit editLink2 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Order Number"].ColumnEdit = editLink2;
            editLink2.Click += new EventHandler(this.Details_Click);

            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView.Columns["Id"].Visible = false;
            this.gridView.OptionsBehavior.ReadOnly = true;

        }

        private void Update_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid orderId = CommonFunctions.GetGuidSafely(dataRow["Id"].ToString());
            OrderTasks orderStatus = new OrderTasks(orderId);
            orderStatus.ShowDialog();
            this.LoadData();
        }

        private void Details_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid orderId = new Guid(dataRow["Id"].ToString());

            OrderDetails orderDetails = new OrderDetails(this.WorkingPatient.Id, orderId);
            orderDetails.ShowDialog();
            this.LoadData();
        }

        private void OrdersForm_Load(object sender, EventArgs e)
        {
            this.SetCaption();

            this.LoadGrid();

            this.LoadData();
        }

        private void SetCaption()
        {
            if(this.WorkingPatient == null)
                this.Text = CommonFunctions.GetDialogTextWithUser("List of orders");
            else
                this.Text = CommonFunctions.GetDialogText($"Patient Id => {this.WorkingPatient.MRN}, {this.WorkingPatient.FirstName} {this.WorkingPatient.LastName}");
        }

        private void txtMRN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                this.WorkingPatient = PatientsDL.Get(txtMRN.Text);

                this.SetCaption();

                this.LoadData();
            }

        }

        private void txtMRN_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
