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
using oms.Forms;

namespace oms.Controls
{
    public partial class Task_PaymentFromInsurance_UC : UserControl, IOrderTaskUserControl
    {
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;
        private Order currentWorkingOrder;
        private OrderTask currentWorkingOrderTask;
        private List<OrderAssay> currentWorkingOrderAssays;
        private double totalcog = 0;
        private double totalbilled = 0;

        public Task_PaymentFromInsurance_UC()
        {
            InitializeComponent();
        }

        public void LoadControl(Guid orderId, string taskCode)
        {
            this.currentWorkingOrder = OrdersDL.Get(orderId);
            this.currentWorkingOrderTask = OrderTasksDL.Get(orderId, taskCode);
            this.currentWorkingOrderAssays = OrderAssayDL.Get(orderId);

            this.LoadGrid();
            this.LoadData();

            this.UpdateTotals();
        }

        public void UpdateTotals()
        {
            float totalUnit = 0;
            foreach (OrderAssay orderAssay in this.currentWorkingOrderAssays)
                totalUnit += orderAssay.Assay * orderAssay.Qty;

            this.totalcog = Math.Round((totalUnit * this.currentWorkingOrder.CogPerUnit),2);
            this.totalbilled = Math.Round((totalUnit * this.currentWorkingOrder.BillPerUnit), 2);

            txtTotalCOGOrder.Text = this.totalcog.ToString("C");
            txtTotalBillCharged.Text = this.totalbilled.ToString("C");

            this.Calculate();
        }

        public void SaveControl()
        {
            XElement element = new XElement("notes");
            List<string> files = new List<string>();
            string orderTaskPath = PathDL.GetOrderTaskPath(this.currentWorkingOrderTask.OrderId, this.currentWorkingOrderTask.TaskCode, true);
            List<OrderPaymentInsurance> orderPayments = new List<OrderPaymentInsurance>();

            foreach (DataRow row in this.dataTable.Rows)
            {
                OrderPaymentInsurance orderPayment = new OrderPaymentInsurance();
                orderPayment.OrderId = this.currentWorkingOrder.Id;
                orderPayment.BatchId = CommonFunctions.GetGuidSafely(row["BatchId"]);
                orderPayment.ChequeDate = CommonFunctions.GetDateTimeSafely(row["Date"]);
                orderPayment.ChequeNumber = CommonFunctions.GetStringSafely(row["Check Number"]);
                orderPayment.Amount = CommonFunctions.GetFloatSafely(row["Amount"]);
                orderPayment.Notes = CommonFunctions.GetStringSafely(row["Notes"]);
                orderPayment.Path = CommonFunctions.GetStringSafely(row["Path"]);
                orderPayment.FileName = CommonFunctions.GetStringSafely(row["FileName"]);
                orderPayment.PaymentType = StaticListDL.GetId(Constants.TableName_PaymentTypes, CommonFunctions.GetStringSafely(row["Type"]));
                orderPayment.IsPap = CommonFunctions.GetBoolSafely(row["Pap"]);

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
                result = OrderPaymentsInsuranceDL.AddOrUpdate(orderPayments);
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
            this.dataTable.Columns.Add("BatchId", typeof(string));
            this.dataTable.Columns.Add("Batch Reference", typeof(string));
            this.dataTable.Columns.Add("Check Number", typeof(string));
            this.dataTable.Columns.Add("Type", typeof(string));
            this.dataTable.Columns.Add("Pap", typeof(bool)); 
            this.dataTable.Columns.Add("Amount", typeof(float));;
            this.dataTable.Columns.Add("Notes", typeof(string)); ;
            this.dataTable.Columns.Add("Path", typeof(string));
            this.dataTable.Columns.Add("FileName", typeof(string));
            this.dataTable.Columns.Add("Attach", typeof(string));
            this.dataTable.Columns.Add("Clear", typeof(string));

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

            RepositoryItemHyperLinkEdit editLink = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Batch Reference"].ColumnEdit = editLink;
            editLink.Click += new EventHandler(this.Batch_Click);

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
            this.gridView.Columns["Clear"].ColumnEdit = editLink3;
            editLink3.Click += new EventHandler(this.Delete_Click);

            RepositoryItemComboBox editLink4 = new RepositoryItemComboBox();
            this.gridView.Columns["Type"].ColumnEdit = editLink4;
            List<BasicModel> items = StaticListDL.GetActive(Constants.TableName_PaymentTypes);
            foreach (BasicModel model in items)
                editLink4.Items.Add(model.Name);

            this.gridView.Columns["Amount"].AppearanceCell.BackColor = Color.LightYellow;
            this.gridView.Columns["Amount"].AppearanceCell.ForeColor = Color.Black;
            this.gridView.Columns["Path"].OptionsColumn.ReadOnly = true;
            this.gridView.Columns["FileName"].OptionsColumn.ReadOnly = true;
            this.gridView.Columns["Path"].Visible = false;
            this.gridView.Columns["BatchId"].Visible = false;
            this.gridView.Columns["Batch Reference"].OptionsColumn.ReadOnly = true;
            //this.gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            //this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

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
                view.SetRowCellValue(e.RowHandle, view.Columns["Clear"], "Clear");
            });
        }

        private void Calculate()
        {
            float total = 0;
            foreach (DataRow dataRow in this.dataTable.Rows)
            {
                total += CommonFunctions.GetFloatSafely(dataRow["Amount"]);
            }
            txtTotalPaymentRecd.Text = Math.Round(total, 2).ToString("C");
            txtBalance.Text = Math.Round(this.totalbilled - total).ToString("C");    
        }

        private void LoadData()
        {
            this.dataTable.Rows.Clear();

            if (this.currentWorkingOrderTask != null)
            {
                List<OrderPaymentInsurance> orderPayments = OrderPaymentsInsuranceDL.Get(this.currentWorkingOrder.Id);

                foreach (OrderPaymentInsurance orderPayment in orderPayments)
                {
                    this.dataTable.Rows.Add(
                        orderPayment.ChequeDate,
                        orderPayment.BatchId,
                        orderPayment.BatchName,
                        orderPayment.ChequeNumber,
                        orderPayment.PaymentTypeName,
                        orderPayment.IsPap,
                        orderPayment.Amount,
                        orderPayment.Notes,
                        orderPayment.Path,
                        orderPayment.FileName,
                        "Attach",
                        "Clear");
                }

                this.gridControl.RefreshDataSource();
                this.gridControl.Refresh();

                this.Calculate();

                chkMarkasComplete.Checked = this.currentWorkingOrderTask.TaskStatus == E_TaskStatus.Complete;
            }
        }

        private void Batch_Click(object sender, EventArgs e)
        {
            DataRow row = this.gridView.GetFocusedDataRow();

            if (row != null)
            {
                Guid batchId = CommonFunctions.GetGuidSafely(row["BatchId"].ToString());

                if (batchId != Guid.Empty)
                {
                    BatchPaymentForm batchPaymentForm = new BatchPaymentForm(batchId);
                    batchPaymentForm.ShowDialog();
                    this.LoadData();
                    this.Calculate();
                }
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
                    row["Path"] = string.Empty;
                    row["FileName"] = string.Empty;

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
