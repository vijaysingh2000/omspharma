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
using DevExpress.XtraEditors.Controls;
using System.Runtime.CompilerServices;

namespace oms.Controls
{
    public partial class Task_OrderPlaced_UC : UserControl, IOrderTaskUserControl
    {
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;
        private Order currentWorkingOrder;
        private OrderTask currentWorkingOrderTask;
        public Task_OrderPlaced_UC()
        {
            InitializeComponent();
        }

        public void LoadControl(Guid orderId, string taskCode)
        {
            this.currentWorkingOrder = OrdersDL.Get(orderId);
            this.currentWorkingOrderTask = OrderTasksDL.Get(orderId, taskCode);

            this.LoadGrid();
            this.LoadData();

            lblDrugName.Text = this.currentWorkingOrder.DrugName;
        }

        public void SaveControl()
        {
            XElement element = new XElement("notes");
            List<string> files = new List<string>();
            string orderTaskPath = PathDL.GetOrderTaskPath(this.currentWorkingOrderTask.OrderId, this.currentWorkingOrderTask.TaskCode, true);
            int stepDoneCount = 0;

            foreach (DataRow row in this.dataTable.Rows)
            {
                bool state = CommonFunctions.GetBoolSafely(row["State"]);
                string step = CommonFunctions.GetStringSafely(row["Step"]);
                DateTime dateTime = CommonFunctions.GetDateTimeSafely(row["Date"]);
                string notes = CommonFunctions.GetStringSafely(row["Notes"]);
                string path = CommonFunctions.GetStringSafely(row["Path"]);
                string filename = CommonFunctions.GetStringSafely(row["FileName"]);
                string newPath = string.Empty;

                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(filename))
                {
                    newPath = Path.Combine(orderTaskPath, filename);

                    if (!path.IsStringEqual(newPath))
                        File.Copy(path, newPath, true);

                    files.Add(newPath.Trim().ToLower());
                }

                XElement item = new XElement("item");
                item.SetAttributeValue("state", state);
                item.SetAttributeValue("step", step);
                item.SetAttributeValue("datetime", dateTime.ToString());
                item.SetAttributeValue("notes", notes);
                item.SetAttributeValue("path", newPath);
                item.SetAttributeValue("fileName", filename);

                element.Add(item);

                if (state)
                    stepDoneCount++;
            }

            CommonFunctions.DeleteUnwantedFiles(orderTaskPath, files);

            this.currentWorkingOrderTask.Notes = element.ToString();

            if(stepDoneCount == this.dataTable.Rows.Count && !chkMarkasComplete.Checked)
            {
                if(MessageBox.Show("All the steps are completed. Should we mark this task done !!!", CommonFunctions.GetDialogText(""), MessageBoxButtons.YesNo) == DialogResult.Yes)
                    chkMarkasComplete.Checked = true;
            }

            this.currentWorkingOrderTask.TaskStatus = chkMarkasComplete.Checked ? E_TaskStatus.Complete : E_TaskStatus.InProgress;
            int result = OrderTasksDL.Update(this.currentWorkingOrderTask);            
        }

        public void RefreshControl()
        {
            this.LoadData();
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Default", typeof(bool));
            this.dataTable.Columns.Add("State", typeof(bool));
            this.dataTable.Columns.Add("Step", typeof(string));
            this.dataTable.Columns.Add("Date", typeof(DateTime));
            this.dataTable.Columns.Add("Notes", typeof(string));
            this.dataTable.Columns.Add("Path", typeof(string));
            this.dataTable.Columns.Add("FileName", typeof(string));
            this.dataTable.Columns.Add("Attach", typeof(string));
            this.dataTable.Columns.Add("Clear", typeof(string));


            this.gridControl = new GridControl();
            this.panGrid.Controls.Add(this.gridControl);
            this.gridControl.DataSource = this.dataTable;
            this.gridControl.Dock = DockStyle.Fill;
            this.gridControl.Visible = true;

            this.gridView = new GridView();
            this.gridControl.MainView = this.gridView;
            BaseView[] views = new BaseView[] { this.gridView };
            this.gridControl.ViewCollection.AddRange(views);

            this.gridView.GridControl = this.gridControl;
            this.gridView.PopulateColumns();

            RepositoryItemCheckEdit editLink0 = new RepositoryItemCheckEdit();
            this.gridView.Columns["State"].ColumnEdit = editLink0;
            editLink0.CheckedChanged += new EventHandler(this.Check_Click);

            RepositoryItemHyperLinkEdit editLink1 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Attach"].ColumnEdit = editLink1;
            editLink1.Click += new EventHandler(this.Attach_Click);

            RepositoryItemHyperLinkEdit editLink2 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["FileName"].ColumnEdit = editLink2;
            editLink2.Click += new EventHandler(this.View_Click);

            RepositoryItemHyperLinkEdit editLink3 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Clear"].ColumnEdit = editLink3;
            editLink3.Click += new EventHandler(this.Delete_Click);

            this.gridView.Columns["Path"].OptionsColumn.ReadOnly = true;
            this.gridView.Columns["FileName"].OptionsColumn.ReadOnly = true;
            this.gridView.Columns["Path"].Visible = false;
            this.gridView.Columns["Default"].Visible = false;
            this.gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

            this.gridView.InitNewRow += ((s, e) =>
            {
                GridView view = s as GridView;
                view.SetRowCellValue(e.RowHandle, view.Columns["Date"], DateTime.Now);
                view.SetRowCellValue(e.RowHandle, view.Columns["Attach"], "Attach");
                view.SetRowCellValue(e.RowHandle, view.Columns["Clear"], "Delete");
            });

            this.gridView.ShowingEditor += ((s, e) =>
            {
                DataRow focusedDataRow = this.gridView.GetFocusedDataRow();
                if (focusedDataRow != null && CommonFunctions.GetBoolSafely(focusedDataRow["Default"]) && this.gridView.FocusedColumn.FieldName.IsStringEqual("Step"))
                    e.Cancel = true;
            });
        }

