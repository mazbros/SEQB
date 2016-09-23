﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Globalization;
using QBFC13Lib;
using System.Linq;

namespace SEQB
{
    public partial class MainForm : Form
    {
        private int _totalQty;
        private double _totalTax;
        private double _totalAmount;
        private string _pkgIdsForUpdate;

        private static readonly string NewLine = Environment.NewLine;

        private DataTable _dt;

        public MainForm()
        {
            InitializeComponent();
            QBHelper.MsgEvent += QBHelper_MsgEvenHandler;

            MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            FillcbPlant();

            FillcbFamilyGroup();

        }

        private void FillcbPlant()
        {
            var plants = new List<Plant> {new Plant {Name = "PA", Id = 1}, new Plant() {Name = "MO", Id = 2}};
            cbPlant.DataSource = plants;
            cbPlant.ValueMember = "Id";
            cbPlant.DisplayMember = "Name";
        }

        private void FillcbFamilyGroup()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                var cmd = new SqlCommand(@"
                    select FamilyGroup from InventoryItem
                    where FamilyGroup <> ''
                    group by FamilyGroup", conn);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                cbFamilyGroup.DataSource = dt;
                cbFamilyGroup.ValueMember = "FamilyGroup";
                cbFamilyGroup.DisplayMember = "FamilyGroup";
            }
        }

