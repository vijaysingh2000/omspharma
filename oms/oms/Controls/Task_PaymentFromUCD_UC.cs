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
using System.Diagnostics;
using System.Xml.Linq;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;

namespace oms.Controls
{
    public partial class Task_PaymentFromUCD_UC : UserControl, IOrderTaskUserControl
    {
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;
        private Order currentWorkingOrder;
        private OrderTask currentWorkingOrderTask;
        private List<OrderAssay> currentWorkingOrderAssays;

        public Task_PaymentFromUCD_UC()
        {
            InitializeComponent();
        }

        public void LoadControl(Guid orderId, string taskCode)
        {
            this.currentWorkingOrder = OrdersDL.Get(orderId);
            this.currentWorkingOrderTask = OrderTasksDL.Get(orderId, taskCode);
            this.currentWorkingOrderAssays = OrderAssayDL.Get(orderId);

            this.UpdateTotals();
            this.LoadGrid();
            this.LoadData();
        }

        public void UpdateTotals()
        {
            float totalUnit = 0;
            foreach (OrderAssay orderAssay in this.currentWorkingOrderAssays)
                totalUnit += orderAssay.Assay * orderAssay.Qty;

            double cog = Math.Round((totalUnit * this.currentWorkingOrder.CogPerUnit),2);
            double billed = Math.Round((totalUnit * this.currentWorkingOrder.BillPerUnit), 2);

            txtTotalCOGOrder.Text = cog.ToString();
            txtTotalBillCharged.Text = billed.ToString();
        }

        public void SaveControl()
        {
            XElement element = new XElement("notes");
            List<string> files = new List<string>();
            string orderTaskPath = PathDL.GetOrderTaskPath(this.currentWorkingOrderTask.OrderId, this.currentWorkingOrderTask.TaskCode, true);
            List<OrderPaymentClient> orderPayments = new List<OrderPaymentClient>();

            foreach (DataRow row in this.dataTable.Rows)
            {
                OrderPaymentClient orderPayment = new OrderPaymentClient();
                orderPayment.OrderId = this.currentWorkingOrder.Id;
                orderPayment.ChequeDate = CommonFunctions.GetDateTimeSafely(row["Date"]);
                orderPayment.ChequeNumber = CommonFunctions.GetStringSafely(row["Check Number"]);
                orderPayment.Amount = CommonFunctions.GetFloatSafely(row["Amount"]);
                orderPayment.Notes = CommonFunctions.GetStringSafely(row["Notes"]);
                orderPayment.Path = CommonFunctions.GetStringSafely(row["Path"]);
                orderPayment.FileName = CommonFunctions.GetStringSafely(row["FileName"]);

                if (!string.IsNullOrEmpty(orderPayment.Path) && !string.IsNullOrEmpty(orderPayment.FileName))
                {
                    string newPath = Path.Combine(orderTaskPath, orderPayment.FileName);

                    if (!orderPayment.Path.IsStringEqual(newPath))
                        File.Copy(orderPayment.Path, newPath, true);

                    orderPayment.Path = newPath;
                    files.Add(newPath.Trim().ToLower());
                }

                orderPayments.Add(orderPayment);
            }

            CommonFunctions.DeleteUnwantedFiles(orderTaskPath, files);

            this.currentWorkingOrderTask.TaskStatus = chkMarkasComplete.Checked ? E_TaskStatus.Complete : E_TaskStatus.InProgress;

            int result = OrderTasksDL.Update(this.currentWorkingOrderTask);
            if (result > 0)
            {
                result = OrderPaymentsClientDL.AddOrUpdate(orderPayments);
                if (result > 0)
                    CommonFunctions.ShowInfomationMessage($"Updated Successful");
            }
        }

        public void RefreshControl()
        {
            this.LoadData();
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Date", typeof(DateTime));
            this.dataTable.Columns.Add("Check Number", typeof(string));
            this.dataTable.Columns.Add("Amount", typeof(float));;
            this.dataTable.Columns.Add("Notes", typeof(string));
            this.dataTable.Columns.Add("Path", typeof(string));
            this.dataTable.Columns.Add("FileName", typeof(string));
            this.dataTable.Columns.Add("Attach", typeof(string));
            this.dataTable.Columns.Add("Delete", typeof(string));

            this.gridControl = new GridControl();
            this.panel1.Controls.Add(this.gridControl);
            this.gridControl.DataSource = this.dataTable;
            this.gridControl.Dock = DockStyle.Fill;
            this.gridControl.Visible = true;

            this.gridView = new GridView();
            this.gridControl.MainView = this.gridView;
            BaseView[] views = new BaseView[] { this.gridView };
            this.gridControl.ViewCollection.AddRange(views);

            this.gridView.GridControl = this.gridControl;
            this.gridView.PopulateColumns();

            RepositoryItemTextEdit editLink0 = new RepositoryItemTextEdit();
            this.gridView.Columns["Amount"].ColumnEdit = editLink0;
            editLink0.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            editLink0.DisplayFormat.FormatString = "c2";

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Attach"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Attach_Click);

            RepositoryItemHyperLinkEdit editLink2 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["FileName"].ColumnEdit = editLink2;
            editLink2.Click += new EventHandler(this.View_Click);

            RepositoryItemHyperLinkEdit editLink3 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Delete"].ColumnEdit = editLink3;
            editLink3.Click += new EventHandler(this.Delete_Click);

            this.gridView.Columns["Amount"].AppearanceCell.BackColor = Color.LightYellow;
            this.gridView.Columns["Amount"].AppearanceCell.ForeColor = Color.Black;
            this.gridView.Columns["Path"].OptionsColumn.ReadOnly = true;
            this.gridView.Columns["FileName"].OptionsColumn.ReadOnly = true;
            this.gridView.Columns["Path"].Visible = false;
            this.gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

            this.gridView.CellValueChanged += ((s, e) => 
            { 
                if(e.Column.GetTextCaption().IsStringEqual("Amount"))
                {
                    this.gridView.UpdateCurrentRow();
                    this.Calculate();
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

        private void Calculate()
        {
            float total = 0;
            foreach (DataRow dataRow in this.dataTable.Rows)
            {
                total += CommonFunctions.GetFloatSafely(dataRow["Amount"]);
            }
            txtTotalPaymentRecd.Text = Math.Round(total, 2).ToString();
        }

        private void LoadData()
        {
            this.dataTable.Rows.Clear();

            if (this.currentWorkingOrderTask != null)
            {
                List<OrderPaymentClient> orderPayments = OrderPaymentsClientDL.Get(this.currentWorkingOrder.Id);

                foreach (OrderPaymentClient orderPayment in orderPayments)
                {
                    this.dataTable.Rows.Add(
                        orderPayment.ChequeDate,
                        orderPayment.ChequeNumber,
                        orderPayment.Amount,
                        orderPayment.Notes,
                        orderPayment.Path,
                        orderPayment.FileName,
                        "Attach",
                        "Delete");
                }

                this.gridControl.RefreshDataSource();
                this.gridControl.Refresh();

                this.Calculate();

                chkMarkasComplete.Checked = this.currentWorkingOrderTask.TaskStatus == E_TaskStatus.Complete;
            }
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
                    row = this.dataTable.Rows.Add(DateTime.Now, string.Empty, 0, string.Empty, string.Empty, string.Empty, "Attach", "Delete");
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

    }
}
