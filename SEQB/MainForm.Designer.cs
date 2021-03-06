﻿using System;
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
            PresentationControls.CheckBoxProperties checkBoxProperties1 = new PresentationControls.CheckBoxProperties();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.LineNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InvoiceNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PartNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FamilyGroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InvoiceFamilyGroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Qty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InvoiceQty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UnitPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Tax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InvoiceTax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Amount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ItemShipDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InvoiceAmount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InvoiceShipDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InvoiceCreateDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Dummy = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Dummy2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblTitle = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbcbFamilyGroup = new PresentationControls.CheckBoxComboBox();
            this.lblDateTo = new System.Windows.Forms.Label();
            this.dtDateTo = new System.Windows.Forms.DateTimePicker();
            this.btnCreateInvoice = new System.Windows.Forms.Button();
            this.lblPlant = new System.Windows.Forms.Label();
            this.cbPlant = new System.Windows.Forms.ComboBox();
            this.lblAmountTitle = new System.Windows.Forms.Label();
            this.lblTaxTitle = new System.Windows.Forms.Label();
            this.lblQtyTitle = new System.Windows.Forms.Label();
            this.lblQty = new System.Windows.Forms.Label();
            this.lblTax = new System.Windows.Forms.Label();
            this.lblAmount = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lvInventories = new System.Windows.Forms.ListView();
            this.dtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblFamilyGroup = new System.Windows.Forms.Label();
            this.btnViewInventories = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnGenerateProofOfShipment = new System.Windows.Forms.Button();
            this.lvInvoices = new System.Windows.Forms.ListView();
            this.btnDeleteInvoice = new System.Windows.Forms.Button();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
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
            // InvoiceNumber
            // 
            this.InvoiceNumber.Text = "Invoice#";
            this.InvoiceNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            // InvoiceFamilyGroup
            // 
            this.InvoiceFamilyGroup.Text = "Family Group";
            this.InvoiceFamilyGroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InvoiceFamilyGroup.Width = 97;
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
            // InvoiceQty
            // 
            this.InvoiceQty.Text = "Qty";
            this.InvoiceQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.InvoiceQty.Width = 56;
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
            // InvoiceTax
            // 
            this.InvoiceTax.Text = "Tax";
            this.InvoiceTax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.InvoiceTax.Width = 84;
            // 
            // Amount
            // 
            this.Amount.Text = "Amount";
            this.Amount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Amount.Width = 84;
            // 
            // ItemShipDate
            // 
            this.ItemShipDate.Text = "Ship Date";
            this.ItemShipDate.Width = 84;
            // 
            // InvoiceAmount
            // 
            this.InvoiceAmount.Text = "Amount";
            this.InvoiceAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.InvoiceAmount.Width = 84;
            // 
            // InvoiceShipDate
            // 
            this.InvoiceShipDate.Text = "Ship Date";
            this.InvoiceShipDate.Width = 134;
            // 
            // InvoiceCreateDate
            // 
            this.InvoiceCreateDate.Text = "Created Date";
            this.InvoiceCreateDate.Width = 84;
            // 
            // Dummy
            // 
            this.Dummy.Text = "";
            // 
            // Dummy2
            // 
            this.Dummy2.Text = "";
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.SystemColors.Control;
            this.lblTitle.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitle.ForeColor = System.Drawing.Color.DarkGray;
            this.lblTitle.Location = new System.Drawing.Point(953, 27);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(197, 42);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "INVOICES";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(9, 86);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1167, 652);
            this.tabControl1.TabIndex = 16;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(1159, 626);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "View Shipped Inventories";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.cbcbFamilyGroup);
            this.groupBox1.Controls.Add(this.lblDateTo);
            this.groupBox1.Controls.Add(this.dtDateTo);
            this.groupBox1.Controls.Add(this.btnCreateInvoice);
            this.groupBox1.Controls.Add(this.lblPlant);
            this.groupBox1.Controls.Add(this.cbPlant);
            this.groupBox1.Controls.Add(this.lblAmountTitle);
            this.groupBox1.Controls.Add(this.lblTaxTitle);
            this.groupBox1.Controls.Add(this.lblQtyTitle);
            this.groupBox1.Controls.Add(this.lblQty);
            this.groupBox1.Controls.Add(this.lblTax);
            this.groupBox1.Controls.Add(this.lblAmount);
            this.groupBox1.Controls.Add(this.lblTotal);
            this.groupBox1.Controls.Add(this.lvInventories);
            this.groupBox1.Controls.Add(this.dtDateFrom);
            this.groupBox1.Controls.Add(this.lblDate);
            this.groupBox1.Controls.Add(this.lblFamilyGroup);
            this.groupBox1.Controls.Add(this.btnViewInventories);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(-5, -3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1168, 633);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // cbcbFamilyGroup
            // 
            checkBoxProperties1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbcbFamilyGroup.CheckBoxProperties = checkBoxProperties1;
            this.cbcbFamilyGroup.DisplayMemberSingleItem = "";
            this.cbcbFamilyGroup.FormattingEnabled = true;
            this.cbcbFamilyGroup.Location = new System.Drawing.Point(259, 35);
            this.cbcbFamilyGroup.Name = "cbcbFamilyGroup";
            this.cbcbFamilyGroup.Size = new System.Drawing.Size(210, 21);
            this.cbcbFamilyGroup.TabIndex = 18;
            // 
            // lblDateTo
            // 
            this.lblDateTo.AutoSize = true;
            this.lblDateTo.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDateTo.Location = new System.Drawing.Point(769, 38);
            this.lblDateTo.Name = "lblDateTo";
            this.lblDateTo.Size = new System.Drawing.Size(17, 13);
            this.lblDateTo.TabIndex = 17;
            this.lblDateTo.Text = "to";
            // 
            // dtDateTo
            // 
            this.dtDateTo.Location = new System.Drawing.Point(789, 34);
            this.dtDateTo.Name = "dtDateTo";
            this.dtDateTo.Size = new System.Drawing.Size(211, 21);
            this.dtDateTo.TabIndex = 16;
            this.dtDateTo.ValueChanged += new System.EventHandler(this.dtDateTo_ValueChanged);
            // 
            // btnCreateInvoice
            // 
            this.btnCreateInvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreateInvoice.Enabled = false;
            this.btnCreateInvoice.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCreateInvoice.Location = new System.Drawing.Point(27, 592);
            this.btnCreateInvoice.Name = "btnCreateInvoice";
            this.btnCreateInvoice.Size = new System.Drawing.Size(101, 24);
            this.btnCreateInvoice.TabIndex = 4;
            this.btnCreateInvoice.Text = "Create Invoice";
            this.btnCreateInvoice.UseVisualStyleBackColor = true;
            this.btnCreateInvoice.Click += new System.EventHandler(this.btnCreateInvoice_Click);
            // 
            // lblPlant
            // 
            this.lblPlant.AutoSize = true;
            this.lblPlant.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblPlant.Location = new System.Drawing.Point(17, 38);
            this.lblPlant.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPlant.Name = "lblPlant";
            this.lblPlant.Size = new System.Drawing.Size(35, 13);
            this.lblPlant.TabIndex = 15;
            this.lblPlant.Text = "Plant:";
            // 
            // cbPlant
            // 
            this.cbPlant.FormattingEnabled = true;
            this.cbPlant.Location = new System.Drawing.Point(56, 34);
            this.cbPlant.Margin = new System.Windows.Forms.Padding(4);
            this.cbPlant.Name = "cbPlant";
            this.cbPlant.Size = new System.Drawing.Size(109, 21);
            this.cbPlant.TabIndex = 14;
            this.cbPlant.SelectedIndexChanged += new System.EventHandler(this.cbPlant_SelectedIndexChanged);
            // 
            // lblAmountTitle
            // 
            this.lblAmountTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAmountTitle.AutoSize = true;
            this.lblAmountTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAmountTitle.Location = new System.Drawing.Point(981, 595);
            this.lblAmountTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAmountTitle.Name = "lblAmountTitle";
            this.lblAmountTitle.Size = new System.Drawing.Size(75, 18);
            this.lblAmountTitle.TabIndex = 8;
            this.lblAmountTitle.Text = "Amount: ";
            // 
            // lblTaxTitle
            // 
            this.lblTaxTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTaxTitle.AutoSize = true;
            this.lblTaxTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTaxTitle.Location = new System.Drawing.Point(862, 595);
            this.lblTaxTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTaxTitle.Name = "lblTaxTitle";
            this.lblTaxTitle.Size = new System.Drawing.Size(45, 18);
            this.lblTaxTitle.TabIndex = 9;
            this.lblTaxTitle.Text = "Tax: ";
            // 
            // lblQtyTitle
            // 
            this.lblQtyTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblQtyTitle.AutoSize = true;
            this.lblQtyTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQtyTitle.Location = new System.Drawing.Point(757, 595);
            this.lblQtyTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblQtyTitle.Name = "lblQtyTitle";
            this.lblQtyTitle.Size = new System.Drawing.Size(44, 18);
            this.lblQtyTitle.TabIndex = 10;
            this.lblQtyTitle.Text = "Qty: ";
            // 
            // lblQty
            // 
            this.lblQty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblQty.AutoSize = true;
            this.lblQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQty.Location = new System.Drawing.Point(806, 596);
            this.lblQty.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(16, 18);
            this.lblQty.TabIndex = 11;
            this.lblQty.Text = "0";
            // 
            // lblTax
            // 
            this.lblTax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTax.AutoSize = true;
            this.lblTax.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTax.Location = new System.Drawing.Point(912, 596);
            this.lblTax.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTax.Name = "lblTax";
            this.lblTax.Size = new System.Drawing.Size(44, 18);
            this.lblTax.TabIndex = 12;
            this.lblTax.Text = "$0.00";
            // 
            // lblAmount
            // 
            this.lblAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAmount.AutoSize = true;
            this.lblAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAmount.Location = new System.Drawing.Point(1061, 596);
            this.lblAmount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(44, 18);
            this.lblAmount.TabIndex = 13;
            this.lblAmount.Text = "$0.00";
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(496, 1014);
            this.lblTotal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(39, 13);
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
            this.lvInventories.Location = new System.Drawing.Point(2, 76);
            this.lvInventories.Margin = new System.Windows.Forms.Padding(15, 32, 15, 32);
            this.lvInventories.Name = "lvInventories";
            this.lvInventories.Size = new System.Drawing.Size(1163, 500);
            this.lvInventories.TabIndex = 3;
            this.lvInventories.UseCompatibleStateImageBehavior = false;
            this.lvInventories.View = System.Windows.Forms.View.Details;
            // 
            // dtDateFrom
            // 
            this.dtDateFrom.Location = new System.Drawing.Point(556, 34);
            this.dtDateFrom.Name = "dtDateFrom";
            this.dtDateFrom.Size = new System.Drawing.Size(211, 21);
            this.dtDateFrom.TabIndex = 6;
            this.dtDateFrom.ValueChanged += new System.EventHandler(this.dtDateFrom_ValueChanged);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblDate.Location = new System.Drawing.Point(475, 38);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(80, 13);
            this.lblDate.TabIndex = 5;
            this.lblDate.Text = "Shipped:   from";
            // 
            // lblFamilyGroup
            // 
            this.lblFamilyGroup.AutoSize = true;
            this.lblFamilyGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblFamilyGroup.Location = new System.Drawing.Point(182, 37);
            this.lblFamilyGroup.Name = "lblFamilyGroup";
            this.lblFamilyGroup.Size = new System.Drawing.Size(73, 13);
            this.lblFamilyGroup.TabIndex = 1;
            this.lblFamilyGroup.Text = "Family Group:";
            // 
            // btnViewInventories
            // 
            this.btnViewInventories.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnViewInventories.Location = new System.Drawing.Point(1043, 32);
            this.btnViewInventories.Name = "btnViewInventories";
            this.btnViewInventories.Size = new System.Drawing.Size(97, 23);
            this.btnViewInventories.TabIndex = 2;
            this.btnViewInventories.Text = "Refresh View";
            this.btnViewInventories.UseVisualStyleBackColor = true;
            this.btnViewInventories.Click += new System.EventHandler(this.btnViewInventories_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(1159, 626);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Open Invoices";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.btnGenerateProofOfShipment);
            this.groupBox2.Controls.Add(this.lvInvoices);
            this.groupBox2.Controls.Add(this.btnDeleteInvoice);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.Location = new System.Drawing.Point(0, -6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1156, 629);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            // 
            // btnGenerateProofOfShipment
            // 
            this.btnGenerateProofOfShipment.AutoSize = true;
            this.btnGenerateProofOfShipment.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGenerateProofOfShipment.Location = new System.Drawing.Point(161, 600);
            this.btnGenerateProofOfShipment.Name = "btnGenerateProofOfShipment";
            this.btnGenerateProofOfShipment.Size = new System.Drawing.Size(158, 23);
            this.btnGenerateProofOfShipment.TabIndex = 20;
            this.btnGenerateProofOfShipment.Text = "Generate proof of shipment";
            this.btnGenerateProofOfShipment.UseVisualStyleBackColor = true;
            this.btnGenerateProofOfShipment.Click += new System.EventHandler(this.btnGenerateProofOfShipment_Click);
            // 
            // lvInvoices
            // 
            this.lvInvoices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvInvoices.BackColor = System.Drawing.SystemColors.Window;
            this.lvInvoices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvInvoices.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lvInvoices.FullRowSelect = true;
            this.lvInvoices.GridLines = true;
            this.lvInvoices.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvInvoices.HideSelection = false;
            this.lvInvoices.Location = new System.Drawing.Point(15, 76);
            this.lvInvoices.Margin = new System.Windows.Forms.Padding(15, 32, 15, 32);
            this.lvInvoices.Name = "lvInvoices";
            this.lvInvoices.Size = new System.Drawing.Size(1127, 515);
            this.lvInvoices.TabIndex = 3;
            this.lvInvoices.UseCompatibleStateImageBehavior = false;
            this.lvInvoices.View = System.Windows.Forms.View.Details;
            this.lvInvoices.SelectedIndexChanged += new System.EventHandler(this.lvInvoices_SelectedIndexChanged);
            // 
            // btnDeleteInvoice
            // 
            this.btnDeleteInvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteInvoice.Enabled = false;
            this.btnDeleteInvoice.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDeleteInvoice.Location = new System.Drawing.Point(29, 600);
            this.btnDeleteInvoice.Name = "btnDeleteInvoice";
            this.btnDeleteInvoice.Size = new System.Drawing.Size(98, 23);
            this.btnDeleteInvoice.TabIndex = 4;
            this.btnDeleteInvoice.Text = "Delete Invoice";
            this.btnDeleteInvoice.UseVisualStyleBackColor = true;
            this.btnDeleteInvoice.Click += new System.EventHandler(this.btnDeleteInvoice_Click);
            // 
            // pbLogo
            // 
            this.pbLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbLogo.Image")));
            this.pbLogo.Location = new System.Drawing.Point(0, 2);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(304, 80);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogo.TabIndex = 17;
            this.pbLogo.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.progressBar1.Location = new System.Drawing.Point(373, 83);
            this.progressBar1.MarqueeAnimationSpeed = 5;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(500, 14);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 18;
            this.progressBar1.UseWaitCursor = true;
            this.progressBar1.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1185, 741);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(938, 495);
            this.Name = "MainForm";
            this.Text = "Sample Express";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void TabPage2_Click1(object sender, EventArgs e)
        {
            fillLvInvoices();
        }

        private void TabPage1_Click(object sender, EventArgs e)
        {
            //fillLvInventories();
            btnViewInventories_Click(this, null);
        }

        
        #endregion
        private ColumnHeader LineNum;
        private ColumnHeader Id;
        private ColumnHeader PartNumber;
        private ColumnHeader FamilyGroup;
        private ColumnHeader Description;
        private ColumnHeader Qty;
        private ColumnHeader UnitPrice;
        private ColumnHeader Tax;
        private ColumnHeader Amount;
        private ColumnHeader ItemShipDate;
        private ColumnHeader Dummy;

        private ColumnHeader InvoiceNumber;
        private ColumnHeader InvoiceFamilyGroup;
        private ColumnHeader InvoiceQty;
        private ColumnHeader InvoiceTax;
        private ColumnHeader InvoiceAmount;
        private ColumnHeader InvoiceShipDate;
        private ColumnHeader InvoiceCreateDate;
        private ColumnHeader Dummy2;

        private TabControl tabControl1;

        private TabPage tabPage1;
        private TabPage tabPage2;

        private GroupBox groupBox1;
        private GroupBox groupBox2;

        private ComboBox cbPlant;

        private Label lblPlant;
        private Label lblTitle;
        private Label lblAmountTitle;
        private Label lblTaxTitle;
        private Label lblQtyTitle;
        private Label lblQty;
        private Label lblTax;
        private Label lblAmount;
        private Label lblTotal;
        private Label lblDate;
        private Label lblFamilyGroup;

        private ListView lvInventories;
        private ListView lvInvoices;

        private DateTimePicker dtDateFrom;
        
        private Button btnCreateInvoice;
        private Button btnViewInventories;
        private Button btnDeleteInvoice;
        private Label lblDateTo;
        private DateTimePicker dtDateTo;
        private PictureBox pbLogo;
        private PresentationControls.CheckBoxComboBox cbcbFamilyGroup;
        private ProgressBar progressBar1;
        private Button btnGenerateProofOfShipment;
    }
}

