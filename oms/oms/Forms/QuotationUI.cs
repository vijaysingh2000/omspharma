namespace IPI.UI
{
    using DevExpress.Utils;
    using DevExpress.XtraEditors.Repository;
    using DevExpress.XtraGrid;
    using DevExpress.XtraGrid.Views.Base;
    using DevExpress.XtraGrid.Views.Grid;
    using IPI.Common;
    using IPI.DataAccessLayer;
    using IPI.Model;
    using IPI.Print;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Linq;

    public class QuotationUI : Form
    {
        private GridControl gridControl;
        private GridView gridView;
        private List<Customer> customerList;
        private List<Quotation> quotationList;
        private Customer workingCustomer;
        private QuotationDetail workingQuotation;
        private DataTable dataTable;
        private string selectedCustomerID;
        private string selectedQuotationID;
        private string selectedWorkorderID;
        private double total_ManHours;
        private double total_Extension;
        private IContainer components;
        private Panel panel1;
        private ComboBox cmbCustomers;
        private Label lblCustomer;
        private SplitContainer splitContainer1;
        private ListBox lstQuotations;
        private GroupBox groupProjectDetails;
        private GroupBox groupProjectLocation;
        private GroupBox groupBox1;
        private Label lblDate;
        private TextBox txtAuthor;
        private Label lblAuthor;
        private DateTimePicker txtDate;
        private TextBox txtRevision;
        private Label lblRevision;
        private TextBox txtProposal;
        private Label lblProposal;
        private TextBox txtAddress;
        private Label lblAddress;
        private TextBox txtProjectSite;
        private Label lblProjectSite;
        private TextBox txtPhone;
        private Label lblPhone;
        private MaskedTextBox txtZipCode;
        private Label lblZipCode;
        private TextBox txtCity;
        private Label lblCity;
        private Label lblLocationOfProduct;
        private Label lblProduct;
        private TextBox txtNumberOfSpecialCuts;
        private Label lblNumberOfSpecialCuts;
        private TextBox txtProduct;
        private Label lblProtection;
        private Label lblFloor;
        private Label lblDockElevStairs;
        private ComboBox cmbOverTime;
        private Label lblOverTime;
        private ComboBox cmbRegularTime;
        private Label lblRegularTime;
        private ComboBox cmbTrash;
        private Label lblTrash;
        private ComboBox cmbMoveExisting;
        private Label lblMoveExisting;
        private ComboBox cmbAreaOfInstall;
        private Label lblAreaOfInstall;
        private SplitContainer splitContainer2;
        private Panel panBottom;
        private Panel panTop;
        private GroupBox groupScopeOfWork;
        private TextBox txtScopeOfWork;
        private Button btnClose;
        private Button btnSave;
        private Button btnDelete;
        private Button btnCreate;
        private Button btnNew;
        private Button btnPrint;
        private Button btnCreateWorkOrder;
        private Button btnCustomer;
        private Label lblTotal;
        private Button btnQPS;
        private TextBox txtProjectContact;
        private Label Contact;
        private TextBox txtNumofdaystocomplete;
        private Label label1;
        private TextBox txtFloor;
        private Button btnBrowse;
        private ComboBox cmbParkingValidated;
        private Label lblParkingValidated;
        private TextBox txtDocElevStairs;
        private ComboBox cmbProtection;
        private ComboBox cmbTerm;
        private Label lblTerm;
        private ComboBox txtLocationOfProduct;
        private Button btnProjectCloseoutReport;
        private Button btnInstallationSchedule;
        private Button btnOutofScope;

        public QuotationUI()
        {
            this.components = null;
            this.InitializeComponent();
        }

        public QuotationUI(string id, string workOrderId)
        {
            this.components = null;
            this.InitializeComponent();
            Quotation basic = QuotationDAL.GetBasic(id);
            if (basic != null)
            {
                this.selectedCustomerID = basic.CustomerID;
                this.selectedQuotationID = id;
            }
            this.selectedWorkorderID = workOrderId;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {
                InitialDirectory = ApplicationPaths.GetTemplatePath(),
                Title = CommonData.LoggedInCompany.Name
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = dialog.FileName;
                if (File.Exists(fileName))
                {
                    this.txtScopeOfWork.Text = File.ReadAllText(fileName);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ReferenceEquals(this.workingQuotation, null))
                {
                    QuotationDetail objA = QuotationDAL.Create(this.workingQuotation);
                    if (!ReferenceEquals(objA, null))
                    {
                        this.selectedQuotationID = objA.ID;
                        this.LoadQuotations();
                    }
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void btnCreateWorkOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReferenceEquals(this.workingQuotation, null))
                {
                    MessageBox.Show("Select a quotation to generate work order.");
                }
                else if (MessageBox.Show("Quotation will be saved in order to access work order. " + Environment.NewLine + "Do you like to save the current working quotation ?", CommonFunctions.GetMessageCaption(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
                {
                    this.selectedQuotationID = this.workingQuotation.ID;
                    this.Save();
                    new WorkOrderUI(this.workingQuotation, string.Empty, this.selectedCustomerID).ShowDialog();
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            new CustomerUI().ShowDialog();
            this.LoadCustomerList();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ReferenceEquals(this.workingQuotation, null) && (MessageBox.Show("Are you sure you like to delete this quotation ?", CommonFunctions.GetMessageCaption(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.No))
                {
                    QuotationDAL.Delete(this.workingQuotation.ID);
                    MessageBox.Show("Quotation Deleted.", CommonFunctions.GetMessageCaption(), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.selectedQuotationID = string.Empty;
                    this.LoadQuotations();
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void btnInstallationSchedule_Click(object sender, EventArgs e)
        {
            if (ReferenceEquals(this.workingQuotation, null))
            {
                MessageBox.Show("Select a quotation to to access Installation Schedule.");
            }
            else if (MessageBox.Show("Quotation will be saved in order to access QPS. " + Environment.NewLine + "Do you like to save the current working quotation ?", CommonFunctions.GetMessageCaption(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
            {
                this.selectedQuotationID = this.workingQuotation.ID;
                this.Save();
                new IPI.UI.InstallationSchedule(this.selectedQuotationID).ShowDialog();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ReferenceEquals(this.workingCustomer, null))
                {
                    this.workingQuotation = QuotationDAL.Insert(this.workingCustomer.ID);
                    this.selectedQuotationID = string.Empty;
                    this.LoadQuotations();
                    this.PopulateControl();
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void btnOutofScope_Click(object sender, EventArgs e)
        {
            if (ReferenceEquals(this.workingQuotation, null))
            {
                MessageBox.Show("Select a quotation to to access out of Scope.");
            }
            else if (MessageBox.Show("Quotation will be saved in order to access QPS. " + Environment.NewLine + "Do you like to save the current working quotation ?", CommonFunctions.GetMessageCaption(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
            {
                this.selectedQuotationID = this.workingQuotation.ID;
                this.Save();
                new OutofScope(this.selectedQuotationID).ShowDialog();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReferenceEquals(this.workingQuotation, null))
                {
                    MessageBox.Show("Select a quotation to print.");
                }
                else if (MessageBox.Show("Quotation will be saved in order to print. " + Environment.NewLine + "Do you like to save the current working quotation ?", CommonFunctions.GetMessageCaption(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
                {
                    this.Save();
                    CommonFunctions.PrintReport(PrintModule.PrintQuotation(this.selectedQuotationID), this);
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void btnProjectCloseoutReport_Click(object sender, EventArgs e)
        {
            if (ReferenceEquals(this.workingQuotation, null))
            {
                MessageBox.Show("Select a quotation to to access Project Close out Report.");
            }
            else if (MessageBox.Show("Quotation will be saved in order to access QPS. " + Environment.NewLine + "Do you like to save the current working quotation ?", CommonFunctions.GetMessageCaption(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
            {
                this.selectedQuotationID = this.workingQuotation.ID;
                this.Save();
                new ProjectCloseoutReport(this.selectedQuotationID, this.total_ManHours, this.total_Extension).ShowDialog();
            }
        }

        private void btnQPS_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReferenceEquals(this.workingQuotation, null))
                {
                    MessageBox.Show("Select a quotation to generate QPS.");
                }
                else if (MessageBox.Show("Quotation will be saved in order to access QPS. " + Environment.NewLine + "Do you like to save the current working quotation ?", CommonFunctions.GetMessageCaption(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
                {
                    this.selectedQuotationID = this.workingQuotation.ID;
                    this.Save();
                    new QPSUI(this.workingQuotation.ID).ShowDialog();
                    this.lstQuotations_SelectedIndexChanged(this.lstQuotations, null);
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.Save();
                if (!ReferenceEquals(this.workingQuotation, null))
                {
                    MessageBox.Show("Quotation Saved", CommonFunctions.GetMessageCaption(), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void Calculate()
        {
            DataRow focusedDataRow = this.gridView.GetFocusedDataRow();
            if (!ReferenceEquals(focusedDataRow, null))
            {
                focusedDataRow["Extension"] = Math.Round((double) (CommonFunctions.GetFloatSafely(focusedDataRow["Unit Price"].ToString(), 0.0, 2) * CommonFunctions.GetFloatSafely(focusedDataRow["Qty"].ToString(), 0.0, 2)), 2);
                this.gridView.UpdateCurrentRow();
                this.UpdateTotal();
            }
        }

        private void cmbCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cmbCustomers.SelectedItem != null)
                {
                    this.workingCustomer = (Customer) this.cmbCustomers.SelectedItem;
                    this.selectedCustomerID = this.workingCustomer.ID;
                    this.LoadQuotations();
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void DeleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow focusedDataRow = this.gridView.GetFocusedDataRow();
                if (!ReferenceEquals(focusedDataRow, null))
                {
                    focusedDataRow.Delete();
                    this.ReIndex();
                    this.gridView.PostEditor();
                    this.UpdateTotal();
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private string GetSelectedCustomerId() => 
            !(this.cmbCustomers.SelectedItem is Customer) ? string.Empty : (this.cmbCustomers.SelectedItem as Customer).ID;

        private void GridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName.IsStringEqual("Qty") || e.Column.FieldName.IsStringEqual("Unit Price"))
                {
                    this.Calculate();
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void GridView_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            try
            {
                DataRow dataRow = this.gridView.GetDataRow(e.RowHandle);
                if (dataRow != null)
                {
                    dataRow["Delete"] = "Delete";
                    dataRow["Line"] = dataRow.Table.Rows.Count + 1;
                    dataRow["Insert"] = "Insert";
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCustomer = new System.Windows.Forms.Button();
            this.btnProjectCloseoutReport = new System.Windows.Forms.Button();
            this.cmbCustomers = new System.Windows.Forms.ComboBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.btnInstallationSchedule = new System.Windows.Forms.Button();
            this.btnCreateWorkOrder = new System.Windows.Forms.Button();
            this.btnQPS = new System.Windows.Forms.Button();
            this.btnOutofScope = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstQuotations = new System.Windows.Forms.ListBox();
            this.panBottom = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.panTop = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtRevision = new System.Windows.Forms.TextBox();
            this.lblRevision = new System.Windows.Forms.Label();
            this.txtProposal = new System.Windows.Forms.TextBox();
            this.lblProposal = new System.Windows.Forms.Label();
            this.txtDate = new System.Windows.Forms.DateTimePicker();
            this.lblDate = new System.Windows.Forms.Label();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupScopeOfWork = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtScopeOfWork = new System.Windows.Forms.TextBox();
            this.groupProjectDetails = new System.Windows.Forms.GroupBox();
            this.txtLocationOfProduct = new System.Windows.Forms.ComboBox();
            this.cmbTerm = new System.Windows.Forms.ComboBox();
            this.lblTerm = new System.Windows.Forms.Label();
            this.cmbParkingValidated = new System.Windows.Forms.ComboBox();
            this.lblParkingValidated = new System.Windows.Forms.Label();
            this.cmbTrash = new System.Windows.Forms.ComboBox();
            this.lblTrash = new System.Windows.Forms.Label();
            this.cmbMoveExisting = new System.Windows.Forms.ComboBox();
            this.lblMoveExisting = new System.Windows.Forms.Label();
            this.cmbAreaOfInstall = new System.Windows.Forms.ComboBox();
            this.lblAreaOfInstall = new System.Windows.Forms.Label();
            this.lblProtection = new System.Windows.Forms.Label();
            this.lblFloor = new System.Windows.Forms.Label();
            this.lblDockElevStairs = new System.Windows.Forms.Label();
            this.cmbProtection = new System.Windows.Forms.ComboBox();
            this.cmbOverTime = new System.Windows.Forms.ComboBox();
            this.lblOverTime = new System.Windows.Forms.Label();
            this.cmbRegularTime = new System.Windows.Forms.ComboBox();
            this.lblRegularTime = new System.Windows.Forms.Label();
            this.txtNumberOfSpecialCuts = new System.Windows.Forms.TextBox();
            this.lblNumberOfSpecialCuts = new System.Windows.Forms.Label();
            this.txtDocElevStairs = new System.Windows.Forms.TextBox();
            this.txtFloor = new System.Windows.Forms.TextBox();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.lblLocationOfProduct = new System.Windows.Forms.Label();
            this.lblProduct = new System.Windows.Forms.Label();
            this.groupProjectLocation = new System.Windows.Forms.GroupBox();
            this.txtNumofdaystocomplete = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtProjectContact = new System.Windows.Forms.TextBox();
            this.Contact = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtZipCode = new System.Windows.Forms.MaskedTextBox();
            this.lblZipCode = new System.Windows.Forms.Label();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.lblCity = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtProjectSite = new System.Windows.Forms.TextBox();
            this.lblProjectSite = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panBottom.SuspendLayout();
            this.panTop.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupScopeOfWork.SuspendLayout();
            this.groupProjectDetails.SuspendLayout();
            this.groupProjectLocation.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnCustomer);
            this.panel1.Controls.Add(this.btnProjectCloseoutReport);
            this.panel1.Controls.Add(this.cmbCustomers);
            this.panel1.Controls.Add(this.lblCustomer);
            this.panel1.Controls.Add(this.btnInstallationSchedule);
            this.panel1.Controls.Add(this.btnCreateWorkOrder);
            this.panel1.Controls.Add(this.btnQPS);
            this.panel1.Controls.Add(this.btnOutofScope);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1155, 66);
            this.panel1.TabIndex = 0;
            // 
            // btnCustomer
            // 
            this.btnCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCustomer.Location = new System.Drawing.Point(561, 20);
            this.btnCustomer.Name = "btnCustomer";
            this.btnCustomer.Size = new System.Drawing.Size(26, 23);
            this.btnCustomer.TabIndex = 1;
            this.btnCustomer.Text = "...";
            this.btnCustomer.UseVisualStyleBackColor = true;
            this.btnCustomer.Click += new System.EventHandler(this.btnCustomer_Click);
            // 
            // btnProjectCloseoutReport
            // 
            this.btnProjectCloseoutReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProjectCloseoutReport.Location = new System.Drawing.Point(1070, 11);
            this.btnProjectCloseoutReport.Name = "btnProjectCloseoutReport";
            this.btnProjectCloseoutReport.Size = new System.Drawing.Size(73, 46);
            this.btnProjectCloseoutReport.TabIndex = 33;
            this.btnProjectCloseoutReport.Text = "&Close Out Report";
            this.btnProjectCloseoutReport.UseVisualStyleBackColor = true;
            this.btnProjectCloseoutReport.Click += new System.EventHandler(this.btnProjectCloseoutReport_Click);
            // 
            // cmbCustomers
            // 
            this.cmbCustomers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCustomers.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmbCustomers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomers.FormattingEnabled = true;
            this.cmbCustomers.Location = new System.Drawing.Point(75, 20);
            this.cmbCustomers.Name = "cmbCustomers";
            this.cmbCustomers.Size = new System.Drawing.Size(480, 21);
            this.cmbCustomers.TabIndex = 0;
            this.cmbCustomers.SelectedIndexChanged += new System.EventHandler(this.cmbCustomers_SelectedIndexChanged);
            // 
            // lblCustomer
            // 
            this.lblCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(9, 23);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(51, 13);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "Customer";
            // 
            // btnInstallationSchedule
            // 
            this.btnInstallationSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInstallationSchedule.Location = new System.Drawing.Point(998, 11);
            this.btnInstallationSchedule.Name = "btnInstallationSchedule";
            this.btnInstallationSchedule.Size = new System.Drawing.Size(73, 46);
            this.btnInstallationSchedule.TabIndex = 33;
            this.btnInstallationSchedule.Text = "&Installation Schedule";
            this.btnInstallationSchedule.UseVisualStyleBackColor = true;
            this.btnInstallationSchedule.Click += new System.EventHandler(this.btnInstallationSchedule_Click);
            // 
            // btnCreateWorkOrder
            // 
            this.btnCreateWorkOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateWorkOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateWorkOrder.Location = new System.Drawing.Point(779, 11);
            this.btnCreateWorkOrder.Name = "btnCreateWorkOrder";
            this.btnCreateWorkOrder.Size = new System.Drawing.Size(73, 46);
            this.btnCreateWorkOrder.TabIndex = 32;
            this.btnCreateWorkOrder.Text = "&Work Order";
            this.btnCreateWorkOrder.UseVisualStyleBackColor = true;
            this.btnCreateWorkOrder.Click += new System.EventHandler(this.btnCreateWorkOrder_Click);
            // 
            // btnQPS
            // 
            this.btnQPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQPS.Location = new System.Drawing.Point(852, 11);
            this.btnQPS.Name = "btnQPS";
            this.btnQPS.Size = new System.Drawing.Size(73, 46);
            this.btnQPS.TabIndex = 33;
            this.btnQPS.Text = "&QPS";
            this.btnQPS.UseVisualStyleBackColor = true;
            this.btnQPS.Click += new System.EventHandler(this.btnQPS_Click);
            // 
            // btnOutofScope
            // 
            this.btnOutofScope.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutofScope.Location = new System.Drawing.Point(925, 11);
            this.btnOutofScope.Name = "btnOutofScope";
            this.btnOutofScope.Size = new System.Drawing.Size(73, 46);
            this.btnOutofScope.TabIndex = 33;
            this.btnOutofScope.Text = "Out of Scope";
            this.btnOutofScope.UseVisualStyleBackColor = true;
            this.btnOutofScope.Click += new System.EventHandler(this.btnOutofScope_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 66);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.splitContainer1.Panel1.Controls.Add(this.lstQuotations);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panBottom);
            this.splitContainer1.Panel2.Controls.Add(this.panTop);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Resize += new System.EventHandler(this.splitContainer1_Panel2_Resize);
            this.splitContainer1.Size = new System.Drawing.Size(1155, 621);
            this.splitContainer1.SplitterDistance = 222;
            this.splitContainer1.TabIndex = 1;
            // 
            // lstQuotations
            // 
            this.lstQuotations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstQuotations.FormattingEnabled = true;
            this.lstQuotations.Location = new System.Drawing.Point(0, 0);
            this.lstQuotations.Name = "lstQuotations";
            this.lstQuotations.Size = new System.Drawing.Size(220, 619);
            this.lstQuotations.TabIndex = 2;
            this.lstQuotations.SelectedIndexChanged += new System.EventHandler(this.lstQuotations_SelectedIndexChanged);
            // 
            // panBottom
            // 
            this.panBottom.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panBottom.Controls.Add(this.lblTotal);
            this.panBottom.Controls.Add(this.btnPrint);
            this.panBottom.Controls.Add(this.btnClose);
            this.panBottom.Controls.Add(this.btnSave);
            this.panBottom.Controls.Add(this.btnDelete);
            this.panBottom.Controls.Add(this.btnCreate);
            this.panBottom.Controls.Add(this.btnNew);
            this.panBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panBottom.Location = new System.Drawing.Point(0, 567);
            this.panBottom.Name = "panBottom";
            this.panBottom.Size = new System.Drawing.Size(927, 52);
            this.panBottom.TabIndex = 5;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.Red;
            this.lblTotal.Location = new System.Drawing.Point(442, 17);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(56, 18);
            this.lblTotal.TabIndex = 33;
            this.lblTotal.Text = "Total :";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(334, 16);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(77, 23);
            this.btnPrint.TabIndex = 31;
            this.btnPrint.Text = "&Preview";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(842, 16);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(77, 23);
            this.btnClose.TabIndex = 34;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(253, 16);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(77, 23);
            this.btnSave.TabIndex = 30;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(172, 16);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(77, 23);
            this.btnDelete.TabIndex = 29;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(91, 16);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(77, 23);
            this.btnCreate.TabIndex = 28;
            this.btnCreate.Text = "&Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(10, 16);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(77, 23);
            this.btnNew.TabIndex = 27;
            this.btnNew.Text = "&New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // panTop
            // 
            this.panTop.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panTop.Controls.Add(this.groupBox1);
            this.panTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panTop.Location = new System.Drawing.Point(0, 0);
            this.panTop.Name = "panTop";
            this.panTop.Size = new System.Drawing.Size(927, 62);
            this.panTop.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox1.Controls.Add(this.txtRevision);
            this.groupBox1.Controls.Add(this.lblRevision);
            this.groupBox1.Controls.Add(this.txtProposal);
            this.groupBox1.Controls.Add(this.lblProposal);
            this.groupBox1.Controls.Add(this.txtDate);
            this.groupBox1.Controls.Add(this.lblDate);
            this.groupBox1.Controls.Add(this.txtAuthor);
            this.groupBox1.Controls.Add(this.lblAuthor);
            this.groupBox1.Location = new System.Drawing.Point(3, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(899, 49);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtRevision
            // 
            this.txtRevision.BackColor = System.Drawing.SystemColors.Info;
            this.txtRevision.Location = new System.Drawing.Point(803, 17);
            this.txtRevision.Name = "txtRevision";
            this.txtRevision.ReadOnly = true;
            this.txtRevision.Size = new System.Drawing.Size(79, 20);
            this.txtRevision.TabIndex = 6;
            // 
            // lblRevision
            // 
            this.lblRevision.AutoSize = true;
            this.lblRevision.Location = new System.Drawing.Point(749, 17);
            this.lblRevision.Name = "lblRevision";
            this.lblRevision.Size = new System.Drawing.Size(48, 13);
            this.lblRevision.TabIndex = 6;
            this.lblRevision.Text = "Revision";
            // 
            // txtProposal
            // 
            this.txtProposal.BackColor = System.Drawing.SystemColors.Info;
            this.txtProposal.Location = new System.Drawing.Point(558, 17);
            this.txtProposal.Name = "txtProposal";
            this.txtProposal.ReadOnly = true;
            this.txtProposal.Size = new System.Drawing.Size(179, 20);
            this.txtProposal.TabIndex = 5;
            // 
            // lblProposal
            // 
            this.lblProposal.AutoSize = true;
            this.lblProposal.Location = new System.Drawing.Point(504, 17);
            this.lblProposal.Name = "lblProposal";
            this.lblProposal.Size = new System.Drawing.Size(48, 13);
            this.lblProposal.TabIndex = 4;
            this.lblProposal.Text = "Proposal";
            // 
            // txtDate
            // 
            this.txtDate.Location = new System.Drawing.Point(298, 17);
            this.txtDate.Name = "txtDate";
            this.txtDate.Size = new System.Drawing.Size(200, 20);
            this.txtDate.TabIndex = 4;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(262, 17);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(30, 13);
            this.lblDate.TabIndex = 2;
            this.lblDate.Text = "Date";
            // 
            // txtAuthor
            // 
            this.txtAuthor.BackColor = System.Drawing.SystemColors.Info;
            this.txtAuthor.Location = new System.Drawing.Point(51, 17);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.ReadOnly = true;
            this.txtAuthor.Size = new System.Drawing.Size(196, 20);
            this.txtAuthor.TabIndex = 3;
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(7, 17);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(38, 13);
            this.lblAuthor.TabIndex = 0;
            this.lblAuthor.Text = "Author";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Location = new System.Drawing.Point(1, 60);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupScopeOfWork);
            this.splitContainer2.Panel1.Controls.Add(this.groupProjectDetails);
            this.splitContainer2.Panel1.Controls.Add(this.groupProjectLocation);
            this.splitContainer2.Size = new System.Drawing.Size(915, 510);
            this.splitContainer2.SplitterDistance = 222;
            this.splitContainer2.TabIndex = 3;
            // 
            // groupScopeOfWork
            // 
            this.groupScopeOfWork.Controls.Add(this.btnBrowse);
            this.groupScopeOfWork.Controls.Add(this.txtScopeOfWork);
            this.groupScopeOfWork.Location = new System.Drawing.Point(8, 250);
            this.groupScopeOfWork.Name = "groupScopeOfWork";
            this.groupScopeOfWork.Size = new System.Drawing.Size(888, 137);
            this.groupScopeOfWork.TabIndex = 3;
            this.groupScopeOfWork.TabStop = false;
            this.groupScopeOfWork.Text = "Scope of Work";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(23, 39);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(72, 61);
            this.btnBrowse.TabIndex = 27;
            this.btnBrowse.Text = "Browse Template";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtScopeOfWork
            // 
            this.txtScopeOfWork.BackColor = System.Drawing.SystemColors.Info;
            this.txtScopeOfWork.Location = new System.Drawing.Point(119, 19);
            this.txtScopeOfWork.Multiline = true;
            this.txtScopeOfWork.Name = "txtScopeOfWork";
            this.txtScopeOfWork.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtScopeOfWork.Size = new System.Drawing.Size(763, 110);
            this.txtScopeOfWork.TabIndex = 26;
            // 
            // groupProjectDetails
            // 
            this.groupProjectDetails.Controls.Add(this.txtLocationOfProduct);
            this.groupProjectDetails.Controls.Add(this.cmbTerm);
            this.groupProjectDetails.Controls.Add(this.lblTerm);
            this.groupProjectDetails.Controls.Add(this.cmbParkingValidated);
            this.groupProjectDetails.Controls.Add(this.lblParkingValidated);
            this.groupProjectDetails.Controls.Add(this.cmbTrash);
            this.groupProjectDetails.Controls.Add(this.lblTrash);
            this.groupProjectDetails.Controls.Add(this.cmbMoveExisting);
            this.groupProjectDetails.Controls.Add(this.lblMoveExisting);
            this.groupProjectDetails.Controls.Add(this.cmbAreaOfInstall);
            this.groupProjectDetails.Controls.Add(this.lblAreaOfInstall);
            this.groupProjectDetails.Controls.Add(this.lblProtection);
            this.groupProjectDetails.Controls.Add(this.lblFloor);
            this.groupProjectDetails.Controls.Add(this.lblDockElevStairs);
            this.groupProjectDetails.Controls.Add(this.cmbProtection);
            this.groupProjectDetails.Controls.Add(this.cmbOverTime);
            this.groupProjectDetails.Controls.Add(this.lblOverTime);
            this.groupProjectDetails.Controls.Add(this.cmbRegularTime);
            this.groupProjectDetails.Controls.Add(this.lblRegularTime);
            this.groupProjectDetails.Controls.Add(this.txtNumberOfSpecialCuts);
            this.groupProjectDetails.Controls.Add(this.lblNumberOfSpecialCuts);
            this.groupProjectDetails.Controls.Add(this.txtDocElevStairs);
            this.groupProjectDetails.Controls.Add(this.txtFloor);
            this.groupProjectDetails.Controls.Add(this.txtProduct);
            this.groupProjectDetails.Controls.Add(this.lblLocationOfProduct);
            this.groupProjectDetails.Controls.Add(this.lblProduct);
            this.groupProjectDetails.Location = new System.Drawing.Point(10, 93);
            this.groupProjectDetails.Name = "groupProjectDetails";
            this.groupProjectDetails.Size = new System.Drawing.Size(888, 151);
            this.groupProjectDetails.TabIndex = 2;
            this.groupProjectDetails.TabStop = false;
            this.groupProjectDetails.Text = "Project Details";
            // 
            // txtLocationOfProduct
            // 
            this.txtLocationOfProduct.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtLocationOfProduct.FormattingEnabled = true;
            this.txtLocationOfProduct.Items.AddRange(new object[] {
            "IPI",
            "CRI",
            "On-site",
            "Will Call",
            "IPI/Site"});
            this.txtLocationOfProduct.Location = new System.Drawing.Point(119, 41);
            this.txtLocationOfProduct.Name = "txtLocationOfProduct";
            this.txtLocationOfProduct.Size = new System.Drawing.Size(159, 21);
            this.txtLocationOfProduct.TabIndex = 35;
            // 
            // cmbTerm
            // 
            this.cmbTerm.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmbTerm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTerm.FormattingEnabled = true;
            this.cmbTerm.Items.AddRange(new object[] {
            "NET 10",
            "NET 30",
            "50%/40%/10%"});
            this.cmbTerm.Location = new System.Drawing.Point(723, 100);
            this.cmbTerm.Name = "cmbTerm";
            this.cmbTerm.Size = new System.Drawing.Size(159, 21);
            this.cmbTerm.TabIndex = 33;
            // 
            // lblTerm
            // 
            this.lblTerm.AutoSize = true;
            this.lblTerm.Location = new System.Drawing.Point(683, 103);
            this.lblTerm.Name = "lblTerm";
            this.lblTerm.Size = new System.Drawing.Size(36, 13);
            this.lblTerm.TabIndex = 34;
            this.lblTerm.Text = "Terms";
            // 
            // cmbParkingValidated
            // 
            this.cmbParkingValidated.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmbParkingValidated.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParkingValidated.FormattingEnabled = true;
            this.cmbParkingValidated.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cmbParkingValidated.Location = new System.Drawing.Point(119, 100);
            this.cmbParkingValidated.Name = "cmbParkingValidated";
            this.cmbParkingValidated.Size = new System.Drawing.Size(163, 21);
            this.cmbParkingValidated.TabIndex = 32;
            // 
            // lblParkingValidated
            // 
            this.lblParkingValidated.AutoSize = true;
            this.lblParkingValidated.Location = new System.Drawing.Point(28, 98);
            this.lblParkingValidated.Name = "lblParkingValidated";
            this.lblParkingValidated.Size = new System.Drawing.Size(90, 13);
            this.lblParkingValidated.TabIndex = 31;
            this.lblParkingValidated.Text = "Parking Validated";
            // 
            // cmbTrash
            // 
            this.cmbTrash.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmbTrash.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTrash.FormattingEnabled = true;
            this.cmbTrash.Items.AddRange(new object[] {
            "IPI",
            "CRI",
            "On-site"});
            this.cmbTrash.Location = new System.Drawing.Point(723, 72);
            this.cmbTrash.Name = "cmbTrash";
            this.cmbTrash.Size = new System.Drawing.Size(159, 21);
            this.cmbTrash.TabIndex = 24;
            // 
            // lblTrash
            // 
            this.lblTrash.AutoSize = true;
            this.lblTrash.Location = new System.Drawing.Point(686, 75);
            this.lblTrash.Name = "lblTrash";
            this.lblTrash.Size = new System.Drawing.Size(34, 13);
            this.lblTrash.TabIndex = 28;
            this.lblTrash.Text = "Trash";
            // 
            // cmbMoveExisting
            // 
            this.cmbMoveExisting.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmbMoveExisting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMoveExisting.FormattingEnabled = true;
            this.cmbMoveExisting.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cmbMoveExisting.Location = new System.Drawing.Point(723, 44);
            this.cmbMoveExisting.Name = "cmbMoveExisting";
            this.cmbMoveExisting.Size = new System.Drawing.Size(159, 21);
            this.cmbMoveExisting.TabIndex = 23;
            // 
            // lblMoveExisting
            // 
            this.lblMoveExisting.AutoSize = true;
            this.lblMoveExisting.Location = new System.Drawing.Point(649, 47);
            this.lblMoveExisting.Name = "lblMoveExisting";
            this.lblMoveExisting.Size = new System.Drawing.Size(73, 13);
            this.lblMoveExisting.TabIndex = 26;
            this.lblMoveExisting.Text = "Move Existing";
            // 
            // cmbAreaOfInstall
            // 
            this.cmbAreaOfInstall.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmbAreaOfInstall.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAreaOfInstall.FormattingEnabled = true;
            this.cmbAreaOfInstall.Items.AddRange(new object[] {
            "Clear",
            "Occupied"});
            this.cmbAreaOfInstall.Location = new System.Drawing.Point(723, 17);
            this.cmbAreaOfInstall.Name = "cmbAreaOfInstall";
            this.cmbAreaOfInstall.Size = new System.Drawing.Size(159, 21);
            this.cmbAreaOfInstall.TabIndex = 22;
            // 
            // lblAreaOfInstall
            // 
            this.lblAreaOfInstall.AutoSize = true;
            this.lblAreaOfInstall.Location = new System.Drawing.Point(649, 20);
            this.lblAreaOfInstall.Name = "lblAreaOfInstall";
            this.lblAreaOfInstall.Size = new System.Drawing.Size(71, 13);
            this.lblAreaOfInstall.TabIndex = 24;
            this.lblAreaOfInstall.Text = "Area of Install";
            // 
            // lblProtection
            // 
            this.lblProtection.AutoSize = true;
            this.lblProtection.Location = new System.Drawing.Point(364, 126);
            this.lblProtection.Name = "lblProtection";
            this.lblProtection.Size = new System.Drawing.Size(55, 13);
            this.lblProtection.TabIndex = 22;
            this.lblProtection.Text = "Protection";
            // 
            // lblFloor
            // 
            this.lblFloor.AutoSize = true;
            this.lblFloor.Location = new System.Drawing.Point(388, 98);
            this.lblFloor.Name = "lblFloor";
            this.lblFloor.Size = new System.Drawing.Size(30, 13);
            this.lblFloor.TabIndex = 20;
            this.lblFloor.Text = "Floor";
            // 
            // lblDockElevStairs
            // 
            this.lblDockElevStairs.AutoSize = true;
            this.lblDockElevStairs.Location = new System.Drawing.Point(334, 71);
            this.lblDockElevStairs.Name = "lblDockElevStairs";
            this.lblDockElevStairs.Size = new System.Drawing.Size(90, 13);
            this.lblDockElevStairs.TabIndex = 18;
            this.lblDockElevStairs.Text = "Dock/Elev/Stairs";
            // 
            // cmbProtection
            // 
            this.cmbProtection.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmbProtection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProtection.FormattingEnabled = true;
            this.cmbProtection.Items.AddRange(new object[] {
            "Standard",
            "Full PPE"});
            this.cmbProtection.Location = new System.Drawing.Point(425, 124);
            this.cmbProtection.Name = "cmbProtection";
            this.cmbProtection.Size = new System.Drawing.Size(163, 21);
            this.cmbProtection.TabIndex = 18;
            // 
            // cmbOverTime
            // 
            this.cmbOverTime.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmbOverTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOverTime.FormattingEnabled = true;
            this.cmbOverTime.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cmbOverTime.Location = new System.Drawing.Point(425, 39);
            this.cmbOverTime.Name = "cmbOverTime";
            this.cmbOverTime.Size = new System.Drawing.Size(163, 21);
            this.cmbOverTime.TabIndex = 18;
            // 
            // lblOverTime
            // 
            this.lblOverTime.AutoSize = true;
            this.lblOverTime.Location = new System.Drawing.Point(362, 42);
            this.lblOverTime.Name = "lblOverTime";
            this.lblOverTime.Size = new System.Drawing.Size(56, 13);
            this.lblOverTime.TabIndex = 16;
            this.lblOverTime.Text = "Over Time";
            // 
            // cmbRegularTime
            // 
            this.cmbRegularTime.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmbRegularTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRegularTime.FormattingEnabled = true;
            this.cmbRegularTime.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cmbRegularTime.Location = new System.Drawing.Point(425, 12);
            this.cmbRegularTime.Name = "cmbRegularTime";
            this.cmbRegularTime.Size = new System.Drawing.Size(163, 21);
            this.cmbRegularTime.TabIndex = 17;
            // 
            // lblRegularTime
            // 
            this.lblRegularTime.AutoSize = true;
            this.lblRegularTime.Location = new System.Drawing.Point(348, 15);
            this.lblRegularTime.Name = "lblRegularTime";
            this.lblRegularTime.Size = new System.Drawing.Size(70, 13);
            this.lblRegularTime.TabIndex = 14;
            this.lblRegularTime.Text = "Regular Time";
            // 
            // txtNumberOfSpecialCuts
            // 
            this.txtNumberOfSpecialCuts.BackColor = System.Drawing.SystemColors.Info;
            this.txtNumberOfSpecialCuts.Location = new System.Drawing.Point(119, 70);
            this.txtNumberOfSpecialCuts.Name = "txtNumberOfSpecialCuts";
            this.txtNumberOfSpecialCuts.Size = new System.Drawing.Size(161, 20);
            this.txtNumberOfSpecialCuts.TabIndex = 16;
            // 
            // lblNumberOfSpecialCuts
            // 
            this.lblNumberOfSpecialCuts.AutoSize = true;
            this.lblNumberOfSpecialCuts.Location = new System.Drawing.Point(28, 70);
            this.lblNumberOfSpecialCuts.Name = "lblNumberOfSpecialCuts";
            this.lblNumberOfSpecialCuts.Size = new System.Drawing.Size(85, 13);
            this.lblNumberOfSpecialCuts.TabIndex = 12;
            this.lblNumberOfSpecialCuts.Text = "# of special cuts";
            // 
            // txtDocElevStairs
            // 
            this.txtDocElevStairs.BackColor = System.Drawing.SystemColors.Info;
            this.txtDocElevStairs.Location = new System.Drawing.Point(425, 70);
            this.txtDocElevStairs.Name = "txtDocElevStairs";
            this.txtDocElevStairs.Size = new System.Drawing.Size(163, 20);
            this.txtDocElevStairs.TabIndex = 17;
            // 
            // txtFloor
            // 
            this.txtFloor.BackColor = System.Drawing.SystemColors.Info;
            this.txtFloor.Location = new System.Drawing.Point(425, 98);
            this.txtFloor.Name = "txtFloor";
            this.txtFloor.Size = new System.Drawing.Size(163, 20);
            this.txtFloor.TabIndex = 17;
            // 
            // txtProduct
            // 
            this.txtProduct.BackColor = System.Drawing.SystemColors.Info;
            this.txtProduct.Location = new System.Drawing.Point(119, 13);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(161, 20);
            this.txtProduct.TabIndex = 14;
            // 
            // lblLocationOfProduct
            // 
            this.lblLocationOfProduct.AutoSize = true;
            this.lblLocationOfProduct.Location = new System.Drawing.Point(13, 44);
            this.lblLocationOfProduct.Name = "lblLocationOfProduct";
            this.lblLocationOfProduct.Size = new System.Drawing.Size(100, 13);
            this.lblLocationOfProduct.TabIndex = 1;
            this.lblLocationOfProduct.Text = "Location of Product";
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(69, 20);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(44, 13);
            this.lblProduct.TabIndex = 0;
            this.lblProduct.Text = "Product";
            // 
            // groupProjectLocation
            // 
            this.groupProjectLocation.Controls.Add(this.txtNumofdaystocomplete);
            this.groupProjectLocation.Controls.Add(this.label1);
            this.groupProjectLocation.Controls.Add(this.txtProjectContact);
            this.groupProjectLocation.Controls.Add(this.Contact);
            this.groupProjectLocation.Controls.Add(this.txtPhone);
            this.groupProjectLocation.Controls.Add(this.lblPhone);
            this.groupProjectLocation.Controls.Add(this.txtZipCode);
            this.groupProjectLocation.Controls.Add(this.lblZipCode);
            this.groupProjectLocation.Controls.Add(this.txtCity);
            this.groupProjectLocation.Controls.Add(this.lblCity);
            this.groupProjectLocation.Controls.Add(this.txtAddress);
            this.groupProjectLocation.Controls.Add(this.lblAddress);
            this.groupProjectLocation.Controls.Add(this.txtProjectSite);
            this.groupProjectLocation.Controls.Add(this.lblProjectSite);
            this.groupProjectLocation.Location = new System.Drawing.Point(10, 8);
            this.groupProjectLocation.Name = "groupProjectLocation";
            this.groupProjectLocation.Size = new System.Drawing.Size(888, 75);
            this.groupProjectLocation.TabIndex = 1;
            this.groupProjectLocation.TabStop = false;
            this.groupProjectLocation.Text = "Project Location";
            // 
            // txtNumofdaystocomplete
            // 
            this.txtNumofdaystocomplete.BackColor = System.Drawing.SystemColors.Info;
            this.txtNumofdaystocomplete.Location = new System.Drawing.Point(751, 17);
            this.txtNumofdaystocomplete.Name = "txtNumofdaystocomplete";
            this.txtNumofdaystocomplete.Size = new System.Drawing.Size(126, 20);
            this.txtNumofdaystocomplete.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(638, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "#of days to complete";
            // 
            // txtProjectContact
            // 
            this.txtProjectContact.BackColor = System.Drawing.SystemColors.Info;
            this.txtProjectContact.Location = new System.Drawing.Point(321, 20);
            this.txtProjectContact.Name = "txtProjectContact";
            this.txtProjectContact.Size = new System.Drawing.Size(129, 20);
            this.txtProjectContact.TabIndex = 8;
            // 
            // Contact
            // 
            this.Contact.AutoSize = true;
            this.Contact.Location = new System.Drawing.Point(277, 23);
            this.Contact.Name = "Contact";
            this.Contact.Size = new System.Drawing.Size(44, 13);
            this.Contact.TabIndex = 17;
            this.Contact.Text = "Contact";
            // 
            // txtPhone
            // 
            this.txtPhone.BackColor = System.Drawing.SystemColors.Info;
            this.txtPhone.Location = new System.Drawing.Point(503, 17);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(126, 20);
            this.txtPhone.TabIndex = 9;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(462, 20);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(38, 13);
            this.lblPhone.TabIndex = 15;
            this.lblPhone.Text = "Phone";
            // 
            // txtZipCode
            // 
            this.txtZipCode.BackColor = System.Drawing.SystemColors.Info;
            this.txtZipCode.Location = new System.Drawing.Point(503, 46);
            this.txtZipCode.Name = "txtZipCode";
            this.txtZipCode.Size = new System.Drawing.Size(126, 20);
            this.txtZipCode.TabIndex = 11;
            // 
            // lblZipCode
            // 
            this.lblZipCode.AutoSize = true;
            this.lblZipCode.Location = new System.Drawing.Point(454, 49);
            this.lblZipCode.Name = "lblZipCode";
            this.lblZipCode.Size = new System.Drawing.Size(46, 13);
            this.lblZipCode.TabIndex = 12;
            this.lblZipCode.Text = "Zipcode";
            // 
            // txtCity
            // 
            this.txtCity.BackColor = System.Drawing.SystemColors.Info;
            this.txtCity.Location = new System.Drawing.Point(321, 46);
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(129, 20);
            this.txtCity.TabIndex = 10;
            // 
            // lblCity
            // 
            this.lblCity.AutoSize = true;
            this.lblCity.Location = new System.Drawing.Point(297, 49);
            this.lblCity.Name = "lblCity";
            this.lblCity.Size = new System.Drawing.Size(24, 13);
            this.lblCity.TabIndex = 10;
            this.lblCity.Text = "City";
            // 
            // txtAddress
            // 
            this.txtAddress.BackColor = System.Drawing.SystemColors.Info;
            this.txtAddress.Location = new System.Drawing.Point(83, 49);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(188, 20);
            this.txtAddress.TabIndex = 9;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(30, 49);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(45, 13);
            this.lblAddress.TabIndex = 8;
            this.lblAddress.Text = "Address";
            // 
            // txtProjectSite
            // 
            this.txtProjectSite.BackColor = System.Drawing.SystemColors.Info;
            this.txtProjectSite.Location = new System.Drawing.Point(83, 20);
            this.txtProjectSite.Name = "txtProjectSite";
            this.txtProjectSite.Size = new System.Drawing.Size(188, 20);
            this.txtProjectSite.TabIndex = 7;
            // 
            // lblProjectSite
            // 
            this.lblProjectSite.AutoSize = true;
            this.lblProjectSite.Location = new System.Drawing.Point(14, 20);
            this.lblProjectSite.Name = "lblProjectSite";
            this.lblProjectSite.Size = new System.Drawing.Size(61, 13);
            this.lblProjectSite.TabIndex = 6;
            this.lblProjectSite.Text = "Project Site";
            // 
            // QuotationUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1155, 687);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "QuotationUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quotation";
            this.Activated += new System.EventHandler(this.QuotationUI_Activated);
            this.Load += new System.EventHandler(this.Quotation_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panBottom.ResumeLayout(false);
            this.panBottom.PerformLayout();
            this.panTop.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupScopeOfWork.ResumeLayout(false);
            this.groupScopeOfWork.PerformLayout();
            this.groupProjectDetails.ResumeLayout(false);
            this.groupProjectDetails.PerformLayout();
            this.groupProjectLocation.ResumeLayout(false);
            this.groupProjectLocation.PerformLayout();
            this.ResumeLayout(false);

        }

        private void InsertItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow focusedDataRow = this.gridView.GetFocusedDataRow();
                if (!ReferenceEquals(focusedDataRow, null))
                {
                    DataTable dataSource = this.gridControl.DataSource as DataTable;
                    if (!ReferenceEquals(dataSource, null))
                    {
                        DataRow row2 = dataSource.NewRow();
                        row2["Delete"] = "Delete";
                        row2["Insert"] = "Insert";
                        dataSource.Rows.InsertAt(row2, dataSource.Rows.IndexOf(focusedDataRow));
                        int num2 = 1;
                        while (true)
                        {
                            if (num2 > dataSource.Rows.Count)
                            {
                                this.gridView.RefreshData();
                                this.gridControl.Refresh();
                                break;
                            }
                            dataSource.Rows[num2 - 1]["Line"] = num2.ToString();
                            num2++;
                        }
                    }
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void LoadCustomerList()
        {
            this.customerList = CustomerDAL.GetAll();
            string selectedCustomerID = string.Empty;
            if (!string.IsNullOrEmpty(this.selectedCustomerID))
            {
                selectedCustomerID = this.selectedCustomerID;
            }
            this.cmbCustomers.DataSource = this.customerList;
            this.cmbCustomers.DisplayMember = "Name";
            if (!string.IsNullOrEmpty(selectedCustomerID))
            {
                this.selectedCustomerID = selectedCustomerID;
            }
            if (!string.IsNullOrEmpty(this.selectedCustomerID))
            {
                this.cmbCustomers.SelectedIndex = this.customerList.FindIndex(pred => pred.ID.IsStringEqual(this.selectedCustomerID));
            }
            else if (this.cmbCustomers.Items.Count > 0)
            {
                this.cmbCustomers.SelectedIndex = 0;
            }
        }

        private void LoadQuotations()
        {
            try
            {
                this.quotationList = QuotationDAL.Get(this.workingCustomer.ID);
                string selectedQuotationID = string.Empty;
                if (!string.IsNullOrEmpty(this.selectedQuotationID))
                {
                    selectedQuotationID = this.selectedQuotationID;
                }
                this.lstQuotations.DataSource = this.quotationList;
                this.lstQuotations.DisplayMember = "DisplayText";
                if (this.lstQuotations.Items.Count <= 0)
                {
                    this.btnNew_Click(this.btnNew, null);
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedQuotationID))
                    {
                        this.selectedQuotationID = selectedQuotationID;
                    }
                    this.lstQuotations.SelectedIndex = !string.IsNullOrEmpty(this.selectedQuotationID) ? this.quotationList.FindIndex(pred => pred.ID.IsStringEqual(this.selectedQuotationID)) : (this.lstQuotations.Items.Count - 1);
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void lstQuotations_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.lstQuotations.SelectedItem != null)
                {
                    this.workingQuotation = QuotationDAL.GetDetails(((Quotation) this.lstQuotations.SelectedItem).ID, QuotationInfo.Quotation);
                    this.selectedQuotationID = this.workingQuotation.ID;
                    this.PopulateControl();
                }
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void Panel2_Resize(object sender, EventArgs e)
        {
            try
            {
                this.gridControl.Location = new Point(0, 0);
                this.gridControl.Size = new Size(this.splitContainer2.Panel2.Width - 10, this.splitContainer2.Panel2.Height - 10);
                this.ResizeGridColumns();
            }
            catch (Exception exception1)
            {
                ErrorMessage.Show(exception1);
            }
        }

        private void PopulateControl()
        {
            User user = UserDAL.Get(this.workingQuotation.UserID);
            this.txtAuthor.Text = user.FirstName + ", " + user.LastName;
            this.txtProposal.Text = this.workingQuotation.Proposal.ToString();
            this.txtRevision.Text = this.workingQuotation.Revision.ToString();
            this.txtDate.Value = CommonFunctions.GetDateTimeSafely(this.workingQuotation.Details.GetAttribute("date"));
            this.txtProjectSite.Text = this.workingQuotation.Details.GetAttribute("ps");
            this.txtProjectContact.Text = this.workingQuotation.Details.GetAttribute("psctct");
            this.txtAddress.Text = this.workingQuotation.Details.GetAttribute("psa");
            this.txtCity.Text = this.workingQuotation.Details.GetAttribute("psc");
            this.txtZipCode.Text = this.workingQuotation.Details.GetAttribute("psz");
            this.txtPhone.Text = this.workingQuotation.Details.GetAttribute("psp");
            this.txtNumofdaystocomplete.Text = this.workingQuotation.Details.GetAttribute("nodtc");
            this.txtProduct.Text = this.workingQuotation.Details.GetAttribute("pdn");
            this.txtLocationOfProduct.Text = this.workingQuotation.Details.GetAttribute("pdlop");
            this.txtNumberOfSpecialCuts.Text = this.workingQuotation.Details.GetAttribute("pdnosc");
            this.cmbParkingValidated.SelectedIndex = -1;
            this.cmbParkingValidated.Text = this.workingQuotation.Details.GetAttribute("pdpv");
            this.cmbRegularTime.SelectedIndex = -1;
            this.cmbRegularTime.Text = this.workingQuotation.Details.GetAttribute("pdrt");
            this.cmbOverTime.SelectedIndex = -1;
            this.cmbOverTime.Text = this.workingQuotation.Details.GetAttribute("pdot");
            this.txtDocElevStairs.Text = this.workingQuotation.Details.GetAttribute("pddes");
            this.txtFloor.Text = this.workingQuotation.Details.GetAttribute("pdf");
            this.cmbProtection.SelectedIndex = -1;
            this.cmbProtection.Text = this.workingQuotation.Details.GetAttribute("pdp");
            this.cmbAreaOfInstall.SelectedIndex = -1;
            this.cmbAreaOfInstall.Text = this.workingQuotation.Details.GetAttribute("pdaoi");
            this.cmbMoveExisting.SelectedIndex = -1;
            this.cmbMoveExisting.Text = this.workingQuotation.Details.GetAttribute("pdme");
            this.cmbTrash.SelectedIndex = -1;
            this.cmbTrash.Text = this.workingQuotation.Details.GetAttribute("pdtrash");
            this.cmbTerm.SelectedIndex = -1;
            this.cmbTerm.Text = this.workingQuotation.Details.GetAttribute("pdterm");
            this.txtScopeOfWork.Text = this.workingQuotation.Details.GetAttribute("sow");
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Line", typeof(float));
            this.dataTable.Columns.Add("Qty", typeof(float));
            this.dataTable.Columns.Add("Description", typeof(string));
            this.dataTable.Columns.Add("Unit Price", typeof(float));
            this.dataTable.Columns.Add("Extension", typeof(float));
            this.dataTable.Columns.Add("Notes", typeof(string));
            this.dataTable.Columns.Add("ManHours", typeof(float));
            this.dataTable.Columns.Add("Delete", typeof(string));
            this.dataTable.Columns.Add("Insert", typeof(string));
            XElement element = this.workingQuotation.Details.Element("quote_items");
            if (element != null)
            {
                int num2 = 1;
                foreach (XElement element2 in element.Elements("item"))
                {
                    DataRow row = this.dataTable.NewRow();
                    row["Line"] = num2;
                    row["Qty"] = CommonFunctions.GetFloatSafely(element2.GetAttribute("q"), 0.0, 2);
                    row["Description"] = element2.GetAttribute("d");
                    row["Unit Price"] = CommonFunctions.GetFloatSafely(element2.GetAttribute("p"), 0.0, 2);
                    row["Extension"] = CommonFunctions.GetFloatSafely(element2.GetAttribute("e"), 0.0, 2);
                    row["Notes"] = element2.GetAttribute("n");
                    row["ManHours"] = CommonFunctions.GetFloatSafely(element2.GetAttribute("m"), 0.0, 2);
                    row["Delete"] = "Delete";
                    row["Insert"] = "Insert";
                    this.dataTable.Rows.Add(row);
                    num2++;
                }
            }
            this.gridView.OptionsBehavior.AllowAddRows = DefaultBoolean.True;
            this.gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            this.gridControl.DataSource = this.dataTable;
            this.gridView.CellValueChanged += new CellValueChangedEventHandler(this.GridView_CellValueChanged);
            this.gridView.InitNewRow += new InitNewRowEventHandler(this.GridView_InitNewRow);
            RepositoryItemTextEdit edit = new RepositoryItemTextEdit();
            this.gridView.Columns["Unit Price"].ColumnEdit = edit;
            edit.DisplayFormat.FormatType = FormatType.Numeric;
            edit.DisplayFormat.FormatString = "c2";
            RepositoryItemTextEdit edit2 = new RepositoryItemTextEdit();
            this.gridView.Columns["Extension"].ColumnEdit = edit2;
            edit2.DisplayFormat.FormatType = FormatType.Numeric;
            edit2.DisplayFormat.FormatString = "c2";
            RepositoryItemHyperLinkEdit edit3 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Delete"].ColumnEdit = edit3;
            RepositoryItemHyperLinkEdit edit4 = new RepositoryItemHyperLinkEdit();
            this.gridView.Columns["Insert"].ColumnEdit = edit4;
            this.gridView.Columns["Extension"].OptionsColumn.AllowEdit = false;
            this.gridView.Columns["Line"].OptionsColumn.AllowEdit = false;
            this.gridView.Columns["ManHours"].Visible = false;
            this.ResizeGridColumns();
            edit3.Click += new EventHandler(this.DeleteItem_Click);
            edit4.Click += new EventHandler(this.InsertItem_Click);
            this.UpdateTotal();
        }

        private void PopulateObject()
        {
            this.workingQuotation.Proposal = CommonFunctions.GetIntSafely(this.txtProposal.Text, -1);
            this.workingQuotation.Revision = CommonFunctions.GetIntSafely(this.txtRevision.Text, -1);
            this.workingQuotation.Date = CommonFunctions.GetDateTimeSafely(this.txtDate.Text);
            this.workingQuotation.Details.SetAttributeValue("date", this.txtDate.Text);
            this.workingQuotation.Details.SetAttributeValue("ps", this.txtProjectSite.Text);
            this.workingQuotation.Details.SetAttributeValue("psctct", this.txtProjectContact.Text);
            this.workingQuotation.Details.SetAttributeValue("psa", this.txtAddress.Text);
            this.workingQuotation.Details.SetAttributeValue("psc", this.txtCity.Text);
            this.workingQuotation.Details.SetAttributeValue("psz", this.txtZipCode.Text);
            this.workingQuotation.Details.SetAttributeValue("psp", this.txtPhone.Text);
            this.workingQuotation.Details.SetAttributeValue("nodtc", this.txtNumofdaystocomplete.Text);
            this.workingQuotation.Details.SetAttributeValue("pdn", this.txtProduct.Text);
            this.workingQuotation.Details.SetAttributeValue("pdlop", this.txtLocationOfProduct.Text);
            this.workingQuotation.Details.SetAttributeValue("pdnosc", this.txtNumberOfSpecialCuts.Text);
            this.workingQuotation.Details.SetAttributeValue("pdpv", this.cmbParkingValidated.Text);
            this.workingQuotation.Details.SetAttributeValue("pdrt", this.cmbRegularTime.Text);
            this.workingQuotation.Details.SetAttributeValue("pdot", this.cmbOverTime.Text);
            this.workingQuotation.Details.SetAttributeValue("pddes", this.txtDocElevStairs.Text);
            this.workingQuotation.Details.SetAttributeValue("pdf", this.txtFloor.Text);
            this.workingQuotation.Details.SetAttributeValue("pdp", this.cmbProtection.Text);
            this.workingQuotation.Details.SetAttributeValue("pdaoi", this.cmbAreaOfInstall.Text);
            this.workingQuotation.Details.SetAttributeValue("pdme", this.cmbMoveExisting.Text);
            this.workingQuotation.Details.SetAttributeValue("pdtrash", this.cmbTrash.Text);
            this.workingQuotation.Details.SetAttributeValue("pdterm", this.cmbTerm.Text);
            this.workingQuotation.Details.SetAttributeValue("sow", this.txtScopeOfWork.Text);
            XElement element = this.workingQuotation.Details.Element("quote_items");
            if (element != null)
            {
                element.Remove();
            }
            XElement content = new XElement("quote_items");
            foreach (DataRow row in ((DataTable) this.gridControl.DataSource).Rows)
            {
                XElement element3 = new XElement("item");
                element3.SetAttributeValue("q", row["Qty"]);
                element3.SetAttributeValue("d", row["Description"]);
                element3.SetAttributeValue("p", row["Unit Price"]);
                element3.SetAttributeValue("e", row["Extension"]);
                element3.SetAttributeValue("n", row["Notes"]);
                element3.SetAttributeValue("m", row["ManHours"]);
                content.Add(element3);
            }
            this.workingQuotation.Details.Add(content);
        }

        private void Quotation_Load(object sender, EventArgs e)
        {
            this.gridControl = new GridControl();
            this.gridView = new GridView();
            this.gridControl.MainView = this.gridView;
            BaseView[] views = new BaseView[] { this.gridView };
            this.gridControl.ViewCollection.AddRange(views);
            this.gridView.GridControl = this.gridControl;
            this.splitContainer2.Panel2.Controls.Add(this.gridControl);
            this.splitContainer2.Panel2.Resize += new EventHandler(this.Panel2_Resize);
            this.gridControl.Location = new Point(0, 0);
            this.gridControl.Size = new Size(this.splitContainer2.Panel2.Width - 10, this.splitContainer2.Panel2.Height - 10);
            this.gridControl.Visible = true;
            this.gridView.OptionsView.ColumnAutoWidth = false;
            this.LoadCustomerList();
            this.SetIndex();
            this.Text = CommonFunctions.GetCaption("Quotation");
        }

        private void QuotationUI_Activated(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.selectedWorkorderID))
            {
                new WorkOrderUI(this.workingQuotation, this.selectedWorkorderID, this.selectedCustomerID).ShowDialog();
                this.selectedWorkorderID = string.Empty;
            }
        }

        private void ReIndex()
        {
            int num = 1;
            foreach (DataRow row in this.dataTable.Rows)
            {
                row["Line"] = num.ToString();
                num++;
            }
            this.gridControl.RefreshDataSource();
        }

        private void ResizeGridColumns()
        {
            try
            {
                this.gridView.Columns["Line"].Width = (int) (this.gridControl.Width * 0.05);
                this.gridView.Columns["Qty"].Width = (int) (this.gridControl.Width * 0.05);
                this.gridView.Columns["Description"].Width = (int) (this.gridControl.Width * 0.2);
                this.gridView.Columns["Unit Price"].Width = (int) (this.gridControl.Width * 0.1);
                this.gridView.Columns["Extension"].Width = (int) (this.gridControl.Width * 0.1);
                this.gridView.Columns["Notes"].Width = (int) (this.gridControl.Width * 0.3);
                this.gridView.Columns["Delete"].Width = (int) (this.gridControl.Width * 0.05);
                this.gridView.Columns["Insert"].Width = (int) (this.gridControl.Width * 0.05);
            }
            catch
            {
            }
        }

        private void Save()
        {
            if (!ReferenceEquals(this.workingQuotation, null))
            {
                this.PopulateObject();
                QuotationDAL.Update(this.workingQuotation, QuotationInfo.Quotation);
            }
        }

        private void SetIndex()
        {
            this.cmbCustomers.TabIndex = 0;
            this.btnCustomer.TabIndex = 1;
            this.lstQuotations.TabIndex = 2;
            this.txtAuthor.TabIndex = 3;
            this.txtDate.TabIndex = 4;
            this.txtProposal.TabIndex = 5;
            this.txtRevision.TabIndex = 6;
            this.txtProjectSite.TabIndex = 7;
            this.txtProjectContact.TabIndex = 8;
            this.txtPhone.TabIndex = 9;
            this.txtNumofdaystocomplete.TabIndex = 10;
            this.txtAddress.TabIndex = 11;
            this.txtCity.TabIndex = 12;
            this.txtZipCode.TabIndex = 13;
            this.txtProduct.TabIndex = 14;
            this.txtLocationOfProduct.TabIndex = 15;
            this.txtNumberOfSpecialCuts.TabIndex = 0x10;
            this.cmbParkingValidated.TabIndex = 0x11;
            this.cmbRegularTime.TabIndex = 0x12;
            this.cmbOverTime.TabIndex = 0x13;
            this.txtDocElevStairs.TabIndex = 20;
            this.txtFloor.TabIndex = 0x15;
            this.cmbProtection.TabIndex = 0x16;
            this.cmbAreaOfInstall.TabIndex = 0x17;
            this.cmbMoveExisting.TabIndex = 0x18;
            this.cmbTrash.TabIndex = 0x19;
            this.cmbTerm.TabIndex = 0x1a;
            this.btnBrowse.TabIndex = 0x1b;
            this.txtScopeOfWork.TabIndex = 0x1c;
            this.gridControl.TabIndex = 0x1d;
            this.btnNew.TabIndex = 30;
            this.btnCreate.TabIndex = 0x1f;
            this.btnDelete.TabIndex = 0x20;
            this.btnSave.TabIndex = 0x21;
            this.btnPrint.TabIndex = 0x22;
            this.btnCreateWorkOrder.TabIndex = 0x23;
            this.btnQPS.TabIndex = 0x24;
            this.btnClose.TabIndex = 0x25;
        }

        private void splitContainer1_Panel2_Resize(object sender, EventArgs e)
        {
            this.splitContainer2.Location = new Point(0, this.panTop.Bottom);
            this.splitContainer2.Size = new Size(this.splitContainer1.Panel2.Width, (this.splitContainer1.Panel2.Height - this.panTop.Height) - this.panTop.Height);
        }

        private void UpdateTotal()
        {
            if (((DataTable) this.gridControl.DataSource) != null)
            {
                this.total_Extension = CommonFunctions.GetFloatSafely(((DataTable) this.gridControl.DataSource).Compute("Sum(Extension)", string.Empty).ToString(), 0.0, 2);
                this.total_ManHours = CommonFunctions.GetFloatSafely(((DataTable) this.gridControl.DataSource).Compute("Sum(ManHours)", string.Empty).ToString(), 0.0, 2);
                this.lblTotal.Text = "Total : " + CommonFunctions.FormatCurrency(this.total_Extension, 2);
            }
        }
    }
}

