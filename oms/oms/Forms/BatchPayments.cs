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
    public partial class BatchPayments : Form
    {
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;

        public BatchPayments()
        {
            InitializeComponent();
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Id", typeof(string));
            this.dataTable.Columns.Add("Name", typeof(string));
            this.dataTable.Columns.Add("Email Date", typeof(DateTime));
            this.dataTable.Columns.Add("Report Date", typeof(DateTime));
            this.dataTable.Columns.Add("Total Amount", typeof(float));
            this.dataTable.Columns.Add("Last Updated Date", typeof(DateTime));
            this.dataTable.Columns.Add("Last Updated By", typeof(string));


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
            this.gridView.Columns["Name"].ColumnEdit = editLink;
            editLink.Click += new EventHandler(this.Details_Click);

            RepositoryItemTextEdit editLink0 = new RepositoryItemTextEdit();
            this.gridView.Columns["Total Amount"].ColumnEdit = editLink0;
            editLink0.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            editLink0.DisplayFormat.FormatString = "c2";
            this.gridView.Columns["Total Amount"].AppearanceCell.BackColor = Color.LightYellow;
            this.gridView.Columns["Total Amount"].AppearanceCell.ForeColor = Color.Black;

            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView.Columns["Id"].Visible = false;
            this.gridView.OptionsBehavior.ReadOnly = true;
        }

        private void Details_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid Id = new Guid(dataRow["Id"].ToString());

            BatchPaymentForm batchPaymentForm = new BatchPaymentForm(Id);
            if(batchPaymentForm.ShowDialog() == DialogResult.OK)
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
            List<Batch> batches = BatchPaymentDL.Get();
            this.dataTable.Rows.Clear();
            foreach (Batch batch in batches)
            {
                dataTable.Rows.Add(batch.Id, batch.Name, batch.EmailDate, batch.ReportDate, batch.TotalAmount, batch.LastUpdatedDate, batch.LastUpdatedBy);
            }

            this.gridControl.RefreshDataSource();
            this.gridControl.Refresh();
        }

        private void btnNewBatch_Click(object sender, EventArgs e)
        {
            BatchPaymentForm batchPaymentForm = new BatchPaymentForm();
            batchPaymentForm.ShowDialog();
            this.LoadData();
        }

        private void BatchPayments_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogTextWithUser("Batch Payments");

            this.LoadGrid();
            this.LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
