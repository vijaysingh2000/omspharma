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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using oms.DataAccessLayer;
using oms.Model;
using DevExpress.XtraPrinting.NativeBricks;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using Tasks = oms.Model.Tasks;
using System.Runtime.CompilerServices;
using DevExpress.XtraSpreadsheet.Import.OpenXml;

namespace oms.Forms
{
    public partial class FinalPacketForm : Form
    {
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;
        private Guid orderId = Guid.Empty;

        public FinalPacketForm(Guid orderId)
        {
            InitializeComponent();
            this.orderId = orderId; 
        }

        private void FinalPacketForm_Load(object sender, EventArgs e)
        {
            this.LoadGrid();
            this.LoadData();
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Sequence", typeof(int));
            this.dataTable.Columns.Add("Include", typeof(bool));
            this.dataTable.Columns.Add("context", typeof(string));
            this.dataTable.Columns.Add("Steps", typeof(string));
            this.dataTable.Columns.Add("Notes", typeof(string));
            this.dataTable.Columns.Add("Path", typeof(string));
            this.dataTable.Columns.Add("FileName", typeof(string));

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

            RepositoryItemHyperLinkEdit editLink2 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["FileName"].ColumnEdit = editLink2;
            editLink2.Click += new EventHandler(this.View_Click);

            this.gridView.Columns["Path"].Visible = false;
            this.gridView.Columns["FileName"].OptionsColumn.ReadOnly = true;

            this.gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
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

        private void LoadData()
        {
            List<OrderTask> orderTasks = OrderTasksDL.Get(this.orderId);
            List<OrderPaymentInsurance> orderPaymentInsurances = OrderPaymentsInsuranceDL.Get(this.orderId);
            List<OrderPaymentClient> orderPaymentClients = OrderPaymentsClientDL.Get(this.orderId);
            List<Tasks> tasks = StaticListDL.GetAllTasks();
            int seqnum = 1;

            this.dataTable.Rows.Clear();

            string headerFile = CommonFunctions.BuildHeaderHtmlFile(this.orderId);
            this.dataTable.Rows.Add(seqnum, true,"Header",string.Empty,string.Empty, headerFile, Path.GetFileName(headerFile));
            seqnum++;

            string orderFile = CommonFunctions.BuildOrderHtmlFile(this.orderId);
            this.dataTable.Rows.Add(seqnum, true, "Order", string.Empty, string.Empty, orderFile, Path.GetFileName(orderFile));
            seqnum++;
                
            foreach (OrderTask orderTask in orderTasks)
            {
                Tasks task = tasks.FirstOrDefault(x => x.Code.IsStringEqual(orderTask.TaskCode));

                XElement elements = CommonFunctions.GetXmlSafely(orderTask.Notes);

                IEnumerable<XElement> items = elements.Elements("item");

                foreach (XElement item in items)
                {
                    if (!string.IsNullOrEmpty(item.GetAttribute("path").Trim()))
                    {
                        this.dataTable.Rows.Add(
                            seqnum,
                            true,
                            task != null ? task.Name : orderTask.TaskCode,
                            item.GetAttribute("step"),
                            item.GetAttribute("notes"),
                            item.GetAttribute("path"),
                            item.GetAttribute("fileName")
                            );

                        seqnum++;
                    }
                }

                
            }

            Tasks insurancePayTask = tasks.FirstOrDefault(x => x.Code.IsStringEqual("IPAY"));
            foreach (OrderPaymentInsurance orderPayment in orderPaymentInsurances)
            {
                this.dataTable.Rows.Add(
                    0,
                    true,
                    insurancePayTask != null ? insurancePayTask.Name : "IPAY",
                    $"{orderPayment.ChequeNumber}",
                    $"{orderPayment.Notes}",
                    $"{orderPayment.Path}",
                    $"{orderPayment.FileName}"
                    );

                seqnum++;
            }

            Tasks clientPayTask = tasks.FirstOrDefault(x => x.Code.IsStringEqual("UCDPAY"));
            foreach (OrderPaymentClient orderPayment in orderPaymentClients)
            {
                this.dataTable.Rows.Add(
                    0,
                    true,
                    clientPayTask != null ? clientPayTask.Name : "UCDPAY",
                    $"{orderPayment.ChequeNumber}",
                    $"{orderPayment.Notes}",
                    $"{orderPayment.Path}",
                    $"{orderPayment.FileName}"
                    );

                seqnum++;
            }

            this.dataTable.DefaultView.Sort = "Sequence";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            List<string> allFiles = new List<string>();
            this.gridControl.RefreshDataSource();
            this.gridControl.Refresh();

            string destFolder = Path.Combine(Path.GetTempPath(), @$"{this.orderId}\packet");
            if (Directory.Exists(destFolder))
                Directory.Delete(destFolder, true);

            Directory.CreateDirectory(destFolder);

            foreach (DataRow dataRow in this.dataTable.Rows)
            {
                string sourceFilePath = CommonFunctions.GetStringSafely(dataRow["Path"]);
                string destFilePath = Path.Combine(destFolder, Path.GetFileName(sourceFilePath));

                File.Copy(sourceFilePath, destFilePath, true);

                string pdfFilePath = string.Empty;
                string extension = Path.GetExtension(destFilePath).Trim().ToLower();

                switch(extension)
                {
                    case ".xlsx":
                        pdfFilePath = PdfLibrary.ExportExcelToPdf(destFilePath);
                        break;

                    case ".docx":
                    case ".html":
                    case ".txt":
                    case ".log":
                        pdfFilePath = PdfLibrary.ExportWordToPdf(destFilePath);
                        break;

                    case ".jpg":
                    case ".png":
                    case ".jpeg":
                        pdfFilePath = PdfLibrary.ExportImageToPdf(destFilePath);
                        break;

                    case ".pdf":
                        pdfFilePath = destFilePath;
                        break;
                }

                if (!string.IsNullOrEmpty(pdfFilePath))
                {
                    allFiles.Add(pdfFilePath);
                }
            }

            string finalFile = PdfLibrary.CombineMultiplePdf(allFiles.ToArray(), destFolder);

            CommonFunctions.OpenFile(finalFile);
        }
    }
}

