using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using oms.DataAccessLayer;
using oms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace oms.Forms
{
    public partial class BatchPaymentForm : Form
    {
        private DataTable dataTable1 = null;
        private GridControl gridControl1 = null;
        private GridView gridView1;
        private DataTable dataTable2;
        private GridControl gridControl2;
        private GridView gridView2;
        private Batch WorkingBatch;
        private bool NewBatch = false;

        public BatchPaymentForm()
        {
            InitializeComponent();

            if (this.WorkingBatch == null)
            {
                this.WorkingBatch = new Batch();
                this.WorkingBatch.Id = Guid.NewGuid();
                this.NewBatch = true;
            }
        }
        public BatchPaymentForm(Guid batchId)
        {
            InitializeComponent();

            this.WorkingBatch = BatchPaymentDL.Get(batchId);
            if (this.WorkingBatch == null)
            {
                this.WorkingBatch = new Batch();
                this.WorkingBatch.Id = Guid.NewGuid();
                this.NewBatch = true;
            }
        }

        private void BatchPaymentForm_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogTextWithUser("Batch Payment");

            this.LoadGrid1();
            this.LoadGrid2();

            this.LoadData1();
            this.LoadData2();

        }
        public void LoadGrid1()
        {
            this.dataTable1 = new DataTable();
            this.dataTable1.Columns.Add("Order Number", typeof(string));
            this.dataTable1.Columns.Add("RX Number", typeof(string));
            this.dataTable1.Columns.Add("Cheque Number", typeof(string));
            this.dataTable1.Columns.Add("Cheque Date", typeof(DateTime));
            this.dataTable1.Columns.Add("Type", typeof(string));
            this.dataTable1.Columns.Add("PAP", typeof(bool));
            this.dataTable1.Columns.Add("Amount", typeof(float));
            this.dataTable1.Columns.Add("Balance", typeof(float));
            this.dataTable1.Columns.Add("Notes", typeof(string));
            this.dataTable1.Columns.Add("Path", typeof(string));
            this.dataTable1.Columns.Add("FileName", typeof(string));
            this.dataTable1.Columns.Add("Attach", typeof(string));
            this.dataTable1.Columns.Add("Delete", typeof(string));
            this.dataTable1.Columns.Add("Order", typeof(Order));
            this.dataTable1.Columns.Add("OrderId", typeof(string));

            this.gridControl1 = new GridControl();
            this.grpPayments.Controls.Add(this.gridControl1);
            this.gridControl1.DataSource = this.dataTable1;
            this.gridControl1.Dock = DockStyle.Fill;
            this.gridControl1.Visible = true;

            this.gridView1 = new GridView();
            this.gridControl1.MainView = this.gridView1;
            BaseView[] views = new BaseView[] { this.gridView1 };
            this.gridControl1.ViewCollection.AddRange(views);

            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.PopulateColumns();

            RepositoryItemComboBox editLink = new RepositoryItemComboBox();
            foreach (BasicModel basicModel in StaticListDL.GetActive(Constants.TableName_PaymentTypes))
                editLink.Items.Add(basicModel.Name);
            this.gridView1.Columns["Type"].ColumnEdit = editLink;

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView1.Columns["Attach"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Attach1_Click);

            RepositoryItemHyperLinkEdit editLink2 = new RepositoryItemHyperLinkEdit();
            this.gridView1.Columns["FileName"].ColumnEdit = editLink2;
            editLink2.Click += new EventHandler(this.View1_Click);

            RepositoryItemHyperLinkEdit editLink3 = new RepositoryItemHyperLinkEdit();
            this.gridView1.Columns["Delete"].ColumnEdit = editLink3;
            editLink3.Click += new EventHandler(this.Delete1_Click);

            RepositoryItemComboBox editLink4 = new RepositoryItemComboBox();
            this.gridView1.Columns["RX Number"].ColumnEdit = editLink4;

            RepositoryItemCheckEdit editLink5 = new RepositoryItemCheckEdit();
            this.gridView1.Columns["PAP"].ColumnEdit = editLink5;

            this.gridView1.Columns["Order"].Visible = false;
            this.gridView1.Columns["OrderId"].Visible = false;
            this.gridView1.Columns["Path"].Visible = false;
            this.gridView1.Columns["Path"].OptionsColumn.ReadOnly = true;
            this.gridView1.Columns["Balance"].OptionsColumn.ReadOnly = true;

            this.gridView1.OptionsView.ShowFooter = true;
            this.gridView1.Columns["Amount"].AppearanceCell.BackColor = Color.LightYellow;
            this.gridView1.Columns["Amount"].AppearanceCell.ForeColor = Color.Black;
            this.gridView1.Columns["Amount"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView1.Columns["Amount"].DisplayFormat.FormatString = "{0:c2}";
            this.gridView1.Columns["Amount"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            this.gridView1.Columns["Amount"].SummaryItem.DisplayFormat = "{0:c2}";

            this.gridView1.Columns["Balance"].AppearanceCell.BackColor = Color.LightYellow;
            this.gridView1.Columns["Balance"].AppearanceCell.ForeColor = Color.Black;
            this.gridView1.Columns["Balance"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.gridView1.Columns["Balance"].DisplayFormat.FormatString = "{0:c2}";
            //this.gridView1.Columns["Balance"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            //this.gridView1.Columns["Balance"].SummaryItem.DisplayFormat = "{0:c2}";

            this.gridView1.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            this.gridView1.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

            this.gridView1.RowCellClick += ((s, e) =>
            {
                if (e.Column.GetTextCaption().IsStringEqual("Order Number"))
                {
                    this.UpdateOrderToCell();
                }
            });

            this.gridView1.CellValueChanged += ((s, e) =>
            {
                if (e.Column.GetTextCaption().IsStringEqual("Order Number"))
                {
                    if (!this.UpdateOrderToCell())
                        CommonFunctions.ShowErrorMessage($"Invalid order number");
                }
            });

            this.gridView1.CustomRowCellEditForEditing += ((s, e) =>
            {
                if (e.Column.FieldName.IsStringEqual("RX Number"))
                {
                    this.gridView1.UpdateCurrentRow();
                    DataRow row = this.gridView1.GetFocusedDataRow();
                    if (row != null)
                    {
                        Order order = row["Order"] as Order;
                        if (order == null)
                        {
                            this.UpdateOrderToCell();
                            order = row["Order"] as Order; 
                        }

                        RepositoryItemComboBox itemx = e.RepositoryItem as RepositoryItemComboBox;
                        if (itemx == null)
                        {
                            itemx = new RepositoryItemComboBox();
                            e.RepositoryItem = itemx;
                        }

                        if (order != null)
                        {
                            List<OrderAssay> orderAssays = OrderAssayDL.Get(order.Id);
                            itemx.Items.Clear();
                            foreach (OrderAssay orderAssay in orderAssays)
                                itemx.Items.Add(orderAssay.RxNumber);   
                        }
                        else
                        {
                            itemx.Items.Clear();
                        }
                    }
                }
            }
            );

            this.gridView1.InitNewRow += ((s, e) =>
            {
                GridView view = s as GridView;

                view.SetRowCellValue(e.RowHandle, view.Columns["Order"], null);
                view.SetRowCellValue(e.RowHandle, view.Columns["Cheque Date"], DateTime.Now);
                view.SetRowCellValue(e.RowHandle, view.Columns["Attach"], "Attach");
                view.SetRowCellValue(e.RowHandle, view.Columns["Delete"], "Delete");

                view.Columns["RX Number"].ColumnEdit = null;
            });
        }
        public bool UpdateOrderToCell()
        {
            this.gridView1.UpdateCurrentRow();
            DataRow currentRow = this.gridView1.GetFocusedDataRow();

            if (currentRow != null)
            {
                GridColumn orderNumberColumn = this.gridView1.Columns["Order Number"];
                GridColumn rxNumberColumn = this.gridView1.Columns["RX Number"];
                string orderNumber = CommonFunctions.GetStringSafely(currentRow["Order Number"]);
                Order order = OrdersDL.Get(orderNumber);
                currentRow["Order"] = order;
                currentRow["OrderId"] = order != null ? order.Id : Guid.Empty;
                currentRow["Balance"] = order != null ? Math.Round((order.TotalUnitsBilled * order.BillPerUnit) - order.TotalPayments, 2) : 0;

                return order != null;
            }

            return false;
        }
        public void LoadGrid2()
        {
            this.dataTable2 = new DataTable();
            this.dataTable2.Columns.Add("Notes", typeof(string));
            this.dataTable2.Columns.Add("Path", typeof(string));
            this.dataTable2.Columns.Add("FileName", typeof(string));
            this.dataTable2.Columns.Add("Attach", typeof(string));
            this.dataTable2.Columns.Add("Delete", typeof(string));

            this.gridControl2 = new GridControl();
            this.grpAttachments.Controls.Add(this.gridControl2);
            this.gridControl2.DataSource = this.dataTable2;
            this.gridControl2.Dock = DockStyle.Fill;
            this.gridControl2.Visible = true;

            this.gridView2 = new GridView();
            this.gridControl2.MainView = this.gridView2;
            BaseView[] views = new BaseView[] { this.gridView2 };
            this.gridControl2.ViewCollection.AddRange(views);

            this.gridView2.GridControl = this.gridControl2;
            this.gridView2.PopulateColumns();

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView2.Columns["Attach"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Attach2_Click);

            RepositoryItemHyperLinkEdit editLink2 = new RepositoryItemHyperLinkEdit();
            this.gridView2.Columns["FileName"].ColumnEdit = editLink2;
            editLink2.Click += new EventHandler(this.View2_Click);

            RepositoryItemHyperLinkEdit editLink3 = new RepositoryItemHyperLinkEdit();
            this.gridView2.Columns["Delete"].ColumnEdit = editLink3;
            editLink3.Click += new EventHandler(this.Delete2_Click);

            this.gridView2.Columns["Path"].Visible = false;
            this.gridView2.Columns["Path"].OptionsColumn.ReadOnly = true;

            this.gridView2.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            this.gridView2.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

            this.gridView2.CellValueChanged += ((s, e) =>
            {
                if (e.Column.GetTextCaption().IsStringEqual("Amount"))
                {
                    this.gridView2.UpdateCurrentRow();
                    this.Calculate();
                }
            });

            this.gridView2.InitNewRow += ((s, e) =>
            {
                GridView view = s as GridView;
                view.SetRowCellValue(e.RowHandle, view.Columns["Attach"], "Attach");
                view.SetRowCellValue(e.RowHandle, view.Columns["Delete"], "Delete");
            });
        }
        public void Calculate()
        {

        }
        public void LoadData1()
        {
            txtEmailDate.Value = this.WorkingBatch.EmailDate;
            txtBatchName.Text = this.WorkingBatch.Name;
            txtLastUpdatedBy.Text = this.WorkingBatch.LastUpdatedBy;
            txtLastUpdatedDate.Value = this.WorkingBatch.LastUpdatedDate;

            List<OrderPaymentInsurance> batchPayments = OrderPaymentsInsuranceDL.GetByBatch(this.WorkingBatch.Id);
            this.dataTable1.Rows.Clear();
            foreach (OrderPaymentInsurance batchPayment in batchPayments)
            {
                this.dataTable1.Rows.Add(
                    batchPayment.OrderNumber,
                    batchPayment.RxNumber,
                    batchPayment.ChequeNumber,
                    batchPayment.ChequeDate,
                    batchPayment.PaymentTypeName,
                    batchPayment.IsPap,
                    batchPayment.Amount,
                    Math.Round(batchPayment.TotalAmountBilled - batchPayment.TotalPayments, 2),
                    batchPayment.Notes,
                    batchPayment.Path,
                    batchPayment.FileName,
                    "Attach",
                    "Delete",
                    null,
                    batchPayment.OrderId);
            }

            this.gridControl1.RefreshDataSource();
            this.gridControl1.Refresh();
        }
        public void LoadData2()
        {
            XElement elements = CommonFunctions.GetXmlSafely(this.WorkingBatch.Notes);

            IEnumerable<XElement> items = elements.Elements("item");
            this.dataTable2.Rows.Clear();
            foreach (XElement item in items)
            {
                this.dataTable2.Rows.Add(
                    item.GetAttribute("notes"),
                    item.GetAttribute("path"),
                    item.GetAttribute("filename"),
                    "Attach",
                    "Delete"
                    );
            }

            this.gridControl2.RefreshDataSource();
            this.gridControl2.Refresh();
        }
        private void View1_Click(object sender, EventArgs e)
        {
            DataRow row = this.gridView1.GetFocusedDataRow();

            if (row != null && row["Path"] != null && !string.IsNullOrEmpty(row["Path"].ToString()))
            {
                string fileName = row["Path"].ToString();
                CommonFunctions.OpenFile(fileName);
            }
        }
        private void Attach1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Title = "Attachment";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DataRow row = this.gridView1.GetFocusedDataRow();
                if (row == null)
                {
                    row = this.dataTable1.Rows.Add(string.Empty, string.Empty, string.Empty, DateTime.Now, 0, string.Empty, string.Empty, string.Empty, "Attach", "Delete");
                    this.gridControl1.RefreshDataSource();
                }

                row["FileName"] = Path.GetFileName(dialog.FileName);
                row["Path"] = dialog.FileName;
            }
        }
        private void Delete1_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = this.gridView1.GetFocusedDataRow();
                if (row != null)
                {
                    this.dataTable1.Rows.Remove(row);
                    this.gridControl1.RefreshDataSource();
                    this.gridControl1.Refresh();
                }
            }
            catch
            {
                this.gridControl1.RefreshDataSource();
                this.gridControl1.Refresh();
            }
        }
        private void View2_Click(object sender, EventArgs e)
        {
            DataRow row = this.gridView2.GetFocusedDataRow();

            if (row != null && row["Path"] != null && !string.IsNullOrEmpty(row["Path"].ToString()))
            {
                string fileName = row["Path"].ToString();
                CommonFunctions.OpenFile(fileName);
            }
        }
        private void Attach2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Title = "Attachment";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DataRow row = this.gridView2.GetFocusedDataRow();
                if (row == null)
                {
                    row = this.dataTable2.Rows.Add(string.Empty, string.Empty, string.Empty, "Attach", "Delete");
                    this.gridControl2.RefreshDataSource();
                }

                row["FileName"] = Path.GetFileName(dialog.FileName);
                row["Path"] = dialog.FileName;
            }
        }
        private void Delete2_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = this.gridView2.GetFocusedDataRow();
                if (row != null)
                {
                    this.dataTable2.Rows.Remove(row);
                    this.gridControl2.RefreshDataSource();
                    this.gridControl2.Refresh();
                }
            }
            catch
            {
                this.gridControl2.RefreshDataSource();
                this.gridControl2.Refresh();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtBatchName.Text.Trim()))
            {
                CommonFunctions.ShowErrorMessage($"Batch name => {txtBatchName.Text} cannot be empty.");
                txtBatchName.Focus();
                return;
            }

            Batch batch = BatchPaymentDL.Get(txtBatchName.Text);
            string batchPath = PathDL.GetBatchPath(this.WorkingBatch.Id, true);
            List<string> files = new List<string>();

            if (batch != null)
            {
                if(this.NewBatch)
                {
                    CommonFunctions.ShowErrorMessage($"Batch name => {txtBatchName.Text} already exists.");
                    txtBatchName.Focus();
                    return;
                }
                else if(!batch.Id.Equals(this.WorkingBatch.Id))
                {
                    CommonFunctions.ShowErrorMessage($"Batch name => {txtBatchName.Text} already exists.");
                    txtBatchName.Focus();
                    return;
                }
            }

            List<string> invalidOrders = new List<string>();
            string message = string.Empty;
            foreach (DataRow dataRow in this.dataTable1.Rows)
            {
                if (dataRow["OrderId"] == DBNull.Value 
                    || dataRow["OrderId"] == null 
                    || dataRow["OrderId"].ToString().Trim() == String.Empty 
                    || CommonFunctions.GetGuidSafely(dataRow["OrderId"]).Equals(Guid.Empty))
                {
                    invalidOrders.Add(CommonFunctions.GetStringSafely(dataRow["Order Number"]));
                    message += CommonFunctions.GetStringSafely(dataRow["Order Number"]);
                }
            }

            if(!string.IsNullOrEmpty(message))
            {
                CommonFunctions.ShowErrorMessage("Following order numbers are invalid => " + Environment.NewLine + message);
                return;
            }

                        
            List<OrderPaymentInsurance> batchPayments = new List<OrderPaymentInsurance>();
            List<BasicModel> paymentTypes = StaticListDL.GetActive(Constants.TableName_PaymentTypes);
            
            foreach (DataRow dataRow in this.dataTable1.Rows)
            {
                OrderPaymentInsurance batchPayment = new OrderPaymentInsurance();
                batchPayment.OrderId = CommonFunctions.GetGuidSafely(dataRow["OrderId"]);
                batchPayment.OrderNumber = CommonFunctions.GetStringSafely(dataRow["Order Number"]);
                batchPayment.RxNumber = CommonFunctions.GetStringSafely(dataRow["RX Number"]);
                batchPayment.ChequeNumber = CommonFunctions.GetStringSafely(dataRow["Cheque Number"]);
                batchPayment.ChequeDate = CommonFunctions.GetDateTimeSafely(dataRow["Cheque Date"]);
                batchPayment.Amount = CommonFunctions.GetFloatSafely(dataRow["Amount"]);
                batchPayment.Notes = CommonFunctions.GetStringSafely(dataRow["Notes"]);
                batchPayment.BatchId = this.WorkingBatch.Id;
                batchPayment.Path = CommonFunctions.GetStringSafely(dataRow["Path"]);
                batchPayment.FileName = CommonFunctions.GetStringSafely(dataRow["FileName"]);
                batchPayment.IsPap = CommonFunctions.GetBoolSafely(dataRow["Pap"]);
                BasicModel model = paymentTypes.FirstOrDefault(x => x.Name.IsStringEqual(dataRow["Type"].ToString()));
                batchPayment.PaymentType = model != null ? model.Id : 0;

                string orderTaskPath = PathDL.GetOrderTaskPath(batchPayment.OrderId, "IPAY", true);

                if (!string.IsNullOrEmpty(batchPayment.Path) && !string.IsNullOrEmpty(batchPayment.FileName))
                {
                    string newPath = Path.Combine(orderTaskPath, batchPayment.FileName);

                    if (!batchPayment.Path.IsStringEqual(newPath))
                        File.Copy(batchPayment.Path, newPath, true);

                    batchPayment.Path = newPath;
                    files.Add(newPath.Trim().ToLower());
                }

                batchPayments.Add(batchPayment);
            }

            XElement items = new XElement("notes");
            foreach (DataRow dataRow in this.dataTable2.Rows)
            {
                XElement item = new XElement("item");
                string fileName = CommonFunctions.GetStringSafely(dataRow["FileName"]);
                string path = CommonFunctions.GetStringSafely(dataRow["Path"]);
                string notes = CommonFunctions.GetStringSafely(dataRow["Notes"]);

                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(fileName))
                {
                    string newPath = Path.Combine(batchPath, fileName);

                    if (!path.IsStringEqual(newPath))
                        File.Copy(path, newPath, true);

                    path = newPath;
                    files.Add(newPath.Trim().ToLower());
                }

                item.SetAttributeValue("filename", fileName);
                item.SetAttributeValue("path", path);
                item.SetAttributeValue("notes", notes);

                items.Add(item);
            }

            this.WorkingBatch.Notes = items.ToString();
            this.WorkingBatch.Name = txtBatchName.Text;
            this.WorkingBatch.EmailDate = txtEmailDate.Value;
            this.WorkingBatch.ReportDate = txtReportDate.Value;

            CommonFunctions.DeleteUnwantedFiles(batchPath, files);

            int result = BatchPaymentDL.UpdateBatchPayments(this.WorkingBatch, batchPayments);

            if (result > 0)
            {
                this.NewBatch = false;
                CommonFunctions.ShowInfomationMessage($"Batch Payment => {txtBatchName.Text} updated successfully.");
                this.LoadData1();
                this.LoadData2();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
