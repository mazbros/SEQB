using System;
using System.Configuration;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Globalization;
using QBFC13Lib;

namespace SEQB
{
    public partial class MainForm : Form
    {
        //private readonly string _appToken = ConfigurationManager.AppSettings["appToken"];
        //private readonly string _accessTokenSecret = ConfigurationManager.AppSettings["appToken"];
        //private readonly string _consumerKey = ConfigurationManager.AppSettings["OAuthConsumerKey"];
        //private readonly string _consumerSecret = ConfigurationManager.AppSettings["OAuthConsumerSecret"];

        //private List<string> familyGroups = new List<string>() { "g1", "g2", "g3" };

        private int _totalQty;
        private double _totalTax;
        private double _totalAmount;

        DataTable dt;

        public MainForm()
        {
            InitializeComponent();

            MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            FillcbFamilyGroup();

            try
            {
                #region Commented
                //var oauthValidator = new OAuthRequestValidator(_appToken,_accessTokenSecret,_consumerKey,_consumerSecret);
                //var context = new ServiceContext(_appToken, IntuitServicesType.QBD, oauthValidator);
                //var conn = new DataService(context);

                //var invoiceItemLine = new Line
                //{
                //    Id = "1",
                //    Description = "Description",
                //    Amount = 10m,
                //    AmountSpecified = true,
                //    DetailType = LineDetailTypeEnum.ItemReceiptLineDetail,
                //    DetailTypeSpecified = true,
                //    AnyIntuitObject = 456.78m,
                //    LineNum = "1",

                //};

                //var lineDetail = new SalesItemLineDetail
                //{
                //    ServiceDate = DateTime.Now,
                //    AnyIntuitObject = 10m,
                //    ItemElementName = ItemChoiceType.UnitPrice,
                //    Qty = 10,
                //    QtySpecified = true,
                //    TaxCodeRef = new ReferenceType()
                //    {
                //        Value = "TAX"
                //    },
                //    TaxInclusiveAmt = 1.25m,
                //    TaxInclusiveAmtSpecified = true
                //};

                //invoiceItemLine.AnyIntuitObject = lineDetail;

                //var invoice = new Invoice
                //{
                //    Id = "1",
                //    Deposit = 0,
                //    DueDate = DateTime.Now.AddMonths(1),
                //    domain = idDomainEnum.QBSDK.ToString(),
                //    sparse = false,
                //    SyncToken = "0",
                //    MetaData = new ModificationMetaData
                //    {
                //        CreateTime = DateTime.Now,
                //        LastUpdatedTime = DateTime.Now
                //    },
                //    BillAddr = new PhysicalAddress
                //    {
                //        Id = "1",
                //        Line1 = "Russ Sonnenschein",
                //        Line2 = "Sonnenschein Family Store",
                //        Line3 = "5647 Cypress Hill Ave.",
                //        Line4 = "Middlefield, CA  94303"
                //    },
                //    ShipAddr = new PhysicalAddress
                //    {
                //        Id = "1",
                //        Line1 = "5647 Cypress Hill Ave.",
                //        City = "Middlefield",
                //        CountrySubDivisionCode = "CA",
                //        PostalCode = "94303"
                //    },
                //    SalesTermRef = new ReferenceType
                //    {
                //        Value = "3"
                //    },
                //    TotalAmt = 362.07M,
                //    ApplyTaxAfterDiscount = false,
                //    PrintStatus = PrintStatusEnum.NotSet,
                //    EmailStatus = EmailStatusEnum.NotSet,
                //    BillEmail = new EmailAddress
                //    {
                //        Address = "amazure@wdsystems.com"
                //    },
                //    Balance = 362.07M,
                //    Line = new[] {invoiceItemLine}
                //};


                //conn.Add(invoice); 
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                cmd.Parameters.Add(new SqlParameter("@FG", cbFamilyGroup.Text));
                cmd.Parameters.Add(new SqlParameter("@date", dtDate.Value));
                cmd.Parameters.Add(new SqlParameter("@TQty", DbType.Int16));
                cmd.Parameters.Add(new SqlParameter("@TTax", SqlDbType.VarChar, 16));
                cmd.Parameters.Add(new SqlParameter("@TAmount", SqlDbType.VarChar, 16));
                cmd.Parameters["@TQty"].Direction = ParameterDirection.Output;
                cmd.Parameters["@TTax"].Direction = ParameterDirection.Output;
                cmd.Parameters["@TAmount"].Direction = ParameterDirection.Output;
                dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    var item = new ListViewItem(row[0].ToString());
                    for (var i = 1; i < dt.Columns.Count; i++)
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

                _totalQty = int.Parse(cmd.Parameters["@TQty"].Value.ToString(), CultureInfo.InvariantCulture);
                _totalTax = double.Parse(cmd.Parameters["@TTax"].Value.ToString(), CultureInfo.InvariantCulture);
                _totalAmount = double.Parse(cmd.Parameters["@TAmount"].Value.ToString(), CultureInfo.InvariantCulture);

                lblQty.Text = _totalQty.ToString();
                lblTax.Text = _totalTax.ToString("C2", CultureInfo.CurrentCulture);
                lblAmount.Text = _totalAmount.ToString("C2", CultureInfo.CurrentCulture);
            }

            SizeLastColumn(lvInventories);

            btnCreateInvoice.Enabled = lvInventories.Items.Count > 0;
        }

