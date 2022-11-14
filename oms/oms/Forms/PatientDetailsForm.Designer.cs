namespace oms.Forms
{
    partial class PatientDetailsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtMRN = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblInsurance = new System.Windows.Forms.Label();
            this.lblDob = new System.Windows.Forms.Label();
            this.lblMRN = new System.Windows.Forms.Label();
            this.txtAddress1 = new System.Windows.Forms.TextBox();
            this.txtDOB = new System.Windows.Forms.DateTimePicker();
            this.txtPhone1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtInsurance = new System.Windows.Forms.ComboBox();
            this.txtAddress2 = new System.Windows.Forms.TextBox();
            this.txtAddress3 = new System.Windows.Forms.TextBox();
            this.radAddress1 = new System.Windows.Forms.RadioButton();
            this.radAddress2 = new System.Windows.Forms.RadioButton();
            this.radAddress3 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPhone2 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtGuardian = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.Location = new System.Drawing.Point(769, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 35);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "&Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSave.Location = new System.Drawing.Point(12, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(150, 35);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtEmail.Location = new System.Drawing.Point(651, 20);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(237, 23);
            this.txtEmail.TabIndex = 6;
            // 
            // txtLastName
            // 
            this.txtLastName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtLastName.Location = new System.Drawing.Point(151, 93);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(237, 23);
            this.txtLastName.TabIndex = 2;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtFirstName.Location = new System.Drawing.Point(151, 56);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(237, 23);
            this.txtFirstName.TabIndex = 1;
            // 
            // txtMRN
            // 
            this.txtMRN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtMRN.Location = new System.Drawing.Point(151, 19);
            this.txtMRN.Name = "txtMRN";
            this.txtMRN.Size = new System.Drawing.Size(237, 23);
            this.txtMRN.TabIndex = 0;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(564, 24);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(34, 14);
            this.lblEmail.TabIndex = 9;
            this.lblEmail.Text = "Email";
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point(27, 96);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(63, 14);
            this.lblLastName.TabIndex = 10;
            this.lblLastName.Text = "Last name";
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point(27, 59);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(63, 14);
            this.lblFirstName.TabIndex = 11;
            this.lblFirstName.Text = "First name";
            // 
            // lblInsurance
            // 
            this.lblInsurance.AutoSize = true;
            this.lblInsurance.Location = new System.Drawing.Point(538, 171);
            this.lblInsurance.Name = "lblInsurance";
            this.lblInsurance.Size = new System.Drawing.Size(60, 14);
            this.lblInsurance.TabIndex = 12;
            this.lblInsurance.Text = "Insurance";
            // 
            // lblDob
            // 
            this.lblDob.AutoSize = true;
            this.lblDob.Location = new System.Drawing.Point(13, 133);
            this.lblDob.Name = "lblDob";
            this.lblDob.Size = new System.Drawing.Size(77, 14);
            this.lblDob.TabIndex = 13;
            this.lblDob.Text = "Date of Birth";
            // 
            // lblMRN
            // 
            this.lblMRN.AutoSize = true;
            this.lblMRN.Location = new System.Drawing.Point(59, 22);
            this.lblMRN.Name = "lblMRN";
            this.lblMRN.Size = new System.Drawing.Size(31, 14);
            this.lblMRN.TabIndex = 14;
            this.lblMRN.Text = "MRN";
            // 
            // txtAddress1
            // 
            this.txtAddress1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtAddress1.Location = new System.Drawing.Point(651, 56);
            this.txtAddress1.Name = "txtAddress1";
            this.txtAddress1.Size = new System.Drawing.Size(237, 23);
            this.txtAddress1.TabIndex = 7;
            // 
            // txtDOB
            // 
            this.txtDOB.Location = new System.Drawing.Point(151, 131);
            this.txtDOB.Name = "txtDOB";
            this.txtDOB.Size = new System.Drawing.Size(237, 22);
            this.txtDOB.TabIndex = 3;
            // 
            // txtPhone1
            // 
            this.txtPhone1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPhone1.Location = new System.Drawing.Point(151, 169);
            this.txtPhone1.Name = "txtPhone1";
            this.txtPhone1.Size = new System.Drawing.Size(237, 23);
            this.txtPhone1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 172);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 14);
            this.label1.TabIndex = 30;
            this.label1.Text = "Phone 1";
            // 
            // txtInsurance
            // 
            this.txtInsurance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txtInsurance.FormattingEnabled = true;
            this.txtInsurance.Location = new System.Drawing.Point(651, 168);
            this.txtInsurance.Name = "txtInsurance";
            this.txtInsurance.Size = new System.Drawing.Size(237, 22);
            this.txtInsurance.TabIndex = 10;
            // 
            // txtAddress2
            // 
            this.txtAddress2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtAddress2.Location = new System.Drawing.Point(651, 93);
            this.txtAddress2.Name = "txtAddress2";
            this.txtAddress2.Size = new System.Drawing.Size(237, 23);
            this.txtAddress2.TabIndex = 8;
            // 
            // txtAddress3
            // 
            this.txtAddress3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtAddress3.Location = new System.Drawing.Point(651, 130);
            this.txtAddress3.Name = "txtAddress3";
            this.txtAddress3.Size = new System.Drawing.Size(237, 23);
            this.txtAddress3.TabIndex = 9;
            // 
            // radAddress1
            // 
            this.radAddress1.AutoSize = true;
            this.radAddress1.Location = new System.Drawing.Point(519, 59);
            this.radAddress1.Name = "radAddress1";
            this.radAddress1.Size = new System.Drawing.Size(79, 18);
            this.radAddress1.TabIndex = 35;
            this.radAddress1.TabStop = true;
            this.radAddress1.Text = "Address 1";
            this.radAddress1.UseVisualStyleBackColor = true;
            // 
            // radAddress2
            // 
            this.radAddress2.AutoSize = true;
            this.radAddress2.Location = new System.Drawing.Point(519, 92);
            this.radAddress2.Name = "radAddress2";
            this.radAddress2.Size = new System.Drawing.Size(79, 18);
            this.radAddress2.TabIndex = 35;
            this.radAddress2.TabStop = true;
            this.radAddress2.Text = "Address 2";
            this.radAddress2.UseVisualStyleBackColor = true;
            // 
            // radAddress3
            // 
            this.radAddress3.AutoSize = true;
            this.radAddress3.Location = new System.Drawing.Point(519, 128);
            this.radAddress3.Name = "radAddress3";
            this.radAddress3.Size = new System.Drawing.Size(79, 18);
            this.radAddress3.TabIndex = 35;
            this.radAddress3.TabStop = true;
            this.radAddress3.Text = "Address 3";
            this.radAddress3.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 14);
            this.label2.TabIndex = 30;
            this.label2.Text = "Phone 2";
            // 
            // txtPhone2
            // 
            this.txtPhone2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPhone2.Location = new System.Drawing.Point(151, 210);
            this.txtPhone2.Name = "txtPhone2";
            this.txtPhone2.Size = new System.Drawing.Size(237, 23);
            this.txtPhone2.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 506);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(931, 57);
            this.panel1.TabIndex = 36;
            // 
            // txtGuardian
            // 
            this.txtGuardian.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtGuardian.Location = new System.Drawing.Point(651, 204);
            this.txtGuardian.Name = "txtGuardian";
            this.txtGuardian.Size = new System.Drawing.Size(237, 23);
            this.txtGuardian.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(544, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 14);
            this.label3.TabIndex = 39;
            this.label3.Text = "Guardian";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtMRN);
            this.splitContainer1.Panel1.Controls.Add(this.txtGuardian);
            this.splitContainer1.Panel1.Controls.Add(this.lblMRN);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.lblDob);
            this.splitContainer1.Panel1.Controls.Add(this.lblInsurance);
            this.splitContainer1.Panel1.Controls.Add(this.radAddress3);
            this.splitContainer1.Panel1.Controls.Add(this.lblFirstName);
            this.splitContainer1.Panel1.Controls.Add(this.radAddress2);
            this.splitContainer1.Panel1.Controls.Add(this.lblLastName);
            this.splitContainer1.Panel1.Controls.Add(this.radAddress1);
            this.splitContainer1.Panel1.Controls.Add(this.lblEmail);
            this.splitContainer1.Panel1.Controls.Add(this.txtAddress3);
            this.splitContainer1.Panel1.Controls.Add(this.txtFirstName);
            this.splitContainer1.Panel1.Controls.Add(this.txtAddress2);
            this.splitContainer1.Panel1.Controls.Add(this.txtLastName);
            this.splitContainer1.Panel1.Controls.Add(this.txtInsurance);
            this.splitContainer1.Panel1.Controls.Add(this.txtEmail);
            this.splitContainer1.Panel1.Controls.Add(this.txtPhone2);
            this.splitContainer1.Panel1.Controls.Add(this.txtAddress1);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.txtDOB);
            this.splitContainer1.Panel1.Controls.Add(this.txtPhone1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.splitContainer1.Size = new System.Drawing.Size(931, 506);
            this.splitContainer1.SplitterDistance = 263;
            this.splitContainer1.TabIndex = 40;
            // 
            // PatientDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(931, 563);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MinimizeBox = false;
            this.Name = "PatientDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Patient";
            this.Load += new System.EventHandler(this.PatientDetailsForm_Load);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnCancel;
        private Button btnSave;
        private TextBox txtEmail;
        private TextBox txtLastName;
        private TextBox txtFirstName;
        private TextBox txtMRN;
        private Label lblEmail;
        private Label lblLastName;
        private Label lblFirstName;
        private Label lblInsurance;
        private Label lblDob;
        private Label lblMRN;
        private TextBox txtAddress1;
        private DateTimePicker txtDOB;
        private TextBox txtPhone1;
        private Label label1;
        private ComboBox txtInsurance;
        private TextBox txtAddress2;
        private TextBox txtAddress3;
        private RadioButton radAddress1;
        private RadioButton radAddress2;
        private RadioButton radAddress3;
        private Label label2;
        private TextBox txtPhone2;
        private Panel panel1;
        private TextBox txtGuardian;
        private Label label3;
        private SplitContainer splitContainer1;
    }
}