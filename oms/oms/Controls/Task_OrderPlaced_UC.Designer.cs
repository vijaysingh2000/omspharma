namespace oms.Controls
{
    partial class Task_OrderPlaced_UC
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkMarkasComplete = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDrugName = new System.Windows.Forms.Label();
            this.panGrid = new System.Windows.Forms.Panel();
            this.panGrid1 = new System.Windows.Forms.Panel();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chkMarkasComplete);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 838);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1696, 70);
            this.panel2.TabIndex = 1;
            // 
            // chkMarkasComplete
            // 
            this.chkMarkasComplete.AutoSize = true;
            this.chkMarkasComplete.Location = new System.Drawing.Point(20, 18);
            this.chkMarkasComplete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkMarkasComplete.Name = "chkMarkasComplete";
            this.chkMarkasComplete.Size = new System.Drawing.Size(182, 29);
            this.chkMarkasComplete.TabIndex = 0;
            this.chkMarkasComplete.Text = "Mark as Complete";
            this.chkMarkasComplete.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblDrugName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1696, 50);
            this.panel1.TabIndex = 2;
            // 
            // lblDrugName
            // 
            this.lblDrugName.AutoSize = true;
            this.lblDrugName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDrugName.ForeColor = System.Drawing.Color.Red;
            this.lblDrugName.Location = new System.Drawing.Point(13, 12);
            this.lblDrugName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDrugName.Name = "lblDrugName";
            this.lblDrugName.Size = new System.Drawing.Size(63, 25);
            this.lblDrugName.TabIndex = 0;
            this.lblDrugName.Text = "label1";
            // 
            // panGrid
            // 
            this.panGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panGrid.Location = new System.Drawing.Point(4, 458);
            this.panGrid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panGrid.Name = "panGrid";
            this.panGrid.Size = new System.Drawing.Size(1687, 370);
            this.panGrid.TabIndex = 0;
            // 
            // panGrid1
            // 
            this.panGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panGrid1.Location = new System.Drawing.Point(4, 60);
            this.panGrid1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panGrid1.Name = "panGrid1";
            this.panGrid1.Size = new System.Drawing.Size(1687, 388);
            this.panGrid1.TabIndex = 0;
            // 
            // Task_OrderPlaced_UC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panGrid);
            this.Controls.Add(this.panGrid1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Task_OrderPlaced_UC";
            this.Size = new System.Drawing.Size(1696, 908);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Panel panel2;
        private CheckBox chkMarkasComplete;
        private Panel panel1;
        private Label lblDrugName;
        private Panel panGrid;
        private Panel panGrid1;
    }
}
