using DevExpress.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace oms.Controls
{
    public partial class Common_DatePicker_UC : UserControl
    {
        public Common_DatePicker_UC()
        {
            InitializeComponent();
        }

        public delegate void ShowReportFromRange(DateTime startDate, DateTime endDate);
        public ShowReportFromRange OnShowReportFromRange;

        private void btnShowDateRange_Click(object sender, EventArgs e)
        {
            if (OnShowReportFromRange != null)
            {
                DateTime st = new DateTime(txtStartDate.Value.Year, txtStartDate.Value.Month, txtStartDate.Value.Day, 0, 0, 0);
                DateTime ed = new DateTime(txtEndDate.Value.Year, txtEndDate.Value.Month, txtEndDate.Value.Day, 23, 59, 59);
                OnShowReportFromRange(st, ed);
            }
        }

        private void btnShowReportYearMoth_Click(object sender, EventArgs e)
        {
            if (OnShowReportFromRange != null)
            {
                int year = CommonFunctions.GetIntSafely(cmbYear.Text);
                int month = cmbMonth.SelectedIndex + 1;

                DateTime st = new DateTime(year, month, 1, 0, 0, 0);
                DateTime ed = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
                OnShowReportFromRange(st, ed);
            }
        }

        private void Common_DatePicker_UC_Load(object sender, EventArgs e)
        {
            cmbYear.Items.Clear();
            for (int iloop = DateTime.Now.Year; iloop > DateTime.Now.Year - 10; iloop--)
                cmbYear.Items.Add(iloop.ToString());

            cmbYear.SelectedIndex = 0;
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;
        }
    }
}
