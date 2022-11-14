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
using DevExpress.XtraEditors.Repository;
using oms.Forms;
namespace oms.Controls
{
    public partial class Report_PurchasingReport_UC : UserControl, IReportUserControl
    {
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;
        private List<Report_Purchasing_Model> report_Purchasing_Models;
        private DateTime startDate;
        private DateTime endDate;

        public Report_PurchasingReport_UC()
        {
            InitializeComponent();

            common_DatePicker_uc1.OnShowReportFromRange += new Common_DatePicker_UC.ShowReportFromRange(this.common_DatePicker_uc1_show);
        }

        public void common_DatePicker_uc1_show(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;

            this.LoadData();
        }

        public void LoadControl()
        {
            this.startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            this.endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 30, 11, 59, 59);

            this.LoadGrid();
            this.LoadData();
        }

        public void RefreshControl()
        {
            
        }

        public void SaveControl()
        {
            
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("OrderId", typeof(string));
            this.dataTable.Columns.Add("PatientId", typeof(string));
            this.dataTable.Columns.Add("Date Ordered", typeof(DateTime));
            this.dataTable.Columns.Add("MFG", typeof(string));
            this.dataTable.Columns.Add("Confirmation #", typeof(string));
            this.dataTable.Columns.Add("Invoice/PO#", typeof(string));
            this.dataTable.Columns.Add("Drug", typeof(string));
            this.dataTable.Columns.Add("Assay", typeof(float));
            this.dataTable.Columns.Add("NDC", typeof(string));
            this.dataTable.Columns.Add("Lot#", typeof(string));
            this.dataTable.Columns.Add("Exp Date", typeof(DateTime));
            this.dataTable.Columns.Add("Tran Type", typeof(string));
            this.dataTable.Columns.Add("Vials", typeof(float));
            this.dataTable.Columns.Add("R/C", typeof(DateTime));
            this.dataTable.Columns.Add("ProphyPRN", typeof(string));
            this.dataTable.Columns.Add("Total Unit Dispensed", typeof(float));
            this.dataTable.Columns.Add("Unit/IU Cost", typeof(float));
            this.dataTable.Columns.Add("Total Order Cost", typeof(float));
            this.dataTable.Columns.Add("340B ID", typeof(string));

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
            this.gridView.OptionsBehavior.ReadOnly = true;
            this.gridView.Columns["PatientId"].Visible = false;
            this.gridView.Columns["OrderId"].Visible = false;

            this.gridView.Columns["Total Unit Dispensed"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView.Columns["Total Unit Dispensed"].DisplayFormat.FormatString = "{0:n0}";
            this.gridView.Columns["Unit/IU Cost"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView.Columns["Unit/IU Cost"].DisplayFormat.FormatString = "{0:c2}";
            this.gridView.Columns["Total Order Cost"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView.Columns["Total Order Cost"].DisplayFormat.FormatString = "{0:c2}";


            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Invoice/PO#"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Order_Details_Click);
        }

        private void Order_Details_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid patientId = new Guid(dataRow["PatientId"].ToString());
            Guid orderId = new Guid(dataRow["OrderId"].ToString());

            OrderDetails orderDetails = new oms.Forms.OrderDetails(patientId, orderId);
            orderDetails.ShowDialog();
            this.LoadData();
        }

        private void LoadData()
        {
            this.report_Purchasing_Models = ReportsDL.GetPurchasingReport(this.startDate, this.endDate);
            this.dataTable.Rows.Clear();
            foreach (Report_Purchasing_Model model in this.report_Purchasing_Models)
            {
                dataTable.Rows.Add(
                    model.OrderId.ToString(),
                    model.PatientId.ToString(),
                    model.DateOrdered,
                    model.ManufacturerName,
                    model.ConfirmationNumber,
                    model.OrderNumber,
                    model.DrugName,
                    model.Assay,
                    model.NDC,
                    model.Lot,
                    model.ExpDate,
                    model.TransactionType,
                    model.Quantity,
                    model.RCDate,
                    model.ProphyPRN,
                    Math.Round(model.Assay * model.Quantity, 2),
                    model.COGPerUnit,
                    Math.Round(model.Assay * model.Quantity * model.COGPerUnit, 2),
                    model.Id340BName
                    );
            }

            this.gridControl.RefreshDataSource();
            this.gridControl.Refresh();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            this.LoadData();
        }

        public void ExportToExcel(string fileName)
        {
            this.gridView.ExportToXlsx(fileName);
        }

        public void UpdateExcel(string fileName)
        {
            ExcelLibrary.UpdateDataTableToExcelMainData(this.dataTable, fileName);
        }
    }
}