        private void btnViewInventories_Click(object sender, EventArgs e)
        {
            lvInventories.Items.Clear();
            FilllvInventories();
        }

        private void cbFamilyGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvInventories.Items.Clear();
            FilllvInventories();
        }

        private void dtDate_ValueChanged(object sender, EventArgs e)
        {
            lvInventories.Items.Clear();
            FilllvInventories();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lvInventories.View = View.Details;
            lvInventories.Columns.AddRange(new[] { LineNum, Id, PartNumber, FamilyGroup, Description, Qty, UnitPrice, Tax, Amount, Dummy });
            SizeLastColumn(lvInventories);
        }
        private void SizeLastColumn(ListView lv)
        {
            if(lv.Columns.Count > 0)
                lv.Columns[lv.Columns.Count - 1].Width = -2;
        }
        private void btnCreateInvoice_Click(object sender, EventArgs e)
        {
            QBFC_AddInvoice();
        }

        public void QBFC_AddInvoice()
        {
            IMsgSetRequest requestMsgSet;
            // Create the session manager object using QBFC
            QBSessionManager sessionManager = new QBSessionManager();

            // We want to know if we begun a session so we can end it if an
            // error happens
            bool booSessionBegun = false;

            try
            {
                // Use SessionManager object to open a connection and begin a session 
                // with QuickBooks. At this time, you should add interop.QBFCxLib into 
                // your Project References
                sessionManager.OpenConnection("", "Sample Express");
                sessionManager.BeginSession("", ENOpenMode.omDontCare);
                booSessionBegun = true;

                // Get the RequestMsgSet based on the correct QB Version
                requestMsgSet = getLatestMsgSetRequest(sessionManager);
                // requestMsgSet = sessionManager.CreateMsgSetRequest("US", 4, 0);

                // Initialize the message set request object
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                // ERROR RECOVERY: 
                // All steps are described in QBFC Developers Guide, on pg 41
                // under section titled "Automated Error Recovery"

                // (1) Set the error recovery ID using ErrorRecoveryID function
                //		Value must be in GUID format
                //	You could use c:\Program Files\Microsoft Visual Studio\Common\Tools\GuidGen.exe 
                //	to create a GUID for your unique ID
                string errecid = "{E74068B5-0D6D-454d-B0FD-BDDF93CE67C3}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                // (2) Set EnableErrorRecovery to true to enable error recovery
                sessionManager.EnableErrorRecovery = true;

                // (3) Set SaveAllMsgSetRequestInfo to true so the entire contents of the MsgSetRequest
                //		will be saved to disk. If SaveAllMsgSetRequestInfo is false (default), only the 
                //		newMessageSetID will be saved. 
                sessionManager.SaveAllMsgSetRequestInfo = true;

                // (4) Use IsErrorRecoveryInfo to check whether an unprocessed response exists. 
                //		If IsErrorRecoveryInfo is true:
                if (sessionManager.IsErrorRecoveryInfo())
                {
                    ProcessQBFCErrors(sessionManager);
                }


                // Add the request to the message set request object
                IInvoiceAdd invoiceAdd = requestMsgSet.AppendInvoiceAddRq();


                // Set the IInvoiceAdd field values
                // Customer:Job
                string customer = "GAF MC Access & Low Slope";//"cmboBx3_CustomerJob.Text";
                if (!customer.Equals(""))
                {
                    invoiceAdd.CustomerRef.FullName.SetValue(customer);
                }
                // Invoice Date
                invoiceAdd.TxnDate.SetValue(dtDate.Value);
                // Invoice Number
                string invoiceNumber = "111";
                if (!invoiceNumber.Equals(""))
                {
                    invoiceAdd.RefNumber.SetValue(invoiceNumber);
                }
                // Bill Address
                string bAddr1 = "txt2_BillTo_Addr1.Text";
                string bAddr2 = "txt3_BillTo_Addr2.Text";
                string bAddr3 = "txt4_BillTo_Addr3.Text";
                string bAddr4 = "txt5_BillTo_Addr4.Text";
                string bCity = "Joplin";
                string bState = "MO";
                string bPostal = "64802-1404";
                string bCountry = "USA";
                invoiceAdd.BillAddress.Addr1.SetValue(bAddr1);
                invoiceAdd.BillAddress.Addr2.SetValue(bAddr2);
                invoiceAdd.BillAddress.Addr3.SetValue(bAddr3);
                invoiceAdd.BillAddress.Addr4.SetValue(bAddr4);
                invoiceAdd.BillAddress.City.SetValue(bCity);
                invoiceAdd.BillAddress.State.SetValue(bState);
                invoiceAdd.BillAddress.PostalCode.SetValue(bPostal);
                invoiceAdd.BillAddress.Country.SetValue(bCountry);
                // P.O. Number
                invoiceAdd.PONumber.SetValue("111");
                // Terms
                string terms = "cmboBx1_Terms.Text";
                if (terms.IndexOf("Please select one from list") >= 0)
                {
                    terms = "";
                }
                if (!terms.Equals(""))
                {
                    //invoiceAdd.TermsRef.FullName.SetValue(terms);
                }
                // Due Date
                invoiceAdd.DueDate.SetValue(DateTime.Now);

                // Customer Message
                //string customerMsg = "cmboBx4_CustomerMessage.Text";
                //if (!customerMsg.Equals(""))
                //{
                //    invoiceAdd.CustomerMsgRef.FullName.SetValue(customerMsg);
                //}

                // Set the values for the invoice line
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    // Create the line item for the invoice
                    //string item = "TAMKO FG:TAMKO FREDERICK FG:41000106";// listView1_InvoiceItems.Items[i].SubItems[0].Text; // txt12_Item.Text;
                    //string desc = "desc";// listView1_InvoiceItems.Items[i].SubItems[1].Text; // txt13_Desc.Text;
                    //string rate = "1,0";// listView1_InvoiceItems.Items[i].SubItems[2].Text; // txt14_Rate.Text;
                    //string qty = "1";// listView1_InvoiceItems.Items[i].SubItems[3].Text; // txt15_Qty.Text;
                    //string amount = "1";// listView1_InvoiceItems.Items[i].SubItems[4].Text; // txt16_Amount.Text;
                    //string taxable = "taxable";// listView1_InvoiceItems.Items[i].SubItems[5].Text; // txt17_Tax.Text;

                    //if (!item.Equals("") || !desc.Equals(""))
                    //{
                        IInvoiceLineAdd invoiceLineAdd = invoiceAdd.ORInvoiceLineAddList.Append().InvoiceLineAdd;

                        invoiceLineAdd.ItemRef.FullName.SetValue(row["Description"].ToString());
                        //invoiceLineAdd.ItemRef.FullName.SetValue(row["Description"].ToString());
                        //invoiceLineAdd.Desc.SetValue(desc);
                        //invoiceLineAdd.ORRatePriceLevel.Rate.SetValue(Convert.ToDouble(rate));
                        
                        //invoiceLineAdd.Quantity.SetValue(Convert.ToDouble(row["Qty"].ToString(), CultureInfo.InvariantCulture));
                        invoiceLineAdd.Quantity.SetAsString(row["Qty"].ToString());

                        invoiceLineAdd.ItemRef.ListID.SetValue(row["PartNumber"].ToString());

                        //invoiceLineAdd.TaxAmount.SetAsString(row["Tax"].ToString());
                        
                        //invoiceLineAdd.Amount.SetValue(Convert.ToDouble(row["Amount"].ToString(), CultureInfo.InvariantCulture));
                        invoiceLineAdd.Amount.SetAsString(row["Amount"].ToString());
                        // Currently IsTaxable is not supported in QBD - QuickBooks Desktop Edition
                        /*
                        if (taxable.ToUpper().Equals("Y") || taxable.ToUpper().Equals("N"))
                        {
                            bool isTaxable = false;
                            if (taxable.ToUpper().Equals("Y")) isTaxable=true;
                                invoiceLineAdd.IsTaxable.SetValue(isTaxable);
                        }
                        */
                    //}
                } // for

                ShowRequestResult(sessionManager, requestMsgSet);

                // Close the session and connection with QuickBooks
                sessionManager.ClearErrorRecovery();
                sessionManager.EndSession();
                booSessionBegun = false;
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\nStack Trace: \n" + ex.StackTrace + "\nExiting the application");
                if (booSessionBegun)
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }

            }

        } // method QBFC_AddInvoice


        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            SizeLastColumn(lvInventories);
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            SizeLastColumn(lvInventories);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            SizeLastColumn(lvInventories);
        }

        public IMsgSetRequest getLatestMsgSetRequest(QBSessionManager sessionManager)
        {
            // Find and adapt to supported version of QuickBooks
            double supportedVersion = QBFCLatestVersion(sessionManager);

            short qbXMLMajorVer = 0;
            short qbXMLMinorVer = 0;

            if (supportedVersion >= 6.0)
            {
                qbXMLMajorVer = 6;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 5.0)
            {
                qbXMLMajorVer = 5;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 4.0)
            {
                qbXMLMajorVer = 4;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 3.0)
            {
                qbXMLMajorVer = 3;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 2.0)
            {
                qbXMLMajorVer = 2;
                qbXMLMinorVer = 0;
            }
            else if (supportedVersion >= 1.1)
            {
                qbXMLMajorVer = 1;
                qbXMLMinorVer = 1;
            }
            else
            {
                qbXMLMajorVer = 1;
                qbXMLMinorVer = 0;
                MessageBox.Show("It seems that you are running QuickBooks 2002 Release 1. We strongly recommend that you use QuickBooks' online update feature to obtain the latest fixes and enhancements");
            }

            // Create the message set request object
            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", qbXMLMajorVer, qbXMLMinorVer);
            return requestMsgSet;
        }

        // Code for handling different versions of QuickBooks
        private double QBFCLatestVersion(QBSessionManager SessionManager)
        {
            // Use oldest version to ensure that this application work with any QuickBooks (US)
            IMsgSetRequest msgset = SessionManager.CreateMsgSetRequest("US", 1, 0);
            msgset.AppendHostQueryRq();
            IMsgSetResponse QueryResponse = SessionManager.DoRequests(msgset);
            //MessageBox.Show("Host query = " + msgset.ToXMLString());
            //SaveXML(msgset.ToXMLString());


            // The response list contains only one response,
            // which corresponds to our single HostQuery request
            IResponse response = QueryResponse.ResponseList.GetAt(0);

            // Please refer to QBFC Developers Guide for details on why 
            // "as" clause was used to link this derrived class to its base class
            IHostRet HostResponse = response.Detail as IHostRet;
            IBSTRList supportedVersions = HostResponse.SupportedQBXMLVersionList as IBSTRList;

            int i;
            double vers;
            double LastVers = 0;
            string svers = null;

            for (i = 0; i <= supportedVersions.Count - 1; i++)
            {
                svers = supportedVersions.GetAt(i);
                vers = Convert.ToDouble(svers, CultureInfo.InvariantCulture);
                if (vers > LastVers)
                {
                    LastVers = vers;
                }
            }
            return LastVers;
        }

        private void ProcessQBFCErrors(QBSessionManager sessionManager)
        {
            //string reqXML;
            //string resXML;
            IMsgSetRequest reqMsgSet = null;
            IMsgSetResponse resMsgSet = null;

            // a. Get the response status, using GetErrorRecoveryStatus
            resMsgSet = sessionManager.GetErrorRecoveryStatus();
            // resXML = resMsgSet.ToXMLString();
            // MessageBox.Show(resXML);

            if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
            {
                // This case may occur when a transaction has failed after QB processed 
                // the request but client app didn't get the response and started with 
                // another company file.
                MessageBox.Show("The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
            }
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
            {
                MessageBox.Show("Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
            }
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
            {
                // Response was not successfully stored or stored properly
                MessageBox.Show("No stored response was found.");
            }
            // 9003 = Not used
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
            {
                // MessageSetID is set with a string of size > 24 char
                MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
            }
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
            {
                MessageBox.Show("Unable to store response.");
            }
            else
            {
                IResponse res = resMsgSet.ResponseList.GetAt(0);
                int sCode = res.StatusCode;
                //string sMessage = res.StatusMessage;
                //string sSeverity = res.StatusSeverity;
                //MessageBox.Show("StatusCode = " + sCode + "\n" + "StatusMessage = " + sMessage + "\n" + "StatusSeverity = " + sSeverity);

                if (sCode == 0)
                {
                    MessageBox.Show("Last request was processed and Invoice was added successfully!");
                }
                else if (sCode > 0)
                {
                    MessageBox.Show("There was a warning but last request was processed successfully!");
                }
                else
                {
                    MessageBox.Show("It seems that there was an error in processing last request");
                    // b. Get the saved request, using GetSavedMsgSetRequest
                    reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                    //reqXML = reqMsgSet.ToXMLString();
                    //MessageBox.Show(reqXML);

                    // c. Process the response, possibly using the saved request
                    resMsgSet = sessionManager.DoRequests(reqMsgSet);
                    IResponse resp = resMsgSet.ResponseList.GetAt(0);
                    int statCode = resp.StatusCode;
                    if (statCode == 0)
                    {
                        string resStr = null;
                        IInvoiceRet invRet = resp.Detail as IInvoiceRet;
                        resStr = resStr + "Following invoice has been successfully submitted to QuickBooks:\n\n\n";
                        if (invRet.TxnNumber != null)
                            resStr = resStr + "Txn Number = " + Convert.ToString(invRet.TxnNumber.GetValue()) + "\n";
                    } // if (statusCode == 0)
                } // else (sCode)
            } // else (MessageSetStatusCode)

            // d. Clear the response status, using ClearErrorRecovery
            sessionManager.ClearErrorRecovery();
            MessageBox.Show("Proceeding with current transaction.");
        }

        private void ShowRequestResult(QBSessionManager sessionManager, IMsgSetRequest requestMsgSet)
        {
            IMsgSetResponse responseMsgSet;
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);

            // Uncomment the following to view and save the request and response XML
            string requestXML = requestMsgSet.ToXMLString();
            MessageBox.Show(requestXML);
            //SaveXML(requestXML);
            // string responseXML = responseSet.ToXMLString();
            // MessageBox.Show(responseXML);
            // SaveXML(responseXML);

            IResponse response = responseMsgSet.ResponseList.GetAt(0);
            int statusCode = response.StatusCode;
             string statusMessage = response.StatusMessage;
             string statusSeverity = response.StatusSeverity;
             MessageBox.Show("Status:\nCode = " + statusCode + "\nMessage = " + statusMessage + "\nSeverity = " + statusSeverity);

            if (statusCode == 0)
            {
                string resString = null;
                IInvoiceRet invoiceRet = response.Detail as IInvoiceRet;
                resString = resString + "Following invoice has been successfully submitted to QuickBooks:\n\n\n";
                if (invoiceRet.TimeCreated != null)
                    resString = resString + "Time Created = " + Convert.ToString(invoiceRet.TimeCreated.GetValue()) + "\n";
                if (invoiceRet.TxnNumber != null)
                    resString = resString + "Txn Number = " + Convert.ToString(invoiceRet.TxnNumber.GetValue()) + "\n";
                if (invoiceRet.TxnDate != null)
                    resString = resString + "Txn Date = " + Convert.ToString(invoiceRet.TxnDate.GetValue()) + "\n";
                if (invoiceRet.RefNumber != null)
                    resString = resString + "Reference Number = " + invoiceRet.RefNumber.GetValue() + "\n";
                if (invoiceRet.CustomerRef.FullName != null)
                    resString = resString + "Customer FullName = " + invoiceRet.CustomerRef.FullName.GetValue() + "\n";
                resString = resString + "\nBilling Address:" + "\n";
                if (invoiceRet.BillAddress.Addr1 != null)
                    resString = resString + "Addr1 = " + invoiceRet.BillAddress.Addr1.GetValue() + "\n";
                if (invoiceRet.BillAddress.Addr2 != null)
                    resString = resString + "Addr2 = " + invoiceRet.BillAddress.Addr2.GetValue() + "\n";
                if (invoiceRet.BillAddress.Addr3 != null)
                    resString = resString + "Addr3 = " + invoiceRet.BillAddress.Addr3.GetValue() + "\n";
                if (invoiceRet.BillAddress.Addr4 != null)
                    resString = resString + "Addr4 = " + invoiceRet.BillAddress.Addr4.GetValue() + "\n";
                if (invoiceRet.BillAddress.City != null)
                    resString = resString + "City = " + invoiceRet.BillAddress.City.GetValue() + "\n";
                if (invoiceRet.BillAddress.State != null)
                    resString = resString + "State = " + invoiceRet.BillAddress.State.GetValue() + "\n";
                if (invoiceRet.BillAddress.PostalCode != null)
                    resString = resString + "Postal Code = " + invoiceRet.BillAddress.PostalCode.GetValue() + "\n";
                if (invoiceRet.BillAddress.Country != null)
                    resString = resString + "Country = " + invoiceRet.BillAddress.Country.GetValue() + "\n";
                if (invoiceRet.PONumber != null)
                    resString = resString + "\nPO Number = " + invoiceRet.PONumber.GetValue() + "\n";
                if (invoiceRet.TermsRef.FullName != null)
                    resString = resString + "Terms = " + invoiceRet.TermsRef.FullName.GetValue() + "\n";
                if (invoiceRet.DueDate != null)
                    resString = resString + "Due Date = " + Convert.ToString(invoiceRet.DueDate.GetValue()) + "\n";
                if (invoiceRet.SalesTaxTotal != null)
                    resString = resString + "Sales Tax = " + Convert.ToString(invoiceRet.SalesTaxTotal.GetValue()) + "\n";
                resString = resString + "\nInvoice Line Items:" + "\n";
                IORInvoiceLineRetList orInvoiceLineRetList = invoiceRet.ORInvoiceLineRetList;
                string fullname = "<empty>";
                string desc = "<empty>";
                string rate = "<empty>";
                string quantity = "<empty>";
                string amount = "<empty>";
                for (int i = 0; i <= orInvoiceLineRetList.Count - 1; i++)
                {
                    if (invoiceRet.ORInvoiceLineRetList.GetAt(i).ortype == ENORInvoiceLineRet.orilrInvoiceLineRet)
                    {
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ItemRef.FullName != null)
                            fullname = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ItemRef.FullName.GetValue();
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Desc != null)
                            desc = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Desc.GetValue();
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ORRate.Rate != null)
                            rate = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ORRate.Rate.GetValue());
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Quantity != null)
                            quantity = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Quantity.GetValue());
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Amount != null)
                            amount = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Amount.GetValue());
                    }
                    else
                    {
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.ItemGroupRef.FullName != null)
                            fullname = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.ItemGroupRef.FullName.GetValue();
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Desc != null)
                            desc = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Desc.GetValue();
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.InvoiceLineRetList.GetAt(i).ORRate.Rate != null)
                            rate = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.InvoiceLineRetList.GetAt(i).ORRate.Rate.GetValue());
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Quantity != null)
                            quantity = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Quantity.GetValue());
                        if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.TotalAmount != null)
                            amount = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.TotalAmount.GetValue());
                    }
                    resString = resString + "Fullname: " + fullname + "\n";
                    resString = resString + "Description: " + desc + "\n";
                    resString = resString + "Rate: " + rate + "\n";
                    resString = resString + "Quantity: " + quantity + "\n";
                    resString = resString + "Amount: " + amount + "\n\n";
                }
                MessageBox.Show(resString);
            } // if statusCode is zero
        }

    }

}
/*
 {
  "Line": [
    {
      "Amount": 100.00,
      "DetailType": "SalesItemLineDetail",
      "SalesItemLineDetail": {
        "ItemRef": {
          "value": "1",
          "name": "Services"
        }
      }
    }
  ],
  "CustomerRef": {
    "value": "1"
  }

    {
    "BillAddr": {
        "Line1": "123 Main Street",
        "City": "Mountain View",
        "Country": "USA",
        "CountrySubDivisionCode": "CA",
        "PostalCode": "94042"
    },
    "Notes": "Here are other details.",
    "Title": "Mr",
    "GivenName": "James",
    "MiddleName": "B",
    "FamilyName": "King",
    "Suffix": "Jr",
    "FullyQualifiedName": "King Groceries",
    "CompanyName": "King Groceries",
    "DisplayName": "King's Groceries",
    "PrimaryPhone": {
        "FreeFormNumber": "(555) 555-5555"
    },
    "PrimaryEmailAddr": {
        "Address": "jdrew@myemail.com"
    }
}

} 
 */
