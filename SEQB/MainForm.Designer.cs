using System;
using System.Windows.Forms;

namespace SEQB
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.cbFamilyGroup = new System.Windows.Forms.ComboBox();
            this.lblFamilyGroup = new System.Windows.Forms.Label();
            this.btnViewInventories = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblAmountTitle = new System.Windows.Forms.Label();
            this.lblTaxTitle = new System.Windows.Forms.Label();
            this.lblQtyTitle = new System.Windows.Forms.Label();
            this.lblQty = new System.Windows.Forms.Label();
            this.lblTax = new System.Windows.Forms.Label();
            this.lblAmount = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lvInventories = new System.Windows.Forms.ListView();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.lblDate = new System.Windows.Forms.Label();
            this.btnCreateInvoice = new System.Windows.Forms.Button();
            this.LineNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PartNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FamilyGroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Qty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UnitPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Tax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Amount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Dummy = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbPlant = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbFamilyGroup
            // 
            this.cbFamilyGroup.FormattingEnabled = true;
            this.cbFamilyGroup.Location = new System.Drawing.Point(298, 42);
            this.cbFamilyGroup.Margin = new System.Windows.Forms.Padding(4);
            this.cbFamilyGroup.Name = "cbFamilyGroup";
            this.cbFamilyGroup.Size = new System.Drawing.Size(209, 25);
            this.cbFamilyGroup.TabIndex = 0;
            this.cbFamilyGroup.SelectedIndexChanged += new System.EventHandler(this.cbFamilyGroup_SelectedIndexChanged);
            // 
            // lblFamilyGroup
            // 
            this.lblFamilyGroup.AutoSize = true;
            this.lblFamilyGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblFamilyGroup.Location = new System.Drawing.Point(199, 46);
            this.lblFamilyGroup.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFamilyGroup.Name = "lblFamilyGroup";
            this.lblFamilyGroup.Size = new System.Drawing.Size(88, 17);
            this.lblFamilyGroup.TabIndex = 1;
            this.lblFamilyGroup.Text = "Family Group";
            // 
            // btnViewInventories
            // 
            this.btnViewInventories.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnViewInventories.Location = new System.Drawing.Point(824, 40);
            this.btnViewInventories.Margin = new System.Windows.Forms.Padding(4);
            this.btnViewInventories.Name = "btnViewInventories";
            this.btnViewInventories.Size = new System.Drawing.Size(129, 28);
            this.btnViewInventories.TabIndex = 2;
            this.btnViewInventories.Text = "View Inventories";
            this.btnViewInventories.UseVisualStyleBackColor = true;
            this.btnViewInventories.Click += new System.EventHandler(this.btnViewInventories_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbPlant);
            this.groupBox1.Controls.Add(this.lblAmountTitle);
            this.groupBox1.Controls.Add(this.lblTaxTitle);
            this.groupBox1.Controls.Add(this.lblQtyTitle);
            this.groupBox1.Controls.Add(this.lblQty);
            this.groupBox1.Controls.Add(this.lblTax);
            this.groupBox1.Controls.Add(this.lblAmount);
            this.groupBox1.Controls.Add(this.lblTotal);
            this.groupBox1.Controls.Add(this.lvInventories);
            this.groupBox1.Controls.Add(this.dtDate);
            this.groupBox1.Controls.Add(this.lblDate);
            this.groupBox1.Controls.Add(this.btnCreateInvoice);
            this.groupBox1.Controls.Add(this.lblFamilyGroup);
            this.groupBox1.Controls.Add(this.btnViewInventories);
            this.groupBox1.Controls.Add(this.cbFamilyGroup);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(41, 106);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1128, 422);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "View shipped inventories";
            // 
            // lblAmountTitle
            // 
            this.lblAmountTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAmountTitle.AutoSize = true;
            this.lblAmountTitle.Location = new System.Drawing.Point(956, 397);
            this.lblAmountTitle.Name = "lblAmountTitle";
            this.lblAmountTitle.Size = new System.Drawing.Size(73, 17);
            this.lblAmountTitle.TabIndex = 8;
            this.lblAmountTitle.Text = "Amount: ";
            // 
            // lblTaxTitle
            // 
            this.lblTaxTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTaxTitle.AutoSize = true;
            this.lblTaxTitle.Location = new System.Drawing.Point(849, 396);
            this.lblTaxTitle.Name = "lblTaxTitle";
            this.lblTaxTitle.Size = new System.Drawing.Size(42, 17);
            this.lblTaxTitle.TabIndex = 9;
            this.lblTaxTitle.Text = "Tax: ";
            // 
            // lblQtyTitle
            // 
            this.lblQtyTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblQtyTitle.AutoSize = true;
            this.lblQtyTitle.Location = new System.Drawing.Point(742, 397);
            this.lblQtyTitle.Name = "lblQtyTitle";
            this.lblQtyTitle.Size = new System.Drawing.Size(42, 17);
            this.lblQtyTitle.TabIndex = 10;
            this.lblQtyTitle.Text = "Qty: ";
            // 
            // lblQty
            // 
            this.lblQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblQty.AutoSize = true;
            this.lblQty.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblQty.Location = new System.Drawing.Point(790, 397);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(16, 17);
            this.lblQty.TabIndex = 11;
            this.lblQty.Text = "0";
            // 
            // lblTax
            // 
            this.lblTax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTax.AutoSize = true;
            this.lblTax.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTax.Location = new System.Drawing.Point(897, 397);
            this.lblTax.Name = "lblTax";
            this.lblTax.Size = new System.Drawing.Size(44, 17);
            this.lblTax.TabIndex = 12;
            this.lblTax.Text = "$0.00";
            // 
            // lblAmount
            // 
            this.lblAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAmount.AutoSize = true;
            this.lblAmount.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblAmount.Location = new System.Drawing.Point(1035, 397);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(44, 17);
            this.lblAmount.TabIndex = 13;
            this.lblAmount.Text = "$0.00";
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(661, 396);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(48, 17);
            this.lblTotal.TabIndex = 7;
            this.lblTotal.Text = "Total:";
            // 
            // lvInventories
            // 
            this.lvInventories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvInventories.BackColor = System.Drawing.SystemColors.Window;
            this.lvInventories.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvInventories.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lvInventories.GridLines = true;
            this.lvInventories.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvInventories.HideSelection = false;
            this.lvInventories.Location = new System.Drawing.Point(20, 94);
            this.lvInventories.Margin = new System.Windows.Forms.Padding(20, 40, 20, 40);
            this.lvInventories.Name = "lvInventories";
            this.lvInventories.Size = new System.Drawing.Size(1089, 287);
            this.lvInventories.TabIndex = 3;
            this.lvInventories.UseCompatibleStateImageBehavior = false;
            this.lvInventories.View = System.Windows.Forms.View.Details;
            // 
            // dtDate
            // 
            this.dtDate.Location = new System.Drawing.Point(585, 43);
            this.dtDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtDate.Name = "dtDate";
            this.dtDate.Size = new System.Drawing.Size(209, 24);
            this.dtDate.TabIndex = 6;
            this.dtDate.ValueChanged += new System.EventHandler(this.dtDate_ValueChanged);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDate.Location = new System.Drawing.Point(540, 45);
            this.lblDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(37, 17);
            this.lblDate.TabIndex = 5;
            this.lblDate.Text = "Date";
            // 
            // btnCreateInvoice
            // 
            this.btnCreateInvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreateInvoice.Enabled = false;
            this.btnCreateInvoice.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCreateInvoice.Location = new System.Drawing.Point(39, 386);
            this.btnCreateInvoice.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateInvoice.Name = "btnCreateInvoice";
            this.btnCreateInvoice.Size = new System.Drawing.Size(131, 28);
            this.btnCreateInvoice.TabIndex = 4;
            this.btnCreateInvoice.Text = "Create Invoice";
            this.btnCreateInvoice.UseVisualStyleBackColor = true;
            this.btnCreateInvoice.Click += new System.EventHandler(this.btnCreateInvoice_Click);
            // 
            // LineNum
            // 
            this.LineNum.Text = "#";
            this.LineNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.LineNum.Width = 30;
            // 
            // Id
            // 
            this.Id.Text = "ID";
            this.Id.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Id.Width = 47;
            // 
            // PartNumber
            // 
            this.PartNumber.Text = "Part Number";
            this.PartNumber.Width = 132;
            // 
            // FamilyGroup
            // 
            this.FamilyGroup.Text = "Family Group";
            this.FamilyGroup.Width = 97;
            // 
            // Description
            // 
            this.Description.Text = "Description";
            this.Description.Width = 420;
            // 
            // Qty
            // 
            this.Qty.Text = "Qty";
            this.Qty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Qty.Width = 56;
            // 
            // UnitPrice
            // 
            this.UnitPrice.Text = "Unit Price";
            this.UnitPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.UnitPrice.Width = 84;
            // 
            // Tax
            // 
            this.Tax.Text = "Tax";
            this.Tax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Tax.Width = 84;
            // 
            // Amount
            // 
            this.Amount.Text = "Amount";
            this.Amount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Amount.Width = 84;
            // 
            // Dummy
            // 
            this.Dummy.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Tahoma", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.DarkGray;
            this.label1.Location = new System.Drawing.Point(32, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(539, 53);
            this.label1.TabIndex = 4;
            this.label1.Text = "QuickBooks integration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(24, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 17);
            this.label2.TabIndex = 15;
            this.label2.Text = "Plant";
            // 
            // cbPlant
            // 
            this.cbPlant.FormattingEnabled = true;
            this.cbPlant.Location = new System.Drawing.Point(68, 42);
            this.cbPlant.Margin = new System.Windows.Forms.Padding(4);
            this.cbPlant.Name = "cbPlant";
            this.cbPlant.Size = new System.Drawing.Size(109, 25);
            this.cbPlant.TabIndex = 14;
            this.cbPlant.SelectedIndexChanged += new System.EventHandler(this.cbPlant_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 573);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(1245, 600);
            this.Name = "MainForm";
            this.Text = "Sample Express QuickBooks";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.FormClosing += MainForm_FormClosing;

        }


        #endregion

        private System.Windows.Forms.ComboBox cbFamilyGroup;
        private System.Windows.Forms.Label lblFamilyGroup;
        private System.Windows.Forms.Button btnViewInventories;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCreateInvoice;
        private System.Windows.Forms.ListView lvInventories;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.ColumnHeader LineNum;
        private System.Windows.Forms.ColumnHeader Id;
        private System.Windows.Forms.ColumnHeader PartNumber;
        private System.Windows.Forms.ColumnHeader FamilyGroup;
        private System.Windows.Forms.ColumnHeader Description;
        private System.Windows.Forms.ColumnHeader Qty;
        private System.Windows.Forms.ColumnHeader UnitPrice;
        private System.Windows.Forms.ColumnHeader Tax;
        private System.Windows.Forms.ColumnHeader Amount;
        private System.Windows.Forms.ColumnHeader Dummy;
        private Label lblTaxTitle;
        private Label lblAmountTitle;
        private Label lblQtyTitle;
        private Label lblQty;
        private Label lblTax;
        private Label lblAmount;
        private Label lblTotal;
        private Label label2;
        private ComboBox cbPlant;
    }
}

