using QBFC13Lib;
using System;
using System.Globalization;

namespace SEQB
{
    public class QBHelper
    {
        public static event EventHandler<QBMsgEventArgs> MsgEvent;

        private static void RaiseEvent(string msg)
        {
            MsgEvent?.Invoke(typeof(QBHelper), new QBMsgEventArgs(msg));
        }

        public static void EnableErrorRecovery(QBSessionManager sessionManager)
        {
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

        }

        private static void ProcessQBFCErrors(QBSessionManager sessionManager)
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
                RaiseEvent("The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
            }
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
            {
                RaiseEvent("Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
            }
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
            {
                // Response was not successfully stored or stored properly
                RaiseEvent("No stored response was found.");
            }
            // 9003 = Not used
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
            {
                // MessageSetID is set with a string of size > 24 char
                RaiseEvent("Invalid MessageSetID, greater than 24 character was given.");
            }
            else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
            {
                RaiseEvent("Unable to store response.");
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
                    //RaiseEvent("Last request was processed and Invoice was added successfully!");
                }
                else if (sCode > 0)
                {
                    //RaiseEvent("There was a warning but last request was processed successfully!");
                }
                else
                {
                    RaiseEvent("It seems that there was an error in processing last request");
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
                        string resStr = string.Empty;
                        IInvoiceRet invRet = resp.Detail as IInvoiceRet;
                        resStr = resStr + "Following invoice has been successfully submitted to QuickBooks:\n\n\n";
                        if (invRet?.TxnNumber != null)
                            resStr = resStr + "Txn Number = " + Convert.ToString(invRet.TxnNumber.GetValue()) + "\n";
                    } // if (statusCode == 0)
                } // else (sCode)
            } // else (MessageSetStatusCode)

            // d. Clear the response status, using ClearErrorRecovery
            sessionManager.ClearErrorRecovery();
            //RaiseEvent("Proceeding with current transaction.");
        }

        public static IMsgSetRequest GetLatestMsgSetRequest(QBSessionManager sessionManager)
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
                RaiseEvent("It seems that you are running QuickBooks 2002 Release 1. We strongly recommend that you use QuickBooks' online update feature to obtain the latest fixes and enhancements");
            }

            // Create the message set request object
            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", qbXMLMajorVer, qbXMLMinorVer);
            return requestMsgSet;
        }

        // Code for handling different versions of QuickBooks
        private static double QBFCLatestVersion(QBSessionManager SessionManager)
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

        public static void ShowRequestResult(QBSessionManager sessionManager, IMsgSetRequest requestMsgSet)
        {
            IMsgSetResponse responseMsgSet;
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);

            // Uncomment the following to view and save the request and response XML
            string requestXML = requestMsgSet.ToXMLString();
            RaiseEvent(requestXML);
            //SaveXML(requestXML);
            // string responseXML = responseSet.ToXMLString();
            // MessageBox.Show(responseXML);
            // SaveXML(responseXML);

            IResponse response = responseMsgSet.ResponseList.GetAt(0);
            int statusCode = response.StatusCode;
            string statusMessage = response.StatusMessage;
            string statusSeverity = response.StatusSeverity;
            RaiseEvent("Status:\nCode = " + statusCode + "\nMessage = " + statusMessage + "\nSeverity = " + statusSeverity);

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
                RaiseEvent(resString);
            } // if statusCode is zero
        }
    }

    public class QBMsgEventArgs : EventArgs
    {
        public readonly string Msg;

        public QBMsgEventArgs(string msg)
        {
            Msg = msg;
        }
    }
}
