using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Data.Mask.Internal;
using DevExpress.SpreadsheetSource.Xlsx.Import.Internal;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.FormShadow;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport.Implementation;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid.Handler;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using oms.Controls;
using oms.DataAccessLayer;
using oms.Model;
namespace oms.Forms
{
    public partial class ReportsForm : Form
    {
        private List<(string, string)> reports;

        public ReportsForm()
        {
            InitializeComponent();
        }

        private void OrderStatus_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogText("Reports");

            this.LoadData();
        }

        private void LoadData(string defaultTaskCode = "")
        {
            this.reports = MiscDL.GetAllReports();

            lstTasks.DataSource = null;
            lstTasks.Items.Clear();
            foreach ((string, string) item in this.reports)
                lstTasks.Items.Add(item.Item2);

            if (string.IsNullOrEmpty(defaultTaskCode))
            {
                if (lstTasks.Items.Count > 0)
                    lstTasks.SelectedIndex = 0;
            }
            else
            {
                int index = reports.IndexOf(reports.FirstOrDefault(x => x.Item1.IsStringEqual(defaultTaskCode)));
                if (index > -1)
                    lstTasks.SelectedIndex = index;
            }
        }

        private void lstTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.groupDetails.Controls)
            {
                if (ctrl as IReportUserControl != null)
                    this.groupDetails.Controls.Remove(ctrl);
            }

            if (lstTasks.SelectedIndex >= 0)
            {
                (string, string) item = this.reports[lstTasks.SelectedIndex];
                UserControl orderTaskUserControl = GetControlToDispatch(item.Item1);

                if (orderTaskUserControl as IReportUserControl != null)
                {
                    this.groupDetails.Controls.Add(orderTaskUserControl);
                    orderTaskUserControl.Dock = DockStyle.Fill;
                    orderTaskUserControl.Visible = true;

                    ((IReportUserControl)orderTaskUserControl).LoadControl();
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.groupDetails.Controls)
            {
                if (ctrl as IReportUserControl != null)
                    (ctrl as IReportUserControl).SaveControl();
            }
        }

        private UserControl GetControlToDispatch(string code)
        {
            switch (code.ToUpper().Trim())
            {
                case "PURCHASEREPORT":

                    return new Report_PurchasingReport_UC();

                case "MONTHLYINVOICE":

                    return new Report_MonthlyInvoice_UC();

                default:

                    return null;
            }
        }

        private void btnExportToCSV_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdateExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files(*.xlsx)|*.xlsx";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (Control ctrl in this.groupDetails.Controls)
                {
                    if (ctrl as IReportUserControl != null)
                        (ctrl as IReportUserControl).UpdateExcel(openFileDialog.FileName);
                }

                if (MessageBox.Show($"Update Completed. Wish you open the file => {openFileDialog.FileName}",
                    CommonFunctions.GetDialogText(String.Empty),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    CommonFunctions.OpenFile(openFileDialog.FileName);
                }
            }
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files(*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (Control ctrl in this.groupDetails.Controls)
                {
                    if (ctrl as IReportUserControl != null)
                        (ctrl as IReportUserControl).ExportToExcel(saveFileDialog.FileName);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
