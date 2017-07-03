using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using QBFC13Lib;
using System.Linq;
using System.Threading;

namespace SEQB
{
    public partial class MainForm : Form
    {
        private int _totalQty;
        private double _totalTax;
        private double _totalAmount;
        private string _pkgIdsForUpdate;
        private readonly BackgroundWorker _asyncWorker = new BackgroundWorker();
        private static readonly string NewLine = Environment.NewLine;
        private Plant _plant;
        private string _familyGroups;

        private DataTable _dt;
        private DataTable _dtPoS;

        public MainForm()
        {
            InitializeComponent();
            QBHelper.MsgEvent += QBHelper_MsgEvenHandler;

            MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            _asyncWorker.WorkerReportsProgress = true;
            _asyncWorker.ProgressChanged += bwAsync_ProgressChanged;
            _asyncWorker.RunWorkerCompleted += bwAsync_RunWorkerCompleted;
            _asyncWorker.DoWork += bwAsync_DoWork;

            fillCbPlant();

            fillCbFamilyGroup();
        }

        private void bwAsync_DoWork(object sender, DoWorkEventArgs e)
        {

            var bwAsync = sender as BackgroundWorker;
            var iCount = new Random().Next(20, 50);

            fillLvInventories();

            bwAsync?.ReportProgress(0);

            for (var i = 1; i <= iCount; i++)
            {
                Thread.Sleep(100);
                var currValue = Convert.ToInt32(i * (100.0 / iCount));
                bwAsync?.ReportProgress(currValue < 100 ? currValue : 100);
                Debug.Print("Current: " + currValue);
            }

            Thread.Sleep(100);
            bwAsync?.ReportProgress(100);
            Application.DoEvents();
        }

        private void bwAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            updateInventoryView();

            lblQty.Text = _totalQty.ToString(CultureInfo.GetCultureInfo("en-US"));
            lblTax.Text = _totalTax.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
            lblAmount.Text = _totalAmount.ToString("C2", CultureInfo.GetCultureInfo("en-US"));

            reSizeLastColumn(lvInventories);

            btnCreateInvoice.Enabled = lvInventories.Items.Count > 0;
            btnViewInventories.Enabled = true;
            progressBar1.Visible = false;
        }