        private void LoadData()
        {
            this.dataTable.Rows.Clear();

            if (this.currentWorkingOrderTask != null && this.currentWorkingOrder != null)
            {
                XElement notesElement = null;
                int checkCount = 0;

                if (!string.IsNullOrEmpty(this.currentWorkingOrderTask.Notes))
                {
                    notesElement = XElement.Parse(this.currentWorkingOrderTask.Notes);
                }
                else
                {
                    notesElement = StaticListDL.GetDefaultStepsForTask(this.currentWorkingOrderTask.TaskCode);
                }

                if (notesElement != null)
                {
                    IEnumerable<XElement> items = notesElement.Elements("item");

                    foreach (XElement item in items)
                    {
                        this.dataTable.Rows.Add(
                            CommonFunctions.GetBoolSafely(item.GetAttribute("default")),
                            CommonFunctions.GetBoolSafely(item.GetAttribute("state")),
                            CommonFunctions.GetStringSafely(item.GetAttribute("step")),
                            CommonFunctions.GetDateTimeSafely(item.GetAttribute("datetime")),
                            CommonFunctions.GetStringSafely(item.GetAttribute("notes")),
                            CommonFunctions.GetStringSafely(item.GetAttribute("path")),
                            CommonFunctions.GetStringSafely(item.GetAttribute("fileName")),
                            "Attach",
                            CommonFunctions.GetBoolSafely(item.GetAttribute("default")) ? "Clear" : "Delete");

                        if (CommonFunctions.GetBoolSafely(item.GetAttribute("state")))
                            checkCount++;
                    }

                    this.gridControl.RefreshDataSource();
                    this.gridControl.Refresh();
                }

                chkMarkasComplete.Checked = this.currentWorkingOrderTask.TaskStatus == E_TaskStatus.Complete;
                chkMarkasComplete.Enabled = checkCount == this.dataTable.Rows.Count;
                
            }
        }

        //private void LoadData1()
        //{
        //    List<OrderAssay> orderplacements = OrderAssayDL.GetOrderPlacements(this.currentWorkingOrder.Id);
        //    IEnumerable<XElement> items = this.currentWorkingOrder.Details.Elements("item");

        //    Action<string> addRowToDataTable = (assayid) =>
        //    {
        //        XElement assayelement = items.FirstOrDefault(x => x.GetAttribute("key").IsStringEqual(assayid));
        //        if (assayelement != null)
        //        {
        //            OrderAssay orderplacement = orderplacements.FirstOrDefault(x => x.AssayId.IsStringEqual(assayid));

        //            if (orderplacement == null)
        //            {
        //                this.dataTable1.Rows.Add(assayid, DateTime.Now, assayelement.GetAttribute("value"), string.Empty, string.Empty);
        //            }
        //            else
        //            {
        //                Manufacturer manufacturer = this.manufacturers.FirstOrDefault(x => x.Id.Equals(orderplacement.ManufacurerId));
        //                this.dataTable1.Rows.Add(orderplacement.AssayId, orderplacement.PlacementDate, assayelement.GetAttribute("value"), manufacturer.Name, orderplacement.Invoice);
        //            }
        //        }
        //    };

        //    addRowToDataTable("assay1");
        //    addRowToDataTable("assay2");
        //    addRowToDataTable("assay3");

        //    this.gridControl1.RefreshDataSource();
        //    this.gridControl1.Refresh();
        //}

        private void Check_Click(object sender, EventArgs e)
        {
            int checkCount = 0;
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            if (dataRow != null && sender != null)
                dataRow["State"] = ((CheckEdit)sender).Checked;

            foreach (DataRow row in this.dataTable.Rows)
            {
                if(CommonFunctions.GetBoolSafely(row["State"]))
                    checkCount++;
            }

            if(this.dataTable.Rows.Count == checkCount)
            {
                chkMarkasComplete.Enabled = true;
                chkMarkasComplete.Checked = true;
            }
            else
            {
                chkMarkasComplete.Enabled = false;
                chkMarkasComplete.Checked = false;
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
                    row = this.dataTable.Rows.Add(false, false, string.Empty, DateTime.Now, string.Empty, string.Empty, string.Empty, "Attach", "Delete");
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
                    if (CommonFunctions.GetBoolSafely(row["Default"].ToString()))
                    {
                        row["Path"] = string.Empty;
                        row["FileName"] = string.Empty;
                    }
                    else
                    {
                        this.dataTable.Rows.Remove(row);
                    }

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
