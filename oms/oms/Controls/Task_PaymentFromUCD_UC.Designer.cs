namespace oms.Controls
{
    partial class Task_PaymentFromUCD_UC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtTotalBillCharged = new System.Windows.Forms.TextBox();
            this.txtTotalCOGOrder = new System.Windows.Forms.TextBox();
            this.txtTotalPaymentRecd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkMarkasComplete = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1138, 362);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtTotalBillCharged);
            this.panel2.Controls.Add(this.txtTotalCOGOrder);
            this.panel2.Controls.Add(this.txtTotalPaymentRecd);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.chkMarkasComplete);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 371);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1144, 108);
            this.panel2.TabIndex = 1;
            // 
            // txtTotalBillCharged
            // 
            this.txtTotalBillCharged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalBillCharged.Location = new System.Drawing.Point(865, 69);
            this.txtTotalBillCharged.Name = "txtTotalBillCharged";
            this.txtTotalBillCharged.ReadOnly = true;
            this.txtTotalBillCharged.Size = new System.Drawing.Size(252, 23);
            this.txtTotalBillCharged.TabIndex = 2;
            // 
            // txtTotalCOGOrder
            // 
            this.txtTotalCOGOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalCOGOrder.Location = new System.Drawing.Point(865, 42);
            this.txtTotalCOGOrder.Name = "txtTotalCOGOrder";
            this.txtTotalCOGOrder.ReadOnly = true;
            this.txtTotalCOGOrder.Size = new System.Drawing.Size(252, 23);
            this.txtTotalCOGOrder.TabIndex = 2;
            // 
            // txtTotalPaymentRecd
            // 
            this.txtTotalPaymentRecd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalPaymentRecd.Location = new System.Drawing.Point(865, 15);
            this.txtTotalPaymentRecd.Name = "txtTotalPaymentRecd";
            this.txtTotalPaymentRecd.ReadOnly = true;
            this.txtTotalPaymentRecd.Size = new System.Drawing.Size(252, 23);
            this.txtTotalPaymentRecd.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(701, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Total Bill Charged";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(707, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Total COG Order";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(668, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Total Payment Received";
            // 
            // chkMarkasComplete
            // 
            this.chkMarkasComplete.AutoSize = true;
            this.chkMarkasComplete.Location = new System.Drawing.Point(14, 43);
            this.chkMarkasComplete.Name = "chkMarkasComplete";
            this.chkMarkasComplete.Size = new System.Drawing.Size(122, 19);
            this.chkMarkasComplete.TabIndex = 0;
            this.chkMarkasComplete.Text = "Mark as Complete";
            this.chkMarkasComplete.UseVisualStyleBackColor = true;
            // 
            // Task_PaymentFromUCD_UC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Task_PaymentFromUCD_UC";
            this.Size = new System.Drawing.Size(1144, 479);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private CheckBox chkMarkasComplete;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox txtTotalBillCharged;
        private TextBox txtTotalCOGOrder;
        private TextBox txtTotalPaymentRecd;
    }
}