        private void bwAsync_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            Debug.Print("Reported: " + e.ProgressPercentage);
        }

        private void fillCbPlant()
        {
            var plants = new List<Plant>
            {
                new Plant {Name = "PA", Id = 1},
                new Plant {Name = "MO", Id = 2},
                new Plant {Name = "MA", Id = 3}
            };
            cbPlant.DataSource = plants;
            cbPlant.ValueMember = "Id";
            cbPlant.DisplayMember = "Name";
        }

        private void fillCbFamilyGroup()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                var cmd = new SqlCommand(@"
                    select FamilyGroup from InventoryItem
                    where FamilyGroup <> ''
                    group by FamilyGroup", conn);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                dt.AsEnumerable().Select(x => x[0]).ToList().ForEach(x => cbcbFamilyGroup.Items.Add(x));
            }
        }

        private string getFamilyGroupsFromControl()
        {
            return cbcbFamilyGroup.Text.Replace(", ", ",");
        }

        private void fillLvInventories()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (var cmd = new SqlCommand("FamilyGroupItemsForQBInvoice", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 900
                })
                {
                    if (_plant != null)
                        cmd.Parameters.Add(new SqlParameter("@plant_Id", _plant.Id));
                    cmd.Parameters.Add(new SqlParameter("@FG", _familyGroups));
                    cmd.Parameters.Add(new SqlParameter("@dateFrom", dtDateFrom.Value));
                    cmd.Parameters.Add(new SqlParameter("@dateTo", dtDateTo.Value));
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

                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    _totalQty = int.Parse(cmd.Parameters["@TQty"].Value.ToString(),
                        CultureInfo.GetCultureInfo("en-US"));
                    _totalTax = double.Parse(cmd.Parameters["@TTax"].Value.ToString(),
                        CultureInfo.GetCultureInfo("en-US"));
                    _totalAmount = double.Parse(cmd.Parameters["@TAmount"].Value.ToString(),
                        CultureInfo.GetCultureInfo("en-US"));
                    _pkgIdsForUpdate = cmd.Parameters["@TIds"].Value.ToString();
                }
            }
        }

        private void updateInventoryView()
        {
            foreach (DataRow row in _dt.Rows)
            {
                var item = new ListViewItem(row[0].ToString());
                // skip 2 last columns (auxilary: Amount, TaxRef)
                for (var i = 1; i < _dt.Columns.Count - 2; i++)
                {
                    if (i == 3) continue; // skip 1 column (auxilary: inventoryid)
                    item.SubItems.Add(row[i].ToString());
                }
                lvInventories.Items.Add(item);
            }
        }

        private void clearInventories()
        {
            lvInventories.Items.Clear();
            lblQty.Text = @"0";
            lblTax.Text = @"$0.00";
            lblAmount.Text = @"$0.00";
            btnViewInventories.Enabled = true;
            btnCreateInvoice.Enabled = false;
        }

        private IEnumerable<Invoice> getAllCreatedInvoices()
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
                            ret.Add(new Invoice {InvoiceNumber = reader.GetValue(0).ToString()});
                        }
                    }
                }
            }
            return ret;
        }

        private IEnumerable<Invoice> getFilteredInvoicesFromQb(List<Invoice> list)
        {
            var unPaidInvoices = new List<Invoice>();
            try
            {
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
                    var invoiceRetList = (IInvoiceRetList) response.Detail;

                    for (var i = 0; i < invoiceRetList.Count; i++)
                    {
                        var invoiceRet = invoiceRetList.GetAt(i);
                        var item = new Invoice
                        {
                            InvoiceNumber = invoiceRet.RefNumber.GetValue(),
                            CreateDate = invoiceRet.TimeCreated.GetValue().ToString("d")
                        };
                        unPaidInvoices.Add(item);
                        //list.RemoveAll(x => x.InvoiceNumber == invoiceRet.RefNumber.GetValue());
                        //returnVal = (int.Parse(invoiceRet.RefNumber.GetValue()) + 1).ToString();
                    }
                }
            }
            catch
            {
                //
            }
            foreach (var invoice in list)
            foreach (var unPaidInvoice in unPaidInvoices)
                if (invoice.InvoiceNumber == unPaidInvoice.InvoiceNumber)
                    invoice.CreateDate = unPaidInvoice.CreateDate;
            return list.Where(x => unPaidInvoices.Select(i => i.InvoiceNumber).Contains(x.InvoiceNumber));
        }

        private void fillLvInvoices()
        {
            lvInvoices.Items.Clear();
            lvInvoices.View = View.Details;

            var invoices = getAllCreatedInvoices();
            invoices = getFilteredInvoicesFromQb(invoices.ToList());

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                conn.Open();

                foreach (var invoice in invoices)
                {
                    var cmd = new SqlCommand("SummaryForQBInvoiceDelete", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.Add(new SqlParameter("@InvoiceNum", invoice.InvoiceNumber));
                    cmd.Parameters.Add(new SqlParameter("@ShipDate", SqlDbType.VarChar, 24));
                    cmd.Parameters.Add(new SqlParameter("@TQty", DbType.Int16));
                    cmd.Parameters.Add(new SqlParameter("@TTax", SqlDbType.VarChar, 16));
                    cmd.Parameters.Add(new SqlParameter("@TAmount", SqlDbType.VarChar, 16));
                    cmd.Parameters.Add(new SqlParameter("@FG", SqlDbType.NVarChar, 255));
                    cmd.Parameters["@ShipDate"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@TQty"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@TTax"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@TAmount"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@FG"].Direction = ParameterDirection.Output;

                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();

                    invoice.ShipDate = cmd.Parameters["@ShipDate"].Value.ToString();
                    invoice.CreateDate = invoice.CreateDate;
                    invoice.Qty = int.Parse(cmd.Parameters["@TQty"].Value.ToString(),
                        CultureInfo.GetCultureInfo("en-US"));
                    invoice.Tax = double.Parse(cmd.Parameters["@TTax"].Value.ToString(),
                        CultureInfo.GetCultureInfo("en-US"));
                    invoice.Amount = double.Parse(cmd.Parameters["@TAmount"].Value.ToString(),
                        CultureInfo.GetCultureInfo("en-US"));
                    invoice.FamilyGroup = cmd.Parameters["@FG"].Value.ToString();

                    var item = new ListViewItem(invoice.InvoiceNumber) {Tag = invoice};
                    item.SubItems.Add(invoice.CreateDate.ToString(CultureInfo.GetCultureInfo("en-US")));
                    item.SubItems.Add(invoice.ShipDate.ToString(CultureInfo.GetCultureInfo("en-US")));
                    item.SubItems.Add(invoice.FamilyGroup);
                    item.SubItems.Add(invoice.Qty.ToString(CultureInfo.GetCultureInfo("en-US")));
                    item.SubItems.Add(invoice.Tax.ToString("C2", CultureInfo.GetCultureInfo("en-US")));
                    item.SubItems.Add(invoice.Amount.ToString("C2", CultureInfo.GetCultureInfo("en-US")));
                    lvInvoices.Items.Add(item);
                }
            }

            reSizeLastColumn(lvInvoices);

            btnDeleteInvoice.Enabled = lvInvoices.SelectedItems.Count > 0;
            btnGenerateProofOfShipment.Enabled = lvInvoices.SelectedItems.Count > 0;
        }

        private void dtDateFrom_ValueChanged(object sender, EventArgs e)
        {
            adjustUpperDateBoundary();
        }

        private void dtDateTo_ValueChanged(object sender, EventArgs e)
        {
            adjustLowerDateBoundary();
        }

        private void adjustUpperDateBoundary()
        {
            if (dtDateFrom.Value.Date > dtDateTo.Value.Date)
                dtDateTo.Value = dtDateFrom.Value;
        }

        private void adjustLowerDateBoundary()
        {
            if (dtDateTo.Value.Date < dtDateFrom.Value.Date)
                dtDateTo.Value = dtDateFrom.Value;
        }

        private void cbPlant_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearInventories();
        }

        private void btnViewInventories_Click(object sender, EventArgs e)
        {
            _plant = cbPlant.SelectedItem as Plant;
            _familyGroups = getFamilyGroupsFromControl();

            clearInventories();

            _asyncWorker.RunWorkerAsync();

            btnViewInventories.Enabled = false;
            lvInventories.View = View.Details;
            progressBar1.Visible = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lvInventories.View = View.Details;
            lvInventories.Columns.AddRange(new[]
                {LineNum, Id, PartNumber, FamilyGroup, Description, Qty, UnitPrice, Tax, Amount, ItemShipDate, Dummy});
            reSizeLastColumn(lvInventories);

            lvInvoices.View = View.Details;
            lvInvoices.Columns.AddRange(new[]
            {
                InvoiceNumber, InvoiceCreateDate, InvoiceShipDate, InvoiceFamilyGroup, InvoiceQty, InvoiceTax,
                InvoiceAmount, Dummy2
            });
            reSizeLastColumn(lvInvoices);
            fillLvInvoices();
        }

        private void reSizeLastColumn(ListView lv)
        {
            if (lv.Columns.Count > 0)
                lv.Columns[lv.Columns.Count - 1].Width = -2;
        }

        private void btnCreateInvoice_Click(object sender, EventArgs e)
        {
            var disabledButton = (Button) sender;
            disabledButton.Enabled = false;

            QBFC_AddInvoice();
            fillLvInventories();
            fillLvInvoices();
        }

        private void QBFC_AddInvoice()
        {
            var custFn = cbcbFamilyGroup.Text.Contains("TRUSLATE")
                ? "GAF MC TRUSLATE"
                : cbcbFamilyGroup.Text.Contains("LOWES")
                    ? "GAF NATIONAL ACCTS"
                    : "GAF MC Steep Slope";

            var billTo = getBillTo(custFn);

            var invoiceNumber = getNextInvoiceNumber();

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

                    // Setup invoice for sedning over email
                    invoiceAdd.IsToBeEmailed.SetValue(true);

                    // Set the IInvoiceAdd field values
                    // Customer:Job
                    invoiceAdd.CustomerRef.FullName.SetValue(custFn);

                    // Invoice Date
                    invoiceAdd.TxnDate.SetValue(DateTime.Now);

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
                    var poNumber = getPoNumber("");
                    invoiceAdd.PONumber.SetValue(poNumber);

                    // F.O.B.
                    string fob;
                    switch (cbPlant.Text)
                    {
                        case "PA":
                            fob = "SEI of PA";
                            break;
                        case "MO":
                            fob = "SEI of MO";
                            break;
                        case "MA":
                            fob = "SEI of MA";
                            break;
                        default:
                            fob = "SEI of PA";
                            break;
                    }
                    invoiceAdd.FOB.SetValue(fob);

                    // VIA
                    /* 
                    * TODO: based on wheather it is UPS Ground, International, GAF or Fright LTL, select proper shipment type
                    * TODO: however, it might just stay UPS for all types (Bob Glass to make final decision)
                    */
                    invoiceAdd.ShipMethodRef.FullName.SetValue("UPS");

                    // Customer Message
                    invoiceAdd.CustomerMsgRef.FullName.SetValue("Thank you for your business.");

                    // Due Date
                    invoiceAdd.DueDate.SetValue(DateTime.Now.AddDays(30));

                    // Set the values for the invoice line
                    for (var i = 0; i < _dt.Rows.Count; i++)
                    {
                        var row = _dt.Rows[i];
                        // Create the line item for the invoice
                        var invoiceLineAdd = invoiceAdd.ORInvoiceLineAddList.Append().InvoiceLineAdd;
                        invoiceLineAdd.ItemRef.FullName.SetValue(row["Description"].ToString());
                        invoiceLineAdd.Quantity.SetValue(
                            Convert.ToDouble(row["Qty"].ToString(), CultureInfo.GetCultureInfo("en-US")));
                        invoiceLineAdd.SalesTaxCodeRef.FullName.SetValue(row["TaxRef"]
                            .ToString()
                            .Equals("Non-Taxable Sales")
                            ? "Non"
                            : "Tax");
                        invoiceLineAdd.ServiceDate.SetValue(Convert.ToDateTime(row["ShipDate"]));
                    } // for

                    // Entire invoice sales tax rate, can only be for a single plant (state: PA, MO, MA)
                    invoiceAdd.ItemSalesTaxRef.FullName.SetValue(cbPlant.Text + " Sales Tax");
                    invoiceAdd.ClassRef.FullName.SetValue(cbPlant.Text);

                    if (qbSessionManager.IsErrorRecoveryInfo())
                        qbSessionManager.ClearErrorRecovery();

                    if (QBHelper.ShowRequestResult(qbSessionManager, requestMsgSet))
                        updatePackagesForInvoiceCreated(invoiceNumber, _pkgIdsForUpdate);

                    qbSessionManager.ClearErrorRecovery();
                }
                catch (Exception ex)
                {
                    QBHelper.RaiseEvent(ex.Message + NewLine + @"Stack Trace: " + NewLine + ex.StackTrace + NewLine +
                                        @"Exiting the application");
                }
                clearInventories();
            }
        }

        private string getPoNumber(string familyGroup)
        {
            var fGroup = string.IsNullOrEmpty(familyGroup) ? getFamilyGroupsFromControl() : familyGroup;
            string poNumber;
            switch (fGroup)
            {
                case "BOARDS":
                    poNumber = "40113929";
                    break;
                case "EASY PRO":
                    poNumber = "40113928";
                    break;
                case "FOLDERS":
                    poNumber = "40113927";
                    break;
                case "LOWES":
                    poNumber = "40113997";
                    break;
                case "POCKET PRO":
                    poNumber = "40113930";
                    break;
                case "TRUSLATE":
                    poNumber = "40113847";
                    break;
                //case "LIFETIME":
                //case "STORYBOOK":
                default:
                    poNumber = "GAFSAMPLES";
                    break;
            }
            return poNumber;
        }