        private void FilllvInventories()
        {
            lvInventories.Items.Clear();
            lvInventories.View = View.Details;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                var cmd = new SqlCommand("FamilyGroupItemsForQBInvoice", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                var plant = cbPlant.SelectedItem as Plant;
                if (plant != null)
                    cmd.Parameters.Add(new SqlParameter("@plant_Id", plant.Id));
                cmd.Parameters.Add(new SqlParameter("@FG", cbFamilyGroup.Text));
                cmd.Parameters.Add(new SqlParameter("@date", dtDate.Value));
                cmd.Parameters.Add(new SqlParameter("@TQty", DbType.Int16));
                cmd.Parameters.Add(new SqlParameter("@TTax", SqlDbType.VarChar, 16));
                cmd.Parameters.Add(new SqlParameter("@TAmount", SqlDbType.VarChar, 16));
                cmd.Parameters.Add(new SqlParameter("@TIds", SqlDbType.NVarChar, -1));
                cmd.Parameters["@TQty"].Direction = ParameterDirection.Output;
                cmd.Parameters["@TTax"].Direction = ParameterDirection.Output;
                cmd.Parameters["@TAmount"].Direction = ParameterDirection.Output;
                cmd.Parameters["@TIds"].Direction = ParameterDirection.Output;
                _dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(_dt);

                foreach (DataRow row in _dt.Rows)
                {
                    var item = new ListViewItem(row[0].ToString());
                    // skip 2 last columns (auxilary: Amount, TaxRef)
                    for (var i = 1; i < _dt.Columns.Count - 2; i++)
                    {
                        item.SubItems.Add(row[i].ToString());
                    }
                    lvInventories.Items.Add(item);
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();

                _totalQty = int.Parse(cmd.Parameters["@TQty"].Value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                _totalTax = double.Parse(cmd.Parameters["@TTax"].Value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                _totalAmount = double.Parse(cmd.Parameters["@TAmount"].Value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                _pkgIdsForUpdate = cmd.Parameters["@TIds"].Value.ToString();

                lblQty.Text = _totalQty.ToString(CultureInfo.GetCultureInfo("en-US"));
                lblTax.Text = _totalTax.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
                lblAmount.Text = _totalAmount.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
            }

            SizeLastColumn(lvInventories);

            btnCreateInvoice.Enabled = lvInventories.Items.Count > 0;
        }

        private IEnumerable<Invoice> GetAllCreatedInvoices()
        {
            var ret = new List<Invoice>();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                conn.Open();
                var query = @"
                    select Invoice from PackageTrack
                    where Invoice is not null
                    group by Invoice";
                using (var command = new SqlCommand(query, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add(new Invoice() { InvoiceNumber = reader.GetString(0) });
                        }
                    }
                }
            }
            return ret;
        }

        private IEnumerable<Invoice> GetFilteredInvoicesFromQB(List<Invoice> list)
        {
            var unPaidInvoices = new List<Invoice>();
            using (var sessionManager = SessionManager.GetInstance)
            {
                var qbSessionManager = sessionManager.OpenSession();
                // Get the RequestMsgSet based on the correct QB Version
                var requestMsgSet = QBHelper.GetLatestMsgSetRequest(qbSessionManager);
                // Initialize the message set request object
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                QBHelper.EnableErrorRecovery(qbSessionManager);

                // Add the request to the message set request object
                var invoiceQuery = requestMsgSet.AppendInvoiceQueryRq();
                //invoiceQuery.ORInvoiceQuery.InvoiceFilter. ORRefNumberFilter. RefNumberFilter. MatchCriterion.SetValue(ENMatchCriterion.mcContains);
                //invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(refNum);
                invoiceQuery.ORInvoiceQuery.InvoiceFilter.PaidStatus.SetValue(ENPaidStatus.psNotPaidOnly);
                //foreach (var inv in list)
                //{
                //    invoiceQuery.ORInvoiceQuery.RefNumberList.Add(inv.InvoiceNumber);
                //}
                invoiceQuery.IncludeLineItems.SetValue(false);
                invoiceQuery.IncludeLinkedTxns.SetValue(false);

                var responseMsgSet = qbSessionManager.DoRequests(requestMsgSet);
                var response = responseMsgSet.ResponseList.GetAt(0);
                var invoiceRetList = (IInvoiceRetList)response.Detail;

                for (var i = 0; i < invoiceRetList.Count; i++)
                {
                    var invoiceRet = invoiceRetList.GetAt(i);
                    var item = new Invoice
                    {
                        InvoiceNumber = invoiceRet.RefNumber.GetValue(),
                        TrxnDate = invoiceRet.TimeCreated.GetValue().ToString("d")
                    };
                    unPaidInvoices.Add(item);
                    //list.RemoveAll(x => x.InvoiceNumber == invoiceRet.RefNumber.GetValue());
                    //returnVal = (int.Parse(invoiceRet.RefNumber.GetValue()) + 1).ToString();
                }
            }
            foreach (var invoice in list)
                foreach (var unPaidInvoice in unPaidInvoices)
                    if (invoice.InvoiceNumber == unPaidInvoice.InvoiceNumber)
                        invoice.TrxnDate = unPaidInvoice.TrxnDate;
            return list.Where(x => unPaidInvoices.Select(i => i.InvoiceNumber).Contains(x.InvoiceNumber));
        }

        private void FilllvInvoices()
        {
            lvInvoices.Items.Clear();
            lvInvoices.View = View.Details;

            var invoices = GetAllCreatedInvoices();
            invoices = GetFilteredInvoicesFromQB(invoices.ToList());

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                conn.Open();

                foreach(var invoice in invoices)
                {
                    var cmd = new SqlCommand("SummaryForQBInvoiceDelete", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.Add(new SqlParameter("@InvoiceNum", invoice.InvoiceNumber));
                    cmd.Parameters.Add(new SqlParameter("@TrxnDate", SqlDbType.VarChar, 16));
                    cmd.Parameters.Add(new SqlParameter("@TQty", DbType.Int16));
                    cmd.Parameters.Add(new SqlParameter("@TTax", SqlDbType.VarChar, 16));
                    cmd.Parameters.Add(new SqlParameter("@TAmount", SqlDbType.VarChar, 16));
                    cmd.Parameters["@TrxnDate"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@TQty"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@TTax"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@TAmount"].Direction = ParameterDirection.Output;

                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();

                    invoice.ShipDate = cmd.Parameters["@TrxnDate"].Value.ToString();
                    invoice.TrxnDate = invoice.TrxnDate;
                    invoice.Qty = int.Parse(cmd.Parameters["@TQty"].Value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                    invoice.Tax = double.Parse(cmd.Parameters["@TTax"].Value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                    invoice.Amount = double.Parse(cmd.Parameters["@TAmount"].Value.ToString(), CultureInfo.GetCultureInfo("en-US"));

                    var item = new ListViewItem(invoice.InvoiceNumber) {Tag = invoice};
                    item.SubItems.Add(invoice.ShipDate.ToString(CultureInfo.GetCultureInfo("en-US")));
                    item.SubItems.Add(invoice.TrxnDate.ToString(CultureInfo.GetCultureInfo("en-US")));
                    item.SubItems.Add(invoice.Qty.ToString(CultureInfo.GetCultureInfo("en-US")));
                    item.SubItems.Add(invoice.Tax.ToString("C2", CultureInfo.GetCultureInfo("en-US")));
                    item.SubItems.Add(invoice.Amount.ToString("C2", CultureInfo.GetCultureInfo("en-US")));
                    lvInvoices.Items.Add(item);
                }
            }

            SizeLastColumn(lvInvoices);

            btnDeleteInvoice.Enabled = lvInvoices.SelectedItems.Count > 0;
        }


        private void cbFamilyGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilllvInventories();
        }

        private void dtDate_ValueChanged(object sender, EventArgs e)
        {
            FilllvInventories();
        }

        private void cbPlant_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilllvInventories();
        }

        private void btnViewInventories_Click(object sender, EventArgs e)
        {
            FilllvInventories();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lvInventories.View = View.Details;
            lvInventories.Columns.AddRange(new[] { LineNum, Id, PartNumber, FamilyGroup, Description, Qty, UnitPrice, Tax, Amount, Dummy });
            SizeLastColumn(lvInventories);

            lvInvoices.View = View.Details;
            lvInvoices.Columns.AddRange(new[] { InvoiceNumber, InvoiceShipDate, InvoiceTrxnDate, InvoiceQty, InvoiceTax, InvoiceAmount, Dummy2 });
            SizeLastColumn(lvInvoices);
            FilllvInvoices();
        }

        private void SizeLastColumn(ListView lv)
        {
            if(lv.Columns.Count > 0)
                lv.Columns[lv.Columns.Count - 1].Width = -2;
        }
        
        private void btnCreateInvoice_Click(object sender, EventArgs e)
        {
            var disabledButton = (Button) sender;
            disabledButton.Enabled = false;

            QBFC_AddInvoice();
            FilllvInventories();
            FilllvInvoices();
        }

        private void QBFC_AddInvoice()
        {
            //TODO: make sure this is correct and doesn't need to be a selection from list
            var custFN = "GAF MC Access & Low Slope";

            var invoiceNumber = GetNextInvoiceNumber();

            var billTo = GetBillTo(custFN);

            using (var sessionManager = SessionManager.GetInstance)
            {
                var qbSessionManager = sessionManager.OpenSession();
                // Get the RequestMsgSet based on the correct QB Version
                var requestMsgSet = QBHelper.GetLatestMsgSetRequest(qbSessionManager);
                // Initialize the message set request object
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                QBHelper.EnableErrorRecovery(qbSessionManager);

                try
                {
                    // Add the request to the message set request object
                    var invoiceAdd = requestMsgSet.AppendInvoiceAddRq();

                    // Set the IInvoiceAdd field values
                    // Customer:Job
                    invoiceAdd.CustomerRef.FullName.SetValue(custFN);

                    // Invoice Date
                    invoiceAdd.TxnDate.SetValue(dtDate.Value);

                    // Invoice Number
                    invoiceAdd.RefNumber.SetValue(invoiceNumber);

                    // Bill Address
                    invoiceAdd.BillAddress.Addr1.SetValue(billTo.Addr1);
                    invoiceAdd.BillAddress.Addr2.SetValue(billTo.Addr2);
                    invoiceAdd.BillAddress.Addr3.SetValue(billTo.Addr3);
                    invoiceAdd.BillAddress.Addr4.SetValue(billTo.Addr4);
                    invoiceAdd.BillAddress.City.SetValue(billTo.City);
                    invoiceAdd.BillAddress.State.SetValue(billTo.State);
                    invoiceAdd.BillAddress.PostalCode.SetValue(billTo.PostalCode);
                    invoiceAdd.BillAddress.Country.SetValue(billTo.Country);

                    // P.O. Number
                    //TODO: make sure this is correct
                    invoiceAdd.PONumber.SetValue("GAFSAMPLES");
                    // Terms
                    //string terms = "cmboBx1_Terms.Text";
                    //if (terms.IndexOf("Please select one from list") >= 0)
                    //{
                    //    terms = "";
                    //}
                    //if (!terms.Equals(""))
                    //{
                    //    invoiceAdd.TermsRef.FullName.SetValue(terms);
                    //}

                    // Due Date
                    invoiceAdd.DueDate.SetValue(DateTime.Now.AddDays(30));

                    // Set the values for the invoice line
                    for (var i = 0; i < _dt.Rows.Count; i++)
                    {
                        var row = _dt.Rows[i];
                        // Create the line item for the invoice
                        var invoiceLineAdd = invoiceAdd.ORInvoiceLineAddList.Append().InvoiceLineAdd;
                        invoiceLineAdd.ItemRef.FullName.SetValue(row["Description"].ToString());
                        invoiceLineAdd.Quantity.SetValue(Convert.ToDouble(row["Qty"].ToString(), CultureInfo.GetCultureInfo("en-US")));
                        invoiceLineAdd.SalesTaxCodeRef.FullName.SetValue(row["TaxRef"].ToString()
                            .Equals("Non-Taxable Sales")
                            ? "Non"
                            : "Tax");
                    } // for

                    // Entire invoice sales tax rate, can only be for a single plant (state: PA, MO, MA)
                    invoiceAdd.ItemSalesTaxRef.FullName.SetValue(cbPlant.Text + " Sales Tax");
                    invoiceAdd.ClassRef.FullName.SetValue(cbPlant.Text);

                    if(QBHelper.ShowRequestResult(qbSessionManager, requestMsgSet))
                        UpdatePackagesInvoiceCreated(invoiceNumber, _pkgIdsForUpdate);

                    qbSessionManager.ClearErrorRecovery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + NewLine + @"Stack Trace: " + NewLine + ex.StackTrace + NewLine + @"Exiting the application");
                }
            }
        } // method QBFC_AddInvoice

        private BillAddress GetBillTo(string custFn)
        {
            BillAddress ret = null;

            using (var sessionManager = SessionManager.GetInstance)
            {
                try
                {
                    var qbSessionManager = sessionManager.OpenSession();
                    // Get the RequestMsgSet based on the correct QB Version
                    var requestMsgSet = QBHelper.GetLatestMsgSetRequest(qbSessionManager);
                    // Initialize the message set request object
                    requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                    QBHelper.EnableErrorRecovery(qbSessionManager);

                    // Add the request to the message set request object
                    var custQ = requestMsgSet.AppendCustomerQueryRq();

                    /* Following VB example is taken from QBFC/DevGuide/pg100
                    Example 3
                    This query uses a list object to look for the employee records of two specific employees:
                    Dim empQuery As QBFCxLib.IEmployeeQuery
                    empQuery.ORListQuery.FullNameList.Add "Almira Smith"
                    empQuery.ORListQuery.FullNameList.Add "Sisko Jones"
                    */
                    custQ.ORCustomerListQuery.FullNameList.Add(custFn);

                    // Optionally, you can put filter on it.
                    // CustQ.ORCustomerListQuery.CustomerListFilter.MaxReturned.SetValue(50);


                    // Do the request and get the response message set object
                    var responseSet = qbSessionManager.DoRequests(requestMsgSet);

                    // Uncomment the following to view and save the request and response XML
                    // string requestXML = requestSet.ToXMLString();
                    // MessageBox.Show(requestXML);
                    // SaveXML(requestXML);
                    // string responseXML = responseSet.ToXMLString();
                    // MessageBox.Show(responseXML);
                    // SaveXML(responseXML);

                    var response = responseSet.ResponseList.GetAt(0);
                    // int statusCode = response.StatusCode;
                    // string statusMessage = response.StatusMessage;
                    // string statusSeverity = response.StatusSeverity;
                    // MessageBox.Show("Status:\nCode = " + statusCode + "\nMessage = " + statusMessage + "\nSeverity = " + statusSeverity);


                    var customerRetList = response.Detail as ICustomerRetList;
                    if (customerRetList != null && customerRetList.Count != 0)
                    {
                        var customerRet = customerRetList.GetAt(0);
                        if (customerRet.BillAddress != null)
                        {
                            ret = new BillAddress
                            {
                                Addr1 = customerRet.BillAddress.Addr1?.GetValue(),
                                Addr2 = customerRet.BillAddress.Addr2?.GetValue(),
                                Addr3 = customerRet.BillAddress.Addr3?.GetValue(),
                                Addr4 = customerRet.BillAddress.Addr4?.GetValue(),
                                City = customerRet.BillAddress.City?.GetValue(),
                                Country = customerRet.BillAddress.Country?.GetValue(),
                                PostalCode = customerRet.BillAddress.PostalCode?.GetValue(),
                                State = customerRet.BillAddress.State?.GetValue()
                            };
                        } // if BillAddress is not null
                    } // if customeRetList.count not equals zero
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + @"\nStack Trace: \n" + ex.StackTrace + @"\nExiting the application");
                }
            }
            return ret;
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            SizeLastColumn(lvInventories);
            SizeLastColumn(lvInvoices);
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            SizeLastColumn(lvInventories);
            SizeLastColumn(lvInvoices);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            SizeLastColumn(lvInventories);
            SizeLastColumn(lvInvoices);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            QBHelper.MsgEvent -= QBHelper_MsgEvenHandler;
        }

        private void QBHelper_MsgEvenHandler(object sender, QBMsgEventArgs e)
        {
            MessageBox.Show(e.Msg);
        }

        private string GetNextInvoiceNumber()
        {
            const int daysBack = -60;
            var returnVal = string.Empty;
            using (var sessionManager = SessionManager.GetInstance)
            {
                var qbSessionManager = sessionManager.OpenSession();
                // Get the RequestMsgSet based on the correct QB Version
                var requestMsgSet = QBHelper.GetLatestMsgSetRequest(qbSessionManager);
                // Initialize the message set request object
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                QBHelper.EnableErrorRecovery(qbSessionManager);

                var invoiceQuery = requestMsgSet.AppendInvoiceQueryRq();
                invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.ModifiedDateRangeFilter.FromModifiedDate.SetValue(DateTime.Now.AddDays(daysBack), true);
                invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.ModifiedDateRangeFilter.ToModifiedDate.SetValue(DateTime.Now.AddDays(0), true);
                invoiceQuery.IncludeLineItems.SetValue(false);
                invoiceQuery.IncludeLinkedTxns.SetValue(false);

                var responseMsgSet = qbSessionManager.DoRequests(requestMsgSet);
                var response = responseMsgSet.ResponseList.GetAt(0);
                var invoiceRetList = (IInvoiceRetList)response.Detail;

                if (invoiceRetList != null)
                {
                    var invoiceRet = invoiceRetList.GetAt(invoiceRetList.Count - 1);
                    returnVal = (int.Parse(invoiceRet.RefNumber.GetValue()) + 1).ToString();
                }
            }
            return returnVal;
        }

        public static bool DeleteInvoiceAndUpdatePackages(string refNum)
        {
            var result = false;
            if (!DeleteInvoiceByNumber(refNum)) return false;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                try
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    var cmd = new SqlCommand("UpdatePackageTrackInvoiceDeleted", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.Add(new SqlParameter("@invoiceNum", refNum));
                    result = cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + NewLine + @"Stack trace: " + NewLine + ex.StackTrace);
                }
            }
            return result;
        }

        private static void UpdatePackagesInvoiceCreated(string refNum, string pkgIds)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand($@"
                    update PackageTrack
                    set invoice = '{refNum}'
                    where Id in ('{pkgIds.Replace(",", "','")}')
                    ", conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool DeleteInvoiceByNumber(string refNum)
        {
            var returnVal = false;
            var trxId = string.Empty;

            using (var sessionManager = SessionManager.GetInstance)
            {
                var qbSessionManager = sessionManager.OpenSession();
                // Get the RequestMsgSet based on the correct QB Version
                var requestMsgSet = QBHelper.GetLatestMsgSetRequest(qbSessionManager);
                // Initialize the message set request object
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                QBHelper.EnableErrorRecovery(qbSessionManager);

                var invoiceQuery = requestMsgSet.AppendInvoiceQueryRq();
                invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcContains);
                invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(refNum);
                invoiceQuery.IncludeLineItems.SetValue(false);
                invoiceQuery.IncludeLinkedTxns.SetValue(false);
                
                var responseMsgSet = qbSessionManager.DoRequests(requestMsgSet);
                var response = responseMsgSet.ResponseList.GetAt(0);
                var invoiceRetList = (IInvoiceRetList)response.Detail;

                if (invoiceRetList != null)
                {
                    var invoiceRet = invoiceRetList.GetAt(0);
                    trxId = invoiceRet.TxnID.GetValue();
                }

                if (trxId != null)
                {
                    var invoiceDel = requestMsgSet.AppendTxnDelRq();
                    invoiceDel.TxnDelType.SetValue(ENTxnDelType.tdtInvoice);
                    invoiceDel.TxnID.SetValue(trxId);

                    QBHelper.EnableErrorRecovery(qbSessionManager);
                    requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                    responseMsgSet = qbSessionManager.DoRequests(requestMsgSet);
                    response = responseMsgSet.ResponseList.GetAt(0);
                }


                if (response.StatusCode != 0)
                {
                    MessageBox.Show(response.StatusMessage);
                }
                else
                {
                    MessageBox.Show(@"Invoice #" + refNum + @" was succesfully deleted.");
                    returnVal = true;
                }
            }
            return returnVal;
        }

        private void btnDeleteInvoice_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvInvoices.SelectedItems)
            {
                var invoice = item.Tag as Invoice;
                DeleteInvoiceAndUpdatePackages(invoice?.InvoiceNumber);
                FilllvInvoices();
                FilllvInventories();
            }
        }

        private void lvInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeleteInvoice.Enabled = lvInvoices.SelectedItems.Count > 0;
        }
    }

}
