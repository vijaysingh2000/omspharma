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
    public partial class Report_MonthlyInvoice_UC : UserControl, IReportUserControl
    {
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;
        private List<Report_MonthlyInvoice_Model> report_MonthlyInvoice_Models;
        private DateTime startDate;
        private DateTime endDate;

        public Report_MonthlyInvoice_UC()
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
            this.endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 11, 59, 59);

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
            this.dataTable.Columns.Add("Order", typeof(string));
            this.dataTable.Columns.Add("Patient Name", typeof(string));
            this.dataTable.Columns.Add("Insurance", typeof(string));
            this.dataTable.Columns.Add("DOS", typeof(DateTime));
            this.dataTable.Columns.Add("Amt Billed", typeof(float));
            this.dataTable.Columns.Add("M-CAL RX1", typeof(string));
            this.dataTable.Columns.Add("M-CAL RX2", typeof(string));
            this.dataTable.Columns.Add("M-CAL RX3", typeof(string));
            this.dataTable.Columns.Add("Total Prescribed", typeof(float));
            this.dataTable.Columns.Add("Drug", typeof(string));
            this.dataTable.Columns.Add("Total Units Billed", typeof(float));

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


            this.gridView.OptionsView.ShowFooter = true;

            this.gridView.Columns["Total Prescribed"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView.Columns["Total Prescribed"].DisplayFormat.FormatString = "{0:n0}";
            this.gridView.Columns["Total Prescribed"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            this.gridView.Columns["Total Prescribed"].SummaryItem.DisplayFormat = "{0:n0}";

            this.gridView.Columns["Total Units Billed"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView.Columns["Total Units Billed"].DisplayFormat.FormatString = "{0:n0}";
            this.gridView.Columns["Total Units Billed"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            this.gridView.Columns["Total Units Billed"].SummaryItem.DisplayFormat = "{0:n0}";

            this.gridView.Columns["Amt Billed"].AppearanceCell.BackColor = Color.LightYellow;
            this.gridView.Columns["Amt Billed"].AppearanceCell.ForeColor = Color.Black;
            this.gridView.Columns["Amt Billed"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView.Columns["Amt Billed"].DisplayFormat.FormatString = "{0:c2}";
            this.gridView.Columns["Amt Billed"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            this.gridView.Columns["Amt Billed"].SummaryItem.DisplayFormat = "{0:c2}";

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Order"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Order_Details_Click);

            RepositoryItemHyperLinkEdit editLink2 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Patient Name"].ColumnEdit = editLink2;
            editLink2.Click += new EventHandler(this.Patient_Details_Click);
        }

        private void Patient_Details_Click(object? sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid patientId = new Guid(dataRow["PatientId"].ToString());

            PatientDetailsForm patientDetails = new oms.Forms.PatientDetailsForm(patientId);
            if (patientDetails.ShowDialog() == DialogResult.OK)
                this.LoadData();
        }

        private void Order_Details_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            Guid patientId = new Guid(dataRow["PatientId"].ToString());
            Guid orderId = new Guid(dataRow["OrderId"].ToString());

            OrderDetails orderDetails = new oms.Forms.OrderDetails(patientId, orderId);
            if (orderDetails.ShowDialog() == DialogResult.OK)
                this.LoadData();
        }

        private void LoadData()
        {
            this.report_MonthlyInvoice_Models = ReportsDL.GetMonthlyInvoiceReport(this.startDate, this.endDate);
            this.dataTable.Rows.Clear();
            foreach (Report_MonthlyInvoice_Model model in this.report_MonthlyInvoice_Models)
            {
                dataTable.Rows.Add(
                    model.OrderId,
                    model.PatientId,
                    model.OrderNumber,
                    model.PatientName,
                    model.InsuranceName,
                    model.DOS,
                    Math.Round(model.AmountBilled, 2),
                    model.RX1,
                    model.RX2,
                    model.RX3,
                    Math.Round(model.TotalPrescribed, 2),
                    model.DrugName,
                    Math.Round(model.TotalUnitsBilled)
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
