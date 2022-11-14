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
using DevExpress.Utils.Extensions;
using DevExpress.Utils.FormShadow;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid.Handler;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using oms.Controls;
using oms.DataAccessLayer;
using oms.Model;
namespace oms.Forms
{
    public partial class ListForm : Form
    {
        private List<(string,string)> listItems;

        public ListForm()
        {
            InitializeComponent();
        }

        private void LoadData(string defaultTaskCode = "")
        {
            this.listItems = MiscDL.GetAllListItems();

            lstItems.DataSource = null;
            lstItems.Items.Clear();
            foreach ((string,string) item in this.listItems)
                lstItems.Items.Add(item.Item2);

            if (string.IsNullOrEmpty(defaultTaskCode))
            {
                if (lstItems.Items.Count > 0)
                    lstItems.SelectedIndex = 0;
            }
            else
            {
                int index = listItems.IndexOf(listItems.FirstOrDefault(x => x.Item1.IsStringEqual(defaultTaskCode)));
                if (index > -1)
                    lstItems.SelectedIndex = index;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.groupDetails.Controls)
            {
                if (ctrl as IListUserControl != null)
                    (ctrl as IListUserControl).SaveControl();
            }
        }

        private UserControl GetControlToDispatch(string code)
        {
            return new List_Basic_UC();
        }

        private void ListForm_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogText("List");

            this.LoadData();
        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.groupDetails.Controls)
            {
                if (ctrl as IListUserControl != null)
                    this.groupDetails.Controls.Remove(ctrl);
            }

            if (lstItems.SelectedIndex >= 0)
            {
                (string, string) item = this.listItems[lstItems.SelectedIndex];
                UserControl orderTaskUserControl = GetControlToDispatch(item.Item1);
                this.groupDetails.Text = item.Item2;

                if (orderTaskUserControl as IListUserControl != null)
                {
                    this.groupDetails.Controls.Add(orderTaskUserControl);
                    orderTaskUserControl.Dock = DockStyle.Fill;
                    orderTaskUserControl.Visible = true;

                    ((IListUserControl)orderTaskUserControl).LoadControl(item.Item1);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
