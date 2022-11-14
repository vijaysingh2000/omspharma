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
    public partial class OrderTasks : Form
    {
        private Guid orderId;
        private Order currentWorkingOrder;
        private Patient currentPatient;

        public OrderTasks(Guid oid)
        {
            this.orderId = oid;
            InitializeComponent();
        }

        private void OrderStatus_Load(object sender, EventArgs e)
        {
            this.GetOrderAndPatientData();
            this.LoadData();
        }

        public void GetOrderAndPatientData()
        {
            this.currentWorkingOrder = OrdersDL.Get(orderId);
            this.currentPatient = PatientsDL.Get(this.currentWorkingOrder.PatientId);
            this.Text = CommonFunctions.GetDialogTextWithUser(this.currentWorkingOrder.OrderNumber);
            lblOrderNumber.Text = this.currentWorkingOrder.OrderNumber;
            lblPatient.Text = $"{this.currentPatient.MRN} => {this.currentPatient.LastName}, {this.currentPatient.FirstName}";
        }

        private void LoadData(string defaultTaskCode = "")
        {
            List<Model.OrderTask> orderTasks;
            List<Tasks> tasks;

            orderTasks = OrderTasksDL.Get(this.currentWorkingOrder.Id);
            if(orderTasks.Count == 0)
            {
                int returnValue = OrderTasksDL.AddAllTasks(this.currentWorkingOrder.Id);
                if(returnValue > 0)
                    orderTasks = OrderTasksDL.Get(this.currentWorkingOrder.Id);
            }

            tasks = StaticListDL.GetAllTasks();

            foreach (Model.OrderTask orderTask in orderTasks)
            {
                Tasks task = tasks.FirstOrDefault(x => x.Code.IsStringEqual(orderTask.TaskCode));

                orderTask.TaskName = task != null ? CommonFunctions.ReplacePlaceHolders(task.Name) : orderTask.TaskCode;
            }

            lstTasks.DataSource = null;
            lstTasks.Items.Clear();
            lstTasks.DataSource = orderTasks;
            lstTasks.DisplayMember = "TaskName";

            if (string.IsNullOrEmpty(defaultTaskCode))
            {
                if (lstTasks.Items.Count > 0)
                    lstTasks.SelectedIndex = 0;
            }
            else
            {
                int index = tasks.IndexOf(tasks.FirstOrDefault(x => x.Code.IsStringEqual(defaultTaskCode)));
                if (index > -1)
                    lstTasks.SelectedIndex = index;
            }
        }

        private void lstTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.groupDetails.Controls)
            {
                if (ctrl as IOrderTaskUserControl != null)
                    this.groupDetails.Controls.Remove(ctrl);
            }

            OrderTask selectedTask = (OrderTask)lstTasks.SelectedItem;
            if (selectedTask != null)
            {
                Model.OrderTask orderState = OrderTasksDL.Get(this.currentWorkingOrder.Id, selectedTask.TaskCode);

                if (orderState != null)
                {
                    txtLastUpdatedBy.Text = orderState.LastUpdatedByName;
                    txtLastUpdateDttm.Text = orderState.LastUpdatedDate.ToString();

                    UserControl orderTaskUserControl = GetControlToDispatch(selectedTask.TaskCode);

                    if (orderTaskUserControl as IOrderTaskUserControl != null)
                    {
                        this.groupDetails.Controls.Add(orderTaskUserControl);
                        orderTaskUserControl.Dock = DockStyle.Fill;
                        orderTaskUserControl.Visible = true;

                        ((IOrderTaskUserControl)orderTaskUserControl).LoadControl(this.currentWorkingOrder.Id, selectedTask.TaskCode);
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.groupDetails.Controls)
            {
                if (ctrl as IOrderTaskUserControl != null)
                    (ctrl as IOrderTaskUserControl).SaveControl();
            }
        }

        private UserControl GetControlToDispatch(string code)
        {
            switch (code)
            {
                case "IPAY":
                    return new Task_PaymentFromInsurance_UC();

                case "UCDPAY":
                    return new Task_PaymentFromUCD_UC();

                default:

                    return new Task_Others_UC();
            }
        }

        private void btnViewOrder_Click(object sender, EventArgs e)
        {
            
        }

        private void lblOrderNumber_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OrderDetails orderDetails = new OrderDetails(this.currentWorkingOrder.PatientId, this.currentWorkingOrder.Id);
            if (orderDetails.ShowDialog() == DialogResult.OK)
            {
                this.GetOrderAndPatientData();

                foreach (Control ctrl in this.groupDetails.Controls)
                {
                    if (ctrl as IOrderTaskUserControl != null)
                        (ctrl as IOrderTaskUserControl).RefreshControl();
                }
            }
        }

        private void lblPatient_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PatientDetailsForm patientDetailsForm = new PatientDetailsForm(this.currentWorkingOrder.PatientId);
            if (patientDetailsForm.ShowDialog() == DialogResult.OK)
            {
                this.GetOrderAndPatientData();

                foreach (Control ctrl in this.groupDetails.Controls)
                {
                    if (ctrl as IOrderTaskUserControl != null)
                        (ctrl as IOrderTaskUserControl).RefreshControl();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
