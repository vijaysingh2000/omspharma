using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Filtering.Templates;
using oms.DataAccessLayer;
using oms.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace oms.Forms
{
    public partial class OrderDetails : Form
    {
        private Patient Patient = null;
        private Order WorkingOrder = null;
        private List<OrderAssay> WorkingOrderAssay = null;
        private bool NewOrder = false;
        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;

        public OrderDetails(Guid patientId)
        {
            InitializeComponent();

            this.Patient = PatientsDL.Get(patientId);
            this.SetupNewOrderObject();
            this.NewOrder = true;


        }

        public OrderDetails(Guid patientId, Guid orderId)
        {
            InitializeComponent();

            this.Patient = PatientsDL.Get(patientId);
            this.WorkingOrder = OrdersDL.Get(orderId, true);

            if (this.WorkingOrder == null)
                this.SetupNewOrderObject();
        }

        private void SetupNewOrderObject()
        {
            this.WorkingOrder = new Order()
            {
                Id = Guid.NewGuid(),
                PatientId = this.Patient.Id,
                InsuranceId = this.Patient.InsuranceId,
                OrderStatus = E_TaskStatus.InProgress,
            };
            if (this.Patient.DefaultAddressType <= 1)
                this.WorkingOrder.DeliveryAddress = this.Patient.Address1;
            else if (this.Patient.DefaultAddressType == 2)
                this.WorkingOrder.DeliveryAddress = this.Patient.Address2;
            else if (this.Patient.DefaultAddressType == 3)
                this.WorkingOrder.DeliveryAddress = this.Patient.Address3;

            this.NewOrder = true;
        }

        private void OrderDetails_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogTextWithUser("Order Details");

            if(this.Patient == null)
            {
                CommonFunctions.ShowErrorMessage("Patient not found.");
                this.Close();
            }

            this.LoadGrid();
            this.LoadData();
        }

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Description", typeof(string));
            this.dataTable.Columns.Add("Item #", typeof(string));
            this.dataTable.Columns.Add("Qty", typeof(string));
            this.dataTable.Columns.Add("Lot/Exp", typeof(string));
            this.dataTable.Columns.Add("Delete", typeof(string));

            this.gridControl = new GridControl();
            this.groupSupplies.Controls.Add(this.gridControl);
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
            this.gridView.Columns["Delete"].ColumnEdit = editLink;
            editLink.Click += new EventHandler(this.Delete_Click);

            this.gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            //this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

            gridView.InitNewRow += (s, e) => {
                GridView view = s as GridView;
                view.SetRowCellValue(e.RowHandle, view.Columns["Delete"], "Delete");
            };
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            DataRow dataRow = this.gridView.GetFocusedDataRow();
            this.dataTable.Rows.Remove(dataRow);
            this.gridControl.Refresh();
        }

        private void SetComboBox(ComboBox comboBox, List<BasicModel> basicModels, int compareId, bool selectFirstIfNotFound = false)
        {
            comboBox.DataSource = basicModels;
            comboBox.DisplayMember = "Name";
            comboBox.SelectedItem = basicModels.FirstOrDefault(x => x.Id.Equals(compareId));
            if (comboBox.SelectedItem == null)
                comboBox.SelectedIndex = 0;
        }

        private void LoadData()
        {
            List<BasicModel> drugs = ListDL.GetActive(Constants.TableName_Drug);
            List<BasicModel> manufacturers = ListDL.GetActive(Constants.TableName_Manufacturer);
            List<BasicModel> insurances = ListDL.GetActive(Constants.TableName_Insurance);
            List<BasicModel> providers = ListDL.GetActive(Constants.TableName_Providers);
            List<BasicModel> acceptableOutdates = StaticListDL.GetActive(Constants.TableName_Acceptableoutdates);
            List<BasicModel> id340B = StaticListDL.GetActive(Constants.TableName_id340B);

            this.WorkingOrderAssay = OrderAssayDL.Get(this.WorkingOrder.Id);

            this.UpdatePatient();

            txtDeliveryAddress.Text = this.WorkingOrder.DeliveryAddress;
            txtDateofService.Value = this.WorkingOrder.DOS;
            txtConfirmedDOS.Value = this.WorkingOrder.ConfirmedDOS;
            txtConfirmedDeliveryDate.Value = this.WorkingOrder.ConfirmedDeliveryDate;
            txtEstimatedDeliveryDate.Value = this.WorkingOrder.EstimatedDeliveryDate;
            txtNextCall.Value = this.WorkingOrder.NextCallDate;
            txtProphyPRN.Text = this.WorkingOrder.ProphyOrPRN;
            SetComboBox(txtDrugName, drugs, this.WorkingOrder.DrugId);
            SetComboBox(txtManufacturer, manufacturers, this.WorkingOrder.ManufacturerId, true);
            SetComboBox(txtInsurancePayor, insurances, this.WorkingOrder.InsuranceId, true);
            SetComboBox(txtProviderName, providers, this.WorkingOrder.ProviderId, true);
            SetComboBox(txtAcceptableOutdate, acceptableOutdates, this.WorkingOrder.AcceptableOutdatesId, true);
            SetComboBox(txt340BID, id340B, this.WorkingOrder.Id340B, true);
            txtTotalPrescribedUnits.Text = this.WorkingOrder.TotalPrescribedUnit.ToString();
            txtDoseCount.Text = this.WorkingOrder.DoseCount.ToString();
            txtCOGPerUnit.Text = this.WorkingOrder.CogPerUnit.ToString();
            txtBilledPerUnit.Text = this.WorkingOrder.BillPerUnit.ToString();
            txtPONumber.Text = this.WorkingOrder.OrderNumber;

            OrderAssay orderAssay = this.WorkingOrderAssay.FirstOrDefault(x => x.AssayId.IsStringEqual("assay1"));
            if(orderAssay != null)
            {
                txtAssat1.Text = orderAssay.Assay.ToString();
                txtNDC1.Text = orderAssay.NDC;
                txtQuantity1.Text = orderAssay.Qty.ToString();
                txtExpDate1.Value = orderAssay.ExpDate;
                txtLotNumber1.Text = orderAssay.Lot;
                txtRXNumber1.Text = orderAssay.RxNumber;
            }
            orderAssay = this.WorkingOrderAssay.FirstOrDefault(x => x.AssayId.IsStringEqual("assay2"));
            if (orderAssay != null)
            {
                txtAssat2.Text = orderAssay.Assay.ToString();
                txtNDC2.Text = orderAssay.NDC;
                txtQuantity2.Text = orderAssay.Qty.ToString();
                txtExpDate2.Value = orderAssay.ExpDate;
                txtLotNumber2.Text = orderAssay.Lot;
                txtRXNumber2.Text = orderAssay.RxNumber;
            }
            orderAssay = this.WorkingOrderAssay.FirstOrDefault(x => x.AssayId.IsStringEqual("assay3"));
            if (orderAssay != null)
            {
                txtAssat3.Text = orderAssay.Assay.ToString();
                txtNDC3.Text = orderAssay.NDC;
                txtQuantity3.Text = orderAssay.Qty.ToString();
                txtExpDate3.Value = orderAssay.ExpDate;
                txtLotNumber3.Text = orderAssay.Lot;
                txtRXNumber3.Text = orderAssay.RxNumber;
            }

            IEnumerable<XElement> elements = this.WorkingOrder.Details.Elements("item");
            txtDirection.Text = elements.GetItemValue("directions");
            txtDosesOnHand.Text = elements.GetItemValue("noofprophyinhand");
            txtWillInfuseLastDose.Text = elements.GetItemValue("willinfuselastdose");
            txtNumberofPRNDosesInHand.Text = elements.GetItemValue("noofprndosesinhand");
            txtInsuranceEligible.Text = elements.GetItemValue("insuranceeligible");
            txtPriorAuthApprovalDates.Text = elements.GetItemValue("priorauthapproval");
            txtAdvanceFactorNeeded.Text = elements.GetItemValue("advancefactorneeded");//
            txtNotes.Text = elements.GetItemValue("notes");
            txtSuppliesMedication.Text = elements.GetItemValue("suppliesmedication");
            txtInsuranceIssues.Text = elements.GetItemValue("insuranceissues");
            txtNotifiedNurseofBleeds.Text = elements.GetItemValue("notifiednurseofbleeds");
            txtUpdatestoreporttoHTC.Text = elements.GetItemValue("updatetoreporttohtc");
            txtCCSCaseNumber.Text = elements.GetItemValue("ccscase#");
            txtIVAccess.Text = elements.GetItemValue("ivaccess");
            txtLastMDVisit.Value = CommonFunctions.GetDateTimeSafely(elements.GetItemValue("lastmdvisit"));
            txtWeight.Text = elements.GetItemValue("weight");
            txtTravelPlans.Text = elements.GetItemValue("travelplans");
            txtProcedures.Text = elements.GetItemValue("procedures");
            txtPTEducation.Text = elements.GetItemValue("pteducation");//
            txtPatientRXScriptInfo.Text = elements.GetItemValue("patientrxscriptinfo");
            txtFillCount1.Text = elements.GetItemValue("fillcount1");
            txtFillCount2.Text = elements.GetItemValue("fillcount2");
            txtBillingCount1.Text = elements.GetItemValue("billingfillcount1");
            txtBillingCount2.Text = elements.GetItemValue("billingfillcount2");
            txtWrittenDate.Text = elements.GetItemValue("writtendate");//
            txtPercentageofvariance.Text = elements.GetItemValue("percentageofvariance");
            
            
            XElement suppliesElement = this.WorkingOrder.Details.Element("supplies");
            if(suppliesElement != null)
            {
                dataTable.Rows.Clear();
                IEnumerable<XElement> supplies = suppliesElement.Elements("item");
                foreach (XElement item in supplies)
                {
                    dataTable.Rows.Add(item.GetAttribute("description"),
                        item.GetAttribute("itemnumber"),
                        item.GetAttribute("qty"),
                        item.GetAttribute("lot"), 
                        "Delete");
                }

                this.gridControl.RefreshDataSource();
                this.gridControl.Refresh();
            }

            this.lblOrderNumber.Text = this.GenerateAndValidateOrderNumber(this.WorkingOrder.DOS, this.WorkingOrder.ProphyOrPRN, this.WorkingOrder.Id);
        }

        private void Set()
        {
            this.WorkingOrder.DeliveryAddress = txtDeliveryAddress.Text = this.WorkingOrder.DeliveryAddress;
            this.WorkingOrder.DOS = txtDateofService.Value;
            this.WorkingOrder.ConfirmedDOS= txtConfirmedDOS.Value;
            this.WorkingOrder.ConfirmedDeliveryDate = txtConfirmedDeliveryDate.Value;
            this.WorkingOrder.EstimatedDeliveryDate = txtEstimatedDeliveryDate.Value;
            this.WorkingOrder.NextCallDate = txtNextCall.Value;
            this.WorkingOrder.ProphyOrPRN = txtProphyPRN.Text;
            this.WorkingOrder.DrugId = ((BasicModel)txtDrugName.SelectedItem).Id;
            this.WorkingOrder.ManufacturerId = ((BasicModel)txtManufacturer.SelectedItem).Id;
            this.WorkingOrder.InsuranceId = ((BasicModel)txtInsurancePayor.SelectedItem).Id;
            this.WorkingOrder.DateOrdered = txtDateOrdered.Value;
            this.WorkingOrder.ConfirmationNumber = txtConfirmationNum.Text;
            this.WorkingOrder.TotalPrescribedUnit = CommonFunctions.GetFloatSafely(txtTotalPrescribedUnits.Text);
            this.WorkingOrder.DoseCount = CommonFunctions.GetFloatSafely(txtDoseCount.Text);
            this.WorkingOrder.CogPerUnit = CommonFunctions.GetFloatSafely(txtCOGPerUnit.Text);
            this.WorkingOrder.BillPerUnit = CommonFunctions.GetFloatSafely(txtBilledPerUnit.Text);
            this.WorkingOrder.ProviderId = ((BasicModel)txtProviderName.SelectedItem).Id;
            this.WorkingOrder.Id340B = ((BasicModel)txt340BID.SelectedItem).Id;
            this.WorkingOrder.AcceptableOutdatesId = ((BasicModel)txtAcceptableOutdate.SelectedItem).Id;


            this.WorkingOrderAssay.Clear();
            OrderAssay assay = new OrderAssay()
            {
                Assay = CommonFunctions.GetFloatSafely(txtAssat1.Text),
                AssayId = "assay1",
                ExpDate = txtExpDate1.Value,
                Lot = txtLotNumber1.Text,
                NDC = txtNDC1.Text,
                OrderId = this.WorkingOrder.Id,
                Qty = CommonFunctions.GetFloatSafely(txtQuantity1.Text),
                RxNumber = txtRXNumber1.Text
            };
            this.WorkingOrderAssay.Add(assay);
            assay = new OrderAssay()
            {
                Assay = CommonFunctions.GetFloatSafely(txtAssat2.Text),
                AssayId = "assay2",
                ExpDate = txtExpDate2.Value,
                Lot = txtLotNumber2.Text,
                NDC = txtNDC2.Text,
                OrderId = this.WorkingOrder.Id,
                Qty = CommonFunctions.GetFloatSafely(txtQuantity2.Text),
                RxNumber = txtRXNumber2.Text
            };
            this.WorkingOrderAssay.Add(assay);
            assay = new OrderAssay()
            {
                Assay = CommonFunctions.GetFloatSafely(txtAssat3.Text),
                AssayId = "assay3",
                ExpDate = txtExpDate3.Value,
                Lot = txtLotNumber3.Text,
                NDC = txtNDC3.Text,
                OrderId = this.WorkingOrder.Id,
                Qty = CommonFunctions.GetFloatSafely(txtQuantity3.Text),
                RxNumber = txtRXNumber3.Text
            };
            this.WorkingOrderAssay.Add(assay);

            XElement elements = this.WorkingOrder.Details;
            elements.RemoveAll();

            elements.SetItemValue("directions", txtDirection.Text);
            elements.SetItemValue("noofprophyinhand", txtDosesOnHand.Text);
            elements.SetItemValue("willinfuselastdose", txtWillInfuseLastDose.Text);
            elements.SetItemValue("noofprndosesinhand", txtNumberofPRNDosesInHand.Text);
            elements.SetItemValue("insuranceeligible", txtInsuranceEligible.Text);
            elements.SetItemValue("priorauthapproval", txtPriorAuthApprovalDates.Text);
            elements.SetItemValue("advancefactorneeded", txtAdvanceFactorNeeded.Text);
            elements.SetItemValue("notes", txtNotes.Text);
            elements.SetItemValue("suppliesmedication", txtSuppliesMedication.Text);
            elements.SetItemValue("insuranceissues", txtInsuranceIssues.Text);
            elements.SetItemValue("notifiednurseofbleeds", txtNotifiedNurseofBleeds.Text);
            elements.SetItemValue("updatetoreporttohtc", txtUpdatestoreporttoHTC.Text);
            elements.SetItemValue("ccscase#", txtCCSCaseNumber.Text);
            elements.SetItemValue("ivaccess", txtIVAccess.Text);
            elements.SetItemValue("lastmdvisit", txtLastMDVisit.Text);
            elements.SetItemValue("weight", txtWeight.Text);
            elements.SetItemValue("travelplans", txtTravelPlans.Text);
            elements.SetItemValue("procedures", txtProcedures.Text);
            elements.SetItemValue("notes", txtNotes.Text);
            elements.SetItemValue("pteducation", txtPTEducation.Text);
            elements.SetItemValue("patientrxscriptinfo", txtPatientRXScriptInfo.Text);
            elements.SetItemValue("fillcount1", txtFillCount1.Text);
            elements.SetItemValue("fillcount2", txtFillCount2.Text);
            elements.SetItemValue("billingfillcount1", txtBillingCount1.Text);
            elements.SetItemValue("billingfillcount2", txtBillingCount2.Text);
            elements.SetItemValue("writtendate", txtWrittenDate.Text);
            elements.SetItemValue("percentageofvariance", txtPercentageofvariance.Text);

            XElement suppliesElement = new XElement("supplies");
            foreach (DataRow dataRow in this.dataTable.Rows)
            {
                XElement item = new XElement("item");
                item.SetAttributeValue("description", dataRow["Description"]);
                item.SetAttributeValue("itemnumber", dataRow["Item #"]);
                item.SetAttributeValue("qty", dataRow["Qty"]);
                item.SetAttributeValue("lot", dataRow["Lot/Exp"]);
                suppliesElement.Add(item);
            }
            elements.Add(suppliesElement);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.WorkingOrder.OrderNumber = this.GenerateAndValidateOrderNumber(txtDateofService.Value, txtProphyPRN.Text, this.WorkingOrder.Id);

            this.Set();

            if (this.NewOrder)
            {
                this.WorkingOrder.OrderStatus = E_TaskStatus.InProgress;
                int result = OrdersDL.Add(this.WorkingOrder, this.WorkingOrderAssay, StaticListDL.GetAllTasks());
                if (result > 0)
                {
                    CommonFunctions.ShowInfomationMessage("Congratulations. New Order Created Successfully.");
                    this.NewOrder = false;
                }
            }
            else
            {
                int result = OrdersDL.Update(this.WorkingOrder, this.WorkingOrderAssay);
                if (result > 0)
                    CommonFunctions.ShowInfomationMessage("Order Updated Successfully.");
            }
        }

        private string GenerateAndValidateOrderNumber(DateTime dateofservice, string prophyorprn, Guid ignoreOrderId)
        {
            int trial = 1;
            string aporan = prophyorprn.IsStringEqual("Prophy") ? "AP" : "AN";
            string orderNumber = aporan + dateofservice.Month + dateofservice.Day + dateofservice.Year + this.Patient.FirstName[0].ToString().ToUpper() + this.Patient.LastName[0].ToString().ToUpper();
            string returnValue = orderNumber;

            while (true)
            {
                Order order = OrdersDL.Get(returnValue);
                if (order == null || (order != null && order.Id == ignoreOrderId))
                { 
                    break;
                }
                else
                {
                    returnValue = orderNumber + "-" + trial;
                    trial++;
                }
            }

            return returnValue;
        }

        private void CalculateAll()
        {
            txtProduct1.Text = txtDrugName.Text;
            txtProduct2.Text = txtDrugName.Text;
            txtProduct3.Text = txtDrugName.Text;

            txtTotalUnits1.Text = (CommonFunctions.GetIntSafely(txtAssat1.Text) * CommonFunctions.GetIntSafely(txtQuantity1.Text)).ToString();
            txtTotalUnits2.Text = (CommonFunctions.GetIntSafely(txtAssat2.Text) * CommonFunctions.GetIntSafely(txtQuantity2.Text)).ToString();
            txtTotalUnits3.Text = (CommonFunctions.GetIntSafely(txtAssat3.Text) * CommonFunctions.GetIntSafely(txtQuantity3.Text)).ToString();

            txtTotalCOGEach1.Text = Math.Round(CommonFunctions.GetIntSafely(txtTotalUnits1.Text) * CommonFunctions.GetFloatSafely(txtCOGPerUnit.Text), 2).ToString();
            txtTotalCOGEach2.Text = Math.Round(CommonFunctions.GetIntSafely(txtTotalUnits2.Text) * CommonFunctions.GetFloatSafely(txtCOGPerUnit.Text), 2).ToString();
            txtTotalCOGEach3.Text = Math.Round(CommonFunctions.GetIntSafely(txtTotalUnits3.Text) * CommonFunctions.GetFloatSafely(txtCOGPerUnit.Text), 2).ToString();

            txtBillChargeEach1.Text = Math.Round(CommonFunctions.GetIntSafely(txtTotalUnits1.Text) * CommonFunctions.GetFloatSafely(txtBilledPerUnit.Text), 2).ToString();
            txtBillChargeEach2.Text = Math.Round(CommonFunctions.GetIntSafely(txtTotalUnits2.Text) * CommonFunctions.GetFloatSafely(txtBilledPerUnit.Text), 2).ToString();
            txtBillChargeEach3.Text = Math.Round(CommonFunctions.GetIntSafely(txtTotalUnits3.Text) * CommonFunctions.GetFloatSafely(txtBilledPerUnit.Text), 2).ToString();

            txtTotalUnitsOrdered.Text = (CommonFunctions.GetIntSafely(txtAssat1.Text) + CommonFunctions.GetIntSafely(txtAssat2.Text) + CommonFunctions.GetIntSafely(txtAssat3.Text)).ToString();

            txtTotalUnitsPrescribed.Text = (CommonFunctions.GetIntSafely(txtTotalPrescribedUnits.Text) * CommonFunctions.GetIntSafely(txtDoseCount.Text)).ToString();
            txtTotalUnitstobill.Text = (CommonFunctions.GetIntSafely(txtTotalUnitsOrdered.Text) * CommonFunctions.GetIntSafely(txtDoseCount.Text)).ToString();

            txtTotalCOGOrder.Text = Math.Round(CommonFunctions.GetIntSafely(txtTotalUnitstobill.Text) * CommonFunctions.GetFloatSafely(txtCOGPerUnit.Text), 2).ToString();
            txtTotalBillCharge.Text = Math.Round(CommonFunctions.GetIntSafely(txtTotalUnitstobill.Text) * CommonFunctions.GetFloatSafely(txtBilledPerUnit.Text), 2).ToString();
        }

        private void txtTotalPrescribedUnits_TextChanged(object sender, EventArgs e)
        {
            this.CalculateAll();
        }

        private void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            if(this.NewOrder)
            {
                MessageBox.Show("This is a new order !!!");
                return;
            }

            OrderTasks orderStatus = new OrderTasks(this.WorkingOrder.Id);
            if(orderStatus.ShowDialog() == DialogResult.OK)
            {
                this.LoadData();
            }
        }

        private void btnCreateNew_Click(object sender, EventArgs e)
        {
            if(this.NewOrder)
            {
                MessageBox.Show("This is a new order !!!");
                return;
            }

            Order order = OrdersDL.Get(this.WorkingOrder.Id, true);
            order.Id = Guid.NewGuid();
            order.DOS = DateTime.Now;
            order.OrderNumber = this.GenerateAndValidateOrderNumber(order.DOS, "Prophy", order.Id);
            order.ConfirmedDOS = DateTime.Now;
            order.ConfirmedDeliveryDate = DateTime.Now;
            order.EstimatedDeliveryDate = DateTime.Now;
            order.NextCallDate = DateTime.Now;
            order.OrderStatus = E_TaskStatus.InProgress;
            
            List<OrderAssay> assays = OrderAssayDL.Get(this.WorkingOrder.Id);
            assays.ForEach(x => { x.OrderId = order.Id; });

            int result = OrdersDL.Add(order, assays, StaticListDL.GetAllTasks());

            if (result > 0)
            {
                CommonFunctions.ShowInfomationMessage("Congratulations. New Order Created Successfully.");
                this.WorkingOrder = OrdersDL.Get(order.Id, true);
                this.LoadData();
            }
        }

        private void radPhropy_CheckedChanged(object sender, EventArgs e)
        {
            lblOrderNumber.Text = this.GenerateAndValidateOrderNumber(txtDateofService.Value, txtProphyPRN.Text, this.WorkingOrder.Id);
        }
            
        private void radPRN_CheckedChanged(object sender, EventArgs e)
        {
            lblOrderNumber.Text = this.GenerateAndValidateOrderNumber(txtDateofService.Value, txtProphyPRN.Text, this.WorkingOrder.Id);
        }

        private void txtDateofService_ValueChanged(object sender, EventArgs e)
        {
            lblOrderNumber.Text = this.GenerateAndValidateOrderNumber(txtDateofService.Value, txtProphyPRN.Text, this.WorkingOrder.Id);
        }

        private void txtDrugName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CalculateAll();
        }

        private void txtManufacturer_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }



        private void txtProphyPRN_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblOrderNumber.Text = this.GenerateAndValidateOrderNumber(txtDateofService.Value, txtProphyPRN.Text, this.WorkingOrder.Id);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (this.NewOrder)
            {
                CommonFunctions.ShowErrorMessage("Please save the new order for it to print");
                return;
            }

            string file = CommonFunctions.BuildOrderHtmlFile(this.WorkingOrder.Id);

            CommonFunctions.PrintReport(file, this);
        }

        private void txtDoseCount_TextChanged(object sender, EventArgs e)
        {
            this.CalculateAll();
        }

        private void txtMRN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(this.Patient != null)
            {
                PatientDetailsForm patientDetailsForm = new PatientDetailsForm(this.Patient.Id);
                if(patientDetailsForm.ShowDialog() == DialogResult.OK)
                {
                    this.Patient = PatientsDL.Get(this.Patient.Id);
                    this.UpdatePatient();
                }
            }
        }

        private void UpdatePatient()
        {
            txtMRN.Text = this.Patient.MRN + " => " + this.Patient.LastName + ", " + this.Patient.FirstName;
            BasicModel insurance = ListDL.Get(Constants.TableName_Insurance, this.Patient.InsuranceId);
            txtInsurancePayor.Text = insurance.Name;
            txtDeliveryAddress.Items.Clear();
            if (!string.IsNullOrEmpty(this.Patient.Address1))
                txtDeliveryAddress.Items.Add(this.Patient.Address1);
            if (!string.IsNullOrEmpty(this.Patient.Address2))
                txtDeliveryAddress.Items.Add(this.Patient.Address2);
            if (!string.IsNullOrEmpty(this.Patient.Address3))
                txtDeliveryAddress.Items.Add(this.Patient.Address3);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblOrderNumber_DoubleClick(object sender, EventArgs e)
        {
            string path = PathDL.GetOrderPath(this.WorkingOrder.Id);
            Process.Start("explorer.exe", path);
        }

        private void btnBuildPacket_Click(object sender, EventArgs e)
        {
            FinalPacketForm form = new FinalPacketForm(this.WorkingOrder.Id);
            form.ShowDialog();
        }
    }
}