// method QBFC_AddInvoice

        private BillAddress getBillTo(string custFn)
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
                    // QBHelper.RaiseEvent(requestXML);
                    // SaveXML(requestXML);
                    // string responseXML = responseSet.ToXMLString();
                    // QBHelper.RaiseEvent(responseXML);
                    // SaveXML(responseXML);

                    var response = responseSet.ResponseList.GetAt(0);
                    // int statusCode = response.StatusCode;
                    // string statusMessage = response.StatusMessage;
                    // string statusSeverity = response.StatusSeverity;
                    // QBHelper.RaiseEvent("Status:" + NewLine + "Code = " + statusCode + NewLine + "Message = " + statusMessage + NewLine + "Severity = " + statusSeverity);


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
                    QBHelper.RaiseEvent(ex.Message + NewLine + @"\nStack Trace:" + NewLine + ex.StackTrace + NewLine +
                                        @"\nExiting the application");
                }
            }
            return ret;
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            reSizeLastColumn(lvInventories);
            reSizeLastColumn(lvInvoices);
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            reSizeLastColumn(lvInventories);
            reSizeLastColumn(lvInvoices);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            reSizeLastColumn(lvInventories);
            reSizeLastColumn(lvInvoices);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            QBHelper.MsgEvent -= QBHelper_MsgEvenHandler;
        }

        private void QBHelper_MsgEvenHandler(object sender, QBMsgEventArgs e)
        {
            MessageBox.Show(e.Msg);
        }

        private string getNextInvoiceNumber()
        {
            var returnVal = string.Empty;
            try
            {
                const int DAYS_BACK = -120;

                using (var sessionManager = SessionManager.GetInstance)
                {
                    var qbSessionManager = sessionManager.OpenSession();
                    // Get the RequestMsgSet based on the correct QB Version
                    var requestMsgSet = QBHelper.GetLatestMsgSetRequest(qbSessionManager);
                    // Initialize the message set request object
                    requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                    QBHelper.EnableErrorRecovery(qbSessionManager);

                    var invoiceQuery = requestMsgSet.AppendInvoiceQueryRq();
                    invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.ModifiedDateRangeFilter.FromModifiedDate
                        .SetValue(DateTime.Now.AddDays(DAYS_BACK), true);
                    invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.ModifiedDateRangeFilter.ToModifiedDate
                        .SetValue(DateTime.Now.AddDays(1), true);
                    invoiceQuery.IncludeLineItems.SetValue(false);
                    invoiceQuery.IncludeLinkedTxns.SetValue(false);

                    var responseMsgSet = qbSessionManager.DoRequests(requestMsgSet);
                    var response = responseMsgSet.ResponseList.GetAt(0);
                    var invoiceRetList = (IInvoiceRetList) response.Detail;

                    if (invoiceRetList == null) return returnVal;
                    var sortedList = new List<string>();
                    for (var i = 0; i < invoiceRetList.Count; i++)
                    {
                        sortedList.Add(invoiceRetList.GetAt(i).RefNumber.GetValue());
                    }
                    sortedList.Sort();
                    returnVal = (int.Parse(sortedList[sortedList.Count - 1]) + 1).ToString();
                }
            }
            catch
            {
                //
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
                    QBHelper.RaiseEvent(ex.Message + NewLine + @"Stack trace: " + NewLine + ex.StackTrace);
                }
            }
            return result;
        }

        private static void updatePackagesForInvoiceCreated(string refNum, string pkgIds)
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
                invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(
                    ENMatchCriterion.mcContains);
                invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(refNum);
                invoiceQuery.IncludeLineItems.SetValue(false);
                invoiceQuery.IncludeLinkedTxns.SetValue(false);

                var responseMsgSet = qbSessionManager.DoRequests(requestMsgSet);
                var response = responseMsgSet.ResponseList.GetAt(0);
                var invoiceRetList = (IInvoiceRetList) response.Detail;

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
                    QBHelper.RaiseEvent(@"Invoice #" + refNum + @" was succesfully deleted.");
                    returnVal = true;
                }
            }
            return returnVal;
        }

        private void btnDeleteInvoice_Click(object sender, EventArgs e)
        {
            var disabledButton = (Button) sender;
            disabledButton.Enabled = false;

            foreach (ListViewItem item in lvInvoices.SelectedItems)
            {
                var invoice = item.Tag as Invoice;
                DeleteInvoiceAndUpdatePackages(invoice?.InvoiceNumber);
                fillLvInvoices();
                //fillLvInventories();
                clearInventories();
            }
        }

        private void lvInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeleteInvoice.Enabled = lvInvoices.SelectedItems.Count > 0;
            btnGenerateProofOfShipment.Enabled = lvInvoices.SelectedItems.Count > 0;
        }

        private List<ProofOfShipment> getProofOfShipmentsForInvoice(string invoice)
        {
            var result = new List<ProofOfShipment>();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (var cmd = new SqlCommand("GetProofOfShipment", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 900
                })
                {
                    if (!string.IsNullOrEmpty(invoice))
                    {
                        cmd.Parameters.Add(new SqlParameter("@invoice", invoice));
                        _dtPoS = new DataTable();
                        new SqlDataAdapter(cmd).Fill(_dtPoS);

                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                if (_dtPoS?.Rows == null) return result;

                foreach (DataRow row in _dtPoS.Rows)
                {
                    result.Add(new ProofOfShipment
                        {
                            ShipDate = row[0].ToString(),
                            PartNumber = row[1].ToString(),
                            TrackingNumber = row[2].ToString(),
                            Freight = row[3].ToString(),
                            PackageReference = row[4].ToString(),
                            Service = row[5].ToString()
                        });
                }
            }
            return result;
        }

        private void btnGenerateProofOfShipment_Click(object sender, EventArgs e)
        {
            var disabledButton = (Button)sender;
            disabledButton.Enabled = false;

            foreach (ListViewItem item in lvInvoices.SelectedItems)
            {
                var invoice = item?.Tag as Invoice;
                var shippedPackages = getProofOfShipmentsForInvoice(invoice?.InvoiceNumber);
                using (var sfDlg = new SaveFileDialog())
                {
                    sfDlg.Title = @"Save proof of shipment as...";
                    sfDlg.DefaultExt = ".csv";
                    sfDlg.Filter = @"Windows comma separated values file (*.csv)|*.csv";

                    var ndr = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    sfDlg.InitialDirectory = ndr.ToString();
                    sfDlg.FileName = "Invoice #" + invoice?.InvoiceNumber + ".csv";
                    var sfDlgResult = sfDlg.ShowDialog();

                    if (sfDlgResult != DialogResult.OK) continue;

                    const string DELIMTER = ",";

                    using (TextWriter writer = File.CreateText(sfDlg.FileName))
                    {
                        writer.WriteLine("Invoice #" + invoice?.InvoiceNumber + ",," + "PO Number: " + getPoNumber(invoice?.FamilyGroup));
                        writer.WriteLine();
                        foreach (var member in shippedPackages.AsEnumerable())
                        {
                            writer.WriteLine(string.Join(DELIMTER, member.GetAllFields()));
                        }
                    }

                    Process.Start("explorer.exe", "/select, " + sfDlg.FileName);
                    Process.Start("explorer.exe", "/open, " + sfDlg.FileName);
                }
            }

            disabledButton.Enabled = true;
        }
    }
}
